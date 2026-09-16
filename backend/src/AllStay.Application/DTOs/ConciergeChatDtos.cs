namespace AllStay.Application.DTOs;

public record ConciergeChatMessage(string Role, string Text);

public record ConciergeChatRequest(string Message, IReadOnlyList<ConciergeChatMessage>? History);

public record ConciergeChatResponse(string Reply);
