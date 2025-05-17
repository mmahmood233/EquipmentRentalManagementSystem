using Microsoft.AspNetCore.Identity;

namespace AdvancedProgrammingASPProject.Areas.Identity.Data;

public class Users : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public int? MainUserId { get; set; } 
}
