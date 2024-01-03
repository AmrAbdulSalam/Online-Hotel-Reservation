﻿using HotelReservation.Application.Services;
using HotelReservation.Domain;
using HotelReservation.Domain.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelReservation.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICityService , CityService>();
            services.AddScoped<IHotelService , HotelService>();
            services.AddScoped<IFeaturedDealService , FeaturedDealService>();
            services.AddScoped<IUserService , UserService>();
            services.AddScoped<IRoomService , RoomService>();
            services.AddScoped<IReservationService , ReservationService>();
            services.AddScoped<IPaymentService , PaymentService>();

            services.AddSingleton<IEncryptionService>(provider =>
            {
                return new EncryptionService(configuration);
            });

            return services;
        }
    }
}
