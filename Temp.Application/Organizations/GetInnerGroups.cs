using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.Organizations
{
    public class GetInnerGroups
    {

        private readonly ApplicationDbContext _ctx;

        public GetInnerGroups(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Response Do(int id)
        {
            //var innerGroups = _ctx.Groups.Where(x => x.OrganizationId == id)
            //    .Select(x => new InnerGruopViewModel
            //    {
            //        Id = x.Id,
            //        Name = x.Name
            //    })
            //    .ToList();

            var innerGroups = _ctx.Organizations.Include(x => x.Groups)
                .Where(x => x.Id == id)
                .Select(x => new Response
                {
                    Name = x.Name,
                    Groups = x.Groups
                })
                .FirstOrDefault();
               
            //Validate

            return innerGroups;
        }

        public class Response
        {
            public string Name;

            public IEnumerable<Group> Groups;
        }

        public class InnerGruopViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }

        }
    }
}
