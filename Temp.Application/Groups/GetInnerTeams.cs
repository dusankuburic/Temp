using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.Groups
{
    public class GetInnerTeams
    {
        public readonly ApplicationDbContext _ctx;

        public GetInnerTeams(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Response Do(int id)
        {
            var innerTeams = _ctx.Groups.Include(x => x.Teams)
                .Where(x => x.Id == id)
                .Select(x => new Response
                {
                    Name = x.Name,
                    Teams = x.Teams
                })
                .FirstOrDefault();

            //Validate

            return innerTeams;
        }

        public class Response
        {
            public string Name;

            public IEnumerable<Team> Teams;
        }

        public class InnerTeamViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
