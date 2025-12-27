using Application.Extensions;
using Dapper;
using Microsoft.Data.SqlClient;
using Model.Binding;
using Model.View;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services {
  public class TestService(ConnectionPool connectionPool) {
    public async Task<PaginatedViewModel> AllAsync(string title, PaginationBindingModel pagination) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@Title", $"%{title}%", DbType.String, ParameterDirection.Input);
      var query = $"select max(t.Id) Id, max(Auth) Auth, t.Title, count(1) Expectations from Test t " +
        $"inner join TestXExpectation te on t.Id = te.TestId " +
        $"inner join Expectation e on te.ExpectationId = e.Id;";
      if(!string.IsNullOrWhiteSpace(title)) {
        query = query.Replace(";", " where Title like @Title;");
      }
      query = query.Replace(";", " group by t.Title " +
        $"order by t.Title {pagination.Direction} " +
        $"offset {pagination.Offset} rows fetch next {pagination.PageSize} rows only;" +
        $"select count(1) from [Test]");
      if(!string.IsNullOrWhiteSpace(title)) {
        query += " where Title like @Title;";
      }
      var result = await cp.QueryMultipleAsync(query, parameters);
      var data = result.Read();
      var count = result.ReadSingle<int>();
      cp.Close();
      return new PaginatedViewModel(count, data.ToJsonElement());
    }

    public async Task<JsonElement> SingleAsync(int id) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@Id", id, DbType.Int32, ParameterDirection.Input);
      var query =
        $"select distinct ex.Id, sc.Title Scenario, ca.Title Category, re.Target, cr.Title Criteria, ex.InsertedAt " +
        $"from Test te " +
        $"inner join TestXExpectation txe on te.Id = txe.TestId " +
        $"inner join Expectation ex on txe.ExpectationId = ex.Id " +
        $"inner join Request re on ex.RequestId = re.Id " +
        $"inner join Scenario sc on re.ScenarioId = sc.Id " +
        $"inner join Category ca on sc.CategoryId = ca.Id " +
        $"inner join Criteria cr on ex.CriteriaId = cr.Id " +
        $"where te.Id = @Id " +
        $"order by ex.Id;";
      var data = await cp.QueryAsync(query, parameters);
      cp.Close();
      return data.ToJsonElement();
    }

    public async Task<int> InsertAsync(InsertTestBindingModel model) {
      var cp = connectionPool.WebApp;
      cp.Open();
      using var tran = cp.BeginTransaction();
      var parameters = new DynamicParameters();
      parameters.Add($"@Title", model.Title, DbType.String, ParameterDirection.Input);
      parameters.Add($"@Auth", model.Auth.ToString(), DbType.String, ParameterDirection.Input);
      var insertTest = "insert into [Test] (Title, Auth) values(@Title, @Auth); select @@identity as TestId;";
      var test = await cp.QueryAsync(insertTest, parameters, tran);
      var affecedRows = 0;
      if(test != null) {
        affecedRows++;
        var testId = test.ToJsonElement()[0].GetProperty("TestId").GetInt32();
        parameters.Add($"@TestId", testId, DbType.String, ParameterDirection.Input);
        var values = new List<string>();
        for(var i = 0; i < model.ExpectationIds?.Length; i++) {
          parameters.Add($"@ExpectationId{i}", model.ExpectationIds[i], DbType.Int32, ParameterDirection.Input);
          values.Add($"(@TestId, @ExpectationId{i})");
        }
        var insertQuery = $"insert into TestXExpectation (TestId, ExpectationId) values{string.Join(",", values)};";
        affecedRows += await cp.ExecuteAsync(insertQuery, parameters, tran);
      }
      tran.Commit();
      cp.Close();
      return affecedRows;
    }

    public async Task<int> UpdateAsync(UpdateTestBindingModel model) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@Id", model.Id, DbType.Int32, ParameterDirection.Input);
      var columns = new List<string>();
      if(!string.IsNullOrWhiteSpace(model.Title)) {
        columns.Add("Title=@Title");
        parameters.Add($"@Title", model.Title, DbType.String, ParameterDirection.Input);
      }
      if(model.Auth.ValueKind != JsonValueKind.Null && model.Auth.ValueKind != JsonValueKind.Undefined) {
        columns.Add("Auth=@Auth");
        parameters.Add($"@Auth", model.Auth.ToString(), DbType.String, ParameterDirection.Input);
      }
      if(columns.Count == 0) {
        return 0;
      }
      var query = $"update [Test] set {string.Join(",", columns)} where Id = @Id;";
      var affecedRows = await cp.ExecuteAsync(query, parameters);
      cp.Close();
      return affecedRows;
    }

    public async Task<int> DeleteAsync(int id) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@Id", id, DbType.Int32, ParameterDirection.Input);
      var query = $"delete from [TestXExpectation] where TestId = @Id; delete from [Test] where Id = @Id;";
      var affecedRows = await cp.ExecuteAsync(query, parameters);
      cp.Close();
      return affecedRows;
    }
  }
}
