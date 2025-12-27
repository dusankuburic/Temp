using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services.Moderators.Service;

namespace Temp.Services.Moderators;

public class UpdateModeratorGroups : ModeratorService
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateModeratorGroups(IUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork;
    }

    public Task<UpdateModeratorGroupsResponse> Do(int id, UpdateModeratorGroupsRequest request) =>
    TryCatch(async () => {
        if (request.Groups.Count() == 0) {
            var mod = await _unitOfWork.ModeratorGroups
                    .FirstOrDefaultAsync(x => x.ModeratorId == id);

            if (mod != null) {
                _unitOfWork.ModeratorGroups.Remove(mod);
            }
        } else {
            var moderatorGroups = await _unitOfWork.ModeratorGroups
                    .FindAsync(x => x.ModeratorId == id);


            ValidateModeratorGroups(moderatorGroups.ToList());

            _unitOfWork.ModeratorGroups.RemoveRange(moderatorGroups);

            foreach (var group in request.Groups) {
                await _unitOfWork.ModeratorGroups.AddAsync(new ModeratorGroup {
                    ModeratorId = id,
                    GroupId = group
                });
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return new UpdateModeratorGroupsResponse {
            Message = $"Groups are assigned",
            Status = true
        };
    });

}


public class UpdateModeratorGroupsRequest
{
    [Required]
    public IEnumerable<int>? Groups { get; set; }
}

public class UpdateModeratorGroupsResponse
{
    public bool Status { get; set; }
    public string? Message { get; set; }
}