using AllStay.Application.Interfaces;
using AllStay.Infrastructure.Persistence;
using AllStay.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AllStay.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AllStayDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<ILeadService, LeadService>();

        return services;
    }
}
