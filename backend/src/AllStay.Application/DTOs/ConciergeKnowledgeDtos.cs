namespace AllStay.Application.DTOs;

public record ConciergeKnowledgeEntryDto(Guid Id, Guid HotelId, string Title, string Content, DateTimeOffset CreatedAt);

public record CreateConciergeKnowledgeEntryRequest(string Title, string Content);
