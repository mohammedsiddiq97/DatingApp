using DatingApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DatingApp.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            try
            {
                if (!userManager.Users.Any())
                {
                    var userData = File.ReadAllText("Data/UserSeedData.json");
                    var users = JsonSerializer.Deserialize<List<User>>(userData);
                    var roles = new List<string> { "Member", "Admin", "Moderator", "VIP" };
                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new Role { Name = role });
                        }
                    }

                    // Create users
                    foreach (var user in users)
                    {
                        // Hash password and create user
                        var password = "password"; // consider using a more secure password
                        CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
                        user.PasswordHash = Convert.ToBase64String(passwordHash); // Adjust your User model to include these properties
                        if(user.UserName != "Admin")
                        {
                            await userManager.CreateAsync(user, password);
                            await userManager.AddToRoleAsync(user, "Member");
                        }
                        else
                        {
                            await userManager.CreateAsync(user, password);
                            await userManager.AddToRoleAsync(user, "Admin");
                        }
                       
                    }
                    var admin = await userManager.FindByNameAsync("Admin");
                    await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
                   
                }
            }catch(Exception ex)
            {

            }
           
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
