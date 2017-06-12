using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WeekInDotnet.Data;
using WeekInDotnet.Models;
using WeekInDotnet.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using WeekInDotnet.Filters;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WeekInDotnet
{
    public class Startup
    {
        IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services
            // Add session support
                .AddSession()
            // Authentication
                .AddAuthentication(options => options
                    .SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme
                )
            // Add MVC
                .AddMvc(options =>
                {
                    if (_env.IsDevelopment())
                    {
                        options.SslPort = 44384;
                        options.Filters.Add(new RequireHttpsAttribute());
                    }
                    options.Filters.Add(typeof(LoginRequiredFilter));
                });
            // Add simple config strings
            services.Add(new ServiceDescriptor(typeof(IConfiguration), Configuration));
            // Add settings
            services
                .AddOptions()
                .Configure<LinksSettings>(Configuration.GetSection("Links"))
                .Configure<CaptchaSettings>(settings =>
                {
                    settings.PublicKey = Configuration["Recaptcha:Public"];
                    settings.PrivateKey = Configuration["Recaptcha:Secret"];
                });
            // Data context
            var connectionString = Configuration["Database:ConnectionString"];
            services
                .AddDbContext<LinksContext>(options => options
                    .UseSqlServer(connectionString)
                    .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.QueryClientEvaluationWarning)))
            // Register the application's services
                .AddSingleton<LinksService>()
                .AddSingleton<CaptchaService>()
                .AddSingleton<ApiKeyService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            LinksContext linksContext)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app
                .UseSession()
                .UseMvc()
                .UseStaticFiles()
                .UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AutomaticAuthenticate = true,
                    AutomaticChallenge = true,
                    LoginPath = new PathString("/login"),
                    LogoutPath = new PathString("/logout")
                })
                .UseMicrosoftAccountAuthentication(new MicrosoftAccountOptions
                {
                    ClientId = Configuration["Authentication:Microsoft:ClientId"],
                    ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"],
                    AutomaticAuthenticate = true,
                    AutomaticChallenge = true,
                    SaveTokens = true,
                    SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme,
                    Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var admin = await linksContext.FindAsync<Administrator>(
                                context.Ticket.Principal.FindFirst(ClaimTypes.Email).Value);
                            if (admin != null)
                            {
                                context.Identity.AddClaims(admin
                                    .Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(role => new Claim(ClaimTypes.Role, role.Trim(), ClaimValueTypes.String)));
                            }
                        }
                    }
                })
                .Map("/login", builder =>
                {
                    builder.Run(async context =>
                    {
                        await context.Authentication.ChallengeAsync("Microsoft", new AuthenticationProperties
                        {
                            RedirectUri = "/"
                        });
                    });
                })
                .Map("/logout", builder =>
                {
                    builder.Run(async context =>
                    {
                        await context.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        context.Response.Redirect("/");
                    });
                });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            linksContext.Database.EnsureCreated();
        }
    }
}
