using Application.Services;
using Model.Binding;
using Model.Common;
using RestSharp;
using Serilog;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application {
  public class ApiCaller {
    private static readonly object syncLock = new();
    private static ApiCallerEngine apiCallerEngine;

    public static ApiCallerEngine Instance(Dictionary<string, object> authHeaders, bool forceAuth = false) {
      lock(syncLock) {
        apiCallerEngine ??= new ApiCallerEngine(authHeaders, forceAuth);
        return apiCallerEngine;
      }
    }
  }

  public class ApiCallerEngine {
    #region ctors
    private static RestClient client;
    private static IDictionary<string, string> headers;
    private static AppSetting appSetting = ServiceLocator.Current.GetInstance<AppSetting>();
    private static ILogger logger = ServiceLocator.Current.GetInstance<ILogger>();

    public ApiCallerEngine(Dictionary<string, object> authHeaders, bool forceAuth) {
      client = new RestClient(appSetting.Destination.Url);
      var authResult = AuthenticationApiService.GetVNBasedResult(authHeaders, forceAuth);
      if(authResult == null) {
        forceAuth = true;
        logger.Warning($"Authentication failed!");
        return;
      }
      headers = new Dictionary<string, string> {
        { "Content-Type", "application/json; charset=utf-8" },
        { "Authorization", $"Bearer {authResult.Token}" }
      };
    }
    #endregion

    public async Task<RestResponse> CallAsync(ApiCallerBindingModel model) {
      var request = new RestRequest($"{appSetting.Destination.Url}{model.Url}", model.Method);
      foreach(var header in headers) {
        request.AddHeader(header.Key, header.Value);
      }
      if(!string.IsNullOrWhiteSpace(model.Header)) {
        var headers = JsonSerializer.Deserialize<IDictionary<string, object>>(model.Header);
        foreach(var header in headers) {
          request.AddHeader(header.Key, header.Value?.ToString());
        }
      }
      if(!string.IsNullOrWhiteSpace(model.Payload)) {
        request.AddJsonBody(model.Payload);
      }
      return await client.ExecuteAsync(request);
    }
  }
}
