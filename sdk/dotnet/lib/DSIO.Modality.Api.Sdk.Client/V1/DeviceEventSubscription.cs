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
        private readonly Stream _stream;
        private readonly Action<DeviceEventData> _onDeviceEvent;
        private CancellationTokenSource _cancellationTokenSource;
        private Thread _messagesThread;

        public DeviceEventSubscription(Stream stream, Action<DeviceEventData> onDeviceEvent)
        {
            this._stream = stream;
            this._onDeviceEvent = onDeviceEvent;
        }

        public event Action<Heartbeat> OnHeartbeat = delegate {};

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _messagesThread = new Thread(new ThreadStart(WaitForMessages)) { IsBackground = true };
            _messagesThread.Start();
        }

        public void Stop()
        {
            this._cancellationTokenSource?.Cancel();
            this._messagesThread.Join(200);
            this._stream.Close();
        }

        private void WaitForMessages()
        {
            using (var reader = new StreamReader(_stream))
            {
                var cancelToken = _cancellationTokenSource.Token;
                cancelToken.ThrowIfCancellationRequested();
                while (!reader.EndOfStream && !cancelToken.IsCancellationRequested)
                {
                    // Wait for the next message
                    var readTask = reader.ReadLineAsync();
                    readTask.Wait(cancelToken);
                    if (!readTask.IsCompleted)
                        break;

                    // Extract the message we just read
                    var message = readTask.Result;
                    if (string.IsNullOrEmpty(message))
                        continue;   // we should not get an empty message

                    // Process this line
                    if (message.StartsWith("event: message"))
                    {
                        // DeviceEventData follows
                        readTask = reader.ReadLineAsync();
                        readTask.Wait(cancelToken);
                        if (!readTask.IsCompleted)
                            break;

                        message = readTask.Result;
                        var jsonPosition = message?.IndexOf("data: ", StringComparison.InvariantCultureIgnoreCase);
                        if (jsonPosition >= 0)
                        {
                            var length = "data: ".Length;
                            var json = message.Substring(jsonPosition.Value + length);
                            var data = JsonConvert.DeserializeObject<DeviceEventData>(json);
                            _onDeviceEvent(data);
                        }
                    }
                    else if (message.StartsWith("event: heartbeat"))
                    {
                        // Heartbeat follows
                        readTask = reader.ReadLineAsync();
                        readTask.Wait(cancelToken);
                        if (!readTask.IsCompleted)
                            break;

                        message = readTask.Result;
                        var jsonPosition = message?.IndexOf("data: ", StringComparison.InvariantCultureIgnoreCase);
                        if (jsonPosition >= 0)
                        {
                            var length = "data: ".Length;
                            var json = message.Substring(jsonPosition.Value + length);
                            var data = JsonConvert.DeserializeObject<Heartbeat>(json);
                            OnHeartbeat(data);
                        }
                    }
                }
            }
        }
    }
}