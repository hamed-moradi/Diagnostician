using Application;
using Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model.Common;
using System.IdentityModel.Tokens.Jwt;

namespace WebApi {
  public class Startup(IConfiguration configuration) {
    public void ConfigureServices(IServiceCollection services) {
      var appSettings = new AppSetting();
      configuration.Bind(appSettings);
      services.AddSingleton(sp => appSettings);

      services.AddScoped<JwtSecurityTokenHandler>();
      services.AddScoped<JWTHandler>();
      services.AddSingleton<ConnectionPool>();
      services.AddScoped<Cryptographer>();
      services.AddScoped<ParameterHelper>();
      services.AddScoped<HarmfulKeywordDetector>();
      services.AddSingleton<JsonCompares>();
      services.AddScoped<ScenarioService>();
      services.AddScoped<RequestService>();
      services.AddScoped<ExpectationService>();
      services.AddScoped<RunService>();
      services.AddScoped<TestService>();
      services.AddScoped<DiagnoseService>();
      services.AddSingleton(new ServiceLocator(services));

      services.AddMemoryCache();
      services.AddMvc().AddRazorRuntimeCompilation();
      services.AddControllers(
        configure => {
          configure.AllowEmptyInputInBodyModelBinding = true;
        })
        .AddControllersAsServices();

      services.AddCors(options => {
        options.AddDefaultPolicy(policy => {
          policy.AllowAnyMethod();
          policy.AllowAnyHeader();
          policy.AllowAnyOrigin();
          policy.SetIsOriginAllowed(_ => true);
        });
      });
    }

    public void Configure(
      IApplicationBuilder app,
      IWebHostEnvironment env) {

      if(env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      }
      else {
        app.UseExceptionHandler("/error");
      }

      app.UseCors();
      app.UseRouting();

      app.UseEndpoints(endpoints => {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
