using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Temp.Core.Helpers;
using Temp.Core.Workplaces.Service;
using Temp.Database;

namespace Temp.Core.Workplaces
{
    public class GetWorkplaces : WorkplaceService
    {
        private readonly ApplicationDbContext _ctx;

        public GetWorkplaces(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public Task<IEnumerable<WorkplacesViewModel>> Do() =>
        TryCatch(async () => 
        { 
            var workplaces = await _ctx.Workplaces
                .Select(x =>  new WorkplacesViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();
        
            ValidateStorageWorkplaces(workplaces);        

            return workplaces;
        });

        public Task<PagedList<WorkplacesViewModel>> Do(Request request) =>
        TryCatch(async () => 
        { 
            var workplaces =_ctx.Workplaces
            .Select(x =>  new WorkplacesViewModel
            {
                Id = x.Id,
                Name = x.Name
            }).AsQueryable();
            
            ValidateStorageWorkplaces(workplaces);        

            return await PagedList<WorkplacesViewModel>.CreateAsync(workplaces, request.PageNumber, request.PageSize);
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

        public class WorkplacesViewModel
        {
            public int Id {get; set;}
            public string Name {get; set;}
        }
    }
}
