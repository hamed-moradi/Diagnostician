using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Common {
  public class UserClaim {
    public int Id { get; set; }
    public int AppUserId { get; set; }
    public string Username { get; set; }
    public string Phone { get; set; }
    public DateTime? LastSignedinAt { get; set; }
  }
}
