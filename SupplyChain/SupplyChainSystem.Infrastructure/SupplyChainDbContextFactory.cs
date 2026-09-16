using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SupplyChainSystem.Infrastructure.Data;

namespace SupplyChainSystem.Infrastructure;

public sealed class SupplyChainDbContextFactory : IDesignTimeDbContextFactory<SupplyChainDbContext>
{
    public SupplyChainDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var settingsPath = new[]
        {
            Path.Combine(basePath, "SupplyChain", "SupplyChainSystem.API", "appsettings.json"),
            Path.Combine(basePath, "..", "SupplyChainSystem.API", "appsettings.json")
        }.FirstOrDefault(File.Exists) ?? throw new FileNotFoundException("The API appsettings.json file was not found.");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(settingsPath)!)
            .AddJsonFile(Path.GetFileName(settingsPath), optional: false)
            .Build();
        var options = new DbContextOptionsBuilder<SupplyChainDbContext>()
            .UseSqlite(configuration.GetConnectionString("DefaultConnection"))
            .Options;
        return new SupplyChainDbContext(options);
    }
}
