namespace Scc.DeviceDataProcessing.DataModels.Input.Foo2;

public class Device
{
    public int DeviceID { get; set; } = default!;

    public string Name { get; set; } = default!;

    public DateTime StartDateTime { get; set; } = default!;

    public SensorData[] SensorData { get; set; } = default!;
}

