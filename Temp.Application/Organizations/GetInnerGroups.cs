using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Temp.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

namespace Temp.Application.Organizations
{
    public class GetInnerGroups
    {
        private readonly ApplicationDbContext _ctx;

        public GetInnerGroups(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<string> Do(int id)
        {
            var innerGroups = await _ctx.Organizations.Include(x => x.Groups)
                .Where(x => x.Id == id)
                .Select(x => new Response
                {
                    Name = x.Name,
                    Groups = x.Groups.Select(g => new InnerGroupViewModel
                    {
                        Id = g.Id,
                        Name =  g.Name
                    })
                })
                .FirstOrDefaultAsync();
               
            //Validate

            return JsonConvert.SerializeObject(innerGroups, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
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
}
