namespace Temp.Services.Caching;

public static class CacheKeys
{
    public const string UserPrefix = "user:";
    public const string EmployeePrefix = "employee:";
    public const string TeamPrefix = "team:";
    public const string OrganizationPrefix = "organization:";

    public static string User(string email) => $"{UserPrefix}{email}";
    public static string Employee(int id) => $"{EmployeePrefix}{id}";
    public static string Team(int id) => $"{TeamPrefix}{id}";
    public static string Organization(int id) => $"{OrganizationPrefix}{id}";

    public static string UserPattern() => $"{UserPrefix}*";
    public static string EmployeePattern() => $"{EmployeePrefix}*";
    public static string TeamPattern() => $"{TeamPrefix}*";
    public static string OrganizationPattern() => $"{OrganizationPrefix}*";
}