using System.Text.Json;
using Scc.Services.Json;

namespace Scc.DeviceDataProcessing.Core;

public class JsonProcessing
{
    JsonSerializerOptions jsonOptions = default!;

    public JsonProcessing()
    {
        jsonOptions = new() { WriteIndented = true };
        jsonOptions.Converters.Add(new JsonDateTimeConverter());
    }

    public T? Deserialize<T>(string fileName)
    {
        return JsonSerializer.Deserialize<T>(File.ReadAllText(fileName), jsonOptions);
    }

    public void Serialize<T>(string outputFileName, T obj)
    {
        string json = JsonSerializer.Serialize<T>(obj, jsonOptions);
        File.WriteAllText(outputFileName, json);
    }
}
