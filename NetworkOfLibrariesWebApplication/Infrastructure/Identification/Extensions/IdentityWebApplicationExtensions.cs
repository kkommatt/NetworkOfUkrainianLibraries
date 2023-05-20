using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace NetworkOfLibrariesWebApplication.Infrastructure.Identity.Extensions
{
    public static class IdentityWebApplicationExtensions
    {
        private record UserInfo(string Username, string Password)
        {
            public UserInfo()
            : this(string.Empty, string.Empty)
            {
            }
        }
        private static async Task
       AddUserIfNotExistsAsync(UserManager<ApplicationUser> userManager, ILogger logger, string userName, string password, ICollection<string> roles)
        {
            var applicationUser = await userManager.FindByNameAsync(userName);
            if (applicationUser is null)
            {
                applicationUser = new ApplicationUser
                {
                    UserName = userName,
                    PasswordHash = password,
                    Email = userName
                };
                await userManager.CreateAsync(applicationUser, password);
                logger.LogInformation("{username} user added", userName);
            }
            else
            {
                logger.LogInformation("User {username} is already in database", userName); 
            }
            var existingRoles = await userManager.GetRolesAsync(applicationUser);
            foreach (var role in roles.Where(role => !existingRoles.Contains(role)))
            {
                await userManager.AddToRoleAsync(applicationUser, role);
                logger.LogInformation("{username} has {rolename} assigned", userName, role); 
            }
        }
        public static async Task InitializeRolesAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var roleName in RoleNames.All)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole { Name = roleName };
                    await roleManager.CreateAsync(role);
                }
            }
        }
        public static async Task InitializeDefaultUsersAsync(this WebApplication app, IConfiguration? superUserConfiguration, IConfiguration? defaultUsersConfiguration)
        {
            using var scope = app.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var superUserInfo = superUserConfiguration.Get<UserInfo>();
            if (superUserInfo is not null)
            {
                await AddUserIfNotExistsAsync(userManager, app.Logger, superUserInfo.Username, superUserInfo.Password, RoleNames.All);
            }
            if (defaultUsersConfiguration is not null)
            {
                var defaultUserInfo = defaultUsersConfiguration.Get<UserInfo>();
                await AddUserIfNotExistsAsync(userManager, app.Logger, defaultUserInfo.Username, defaultUserInfo.Password, RoleNames.All);
            }
        }
    }

        public static class RoleNames
        {
            public const string Admin = "Admin";
            public const string User = "User";

            public static ICollection<string> All => new[] { Admin, User };
        }
}
