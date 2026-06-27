# TODOAPI-Auth — TODO API with Authentication & Third-Party Integrations

A production-quality RESTful API built with **ASP.NET Core Web API**, **Entity Framework Core**, and **JWT Authentication**. Features full user authentication, one-to-many relationships, and integrations with **OpenWeatherMap**, **Slack**, **GitHub**, and **Google Calendar**.

---

## Tech Stack

| Technology | Purpose |
|---|---|
| C# / ASP.NET Core Web API | Core framework |
| .NET 8 | Runtime |
| Entity Framework Core | Database ORM |
| SQL Server | Database |
| JWT (JSON Web Tokens) | Authentication & Authorization |
| BCrypt.Net | Password hashing |
| Swagger / OpenAPI | API documentation & testing |
| OpenWeatherMap API | Weather-based todo suggestions |
| Slack Incoming Webhooks | Todo event notifications |
| GitHub REST API | Import issues as todos |
| Google Calendar API | Sync todos to calendar events |

---

## Project Structure

```
TODOAPI-Auth/
├── Controllers/
│   ├── AuthController.cs          # Register & Login
│   ├── TodoItemsController.cs     # CRUD + integrations
│   └── WebhooksController.cs      # Webhook receivers
├── Data/
│   └── ApplicationDbContext.cs    # EF Core DB context
├── DTOs/
│   ├── RegisterDTO.cs
│   ├── LoginDTO.cs
│   ├── AuthResponseDTO.cs
│   ├── CreateTodoDTO.cs
│   ├── UpdateTodoDTO.cs
│   ├── TodoResponseDto.cs
│   ├── GitHubIssueDto.cs
│   └── WeatherDto.cs
├── Helpers/
│   └── JwtHelper.cs               # JWT token generation
├── Models/
│   ├── User.cs                    # User model
│   ├── TodoItem.cs                # Todo model (FK to User)
│   ├── GitHubIssue.cs
│   ├── SlackModels.cs
│   ├── WeatherModels.cs
│   ├── WebhookModels.cs
│   └── GoogleCalendarEventPayload.cs
├── Services/
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── ITodoService.cs
│   ├── TodoService.cs
│   ├── WeatherService.cs
│   ├── SlackService.cs
│   ├── GitHubService.cs
│   └── GoogleCalendarService.cs
├── appsettings.json               # Config with placeholders
└── Program.cs                     # App entry point
```

---

## Database Setup

### Prerequisites
- .NET 8 SDK
- SQL Server or SQL Server Express
- Visual Studio 2022

### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ToDoDB-User;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Run Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## Getting Started

```bash
# 1. Clone the repository
git clone https://github.com/peculiarprecious/TODOAPI-Auth.git

# 2. Navigate to project
cd TODOAPI-Auth

# 3. Restore packages
dotnet restore

# 4. Create appsettings.Development.json with your real API keys

# 5. Run migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

# 6. Run the project
dotnet run

# 7. Open Swagger UI
# https://localhost:7064/swagger
```

---

## Authentication

This API uses **JWT Bearer Authentication**. All todo endpoints require a valid token.

### POST /api/Auth/register
```http
POST /api/Auth/register
Content-Type: application/json

{
  "firstName": "Precious",
  "lastName": "Nwajei",
  "email": "precious@example.com",
  "password": "Password123!"
}
```

Response `201 Created`:
```json
{
  "userId": 1,
  "email": "precious@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### POST /api/Auth/login
```http
POST /api/Auth/login
Content-Type: application/json

{
  "email": "precious@example.com",
  "password": "Password123!"
}
```

Response `200 OK`:
```json
{
  "userId": 1,
  "email": "precious@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Using the Token
Add to all protected requests:
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Todo Endpoints

Base URL: `https://localhost:7064/api/TodoItems`

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/TodoItems/all` | Get all todos (admin view) |
| `GET` | `/api/TodoItems` | Get my todos |
| `POST` | `/api/TodoItems` | Create new todo |
| `GET` | `/api/TodoItems/statistics` | Get todo statistics |
| `GET` | `/api/TodoItems/search` | Search todos by keyword |
| `GET` | `/api/TodoItems/{id}` | Get todo by ID |
| `PUT` | `/api/TodoItems/{id}` | Update todo |
| `DELETE` | `/api/TodoItems/{id}` | Delete todo |
| `GET` | `/api/TodoItems/suggestions/weather` | Weather-based suggestions |
| `POST` | `/api/TodoItems/import-github-issues` | Import GitHub issues |

### Create Todo
```http
POST /api/TodoItems
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "title": "Buy groceries",
  "description": "Milk and eggs",
  "dueDate": "2026-12-01T00:00:00Z",
  "priority": "High"
}
```

Response `201 Created`:
```json
{
  "id": 1,
  "title": "Buy groceries",
  "description": "Milk and eggs",
  "isCompleted": false,
  "createdAt": "2026-06-27T10:00:00Z",
  "dueDate": "2026-12-01T00:00:00Z",
  "priority": "High"
}
```

### Search Todos
```http
GET /api/TodoItems/search?q=groceries
Authorization: Bearer YOUR_TOKEN
```

### Get Statistics
```http
GET /api/TodoItems/statistics
Authorization: Bearer YOUR_TOKEN
```

Response `200 OK`:
```json
{
  "totalTodos": 10,
  "completedTodos": 4,
  "pendingTodos": 6,
  "highPriority": 3,
  "mediumPriority": 4,
  "lowPriority": 3
}
```

### Validation Rules

| Field | Rules |
|---|---|
| `Title` | Required, 3–100 characters, no whitespace only |
| `Description` | Optional, max 500 characters |
| `Priority` | Must be `Low`, `Medium`, or `High` |
| `DueDate` | Optional, cannot be in the past |

---

## External APIs

---

### 1. OpenWeatherMap Integration

#### Setup
1. Sign up at [openweathermap.org](https://openweathermap.org)
2. Go to **API Keys** tab → copy your free API key
3. Add to `appsettings.Development.json`:

```json
"ExternalApis": {
  "OpenWeather": {
    "ApiKey": "your-api-key-here",
    "ApiUrl": "https://api.openweathermap.org/data/2.5"
  }
}
```

#### Endpoint
```http
GET /api/TodoItems/suggestions/weather?city=Krakow
Authorization: Bearer YOUR_TOKEN
```

#### Response Example
```json
{
  "weather": {
    "city": "Lagos",
    "temperature": 18.5,
    "description": "partly cloudy",
    "humidity": 65,
    "windSpeed": 3.2,
    "condition": "Cool"
  },
  "suggestions": [
    "Take a brisk walk",
    "Visit a museum or gallery",
    "Try a new cafe or restaurant",
    "Go shopping",
    "Attend a local event"
  ],
  "message": "Cool weather — layer up!"
}
```

#### Suggestion Logic
| Temperature | Suggestions |
|---|---|
| > 30°C | Stay indoors, hydrate |
| > 20°C | Outdoor activities |
| > 10°C | Light outdoor activities |
| ≤ 10°C | Indoor activities |

---

### 2. Slack Integration

Receive real-time Slack notifications when todos are created, completed, or deleted.

#### Setup
1. Go to [api.slack.com/apps](https://api.slack.com/apps)
2. Click **Create New App** → **From Scratch**
3. App Name: `TODO Bot` → Select your workspace
4. Click **Incoming Webhooks** → Toggle **ON**
5. Click **Add New Webhook to Workspace** → Select `#todo-alerts`
6. Copy the **Webhook URL**
7. Add to `appsettings.Development.json`:

```json
"ExternalApis": {
  "Slack": {
    "WebhookUrl": "https://hooks.slack.com/services/YOUR/WEBHOOK/URL"
  }
}
```

#### Test Your Webhook
```cmd
curl -X POST -H "Content-type: application/json" ^
--data "{\"text\":\"Hello from TODO API!\"}" ^
https://hooks.slack.com/services/YOUR/WEBHOOK/URL
```

#### Notification Examples

** Todo Created** (Blue `#3498db`):
```
New TODO Created
Title:       Buy groceries
Priority:    High          Created By: Precious Nwajei
Due Date:    2026-12-01
```

** Todo Completed** (Green `#2ecc71`):
```
 TODO Completed
Title:          Buy groceries
Completed By:   Precious      Priority: High
```

**Todo Deleted** (Red `#e74c3c`):
```
 TODO Deleted
Title:      Buy groceries
Priority:   High
```

---

### 3. GitHub Integration

Import open GitHub issues directly as todo items.

#### Setup
1. Go to **github.com** → Settings → Developer settings
2. Click **Personal access tokens** → **Tokens (classic)**
3. Click **Generate new token** → select `repo` and `read:user` scopes
4. Copy the token
5. Add to `appsettings.Development.json`:

```json
"ExternalApis": {
  "GitHub": {
    "Token": "ghp_your_personal_access_token",
    "Owner": "peculiarprecious",
    "Repo": "TODOAPI-Auth"
  }
}
```

#### Import GitHub Issues
```http
POST /api/TodoItems/import-github-issues
Authorization: Bearer YOUR_TOKEN
```

Response `200 OK`:
```json
{
  "createdTodos": 1
}
```

#### How Issues Become Todos

| GitHub Issue | Todo Created |
|---|---|
| Issue `#1` — `Fix bug` | `[GH-1] Fix bug` |
| Label `urgent` or `high` | Priority: `High` |
| Label `low` | Priority: `Low` |
| No priority label | Priority: `Medium` |
| High priority | Due date: +3 days |
| Medium priority | Due date: +7 days |
| Low priority | Due date: +14 days |

> Duplicate issues are skipped — the `[GH-1]` tag is checked before importing!

---

### 4. Google Calendar Integration

Sync todos with due dates to Google Calendar as events.

#### Setup
1. Go to [console.cloud.google.com](https://console.cloud.google.com)
2. Create project → Enable **Google Calendar API**
3. Go to **Google Auth Platform** → **Clients** → Create **OAuth 2.0 Client**
4. Add redirect URI: `https://developers.google.com/oauthplayground`
5. Get access token from [developers.google.com/oauthplayground](https://developers.google.com/oauthplayground):
   - Use own credentials → Enter Client ID + Secret
   - Add scope: `https://www.googleapis.com/auth/calendar`
   - Authorize APIs → Sign in → Allow
   - Step 2 → Exchange code → Copy `access_token`
6. Add to `appsettings.Development.json`:

```json
"ExternalApis": {
  "GoogleCalendar": {
    "ApiUrl": "https://www.googleapis.com/calendar/v3",
    "AccessToken": "ya29.your_access_token_here"
  }
}
```

> Access tokens expire after ~1 hour. Refresh via OAuth Playground when needed.

#### How It Works
When a todo with a `DueDate` is created, a Google Calendar event is automatically created:

---

##  Webhook Endpoints

Receive HTTP notifications from external services.

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/Webhooks/todo-created` | Receive todo created event |
| `POST` | `/api/Webhooks/github-issue` | Receive GitHub issue events |


Use that URL as your webhook endpoint:
```
https://abc123.ngrok.io/api/Webhooks/todo-created
https://abc123.ngrok.io/api/Webhooks/github-issue
```

---

### POST /api/Webhooks/todo-created

Receives notification when a todo is created.

**Example Payload:**
```json
{
  "userId": 1,
  "todoId": 1,
  "action": "created",
  "timestamp": "2026-06-27T10:00:00Z",
  "title": "Buy groceries",
  "priority": "High"
}
```

**Response** `202 Accepted`:
```json
{
  "message": "Webhook received and processing",
  "todoId": 1,
  "action": "created",
  "timestamp": "2026-06-27T10:00:00Z"
}
```

---

### POST /api/Webhooks/github-issue

Receives GitHub issue events (opened, closed, reopened).

**Payload — Issue Opened:**
```json
{
  "action": "opened",
  "issue": {
    "number": 1,
    "title": "Fix database indexing bottlenecks",
    "body": "Database queries are running slow under load.",
    "html_url": "https://github.com/peculiarprecious/TODOAPI-Auth/issues/1",
    "labels": [{ "name": "bug" }]
  }
}
```

**Payload — Issue Closed** (auto-completes matching todo):
```json
{
  "action": "closed",
  "issue": {
    "number": 1,
    "title": "Fix database indexing bottlenecks",
    "body": "Fixed!",
    "html_url": "https://github.com/peculiarprecious/TODOAPI-Auth/issues/1",
    "labels": []
  }
}
```

**Response** `202 Accepted`:
```json
{
  "message": "GitHub webhook received and processing",
  "action": "opened",
  "issueNumber": 1,
  "issueTitle": "Fix database indexing bottlenecks",
  "timestamp": "2026-06-27T10:00:00Z"
}
```

> When a GitHub issue is **closed**, the matching todo `[GH-1]` is automatically marked as complete!

---

##  Configuration

All secrets use placeholders in `appsettings.json`.
Create `appsettings.Development.json` with real values — **never commit this file!**

```json
{
  "Jwt": {
    "Key": "YOUR_JWT_SECRET_KEY_MIN_32_CHARS"
  },
  "ExternalApis": {
    "OpenWeather": {
      "ApiKey": "your_openweather_api_key",
      "ApiUrl": "https://api.openweathermap.org/data/2.5"
    },
    "GitHub": {
      "Token": "ghp_your_github_token",
      "Owner": "peculiarprecious",
      "Repo": "TODOAPI-Auth"
    },
    "Slack": {
      "WebhookUrl": "https://hooks.slack.com/services/YOUR/WEBHOOK/URL"
    },
    "GoogleCalendar": {
      "ApiUrl": "https://www.googleapis.com/calendar/v3",
      "AccessToken": "ya29.your_google_access_token"
    }
  }
}
```

### Where to Get Each Key

| Key | Where to Get |
|---|---|
| `Jwt:Key` | Any random 32+ character string |
| `OpenWeather:ApiKey` | [openweathermap.org](https://openweathermap.org) → API Keys |
| `GitHub:Token` | GitHub → Settings → Developer Settings → Personal Access Tokens |
| `Slack:WebhookUrl` | [api.slack.com/apps](https://api.slack.com/apps) → Incoming Webhooks |
| `GoogleCalendar:AccessToken` | [developers.google.com/oauthplayground](https://developers.google.com/oauthplayground) |

---

##  HTTP Status Codes

| Code | Meaning |
|---|---|
| `200 OK` | Request successful |
| `201 Created` | Resource created |
| `202 Accepted` | Webhook received and processing |
| `204 No Content` | Deleted successfully |
| `400 Bad Request` | Validation failed |
| `401 Unauthorized` | Missing or invalid JWT token |
| `404 Not Found` | Resource not found |
| `500 Internal Server Error` | Server error |

---

##  Architecture

```
HTTP Request
     ↓
[Authorize] → JWT Validation
     ↓
Controller → validates input & returns response
     ↓
ITodoService → interface contract
     ↓
TodoService → business logic & EF Core
     ↓
SQL Server Database
     ↓
Fire & Forget → Slack / Google Calendar notifications
```

---

##  Author

**Precious Nwajei**
- GitHub: [@peculiarprecious](https://github.com/peculiarprecious)

---

