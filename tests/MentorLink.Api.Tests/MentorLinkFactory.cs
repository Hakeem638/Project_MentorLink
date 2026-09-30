using System.Net.Http.Headers;
using System.Net.Http.Json;
using MentorLink.Api.Data;
using MentorLink.Shared.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MentorLink.Api.Tests;

/// <summary>
/// Boots the real API in memory. Each factory gets its own in-memory database,
/// seeded with the same demo data as local development.
/// </summary>
public class MentorLinkFactory : WebApplicationFactory<Program>
{
    public const string DemoPassword = "demo1234";
    public const string StudentEmail = "amara@student.dev";
    public const string MentorEmail = "nnamdi@mentor.dev";
    public const string AdminEmail = "admin@mentorlink.dev";

    private readonly string _databaseName = $"mentorlink-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(_databaseName));
        });
    }

    public async Task<AuthResponse> LoginAsync(string email, string password = DemoPassword)
    {
        var response = await CreateClient().PostAsJsonAsync("api/auth/login", new LoginRequest(email, password));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
    }

    /// <summary>A client that sends the JWT for the given account on every request.</summary>
    public async Task<(HttpClient Client, UserDto User)> SignedInClientAsync(string email, string password = DemoPassword)
    {
        var auth = await LoginAsync(email, password);
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);
        return (client, auth.User);
    }
}
