using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Application {
  public class Cryptographer {
    public string CalculateMD5Hash(string input) {
      var hash = MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(input));

      var sb = new StringBuilder();
      for(int i = 0; i < hash.Length; i++) {
        sb.Append(hash[i].ToString("X2"));
      }

      return sb.ToString();
    }

    public string NewToken {
      get {
        var time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
        var key = Guid.NewGuid().ToByteArray();
        var md5 = MD5.Create().ComputeHash(time.Concat(key).ToArray());
        var sha = new SHA256Managed().ComputeHash(md5);
        var token = Convert.ToBase64String(sha);
        return token;
      }
    }
  }
}
