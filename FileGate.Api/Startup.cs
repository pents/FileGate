using System;
using FileGate.Api.Composition;
using FileGate.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileGate.Api
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
                                 options.AddPolicy("CorsPolicy",
                                     builder =>
                                     {
                                         builder.AllowAnyOrigin();
                                     });
                             });
            
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });
            
            services.AddApplicationDependensies();
            services.AddApplicationOptions(Configuration);
            
            services.AddLogging(options =>
            {
                options.AddConsole().AddConfiguration(Configuration);
            });
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseWebSockets();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCors("CorsPolicy");



            app.UseMvc();
        }
    }
}
