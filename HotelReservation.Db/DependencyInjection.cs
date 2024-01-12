using HotelReservation.Db.Repositories;
using HotelReservation.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HotelReservation.Db
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHotelReservationDbContext
            (this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddDbContext<HotelReservationDbContext>(options =>
            {
                if (environment.IsDevelopment())
                {
                    options.UseSqlServer(configuration.GetConnectionString("SQL-ConnectionString"));
                }
                else if (environment.IsProduction())
                {
                    options.UseSqlServer(configuration.GetConnectionString("SQL-Development"));
                }
            });

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
