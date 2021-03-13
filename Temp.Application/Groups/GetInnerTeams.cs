using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Temp.Database;

namespace Temp.Application.Groups
{
    public class GetInnerTeams
    {
        private readonly ApplicationDbContext _ctx;

        public GetInnerTeams(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public string Do(int id)
        {
            var innerTeams = _ctx.Groups.Include(x => x.Teams)
                .Where(x => x.Id == id)
                .Select(x => new Response
                {
                    Name = x.Name,
                    Teams = x.Teams.Select(t => new InnerTeamViewModel
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
                })
                .FirstOrDefault();

            //Validate

            return JsonConvert.SerializeObject(innerTeams, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

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
