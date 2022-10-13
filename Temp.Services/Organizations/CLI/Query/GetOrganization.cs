using Temp.Database;

namespace Temp.Services.Organizations.CLI.Query;

public class GetOrganization
{
    private readonly ApplicationDbContext _ctx;

    public GetOrganization(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<OrganizationViewModel> Do(int id) {
        var organization = await _ctx.Organizations
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new OrganizationViewModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefaultAsync();

        return organization;
    }

    public class OrganizationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

