using LibraryApi.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace LibraryApi.Tests;

public class LibraryApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:latest")
        .WithDatabase("librarydb_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Usuń istniejącą rejestrację DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // Dodaj DbContext z connection stringiem do kontenera
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));
            var rateLimiterDescriptor = services.SingleOrDefault(
                d => d.ServiceType.FullName != null && 
                     d.ServiceType.FullName.Contains("RateLimiter"));

            if (rateLimiterDescriptor != null)
                services.Remove(rateLimiterDescriptor);

// Dodaj liberalny Rate Limiter
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("fixed", o =>
                {
                    o.PermitLimit = 1000;
                    o.Window = TimeSpan.FromSeconds(1);
                });
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Zastosuj migracje na testowej bazie
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}