using System.Threading.Tasks;
using System;
using System.IO;
using System.Threading;

using Newtonsoft.Json;

using DSIO.Modality.Api.Sdk.Types.V1;

namespace DSIO.Modality.Api.Sdk.Client.V1
{
    public class AcquisitionStatusSubscription : ISubscription
    {
        private Stream _stream;
        private Action<AcquisitionStatus> _onStatus;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _messagesTask;

        public AcquisitionStatusSubscription(Stream stream, Action<AcquisitionStatus> onStatus)
        {
            this._stream = stream;
            this._onStatus = onStatus;
        }

        public event Action<Heartbeat> OnHeartbeat = delegate {};

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _messagesTask = Task.Factory.StartNew(WaitForMessages);
        }

        public void Stop()
        {
            this._cancellationTokenSource?.Cancel();
            this._stream.Close();
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
                        // Acquisition Status follows
                        message = await reader.ReadLineAsync();
                        int jsonPosition = message.IndexOf("data: ");
                        if (jsonPosition >= 0)
                        {
                            var length = "data: ".Length;
                            var json = message.Substring(jsonPosition + length);
                            var data = JsonConvert.DeserializeObject<AcquisitionStatus>(json);
                            _onStatus(data);
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