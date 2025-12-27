using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Temp.Database;
using Temp.Domain.Models;
using Temp.Services._Helpers;

namespace Temp.Tests.Unit.Repositories;

public sealed class PagedListTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public PagedListTests() {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
    }

    public void Dispose() {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateAsync_WithEmptySource_ReturnsEmptyPagedList() {

        var source = Array.Empty<Team>().AsQueryable().BuildMockDbSet().Object;


        var result = await PagedList<Team>.CreateAsync(source, 1, 10);


        result.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.PageSize.Should().Be(10);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task CreateAsync_WithSinglePage_ReturnsAllItems() {

        var teams = Enumerable.Range(1, 5).Select(i => CreateTeam(i)).ToList();
        var source = teams.AsQueryable().BuildMockDbSet().Object;


        var result = await PagedList<Team>.CreateAsync(source, 1, 10);


        result.Should().HaveCount(5);
        result.TotalCount.Should().Be(5);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_WithMultiplePages_ReturnsCorrectPage() {

        var teams = Enumerable.Range(1, 25).Select(i => CreateTeam(i)).ToList();
        var source = teams.AsQueryable().BuildMockDbSet().Object;


        var result = await PagedList<Team>.CreateAsync(source, 2, 10);


        result.Should().HaveCount(10);
        result.TotalCount.Should().Be(25);
        result.CurrentPage.Should().Be(2);
        result.TotalPages.Should().Be(3);
        result.First().Id.Should().Be(11);
    }

    [Fact]
    public async Task CreateAsync_WithLastPage_ReturnsRemainingItems() {

        var teams = Enumerable.Range(1, 25).Select(i => CreateTeam(i)).ToList();
        var source = teams.AsQueryable().BuildMockDbSet().Object;


        var result = await PagedList<Team>.CreateAsync(source, 3, 10);


        result.Should().HaveCount(5);
        result.TotalCount.Should().Be(25);
        result.CurrentPage.Should().Be(3);
        result.TotalPages.Should().Be(3);
        result.First().Id.Should().Be(21);
    }

    [Fact]
    public async Task CreateAsync_WithFirstPage_ReturnsFirstPageItems() {

        var teams = Enumerable.Range(1, 25).Select(i => CreateTeam(i)).ToList();
        var source = teams.AsQueryable().BuildMockDbSet().Object;


        var result = await PagedList<Team>.CreateAsync(source, 1, 10);


        result.Should().HaveCount(10);
        result.CurrentPage.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_WithExactPageSize_CalculatesTotalPagesCorrectly() {

        var teams = Enumerable.Range(1, 20).Select(i => CreateTeam(i)).ToList();
        var source = teams.AsQueryable().BuildMockDbSet().Object;


        var result = await PagedList<Team>.CreateAsync(source, 1, 10);


        result.TotalPages.Should().Be(2);
        result.TotalCount.Should().Be(20);
    }

    [Fact]
    public async Task CreateAsync_WithDatabaseContext_ExecutesCorrectly() {

        var teams = Enumerable.Range(1, 15).Select(i => CreateTeam(i));
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await PagedList<Team>.CreateAsync(
            _context.Teams.AsQueryable(), 1, 5);


        result.Should().HaveCount(5);
        result.TotalCount.Should().Be(15);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task CreateAsync_WithFilter_AppliesFilterBeforePaging() {

        var teams = Enumerable.Range(1, 20).Select(i =>
            CreateTeam(i, isActive: i % 2 == 0)).ToList();
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await PagedList<Team>.CreateAsync(
            _context.Teams.Where(t => t.IsActive), 1, 5);


        result.Should().HaveCount(5);
        result.TotalCount.Should().Be(10);
        result.TotalPages.Should().Be(2);
        result.Should().OnlyContain(t => t.IsActive);
    }

    [Fact]
    public async Task CreateAsync_WithOrdering_MaintainsOrder() {

        var teams = new[] {
            CreateTeam(3, "Charlie"),
            CreateTeam(1, "Alpha"),
            CreateTeam(2, "Bravo")
        };
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await PagedList<Team>.CreateAsync(
            _context.Teams.OrderBy(t => t.Name), 1, 10);


        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Alpha");
        result[1].Name.Should().Be("Bravo");
        result[2].Name.Should().Be("Charlie");
    }





    [Fact]
    public async Task CreateWithTotalCountAsync_WithKnownCount_SkipsCountQuery() {

        var teams = Enumerable.Range(1, 25).Select(i => CreateTeam(i)).ToList();
        var source = teams.AsQueryable().BuildMockDbSet().Object;


        var result = await PagedList<Team>.CreateWithTotalCountAsync(source, 1, 10, 25);


        result.Should().HaveCount(10);
        result.TotalCount.Should().Be(25);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task CreateWithTotalCountAsync_WithZeroTotal_ReturnsEmptyList() {

        var source = Array.Empty<Team>().AsQueryable().BuildMockDbSet().Object;


        var result = await PagedList<Team>.CreateWithTotalCountAsync(source, 1, 10, 0);


        result.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task CreateWithTotalCountAsync_WithSecondPage_PaginatesCorrectly() {

        var teams = Enumerable.Range(1, 30).Select(i => CreateTeam(i)).ToList();
        var source = teams.AsQueryable().BuildMockDbSet().Object;


        var result = await PagedList<Team>.CreateWithTotalCountAsync(source, 2, 10, 30);


        result.Should().HaveCount(10);
        result.First().Id.Should().Be(11);
        result.Last().Id.Should().Be(20);
    }

    [Fact]
    public async Task CreateWithTotalCountAsync_WithDatabaseContext_ExecutesCorrectly() {

        var teams = Enumerable.Range(1, 50).Select(i => CreateTeam(i));
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var totalCount = 50;


        var result = await PagedList<Team>.CreateWithTotalCountAsync(
            _context.Teams.AsQueryable(), 3, 10, totalCount);


        result.Should().HaveCount(10);
        result.TotalCount.Should().Be(50);
        result.CurrentPage.Should().Be(3);
        result.First().Id.Should().Be(21);
    }

    [Fact]
    public async Task CreateWithTotalCountAsync_IsMoreEfficientThanCreateAsync() {

        var teams = Enumerable.Range(1, 100).Select(i => CreateTeam(i));
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();



        var cachedTotalCount = 100;


        var result = await PagedList<Team>.CreateWithTotalCountAsync(
            _context.Teams.AsQueryable(), 1, 10, cachedTotalCount);


        result.TotalCount.Should().Be(cachedTotalCount);
        result.Should().HaveCount(10);
    }





    [Fact]
    public async Task CreateAsync_WithPageBeyondTotal_ReturnsEmptyList() {

        var teams = Enumerable.Range(1, 10).Select(i => CreateTeam(i)).ToList();
        var source = teams.AsQueryable().BuildMockDbSet().Object;


        var result = await PagedList<Team>.CreateAsync(source, 100, 10);


        result.Should().BeEmpty();
        result.TotalCount.Should().Be(10);
        result.CurrentPage.Should().Be(100);
    }

    [Fact]
    public async Task CreateAsync_WithSingleItem_CalculatesPagesCorrectly() {

        var teams = new[] { CreateTeam(1) };
        var source = teams.AsQueryable().BuildMockDbSet().Object;


        var result = await PagedList<Team>.CreateAsync(source, 1, 10);


        result.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_WithLargePageSize_HandlesCorrectly() {

        var teams = Enumerable.Range(1, 5).Select(i => CreateTeam(i)).ToList();
        var source = teams.AsQueryable().BuildMockDbSet().Object;


        var result = await PagedList<Team>.CreateAsync(source, 1, 1000);


        result.Should().HaveCount(5);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_WithComplexProjection_WorksCorrectly() {

        var teams = Enumerable.Range(1, 10).Select(i => CreateTeam(i, $"Team {i}"));
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await PagedList<string>.CreateAsync(
            _context.Teams.Select(t => t.Name), 1, 5);


        result.Should().HaveCount(5);
        result.TotalCount.Should().Be(10);
        result.Should().Contain("Team 1");
    }

    [Fact]
    public async Task PagedList_IsEnumerable() {

        var teams = Enumerable.Range(1, 5).Select(i => CreateTeam(i)).ToList();
        var source = teams.AsQueryable().BuildMockDbSet().Object;


        var result = await PagedList<Team>.CreateAsync(source, 1, 10);


        result.Count().Should().Be(5);
        result.Any(t => t.Id == 1).Should().BeTrue();
        result.Where(t => t.IsActive).Should().HaveCount(5);
    }





    [Fact]
    public async Task CreateAsync_ExecutesCountAndItemsInParallel() {

        var teams = Enumerable.Range(1, 1000).Select(i => CreateTeam(i));
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await PagedList<Team>.CreateAsync(
            _context.Teams.AsQueryable(), 5, 50);


        result.Should().HaveCount(50);
        result.TotalCount.Should().Be(1000);
        result.TotalPages.Should().Be(20);
        result.CurrentPage.Should().Be(5);
        result.First().Id.Should().Be(201);
    }

    [Fact]
    public async Task CreateAsync_WithFilteredQuery_ParallelExecutionCorrect() {

        var teams = Enumerable.Range(1, 100).Select(i =>
            CreateTeam(i, isActive: i <= 50));
        await _context.Teams.AddRangeAsync(teams);
        await _context.SaveChangesAsync();


        var result = await PagedList<Team>.CreateAsync(
            _context.Teams.Where(t => t.IsActive), 2, 10);


        result.Should().HaveCount(10);
        result.TotalCount.Should().Be(50);
        result.TotalPages.Should().Be(5);
        result.Should().OnlyContain(t => t.IsActive);
    }





    private static Team CreateTeam(int id, string? name = null, bool isActive = true) {
        return new Team {
            Id = id,
            Name = name ?? $"Team {id}",
            IsActive = isActive,
            CreatedBy = "test-user",
            UpdatedBy = "test-user"
        };
    }


}