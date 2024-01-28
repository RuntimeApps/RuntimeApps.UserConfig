using Microsoft.Extensions.Options;
using Moq;
using RuntimeApps.UserConfig.Services;

namespace RuntimeApps.UserConfig.Test {
    public class OptionUserConfigValidationTests {
        [Fact]
        public async Task ValidateKey_WhenValidationIsDisabled_ReturnsTrue() {
            // Arrange
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { ValidateKey = false });

            var keyValidation = new OptionUserConfigValidation(optionsMock.Object);

            // Act
            var result = await keyValidation.ValidateKeyAsync("someKey", ActionType.Get);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateKet_WhenKeyIsValid_ReturnsTrue() {
            // Arrange
            var validKey = "validKey";
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { ValidateKey = true, ValidKeys = new[] { validKey } });

            var keyValidation = new OptionUserConfigValidation(optionsMock.Object);

            // Act
            var result = await keyValidation.ValidateKeyAsync(validKey, ActionType.Get);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateKey_WhenKeyIsInvalid_ReturnsFalse() {
            // Arrange
            var validKey = "validKey";
            var invalidKey = "invalidKey";
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { ValidateKey = true, ValidKeys = new[] { validKey } });

            var keyValidation = new OptionUserConfigValidation(optionsMock.Object);

            // Act
            var result = await keyValidation.ValidateKeyAsync(invalidKey, ActionType.Get);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateKey_WhenKeyIsReadonlyAndActionGet_ReturnsTrue() {
            // Arrange
            var validReadonlyKey = "validReadonlyKey";
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { ValidateKey = true, ValidReadonlyKeys = new[] { validReadonlyKey } });

            var keyValidation = new OptionUserConfigValidation(optionsMock.Object);

            // Act
            var result = await keyValidation.ValidateKeyAsync(validReadonlyKey, ActionType.Get);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateKey_WhenKeyIsReadonlyAndActionSet_ReturnsFalse() {
            // Arrange
            var validReadonlyKey = "validReadonlyKey";
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { ValidateKey = true, ValidReadonlyKeys = new[] { validReadonlyKey } });

            var keyValidation = new OptionUserConfigValidation(optionsMock.Object);

            // Act
            var result = await keyValidation.ValidateKeyAsync(validReadonlyKey, ActionType.Set);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateKey_WhenKeyIsReadonlyAndActionReset_ReturnsFalse() {
            // Arrange
            var validReadonlyKey = "validReadonlyKey";
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { ValidateKey = true, ValidReadonlyKeys = new[] { validReadonlyKey } });

            var keyValidation = new OptionUserConfigValidation(optionsMock.Object);

            // Act
            var result = await keyValidation.ValidateKeyAsync(validReadonlyKey, ActionType.Reset);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateKey_WhenKeyIsDefaultAndActionGet_ReturnsTrue() {
            // Arrange
            var validDefaultKey = "validDefaultKey";
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { ValidateKey = true, ValidDefaultKeus = new[] { validDefaultKey } });

            var keyValidation = new OptionUserConfigValidation(optionsMock.Object);

            // Act
            var result = await keyValidation.ValidateKeyAsync(validDefaultKey, ActionType.Get);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateKey_WhenKeyIsDefaultAndActionSetDefault_ReturnsTrue() {
            // Arrange
            var validDefaultKey = "validDefaultKey";
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { ValidateKey = true, ValidDefaultKeus = new[] { validDefaultKey } });

            var keyValidation = new OptionUserConfigValidation(optionsMock.Object);

            // Act
            var result = await keyValidation.ValidateKeyAsync(validDefaultKey, ActionType.Set);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateKey_WhenKeyIsDefaultAndActionSetByUser_ReturnsTrue() {
            // Arrange
            var validDefaultKey = "validDefaultKey";
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { ValidateKey = true, ValidDefaultKeus = new[] { validDefaultKey } });

            var keyValidation = new OptionUserConfigValidation(optionsMock.Object);

            // Act
            var result = await keyValidation.ValidateKeyAsync(validDefaultKey, ActionType.Set, "userId");

            // Assert
            Assert.False(result);
        }
    }
}
