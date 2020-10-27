using System.Linq;
using Temp.Database;

namespace Temp.Application.Workplaces
{
    public class GetWorkplace
    {
        readonly private ApplicationDbContext _ctx;

        public GetWorkplace(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public WorkplaceViewModel Do(int id) =>
            _ctx.Workplaces.Where(x => x.Id == id).Select(x => new WorkplaceViewModel{ 
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefault();

        public class WorkplaceViewModel
        {
            public int Id {get; set;}
            public string Name {get; set;}
        }
    }
}
