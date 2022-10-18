namespace Temp.Services.Organizations.CLI.Query;

public class GetInnerGroups
{
    public class Response
    {
        public string Name;
        public IEnumerable<InnerGroupViewModel> Groups;
    }

    public class InnerGroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

