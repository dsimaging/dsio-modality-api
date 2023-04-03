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

            // ServiceProxy HttpClient calls will throw exceptions when
            // unsuccessful. Handle exceptions and show errors
            try
            {
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

                    // retrieve a list of active sessions
                    var sessions = (await service.GetAcquisitionSessions())?.ToList();
                    if (sessions != null)
                    {
                        Console.WriteLine($"Found {sessions.Count} active sessions");
                        foreach (var session in sessions)
                        {
                            Console.WriteLine($"Session {session.SessionId} is using Device {session.DeviceId}");
                        }
                    }

                    // Subscribe to the Device Events
                    Console.WriteLine("Subscribing to device events...");
                    var subscription = await service.SubscribeToDeviceEvents(data =>
                    {
                        Console.WriteLine("\nSubscription callback");
                        Console.WriteLine($"\tAction:      {data.Action}");
                        Console.WriteLine($"\tDevice Id:   {data.DeviceInfo.DeviceId}");
                        Console.WriteLine($"\tDevice Name: {data.DeviceInfo.Name}");
                    });

                    // Optionally listen to Heartbeats
                    subscription.OnHeartbeat += (data =>
                    {
                        Console.WriteLine($"\nHeartbeat timeout: {data.HeartbeatTimeout}ms");
                    });

                    subscription.OnStarted += () => {
                        Console.WriteLine("\nDevice Event Subscription started");
                    };

                    subscription.OnStopped += () => {
                        Console.WriteLine("\nDevice Event Subscription stopped");
                    };

                    subscription.OnError += (ex) => {
                        Console.WriteLine($"\nDevice Event Subscription encountered an error\n{ex}");
                    };

                    // Start listening to events
                    subscription.Start();

                    // We are now listening for changes in the Device list. Try
                    // changing the connected sensor of the Simulator to see examples
                    // of event data sent to this client.
                    Console.WriteLine("Press Enter to stop subscription...");
                    Console.ReadLine();
                    subscription.Stop();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception encountered: {ex.Message}");
            }

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }
    }
}
