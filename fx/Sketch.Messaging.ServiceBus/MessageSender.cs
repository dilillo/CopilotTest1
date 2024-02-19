using Azure.Messaging.ServiceBus;

namespace Sketch.Messaging.ServiceBus
{
    public interface IMessageSender : IAsyncDisposable, IDisposable
    {
        Task Send(IEnumerable<ServiceBusMessage> messages);
    }

    public class MessageSender : IMessageSender
    {
        private ServiceBusSender _serviceBusSender;

        public MessageSender(ServiceBusSender serviceBusSender)
        {
            _serviceBusSender = serviceBusSender;
        }

        public Task Send(IEnumerable<ServiceBusMessage> messages) => _serviceBusSender.SendMessagesAsync(messages);

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(disposing: false);

            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_serviceBusSender is not null)
            {
                await _serviceBusSender.DisposeAsync().ConfigureAwait(false);
            }

            _serviceBusSender = null;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _serviceBusSender = null;
            }
        }
    }
}
