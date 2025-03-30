using DOANCOSO26.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DOANCOSO26.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Bus> Buses { get; set; }
        public DbSet<Stop> Stops { get; set; }
        public DbSet<BusTrip> BusTrips { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BusRoute> BusRoutes { get; set; }
        public DbSet<BusTripImage> BusTripImages { get; set; }
        public DbSet<TripReport> TripReports { get; set; }
        public DbSet<Driverregis> Driverregis { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BusTrip>()
                .HasMany(b => b.Images)
                .WithOne(i => i.BusTrip)
                .HasForeignKey(i => i.BusTripId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId);
            modelBuilder.Entity<BusTrip>()
               .HasMany(bt => bt.Seats)
               .WithOne(s => s.BusTrip)
               .HasForeignKey(s => s.BusTripId);

        }
    }
}
