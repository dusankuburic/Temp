using Microsoft.EntityFrameworkCore.Storage;
using Temp.Database.Repositories;
using Temp.Domain.Models;
using Temp.Domain.Models.Applications;

namespace Temp.Database.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context) {
        _context = context;

        Employees = new Repository<Employee>(context);
        Teams = new Repository<Team>(context);
        Organizations = new Repository<Organization>(context);
        Groups = new Repository<Group>(context);
        Engagements = new Repository<Engagement>(context);
        Applications = new Repository<Application>(context);
        Workplaces = new Repository<Workplace>(context);
        EmploymentStatuses = new Repository<EmploymentStatus>(context);
        ModeratorGroups = new Repository<ModeratorGroup>(context);
    }

    public IRepository<Employee> Employees { get; }
    public IRepository<Team> Teams { get; }
    public IRepository<Organization> Organizations { get; }
    public IRepository<Group> Groups { get; }
    public IRepository<Engagement> Engagements { get; }
    public IRepository<Application> Applications { get; }
    public IRepository<Workplace> Workplaces { get; }
    public IRepository<EmploymentStatus> EmploymentStatuses { get; }
    public IRepository<ModeratorGroup> ModeratorGroups { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default) {
        try {
            await _context.SaveChangesAsync(cancellationToken);

            if (_transaction != null) {
                await _transaction.CommitAsync(cancellationToken);
            }
        } catch {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        } finally {
            if (_transaction != null) {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) {
        if (_transaction != null) {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose() {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync() {
        if (_transaction != null) {
            await _transaction.DisposeAsync();
        }
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}