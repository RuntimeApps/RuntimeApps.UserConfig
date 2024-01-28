using Microsoft.EntityFrameworkCore;
using Moq;

namespace RuntimeApps.UserConfig.EntityFrameworkCore.Test {
    public class UserConfigStoreTests {

        [Fact]
        public async Task GetAsync_WithValidKeyAndUserId_ReturnsUserConfigModel() {
            // Arrange
            var key = "validKey";
            var userId = "userId";
            var dbContext = new TestDbContext();
            var serializerMock = new Mock<IUserConfigValueSerializer>();
            var expectedDbUserConfig = new DbUserConfigModel { Key = key, UserId = userId, Value = "\"Value\"" };
            await dbContext.UserConfigs.AddAsync(expectedDbUserConfig);
            await dbContext.SaveChangesAsync();
            serializerMock.Setup(s => s.DeserializeAsync<string>(expectedDbUserConfig.Value))
                          .ReturnsAsync("Value");

            var store = new UserConfigStore<DbContext>(dbContext, serializerMock.Object);

            // Act
            var result = await store.GetAsync<string>(key, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(key, result?.Key);
            Assert.Equal(userId, result?.UserId);
            Assert.Equal("Value", result?.Value);
        }

        [Fact]
        public async Task GetAsync_WithInvalidKey_ReturnsDefault() {
            // Arrange
            var key = "invalidKey";
            var userId = "userId";
            var dbContext = new TestDbContext();
            var serializerMock = new Mock<IUserConfigValueSerializer>();
            var store = new UserConfigStore<DbContext>(dbContext, serializerMock.Object);

            // Act
            var result = await store.GetAsync<object>(key, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ResetAsync_WithValidKeyAndUserId_RemovesUserConfigFromDb() {
            // Arrange
            var key = "validKey";
            var userId = "userId";
            var dbContext = new TestDbContext();
            var serializerMock = new Mock<IUserConfigValueSerializer>();
            var expectedDbUserConfig = new DbUserConfigModel { Key = key, UserId = userId, Value = "{\"Key\":\"Value\"}" };
            await dbContext.UserConfigs.AddAsync(expectedDbUserConfig);
            await dbContext.SaveChangesAsync();
            var store = new UserConfigStore<DbContext>(dbContext, serializerMock.Object);

            // Act
            await store.ResetAsync(key, userId);

            // Assert
            var exists = dbContext.UserConfigs.Any(u => u.Key == key && u.UserId == userId);
            Assert.False(exists);
        }

        [Fact]
        public async Task SetAsync_WithValidUserConfig_InsertsUserConfigToDb() {
            // Arrange
            var key = "validKey";
            var userId = "userId";
            var serializedValue = "{\"key\":\"Value\"}";
            var dbContext = new TestDbContext();
            var serializerMock = new Mock<IUserConfigValueSerializer>();
            serializerMock.Setup(s => s.SerializeAsync<object>(It.IsAny<object>())).ReturnsAsync(serializedValue);
            var store = new UserConfigStore<DbContext>(dbContext, serializerMock.Object);
            var userConfig = new UserConfigModel<object> { Key = key, UserId = userId, Value = new { Key = "Value" } };

            // Act
            await store.SetAsync(userConfig);

            // Assert
            var dbUserConfigs = await dbContext.UserConfigs.FirstAsync(u => u.Key == key && u.UserId == userId);
            Assert.NotNull(dbUserConfigs);
            Assert.Equal(key, dbUserConfigs.Key);
            Assert.Equal(userId, dbUserConfigs.UserId);
            Assert.Equal(serializedValue, dbUserConfigs.Value);
            serializerMock.Verify(s => s.SerializeAsync(It.IsAny<object?>()), Times.Once);
        }

        [Fact]
        public async Task SetAsync_WithExistingUserConfig_UpdatesUserConfigInDb() {
            // Arrange
            var key = "existingKey";
            var userId = "userId";
            var updatedSeralizedValue = "{\"Key\":\"UpdatedValue\"}";
            var dbContext = new TestDbContext();
            var serializerMock = new Mock<IUserConfigValueSerializer>();
            var existingDbUserConfig = new DbUserConfigModel { Key = key, UserId = userId, Value = "{\"Key\":\"ExistingValue\"}" };
            await dbContext.AddAsync(existingDbUserConfig);
            await dbContext.SaveChangesAsync();
            var store = new UserConfigStore<DbContext>(dbContext, serializerMock.Object);
            var userConfig = new UserConfigModel<object> { Key = key, UserId = userId, Value = new { Key = "UpdatedValue" } };
            serializerMock.Setup(s => s.SerializeAsync(It.IsAny<object?>()))
                          .ReturnsAsync(updatedSeralizedValue);

            // Act
            await store.SetAsync(userConfig);

            // Assert
            var dbUserConfigs = await dbContext.UserConfigs.FirstAsync(u => u.Key == key && u.UserId == userId);
            Assert.NotNull(dbUserConfigs);
            Assert.Equal(updatedSeralizedValue, dbUserConfigs.Value);
            serializerMock.Verify(s => s.SerializeAsync(It.IsAny<object?>()), Times.Once);
        }

    }
}
