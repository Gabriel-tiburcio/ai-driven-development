using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AllStay.Web.HotelPortal.Models;

namespace AllStay.Web.HotelPortal.Services;

/// <summary>
/// Plain request/response HTTP calls to the AllStay API — no persistent connection, no circuit,
/// nothing to keep alive between requests. Each call takes the JWT explicitly since there's no
/// per-connection session state (this is a stateless Razor Pages app; the JWT lives in the auth cookie).
/// </summary>
public class ApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private HttpClient Client => httpClientFactory.CreateClient("AllStayApi");

    private HttpClient AuthorizedClient(string jwt)
    {
        var client = Client;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        return client;
    }

    private HttpClient AdminClient()
    {
        var client = Client;
        client.DefaultRequestHeaders.Add("X-Admin-Key", configuration["Admin:ApiKey"]);
        return client;
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, password), JsonOptions, ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions, ct);
    }

    public async Task<List<ActivityDto>> GetActivitiesAsync(Guid hotelId, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<ActivityDto>>($"/api/hotels/{hotelId}/activities", JsonOptions, ct) ?? [];

    public async Task<ActivityDto?> GetActivityAsync(Guid hotelId, Guid activityId, CancellationToken ct = default)
    {
        var response = await Client.GetAsync($"/api/hotels/{hotelId}/activities/{activityId}", ct);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<ActivityDto>(JsonOptions, ct) : null;
    }

    public async Task<bool> CreateActivityAsync(string jwt, Guid hotelId, CreateActivityRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PostAsJsonAsync($"/api/hotels/{hotelId}/activities", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateActivityAsync(string jwt, Guid hotelId, Guid activityId, UpdateActivityRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PutAsJsonAsync($"/api/hotels/{hotelId}/activities/{activityId}", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteActivityAsync(string jwt, Guid hotelId, Guid activityId, CancellationToken ct = default)
        => (await AuthorizedClient(jwt).DeleteAsync($"/api/hotels/{hotelId}/activities/{activityId}", ct)).IsSuccessStatusCode;

    public async Task<bool> AddSlotAsync(string jwt, Guid hotelId, Guid activityId, CreateActivitySlotRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PostAsJsonAsync($"/api/hotels/{hotelId}/activities/{activityId}/slots", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteSlotAsync(string jwt, Guid hotelId, Guid activityId, Guid slotId, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).DeleteAsync($"/api/hotels/{hotelId}/activities/{activityId}/slots/{slotId}", ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<ReservationDto>> GetReservationsAsync(string jwt, Guid hotelId, CancellationToken ct = default)
        => await AuthorizedClient(jwt).GetFromJsonAsync<List<ReservationDto>>($"/api/hotels/{hotelId}/reservations", JsonOptions, ct) ?? [];

    public async Task<List<HotelDto>> GetHotelsAsync(CancellationToken ct = default)
        => await AdminClient().GetFromJsonAsync<List<HotelDto>>("/api/admin/hotels", JsonOptions, ct) ?? [];

    public async Task<bool> CreateHotelAsync(CreateHotelRequest request, CancellationToken ct = default)
    {
        var response = await AdminClient().PostAsJsonAsync("/api/admin/hotels", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SetHotelActiveAsync(Guid hotelId, bool isActive, CancellationToken ct = default)
    {
        var response = await AdminClient().PatchAsJsonAsync($"/api/admin/hotels/{hotelId}/status", new UpdateActiveStatusRequest(isActive), JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateHotelLocationAsync(Guid hotelId, UpdateHotelLocationRequest request, CancellationToken ct = default)
    {
        var response = await AdminClient().PatchAsJsonAsync($"/api/admin/hotels/{hotelId}/location", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<HotelStaffDto>> GetHotelStaffAsync(Guid hotelId, CancellationToken ct = default)
        => await AdminClient().GetFromJsonAsync<List<HotelStaffDto>>($"/api/admin/hotels/{hotelId}/staff", JsonOptions, ct) ?? [];

    public async Task<bool> CreateStaffAsync(Guid hotelId, CreateStaffRequest request, CancellationToken ct = default)
    {
        var response = await AdminClient().PostAsJsonAsync($"/api/admin/hotels/{hotelId}/staff", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SetStaffActiveAsync(Guid hotelId, Guid staffId, bool isActive, CancellationToken ct = default)
    {
        var response = await AdminClient().PatchAsJsonAsync($"/api/admin/hotels/{hotelId}/staff/{staffId}/status", new UpdateActiveStatusRequest(isActive), JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<LeadDto>> GetLeadsAsync(CancellationToken ct = default)
        => await AdminClient().GetFromJsonAsync<List<LeadDto>>("/api/leads", JsonOptions, ct) ?? [];

    public async Task<List<HotelInfoSectionDto>> GetInfoSectionsAsync(Guid hotelId, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<HotelInfoSectionDto>>($"/api/hotels/{hotelId}/info-sections", JsonOptions, ct) ?? [];

    public async Task<bool> CreateInfoSectionAsync(string jwt, Guid hotelId, CreateHotelInfoSectionRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PostAsJsonAsync($"/api/hotels/{hotelId}/info-sections", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateInfoSectionAsync(string jwt, Guid hotelId, Guid sectionId, UpdateHotelInfoSectionRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PutAsJsonAsync($"/api/hotels/{hotelId}/info-sections/{sectionId}", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteInfoSectionAsync(string jwt, Guid hotelId, Guid sectionId, CancellationToken ct = default)
        => (await AuthorizedClient(jwt).DeleteAsync($"/api/hotels/{hotelId}/info-sections/{sectionId}", ct)).IsSuccessStatusCode;

    public async Task<List<EventDto>> GetEventsAsync(Guid hotelId, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<EventDto>>($"/api/hotels/{hotelId}/events", JsonOptions, ct) ?? [];

    public async Task<bool> CreateEventAsync(string jwt, Guid hotelId, CreateEventRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PostAsJsonAsync($"/api/hotels/{hotelId}/events", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateEventAsync(string jwt, Guid hotelId, Guid eventId, UpdateEventRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PutAsJsonAsync($"/api/hotels/{hotelId}/events/{eventId}", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteEventAsync(string jwt, Guid hotelId, Guid eventId, CancellationToken ct = default)
        => (await AuthorizedClient(jwt).DeleteAsync($"/api/hotels/{hotelId}/events/{eventId}", ct)).IsSuccessStatusCode;

    public async Task<List<GuestRequestDto>> GetGuestRequestsAsync(string jwt, Guid hotelId, CancellationToken ct = default)
        => await AuthorizedClient(jwt).GetFromJsonAsync<List<GuestRequestDto>>($"/api/hotels/{hotelId}/requests", JsonOptions, ct) ?? [];

    public async Task<bool> UpdateGuestRequestStatusAsync(string jwt, Guid hotelId, Guid requestId, GuestRequestStatus status, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PatchAsJsonAsync($"/api/hotels/{hotelId}/requests/{requestId}/status", new UpdateGuestRequestStatusRequest(status), JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<KidsActivityDto>> GetKidsActivitiesAsync(Guid hotelId, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<KidsActivityDto>>($"/api/hotels/{hotelId}/kids-activities", JsonOptions, ct) ?? [];

    public async Task<bool> CreateKidsActivityAsync(string jwt, Guid hotelId, CreateKidsActivityRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PostAsJsonAsync($"/api/hotels/{hotelId}/kids-activities", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateKidsActivityAsync(string jwt, Guid hotelId, Guid kidsActivityId, UpdateKidsActivityRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PutAsJsonAsync($"/api/hotels/{hotelId}/kids-activities/{kidsActivityId}", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteKidsActivityAsync(string jwt, Guid hotelId, Guid kidsActivityId, CancellationToken ct = default)
        => (await AuthorizedClient(jwt).DeleteAsync($"/api/hotels/{hotelId}/kids-activities/{kidsActivityId}", ct)).IsSuccessStatusCode;

    public async Task<List<KidsEnrollmentDto>> GetKidsEnrollmentsAsync(string jwt, Guid hotelId, CancellationToken ct = default)
        => await AuthorizedClient(jwt).GetFromJsonAsync<List<KidsEnrollmentDto>>($"/api/hotels/{hotelId}/kids-activities/enrollments", JsonOptions, ct) ?? [];

    public async Task<List<RestaurantDto>> GetRestaurantsAsync(Guid hotelId, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<RestaurantDto>>($"/api/hotels/{hotelId}/restaurants", JsonOptions, ct) ?? [];

    public async Task<bool> CreateRestaurantAsync(string jwt, Guid hotelId, CreateRestaurantRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PostAsJsonAsync($"/api/hotels/{hotelId}/restaurants", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateRestaurantAsync(string jwt, Guid hotelId, Guid restaurantId, UpdateRestaurantRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PutAsJsonAsync($"/api/hotels/{hotelId}/restaurants/{restaurantId}", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteRestaurantAsync(string jwt, Guid hotelId, Guid restaurantId, CancellationToken ct = default)
        => (await AuthorizedClient(jwt).DeleteAsync($"/api/hotels/{hotelId}/restaurants/{restaurantId}", ct)).IsSuccessStatusCode;

    public async Task<List<ServiceDto>> GetServicesAsync(Guid hotelId, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<ServiceDto>>($"/api/hotels/{hotelId}/services", JsonOptions, ct) ?? [];

    public async Task<bool> CreateServiceAsync(string jwt, Guid hotelId, CreateServiceItemRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PostAsJsonAsync($"/api/hotels/{hotelId}/services", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateServiceAsync(string jwt, Guid hotelId, Guid serviceId, UpdateServiceItemRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PutAsJsonAsync($"/api/hotels/{hotelId}/services/{serviceId}", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteServiceAsync(string jwt, Guid hotelId, Guid serviceId, CancellationToken ct = default)
        => (await AuthorizedClient(jwt).DeleteAsync($"/api/hotels/{hotelId}/services/{serviceId}", ct)).IsSuccessStatusCode;

    public async Task<List<ServiceRequestDto>> GetServiceRequestsAsync(string jwt, Guid hotelId, CancellationToken ct = default)
        => await AuthorizedClient(jwt).GetFromJsonAsync<List<ServiceRequestDto>>($"/api/hotels/{hotelId}/services/requests", JsonOptions, ct) ?? [];

    public async Task<List<ExternalExperienceDto>> GetExternalExperiencesAsync(Guid hotelId, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<ExternalExperienceDto>>($"/api/hotels/{hotelId}/external-experiences", JsonOptions, ct) ?? [];

    public async Task<bool> CreateExternalExperienceAsync(string jwt, Guid hotelId, CreateExternalExperienceRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PostAsJsonAsync($"/api/hotels/{hotelId}/external-experiences", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateExternalExperienceAsync(string jwt, Guid hotelId, Guid experienceId, UpdateExternalExperienceRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PutAsJsonAsync($"/api/hotels/{hotelId}/external-experiences/{experienceId}", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteExternalExperienceAsync(string jwt, Guid hotelId, Guid experienceId, CancellationToken ct = default)
        => (await AuthorizedClient(jwt).DeleteAsync($"/api/hotels/{hotelId}/external-experiences/{experienceId}", ct)).IsSuccessStatusCode;

    public async Task<List<ExperienceRequestDto>> GetExperienceRequestsAsync(string jwt, Guid hotelId, CancellationToken ct = default)
        => await AuthorizedClient(jwt).GetFromJsonAsync<List<ExperienceRequestDto>>($"/api/hotels/{hotelId}/external-experiences/requests", JsonOptions, ct) ?? [];

    public async Task<List<ConciergeKnowledgeEntryDto>> GetConciergeKnowledgeAsync(string jwt, Guid hotelId, CancellationToken ct = default)
        => await AuthorizedClient(jwt).GetFromJsonAsync<List<ConciergeKnowledgeEntryDto>>($"/api/hotels/{hotelId}/concierge-knowledge", JsonOptions, ct) ?? [];

    public async Task<bool> CreateConciergeKnowledgeAsync(string jwt, Guid hotelId, CreateConciergeKnowledgeEntryRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PostAsJsonAsync($"/api/hotels/{hotelId}/concierge-knowledge", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateConciergeKnowledgeAsync(string jwt, Guid hotelId, Guid entryId, UpdateConciergeKnowledgeEntryRequest request, CancellationToken ct = default)
    {
        var response = await AuthorizedClient(jwt).PutAsJsonAsync($"/api/hotels/{hotelId}/concierge-knowledge/{entryId}", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteConciergeKnowledgeAsync(string jwt, Guid hotelId, Guid entryId, CancellationToken ct = default)
        => (await AuthorizedClient(jwt).DeleteAsync($"/api/hotels/{hotelId}/concierge-knowledge/{entryId}", ct)).IsSuccessStatusCode;
}
