namespace Temp.Services.Organizations.Models.Query;

public class GetInnerGroups
{
    public class Response
    {
        public string Name { get; set; }
        public IEnumerable<InnerGroupViewModel> Groups { get; set; }
    }

    public class InnerGroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

