using Azure.Messaging.ServiceBus;
using CopilotTest1.People.Data;
using CopilotTest1.People.Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CopilotTest1.People.Functions
{
    public class ReceivePlaceEventsFunction
    {
        private readonly ILogger<ReceivePlaceEventsFunction> _logger;
        private readonly PeopleDbContext _dbContext;

        public ReceivePlaceEventsFunction(ILogger<ReceivePlaceEventsFunction> logger, PeopleDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [Function(nameof(ReceivePlaceEventsFunction))]
        public async Task Run([ServiceBusTrigger("mytopic", "mysubscription", Connection = "PlacesSubscription")] ServiceBusReceivedMessage message)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            var eventExists = await _dbContext.GetPlaceEventExists(Guid.Parse(message.MessageId));

            if (eventExists)
            {
                _logger.LogInformation("Event already exists. Skipping.");

                return;
            }

            _dbContext.PlaceEvents.Add(new PlaceEvent
            {
                Id = Guid.Parse(message.MessageId),
                EventType = message.ContentType,
                Timestamp = message.ScheduledEnqueueTime,
                Payload = Encoding.UTF8.GetString(message.Body)
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}
