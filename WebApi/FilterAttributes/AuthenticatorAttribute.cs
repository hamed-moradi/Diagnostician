using Application;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Data;
using System.Net.Http.Headers;

namespace WebApi.FilterAttributes {
  public class AuthenticatorAttribute: ActionFilterAttribute {
    private readonly JWTHandler jwtHandler = ServiceLocator.Current.GetInstance<JWTHandler>();

    #region ctor
    private readonly IDbConnection _dbconn;

    public AuthenticatorAttribute() {
      _dbconn = ServiceLocator.Current.GetInstance<ConnectionPool>().WebApp;
    }
    #endregion

    public override void OnActionExecuting(ActionExecutingContext context) {
      var header = context.HttpContext.Request.Headers;
      var validToken = AuthenticationHeaderValue.TryParse(header.Authorization, out var parsedToken);
      if(!validToken) {
        throw new Exception("Token not found");
        //context.Result = new CustomActionResult(HttpStatusCode.Unauthorized, "Token not found");
        //return;
      }
      if(parsedToken.Scheme.ToLower() != "bearer") {
        throw new Exception("Token is empty");
        //context.Result = new CustomActionResult(HttpStatusCode.Unauthorized, "Invalid token");
        //return;
      }
      var isValid = jwtHandler.Verify(parsedToken.Parameter);

      base.OnActionExecuting(context);
    }
  }
}
