using Application.Extensions;
using Dapper;
using Model.Binding;
using Model.View;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services {
  public class RunService(ConnectionPool connectionPool) {
    public async Task<PaginatedViewModel> AllAsync(int expectationId, PaginationBindingModel pagination) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@ExpectationId", expectationId, DbType.Int32, ParameterDirection.Input);
      var query = $"select * from [Run] " +
        $"where ExpectationId = @ExpectationId " +
        $"order by {pagination.OrderBy} {pagination.Direction} " +
        $"offset {pagination.Offset} rows fetch next {pagination.PageSize} rows only;" +
        $"select count(1) from [Run] where ExpectationId = @ExpectationId;";
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
      var data = await cp.QueryAsync($"select * from [Run] where Id = @Id;", parameters);
      cp.Close();
      return data.ToJsonElement();
    }

    public async Task<int> InsertAsync(InsertRunBindingModel model) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@ExpectationId", model.ExpectationId, DbType.Int32, ParameterDirection.Input);
      parameters.Add($"@ReceivedStatus", model.ReceivedStatus, DbType.Int32, ParameterDirection.Input);
      parameters.Add($"@ReceivedResponse", model.ReceivedResponse.ToString(), DbType.String, ParameterDirection.Input);
      parameters.Add($"@Duration", model.Duration, DbType.Int64, ParameterDirection.Input);
      parameters.Add($"@Accepted", model.Accepted, DbType.Boolean, ParameterDirection.Input);
      var query = "insert into [Run] (ExpectationId,ReceivedStatus,ReceivedResponse,Duration,Accepted) " +
        $"values(@ExpectationId,@ReceivedStatus,@ReceivedResponse,@Duration,@Accepted);";
      var affecedRows = await cp.ExecuteAsync(query, parameters);
      cp.Close();
      return affecedRows;
    }

    public async Task<int> DeleteAsync(int id) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@Id", id, DbType.Int32, ParameterDirection.Input);
      var query = $"delete from [Run] where Id = @Id;";
      var affecedRows = await cp.ExecuteAsync(query, parameters);
      cp.Close();
      return affecedRows;
    }
  }
}
