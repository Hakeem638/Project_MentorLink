# MentorLink — Architecture

MentorLink is a mentorship platform: students find verified mentors, send mentorship requests, chat in real time and track goals with milestones.

## Components

```
Browser (Blazor WebAssembly)
   │  HTTPS + JWT bearer token            WebSocket (SignalR)
   ▼                                          ▼
ASP.NET Core Web API  ───────────────  ChatHub  (/hubs/chat)
   │  Entity Framework Core
   ▼
PostgreSQL (Supabase)   — or an in-memory database for local development
```

| Project | Purpose |
|---------|---------|
| `src/MentorLink.Client` | Blazor WebAssembly front end: 14 screens, layouts, shared components, typed API client |
| `src/MentorLink.Api` | REST controllers, SignalR chat hub, JWT auth, EF Core `AppDbContext`, migrations, seed data |
| `src/MentorLink.Shared` | Entities and DTOs used by both client and server, so both sides agree on the data shapes |
| `tests/MentorLink.Api.Tests` | xUnit integration tests that boot the real API against an in-memory database |

## Deployment

| Part | Host | Deployed from |
|------|------|---------------|
| Front end | Vercel — https://mentorlink-client.vercel.app | `build-vercel.sh` publishes the client; `vercel.json` sends every route to `index.html` so refreshing a page works |
| API + chat hub | Render — https://project-mentorlink.onrender.com | Root `Dockerfile` |
| Database | Supabase PostgreSQL (session pooler) | EF Core migrations run automatically when the API starts |

The client reads the API address from `wwwroot/appsettings.json` (`ApiBaseUrl`). For local runs `appsettings.Development.json` blanks it so the client calls the API it is served from.

## Data model

| Entity | Key fields |
|--------|------------|
| `User` | name, email, hashed password, role (Student / Mentor / Admin), field, bio, notification preferences |
| `MentorProfile` | title, company, skills, availability, verification status (Pending / Approved / Rejected) |
| `Review` | mentor, student name, 1–5 rating, text |
| `MentorshipRequest` | student, mentor, goal type, message, preferred frequency, status (Pending / Accepted / Declined) |
| `Mentorship` | student, mentor, status (Active / Completed), start date, last session |
| `Goal` / `Milestone` | a student's goal, optionally linked to a mentor, broken into milestones that can be ticked off |
| `ChatMessage` | sender, recipient, content, sent time, read flag |
| `Notification` | recipient, kind (request received / accepted, new message, milestone completed …), title, body, read flag |

## Key flows

**Sign in.** `POST /api/auth/login` checks the PBKDF2 password hash and returns a JWT. The client stores the session in `localStorage` (so a refresh keeps you signed in) and `AuthHeaderHandler` attaches the token to every API call.

**Mentorship request.** A student sends a request → the mentor gets a notification → the mentor accepts (a `Mentorship` is created and the student is notified) or declines.

**Chat.** The Messages screen connects to `/hubs/chat` and calls `Register(userId)`. `SendMessage` saves the message, creates a notification and pushes `ReceiveMessage` to both people instantly.

**Goals.** Ticking a milestone recalculates progress; when every milestone is done the goal is marked Completed. Each completed milestone creates a notification.

## Responsive layout

All screens work on phones and tablets. Below 860px the sidebar becomes a top bar with a menu button, and Messages shows either the conversation list or one open chat (with a back button). Styles live in `src/MentorLink.Client/wwwroot/css/app.css`.
