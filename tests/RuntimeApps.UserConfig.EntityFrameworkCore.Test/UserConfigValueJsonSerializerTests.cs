namespace RuntimeApps.UserConfig.EntityFrameworkCore.Test {
    public class UserConfigValueJsonSerializerTests {
        [Fact]
        public async Task SerializeAsync_WithValidValue_ReturnsSerializedString() {
            // Arrange
            var value = new { Key = "Value" };
            var serializer = new UserConfigValueJsonSerializer();

            // Act
            var result = await serializer.SerializeAsync(value);

            // Assert
            Assert.Equal("{\"Key\":\"Value\"}", result);
        }

        [Fact]
        public async Task SerializeAsync_WithNullValue_ReturnsNull() {
            // Arrange
            var serializer = new UserConfigValueJsonSerializer();

            // Act
            var result = await serializer.SerializeAsync<object>(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeserializeAsync_WithValidJson_ReturnsDeserializedValue() {
            // Arrange
            var json = "{\"Key\":\"Value\"}";
            var serializer = new UserConfigValueJsonSerializer();

            // Act
            var result = await serializer.DeserializeAsync<UserConfigModel<object>>(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Value", result.Key);
        }

        [Fact]
        public async Task DeserializeAsync_WithNullOrEmptyJson_ReturnsDefault() {
            // Arrange
            var serializer = new UserConfigValueJsonSerializer();

            // Act
            var result = await serializer.DeserializeAsync<object>(null);

            // Assert
            Assert.Null(result);
        }
    }
}
