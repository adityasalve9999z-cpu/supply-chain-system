using SupplyChainSystem.Application.DTOs;

namespace SupplyChainSystem.Application.Interfaces;

public interface IAnalysisService
{
    Task<AnalysisResponse> AnalyzeAsync(AnalysisRequest request, CancellationToken cancellationToken);
}
