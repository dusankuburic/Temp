﻿using Temp.Core.Groups.Service;
using Temp.Database;

namespace Temp.Core.Groups;

public class GetGroup : GroupService
{
    private readonly ApplicationDbContext _ctx;

    public GetGroup(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public Task<GroupViewModel> Do(int id) =>
    TryCatch(async () => {
        var group = await _ctx.Groups.Include(x => x.Organization)
                .Where(x => x.Id == id && x.IsActive)
                .Select(x => new GroupViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    OrganizationId = x.Organization.Id
                })
                .FirstOrDefaultAsync();

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
