using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Entity {
  public class User {
    public int? Id { get; set; }
    public int? AppUserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
    public string Token { get; set; }
    public DateTime? LastSignedinAt { get; set; }
    public DateTime? CreatedAt { get; set; }
  }
}
