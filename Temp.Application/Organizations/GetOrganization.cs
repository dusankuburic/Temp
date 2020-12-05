using System.Linq;
using Temp.Database;
using Temp.Application.Organizations.Service;

namespace Temp.Application.Organizations
{
    public class GetOrganization : OrganizationService
    {
        private readonly ApplicationDbContext _ctx;

        public GetOrganization(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public OrganizationViewModel Do(int id) =>
        TryCatch(() =>
        {
            var organization = _ctx.Organizations.Where(x => x.Id == id)
            .Select(x => new OrganizationViewModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefault();

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
