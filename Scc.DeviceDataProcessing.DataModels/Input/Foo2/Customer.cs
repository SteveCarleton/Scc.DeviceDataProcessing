namespace Scc.DeviceDataProcessing.DataModels.Input.Foo2;

public class Customer
{
    public int CompanyId { get; set; } = default!;

    public string Company { get; set; } = default!;

    public Device[] Devices { get; set; } = default!;
}
