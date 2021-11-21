using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.Organizations.Service;
using Temp.Database;

namespace Temp.Core.Organizations
{
    public class GetOrganization : OrganizationService
    {
        private readonly ApplicationDbContext _ctx;

        public GetOrganization(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        public Task<OrganizationViewModel> Do(int id) =>
        TryCatch(async () => {
            var organization = await _ctx.Organizations
                .Where(x => x.Id == id && x.IsActive)
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