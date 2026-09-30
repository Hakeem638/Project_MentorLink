using System.Net;
using System.Net.Http.Headers;

namespace MentorLink.Api.Tests;

public class AuthorizationTests : IClassFixture<MentorLinkFactory>
{
    private readonly MentorLinkFactory _factory;
    public AuthorizationTests(MentorLinkFactory factory) => _factory = factory;

    [Theory]
    [InlineData("api/mentors")]
    [InlineData("api/goals/student/1")]
    [InlineData("api/notifications/1")]
    [InlineData("api/messages/conversations/1")]
    [InlineData("api/dashboard/student/1")]
    [InlineData("api/admin/verifications")]
    public async Task Protected_endpoints_require_a_token(string url)
    {
        var response = await _factory.CreateClient().GetAsync(url);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task A_forged_token_is_rejected()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not.a.real-token");

        var response = await client.GetAsync("api/mentors");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Signed_in_student_can_browse_mentors()
    {
        var (client, _) = await _factory.SignedInClientAsync(MentorLinkFactory.StudentEmail);
        var response = await client.GetAsync("api/mentors");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(MentorLinkFactory.StudentEmail)]
    [InlineData(MentorLinkFactory.MentorEmail)]
    public async Task Only_admins_can_see_verifications(string email)
    {
        var (client, _) = await _factory.SignedInClientAsync(email);
        var response = await client.GetAsync("api/admin/verifications");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Admin_can_see_verifications()
    {
        var (client, _) = await _factory.SignedInClientAsync(MentorLinkFactory.AdminEmail);
        var response = await client.GetAsync("api/admin/verifications");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
