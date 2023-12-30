using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelReservation.Db.Extensions
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddHotelReservationDbContext
            (this IServiceCollection services , Action<DbContextOptionsBuilder> optionsAction)
        {
            services.AddDbContext<HotelReservationDbContext>(optionsAction);

            return services;
        }
    }
}
