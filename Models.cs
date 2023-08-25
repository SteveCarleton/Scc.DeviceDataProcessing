namespace Scc.DeviceDataProcessing.Models;

public class Partner
{
    public int PartnerId { get; set; } = default!;
    public string PartnerName { get; set; } = default!;
    public Tracker[] Trackers { get; set; } = default!;
}

public class Tracker
{
    public int Id { get; set; } = default!;
    public string Model { get; set; } = default!;
    public DateTime ShipmentStartDtm { get; set; } = default!;
    public Sensor[] Sensors { get; set; } = default!;
}

public class Sensor
{
    public int Id { get; set; } = default!;
    public string Name { get; set; } = default!;    // temp or humidity
    public Crumb[] Crumbs { get; set; } = default!;
}

public class Crumb
{
    public DateTime CreatedDtm { get; set; } = default!;
    public double Value { get; set; } = default!;
}


public class Customer
{
    public int CompanyId { get; set; } = default!;
    public string Company { get; set; } = default!;

    public Device[] Devices { get; set; } = default!;

}

public class Device
{
    public int DeviceId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DateTime StartDateTime { get; set; } = default!;
    public SensorData[] SensorData { get; set; } = default!;
}

public class SensorData
{
    public string SensorType { get; set; } = default!;  // temp or humidity
    public DateTime DateTime { get; set; } = default!;
    public double Value { get; set; } = default!;
}

public class SensorResult
{
    public int CompanyId { get; set; } // Foo1: PartnerId, Foo2: CompanyId
    public string CompanyName { get; set; } = default!; // Foo1: PartnerName, Foo2: Company
    public int? DeviceId { get; set; } // Foo1: Id, Foo2: DeviceID
    public string DeviceName { get; set; } = default!; // Foo1: Model, Foo2: Name
    public DateTime? FirstReadingDtm { get; set; } // Foo1: Trackers.Sensors.Crumbs, Foo2: Devices.SensorData
    public DateTime? LastReadingDtm { get; set; }
    public int? TemperatureCount { get; set; }
    public double? AverageTemperature { get; set; }
    public int? HumidityCount { get; set; }
    public double? AverageHumdity { get; set; }
}


public enum SensorTypes
{
    Temperature,
    Humidity
}
