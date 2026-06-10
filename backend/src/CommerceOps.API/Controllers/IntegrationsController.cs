using AutoMapper;
using CommerceOps.API.DTOs;
using CommerceOps.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOps.API.Controllers;

[ApiController]
[Route("api/integrations")]
public class IntegrationsController(IIntegrationHealthRepository healthRepo, IMapper mapper) : ControllerBase
{
    [HttpGet("health")]
    public async Task<ActionResult<IReadOnlyList<IntegrationHealthDto>>> GetHealth(CancellationToken ct)
    {
        var health = await healthRepo.GetAllAsync(ct);
        return Ok(mapper.Map<IReadOnlyList<IntegrationHealthDto>>(health));
    }
}
