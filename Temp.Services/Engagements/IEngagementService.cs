using Temp.Services.Engagements.Models.Commands;
using Temp.Services.Engagements.Models.Queries;

namespace Temp.Services.Engagements;

public interface IEngagementService
{
    Task<CreateEngagementResponse> CreateEngagement(CreateEngagementRequest request);
    Task<List<GetUserEmployeeEngagementsResponse>> GetUserEmployeeEngagements(GetUserEmployeeEngagementsRequest request);
    Task<List<GetEngagementsForEmployeeResponse>> GetEngagementForEmployee(GetEngagementsForEmployeeRequest request);
}
