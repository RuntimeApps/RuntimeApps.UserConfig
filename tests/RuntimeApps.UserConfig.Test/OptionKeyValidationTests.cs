using Microsoft.Extensions.Options;
using Moq;
using RuntimeApps.UserConfig.Models;
using RuntimeApps.UserConfig.Services;

namespace RuntimeApps.UserConfig.Test {
    public class OptionKeyValidationTests {
        [Fact]
        public async Task Validate_WhenValidationIsDisabled_ReturnsTrue() {
            // Arrange
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { ValidateKey = false });

            var keyValidation = new OptionKeyValidation(optionsMock.Object);

            // Act
            var result = await keyValidation.Validate("someKey", ActionType.Get);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Validate_WhenKeyIsValid_ReturnsTrue() {
            // Arrange
            var validKey = "validKey";
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { ValidateKey = true, ValidKeys = new[] { validKey } });

            var keyValidation = new OptionKeyValidation(optionsMock.Object);

            // Act
            var result = await keyValidation.Validate(validKey, ActionType.Get);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Validate_WhenKeyIsInvalid_ReturnsFalse() {
            // Arrange
            var validKey = "validKey";
            var invalidKey = "invalidKey";
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { ValidateKey = true, ValidKeys = new[] { validKey } });

            var keyValidation = new OptionKeyValidation(optionsMock.Object);

            // Act
            var result = await keyValidation.Validate(invalidKey, ActionType.Get);

            // Assert
            Assert.False(result);
        }
    }
}
