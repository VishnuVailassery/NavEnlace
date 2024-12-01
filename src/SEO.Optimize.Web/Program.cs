using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SEO.Optimize.Web.Services;
using Microsoft.EntityFrameworkCore;
using SEO.Optimize.Postgres.Context;
using SEO.Optimize.Postgres.Repository;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using SEO.Optimize.Jobs.Workers;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Handlers;
using SEO.Optimize.Jobs.Handlers;
using SEO.Optimize.Core.Clients;

namespace SEO.Optimize.Web
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var configuration = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory())
            //.AddJsonFile("appsettings.json")
            //.Build();

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<AppStateCache>();
            builder.Services.AddHostedService<CrawlWorker>();

            builder.Services.AddScoped<WebflowClient>();
            builder.Services.AddScoped<GscClient>();
            builder.Services.AddScoped<IPageCrawlModelHandler, GitHubModelCrawlHandler>();
            builder.Services.AddScoped<ICrawlJobHandler, CrawlJobHandler>();
            builder.Services.AddScoped<AuthenticationHandlerService>();
            builder.Services.AddScoped<TokenManagementService>();
            builder.Services.AddScoped<SiteManagementService>();
            builder.Services.AddScoped<DashboardService>();
            builder.Services.AddScoped<GSCService>();
            builder.Services.AddScoped<ActionHandlerService>();
            builder.Services.AddScoped<JobService>();
            builder.Services.AddScoped<IntegrationService>();
            builder.Services.AddScoped<IContentRepository,PostgresContentRepository>();
            builder.Services.AddDbContext<DataContext>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Home/Login"; // Redirect to this path if not authenticated
                options.AccessDeniedPath = "/Home/AccessDenied"; // Optional: Redirect for denied access
            })
            .AddGoogle(options =>
            {
                options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                options.SaveTokens = true;

                options.Scope.Add("profile");
                options.Scope.Add("email");
            })
            .AddOpenIdConnect(options =>
            {
                options.Authority = "https://accounts.google.com"; // The URL of the OIDC provider
                options.ResponseType = "code"; // Use "code" for authorization code flow

                options.SaveTokens = true; // Save access and refresh tokens in the authentication cookie
                options.CallbackPath = "/signin-oidc";

                options.Scope.Add("openid"); // Add the openid scope
                options.Scope.Add("profile"); // Add the profile scope
                options.Scope.Add("email"); // Add the email scope
                options.MetadataAddress = "https://accounts.google.com/.well-known/openid-configuration";
                options.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = context =>
                    {
                        var idToken = context.SecurityToken as JwtSecurityToken;
                        if (idToken != null)
                        {
                            var userId = idToken.Subject;
                            var email = idToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                            var iss = idToken.Claims.FirstOrDefault(c => c.Type == "iss")?.Value;

                            // You can add custom logic here, like adding extra claims
                            var identity = context?.Principal?.Identity as ClaimsIdentity;
                            if (identity != null)
                            {
                                identity?.AddClaim(new Claim("sub", userId));
                                identity?.AddClaim(new Claim("email", email));
                                identity?.AddClaim(new Claim("iss", iss));

                                identity?.AddClaim(new Claim("seopt_session", Guid.NewGuid().ToString()));
                            }
                        }

                        context.Success();
                        return Task.CompletedTask;
                    },

                    OnRemoteFailure = context =>
                    {
                        context.Response.Redirect("/Home/Error"); // Handle errors
                        context.HandleResponse(); // Suppress the default error response
                        return Task.CompletedTask;
                    },
                    
                };
            });

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();

            app.MapControllers();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            try
            {
                app.Run();
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}