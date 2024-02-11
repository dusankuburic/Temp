namespace Temp.Services.Engagements.Models.Queries;

public class GetUserEmployeeEngagementsResponse
{
    public string WorkplaceName { get; set; }

    public string EmploymentStatusName { get; set; }

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public int Salary { get; set; }
}
