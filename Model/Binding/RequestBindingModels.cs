using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Model.Binding {
  public class InsertRequestBindingModel {
    [Required(ErrorMessage = $"{nameof(ScenarioId)} is requiered!")]
    public int ScenarioId { get; set; }
    [Required(ErrorMessage = $"{nameof(Method)} is requiered!")]
    public string Method { get; set; }
    [Required(ErrorMessage = $"{nameof(Target)} is requiered!")]
    public string Target { get; set; }
    public JsonElement Header { get; set; }
    public JsonElement Payload { get; set; }
  }

  public class UpdateRequestBindingModel {
    [Required(ErrorMessage = $"{nameof(Id)} is requiered!")]
    public int Id { get; set; }
    public string Method { get; set; }
    public string Target { get; set; }
    public JsonElement Header { get; set; }
    public JsonElement Payload { get; set; }
  }
}
