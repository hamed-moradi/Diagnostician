using System.Text.Json;

namespace Model.Binding {
  public class DiagnoseBindingModel {
    public int ExpectedStatus { get; set; }
    public JsonElement ExpectedResponse { get; set; }
    public int ReceivedStatus { get; set; }
    public JsonElement ReceivedResponse { get; set; }
    public string Criteria { get; set; }
  }
}
