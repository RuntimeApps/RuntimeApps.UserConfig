using Moq;
using RuntimeApps.UserConfig.Services;

namespace RuntimeApps.UserConfig.Test {
    public class UserConfigServiceTests {
        [Fact]
        public async Task GetAsync_WithValidKey_ReturnsConfigFromCache() {
            // Arrange
            var userConfigModel = new UserConfigModel<string>() {
                Key = "validKey",
                UserId = "userId",
                Value = "value",
            };
            var cacheMock = new Mock<IUserConfigCache>();
            cacheMock.Setup(c => c.GetAsync<string>(userConfigModel.Key, userConfigModel.UserId))
                .ReturnsAsync(userConfigModel);
            var userConfigValidationMock = new Mock<IUserConfigValidation>();
            userConfigValidationMock.Setup(kv => kv.ValidateKeyAsync(userConfigModel.Key, ActionType.Get, userConfigModel.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var service = new UserConfigService(
                Mock.Of<IUserConfigStore>(),
                userConfigValidationMock.Object,
                cacheMock.Object
            );

            // Act
            var result = await service.GetAsync<string>(userConfigModel.Key, userConfigModel.UserId);

            // Assert
            Assert.Equal(userConfigModel.Value, result);
            userConfigValidationMock.Verify(kv => kv.ValidateKeyAsync(userConfigModel.Key, ActionType.Get, userConfigModel.UserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WithValidKey_ReturnsConfigFromStoreAndCaches() {
            // Arrange
            var userConfigModel = new UserConfigModel<string>() {
                Key = "validKey",
                UserId = "userId",
                Value = "value",
            };
            var storeMock = new Mock<IUserConfigStore>();
            storeMock.Setup(s => s.GetAsync<string>(userConfigModel.Key, userConfigModel.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userConfigModel);
            var userConfigValidationMock = new Mock<IUserConfigValidation>();
            userConfigValidationMock.Setup(kv => kv.ValidateKeyAsync(userConfigModel.Key, ActionType.Get, userConfigModel.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var cacheMock = new Mock<IUserConfigCache>();
            var service = new UserConfigService(
                storeMock.Object,
                userConfigValidationMock.Object,
                cacheMock.Object
            );

            // Act
            var result = await service.GetAsync<string>(userConfigModel.Key, userConfigModel.UserId);

            // Assert
            Assert.Equal(userConfigModel.Value, result);
            cacheMock.Verify(c => c.SetAsync(It.IsAny<UserConfigModel<string>>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WithInvalidKey_ThrowsKeyNotFoundException() {
            // Arrange
            var key = "invalidKey";
            var userConfigValidationMock = new Mock<IUserConfigValidation>();
            userConfigValidationMock.Setup(kv => kv.ValidateKeyAsync(key, ActionType.Get, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var service = new UserConfigService(
                Mock.Of<IUserConfigStore>(),
                userConfigValidationMock.Object,
                Mock.Of<IUserConfigCache>()
            );

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetAsync<string>(key));
        }

        [Fact]
        public async Task ResetAsync_WithValidKey_ResetsStoreAndCache() {
            // Arrange
            var key = "validKey";
            var userId = "userId";
            var storeMock = new Mock<IUserConfigStore>();
            var cacheMock = new Mock<IUserConfigCache>();
            var userConfigValidationMock = new Mock<IUserConfigValidation>();
            userConfigValidationMock.Setup(kv => kv.ValidateKeyAsync(key, ActionType.Reset, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var service = new UserConfigService(
                storeMock.Object,
                userConfigValidationMock.Object,
                cacheMock.Object
            );

            // Act
            await service.ResetAsync(key, userId);

            // Assert
            storeMock.Verify(s => s.ResetAsync(key, userId, It.IsAny<CancellationToken>()), Times.Once);
            cacheMock.Verify(c => c.RemoveAsync(key, userId), Times.Once);
        }

        [Fact]
        public async Task SetAsync_WithValidConfig_SetsStoreAndInvalidatesCache() {
            // Arrange
            var config = new UserConfigModel<string>() {
                Key = "validKey",
                UserId = "userId",
                Value = "value",
            };
            var storeMock = new Mock<IUserConfigStore>();
            var cacheMock = new Mock<IUserConfigCache>();
            var userConfigValidationMock = new Mock<IUserConfigValidation>();
            userConfigValidationMock.Setup(kv => kv.ValidateKeyAsync(config.Key, ActionType.Set, config.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            userConfigValidationMock.Setup(v => v.ValidateValueAsync<string>(config, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var service = new UserConfigService(
                storeMock.Object,
                userConfigValidationMock.Object,
                cacheMock.Object
            );

            // Act
            await service.SetAsync(config);

            // Assert
            storeMock.Verify(s => s.SetAsync(config, It.IsAny<CancellationToken>()), Times.Once);
            cacheMock.Verify(c => c.RemoveAsync(config.Key, config.UserId), Times.Once);
            userConfigValidationMock.Verify(v => v.ValidateKeyAsync(config.Key, ActionType.Set, config.UserId, It.IsAny<CancellationToken>()), Times.Once);
            userConfigValidationMock.Verify(v => v.ValidateValueAsync<string>(config, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetAsync_WithInvalidValueConfig_ThrowFormatException() {
            // Arrange
            var config = new UserConfigModel<string>() {
                Key = "validKey",
                UserId = "userId",
                Value = "value",
            };
            var storeMock = new Mock<IUserConfigStore>();
            var cacheMock = new Mock<IUserConfigCache>();
            var userConfigValidationMock = new Mock<IUserConfigValidation>();
            userConfigValidationMock.Setup(kv => kv.ValidateKeyAsync(config.Key, ActionType.Set, config.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            userConfigValidationMock.Setup(v => v.ValidateValueAsync<string>(config, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var service = new UserConfigService(
                storeMock.Object,
                userConfigValidationMock.Object,
                cacheMock.Object
            );

            // Act
            await Assert.ThrowsAsync<FormatException>(() => service.SetAsync(config));

            // Assert
            storeMock.Verify(s => s.SetAsync(It.IsAny<UserConfigModel<string>>(), It.IsAny<CancellationToken>()), Times.Never);
            cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            userConfigValidationMock.Verify(v => v.ValidateValueAsync<string>(config, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CheckKey_WithNullOrEmptyKey_ThrowsNullReferenceException() {
            // Arrange
            var service = new UserConfigService(
                Mock.Of<IUserConfigStore>(),
                Mock.Of<IUserConfigValidation>(),
                Mock.Of<IUserConfigCache>()
            );

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => service.GetAsync<string>(string.Empty));
        }

    }
}
