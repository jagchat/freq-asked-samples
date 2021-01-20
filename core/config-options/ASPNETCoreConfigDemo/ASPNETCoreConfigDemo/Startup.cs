using DataLib.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ASPNETCoreConfigDemo
{
    public class Startup
    {
        public static Type DynType = null; //Just to prove that we can have DI across dyn loaded dlls/classes

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var myKeyValue = Configuration["MyKey"]; //from appsettings.json
            Configuration["MyOptions:MyStartupUrl"] = "http://..."; //add custom setting
            
            ////scenario #1
            //register external (referenced) class to use DI in themselves to access config
            services.AddTransient<DataFactory>(); //accessed in HomeController.Index

            ////scenario #2
            //register external (dynamically loaded) class to use DI in themselves to access config
            //need to copy "DemoLib.dll" manually to executing folder
            //accessed completely using reflection (with no interface)
            var dirPath = System.AppDomain.CurrentDomain.BaseDirectory;
            var asmName1 = "DemoLib.dll";
            var assembly1 = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(System.IO.Path.Combine(dirPath, asmName1));
            DynType = assembly1.GetType("DemoLib.DataFactory"); //only to prove the concept
            services.AddTransient(DynType); //accessed in HomeController.Index

            ////scenario #3
            //register external (dynamically loaded) class to use DI in themselves to access config
            //need to copy "DemoLib?.dll" manually to executing folder
            //accessed using interface 
            var asmName2 = Configuration["MyCustomAsmName"]; // or "DataLib1.dll";
            var typeName2 = Configuration["MyCustomTypeName"]; //or "DataLib1.DataProcess";
            var assembly2 = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(System.IO.Path.Combine(dirPath, asmName2));
            services.AddTransient(typeof(IData), assembly2.GetType(typeName2)); //accessed in HomeController.Index

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> log)
        {
            var myKeyValue = Configuration["MyKey"]; //from appsettings.json
            var myUrl = Configuration["MyOptions:MyStartupUrl"]; //from startup.cs
            var myCustomOption1 = Configuration["MyCustomOption1"]; //from custom config provider
            var myCustomJsonOption1 = Configuration["My2ndLevelOptionSet:SetOption1"]; //from custom json config provider
            log.LogInformation($"Startup.Configure => {myCustomJsonOption1}");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
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
