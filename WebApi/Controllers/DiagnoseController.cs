using Application;
using Application.Extensions;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Binding;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApi.Controllers {
  public class DiagnoseController(
    DiagnoseService diagnoseService,
    RunService runService): BaseController {
    [HttpGet, Route("run")]
    public async Task<HttpResponseMessage> Run([FromQuery] int id) {
      var response = new HttpResponseMessage(HttpStatusCode.OK);
      var scenarios = (await diagnoseService.Run(id)).ToList();
      for(var i = 0; i < scenarios?.Count; i++) {
        var timer = new Stopwatch();
        timer.Start();
        var result = await ApiCaller.Instance(scenarios[i].Auth.ToDictionary(), i == 0)
          .CallAsync(new ApiCallerBindingModel {
            Url = scenarios[i].Target,
            Method = scenarios[i].Method.ToRestSharpMethod(),
            Header = scenarios[i].Header,
            Payload = scenarios[i].Payload
          });
        scenarios[i].Accepted = diagnoseService.Validate(new DiagnoseBindingModel {
          ExpectedStatus = scenarios[i].Status,
          ExpectedResponse = scenarios[i].Response.ToJsonElement(),
          ReceivedStatus = (int)result.StatusCode,
          ReceivedResponse = result.Content.ToJsonElement(),
          Criteria = scenarios[i].Criteria
        });
        timer.Stop();
        await runService.InsertAsync(new InsertRunBindingModel {
          ExpectationId = scenarios[i].ExpectationId,
          ReceivedStatus = (int)result.StatusCode,
          ReceivedResponse = result.Content.ToJsonElement(),
          Duration = timer.ElapsedMilliseconds,
          Accepted = scenarios[i].Accepted
        });
        await HttpContext.Response.WriteAsync(JsonSerializer.Serialize(scenarios[i]));
        await HttpContext.Response.Body.FlushAsync();
      }
      return response;
    }
  }
}