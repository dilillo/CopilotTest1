using CopilotTest1.People.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Sketch.EventSourcing.Data;

namespace CopilotTest1.People.Data
{
    public class ProviderDbContext : AggregateDbContext
    {
        public ProviderDbContext(DbContextOptions<ProviderDbContext> options)
            : base(options)
        {
        }

        public DbSet<CustomerRef> CustomerRefs { get; set; }

        public DbSet<ProviderRef> ProviderRefs { get; set; }

        public DbSet<LocationServiceRef> LocationServiceRefs { get; set; }

        public DbSet<PlaceEvent> PlaceEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerRef>()
                .HasIndex(b => new { b.Email });

            modelBuilder.Entity<ProviderRef>()
                .HasIndex(b => new { b.Email });

            modelBuilder.Entity<PlaceEvent>()
                .HasIndex(b => new { b.IsProcessed });
        }

        public ValueTask<CustomerRef?> GetCustomerRef(Guid id) => CustomerRefs.FindAsync(id);

        public Task<CustomerRef?> GetCustomerRefByEmail(string email) => CustomerRefs.FirstOrDefaultAsync(i => i.Email == email);

        public ValueTask<ProviderRef?> GetProviderRef(Guid id) => ProviderRefs.FindAsync(id);

        public Task<ProviderRef?> GetProviderRefByEmail(string email) => ProviderRefs.FirstOrDefaultAsync(i => i.Email == email);

        public ValueTask<LocationServiceRef?> GetLocationServiceRef(Guid id) => LocationServiceRefs.FindAsync(id);

        public Task<bool> GetPlaceEventExists(Guid id) => PlaceEvents.AnyAsync(i => i.Id == id);

        public ValueTask<PlaceEvent?> GetPlaceEvent(Guid id) => PlaceEvents.FindAsync(id);

        public async Task<List<PlaceEvent>> GetUnprocessedPlaceEvents()
        {
            var unsorted = await PlaceEvents.Where(i => i.IsProcessed == false).ToListAsync();

            return unsorted.OrderBy(i => i.Timestamp).ToList();
        }
    }
}
