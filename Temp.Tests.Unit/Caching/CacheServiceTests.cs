using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;
using Temp.Services.Caching;
using Xunit;

namespace Temp.Tests.Unit.Caching;

public class CacheServiceTests
{
    private readonly Mock<IConnectionMultiplexer> _mockRedis;
    private readonly Mock<IDatabase> _mockDatabase;
    private readonly RedisCacheService _cacheService;

    public CacheServiceTests()
    {
        _mockRedis = new Mock<IConnectionMultiplexer>();
        _mockDatabase = new Mock<IDatabase>();

        _mockRedis.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);

        _cacheService = new RedisCacheService(_mockRedis.Object);
    }

    [Fact]
    public async Task GetAsync_WithExistingKey_ReturnsValue()
    {
        // Arrange
        var key = "test:key";
        var testData = new TestCacheData { Id = 1, Name = "Test" };
        var serialized = JsonConvert.SerializeObject(testData);

        _mockDatabase.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(new RedisValue(serialized));

        // Act
        var result = await _cacheService.GetAsync<TestCacheData>(key);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test");
    }

    private class TestCacheData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public async Task GetAsync_WithNonExistingKey_ReturnsDefault()
    {
        // Arrange
        var key = "nonexistent:key";

        _mockDatabase.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_WithValue_StoresInCache()
    {
        // Arrange
        var key = "test:key";
        var value = new { Id = 1, Name = "Test" };
        var expiration = TimeSpan.FromMinutes(30);

        _mockDatabase.Setup(x => x.StringSetAsync(
            key,
            It.IsAny<RedisValue>(),
            expiration,
            It.IsAny<bool>(),
            It.IsAny<When>(),
            It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        await _cacheService.SetAsync(key, value, expiration);

        // Assert
        _mockDatabase.Verify(x => x.StringSetAsync(
            key,
            It.IsAny<RedisValue>(),
            expiration,
            It.IsAny<bool>(),
            It.IsAny<When>(),
            It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_WithKey_DeletesFromCache()
    {
        // Arrange
        var key = "test:key";

        _mockDatabase.Setup(x => x.KeyDeleteAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        await _cacheService.RemoveAsync(key);

        // Assert
        _mockDatabase.Verify(x => x.KeyDeleteAsync(key, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingKey_ReturnsTrue()
    {
        // Arrange
        var key = "test:key";

        _mockDatabase.Setup(x => x.KeyExistsAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await _cacheService.ExistsAsync(key);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingKey_ReturnsFalse()
    {
        // Arrange
        var key = "nonexistent:key";

        _mockDatabase.Setup(x => x.KeyExistsAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        // Act
        var result = await _cacheService.ExistsAsync(key);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CacheKeys_User_GeneratesCorrectKey()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var key = CacheKeys.User(email);

        // Assert
        key.Should().Be("user:test@example.com");
    }

    [Fact]
    public void CacheKeys_Employee_GeneratesCorrectKey()
    {
        // Arrange
        var id = 123;

        // Act
        var key = CacheKeys.Employee(id);

        // Assert
        key.Should().Be("employee:123");
    }

    [Fact]
    public void CacheKeys_UserPattern_ReturnsCorrectPattern()
    {
        // Act
        var pattern = CacheKeys.UserPattern();

        // Assert
        pattern.Should().Be("user:*");
    }
}
