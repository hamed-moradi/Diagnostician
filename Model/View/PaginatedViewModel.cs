using System.Text.Json;

namespace Model.View {
  public class PaginatedViewModel {
    public PaginatedViewModel() { }
    public PaginatedViewModel(int totalCount, JsonElement data) {
      TotalCount = totalCount;
      Data = data;
    }
    public int TotalCount { get; set; }
    public JsonElement Data { get; set; }
  }
}
