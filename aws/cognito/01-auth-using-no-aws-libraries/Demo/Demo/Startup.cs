using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.ResponseType = Configuration["Authentication:Cognito:ResponseType"];
                //syntax - https://cognito-idp.{region}.amazonaws.com/{userPoolId}/.well-known/openid-configuration
                options.MetadataAddress = Configuration["Authentication:Cognito:MetadataAddress"];
                options.ClientId = Configuration["Authentication:Cognito:ClientId"];
                options.ClientSecret = Configuration["Authentication:Cognito:ClientSecret"];
                options.SaveTokens = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    //translates "cognito:groups" in claims (of id token) to roles to be used in "Authorize" attribute
                    RoleClaimType = "cognito:groups"
                };
                options.Events = new OpenIdConnectEvents()
                {
                    //automatically called, for aws cognito server-side logout
                    OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        context.ProtocolMessage.IssuerAddress =
                           GetAbsoluteUri(Configuration["Authentication:Cognito:EndSessionEndPoint"], Configuration["Authentication:Cognito:Authority"]);
                        context.ProtocolMessage.SetParameter("client_id", Configuration["Authentication:Cognito:ClientId"]);
                        context.ProtocolMessage.SetParameter("logout_uri", Configuration["Authentication:Cognito:LogoutUri"]); //should match with aws cognito signout url for app client
                        return Task.CompletedTask;
                    },
                    //to get hold of tokens
                    OnTokenResponseReceived = context => 
                    {
                        var idToken = context.TokenEndpointResponse.IdToken;
                        var accessToken = context.TokenEndpointResponse.AccessToken;
                        var refreshToken = context.TokenEndpointResponse.RefreshToken;
                        return Task.CompletedTask;
                    }
                };
            });
        }

        private string GetAbsoluteUri(string signoutUri, string authority)
        {
            var signOutUri = new Uri(signoutUri, UriKind.RelativeOrAbsolute);
            var authorityUri = new Uri(authority, UriKind.Absolute);

            var uri = signOutUri.IsAbsoluteUri ? signOutUri : new Uri(authorityUri, signOutUri);
            return uri.AbsoluteUri;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
