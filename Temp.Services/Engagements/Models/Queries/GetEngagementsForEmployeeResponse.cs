namespace Temp.Services.Engagements.Models.Queries;

public class GetEngagementsForEmployeeResponse
{
    public int Id { get; set; }
    public string WorkplaceName { get; set; }
    public string EmploymentStatusName { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int Salary { get; set; }
}
