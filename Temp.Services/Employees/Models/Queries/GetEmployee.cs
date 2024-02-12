namespace Temp.Services.Employees.Models.Queries;

public class GetEmployee
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? TeamId { get; set; }
        public string Role { get; set; }
    }
}
