using Application.Extensions;
using Dapper;
using Model.Binding;
using Model.View;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services {
  public class ScenarioService(ConnectionPool connectionPool) {
    public async Task<PaginatedViewModel> AllAsync(string title, PaginationBindingModel pagination) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var query = $"select s.Id, c.Title Category, s.Title, s.Description from Scenario s inner join dbo.Category c on c.Id = s.CategoryId;";
      if(!string.IsNullOrWhiteSpace(title)) {
        query = query.Replace(";", $" where Title like %{title.ToLower()}%;");
      }
      query = query.Replace(";", $" order by s.{pagination.OrderBy} {pagination.Direction} " +
        $"offset {pagination.Offset} rows fetch next {pagination.PageSize} rows only;" +
        "select count(1) from [Scenario];");
      var result = await cp.QueryMultipleAsync(query);
      var data = result.Read();
      var count = result.ReadSingle<int>();
      cp.Close();
      return new PaginatedViewModel(count, data.ToJsonElement());
    }

    public async Task<JsonElement> SingleAsync(int id) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var query = $"select * from [Scenario] where Id = @Id;";
      var parameters = new DynamicParameters();
      parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
      var scenario = await cp.QueryAsync(query, parameters);
      cp.Close();
      return scenario.ToJsonElement();
    }

    public async Task<int> InsertAsync(InsertScenarioBindingModel model) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@CategoryId", model.CategoryId, DbType.Int32, ParameterDirection.Input);
      parameters.Add($"@Title", model.Title, DbType.String, ParameterDirection.Input);
      parameters.Add($"@Description", model.Description, DbType.String, ParameterDirection.Input);
      var query = "insert into [Scenario] (CategoryId,Title,Description) " +
        $"values(@CategoryId,@Title,@Description);";
      var affecedRows = await cp.ExecuteAsync(query, parameters);
      cp.Close();
      return affecedRows;
    }

    public async Task<int> UpdateAsync(UpdateScenarioBindingModel model) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@Id", model.Id, DbType.Int32, ParameterDirection.Input);
      var columns = new List<string>();
      if(model.CategoryId.HasValue) {
        columns.Add("CategoryId=@CategoryId");
        parameters.Add($"@CategoryId", model.CategoryId, DbType.Int32, ParameterDirection.Input);
      }
      if(!string.IsNullOrWhiteSpace(model.Title)) {
        columns.Add("Title=@Title");
        parameters.Add($"@Title", model.Title, DbType.String, ParameterDirection.Input);
      }
      if(!string.IsNullOrWhiteSpace(model.Description)) {
        columns.Add("Description=@Description");
        parameters.Add($"@Description", model.Description, DbType.String, ParameterDirection.Input);
      }
      if(columns.Count == 0) {
        return 0;
      }
      var query = $"update [Scenario] set {string.Join(",", columns)} where Id = @Id;";
      var affecedRows = await cp.ExecuteAsync(query, parameters);
      cp.Close();
      return affecedRows;
    }

    public async Task<int> DeleteAsync(int id) {
      var cp = connectionPool.WebApp;
      cp.Open();
      var parameters = new DynamicParameters();
      parameters.Add($"@Id", id, DbType.Int32, ParameterDirection.Input);
      var query = $"delete from [Scenario] where Id = @Id;";
      var affecedRows = await cp.ExecuteAsync(query, parameters);
      cp.Close();
      return affecedRows;
    }
  }
}
