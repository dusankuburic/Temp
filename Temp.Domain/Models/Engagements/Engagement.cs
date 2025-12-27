namespace Temp.Domain.Models;

public class Engagement : BaseEntity
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int WorkplaceId { get; set; }
    public Workplace? Workplace { get; set; }

    public int EmploymentStatusId { get; set; }
    public EmploymentStatus? EmploymentStatus { get; set; }

    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }

    public int Salary { get; set; }
}