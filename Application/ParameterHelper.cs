using Application.Extensions;
using Dapper;
using Model.Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Application {
  public class ParameterHelper {
    public DynamicParameters GetParams<T>(T model, int length) {
      var result = new DynamicParameters();

      var modelparams = model.GetType().GetProperties();
      for(int i = 0; i < length; i++) {
        foreach(var param in modelparams) {
          var type = param.PropertyType;
          var val = param.GetValue(model, null);
          if(type == typeof(string) && val != null) {
            var strval = val.ToString();
            var splitedval = strval.Split(",");
            if(splitedval.Length > 1)
              val = splitedval[i];
            val = val.ToString().Trim();
          }
          result.Add($"{param.Name}{i}", value: val, dbType: type.ToDbType(), direction: ParameterDirection.Input);
        }
      }

      return result;
    }

    public DynamicParameters GetParams<T>(List<T> model, int lowBound, int highBound) {
      var result = new DynamicParameters();

      var modelparams = typeof(T).GetProperties();
      for(int i = lowBound; i < highBound; i++) {
        foreach(var param in modelparams) {
          var type = param.PropertyType;
          var val = param.GetValue(model[i], null);
          if(type == typeof(string) && val != null) {
            var strval = val.ToString();
            var splitedval = strval.Split(",");
            if(splitedval.Length > 1)
              val = splitedval[i];
            val = val.ToString().Trim();
          }
          result.Add($"{param.Name}{i}", value: val, dbType: type.ToDbType(), direction: ParameterDirection.Input);
        }
      }

      return result;
    }

    public DynamicParameters GetParams<T>(List<T> model) {
      return GetParams<T>(model, 0, model.Count);
    }
  }
}
