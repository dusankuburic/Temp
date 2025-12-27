using Microsoft.AspNetCore.Identity;

namespace Temp.Domain.Models.Identity;

public class AppUser : IdentityUser
{
    public string? DisplayName { get; set; }
}