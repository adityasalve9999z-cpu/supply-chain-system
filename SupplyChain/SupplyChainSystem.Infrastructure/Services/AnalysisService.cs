using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SupplyChainSystem.Application.DTOs;
using SupplyChainSystem.Application.Interfaces;
using SupplyChainSystem.Infrastructure.Data;

namespace SupplyChainSystem.Infrastructure.Services;

public sealed class AnalysisService(
    SupplyChainDbContext db,
    HttpClient client,
    IConfiguration configuration) : IAnalysisService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<AnalysisResponse> AnalyzeAsync(AnalysisRequest request, CancellationToken cancellationToken)
    {
        var endpoint = configuration["AzureOpenAI:Endpoint"]?.TrimEnd('/');
        var deployment = configuration["AzureOpenAI:Deployment"];
        var apiKey = configuration["AzureOpenAI:ApiKey"];

        if (string.IsNullOrWhiteSpace(endpoint) ||
            string.IsNullOrWhiteSpace(deployment) ||
            string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Azure OpenAI is not configured. Set AzureOpenAI__Endpoint, AzureOpenAI__Deployment, and AzureOpenAI__ApiKey.");
        }

        var inventory = await db.InventoryItems
            .AsNoTracking()
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .OrderBy(i => i.Product.Name)
            .ThenBy(i => i.Warehouse.Name)
            .Select(i => new
            {
                Product = i.Product.Name,
                Sku = i.Product.Sku,
                Warehouse = i.Warehouse.Name,
                i.Quantity,
                i.LowStockThreshold,
                IsLowStock = i.Quantity <= i.LowStockThreshold,
                i.LastUpdated
            })
            .ToListAsync(cancellationToken);

        var lowStockRecords = inventory.Count(i => i.IsLowStock);
        var snapshot = JsonSerializer.Serialize(inventory, JsonOptions);
        var systemPrompt = """
            You are Flowline Inventory Analyst, a read-only supply-chain operations assistant.
            Analyze only the inventory snapshot supplied by the application. Do not invent products,
            quantities, dates, suppliers, or business policies. Clearly distinguish observations from
            assumptions. Prioritize low-stock risks, warehouse concentration, and practical next steps.
            Never claim that you changed data or placed an order. Use concise headings and bullet points.
            """;
        var userPrompt = $"""
            User question:
            {request.Question}

            Current inventory snapshot (JSON):
            {snapshot}
            """;

        var payload = new
        {
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            temperature = 0.2,
            max_tokens = 900
        };

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            $"{endpoint}/openai/deployments/{Uri.EscapeDataString(deployment)}/chat/completions?api-version=2024-10-21");
        message.Headers.Add("api-key", apiKey);
        message.Content = new StringContent(
            JsonSerializer.Serialize(payload, JsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await client.SendAsync(message, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Azure OpenAI analysis failed with status {(int)response.StatusCode}: {responseBody}");
        }

        using var document = JsonDocument.Parse(responseBody);
        var answer = document.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(answer))
        {
            throw new InvalidOperationException("Azure OpenAI returned an empty analysis.");
        }

        return new AnalysisResponse(answer, DateTime.UtcNow, inventory.Count, lowStockRecords);
    }
}
