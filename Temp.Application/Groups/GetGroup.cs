using Microsoft.EntityFrameworkCore;
using System.Linq;
using Temp.Application.Groups.Service;
using Temp.Database;

namespace Temp.Application.Groups
{
    public class GetGroup : GroupService
    {
        private readonly ApplicationDbContext _ctx;

        public GetGroup(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public GroupViewModel Do(int id) =>
        TryCatch(() =>
        {
            var group = _ctx.Groups.Include(x => x.Organization)
                .Where(x => x.Id == id)
                .Select(x => new GroupViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    OrganizationId = x.Organization.Id
                })
                .FirstOrDefault();

            ValidateGetGroupViewModel(group);

            return group;
        });
         
        public class GroupViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int OrganizationId { get; set; }
        }        
    }
}
