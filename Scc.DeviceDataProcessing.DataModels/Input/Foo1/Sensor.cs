namespace Scc.DeviceDataProcessing.DataModels.Input.Foo1;

public class Sensor
{
    public int Id { get; set; } = 0;

    public string Name { get; set; } = default!;

    public Crumb[] Crumbs { get; set; } = default!;
}
