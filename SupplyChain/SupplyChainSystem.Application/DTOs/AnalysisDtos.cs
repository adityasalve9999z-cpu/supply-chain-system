using System.ComponentModel.DataAnnotations;

namespace SupplyChainSystem.Application.DTOs;

public sealed record AnalysisRequest(
    [Required, MaxLength(2000)] string Question);

public sealed record AnalysisResponse(
    string Answer,
    DateTime GeneratedAtUtc,
    int InventoryRecords,
    int LowStockRecords);
