# MentorLink — API Reference

Base URL: `https://project-mentorlink.onrender.com` in production, `http://localhost:5180` locally.

Endpoints marked 🔒 need an `Authorization: Bearer <token>` header; the token comes from login or sign-up. Without it they return `401 Unauthorized`. Request and response bodies are JSON; enums are sent as numbers.

## Auth

| Method | Path | Body | Returns |
|--------|------|------|---------|
| POST | `/api/auth/login` | `{ email, password }` | `{ user, token }` · `401` if the email or password is wrong |
| POST | `/api/auth/signup` | `{ fullName, email, field, password, role }` | `{ user, token }` · `409` if the email is taken. Mentors start as *Pending* verification |

## Users 🔒

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/users/{id}` | Get a profile |
| PUT | `/api/users/{id}` | Update name, email, field, bio, links, photo |
| PUT | `/api/users/{id}/preferences` | Update notification and privacy toggles |
| POST | `/api/users/{id}/change-password` | `{ currentPassword, newPassword }` |

## Mentors 🔒

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/mentors?search=` | Verified mentors for Discover, optionally filtered by name, field or company |
| GET | `/api/mentors/{userId}` | Full mentor profile with skills and reviews |
| POST | `/api/mentors/{userId}/reviews` | `{ studentName, rating, text }` — add a review |

## Mentorship requests 🔒

| Method | Path | Purpose |
|--------|------|---------|
| POST | `/api/requests` | `{ studentId, mentorUserId, goalType, message, frequency }` — send a request; notifies the mentor |
| POST | `/api/requests/{id}/accept` | Accept; creates an active mentorship and notifies the student. `409` if already handled |
| POST | `/api/requests/{id}/decline` | Decline |

## Mentorships

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/mentorships/student/{studentId}` | Active, pending and past mentorships for My Mentorships |

## Goals 🔒

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/goals/student/{studentId}` | A student's goals with milestones |
| POST | `/api/goals` | `{ studentId, title, type, mentorUserId?, milestones[] }` — blank milestones are ignored |
| POST | `/api/goals/milestones/{milestoneId}/toggle` | Tick / untick a milestone; returns the updated goal |

## Messages 🔒

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/messages/conversations/{userId}` | Conversation list with last message and unread count |
| GET | `/api/messages/thread/{userId}/{partnerId}` | Full message history between two users |

### Real-time chat — SignalR hub `/hubs/chat`

| Direction | Name | Payload |
|-----------|------|---------|
| client → server | `Register` | `userId` — join your personal group after connecting |
| client → server | `SendMessage` | `{ senderId, recipientId, content }` |
| server → client | `ReceiveMessage` | `{ id, senderId, recipientId, content, sentAt }` — sent to both people |

## Notifications 🔒

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/notifications/{userId}` | Newest first |
| POST | `/api/notifications/{userId}/read-all` | Mark all as read |

## Dashboards 🔒

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/dashboard/student/{studentId}` | Stats, active mentors and goals for the Student Dashboard |
| GET | `/api/dashboard/mentor/{mentorUserId}` | Stats, pending requests and mentees for the Mentor Dashboard |

## Admin 🔒 (Admin role only — others get `403`)

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/api/admin/verifications` | Mentor applications |
| POST | `/api/admin/verifications/{profileId}/approve` | Approve — mentor appears in Discover |
| POST | `/api/admin/verifications/{profileId}/reject` | Reject |
