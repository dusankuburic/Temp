namespace Temp.Services.Applications.Models.Commands;

public class CreateApplicationRequest
{
    public int UserId { get; set; }

    public int TeamId { get; set; }

    public string Content { get; set; }

    public string Category { get; set; }
}
