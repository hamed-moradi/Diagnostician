using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Model.Binding {
  public class InsertExpectationBindingModel {
    [Required(ErrorMessage = $"{nameof(CriteriaId)} is requiered!")]
    public int CriteriaId { get; set; }
    [Required(ErrorMessage = $"{nameof(RequestId)} is requiered!")]
    public int RequestId { get; set; }
    [Required(ErrorMessage = $"{nameof(Status)} is requiered!")]
    public int Status { get; set; }
    public JsonElement Response { get; set; }
  }

  public class UpdateExpectationBindingModel {
    [Required(ErrorMessage = $"{nameof(Id)} is requiered!")]
    public int Id { get; set; }
    public int? Status { get; set; }
    public JsonElement Response { get; set; }
  }
}
