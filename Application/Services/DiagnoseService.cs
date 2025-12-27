using Dapper;
using Model.Binding;
using Model.View;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Application.Services {
  public class DiagnoseService(ConnectionPool connectionPool, JsonCompares jsonCompares) {
    public async Task<IEnumerable<DiagnoseViewModel>> Run(int testId) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@TestId", testId, DbType.Int32, ParameterDirection.Input);
      var query =
        $"select ex.Id ExpectationId, te.Auth, ca.Title Category, sc.Title Scenario, " +
        $"re.Target, re.Method, re.Header, re.Payload, ex.Status, ex.Response, " +
        $"cr.Title Criteria, ex.InsertedAt from Test te " +
        $"inner join TestXExpectation txe on te.Id = txe.TestId " +
        $"inner join Expectation ex on txe.ExpectationId = ex.Id " +
        $"inner join Request re on ex.RequestId = re.Id " +
        $"inner join Scenario sc on re.ScenarioId = sc.Id " +
        $"inner join Category ca on sc.CategoryId = ca.Id " +
        $"inner join Criteria cr on ex.CriteriaId = cr.Id " +
        $"where te.Id = @TestId " +
        $"order by ex.Id;";
      var Diagnosed = await cp.QueryAsync<DiagnoseViewModel>(query, parameters);
      cp.Close();
      return Diagnosed;
    }

    public bool Validate(DiagnoseBindingModel model) {
      return model.Criteria.ToLower() switch {
        "exact" => jsonCompares.Normalize(model.ExpectedResponse) == jsonCompares.Normalize(model.ReceivedResponse),
        "semi" => jsonCompares.Includes(model.ExpectedResponse, model.ReceivedResponse),
        _ => false,
      };
    }
  }
}
