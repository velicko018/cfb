using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CFB.Infrastructure.Identity.Models;
using CFB.Infrastructure.Identity.Extensions;
using Gremlin.Net.Driver;
using Microsoft.Extensions.Caching.Memory;
using CFB.Infrastructure.Persistence.CosmosGremlinClient;

namespace CFB.Infrastructure.Persistence.Extensions
{
    public static class IWebHostExtensions
    {
        public static async Task SeedIdentityDateAsync(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                try
                {
                    var roleManager = scope.ServiceProvider.GetService<RoleManager<CosmosIdentityRole>>();
                    var userManager = scope.ServiceProvider.GetService<UserManager<CosmosIdentityUser>>();

                    await AddRoles(roleManager);
                    await AddAdminAccount(userManager);
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                }
            }
        }

        public static async Task SeedCacheDateAsync(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                try
                {
                    var gremlinClient = scope.ServiceProvider.GetService<IGremlinClient>();
                    var memoryCache = scope.ServiceProvider.GetService<IMemoryCache>();
                    var cacheSeeder = new CacheSeeder(gremlinClient, memoryCache);

                    await cacheSeeder.InitData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while migrating or seeding the database.");
                }
            }
        }

        private static async ValueTask AddAdminAccount(UserManager<CosmosIdentityUser> userManager)
        {
            if (await userManager.FindByNameAsync("admin") is null)
            {
                var admin = new CosmosIdentityUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "admin@admin.com",
                    UserName = "admin"
                };

                var identityResult = await userManager.CreateAsync(admin, "@tomiA123");

                if (!identityResult.Succeeded)
                {
                    throw new Exception(identityResult.ToFormatedString());
                }

                identityResult = await userManager.AddToRoleAsync(admin, CosmosIdentityRole.Admin);

                if (!identityResult.Succeeded)
                {
                    await userManager.DeleteAsync(admin);

                    throw new Exception(identityResult.ToFormatedString());
                }
            }
        }

        private static async ValueTask AddRoles(RoleManager<CosmosIdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(CosmosIdentityRole.User))
            {
                var identityResult = await roleManager.CreateAsync(new CosmosIdentityRole { Name = CosmosIdentityRole.User });

                if (!identityResult.Succeeded)
                {
                    throw new Exception(identityResult.ToFormatedString());
                }
            }

            if (!await roleManager.RoleExistsAsync(CosmosIdentityRole.Admin))
            {
                var identityResult = await roleManager.CreateAsync(new CosmosIdentityRole { Name = CosmosIdentityRole.Admin });

                if (!identityResult.Succeeded)
                {
                    throw new Exception(identityResult.ToFormatedString());
                }
            }
        }
    }
}
