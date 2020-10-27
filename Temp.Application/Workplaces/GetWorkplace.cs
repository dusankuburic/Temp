using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.Application.Workplaces
{
    public class GetWorkplace
    {
        private ApplicationDbContext _ctx;

        public GetWorkplace(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public WorkplaceViewModel Do(int id) =>
            _ctx.Workplaces.Where(x => x.Id == id).Select(x => new WorkplaceViewModel{ 
                
            })
            .FirstOrDefault();

        public class WorkplaceViewModel
        {
            public int Id {get; set;}
            public string Name {get; set;}
        }
    }
}
