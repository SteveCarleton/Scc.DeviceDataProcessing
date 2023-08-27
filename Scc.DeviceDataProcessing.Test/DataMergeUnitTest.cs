using FluentAssertions;
using Scc.DeviceDataProcessing.Core;
using Scc.DeviceDataProcessing.DataModels.Input.Foo1;
using Scc.DeviceDataProcessing.DataModels.Input.Foo2;
using Scc.DeviceDataProcessing.DataModels.Output;

namespace Scc.DeviceDataProcessing.UnitTest;

public class DataMergeUnitTest
{
    readonly static string DateFormat = "MM-dd-yyyy HH:mm:ss";

    readonly Partner testPartner = new()
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

    readonly Customer testCustomer = new()
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



    [Fact]
    public void TestDataMerge()
    {
        DataProcessing proc = new();

        // Act

        List<SensorResult> sensorResultList = proc.MergeDeviceData(testPartner, testCustomer);

        // Assert

        sensorResultList.Should().NotBeNullOrEmpty();
        sensorResultList.Count.Should().Be(testPartner.Trackers.Length + testCustomer.Devices.Length);

        // Foo1

        for (int index = 0; index < testPartner.Trackers.Length; index++)
        {
            sensorResultList[index].CompanyId.Should().Be(testPartner.PartnerId);
            sensorResultList[index].CompanyName.Should().Be(testPartner.PartnerName);

            Sensor sensor = testPartner.Trackers[index].Sensors[0];
            sensorResultList[index].DeviceId.Should().Be(testPartner.Trackers[index].Id);
            sensorResultList[index].DeviceName.Should().Be(testPartner.Trackers[index].Model);
            sensorResultList[index].FirstReadingDtm.Should().Be(sensor.Crumbs[0].CreatedDtm);
            sensorResultList[index].LastReadingDtm.Should().Be(sensor.Crumbs[2].CreatedDtm);
            sensorResultList[index].TemperatureCount.Should().Be(sensor.Crumbs.Length);

            sensorResultList[index].AverageTemperature.Should().Be(
                (from c in sensor.Crumbs select c).Sum(c => c.Value) / sensor.Crumbs.Length);

            sensor = testPartner.Trackers[index].Sensors[1];
            sensorResultList[index].HumidityCount.Should().Be(sensor.Crumbs.Length);

            sensorResultList[index].AverageHumidity.Should().Be(
                (from c in sensor.Crumbs select c).Sum(c => c.Value) / sensor.Crumbs.Length);
        }

        // Foo2

        for (int index = testPartner.Trackers.Length; index < testPartner.Trackers.Length + testCustomer.Devices.Length; index++)
        {
            sensorResultList[index].CompanyId.Should().Be(testCustomer.CompanyId);
            sensorResultList[index].CompanyName.Should().Be(testCustomer.Company);

            Device device = testCustomer.Devices[index - testPartner.Trackers.Length];
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

    [Fact]
    public void TestDeserializeDeviceDataFoo1()
    {
        // Arrange

        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeviceDataFoo1.json");
        JsonProcessing jsonProc = new();

        // Act
        Partner? actPartner = jsonProc.Deserialize<Partner>(filePath);

        // Assert

        actPartner?.PartnerId.Should().Be(testPartner.PartnerId);
        actPartner?.PartnerName.Should().Be(testPartner.PartnerName);
        actPartner?.Trackers.Length.Should().Be(testPartner?.Trackers.Length);

        for (int trackerIndex = 0; trackerIndex < actPartner?.Trackers.Length; trackerIndex++)
        {
            Tracker? actTracker = actPartner?.Trackers[trackerIndex];
            Tracker? expTracker = testPartner?.Trackers[trackerIndex];
            actTracker?.Id.Should().Be(expTracker?.Id);
            actTracker?.Model.Should().Be(expTracker?.Model);
            actTracker?.ShipmentStartDtm.Should().Be(expTracker?.ShipmentStartDtm);
            actTracker?.Sensors.Length.Should().Be(expTracker?.Sensors.Length);

            for (int sensorIndex = 0; sensorIndex < actPartner?.Trackers[0]?.Sensors!.Length; sensorIndex++)
            {
                Sensor? actSensor = actPartner?.Trackers[0]?.Sensors[sensorIndex];
                Sensor? expSensor = testPartner?.Trackers[0]?.Sensors[sensorIndex];
                actSensor?.Id.Should().Be(expSensor?.Id);
                actSensor?.Name.Should().Be(expSensor?.Name);

                for (int crumbIndex = 0; crumbIndex < actSensor?.Crumbs!.Length; crumbIndex++)
                {
                    Crumb actCrumb = actSensor.Crumbs![crumbIndex];
                    Crumb? expCrumb = expSensor?.Crumbs![crumbIndex];
                    actCrumb.CreatedDtm.Should().Be(expCrumb?.CreatedDtm);
                    actCrumb.Value.Should().Be(expCrumb?.Value);
                }
            }
        }
    }

    [Fact]
    public void TestDeserializeDeviceDataFoo2()
    {
        // Arrange

        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeviceDataFoo2.json");
        JsonProcessing jsonProc = new();

        // Act
        Customer? actCustomer = jsonProc.Deserialize<Customer>(filePath);

        // Assert

        actCustomer.Should().NotBeNull();
        actCustomer?.Company.Should().Be(testCustomer.Company);
        actCustomer?.CompanyId.Should().Be(testCustomer.CompanyId);
        actCustomer?.Devices.Length.Should().Be(testCustomer?.Devices.Length);

        for (int deviceIndex = 0; deviceIndex < actCustomer?.Devices.Length; deviceIndex++)
        {
            Device? actDevice = actCustomer?.Devices[deviceIndex];
            Device? expDevice = testCustomer?.Devices[deviceIndex];
            actDevice?.DeviceID.Should().Be(expDevice?.DeviceID);
            actDevice?.Name.Should().Be(expDevice?.Name);
            actDevice?.StartDateTime.Should().Be(expDevice?.StartDateTime);

            for (int sensorDataIndex = 0; sensorDataIndex < actDevice?.SensorData.Length; sensorDataIndex++)
            {
                SensorData actSenorData = actDevice.SensorData[sensorDataIndex];
                SensorData? expSenorData = expDevice?.SensorData[sensorDataIndex];
                actSenorData.SensorType.Should().Be(expSenorData?.SensorType);
                actSenorData.DateTime.Should().Be(expSenorData?.DateTime);
                actSenorData.Value.Should().Be(expSenorData?.Value);
            }
        }
    }

    [Fact]
    public void TestSerializeDeviceMergeData()
    {
        // Arrange

        const string expJson = "[\r\n  {\r\n    \"CompanyId\": 1,\r\n    \"CompanyName\": \"Foo1\",\r\n    \"DeviceId\": 1,\r\n    \"DeviceName\": \"ABC-100\",\r\n    \"FirstReadingDtm\": \"08-17-2020 10:35:00\",\r\n    \"LastReadingDtm\": \"08-17-2020 10:45:00\",\r\n    \"TemperatureCount\": 3,\r\n    \"AverageTemperature\": 23.149999999999995,\r\n    \"HumidityCount\": 3,\r\n    \"AverageHumidity\": 81.5\r\n  },\r\n  {\r\n    \"CompanyId\": 1,\r\n    \"CompanyName\": \"Foo1\",\r\n    \"DeviceId\": 2,\r\n    \"DeviceName\": \"ABC-200\",\r\n    \"FirstReadingDtm\": \"08-18-2020 10:35:00\",\r\n    \"LastReadingDtm\": \"08-18-2020 10:45:00\",\r\n    \"TemperatureCount\": 3,\r\n    \"AverageTemperature\": 24.149999999999995,\r\n    \"HumidityCount\": 3,\r\n    \"AverageHumidity\": 82.5\r\n  },\r\n  {\r\n    \"CompanyId\": 2,\r\n    \"CompanyName\": \"Foo2\",\r\n    \"DeviceId\": 1,\r\n    \"DeviceName\": \"XYZ-100\",\r\n    \"FirstReadingDtm\": \"08-18-2020 10:35:00\",\r\n    \"LastReadingDtm\": \"08-18-2020 10:45:00\",\r\n    \"TemperatureCount\": 3,\r\n    \"AverageTemperature\": 33.15,\r\n    \"HumidityCount\": 3,\r\n    \"AverageHumidity\": 91.5\r\n  },\r\n  {\r\n    \"CompanyId\": 2,\r\n    \"CompanyName\": \"Foo2\",\r\n    \"DeviceId\": 2,\r\n    \"DeviceName\": \"XYZ-200\",\r\n    \"FirstReadingDtm\": \"08-19-2020 10:35:00\",\r\n    \"LastReadingDtm\": \"08-19-2020 10:45:00\",\r\n    \"TemperatureCount\": 3,\r\n    \"AverageTemperature\": 43.15,\r\n    \"HumidityCount\": 3,\r\n    \"AverageHumidity\": 92.5\r\n  }\r\n]";

        List<SensorResult> testSensorResultList = new()
        {
            new SensorResult
            {
                CompanyId = 1,
                CompanyName = "Foo1",
                DeviceId = 1,
                DeviceName = "ABC-100",
                FirstReadingDtm = DateTime.ParseExact("08-17-2020 10:35:00", DateFormat, null),
                LastReadingDtm = DateTime.ParseExact("08-17-2020 10:45:00", DateFormat, null),
                TemperatureCount = 3,
                AverageTemperature = 23.149999999999995,
                HumidityCount = 3,
                AverageHumidity = 81.5
            },
            new SensorResult
            {
                CompanyId = 1,
                CompanyName = "Foo1",
                DeviceId = 2,
                DeviceName = "ABC-200",
                FirstReadingDtm = DateTime.ParseExact("08-18-2020 10:35:00", DateFormat, null),
                LastReadingDtm = DateTime.ParseExact("08-18-2020 10:45:00", DateFormat, null),
                TemperatureCount = 3,
                AverageTemperature = 24.149999999999995,
                HumidityCount = 3,
                AverageHumidity = 82.5
            },
            new SensorResult
            {
                CompanyId = 2,
                CompanyName = "Foo2",
                DeviceId = 1,
                DeviceName = "XYZ-100",
                FirstReadingDtm = DateTime.ParseExact("08-18-2020 10:35:00", DateFormat, null),
                LastReadingDtm = DateTime.ParseExact("08-18-2020 10:45:00", DateFormat, null),
                TemperatureCount = 3,
                AverageTemperature = 33.15,
                HumidityCount = 3,
                AverageHumidity = 91.5
            },
            new SensorResult
            {
                CompanyId = 2,
                CompanyName = "Foo2",
                DeviceId = 2,
                DeviceName = "XYZ-200",
                FirstReadingDtm = DateTime.ParseExact("08-19-2020 10:35:00", DateFormat, null),
                LastReadingDtm = DateTime.ParseExact("08-19-2020 10:45:00", DateFormat, null),
                TemperatureCount = 3,
                AverageTemperature = 43.15,
                HumidityCount = 3,
                AverageHumidity = 92.5
            },
        };

        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MergedDeviceData.json");
        JsonProcessing jsonProc = new();

        // Act

        jsonProc.Serialize(filePath, testSensorResultList);

        // Assert

        string actJson = File.ReadAllText(filePath);
        actJson.Should().NotBeNullOrEmpty();
        actJson.Should().Be(expJson);
    }
}