using Microsoft.EntityFrameworkCore;
using Sketch.EventSourcing.Data.Entities;

namespace Sketch.EventSourcing.Data
{
    public abstract class AggregateDbContext : DbContext
    {
        public AggregateDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }

        public DbSet<Snapshot> Snapshots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .HasKey(b => new { b.AggregateId, b.Version });

            modelBuilder.Entity<Event>()
                .HasIndex(b => new { b.Id });

            modelBuilder.Entity<Event>()
                .HasIndex(b => new { b.IsPublished });
        }

        public async Task<List<Event>> GetEventsByAggregateId(Guid aggregateId, int eTag)
        {
            var events = await Events
                .Where(e => e.AggregateId == aggregateId && e.Version > eTag)
                .ToListAsync();

            return events.OrderBy(e => e.Version).ToList();
        }

        public async Task<List<Event>> GetUnpublishedEvents()
        {
            var events = await Events
                .Where(e => e.IsPublished == false)
                .ToListAsync();

            return events.OrderBy(e => e.Timestamp).ToList();
        }

        public async Task<int> GetAggregateVersion(Guid aggregateId) => await Events.Where(i => i.AggregateId == aggregateId).MaxAsync(i => i.Version);

        public ValueTask<Snapshot?> GetSnapshotByAggregateId(Guid aggregateId) => Snapshots.FindAsync(aggregateId);
    }
}
