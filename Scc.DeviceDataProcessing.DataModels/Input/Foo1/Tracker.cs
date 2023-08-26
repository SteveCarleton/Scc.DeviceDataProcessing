namespace Scc.DeviceDataProcessing.DataModels.Input.Foo1;

public class Tracker
{
    public int Id { get; set; } = 0;

    public string Model { get; set; } = default!;

    public DateTime ShipmentStartDtm { get; set; } = default!;

    public Sensor[] Sensors { get; set; } = default!;
}
