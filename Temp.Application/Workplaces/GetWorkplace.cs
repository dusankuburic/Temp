using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Temp.Application.Workplaces.Service;
using Temp.Database;

namespace Temp.Application.Workplaces
{
    public class GetWorkplace : WorkplaceService
    {
        readonly private ApplicationDbContext _ctx;

        public GetWorkplace(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<WorkplaceViewModel> Do(int id) =>
        TryCatch(async () => 
        { 
             var workplace = await _ctx.Workplaces.Where(x => x.Id == id)
            .Select(x => new WorkplaceViewModel
            { 
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefaultAsync();

            ValidateGetWorkplaceViewModel(workplace);

            return workplace;        
        });

        public class WorkplaceViewModel
        {
            public int Id {get; set;}
            public string Name {get; set;}
        }
    }
}
