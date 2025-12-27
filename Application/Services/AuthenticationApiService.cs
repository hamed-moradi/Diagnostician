using Model.Common;
using Model.View;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;

namespace Application.Services {
  public class AuthenticationApiService {
    private static AuthResultViewModel authResult;
    private static DateTime lastCall;
    private static object syncLock = new();
    private static AppSetting appSetting = ServiceLocator.Current.GetInstance<AppSetting>();
    private static ILogger logger = ServiceLocator.Current.GetInstance<ILogger>();

    public static AuthResultViewModel SignIn(Dictionary<string, object> authHeaders)   {
      var client = new RestClient(appSetting.Destination.Url);
      var request = new RestRequest("/api/appuser/signin", Method.Post);

      request.AddHeader("Content-Type", "application/json");
      request.AddHeader("Accept", "application/json");
      request.AddJsonBody(authHeaders);

      //logger.Debug($"Trying to authenticate of {appSetting.Destination.Username} in {appSetting.Destination.Url}");
      var response = client.Execute<AuthResultViewModel>(request);
      switch(response.StatusCode) {
        case HttpStatusCode.OK:
          return new AuthResultViewModel {
            Token = response.Data.Token,
            ExpiresAt = response.Data.ExpiresAt
          };
        default:
          logger.Error(response.ErrorException, response.ErrorMessage, response.Content);
          return null;
      }
    }

    public static AuthResultViewModel GetVNBasedResult(Dictionary<string, object> authHeaders, bool force = true) {
      lock(syncLock) {
        var timeDiff = DateTime.Now - lastCall;
        if(authResult == null || force || timeDiff >= TimeSpan.FromDays(7)) {
          lastCall = DateTime.Now;
          authResult = SignIn(authHeaders);
        }
        return authResult;
      }
    }
  }
}
