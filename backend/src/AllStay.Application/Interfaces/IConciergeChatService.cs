using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface IConciergeChatService
{
    Task<ConciergeChatResponse> AskAsync(Guid hotelId, ConciergeChatRequest request, CancellationToken ct = default);
}
