using System.Net;
using System.Net.Http.Json;
using MentorLink.Shared.Dtos;
using MentorLink.Shared.Models;

namespace MentorLink.Api.Tests;

/// <summary>Student requests a mentor → mentor accepts or declines → both sides are notified.</summary>
public class MentorshipFlowTests : IClassFixture<MentorLinkFactory>
{
    private readonly MentorLinkFactory _factory;
    public MentorshipFlowTests(MentorLinkFactory factory) => _factory = factory;

    private async Task<(HttpClient Client, UserDto User)> NewStudentAsync()
    {
        var email = $"flow-{Guid.NewGuid():N}@test.dev";
        var signup = await _factory.CreateClient().PostAsJsonAsync("api/auth/signup",
            new SignupRequest("Flow Student", email, "Statistics", "pass-1234", UserRole.Student));
        signup.EnsureSuccessStatusCode();
        return await _factory.SignedInClientAsync(email, "pass-1234");
    }

    private static async Task<RequestDto> SendRequestAsync(HttpClient student, int studentId, int mentorId)
    {
        var response = await student.PostAsJsonAsync("api/requests",
            new CreateMentorshipRequest(studentId, mentorId, GoalType.Career, "  Please mentor me.  ", "Weekly"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RequestDto>())!;
    }

    [Fact]
    public async Task Sending_a_request_notifies_the_mentor()
    {
        var (student, me) = await NewStudentAsync();
        var (mentor, mentorUser) = await _factory.SignedInClientAsync(MentorLinkFactory.MentorEmail);

        var request = await SendRequestAsync(student, me.Id, mentorUser.Id);

        Assert.Equal(RequestStatus.Pending, request.Status);
        Assert.Equal("Please mentor me.", request.Message);

        var notifications = await mentor.GetFromJsonAsync<List<NotificationDto>>($"api/notifications/{mentorUser.Id}");
        Assert.Contains(notifications!, n => n.Kind == NotificationKind.RequestReceived && n.Body.Contains("Flow Student"));
    }

    [Fact]
    public async Task Accepting_creates_a_mentorship_and_notifies_the_student()
    {
        var (student, me) = await NewStudentAsync();
        var (mentor, mentorUser) = await _factory.SignedInClientAsync(MentorLinkFactory.MentorEmail);
        var request = await SendRequestAsync(student, me.Id, mentorUser.Id);

        var accept = await mentor.PostAsync($"api/requests/{request.Id}/accept", null);
        Assert.Equal(HttpStatusCode.NoContent, accept.StatusCode);

        var mentorships = await student.GetFromJsonAsync<List<MentorshipDto>>($"api/mentorships/student/{me.Id}");
        Assert.Contains(mentorships!, m => m.PartnerId == mentorUser.Id && m.Status == MentorshipStatus.Active);

        var notifications = await student.GetFromJsonAsync<List<NotificationDto>>($"api/notifications/{me.Id}");
        Assert.Contains(notifications!, n => n.Kind == NotificationKind.RequestAccepted);
    }

    [Fact]
    public async Task A_request_cannot_be_accepted_twice()
    {
        var (student, me) = await NewStudentAsync();
        var (mentor, mentorUser) = await _factory.SignedInClientAsync(MentorLinkFactory.MentorEmail);
        var request = await SendRequestAsync(student, me.Id, mentorUser.Id);

        await mentor.PostAsync($"api/requests/{request.Id}/accept", null);
        var second = await mentor.PostAsync($"api/requests/{request.Id}/accept", null);

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Declining_does_not_create_a_mentorship()
    {
        var (student, me) = await NewStudentAsync();
        var (mentor, mentorUser) = await _factory.SignedInClientAsync(MentorLinkFactory.MentorEmail);
        var request = await SendRequestAsync(student, me.Id, mentorUser.Id);

        var decline = await mentor.PostAsync($"api/requests/{request.Id}/decline", null);
        Assert.Equal(HttpStatusCode.NoContent, decline.StatusCode);

        var mentorships = await student.GetFromJsonAsync<List<MentorshipDto>>($"api/mentorships/student/{me.Id}");
        Assert.DoesNotContain(mentorships!, m => m.PartnerId == mentorUser.Id && m.Status == MentorshipStatus.Active);
    }

    [Fact]
    public async Task Requesting_an_unknown_mentor_returns_not_found()
    {
        var (student, me) = await NewStudentAsync();
        var response = await student.PostAsJsonAsync("api/requests",
            new CreateMentorshipRequest(me.Id, 999_999, GoalType.Academic, "Hi", "Weekly"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
