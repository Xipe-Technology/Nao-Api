using System;
using Microsoft.AspNetCore.Identity;
using XipeADNWeb.Entities;

namespace XipeADNWeb.Data
{
    public static class XipeADNDbInitializer
    {
        public static void SeedData(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("XipeRoot").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "XipeRoot"
                };
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("SofomStaff").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "SofomStaff"
                };
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Patron").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Patron"
                };
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Usuario").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Usuario"
                };
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }

        public static void SeedUsers(UserManager<User> userManager)
        {
            if (userManager.FindByNameAsync("root").Result == null)
            {
                User user = new User
                {
                    UserName = "root",
                    Email = "root@xipeadn.com",
                    CreationDate = DateTime.UtcNow,
                    IsDeleted = false
                };
                IdentityResult result = userManager.CreateAsync(user, "Test123!").Result;

                if (result.Succeeded)
                    userManager.AddToRoleAsync(user, "XipeRoot").Wait();
            }
            if (userManager.FindByNameAsync("sofomstaff").Result == null)
            {
                User user = new User
                {
                    UserName = "sofomstaff",
                    Email = "sofomstaff@xipeadn.com",
                    CreationDate = DateTime.UtcNow,
                    IsDeleted = false
                };
                IdentityResult result = userManager.CreateAsync(user, "Test123!").Result;

                if (result.Succeeded)
                    userManager.AddToRoleAsync(user, "SofomStaff").Wait();
            }
            if (userManager.FindByNameAsync("patron").Result == null)
            {
                User user = new User
                {
                    UserName = "patron",
                    Email = "patron@xipeadn.com",
                    CreationDate = DateTime.UtcNow,
                    IsDeleted = false
                };
                IdentityResult result = userManager.CreateAsync(user, "Test123!").Result;

                if (result.Succeeded)
                    userManager.AddToRoleAsync(user, "Patron").Wait();
            }
            if (userManager.FindByNameAsync("Usuario").Result == null)
            {
                User user = new User
                {
                    UserName = "usuario",
                    Email = "usuario@xipeadn.com",
                    CreationDate = DateTime.UtcNow,
                    IsDeleted = false
                };
                IdentityResult result = userManager.CreateAsync(user, "Test123!").Result;

                if (result.Succeeded)
                    userManager.AddToRoleAsync(user, "Usuario").Wait();
            }
        }

    }
}
