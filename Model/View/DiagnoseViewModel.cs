using System;

namespace Model.View {
  public class DiagnoseViewModel {
    public int ExpectationId { get; set; }
    public string Auth { get; set; }
    public string Category { get; set; }
    public string Scenario { get; set; }
    public string Target { get; set; }
    public string Method { get; set; }
    public string Header { get; set; }
    public string Payload { get; set; }
    public int Status { get; set; }
    public string Response { get; set; }
    public string Criteria { get; set; }
    public DateTime InsertedAt { get; set; }
    public bool Accepted { get; set; } = false;
  }
}
