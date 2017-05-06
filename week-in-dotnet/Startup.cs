using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WeekInDotnet.Data;
using WeekInDotnet.Models;
using System;
using WeekInDotnet.Services;

namespace WeekInDotnet
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddSession();
            services.AddMvc();
            // Add simple config strings
            services.Add(new ServiceDescriptor(typeof(IConfiguration), Configuration));
            // Add settings
            services
                .AddOptions()
                .Configure<LinksSettings>(Configuration.GetSection("Links"))
                .Configure<CaptchaSettings>(settings =>
                {
                    settings.PublicKey = Configuration["WEEK_IN_NET_CAPTCHA_PUBLIC"];
                    settings.PrivateKey = Configuration["WEEK_IN_NET_CAPTCHA_SECRET"];
                });
            // Data context
            var connectionString = Configuration["WEEK_IN_NET_CONNECTION_STRING"];
            services
                .AddDbContext<LinksContext>(options => options.UseSqlServer(connectionString));
            // Add the application's services
            services
                .AddSingleton<LinksService>()
                .AddSingleton<CaptchaService>();
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
                .UseStaticFiles();

            linksContext.Database.EnsureCreated();
        }
    }
}
