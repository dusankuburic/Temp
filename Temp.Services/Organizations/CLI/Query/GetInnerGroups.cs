using Temp.Database;

namespace Temp.Services.Organizations.CLI.Query;

public class GetInnerGroups
{
    private readonly ApplicationDbContext _ctx;

    public GetInnerGroups(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<Response> Do(int id) {
        var innerGroups = await _ctx.Organizations
            .Include(x => x.Groups)
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new Response
            {
                Name = x.Name,
                Groups = x.Groups.Select(g => new InnerGroupViewModel
                {
                    Id = g.Id,
                    Name = g.Name
                })
            })
            .FirstOrDefaultAsync();

        return innerGroups;
    }

    public class Response
    {
        public string Name;
        public IEnumerable<InnerGroupViewModel> Groups;
    }

    public class InnerGroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

