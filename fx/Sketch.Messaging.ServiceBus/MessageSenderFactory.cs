using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Sketch.Messaging.ServiceBus
{
    public interface IMessageSenderFactory : IAsyncDisposable, IDisposable
    {
        IMessageSender BuildForTopic(string topic);
    }

    public class MessageSenderFactory : IMessageSenderFactory
    {
        private readonly MessageSenderOptions _options;
        private ServiceBusClient _serviceBusClient;

        public MessageSenderFactory(IOptions<MessageSenderOptions> options)
        {
            _options = options.Value;
            _serviceBusClient = new ServiceBusClient(_options.ServiceBusEndpoint, new DefaultAzureCredential());
        }

        public IMessageSender BuildForTopic(string topic) => new MessageSender(_serviceBusClient.CreateSender(topic));

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(disposing: false);

            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_serviceBusClient is not null)
            {
                await _serviceBusClient.DisposeAsync().ConfigureAwait(false);
            }

            _serviceBusClient = null;
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
                _serviceBusClient = null;
            }
        }
    }
}
