using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Application {
  public class JsonCompares {
    private void WriteNormalized(Utf8JsonWriter writer, JsonElement element) {
      switch(element.ValueKind) {
        case JsonValueKind.Object:
          writer.WriteStartObject();
          foreach(var prop in element.EnumerateObject()
                                       .OrderBy(p => p.Name, StringComparer.Ordinal)) {
            writer.WritePropertyName(prop.Name);
            WriteNormalized(writer, prop.Value);
          }
          writer.WriteEndObject();
          break;

        case JsonValueKind.Array:
          writer.WriteStartArray();
          foreach(var item in element.EnumerateArray()
                                      .Select(Normalize)
                                      .OrderBy(x => x, StringComparer.Ordinal)) {
            using var doc = JsonDocument.Parse(item);
            WriteNormalized(writer, doc.RootElement);
          }
          writer.WriteEndArray();
          break;

        default:
          element.WriteTo(writer);
          break;
      }
    }

    public string Normalize(JsonElement element) {
      using var stream = new MemoryStream();
      using var writer = new Utf8JsonWriter(stream);

      WriteNormalized(writer, element);
      writer.Flush();

      return Encoding.UTF8.GetString(stream.ToArray());
    }

    public bool EqualsIgnoreOrder(JsonElement a, JsonElement b) {
      if(a.ValueKind != b.ValueKind)
        return false;

      return Normalize(a) == Normalize(b);
    }

    public bool Includes(JsonElement superset, JsonElement subset) {
      if(subset.ValueKind != superset.ValueKind)
        return false;

      switch(subset.ValueKind) {
        case JsonValueKind.Object:
          var superProps = superset.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);

          foreach(var subProp in subset.EnumerateObject()) {
            if(!superProps.TryGetValue(subProp.Name, out var superVal))
              return false;

            if(!Includes(superVal, subProp.Value))
              return false;
          }
          return true;

        case JsonValueKind.Array:
          var superArr = superset.EnumerateArray().ToList();
          var subArr = subset.EnumerateArray().ToList();

          foreach(var subItem in subArr) {
            if(!superArr.Any(superItem => EqualsIgnoreOrder(superItem, subItem))) {
              return false;
            }
          }
          return true;

        default:
          return EqualsIgnoreOrder(superset, subset);
      }
    }
  }
}
