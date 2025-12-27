namespace Model.Binding {
  public class InsertScenarioBindingModel {
    public int CategoryId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
  }

  public class UpdateScenarioBindingModel {
    public int Id { get; set; }
    public int? CategoryId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
  }
}
