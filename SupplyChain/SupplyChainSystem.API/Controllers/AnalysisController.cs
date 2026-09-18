using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyChainSystem.Application.DTOs;
using SupplyChainSystem.Application.Interfaces;

namespace SupplyChainSystem.API.Controllers;

[ApiController, Route("api/analysis"), Authorize]
public sealed class AnalysisController(IAnalysisService analysis) : ControllerBase
{
    [HttpPost]
    public Task<AnalysisResponse> Analyze(
        [FromBody] AnalysisRequest request,
        CancellationToken cancellationToken) =>
        analysis.AnalyzeAsync(request, cancellationToken);
}
