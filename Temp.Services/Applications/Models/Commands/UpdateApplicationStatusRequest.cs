namespace Temp.Services.Applications.Models.Commands;

public class UpdateApplicationStatusRequest
{
    public int Id { get; set; }
    public int ModeratorId { get; set; }
}