//using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Scc.DeviceDataProcessing.DataModels.Input.Foo1;
using Scc.DeviceDataProcessing.DataModels.Input.Foo2;
using Scc.DeviceDataProcessing.DataModels.Output;

namespace Scc.DeviceDataProcessing.Core;

public class Application
{
    readonly ILogger log;
    readonly JsonProcessing jsonProcessing;
    readonly DataProcessing dataProcessing;

    public Application(ILogger<Application> logger, DataProcessing dataProc, JsonProcessing jsonProc)
    {
        log = logger;
        jsonProcessing = jsonProc;
        dataProcessing = dataProc;
    }

    public void Merge(string inputFilename1, string inputFilename2, string outputFilename)
    {
        Partner? partner = jsonProcessing.Deserialize<Partner>(inputFilename1);
        Customer? customer = jsonProcessing.Deserialize<Customer>(inputFilename2);

        List<SensorResult> sensorResultList = dataProcessing.MergeDeviceData(partner, customer);

        jsonProcessing.Serialize(outputFilename, sensorResultList);
    }
}