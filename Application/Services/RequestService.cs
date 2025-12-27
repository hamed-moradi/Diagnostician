using Application.Extensions;
using Dapper;
using Model.Binding;
using Model.View;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services {
  public class RequestService(ConnectionPool connectionPool) {
    public async Task<PaginatedViewModel> AllAsync(int scenarioId, PaginationBindingModel pagination) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@ScenarioId", scenarioId, DbType.Int32, ParameterDirection.Input);
      var query = $"select * from [Request] " +
        $"where ScenarioId = @ScenarioId " +
        $"order by {pagination.OrderBy} {pagination.Direction} " +
        $"offset {pagination.Offset} rows fetch next {pagination.PageSize} rows only;" +
        $"select count(1) from [Request] where ScenarioId = @ScenarioId;";
      var result = await cp.QueryMultipleAsync(query, parameters);
      var requests = result.Read();
      var count = result.ReadSingle<int>();
      cp.Close();
      return new PaginatedViewModel(count, requests.ToJsonElement());
    }

    public async Task<JsonElement> SingleAsync(int id) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@Id", id, DbType.Int32, ParameterDirection.Input);
      var requests = await cp.QueryAsync($"select * from [Request] where Id = @Id;", parameters);
      cp.Close();
      return requests.ToJsonElement();
    }

    public async Task<int> InsertAsync(InsertRequestBindingModel model) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@ScenarioId", model.ScenarioId, DbType.Int32, ParameterDirection.Input);
      parameters.Add($"@Method", model.Method, DbType.String, ParameterDirection.Input);
      parameters.Add($"@Target", model.Target, DbType.String, ParameterDirection.Input);
      parameters.Add($"@Header", model.Header.ToString(), DbType.String, ParameterDirection.Input);
      parameters.Add($"@Payload", model.Payload.ToString(), DbType.String, ParameterDirection.Input);
      var query = "insert into [Request] (ScenarioId,Method,Target,Header,Payload) " +
        $"values(@ScenarioId,@Method,@Target,@Header,@Payload);";
      var affecedRows = await cp.ExecuteAsync(query, parameters);
      cp.Close();
      return affecedRows;
    }

    public async Task<int> UpdateAsync(UpdateRequestBindingModel model) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@Id", model.Id, DbType.Int32, ParameterDirection.Input);
      var columns = new List<string>();
      if(!string.IsNullOrWhiteSpace(model.Method)) {
        columns.Add("Method=@Method");
        parameters.Add($"@Method", model.Method, DbType.String, ParameterDirection.Input);
      }
      if(!string.IsNullOrWhiteSpace(model.Target)) {
        columns.Add("Target=@Target");
        parameters.Add($"@Target", model.Target, DbType.String, ParameterDirection.Input);
      }
      if(model.Header.ValueKind != JsonValueKind.Null && model.Header.ValueKind != JsonValueKind.Undefined) {
        columns.Add("Header=@Header");
        parameters.Add($"@Header", model.Header.ToString(), DbType.String, ParameterDirection.Input);
      }
      if(model.Payload.ValueKind != JsonValueKind.Null && model.Payload.ValueKind != JsonValueKind.Undefined) {
        columns.Add("Payload=@Payload");
        parameters.Add($"@Payload", model.Payload.ToString(), DbType.String, ParameterDirection.Input);
      }
      if(columns.Count == 0) {
        return 0;
      }
      var query = $"update [Request] set {string.Join(",", columns)} where Id = @Id;";
      var affecedRows = await cp.ExecuteAsync(query, parameters);
      cp.Close();
      return affecedRows;
    }

    public async Task<int> DeleteAsync(int id) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@Id", id, DbType.Int32, ParameterDirection.Input);
      var query = $"delete from [Request] where Id = @Id;";
      var affecedRows = await cp.ExecuteAsync(query, parameters);
      cp.Close();
      return affecedRows;
    }
  }
}
