using Application.Extensions;
using System.Collections.Generic;

namespace Application {
  public class HarmfulKeywordDetector {
    public static string[] SqlUnsafeKeywords = { "shutdown", "exec", "having", "union", "insert", "update", "delete", "drop", "truncate", "script" };

    public bool IsDangerous(string text) {
      return text.ContainHarmfulKeywords();
    }

    public bool IsDangerous(string[] texts) {
      foreach(var text in texts)
        if(text.ContainHarmfulKeywords())
          return true;
      return false;
    }

    public bool IsDangerous<T>(T model) {
      foreach(var prop in model.GetType().GetProperties()) {
        if(prop.PropertyType == typeof(string)) {
          var val = prop.GetValue(model, null)?.ToString();
          return IsDangerous(val);
        }
        else if(prop.PropertyType == typeof(string[])) {
          var val = (string[])prop.GetValue(model, null);
          return IsDangerous(val);
        }
        else if(prop.PropertyType == typeof(List<string>)) {
          var val = (List<string>)prop.GetValue(model, null);
          return IsDangerous(val.ToArray());
        }
      }
      return false;
    }
  }
}
