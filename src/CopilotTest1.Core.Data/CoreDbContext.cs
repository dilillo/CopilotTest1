using CopilotTest1.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Sketch.EventSourcing.Data;

namespace CopilotTest1.Core.Data
{
    public class CoreDbContext : AggregateDbContext
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<BusinessRef> BusinessRefs { get; set; }

        public DbSet<LocationRef> LocationRefs { get; set; }

        public DbSet<ProviderRef> ProviderRefs { get; set; }

        public DbSet<LocationServiceRef> LocationServiceRefs { get; set; }

        public DbSet<ProviderServiceAppointmentRef> ProviderServiceAppointmentRefs { get; set; }

        public DbSet<ProviderTimeOffRef> ProviderTimeOffRefs { get; set; }

        public DbSet<LocationServiceProviderRef> LocationServiceProviderRefs { get; set; }

        public DbSet<LocationClosingRef> LocationClosingRefs { get; set; }

        public DbSet<LocationHourRef> LocationHourRefs { get; set; }

        public DbSet<ProviderHourRef> ProviderHourRefs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BusinessRef>()
                .HasIndex(b => new { b.Name });

            modelBuilder.Entity<LocationRef>()
                .HasIndex(b => new { b.BusinessId, b.Name });

            modelBuilder.Entity<LocationServiceProviderRef>()
                .HasKey(b => new { b.LocationServiceId, b.ProviderId });

            modelBuilder.Entity<LocationHourRef>()
                .HasKey(b => new { b.LocationId, b.Day, b.StartHour, b.StartMinute, b.EndHour, b.EndMinute });

            modelBuilder.Entity<ProviderHourRef>()
                .HasKey(b => new { b.ProviderId, b.LocationId, b.Day, b.StartHour, b.StartMinute, b.EndHour, b.EndMinute });
        }

        public Task<bool> GetConflictingProviderServiceAppointmentExists(Guid providerId, Guid locationServiceId, DateTimeOffset start, DateTimeOffset end) =>
            ProviderServiceAppointmentRefs.AnyAsync(i => i.ProviderId == providerId && i.LocationServiceId == locationServiceId && ((i.Start >= start && i.Start < end) || (i.End > start && i.End <= end)));

        public async Task<bool> GetOutsideLocationHours(Guid locationId, DateTimeOffset start, DateTimeOffset end)
        {
            var locationHours = await LocationHourRefs.Where(i => i.LocationId == locationId).ToListAsync();

            var startHours = locationHours
                .Where(i => i.Day == start.DayOfWeek && i.StartTime <= start.TimeOfDay)
                .OrderByDescending(i => i.StartTime)
                .FirstOrDefault();

            var endHours = locationHours
                .Where(i => i.Day == end.DayOfWeek && i.EndTime >= end.TimeOfDay)
                .OrderBy(i => i.EndTime)
                .FirstOrDefault();

            return (startHours == null || endHours == null);
        }

        public Task<bool> GetAppointmentConflictsWithProviderTimeOff(Guid providerId, DateTimeOffset start, DateTimeOffset end) => ProviderTimeOffRefs
            .AnyAsync(i => i.ProviderId == providerId && ((i.Start > start && i.Start <= end) || (i.End <= end && i.End >= start)));

        public Task<bool> GetLocationClosingHasAppointmentConflicts(Guid locationId, DateTimeOffset start, DateTimeOffset end) => ProviderServiceAppointmentRefs
            .AnyAsync(i => i.LocationService.LocationId == locationId && ((i.Start >= start && i.Start <= end) || (i.End <= end && i.End >= start)));

        public ValueTask<BusinessRef?> GetBusinessRef(Guid id) => BusinessRefs.FindAsync(id);

        public Task<BusinessRef?> GetBusinessRefByName(string name) => BusinessRefs.FirstOrDefaultAsync(i => i.Name == name);

        public ValueTask<LocationRef?> GetLocationRef(Guid id) => LocationRefs.FindAsync(id);

        public Task<LocationRef?> GetLocationRefByName(Guid businessId, string name) => LocationRefs.FirstOrDefaultAsync(i => i.BusinessId == businessId && i.Name == name);

        public ValueTask<ProviderRef?> GetProviderRef(Guid id) => ProviderRefs.FindAsync(id);

        public ValueTask<LocationServiceRef?> GetLocationServiceRef(Guid id) => LocationServiceRefs.FindAsync(id);
        
        public ValueTask<ProviderServiceAppointmentRef?> GetProviderServiceAppointmentRef(Guid id) => ProviderServiceAppointmentRefs.FindAsync(id);
        
        public ValueTask<ProviderTimeOffRef?> GetProviderTimeOffRef(Guid id) => ProviderTimeOffRefs.FindAsync(id);

        public ValueTask<LocationClosingRef?> GetLocationClosingRef(Guid id) => LocationClosingRefs.FindAsync(id);

        public ValueTask<LocationServiceProviderRef?> GetLocationServiceProviderRef(Guid locationServiceId, Guid providerId) => LocationServiceProviderRefs.FindAsync(locationServiceId, providerId);

        public ValueTask<LocationHourRef?> GetLocationHourRef(Guid locationId, DayOfWeek day, TimeOnly startTime, TimeOnly stopTime) => LocationHourRefs.FindAsync(locationId, day, startTime, stopTime);

        public Task<List<LocationHourRef>> GetLocationHourRefs(Guid locationId) => LocationHourRefs.Where(i => i.LocationId == locationId).ToListAsync();

        public Task<List<ProviderHourRef>> GetProviderHourRefs(Guid providerId) => ProviderHourRefs.Where(i => i.ProviderId == providerId).ToListAsync();
    }
}