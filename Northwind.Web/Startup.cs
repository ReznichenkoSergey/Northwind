using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Northwind.Database;
using Northwind.Web.Infrastructure.Extensions;
using Northwind.Web.Infrastructure.Filters;
using Serilog;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Northwind.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            services.ConfigureOptionModels(Configuration);
            services.AddCors();

            services.AddDbContext<NorthwindContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddResponseCaching();
            services.AddRazorPages()
                .AddMvcOptions(options =>
                {
                    options.MaxModelValidationErrors = 50;
                    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "The field is required.");
                });

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Northwind API", Version = "v1" });
            //});

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                // Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-3.1
                options.HandleSameSiteCookieCompatibility();
            });

            // Sign-in users with the Microsoft identity platform
            //services.AddMicrosoftIdentityWebAppAuthentication(Configuration);

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ActionFilter>();

                var cacheProfile = Configuration
                    .GetSection("CacheProfiles")
                    .GetChildren()
                    .First(x => x.Key == "CustomCache");
                options.CacheProfiles
                        .Add(cacheProfile.Key, cacheProfile.Get<CacheProfile>());
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddMicrosoftIdentityUI()
            .AddRazorRuntimeCompilation();

            var builder = services.AddControllersWithViews();
            if (Env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }

            //configuration reading (Additional information: current configuration values)
            Log.Logger.Information((Configuration as IConfigurationRoot).GetDebugView());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //application startup (Additional information: application location - folder path)
            Log.Logger.Information($"EnvironmentName= {env.EnvironmentName}, ContentRootPath= {env.ContentRootPath}, WebRootPath= {env.WebRootPath}");

            if (!env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Northwind API V1");
                });

                //app.UseExceptionHandler("/Home/Error");
                app.UseExceptionHandler(exceptionHandlerApp =>
                {
                    exceptionHandlerApp.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = Text.Plain;
                        await context.Response.WriteAsync("An exception was thrown.");

                        var exceptionHandlerPathFeature =
                            context.Features.Get<IExceptionHandlerPathFeature>();

                        Log.Error($"Error, Source= {exceptionHandlerPathFeature.Error?.Source}, message= {exceptionHandlerPathFeature.Error?.Message}");

                        if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
                        {
                            await context.Response.WriteAsync(" The file was not found.");
                        }

                        if (exceptionHandlerPathFeature?.Path == "/")
                        {
                            await context.Response.WriteAsync(" Page: Home.");
                        }
                    });
                });
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseResponseCaching();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
