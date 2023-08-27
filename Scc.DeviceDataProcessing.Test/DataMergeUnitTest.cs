using FluentAssertions;
using Scc.DeviceDataProcessing.Core;
using Scc.DeviceDataProcessing.DataModels.Input.Foo1;
using Scc.DeviceDataProcessing.DataModels.Input.Foo2;
using Scc.DeviceDataProcessing.DataModels.Output;

namespace Scc.DeviceDataProcessing.UnitTest;

public class DataMergeUnitTest
{
    [Fact]
    public void TestDataMerge()
    {
        const string DateFormat = "MM-dd-yyyy HH:mm:ss";

        // Arrange
        Partner partner = new()
        {
            PartnerId = 1,
            PartnerName = "Foo1",
            Trackers = new[]
            {
                new Tracker
                {
                    Id = 1,
                    Model = "ABC-100",
                    ShipmentStartDtm = DateTime.ParseExact("08-17-2020 10:30:00", DateFormat, null),
                    Sensors = new[]
                    {
                        new Sensor
                        {
                            Id = 100,
                            Name = "Temperature",
                            Crumbs = new[]
                            {
                                new Crumb { CreatedDtm = DateTime.ParseExact("08-17-2020 10:35:00", DateFormat, null), Value = 22.15 },
                                new Crumb { CreatedDtm = DateTime.ParseExact("08-17-2020 10:40:00", DateFormat, null), Value = 23.15 },
                                new Crumb { CreatedDtm = DateTime.ParseExact("08-17-2020 10:45:00", DateFormat, null), Value = 24.15 }
                            }
                        },
                        new Sensor
                        {
                            Id = 101,
                            Name = "Humidty",
                            Crumbs = new[]
                            {
                                new Crumb { CreatedDtm = DateTime.ParseExact("08-17-2020 10:35:00", DateFormat, null), Value = 80.5 },
                                new Crumb { CreatedDtm = DateTime.ParseExact("08-17-2020 10:40:00", DateFormat, null), Value = 81.5 },
                                new Crumb { CreatedDtm = DateTime.ParseExact("08-17-2020 10:45:00", DateFormat, null), Value = 82.5 }
                            }
                        }
                    }
                },
                new Tracker
                {
                    Id = 2,
                    Model = "ABC-200",
                    ShipmentStartDtm = DateTime.ParseExact("08-18-2020 10:30:00", DateFormat, null),
                    Sensors = new[]
                    {
                        new Sensor
                        {
                            Id = 200,
                            Name = "Temperature",
                            Crumbs = new[]
                            {
                                new Crumb { CreatedDtm = DateTime.ParseExact("08-18-2020 10:35:00", DateFormat, null), Value = 23.15 },
                                new Crumb { CreatedDtm = DateTime.ParseExact("08-18-2020 10:40:00", DateFormat, null), Value = 24.15 },
                                new Crumb { CreatedDtm = DateTime.ParseExact("08-18-2020 10:45:00", DateFormat, null), Value = 25.15 }
                            }
                        },
                        new Sensor
                        {
                            Id = 201,
                            Name = "Humidty",
                            Crumbs = new[]
                            {
                                new Crumb { CreatedDtm = DateTime.ParseExact("08-18-2020 10:35:00", DateFormat, null), Value = 81.5 },
                                new Crumb { CreatedDtm = DateTime.ParseExact("08-17-2020 10:40:00", DateFormat, null), Value = 82.5 },
                                new Crumb { CreatedDtm = DateTime.ParseExact("08-18-2020 10:45:00", DateFormat, null), Value = 83.5 }
                            }
                        }
                    }
                }
            }
        };

        Customer customer = new()
        {
            CompanyId = 2,
            Company = "Foo2",
            Devices = new[]
            {
                new Device
                {
                    DeviceID = 1,
                    Name = "XYZ-100",
                    StartDateTime = DateTime.ParseExact("08-18-2020 10:30:00", DateFormat, null),
                    SensorData  = new []
                    {
                        new SensorData 
                        {
                            SensorType = "TEMP",
                            DateTime = DateTime.ParseExact("08-18-2020 10:35:00", DateFormat, null),
                            Value = 32.15
                        },
                        new SensorData
                        {
                            SensorType = "TEMP",
                            DateTime = DateTime.ParseExact("08-18-2020 10:40:00", DateFormat, null),
                            Value = 33.15
                        },
                        new SensorData
                        {
                            SensorType = "TEMP",
                            DateTime = DateTime.ParseExact("08-18-2020 10:45:00", DateFormat, null),
                            Value = 34.15
                        },
                        new SensorData
                        {
                            SensorType = "HUM",
                            DateTime = DateTime.ParseExact("08-18-2020 10:35:00", DateFormat, null),
                            Value = 90.5
                        },
                        new SensorData
                        {
                            SensorType = "HUM",
                            DateTime = DateTime.ParseExact("08-18-2020 10:40:00", DateFormat, null),
                            Value = 91.5
                        },
                        new SensorData
                        {
                            SensorType = "HUM",
                            DateTime = DateTime.ParseExact("08-18-2020 10:45:00", DateFormat, null),
                            Value = 92.5
                        }
                    }
                },
                new Device
                {
                    DeviceID = 2,
                    Name = "XYZ-200",
                    StartDateTime = DateTime.ParseExact("08-19-2020 10:30:00", DateFormat, null),
                    SensorData  = new []
                    {
                        new SensorData
                        {
                            SensorType = "TEMP",
                            DateTime = DateTime.ParseExact("08-19-2020 10:35:00", DateFormat, null),
                            Value = 42.15
                        },
                        new SensorData
                        {
                            SensorType = "TEMP",
                            DateTime = DateTime.ParseExact("08-19-2020 10:40:00", DateFormat, null),
                            Value = 43.15
                        },
                        new SensorData
                        {
                            SensorType = "TEMP",
                            DateTime = DateTime.ParseExact("08-19-2020 10:45:00", DateFormat, null),
                            Value = 44.15
                        },
                        new SensorData
                        {
                            SensorType = "HUM",
                            DateTime = DateTime.ParseExact("08-19-2020 10:35:00", DateFormat, null),
                            Value = 91.5
                        },
                        new SensorData
                        {
                            SensorType = "HUM",
                            DateTime = DateTime.ParseExact("08-19-2020 10:40:00", DateFormat, null),
                            Value = 92.5
                        },
                        new SensorData
                        {
                            SensorType = "HUM",
                            DateTime = DateTime.ParseExact("08-19-2020 10:45:00", DateFormat, null),
                            Value = 93.5
                        }
                    }
                }
            }
        };

        DataProcessing proc = new();

        // Act

        List<SensorResult> sensorResultList = proc.MergeDeviceData(partner, customer);

        // Assert

        sensorResultList.Should().NotBeNullOrEmpty();
        sensorResultList.Count.Should().Be(partner.Trackers.Length + customer.Devices.Length);

        // Foo1

        for (int index = 0; index < partner.Trackers.Length; index++)
        {
            sensorResultList[index].CompanyId.Should().Be(partner.PartnerId);
            sensorResultList[index].CompanyName.Should().Be(partner.PartnerName);

            Sensor sensor = partner.Trackers[index].Sensors[0];
            sensorResultList[index].DeviceId.Should().Be(partner.Trackers[index].Id);
            sensorResultList[index].DeviceName.Should().Be(partner.Trackers[index].Model);
            sensorResultList[index].FirstReadingDtm.Should().Be(sensor.Crumbs[0].CreatedDtm);
            sensorResultList[index].LastReadingDtm.Should().Be(sensor.Crumbs[2].CreatedDtm);
            sensorResultList[index].TemperatureCount.Should().Be(sensor.Crumbs.Length);

            sensorResultList[index].AverageTemperature.Should().Be(
                (from c in sensor.Crumbs select c).Sum(c => c.Value) / sensor.Crumbs.Length);

            sensor = partner.Trackers[index].Sensors[1];
            sensorResultList[index].HumidityCount.Should().Be(sensor.Crumbs.Length);

            sensorResultList[index].AverageHumidity.Should().Be(
                (from c in sensor.Crumbs select c).Sum(c => c.Value) / sensor.Crumbs.Length);
        }

        // Foo2

        for (int index = partner.Trackers.Length; index < partner.Trackers.Length + customer.Devices.Length; index++)
        {
            sensorResultList[index].CompanyId.Should().Be(customer.CompanyId);
            sensorResultList[index].CompanyName.Should().Be(customer.Company);

            Device device = customer.Devices[index - partner.Trackers.Length];
            sensorResultList[index].DeviceId.Should().Be(device.DeviceID);
            sensorResultList[index].DeviceName.Should().Be(device.Name);
            sensorResultList[index].FirstReadingDtm.Should().Be(device.SensorData[0].DateTime);
            sensorResultList[index].LastReadingDtm.Should().Be(device.SensorData[2].DateTime);
            sensorResultList[index].TemperatureCount.Should().Be(device.SensorData.Length / 2);

            sensorResultList[index].AverageTemperature.Should().Be(
                (from s in device.SensorData where s.SensorType == "TEMP" select s).Sum(s => s.Value) / (device.SensorData.Length / 2));

            sensorResultList[index].HumidityCount.Should().Be(device.SensorData.Length / 2);

            sensorResultList[index].AverageHumidity.Should().Be(
                (from s in device.SensorData where s.SensorType == "HUM" select s).Sum(s => s.Value) / (device.SensorData.Length / 2));
        }
    }
}
