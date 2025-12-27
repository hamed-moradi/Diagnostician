using Application.Extensions;
using Dapper;
using Model.Binding;
using Model.View;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services {
  public class ExpectationService(ConnectionPool connectionPool) {
    public async Task<PaginatedViewModel> AllAsync(int requestId, PaginationBindingModel pagination) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@RequestId", requestId, DbType.Int32, ParameterDirection.Input);
      var query = $"select e.Id, c.Title Criteria, e.RequestId, e.Status, e.Response from Expectation e inner join dbo.Criteria c on c.Id = e.CriteriaId " +
        $"where RequestId = @RequestId " +
        $"order by e.{pagination.OrderBy} {pagination.Direction} " +
        $"offset {pagination.Offset} rows fetch next {pagination.PageSize} rows only;" +
        $"select count(1) from [Expectation] where RequestId = @RequestId;";
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
      var data = await cp.QueryAsync($"select * from [Expectation] where Id = @Id;", parameters);
      cp.Close();
      return data.ToJsonElement();
    }

    public async Task<int> InsertAsync(InsertExpectationBindingModel model) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@CriteriaId", model.CriteriaId, DbType.Int32, ParameterDirection.Input);
      parameters.Add($"@RequestId", model.RequestId, DbType.Int32, ParameterDirection.Input);
      parameters.Add($"@Status", model.Status, DbType.Int32, ParameterDirection.Input);
      parameters.Add($"@Response", model.Response.ToString(), DbType.String, ParameterDirection.Input);
      var query = "insert into [Expectation] (CriteriaId,RequestId,Status,Response) " +
        $"values(@CriteriaId,@RequestId,@Status,@Response);";
      var affecedRows = await cp.ExecuteAsync(query, parameters);
      cp.Close();
      return affecedRows;
    }

    public async Task<int> UpdateAsync(UpdateExpectationBindingModel model) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@Id", model.Id, DbType.Int32, ParameterDirection.Input);
      var columns = new List<string>();
      if(model.Status.HasValue) {
        columns.Add("Status=@Status");
        parameters.Add($"@Status", model.Status, DbType.Int32, ParameterDirection.Input);
      }
      if(model.Response.ValueKind != JsonValueKind.Null && model.Response.ValueKind != JsonValueKind.Undefined) {
        columns.Add("Response=@Response");
        parameters.Add($"@Response", model.Response.ToString(), DbType.String, ParameterDirection.Input);
      }
      if(columns.Count == 0) {
        return 0;
      }
      var query = $"update [Expectation] set {string.Join(",", columns)} where Id = @Id;";
      var affecedRows = await cp.ExecuteAsync(query, parameters);
      cp.Close();
      return affecedRows;
    }

    public async Task<int> DeleteAsync(int id) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@Id", id, DbType.Int32, ParameterDirection.Input);
      var query = $"delete from [Expectation] where Id = @Id;";
      var affecedRows = await cp.ExecuteAsync(query, parameters);
      cp.Close();
      return affecedRows;
    }
  }
}
