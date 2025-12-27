using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;
using Temp.Services.Caching;

namespace Temp.Tests.Unit.Caching;

public class CacheServiceTests
{
    private readonly Mock<IConnectionMultiplexer> _mockRedis;
    private readonly Mock<IDatabase> _mockDatabase;
    private readonly RedisCacheService _cacheService;

    public CacheServiceTests() {
        _mockRedis = new Mock<IConnectionMultiplexer>();
        _mockDatabase = new Mock<IDatabase>();

        _mockRedis.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);

        _cacheService = new RedisCacheService(_mockRedis.Object);
    }

    [Fact]
    public async Task GetAsync_WithExistingKey_ReturnsValue() {

        var key = "test:key";
        var testData = new TestCacheData { Id = 1, Name = "Test" };
        var serialized = JsonConvert.SerializeObject(testData);

        _mockDatabase.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(new RedisValue(serialized));


        var result = await _cacheService.GetAsync<TestCacheData>(key);


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
    public async Task GetAsync_WithNonExistingKey_ReturnsDefault() {

        var key = "nonexistent:key";

        _mockDatabase.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        var result = await _cacheService.GetAsync<string>(key);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_WithValue_StoresInCache() {
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

        await _cacheService.SetAsync(key, value, expiration);

        _mockDatabase.Verify(x => x.StringSetAsync(
            key,
            It.IsAny<RedisValue>(),
            expiration,
            It.IsAny<bool>(),
            It.IsAny<When>(),
            It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_WithKey_DeletesFromCache() {

        var key = "test:key";

        _mockDatabase.Setup(x => x.KeyDeleteAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        await _cacheService.RemoveAsync(key);

        _mockDatabase.Verify(x => x.KeyDeleteAsync(key, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingKey_ReturnsTrue() {

        var key = "test:key";

        _mockDatabase.Setup(x => x.KeyExistsAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        var result = await _cacheService.ExistsAsync(key);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingKey_ReturnsFalse() {

        var key = "nonexistent:key";

        _mockDatabase.Setup(x => x.KeyExistsAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        var result = await _cacheService.ExistsAsync(key);

        result.Should().BeFalse();
    }

    [Fact]
    public void CacheKeys_User_GeneratesCorrectKey() {

        var email = "test@example.com";

        var key = CacheKeys.User(email);

        key.Should().Be("user:test@example.com");
    }

    [Fact]
    public void CacheKeys_Employee_GeneratesCorrectKey() {

        var id = 123;

        var key = CacheKeys.Employee(id);

        key.Should().Be("employee:123");
    }

    [Fact]
    public void CacheKeys_UserPattern_ReturnsCorrectPattern() {

        var pattern = CacheKeys.UserPattern();

        pattern.Should().Be("user:*");
    }
}