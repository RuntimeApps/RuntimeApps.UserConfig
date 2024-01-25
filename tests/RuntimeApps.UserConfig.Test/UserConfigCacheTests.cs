using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using RuntimeApps.UserConfig.Models;
using RuntimeApps.UserConfig.Services;

namespace RuntimeApps.UserConfig.Test {
    public class UserConfigCacheTests {
        [Fact]
        public async Task GetAsync_WhenUseCacheIsFalse_ReturnsNull() {
            // Arrange
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { UseCache = false });

            var cache = new UserConfigCache(optionsMock.Object);

            // Act
            var result = await cache.GetAsync<string>("someKey", "userId");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_WhenCacheContainsValue_ReturnsCachedValue() {
            // Arrange
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { UseCache = true, CachePrefix = "CachePrefix" });

            var memoryCacheMock = new Mock<IMemoryCache>();
            object cachedValue = new UserConfigModel<string> { Key = "someKey", UserId = "userId", Value = "cachedValue" };
            memoryCacheMock.Setup(x => x.TryGetValue("CachePrefix_someKey_userId", out cachedValue!))
                           .Returns(true);

            var cache = new UserConfigCache(optionsMock.Object, memoryCacheMock.Object);

            // Act
            var result = await cache.GetAsync<string>("someKey", "userId");

            // Assert
            Assert.Equal("cachedValue", result?.Value);
        }

        [Fact]
        public async Task GetAsync_WhenCacheNotContainsValue_ReturnsCachedValue() {
            // Arrange
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { UseCache = true, CachePrefix = "CachePrefix" });

            var memoryCacheMock = new Mock<IMemoryCache>();
            object cachedValue = new UserConfigModel<string> { Key = "someKey", UserId = "userId", Value = "cachedValue" };
            memoryCacheMock.Setup(x => x.TryGetValue("CachePrefix_someKey_userId", out cachedValue!))
                           .Returns(false);

            var cache = new UserConfigCache(optionsMock.Object, memoryCacheMock.Object);

            // Act
            var result = await cache.GetAsync<string>("someKey", "userId");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveAsync_WhenUseCacheIsFalse_DoesNotCallCacheMethod() {
            // Arrange
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { UseCache = false });

            var memoryCacheMock = new Mock<IMemoryCache>();

            var cache = new UserConfigCache(optionsMock.Object, memoryCacheMock.Object);

            // Act
            await cache.RemoveAsync("someKey", "userId");

            // Assert
            memoryCacheMock.Verify(c => c.Remove(It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task RemoveAsync_WhenUseCacheIsTrue_CallCacheMethod() {
            // Arrange
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { UseCache = true, CachePrefix = "CachePrefix" });

            var memoryCacheMock = new Mock<IMemoryCache>();

            var cache = new UserConfigCache(optionsMock.Object, memoryCacheMock.Object);

            // Act
            await cache.RemoveAsync("someKey", "userId");

            // Assert
            memoryCacheMock.Verify(c => c.Remove(It.Is<string>(k => k == "CachePrefix_someKey_userId")), Times.Once);
        }

        [Fact]
        public async Task SetAsync_WhenUseCacheIsFalse_DoesNotCallCacheMethod() {
            // Arrange
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { UseCache = false });

            var memoryCacheMock = new Mock<IMemoryCache>();

            var cache = new UserConfigCache(optionsMock.Object, memoryCacheMock.Object);

            // Act
            await cache.SetAsync(new UserConfigModel<string>());

            // Assert
            memoryCacheMock.Verify(c => c.CreateEntry(It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task SetAsync_WhenUseCacheIsFalse_CallCacheMethod() {
            // Arrange
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { UseCache = true, CachePrefix = "CachePrefix" });

            var memoryCacheMock = new Mock<IMemoryCache>();
            var cachEntryMock = new Mock<ICacheEntry>();
            memoryCacheMock
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(cachEntryMock.Object);

            var cache = new UserConfigCache(optionsMock.Object, memoryCacheMock.Object);
            var cachedValue = new UserConfigModel<string> { Key = "someKey", UserId = "userId", Value = "cachedValue" };

            // Act
            await cache.SetAsync(cachedValue);

            // Assert
            memoryCacheMock.Verify(c => c.CreateEntry(It.Is<string>(k=>k == "CachePrefix_someKey_userId")), Times.Once);
            cachEntryMock.VerifySet(v=>v.Value = cachedValue);
        }

        [Fact]
        public void Constructor_WhenUseCacheIsTrueAndMemoryCacheIsNull_ThrowsException() {
            // Arrange
            var optionsMock = new Mock<IOptions<UserConfigOption>>();
            optionsMock.Setup(x => x.Value).Returns(new UserConfigOption { UseCache = true });

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => new UserConfigCache(optionsMock.Object, null));
        }
    }
}
