using AdvancedProgrammingASPProject.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using ProjectDBClassLibrary.Model;

public class SeedUsers
{
    public static async Task SyncMainUsersAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<Users>>();
        var mainDb = serviceProvider.GetRequiredService<ProjectDBContext>();

        var mainUsers = mainDb.Users.ToList();

        foreach (var user in mainUsers)
        {
            var identityUser = await userManager.FindByEmailAsync(user.Email);
            if (identityUser == null)
            {
                var newUser = new Users
                {
                    UserName = user.Email,
                    Email = user.Email,
                    FullName = user.Fullname,
                    MainUserId = user.UserId
                };

                newUser.PasswordHash = user.Password; // Use already-hashed password directly



                var result = await userManager.CreateAsync(newUser);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }
            }
        }
    }
}
