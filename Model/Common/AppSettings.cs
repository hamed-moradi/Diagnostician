namespace Model.Common {
  public class AppSetting {
    public Destination Destination { get; set; }
    public ConnectionString ConnectionStrings { get; set; }
    public Identity Identity { get; set; }
  }

  public class Destination {
    public string Url { get; set; }
  }

  public class ConnectionString {
    public string WebApp { get; set; }
  }

  public class Identity {
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
  }
}
