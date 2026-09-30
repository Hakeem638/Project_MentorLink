# MentorLink

Mentorship platform connecting students with verified professional mentors — structured goals, milestone tracking, and real-time chat.

**Stack:** ASP.NET Core Web API · Blazor WebAssembly · SignalR · Entity Framework Core · PostgreSQL (Supabase)

## Project layout

```
MentorLink.sln
src/
  MentorLink.Shared/    Entities + DTOs shared by client and server
  MentorLink.Api/       ASP.NET Core Web API + SignalR hub + EF Core (hosts the client)
  MentorLink.Client/    Blazor WebAssembly front end (all 14 screens, phone / tablet / desktop)
tests/
  MentorLink.Api.Tests/ xUnit integration tests for the API
docs/                   Architecture, API reference, user guide, test plan
```

## Run it

```bash
dotnet run --project src/MentorLink.Api
```

Then open http://localhost:5180. The Blazor client is served by the API, so that one command runs everything.

**Demo accounts** (password `demo1234`):

| Role    | Email                  | Lands on              |
|---------|------------------------|-----------------------|
| Student | `amara@student.dev`    | Student Dashboard     |
| Mentor  | `nnamdi@mentor.dev`    | Mentor Dashboard      |
| Admin   | `admin@mentorlink.dev` | Mentor Verification   |

Tip: open two browser windows (one normal, one private), sign in as Amara in one and Dr. Okafor in the other, and chat — messages arrive live over SignalR.

## Live site

- Front end (Vercel): https://mentorlink-client.vercel.app
- API (Render): https://project-mentorlink.onrender.com

## Tests

```bash
dotnet test
```

38 automated tests boot the API against an in-memory database and cover login and sign-up, password hashing, access control, the request → accept / decline flow, goals and notifications. The manual checklist is in [docs/TEST_PLAN.md](docs/TEST_PLAN.md).

## Documentation

- [Architecture](docs/ARCHITECTURE.md) — components, deployment, data model, key flows
- [API reference](docs/API_REFERENCE.md) — every endpoint and the SignalR chat hub
- [User guide](docs/USER_GUIDE.md) — how students, mentors and admins use each screen
- [Test plan](docs/TEST_PLAN.md) — automated tests and manual checklist

## Database — Supabase (PostgreSQL)

Out of the box the app uses an **in-memory database** with seed data, so it runs with zero setup (data resets on restart).

To switch to Supabase:

1. In your Supabase project go to **Settings → Database → Connection string** and copy the direct connection values.
2. Put them in `src/MentorLink.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=db.YOUR-PROJECT-REF.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR-DB-PASSWORD;Ssl Mode=Require;Trust Server Certificate=true"
}
```

3. Restart the app. On startup EF Core applies the migrations in `src/MentorLink.Api/Migrations/` and seeds the demo data into an empty database.

> If your network blocks direct connections (IPv6), use Supabase's **session pooler** connection string instead — same format, host like `aws-0-REGION.pooler.supabase.com`, port `5432`, username `postgres.YOUR-PROJECT-REF`.

## The 14 screens

| # | Screen | Route |
|---|--------|-------|
| 1 | Landing Page | `/` |
| 2 | Sign Up | `/signup` |
| 3 | Login | `/login` |
| 4 | Student Dashboard | `/dashboard` |
| 5 | Discover Mentors | `/discover` |
| 6 | Mentor Profile | `/mentor/{id}` |
| 7 | Mentorship Request | `/request/{mentorId}` |
| 8 | Messages (SignalR) | `/messages` |
| 9 | Goal Tracker | `/goals` |
| 10 | Mentor Dashboard | `/mentor-dashboard` |
| 11 | Notifications | `/notifications` |
| 12 | Settings & Profile | `/settings` |
| 13 | My Mentorships | `/mentorships` |
| 14 | Admin — Mentor Verification | `/admin/verifications` |

## Team — group 5ive9ine

| # | Name | Student ID | Role | What they own in the project | Breakdown |
|---|------|------------|------|------------------------------|-----------|
| 1 | Sonpon Ye-shua Chief | 22082706 | Project Manager / UI / UX Designer | Overall direction and task allocation; visual design system (`Client/wwwroot/css/app.css`), layouts (`Client/Layout/`), shared components (`Client/Components/` — Sidebar, Avatar, Icon, Stars) | 1. Define scope, screens and task allocation<br>2. Design the 14 screens (UI/UX)<br>3. Build the design system: colours, typography, spacing (`app.css`)<br>4. Build the Public, App and Admin layouts<br>5. Build the Sidebar navigation, with a collapsible menu on phones<br>6. Build the Avatar, Icon and Stars components<br>7. Responsive layouts for phone and tablet<br>8. Review and merge team work |
| 2 | Mubarack Jibriel | 22146249 | Backend Developer (ASP.NET Core API) | API setup (`Api/Program.cs`), controllers for mentors, mentorships, users, dashboard and admin (`Api/Controllers/`), shared DTOs (`Shared/Dtos/Dtos.cs`) | 1. Set up the API project, dependency injection and middleware (`Program.cs`)<br>2. Define the shared DTOs<br>3. Mentor listing with search filters (`GET /api/mentors`)<br>4. Mentor profile endpoint and review submission<br>5. My Mentorships endpoint<br>6. Student and mentor dashboard endpoints<br>7. User profile update, preferences and change password<br>8. Admin verification list, approve and reject |
| 3 | Haruna Hakeem | 22046736 | Backend Developer (Auth & Matching) | Authentication — `AuthController`, JWT (`Services/JwtTokenService.cs`), password hashing (`Services/PasswordHasher.cs`), client token handling (`Client/Services/AuthHeaderHandler.cs`); matching/requests (`RequestsController`); deployment to Vercel + Render (`Dockerfile`, `build-vercel.sh`, CORS) | 1. Sign-up and login endpoints<br>2. Password hashing<br>3. JWT token issuing and validation<br>4. Attach the token to client requests (`AuthHeaderHandler`)<br>5. Send mentorship request endpoint<br>6. Accept / decline request, creating the mentorship on accept<br>7. Dockerfile and Render deployment of the API<br>8. Vercel build script and frontend deployment<br>9. CORS configuration for the Vercel frontend |
| 4 | Adom Bempong Franklin | 22020618 | Frontend Developer (Blazor) | Public and student-facing screens: Landing, Sign Up, Login, Student Dashboard, Discover Mentors, Mentor Profile | 1. Landing page<br>2. Sign Up screen with Student / Mentor choice<br>3. Login screen with role-based redirect<br>4. Student Dashboard<br>5. Discover Mentors with search and filters<br>6. Mentor Profile screen with reviews |
| 5 | Esi Twi Tawiah | 22231719 | Frontend Developer (Blazor) | Mentorship flow screens: Mentorship Request, My Mentorships, Mentor Dashboard, Settings & Profile | 1. Mentorship Request form<br>2. My Mentorships screen<br>3. Mentor Dashboard with incoming requests (accept / decline)<br>4. Settings & Profile: edit profile, preferences, change password |
| 6 | Asampong Godswill Nana | 22013557 | Frontend Developer (Blazor) | Admin — Mentor Verification screen; client API layer and state (`Client/Services/Api.cs`, `AppState.cs`, `Client/Program.cs`) | 1. Admin Mentor Verification screen<br>2. Typed client for all API calls (`Api.cs`)<br>3. Signed-in user state, sign in / sign out (`AppState.cs`)<br>4. Client startup and API base URL configuration (`Program.cs`, `appsettings.json`) |
| 7 | Nathaniel Edem Aguze | 22024807 | Database Administrator | Data model (`Shared/Models/Entities.cs`), `Api/Data/AppDbContext.cs`, EF Core migrations (`Api/Migrations/`), seed data (`Api/Data/SeedData.cs`), Supabase PostgreSQL database | 1. Design the 9 entities: User, MentorProfile, Review, MentorshipRequest, Mentorship, Goal, Milestone, ChatMessage, Notification<br>2. Configure relationships in `AppDbContext`<br>3. Create the initial EF Core migration<br>4. Write the demo seed data<br>5. Set up the Supabase PostgreSQL database and connection |
| 8 | Adzraku Prosper Awoenam | 22042713 | Real-Time Messaging (SignalR) | SignalR hub (`Api/Hubs/ChatHub.cs`), `MessagesController`, Messages screen | 1. SignalR chat hub with live message delivery<br>2. Conversations list endpoint<br>3. Message thread endpoint<br>4. Messages screen: inbox, thread view and live updates |
| 9 | Akplu Kelvin Mawuli | 22042260 | Goal Tracking & Progress Module | `GoalsController`, Goal Tracker screen (goals, milestones, progress) | 1. Get a student's goals endpoint<br>2. Create goal with milestones endpoint<br>3. Toggle milestone complete endpoint<br>4. Goal Tracker screen with progress bars |
| 10 | Ofori Richard | 22106332 | Notifications & Feed System | `NotificationsController`, Notifications screen | 1. Get notifications endpoint<br>2. Mark all as read endpoint<br>3. Notifications screen<br>4. Group notifications into Today and Earlier |
| 11 | Dzah Solomon Sampson | 22012447 | Testing & QA | Automated API tests (`tests/MentorLink.Api.Tests/`), manual test plan (`docs/TEST_PLAN.md`) | 1. Set up the xUnit test project and in-memory test host (`MentorLinkFactory.cs`)<br>2. Password hashing tests<br>3. Login and sign-up tests<br>4. Access control tests: 401 without a token, 403 for non-admins<br>5. Mentorship request, accept and decline flow tests<br>6. Goal progress and notification tests<br>7. Manual checklist for the 14 screens at phone, tablet and desktop sizes<br>8. Report and re-test bugs |
| 12 | Glory Joy Seyram | 22150299 | Documentation & Presentation | Project proposal, this README, `docs/` (architecture, API reference, user guide), final presentation | 1. Write the project proposal<br>2. Write and maintain the README<br>3. Architecture overview (`docs/ARCHITECTURE.md`)<br>4. API reference (`docs/API_REFERENCE.md`)<br>5. User guide for students, mentors and admins (`docs/USER_GUIDE.md`)<br>6. Final presentation slides and demo script (to be added) |

Paths starting with `Api/`, `Client/` or `Shared/` are under `src/MentorLink.*`; `tests/` and `docs/` are at the repository root.

## Notes / known simplifications (fine for an MVP, fix before production)

- The chat hub trusts the user ID the client sends; it should use the signed-in user from the JWT instead.
- `GET /api/mentorships/student/{id}` doesn't require a token yet.
- Endpoints take user IDs in the URL and don't check they belong to the signed-in user.
- An empty database is seeded with demo accounts — turn this off before real users sign up.
- File attachments in chat and photo upload are placeholders.
