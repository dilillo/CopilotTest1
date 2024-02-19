using CopilotTest1.People.Data;
using CopilotTest1.People.Data.Entities;
using CopilotTest1.Shared.Domain.Infrastructure;
using CopilotTest1.Shared.Domain.Owner.Locations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CopilotTest1.People.Functions
{
    public class ProcessPlaceEventsFunction
    {
        private readonly ILogger _logger;
        private readonly PeopleDbContext _dbContext;

        public ProcessPlaceEventsFunction(ILoggerFactory loggerFactory, PeopleDbContext dbContext)
        {
            _logger = loggerFactory.CreateLogger<ProcessPlaceEventsFunction>();
            _dbContext = dbContext;
        }

        [Function("ProcessPlaceEventsFunction")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"Timer trigger function executed at: {DateTime.Now}");

            var unprocessedEvents = await _dbContext.GetUnprocessedPlaceEvents();

            if (unprocessedEvents.Count == 0)
            {
                _logger.LogInformation("No unprocessed events found.");

                return;
            }

            foreach (var unprocessedEvent in unprocessedEvents)
            {
                var domainEventType = Array.Find(typeof(DomainEvent).Assembly.GetTypes(), i => i.Name == unprocessedEvent.EventType);

                if (domainEventType == null)
                {
                    _logger.LogError($"Domain event type not found: {unprocessedEvent.EventType}");

                    continue;
                }

                var domainEvent = (DomainEvent)JsonSerializer.Deserialize(unprocessedEvent.Payload, domainEventType);

                switch (domainEvent)
                {
                    case LocationServiceProvidedEvent locationServiceOfferredEvent:

                        await HandleLocationServiceOfferredEvent(locationServiceOfferredEvent);

                        break;

                    case LocationServiceUnprovidedEvent locationServiceSuspendedEvent:

                        await HandleLocationServiceSuspendedEvent(locationServiceSuspendedEvent);

                        break;
                }
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }

        private async Task HandleLocationServiceSuspendedEvent(LocationServiceUnprovidedEvent locationServiceSuspendedEvent)
        {
            var existingRef = await _dbContext.GetLocationServiceRef(locationServiceSuspendedEvent.AggregateId);

            if (existingRef != null)
            {
                _dbContext.LocationServiceRefs.Remove(existingRef);
            }
        }

        private async Task HandleLocationServiceOfferredEvent(LocationServiceProvidedEvent locationServiceOfferredEvent)
        {
            var existingRef = await _dbContext.GetLocationServiceRef(locationServiceOfferredEvent.AggregateId);

            if (existingRef == null)
            {
                _dbContext.LocationServiceRefs.Add(new LocationServiceRef
                {
                    Id = locationServiceOfferredEvent.AggregateId
                });
            }
        }
    }
}
