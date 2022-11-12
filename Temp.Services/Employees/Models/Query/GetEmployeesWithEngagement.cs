namespace Temp.Services.Employees.Models.Query;

public class GetEmployeesWithEngagement
{
    public class Request
    {
        private const int MaxPageSize = 20;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        public string Workplace { get; set; }
        public string EmploymentStatus { get; set; }

        public int MinSalary { get; set; } = 0;
        public int MaxSalary { get; set; } = 5000;
    }

    public class EmployeesWithEngagementViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }

        public IEnumerable<string> Workplace { get; set; }
        public IEnumerable<string> EmploymentStatus { get; set; }
        public IEnumerable<int> Salary { get; set; }
    }
}
