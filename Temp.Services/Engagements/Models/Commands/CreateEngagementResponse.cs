namespace Temp.Services.Engagements.Models.Commands;

public class CreateEngagementResponse
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int WorkplaceId { get; set; }

    public int EmploymentStatusId { get; set; }

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public int Salary { get; set; }
}
