using System.Threading;
using System.Net.Security;
using System.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSIO.Modality.Api.Sdk.Client.V1;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // API Keys are long, modify Console to get around 254 char limit
            Console.SetIn(new StreamReader(Console.OpenStandardInput(8192)));

            Console.WriteLine("Sample console app for Modality Api");

            // Read credentials
            Console.Write("Enter your API Username: ");
            var username = Console.ReadLine();
            Console.Write("Enter your API Key: ");
            var apikey = Console.ReadLine();

            // Create service proxy and set credentials
            var service = new ServiceProxy();
            service.SetBasicAuthenticationHeader(username, apikey);

            // Test availability
            Console.WriteLine("Checking availability of service...");
            var isAvailable = await service.IsServiceAvailable();
            Console.WriteLine($"Modality Api V1 service isAvailable: {isAvailable}");

            if (isAvailable)
            {
                // Retrieve all devices
                Console.WriteLine("Retrieving device list...");
                var devices = (await service.GetAllDevices())?.ToList();
                if (devices != null)
                {
                    Console.WriteLine($"Found {devices.Count} devices");
                    foreach (var device in devices)
                    {
                        Console.WriteLine($"Device with name '{device.Name}' and id {device.DeviceId} is {device.Status}");
                        // get the sensor info
                        var sensorInfo = await service.GetSensor(device.DeviceId);
                        if (sensorInfo != null)
                        {
                            Console.WriteLine($"\tSensor {sensorInfo.ModelName} attached with Serial Number {sensorInfo.SerialNumber}");
                        }
                    }
                }
            }

            Console.WriteLine("Subscribing to device events...");
            var subscription = await service.SubscribeToDeviceEvents(data =>
            {
                Console.WriteLine("Subscription callback");
                Console.WriteLine($"\tAction:      {data.Action}");
                Console.WriteLine($"\tDevice Id:   {data.DeviceInfo.DeviceId}");
                Console.WriteLine($"\tDevice Name: {data.DeviceInfo.Name}");
            });

            // Start listening to events
            subscription.Start();

            Console.WriteLine("Press Enter to stop subscription...");
            Console.ReadLine();
            subscription.Stop();

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }
    }
}
