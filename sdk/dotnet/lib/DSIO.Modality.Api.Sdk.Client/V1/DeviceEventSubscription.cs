using System.Threading.Tasks;
using System;
using System.IO;
using System.Threading;

using Newtonsoft.Json;

using DSIO.Modality.Api.Sdk.Types.V1;

namespace DSIO.Modality.Api.Sdk.Client.V1
{
    public class DeviceEventSubscription : ISubscription
    {
        private Stream _stream;
        private Action<DeviceEventData> _onDeviceEvent;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _messagesTask;

        public DeviceEventSubscription(Stream stream, Action<DeviceEventData> onDeviceEvent)
        {
            this._stream = stream;
            this._onDeviceEvent = onDeviceEvent;
        }

        public event Action<Heartbeat> OnHeartbeat = delegate {};

        public void Start()
        {
            Stop();

            _cancellationTokenSource = new CancellationTokenSource();
            _messagesTask = Task.Factory.StartNew(WaitForMessages);
        }

        public void Stop()
        {
            this._cancellationTokenSource?.Cancel();
        }

        private async Task WaitForMessages()
        {
            using (var reader = new StreamReader(_stream))
            {
                var cancelToken = _cancellationTokenSource.Token;
                cancelToken.ThrowIfCancellationRequested();
                while (!reader.EndOfStream && !cancelToken.IsCancellationRequested)
                {
                    var message = await reader.ReadLineAsync();

                    // Process this line
                    if (message.StartsWith("event: message"))
                    {
                        // DeviceEventData follows
                        message = await reader.ReadLineAsync();
                        int jsonPosition = message.IndexOf("data: ");
                        if (jsonPosition >= 0)
                        {
                            var length = "data: ".Length;
                            var json = message.Substring(jsonPosition + length);
                            var data = JsonConvert.DeserializeObject<DeviceEventData>(json);
                            _onDeviceEvent(data);
                        }
                    }
                    else if (message.StartsWith("event: heartbeat"))
                    {
                        // Heartbeat follows
                        message = await reader.ReadLineAsync();
                        int jsonPosition = message.IndexOf("data: ");
                        if (jsonPosition >= 0)
                        {
                            var length = "data: ".Length;
                            var json = message.Substring(jsonPosition + length);
                            var data = JsonConvert.DeserializeObject<Heartbeat>(json);
                            OnHeartbeat(data);
                        }
                    }
                }
            }
        }
    }
}