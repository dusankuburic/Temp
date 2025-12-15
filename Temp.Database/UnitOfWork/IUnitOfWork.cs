using System;
using System.Threading;
using System.Threading.Tasks;
using Temp.Database.Repositories;
using Temp.Domain.Models;
using Temp.Domain.Models.Applications;

namespace Temp.Database.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<Employee> Employees { get; }
    IRepository<Team> Teams { get; }
    IRepository<Organization> Organizations { get; }
    IRepository<Group> Groups { get; }
    IRepository<Engagement> Engagements { get; }
    IRepository<Application> Applications { get; }
    IRepository<Workplace> Workplaces { get; }
    IRepository<EmploymentStatus> EmploymentStatuses { get; }
    IRepository<ModeratorGroup> ModeratorGroups { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
