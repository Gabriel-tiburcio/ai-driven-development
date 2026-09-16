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
        var config = configuration.GetConnectionString("Default");
        services.AddDbContext<AllStayDbContext>(
           options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("Default"),
                    sqlOptions => sqlOptions.CommandTimeout(60) // Tempo em segundos (ex: 60 segundos)
                ));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<DeepSeekOptions>(configuration.GetSection(DeepSeekOptions.SectionName));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<ILeadService, LeadService>();
        services.AddScoped<IHotelInfoSectionService, HotelInfoSectionService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IGuestRequestService, GuestRequestService>();
        services.AddScoped<IKidsActivityService, KidsActivityService>();
        services.AddScoped<IRestaurantService, RestaurantService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IExternalExperienceService, ExternalExperienceService>();
        services.AddScoped<IConciergeKnowledgeService, ConciergeKnowledgeService>();
        services.AddHttpClient<IConciergeChatService, ConciergeChatService>();

        return services;
    }
}
