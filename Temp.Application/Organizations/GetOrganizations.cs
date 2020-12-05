using System.Collections.Generic;
using System.Linq;
using Temp.Application.Organizations.Service;
using Temp.Database;

namespace Temp.Application.Organizations
{
    public class GetOrganizations : OrganizationService
    {
        private readonly ApplicationDbContext _ctx;

        public GetOrganizations(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<OrganizationViewModel> Do() =>
        TryCatch(() =>
        {
            var organizations = _ctx.Organizations.ToList()
            .Select(x => new OrganizationViewModel
            {
                Id = x.Id,
                Name = x.Name
            });

            ValidateStorageOrganizations(organizations);

            return organizations;
        });
   
        public class OrganizationViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
