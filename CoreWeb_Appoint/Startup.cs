using Castle.Windsor;
using CoreBaseClass;
using CoreEntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreWeb_Appoint
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public ILogger Logger;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<App_DbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("default")))
               .AddTransient(typeof(App_DbContext));
            services.AddSingleton<IWindsorContainer, WindsorContainer>();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IncludeFields = true;
            });
            services.AddAutoMapper(typeof(MapperConfigProfile));
            ConstConfig._configuration = Configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            Logger = logger;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Error");
                app.UseExceptionHandler(options => options.Use(ExceptionHandler));
            }
            app.UseExceptionHandler(options => options.Use(ExceptionHandler));

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        async Task ExceptionHandler(HttpContext httpContext, Func<Task> next)
        {
            //该信息由ExceptionHandlerMiddleware中间件提供，里面包含了ExceptionHandlerMiddleware中间件捕获到的异常信息。
            var exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
            var ex = exceptionDetails?.Error;

            if (ex != null)
            {

                var title = "An error occured: " + ex.Message;
                var details = ex.ToString();

                var problem = new
                {
                    Status = 500,
                    Title = title,
                    Detail = details
                };
                var stream = httpContext.Response.Body;
                Logger.LogError(ex,ex.Message);
                await JsonSerializer.SerializeAsync(stream, problem);
            }
        }
    }
}
