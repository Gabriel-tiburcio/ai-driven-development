using System.Net.Http.Json;
using System.Text.Json;
using AllStay.Web.Backoffice.Models;

namespace AllStay.Web.Backoffice.Services;

public class ApiClient(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private HttpClient Client => httpClientFactory.CreateClient("AllStayApi");

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

    public async Task<ActivityDto?> CreateActivityAsync(Guid hotelId, CreateActivityRequest request, CancellationToken ct = default)
    {
        var response = await Client.PostAsJsonAsync($"/api/hotels/{hotelId}/activities", request, JsonOptions, ct);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<ActivityDto>(JsonOptions, ct) : null;
    }

    public async Task<bool> UpdateActivityAsync(Guid hotelId, Guid activityId, UpdateActivityRequest request, CancellationToken ct = default)
    {
        var response = await Client.PutAsJsonAsync($"/api/hotels/{hotelId}/activities/{activityId}", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteActivityAsync(Guid hotelId, Guid activityId, CancellationToken ct = default)
    {
        var response = await Client.DeleteAsync($"/api/hotels/{hotelId}/activities/{activityId}", ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AddSlotAsync(Guid hotelId, Guid activityId, CreateActivitySlotRequest request, CancellationToken ct = default)
    {
        var response = await Client.PostAsJsonAsync($"/api/hotels/{hotelId}/activities/{activityId}/slots", request, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteSlotAsync(Guid hotelId, Guid activityId, Guid slotId, CancellationToken ct = default)
    {
        var response = await Client.DeleteAsync($"/api/hotels/{hotelId}/activities/{activityId}/slots/{slotId}", ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<ReservationDto>> GetReservationsAsync(Guid hotelId, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<List<ReservationDto>>($"/api/hotels/{hotelId}/reservations", JsonOptions, ct) ?? [];
}
