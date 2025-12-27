
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Model.Binding {
  public class InsertRunBindingModel {
    [Required(ErrorMessage = $"{nameof(ExpectationId)} is requiered!")]
    public int ExpectationId { get; set; }
    [Required(ErrorMessage = $"{nameof(ReceivedStatus)} is requiered!")]
    public int ReceivedStatus { get; set; }
    public JsonElement ReceivedResponse { get; set; }
    [Required(ErrorMessage = $"{nameof(Duration)} is requiered!")]
    public long Duration { get; set; }
    [Required(ErrorMessage = $"{nameof(Accepted)} is requiered!")]
    public bool Accepted { get; set; }
  }

  public class UpdateRunBindingModel {
    [Required(ErrorMessage = $"{nameof(Id)} is requiered!")]
    public int Id { get; set; }
    public int? ReceivedStatus { get; set; }
    public JsonElement ReceivedResponse { get; set; }
  }
}
