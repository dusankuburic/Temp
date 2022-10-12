using Temp.Core.Auth.Moderators.Service;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Core.Auth.Moderators;

public class UpdateModeratorGroups : ModeratorService
{
    private readonly ApplicationDbContext _ctx;

    public UpdateModeratorGroups(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public Task<Response> Do(int id, Request request) =>
    TryCatch(async () => {
        if (request.Groups.Count() == 0) {
            var mod = await _ctx.ModeratorGroups
                    .Where(x => x.ModeratorId == id)
                    .FirstOrDefaultAsync();

            _ctx.Remove(mod);
        } else {
            var moderatorGroups = await _ctx.ModeratorGroups
                    .Where(x => x.ModeratorId == id)
                    .ToListAsync();


            ValidateModeratorGroups(moderatorGroups);

            _ctx.RemoveRange(moderatorGroups);

            foreach (var group in request.Groups) {
                _ctx.ModeratorGroups.Add(new ModeratorGroup {
                    ModeratorId = id,
                    GroupId = group
                });
            }
        }

        await _ctx.SaveChangesAsync();

        return new Response {
            Message = $"Groups are assigned",
            Status = true
        };
    });


    public class Request
    {
        [Required]
        public IEnumerable<int> Groups { get; set; }
    }

    public class Response
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}
