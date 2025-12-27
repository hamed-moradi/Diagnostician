
namespace Model.Binding {
  public class PaginationBindingModel {
    public PaginationBindingModel() { }
    public PaginationBindingModel(
      int pageIndex = 1,
      int pageSize = 10,
      string orderBy = "Id",
      string direction = "asc") {
      PageIndex = pageIndex;
      PageSize = pageSize;
      OrderBy = orderBy;
      Direction = direction;
    }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string OrderBy { get; set; } = "Id";
    public string Direction { get; set; } = "Asc";
    public int Offset { get { return (PageIndex - 1) * PageSize; } }
  }
}
