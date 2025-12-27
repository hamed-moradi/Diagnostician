using Application;
using Dapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Controllers {
  [Route("")]
  public class HomeController(
      AppSetting appSettings,
      Cryptographer cryptographer): BaseController {
    private readonly AppSetting _appSettings = appSettings;
    private readonly Cryptographer _cryptographer = cryptographer;

    public async Task<IActionResult> Index() {
      if(string.IsNullOrWhiteSpace(_appSettings.ConnectionStrings.WebApp))
        return BadRequest("Please set the connection string!");

      var sqlScript = "DBSqlScript.sql";
      var sqlScriptPath = $"{Environment.CurrentDirectory}\\{sqlScript}";

      try {
        if(System.IO.File.Exists(sqlScriptPath)) {
          var lines = new List<string>();
          foreach(var line in System.IO.File.ReadAllLines(sqlScriptPath, Encoding.UTF8)) {
            if(line.StartsWith("--"))
              continue;

            if(line.Contains("--"))
              lines.Add(line.Split("--")[0]);
            else
              lines.Add(line);
          }

          var content = string.Join(" ", lines);
          content = content.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace(" n'", " N'");
          //content = content.Replace("DefaultAppUserId", _appSettings.Identity.AppUserId);
          //content = content.Replace("DefaultPassword", _cryptographer.CalculateMD5Hash(_appSettings.Identity.Password));
          //content = content.Replace("DefaultPhone", _appSettings.Identity.Phone);

          if(string.IsNullOrWhiteSpace(content)) {
            return BadRequest($"{sqlScript} file is empty!");
          }
          else {
            await _webapp.ExecuteAsync(content);
            //return Ok(new { message = $"WebApp database: {_webapp.Database}" });
            return View(new { message = $"WebApp database: {_webapp.Database}" });
          }
        }
        return BadRequest($"{sqlScript} not found");
      }
      catch(Exception ex) {
        Log.Error(ex, ex.Source);
        return Problem(detail: ex.StackTrace, title: ex.Message);
      }
    }

    [Route("/error")]
    public IActionResult Error() {
      var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
      return Problem(detail: context.Error.StackTrace, title: context.Error.Message);
    }
  }
}