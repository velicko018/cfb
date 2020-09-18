using CFB.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CFB.Application.Extensions
{
    public static class AddServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IFlightService, FlightService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IAirportService, AirportService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBookingService, BookingService>();

            return services;
        }
    }
}
