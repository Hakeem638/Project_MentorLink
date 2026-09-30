using System.Net;
using System.Net.Http.Json;
using MentorLink.Shared.Dtos;
using MentorLink.Shared.Models;

namespace MentorLink.Api.Tests;

public class AuthTests : IClassFixture<MentorLinkFactory>
{
    private readonly MentorLinkFactory _factory;
    public AuthTests(MentorLinkFactory factory) => _factory = factory;

    [Theory]
    [InlineData(MentorLinkFactory.StudentEmail, UserRole.Student)]
    [InlineData(MentorLinkFactory.MentorEmail, UserRole.Mentor)]
    [InlineData(MentorLinkFactory.AdminEmail, UserRole.Admin)]
    public async Task Demo_accounts_can_log_in(string email, UserRole role)
    {
        var auth = await _factory.LoginAsync(email);

        Assert.Equal(email, auth.User.Email);
        Assert.Equal(role, auth.User.Role);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));
    }

    [Fact]
    public async Task Login_ignores_email_case_and_whitespace()
    {
        var auth = await _factory.LoginAsync("  AMARA@Student.dev ");
        Assert.Equal(MentorLinkFactory.StudentEmail, auth.User.Email);
    }

    [Fact]
    public async Task Wrong_password_is_rejected()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("api/auth/login",
            new LoginRequest(MentorLinkFactory.StudentEmail, "not-the-password"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Unknown_email_is_rejected()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("api/auth/login",
            new LoginRequest("nobody@example.com", "whatever"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task New_student_can_sign_up_and_then_log_in()
    {
        var email = $"student-{Guid.NewGuid():N}@test.dev";
        var signup = await _factory.CreateClient().PostAsJsonAsync("api/auth/signup",
            new SignupRequest("Test Student", email, "Computer Science", "pass-1234", UserRole.Student));

        Assert.Equal(HttpStatusCode.OK, signup.StatusCode);
        var created = await signup.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.Equal(UserRole.Student, created!.User.Role);

        var login = await _factory.LoginAsync(email, "pass-1234");
        Assert.Equal(created.User.Id, login.User.Id);
    }

    [Fact]
    public async Task Signing_up_with_an_existing_email_returns_conflict()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("api/auth/signup",
            new SignupRequest("Copy", MentorLinkFactory.StudentEmail, "Anything", "pass-1234", UserRole.Student));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task New_mentor_starts_as_pending_verification()
    {
        var email = $"mentor-{Guid.NewGuid():N}@test.dev";
        var signup = await _factory.CreateClient().PostAsJsonAsync("api/auth/signup",
            new SignupRequest("Test Mentor", email, "Finance", "pass-1234", UserRole.Mentor));
        signup.EnsureSuccessStatusCode();

        // Unverified mentors must not appear in Discover.
        var (client, _) = await _factory.SignedInClientAsync(MentorLinkFactory.StudentEmail);
        var mentors = await client.GetFromJsonAsync<List<MentorCardDto>>("api/mentors");
        Assert.DoesNotContain(mentors!, m => m.Name == "Test Mentor");
    }
}
