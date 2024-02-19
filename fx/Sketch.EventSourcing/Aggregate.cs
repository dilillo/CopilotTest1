using Orleans;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;
using Sketch.EventSourcing.Data;
using Sketch.EventSourcing.Data.Entities;
using System.Text.Json;

namespace Sketch.EventSourcing
{
    public abstract class Aggregate<TGrainState, TDbContext> : JournaledGrain<TGrainState, Event>, ICustomStorageInterface<TGrainState, Event>
        where TGrainState : class, new()
        where TDbContext : AggregateDbContext
    {
        public Aggregate(TDbContext context)
        {
            DbContext = context;
        }

        public TDbContext DbContext { get; }

        protected void RaiseDomainEvent<TEventBase>() where TEventBase : Event, new() => RaiseDomainEvent<TEventBase>((@event) => { });

        protected void RaiseDomainEvent<TEventBase>(Action<TEventBase> configurator) where TEventBase : Event, new()
        {
            var @event = new TEventBase()
            {
                Id = Guid.NewGuid(),
                AggregateId = this.GetPrimaryKey(),
                OccurredOn = DateTimeOffset.UtcNow
            };

            configurator(@event);

            RaiseEvent(@event);
        }

        public async Task<KeyValuePair<int, TGrainState>> ReadStateFromStorage()
        {
            var snapshot = await DbContext.GetSnapshotByAggregateId(this.GetPrimaryKey()) ?? new Snapshot
            {
                AggregateId = this.GetPrimaryKey(),
                Version = 0,
                Payload = JsonSerializer.Serialize(new TGrainState())
            };

            var state = JsonSerializer.Deserialize<TGrainState>(snapshot.Payload);

            var newerEventData = await DbContext.GetEventsByAggregateId(snapshot.AggregateId, snapshot.Version);
            var newerEvents = newerEventData.Select(e => JsonSerializer.Deserialize<Event>(e.Payload)).ToList();

            foreach (var @event in newerEvents)
            {
                var apply = typeof(TGrainState).GetMethod("Apply", new Type[] { @event.GetType() });

                if (apply == null)
                {
                    continue;
                }

                apply.Invoke(state, new object[] { @event });
            }

            var newVersion = newerEventData.Max(e => e.Version);

            if (snapshot.Version < newVersion)
            {
                snapshot.Version = newVersion;
                snapshot.Payload = JsonSerializer.Serialize(state);

                DbContext.Snapshots.Update(snapshot);

                await DbContext.SaveChangesAsync();
            }

            return new KeyValuePair<int, TGrainState>(snapshot.Version, state);
        }

        public async Task<bool> ApplyUpdatesToStorage(IReadOnlyList<Event> updates, int expectedversion)
        {
            var currentVersion = await DbContext.GetAggregateVersion(this.GetPrimaryKey());

            if (currentVersion != expectedversion)
            {
                return false;
            }

            if (currentVersion == 0)
            {
                var initializedEvent = new InitializedEvent
                {
                    Id = Guid.NewGuid(),
                    AggregateId = this.GetPrimaryKey(),
                    OccurredOn = DateTimeOffset.UtcNow
                };

                DbContext.Events.Add(new Data.Entities.Event
                {
                    Id = initializedEvent.Id,
                    Version = 0,
                    AggregateId = this.GetPrimaryKey(),
                    EventType = initializedEvent.GetType().Name,
                    Timestamp = initializedEvent.OccurredOn,
                    IsPublished = true,
                    Payload = JsonSerializer.Serialize(initializedEvent)
                });

                var snapshot = new Snapshot
                {
                    AggregateId = this.GetPrimaryKey(),
                    Version = 0,
                    Payload = JsonSerializer.Serialize(State)
                };

                DbContext.Snapshots.Add(snapshot);
            }

            foreach (var item in updates)
            {
                currentVersion++;

                DbContext.Events.Add(new Data.Entities.Event
                {
                    Id = item.Id,
                    Version = currentVersion,
                    AggregateId = item.AggregateId,
                    EventType = item.GetType().Name,
                    Timestamp = item.OccurredOn,
                    Payload = JsonSerializer.Serialize(item)
                });
            }

            await ApplyingUpdatesToStorage(updates);

            await DbContext.SaveChangesAsync();

            return true;
        }

        protected virtual Task ApplyingUpdatesToStorage(IReadOnlyList<Event> updates) => Task.CompletedTask;
    }
}
