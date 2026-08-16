using AllStay.Api.Auth;
using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

[ApiController]
[Route("api/leads")]
public class LeadsController(ILeadService leadService) : ControllerBase
{
    /// <summary>Institutional website's contact/lead form posts here — feeds the B2B sales funnel.</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<LeadDto>> Create(CreateLeadRequest request, CancellationToken ct)
        => Ok(await leadService.CreateAsync(request, ct));

    /// <summary>Sales-pipeline view across every hotel lead — admin only, not a hotel-staff concern.</summary>
    [HttpGet]
    [AdminApiKey]
    public async Task<ActionResult<IReadOnlyList<LeadDto>>> List(CancellationToken ct)
        => Ok(await leadService.ListAsync(ct));
}
