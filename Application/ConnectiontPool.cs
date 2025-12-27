using Microsoft.Data.SqlClient;
using Model.Common;
using System.Data;

namespace Application {
  public class ConnectionPool(AppSetting appSettings) {
    public IDbConnection WebApp => new SqlConnection(appSettings.ConnectionStrings.WebApp);
  }
}
