using Model.Binding;
using Model.View;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Application.Extensions {
  public static class CommonExtension {
    public static bool IsPhoneNumber(this string number) {
      //return Regex.Match(number, "^[+][0-9]{12}$").Success;
      return Regex.Match(number, "^[0][0-9]{10}$").Success;
    }

    public static string RemoveAccent(this string txt) {
      var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
      return Encoding.ASCII.GetString(bytes);
    }

    public static string Slugify(this string phrase) {
      string str = phrase.RemoveAccent().ToLower();
      str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // Remove all non valid chars          
      str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space  
      str = Regex.Replace(str, @"\s", "-"); // //Replace spaces by dashes
      return str;
    }

    public static bool ContainHarmfulKeywords(this string text) {
      if(string.IsNullOrWhiteSpace(text))
        return false;
      var dangerousKeywords = HarmfulKeywordDetector.SqlUnsafeKeywords.Where(keyword => text.ToLower().Contains(keyword));
      return dangerousKeywords.Any();
    }

    public static Method ToRestSharpMethod(this string method) {
      return method.ToLower() switch {
        "get" => Method.Get,
        "post" => Method.Post,
        _ => throw new Exception("Invalid Method!"),
      };
    }

    public static JsonElement ToJsonElement(this IEnumerable<dynamic> items) {
      string json = JsonSerializer.Serialize(items);
      using var doc = JsonDocument.Parse(json);
      return doc.RootElement.Clone();
    }

    public static JsonElement ToJsonElement(this string items) {
      string json = JsonSerializer.Serialize(items);
      using var doc = JsonDocument.Parse(json);
      return doc.RootElement.Clone();
    }

    public static Dictionary<string, object> ToDictionary(this string model) {
      return JsonSerializer.Deserialize<Dictionary<string, object>>(model);
    }

    //public static ApiCallerBindingModel ToApiCallerBindingModel(this DiagnoseViewModel model) {
    //  var result = new ApiCallerBindingModel {
    //    Url = model.Target,
    //    Method = model.Method.ToRestSharpMethod()
    //  };
    //  if(!string.IsNullOrWhiteSpace(model.Header)) {
    //    result.Header = model.Header.ToJsonElement();
    //  }
    //  if(!string.IsNullOrWhiteSpace(model.Payload)) {
    //    result.Payload = model.Payload.ToJsonElement();
    //  }
    //  return result;
    //}
  }
}
