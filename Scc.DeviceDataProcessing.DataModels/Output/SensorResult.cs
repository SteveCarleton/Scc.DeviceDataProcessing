namespace Scc.DeviceDataProcessing.DataModels.Output;

public class SensorResult
{
    public int CompanyId { get; set; } = default!;

    public string CompanyName { get; set; } = default!;

    public int? DeviceId { get; set; }

    public string DeviceName { get; set; } = default!;

    public DateTime? FirstReadingDtm { get; set; } = default!;

    public DateTime? LastReadingDtm { get; set; }

    public int? TemperatureCount { get; set; }

    public double? AverageTemperature { get; set; }

    public int? HumidityCount { get; set; }

    public double? AverageHumidity { get; set; }
}
