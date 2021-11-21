using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Temp.Core.Organizations.Service;
using Temp.Database;

namespace Temp.Core.Organizations
{
    public class GetInnerGroups : OrganizationService
    {
        private readonly ApplicationDbContext _ctx;

        public GetInnerGroups(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        public Task<string> Do(int id) =>
            TryCatch(async () => {
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

                ValidateStorageOrganizationInnerGroups(innerGroups.Groups);

                return JsonConvert.SerializeObject(innerGroups, Formatting.Indented, new JsonSerializerSettings {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            });

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