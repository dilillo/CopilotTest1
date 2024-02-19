using CopilotTest1.Customer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Sketch.EventSourcing.Data;

namespace CopilotTest1.Customer.Data
{
    public class CustomerDbContext : AggregateDbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
            : base(options)
        {
        }

        public DbSet<CustomerRef> CustomerRefs { get; set; }

        public DbSet<BusinessRef> BusinessRefs { get; set; }

        public DbSet<OwnerEvent> OwnerEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerRef>()
                .HasIndex(b => new { b.Email });

            modelBuilder.Entity<BusinessRef>()
                .HasIndex(b => new { b.Email });

            modelBuilder.Entity<OwnerEvent>()
                .HasIndex(b => new { b.IsProcessed });
        }

        public ValueTask<CustomerRef?> GetCustomerRef(Guid id) => CustomerRefs.FindAsync(id);

        public Task<CustomerRef?> GetCustomerRefByEmail(string email) => CustomerRefs.FirstOrDefaultAsync(i => i.Email == email);

        public ValueTask<BusinessRef?> GetProviderRef(Guid id) => BusinessRefs.FindAsync(id);

        public Task<BusinessRef?> GetProviderRefByEmail(string email) => BusinessRefs.FirstOrDefaultAsync(i => i.Email == email);

        public Task<bool> GetPlaceEventExists(Guid id) => OwnerEvents.AnyAsync(i => i.Id == id);

        public ValueTask<OwnerEvent?> GetPlaceEvent(Guid id) => OwnerEvents.FindAsync(id);

        public async Task<List<OwnerEvent>> GetUnprocessedPlaceEvents()
        {
            var unsorted = await OwnerEvents.Where(i => i.IsProcessed == false).ToListAsync();

            return unsorted.OrderBy(i => i.Timestamp).ToList();
        }
    }
}
