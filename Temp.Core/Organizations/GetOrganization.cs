using System.Linq;
using Temp.Database;
using Temp.Core.Organizations.Service;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Temp.Core.Organizations
{
    public class GetOrganization : OrganizationService
    {
        private readonly ApplicationDbContext _ctx;

        public GetOrganization(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<OrganizationViewModel> Do(int id) =>
        TryCatch(async() =>
        {
            var organization = await _ctx.Organizations.Where(x => x.Id == id)
            .Select(x => new OrganizationViewModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefaultAsync();

            ValidateGetOrganizationViewModel(organization);

            return organization;
        });

        public class OrganizationViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
