using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Temp.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
using Temp.Core.Organizations.Service;

namespace Temp.Core.Organizations
{
    public class GetInnerGroups : OrganizationService
    {
        private readonly ApplicationDbContext _ctx;

        public GetInnerGroups(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<string> Do(int id) =>
            TryCatch(async () =>
            {
                var innerGroups = await _ctx.Organizations
                .Include(x => x.Groups)
                .Where(x => x.Id == id)
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

                return JsonConvert.SerializeObject(innerGroups, Formatting.Indented, new JsonSerializerSettings
                {
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
