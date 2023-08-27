using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Scc.Services.Json;

public class JsonDateTimeConverter : JsonConverter<DateTime>
{
    private const string DateFormat = "MM-dd-yyyy HH:mm:ss";

    public override DateTime Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString()!, DateFormat, null);
    }

    public override void Write(
      Utf8JsonWriter writer,
      DateTime value,
      JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}
