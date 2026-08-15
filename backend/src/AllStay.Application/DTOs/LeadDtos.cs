namespace AllStay.Application.DTOs;

public record CreateLeadRequest(string HotelName, string ContactName, string Email, string? Phone, string? Message);

public record LeadDto(Guid Id, string HotelName, string ContactName, string Email, string? Phone, string? Message, DateTimeOffset CreatedAt);
