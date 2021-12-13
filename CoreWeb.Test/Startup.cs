using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using CoreApplication;
using CoreBaseClass;
using CoreEntityFramework;
using CoreIocManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreWeb.Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        //public void ConfigureServices(IServiceCollection services)
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddDbContextPool<App_DbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("default")))
                .AddTransient(typeof(App_DbContext));


            services.AddSingleton<IWindsorContainer, WindsorContainer>();

            //EFcore 3.0 以上不支持sqlserver2008，可降低efcore版本以解决
            //services.AddDbContextPool<App_DbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("default")
            //    , s => s.UseRowNumberForPaging()))
            //    .AddTransient(typeof(App_DbContext));

            //services.AddRazorPages();
            services.AddMvc();
            
            services.AddAutoMapper(typeof(MapperConfigProfile));

            //https://github.com/cnblogs/EnyimMemcachedCore
            //memcache
            services.AddEnyimMemcached();

            #if DEBUG
            
                services.AddRazorPages().AddRazorRuntimeCompilation();
            
            #endif

            #region redis
            var redis_section = Configuration.GetSection("Redis:Default");
            string redis_connection_string = redis_section.GetSection("Connection").Value;
            string redis_instance_name = redis_section.GetSection("InstanceName").Value;
            int redis_default_db =int.Parse(redis_section.GetSection("DefaultDB").Value ?? "0");
            services.AddSingleton(new RedisHelper(redis_connection_string, redis_instance_name, redis_default_db));
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Error");
                app.UseExceptionHandler(options=>options.Use(ExceptionHandler));
            }

            app.UseStaticFiles();
            
            app.UseRouting();

            //app.UseAuthorization();

            app.UseEnyimMemcached();


            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
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
                await JsonSerializer.SerializeAsync(stream, problem);
            }
        }
    }
}
