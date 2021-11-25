
using Common.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MovieAPI.Repositories;
using MovieAPI.Settings;

namespace MovieAPI
{ //in startup noi facem injectarea datelor
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
            services.AddHttpContextAccessor();

            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDbSettings"));
            services.Configure<SyncServiceSettings>(Configuration.GetSection("SyncServiceSettings"));

            //pentru o instanta de MongoDbSettings va fi extrasa valoarea lui
            services.AddSingleton<IMongoDbSettings>(provider =>
                 provider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

            services.AddSingleton<ISyncServiceSettigns>(provider =>
                 provider.GetRequiredService<IOptions<SyncServiceSettings>>().Value);

            services.AddScoped<IMongoRepository<Movie>, MongoRepository<Movie>>();

            services.AddScoped<ISyncService<Movie>, SyncService<Movie>>();// adaugarea serviciului de sincronizare

            services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
            })
                .AddXmlSerializerFormatters()
                .AddXmlDataContractSerializerFormatters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
