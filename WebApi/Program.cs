using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace WebApi {
  public class Program {
    public static void Main(string[] args) {
      var builder = Host.CreateDefaultBuilder(args)
          .ConfigureAppConfiguration((hostingContext, config) => {
            config.AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
          })
          .UseSerilog((hostingContext, services, loggerConfiguration) => {
            loggerConfiguration
              .ReadFrom.Configuration(hostingContext.Configuration)
              .Enrich.FromLogContext();
          })
          .ConfigureWebHostDefaults(webBuilder => {
            webBuilder.UseStartup<Startup>()
                      .CaptureStartupErrors(true)
                      .UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
          });
      builder.Build().Run();
    }
  }
}
