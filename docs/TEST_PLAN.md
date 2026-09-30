# MentorLink — Test Plan

## Automated tests

`tests/MentorLink.Api.Tests` holds xUnit integration tests. Each test class starts the real API in memory with its own fresh copy of the demo data, so no database or network is needed.

Run them from the repository root:

```bash
dotnet test
```

| File | What it covers |
|------|----------------|
| `PasswordHasherTests.cs` | Correct / wrong passwords, unique salts, the plain password never stored, malformed hashes rejected |
| `AuthTests.cs` | Demo logins for all three roles, email case-insensitivity, wrong password and unknown email, sign-up then login, duplicate email (`409`), new mentors hidden from Discover until verified |
| `AuthorizationTests.cs` | Protected endpoints return `401` without a token or with a forged one; students and mentors get `403` on admin endpoints; admins get `200` |
| `MentorshipFlowTests.cs` | Sending a request notifies the mentor; accepting creates a mentorship and notifies the student; a request can't be accepted twice; declining creates no mentorship; unknown mentor gives `404` |
| `GoalsAndNotificationsTests.cs` | Blank milestones ignored; progress goes 0 → 50 → 100% and the goal completes; unticking reopens it; milestone notifications; unknown milestone gives `404`; Mark all read |

## Manual test checklist

Use the demo accounts (password `demo1234`). Repeat the ★ rows at phone width (about 390px) and tablet width (about 820px) — in Chrome or Edge, press F12 and use the device toolbar.

| # | Screen / flow | Steps | Expected |
|---|---------------|-------|----------|
| 1 ★ | Landing | Open `/` | Hero, features, how-it-works and testimonials; nothing overflows sideways |
| 2 | Sign up | Create a student, then a mentor | Student lands on Dashboard; mentor is Pending in Admin |
| 3 | Login | Wrong password, then the right one | Error message, then role-based landing page |
| 4 ★ | Navigation | On a phone, open ☰ and pick each item | Menu opens and closes, and each screen loads |
| 5 | Refresh | Refresh on `/goals`, `/messages`, `/mentor/2` | Same page reloads — no 404, still signed in |
| 6 ★ | Discover | Search "data", tick a field, set minimum rating | List filters correctly |
| 7 | Mentor profile | Open a mentor, add a review | Review appears; rating updates |
| 8 | Request | Send a request as Amara | Appears under Pending; mentor gets a notification |
| 9 | Accept / decline | As Dr. Okafor, accept one request and decline another | Accepted → student's Active tab and a notification |
| 10 ★ | Messages | Chat between student and mentor in two windows | Messages arrive instantly; on a phone the back arrow returns to the list |
| 11 ★ | Goals | Add a goal with 3 milestones; tick all | Progress 33 → 67 → 100%, card turns dark (Completed) |
| 12 | Notifications | Mark all read | Unread dots disappear |
| 13 | Settings | Edit profile, toggle preferences, change password | Saved; new password works at next login |
| 14 ★ | Admin | Approve and reject applications | Status badges update; approved mentor appears in Discover |
| 15 | Sign out | Sign out, then press Back | Redirected to login |

## Reporting bugs

Open a GitHub issue with the screen, steps to reproduce, expected and actual result, device and browser, and a screenshot.
