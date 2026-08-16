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
}
