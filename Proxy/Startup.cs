using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocelot.Cache;

namespace Proxy
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var ocelotBuilder = services.AddOcelot(); // adaugam ocelot
            
            ocelotBuilder.AddCacheManager(x =>
            {
                x.WithDictionaryHandle();
            }); // Ocelot Caching

            ocelotBuilder.AddGracefullLoadBalancer();

            ocelotBuilder.Services.RemoveAll(typeof(ICacheKeyGenerator));
            ocelotBuilder.Services.AddSingleton<ICacheKeyGenerator, CustomCacheKeyGenerator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Smart Proxy");
                });
            });

            app.UseOcelot().Wait(); // am adaugat ocelot-ul
        }
    }
}
