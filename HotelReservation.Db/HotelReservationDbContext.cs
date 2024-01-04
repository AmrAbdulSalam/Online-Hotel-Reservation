using HotelReservation.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Db
{
    public class HotelReservationDbContext : DbContext
    {
        internal DbSet<City> Cities { get; set; }
        internal DbSet<FeaturedDeal> FeaturedDeals { get; set; }
        internal DbSet<Hotel> Hotels { get; set; }
        internal DbSet<User> Users { get; set; }
        internal DbSet<Room> Rooms { get; set; }
        internal DbSet<Reservation> Reservations { get; set; }
        internal DbSet<Payment> Payments { get; set; }

        public HotelReservationDbContext(DbContextOptions<HotelReservationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<Reservation>()
                .HasIndex(x => x.ReferenceceNo)
                .IsUnique();
        }
    }
}
