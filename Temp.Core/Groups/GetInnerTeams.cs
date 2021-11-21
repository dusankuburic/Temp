using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Temp.Core.Groups.Service;
using Temp.Database;

namespace Temp.Core.Groups
{
    public class GetInnerTeams : GroupService
    {
        private readonly ApplicationDbContext _ctx;

        public GetInnerTeams(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        public Task<string> Do(int id) =>
        TryCatch(async () => {
            var innerTeams = await _ctx.Groups
                .Include(x => x.Teams)
                .Where(x => x.Id == id && x.IsActive)
                .Select(x => new Response
                {
                    Name = x.Name,
                    Teams = x.Teams.Select(t => new InnerTeamViewModel
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
                })
                .FirstOrDefaultAsync();

            ValidateGetInnerTeamResponse(innerTeams);
            ValidateGetInnerTeamsViewModel(innerTeams.Teams);

            return JsonConvert.SerializeObject(innerTeams, Formatting.Indented, new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        });


        public class Response
        {
            public string Name;
            public IEnumerable<InnerTeamViewModel> Teams;
        }

        public class InnerTeamViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}