using Scc.DeviceDataProcessing.DataModels.Input.Foo1;
using Scc.DeviceDataProcessing.DataModels.Input.Foo2;
using Scc.DeviceDataProcessing.DataModels.Output;

namespace Scc.DeviceDataProcessing.Core;

public class DataProcessing
{
    public List<SensorResult> MergeDeviceData(Partner? partner, Customer? customer)
    {
        List<SensorResult> sensorResultList = new();

        foreach (Tracker tracker in partner!.Trackers)
        {
            //foreach (Sensor sensor in tracker.Sensors)
            //{
                SensorResult sensorResult = new()
                {
                    CompanyId = partner.PartnerId,
                    CompanyName = partner.PartnerName,
                    DeviceId = tracker.Id,  //sensor.Id,
                    DeviceName = tracker.Model, //sensor.Name,
                    FirstReadingDtm = (from c in tracker.Sensors[0].Crumbs select c).Min(c => c.CreatedDtm),
                    LastReadingDtm = (from c in tracker.Sensors[0].Crumbs select c).Max(c => c.CreatedDtm),

                    TemperatureCount = (from s in tracker.Sensors where s.Name == "Temperature" select s.Crumbs)
                        .FirstOrDefault()?.Length,

                    AverageTemperature = (from s in tracker.Sensors where s.Name == "Temperature" select s.Crumbs)
                        .FirstOrDefault<Crumb[]>()?
                        .ToList()
                        .Average(c => c.Value),

                    HumidityCount = (from s in tracker.Sensors where s.Name == "Humidty" select s.Crumbs)
                        .FirstOrDefault()?.Length,

                    AverageHumidity = (from s in tracker.Sensors where s.Name == "Humidty" select s.Crumbs)
                        .FirstOrDefault<Crumb[]>()?
                        .ToList()
                        .Average(c => c.Value),
                };

                sensorResultList.Add(sensorResult);
            //}
        }

        foreach (Device device in customer!.Devices)
        {
            SensorResult sensorResult = new()
            {
                CompanyId = customer.CompanyId,
                CompanyName = customer.Company,
                DeviceId = device.DeviceID,
                DeviceName = device.Name,
                FirstReadingDtm = (from s in device.SensorData select s).Min(c => c.DateTime),
                LastReadingDtm = (from s in device.SensorData select s).Max(c => c.DateTime),
                TemperatureCount = (from s in device.SensorData where s.SensorType == "TEMP" select s).Count(),
                AverageTemperature = (from s in device.SensorData where s.SensorType == "TEMP" select s).Average(d => d.Value),
                HumidityCount = (from s in device.SensorData where s.SensorType == "HUM" select s).Count(),
                AverageHumidity = (from s in device.SensorData where s.SensorType == "HUM" select s).Average(d => d.Value)
            };

            sensorResultList.Add(sensorResult);
        }

        return sensorResultList;
    }

    //public List<SensorResult> MergeDeviceData(Partner? partner, Customer? customer)
    //{
    //    List<SensorResult> sensorResultList = new();

    //    foreach (Tracker tracker in partner!.Trackers)
    //    {
    //        foreach (Sensor sensor in tracker.Sensors)
    //        {
    //            SensorResult sensorResult = new()
    //            {
    //                CompanyId = partner.PartnerId,
    //                CompanyName = partner.PartnerName,
    //                DeviceId = sensor.Id,
    //                DeviceName = sensor.Name,
    //                FirstReadingDtm = (from c in sensor.Crumbs select c).Min(c => c.CreatedDtm),
    //                LastReadingDtm = (from c in sensor.Crumbs select c).Max(c => c.CreatedDtm),

    //                TemperatureCount = (from s in tracker.Sensors where s.Name == "Temperature" select s.Crumbs)
    //                    .FirstOrDefault()?.Count(),

    //                AverageTemperature = (from s in tracker.Sensors where s.Name == "Temperature" select s.Crumbs)
    //                    .FirstOrDefault<Crumb[]>()?
    //                    .ToList()
    //                    .Average(c => c.Value),

    //                HumidityCount = (from s in tracker.Sensors where s.Name == "Humidty" select s.Crumbs)
    //                    .FirstOrDefault()?.Count(),

    //                AverageHumidity = (from s in tracker.Sensors where s.Name == "Humidty" select s.Crumbs)
    //                    .FirstOrDefault<Crumb[]>()?
    //                    .ToList()
    //                    .Average(c => c.Value),
    //            };

    //            sensorResultList.Add(sensorResult);
    //        }
    //    }

    //    foreach (Device device in customer!.Devices)
    //    {
    //        SensorResult sensorResult = new()
    //        {
    //            CompanyId = customer.CompanyId,
    //            CompanyName = customer.Company,
    //            DeviceId = device.DeviceId,
    //            DeviceName = device.Name,
    //            FirstReadingDtm = (from s in device.SensorData select s).Min(c => c.DateTime),
    //            LastReadingDtm = (from s in device.SensorData select s).Max(c => c.DateTime),
    //            TemperatureCount = (from s in device.SensorData where s.SensorType == "TEMP" select s).Count(),
    //            AverageTemperature = (from s in device.SensorData where s.SensorType == "TEMP" select s).Average(d => d.Value),
    //            HumidityCount = (from s in device.SensorData where s.SensorType == "HUM" select s).Count(),
    //            AverageHumidity = (from s in device.SensorData where s.SensorType == "HUM" select s).Average(d => d.Value)
    //        };

    //        sensorResultList.Add(sensorResult);
    //    }

    //    return sensorResultList;
    //}
}
