using System.Collections.Generic;
using System.Linq;
using Temp.Database;

namespace Temp.Application.Workplaces
{
    public class GetWorkplaces
    {
        private readonly ApplicationDbContext _ctx;

        public GetWorkplaces(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<WorkplacesViewModel> Do() =>
            _ctx.Workplaces.ToList().Select(x =>  new WorkplacesViewModel
            {
                Id = x.Id,
                Name = x.Name
            });


        public class WorkplacesViewModel
        {
            public int Id {get; set;}
            public string Name {get; set;}
        }
    }
}
