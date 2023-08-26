namespace Scc.DeviceDataProcessing.DataModels.Input.Foo1;

public class Partner
{
    public int PartnerId { get; set; } = 0;

    public string PartnerName { get; set; } = default!;

    public Tracker[] Trackers { get; set; } = default!;
}
