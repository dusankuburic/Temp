using System.Collections.Generic;
using System.Linq;
using Temp.Application.Workplaces.Service;
using Temp.Database;

namespace Temp.Application.Workplaces
{
    public class GetWorkplaces : WorkplaceService
    {
        private readonly ApplicationDbContext _ctx;

        public GetWorkplaces(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<WorkplacesViewModel> Do() =>
        TryCatch(() => 
        { 
            var workplaces =_ctx.Workplaces.ToList()
            .Select(x =>  new WorkplacesViewModel
            {
                Id = x.Id,
                Name = x.Name
            });
            
            ValidateStorageWorkplaces(workplaces);        

            return workplaces;
        });



        public class WorkplacesViewModel
        {
            public int Id {get; set;}
            public string Name {get; set;}
        }
    }
}
