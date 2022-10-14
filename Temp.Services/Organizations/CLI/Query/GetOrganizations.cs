using Temp.Database;

namespace Temp.Services.Organizations.CLI.Query;

public class GetOrganizations
{
    private readonly ApplicationDbContext _ctx;

    public GetOrganizations(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<IEnumerable<OrganizationViewModel>> Do() {

        var organizations = await _ctx.Organizations
            .Where(x => x.IsActive)
            .Select(x => new OrganizationViewModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        return organizations;
    }

    public class OrganizationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

