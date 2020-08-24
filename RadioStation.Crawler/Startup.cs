using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RadioStation.Crawler.ConfigurationExtensions;
using RadioStation.Crawler.Database;
using System;

namespace RadioStation.Crawler {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {

      services.AddApplicationConfiguration(Configuration)
        .AddHostedServices()
        //.AddRepositories()
        .AddCoreServices()
        .AddBearerAuthConfiguration(Configuration)
        .AddCors(options => {
          options.AddPolicy("AllowAngularDevClient",
            builder => {
              builder
                .WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        })
        .AddControllers();

      //services.AddControllers();

      services.AddDbContext<CrawlerDbContext>(c => c.UseMySql(Configuration.GetConnectionString("SqliteContext"), o => o.EnableRetryOnFailure().CommandTimeout((int)TimeSpan.FromMinutes(1).TotalSeconds))
                                                    .UseLoggerFactory(LoggerFactory.Create(b => b.AddDebug()))
                                                    );
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      }

      app.UseHttpsRedirection()
      .UseRouting()
      .UseCors("AllowAngularDevClient")
      .UseAuthentication()
      .UseAuthorization()
      .UseEndpoints(endpoints => {
        endpoints.MapControllers();
      });
    }
  }
}
