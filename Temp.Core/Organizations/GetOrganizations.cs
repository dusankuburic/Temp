using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.Organizations.Service;
using Temp.Database;

namespace Temp.Core.Organizations;

public class GetOrganizations : OrganizationService
{
    private readonly ApplicationDbContext _ctx;

    public GetOrganizations(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public Task<IEnumerable<OrganizationViewModel>> Do() =>
    TryCatch(async () => {
        var organizations = await _ctx.Organizations
                .Where(x => x.IsActive)
                .Select(x => new OrganizationViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();

        ValidateStorageOrganizations(organizations);

        return organizations;
    });

    public class OrganizationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
