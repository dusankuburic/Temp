using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.EmploymentStatuses.Service;
using Temp.Core.Helpers;
using Temp.Database;

namespace Temp.Core.EmploymentStatuses;

public class GetEmploymentStatuses : EmploymentStatusService
{
    private readonly ApplicationDbContext _ctx;

    public GetEmploymentStatuses(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public Task<IEnumerable<EmploymentStatusViewModel>> Do() =>
        TryCatch(async () => {
            var employmentStatuses = await _ctx.EmploymentStatuses
                    .Where(x => x.IsActive)
                    .Select(x => new EmploymentStatusViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToListAsync();

            ValidateEmploymentStatuses(employmentStatuses);

            return employmentStatuses;
        });


    public Task<PagedList<EmploymentStatusViewModel>> Do(Request request) =>
    TryCatch(async () => {
        var employmentStatuses = _ctx.EmploymentStatuses
                .Where(x => x.IsActive)
                .Select(x => new EmploymentStatusViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).AsQueryable();

        ValidateEmploymentStatuses(employmentStatuses);

        return await PagedList<EmploymentStatusViewModel>.CreateAsync(employmentStatuses, request.PageNumber, request.PageSize);
    });

    public class Request
    {
        private const int MaxPageSize = 20;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }
    }

    public class EmploymentStatusViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
