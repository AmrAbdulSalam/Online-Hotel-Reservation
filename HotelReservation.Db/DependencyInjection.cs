using HotelReservation.Db.Repositories;
using HotelReservation.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelReservation.Db
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHotelReservationDbContext
            (this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        {
            services.AddDbContext<HotelReservationDbContext>(optionsAction);

            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IFeaturedDealRepository, FeaturedDealRepository>();
            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}
