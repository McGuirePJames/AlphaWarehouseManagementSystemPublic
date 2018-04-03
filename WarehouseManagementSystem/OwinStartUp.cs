using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Owin;
using System.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Hangfire;





[assembly: OwinStartup(typeof(WarehouseManagementSystem.OwinStartUp))]

namespace WarehouseManagementSystem
{
    public class OwinStartUp
    {

        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(Models.Database.GetConnectionString());

            app.UseHangfireDashboard();
            app.UseHangfireServer();


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

            app.CreatePerOwinContext(() =>
                new IdentityDbContext(connectionString));
            app.CreatePerOwinContext<UserStore<IdentityUser>>(
                (opt, cont) => new UserStore<IdentityUser>(cont.Get<IdentityDbContext>()));
            app.CreatePerOwinContext<UserManager<IdentityUser>>(
                (opt, cont) =>
                {
                    UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(cont.Get<UserStore<IdentityUser>>());
                    userManager.RegisterTwoFactorProvider("SMS", new PhoneNumberTokenProvider<IdentityUser> { MessageFormat = "Token: {0}" });
                    userManager.UserTokenProvider = new DataProtectorTokenProvider<IdentityUser>(opt.DataProtectionProvider.Create());


                    userManager.EmailService = new WarehouseManagementSystem.Models.EmailService();

                    userManager.UserValidator = new UserValidator<IdentityUser>(userManager) { RequireUniqueEmail = true };
                    userManager.PasswordValidator = new PasswordValidator
                    {
                        RequireDigit = true,
                        RequiredLength = 8
                    };
                    userManager.UserLockoutEnabledByDefault = true;
                    userManager.MaxFailedAccessAttemptsBeforeLockout = 5;
                    userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(3);
                    return userManager;
                });

            app.CreatePerOwinContext<SignInManager<IdentityUser, string>>(
                (opt, cont) => new SignInManager<IdentityUser, string>(cont.Get<UserManager<IdentityUser>>(), cont.Authentication));
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                //ExpireTimeSpan = TimeSpan.FromMinutes(5),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/User/Login"),
                SlidingExpiration = true //if user has no communication with server                               
            });

        }
    }
}
