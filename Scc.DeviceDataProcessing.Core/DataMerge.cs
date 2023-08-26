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
            foreach (Sensor sensor in tracker.Sensors)
            {
                //var tempCrumbs = (from s in tracker.Sensors where s.Name == "Temperature" select s.Crumbs).FirstOrDefault();
                //var humidCrumbs = (from s in tracker.Sensors where s.Name == "Humidty" select s.Crumbs).FirstOrDefault();

                SensorResult sensorResult = new()
                {
                    CompanyId = partner.PartnerId,
                    CompanyName = partner.PartnerName,
                    DeviceId = sensor.Id,
                    DeviceName = sensor.Name,

                    //FirstReadingDtm = new DateTime?(((IEnumerable<Crumb>)sensor.Crumbs).Select<Crumb, DateTime>((Func<Crumb, DateTime>)(c => c.CreatedDtm)).Min<DateTime>()),
                    FirstReadingDtm = (from c in sensor.Crumbs select c).Min(c => c.CreatedDtm),

                    //LastReadingDtm = new DateTime?(((IEnumerable<Crumb>)sensor.Crumbs).Select<Crumb, DateTime>((Func<Crumb, DateTime>)(c => c.CreatedDtm)).Max<DateTime>()),
                    LastReadingDtm = (from c in sensor.Crumbs select c).Max(c => c.CreatedDtm),

                    //TemperatureCount = new int?(source1.Count<Crumb[]>()),
                    TemperatureCount = (from s in tracker.Sensors where s.Name == "Temperature" select s.Crumbs).FirstOrDefault()?.Count(),

                    //IEnumerable<Crumb[]> source1 = ((IEnumerable<Sensor>)tracker.Sensors).Where<Sensor>((Func<Sensor, bool>)(s => s.Name == "Tempurature")).Select<Sensor, Crumb[]>((Func<Sensor, Crumb[]>)(s => s.Crumbs));
                    //Crumb[] source3 = source1.FirstOrDefault<Crumb[]>(),
                    //AverageTemperature = source3 != null ? new double?(((IEnumerable<Crumb>)source3).ToList<Crumb>().Average<Crumb>((Func<Crumb, double>)(c => c.Value))) : new double?(),
                    AverageTemperature = (from s in tracker.Sensors where s.Name == "Temperature" select s.Crumbs)
                        .FirstOrDefault<Crumb[]>()?
                        .ToList()
                        .Average(c => c.Value),

                    //HumidityCount = new int?(source2.Count<Crumb[]>()),
                    HumidityCount = (from s in tracker.Sensors where s.Name == "Humidty" select s.Crumbs).FirstOrDefault()?.Count(),

                    //IEnumerable<Crumb[]> source2 = ((IEnumerable<Sensor>)tracker.Sensors).Where<Sensor>((Func<Sensor, bool>)(s => s.Name == "Humidity")).Select<Sensor, Crumb[]>((Func<Sensor, Crumb[]>)(s => s.Crumbs));
                    //Crumb[] source4 = source2.FirstOrDefault<Crumb[]>(),
                    //AverageHumdity = source4 != null ? new double?(((IEnumerable<Crumb>)source4).ToList<Crumb>().Average<Crumb>((Func<Crumb, double>)(c => c.Value))) : new double?(),
                    AverageHumidity = (from s in tracker.Sensors where s.Name == "Humidty" select s.Crumbs)
                        .FirstOrDefault<Crumb[]>()?
                        .ToList()
                        .Average(c => c.Value),
                };

                sensorResultList.Add(sensorResult);
            }
        }

        foreach (Device device in customer!.Devices)
        {
            SensorResult sensorResult = new()
            {
                CompanyId = customer.CompanyId,
                CompanyName = customer.Company,
                DeviceId = device.DeviceId,
                DeviceName = device.Name,

                //sensorResult3.FirstReadingDtm = new DateTime?(((IEnumerable<SensorData>)device.SensorData).Select<SensorData, DateTime>((Func<SensorData, DateTime>)(c => c.DateTime)).Min<DateTime>());
                FirstReadingDtm = (from s in device.SensorData select s).Min(c => c.DateTime),

                //sensorResult3.LastReadingDtm = new DateTime?(((IEnumerable<SensorData>)device.SensorData).Select<SensorData, DateTime>((Func<SensorData, DateTime>)(c => c.DateTime)).Max<DateTime>());
                LastReadingDtm = (from s in device.SensorData select s).Max(c => c.DateTime),

                //sensorResult3.TemperatureCount = new int?(((IEnumerable<SensorData>)device.SensorData).Where<SensorData>((Func<SensorData, bool>)(d => d.SensorType == "TEMP")).Count<SensorData>());
                TemperatureCount = (from s in device.SensorData where s.SensorType == "TEMP" select s).Count(),

                //IEnumerable<SensorData> source5 = ((IEnumerable<SensorData>)device.SensorData).Where<SensorData>((Func<SensorData, bool>)(d => d.SensorType == "TEMP"));
                //AverageTemperature = source5 != null ? new double?(source5.ToList<SensorData>().Average<SensorData>((Func<SensorData, double>)(d => d.Value))) : new double?();
                AverageTemperature = (from s in device.SensorData where s.SensorType == "TEMP" select s).Average(d => d.Value),

                //sensorResult3.HumidityCount = new int?(((IEnumerable<SensorData>)device.SensorData).Where<SensorData>((Func<SensorData, bool>)(d => d.SensorType == "HUM")).Count<SensorData>());
                HumidityCount = (from s in device.SensorData where s.SensorType == "HUM" select s).Count(),

                //IEnumerable<SensorData> source6 = ((IEnumerable<SensorData>)device.SensorData).Where<SensorData>((Func<SensorData, bool>)(d => d.SensorType == "HUM"));
                //AverageHumdity = source6 != null ? new double?(source6.ToList<SensorData>().Average<SensorData>((Func<SensorData, double>)(d => d.Value))) : new double?();
                AverageHumidity = (from s in device.SensorData where s.SensorType == "HUM" select s).Average(d => d.Value)
            };

            sensorResultList.Add(sensorResult);
        }

        return sensorResultList;
    }
}
