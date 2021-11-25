using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SyncNode.Services;
using SyncNode.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncNode
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
               services.Configure<MovieAPISettings>(Configuration.GetSection("MovieAPISettings"));

               //pentru o instanta de MovieAPISettings c=va fi extrasa valoarea lui
               services.AddSingleton<IMovieAPISettings>(provider =>
                    provider.GetRequiredService<IOptions<MovieAPISettings>>().Value);

               services.AddSingleton<SyncWorkJobService>();//am creat o sungutra instanta pentru toata aplicatia de un singur Work job service
               //si este adaugat ca un hosted service deci va rula in background si in background fiecare 30 se ce el v-a porni metoda DoSendWork
               services.AddHostedService(provider => provider.GetService<SyncWorkJobService>());

               services.AddControllers();
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
