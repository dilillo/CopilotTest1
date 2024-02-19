using Azure.Messaging.ServiceBus;
using CopilotTest1.People.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Sketch.Messaging.ServiceBus;
using System.Text;

namespace CopilotTest1.People.Functions
{
    public class PublishEventsFunction
    {
        private readonly ILogger _logger;
        private readonly IMessageSenderFactory _messagePublisherFactory;
        private readonly PeopleDbContext _dbContext;

        public PublishEventsFunction(
            ILoggerFactory loggerFactory, 
            IMessageSenderFactory messagePublisherFactory, 
            PeopleDbContext dbContext)
        {
            _logger = loggerFactory.CreateLogger<PublishEventsFunction>();
            _messagePublisherFactory = messagePublisherFactory;
            _dbContext = dbContext;
        }

        [Function(nameof(PublishEventsFunction))]
        public async Task Run([TimerTrigger("0 */5 * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"Timer trigger function executed at: {DateTime.Now}");

            var unpublishedEvents = await _dbContext.GetUnpublishedEvents();

            if (unpublishedEvents.Count == 0)
            {
                _logger.LogInformation("No unpublished events found.");

                return;
            }

            var messages = unpublishedEvents.Select(i => new ServiceBusMessage(Encoding.UTF8.GetBytes(i.Payload))
            {
                MessageId = i.Id.ToString(),
                ContentType = i.EventType,
                SessionId = "DefaultSessionId"
            });

            using var messagePublisher = _messagePublisherFactory.BuildForTopic("attendees");

            await messagePublisher.Send(messages);

            unpublishedEvents.ForEach(i => i.IsPublished = true);

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
