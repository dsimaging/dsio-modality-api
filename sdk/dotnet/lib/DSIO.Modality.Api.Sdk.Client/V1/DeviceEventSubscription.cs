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

        // The OnStarted event is sent once when the subscription is started
        public event Action OnStarted = delegate {};

        // The OnStopped event is sent once when the subscription is stopped
        public event Action OnStopped = delegate {};

        // The OnError event is sent along with Exception details when an exception occurs
        public event Action<Exception> OnError = delegate {};

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

                // Fire the OnStarted event
                OnStarted();

                while (!cancelToken.IsCancellationRequested)
                {
                    try
                    {
                        // Wait for the next message
                        var readTask = reader.ReadLineAsync();
                        readTask.Wait(cancelToken);
                        if (readTask.IsFaulted || readTask.IsCanceled)
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
                            if (readTask.IsFaulted || readTask.IsCanceled)
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
                            if (readTask.IsFaulted || readTask.IsCanceled)
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
                    catch (OperationCanceledException)
                    {
                        // Handle the Cancellation exception and squash it. We will simply exit the loop
                        break;
                    }
                    catch(Exception ex)
                    {
                        // Handle other exceptions in this thread and pass along to the client using our error event
                        OnError(ex);
                    }
                }
            }

            // Thread is ending, fire the OnStopped event
            OnStopped();

        }
    }
}