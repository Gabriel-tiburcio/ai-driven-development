using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

/// <summary>Guest-facing Concierge Premium chat — no auth (guest never logs in).</summary>
[ApiController]
[Route("api/hotels/{hotelId:guid}/concierge/chat")]
public class ConciergeChatController(IConciergeChatService conciergeChatService) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ConciergeChatResponse>> Ask(Guid hotelId, ConciergeChatRequest request, CancellationToken ct)
        => Ok(await conciergeChatService.AskAsync(hotelId, request, ct));
}
