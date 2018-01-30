using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Autho_MVA_3.Data;
using System.Security.Claims;

namespace Autho_MVA_3
{
    /* Work directly with the DI object
     * We need to declare the scope
     * Ref: Video "intermideate", part 3, @03:07/27:54
     */

    internal class SampleData
    {
        public static async Task InitializeData(IServiceProvider services, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("SampleData");

            using (var serviceScope =services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var env = serviceScope.ServiceProvider.GetService<IHostingEnvironment>();
                if(!env.IsDevelopment()) { return; }

                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                //create our roles
                var adminTask = roleManager.CreateAsync(
                    new IdentityRole { Name = "Admin" });
                var pwrUserTask = roleManager.CreateAsync(
                    new IdentityRole { Name = "Power User" });
                Task.WaitAll(adminTask, pwrUserTask);
                logger.LogInformation("==> Added Admin and Power User roles");

                var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                //create our default user
                var user = new ApplicationUser
                {
                    Email = "geoff@test.com",
                    UserName = "geoff@test.com"
                };
                await userManager.CreateAsync(user, "Passw0rd!");
                logger.LogInformation($"==> Created user geoff@test.com with Password Passw0rd!");

                await userManager.AddToRoleAsync(user, "Admin");
                logger.LogInformation($"==> User geoff@test.com is now an Admin!");

                await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Country, "Canada"));
                logger.LogInformation($"==> User geoff@test.com is now Canadian Aye!");
            }
        }
    }
}