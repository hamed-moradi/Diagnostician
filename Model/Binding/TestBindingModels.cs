using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Model.Binding {
  public class InsertTestBindingModel {
    [Required(ErrorMessage = $"{nameof(Title)} is requiered!")]
    public string Title { get; set; }
    [Required(ErrorMessage = $"{nameof(Auth)} is requiered!")]
    public JsonElement Auth { get; set; }
    [Required(ErrorMessage = $"{nameof(ExpectationIds)} is requiered!")]
    public int[] ExpectationIds { get; set; }
  }

  public class UpdateTestBindingModel {
    [Required(ErrorMessage = $"{nameof(Id)} is requiered!")]
    public int Id { get; set; }
    public string Title { get; set; }
    public JsonElement Auth { get; set; }
  }
}
