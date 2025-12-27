using Application;
using Microsoft.AspNetCore.Mvc;
using Model.Common;
using System.Data;

namespace WebApi.Controllers {
  [Route("api/[controller]")]
  public class BaseController(
      ConnectionPool dbconn = null,
      HarmfulKeywordDetector keywordDetector = null): Controller {
    public readonly IDbConnection _webapp = dbconn?.WebApp ?? ServiceLocator.Current.GetInstance<ConnectionPool>().WebApp;
    public readonly HarmfulKeywordDetector _keywordDetector = keywordDetector ?? ServiceLocator.Current.GetInstance<HarmfulKeywordDetector>();
    public UserClaim UserClaims { get { return (UserClaim)HttpContext.Items[nameof(UserClaim)]; } }
  }
}
