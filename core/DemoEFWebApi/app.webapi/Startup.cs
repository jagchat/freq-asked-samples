using app.data;
using app.webapi.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("SampleOrigin",
                    builder => builder.AllowAnyOrigin()
                                        .AllowAnyHeader()
                                        .AllowAnyMethod());
            });

            services.AddControllers();

            //-------
            //NOTE: trying to inject without adding references to "EntityFramework" packages to this project
            //--------

            ////method 1
            //services.AddScoped<EmployeeRepository>(provider =>
            //{
            //    var dbContext = new SampleDbContext(
            //        Configuration.GetConnectionString("SampleDb"),
            //        provider.GetService<ILoggerFactory>(),
            //        provider.GetService<ILogger<SampleDbContext>>()
            //        );
            //    var rep = new EmployeeRepository(provider.GetService<ILogger<EmployeeRepository>>(), dbContext);
            //    return rep;
            //});

            ////method 2
            services.AddScoped<EmployeeRepository>(); //check NOTE in EmployeeRepository to use this
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> log)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("SampleOrigin");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
