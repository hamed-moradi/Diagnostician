using RestSharp;
using System.Text.Json;

namespace Model.Binding {
  public class ApiCallerBindingModel {
    public Method Method { get; set; }
    public string Url { get; set; }
    //public IDictionary<string, string> Header { get; set; }
    public string Header { get; set; }
    public string Payload { get; set; }
  }
}
