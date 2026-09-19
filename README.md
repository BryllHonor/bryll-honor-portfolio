# Bryll Honor — Portfolio (ASP.NET Core MVC)

Same app as the Node/Express version, rebuilt as ASP.NET Core MVC with Razor
views, since the coursework repos are C#/.NET.

## Run it

```
dotnet run
```

Then open the URL it prints (usually `https://localhost:5001`).

Demo login: **admin** / **admin123**

## MVC folders, and how they map to the Node version

**Model — `Models/` + `Services/`**

- `Models/Project.cs` — the shape of one portfolio entry
- `Services/ProjectRepository.cs` — reads `Data/projects.json` (was `models/Project.js`)
- `Models/Comment.cs` — the shape of a comment
- `Services/CommentRepository.cs` — reads/writes `Data/comments.json` (was `models/Comment.js`)
- `Services/UserService.cs` — checks login credentials against the single hardcoded
  account in `appsettings.json`'s `Login` section (was `models/User.js` + `config/auth.js`)

**View — `Views/`** (Razor `.cshtml`, the HTML)

- `Views/Project/Index.cshtml` — table of contents (was `views/index.ejs`)
- `Views/Project/Show.cshtml` — project detail + comments (was `views/projects/show.ejs`)
- `Views/Auth/Login.cshtml`, `Views/Shared/Error.cshtml`
- `Views/Shared/_Layout.cshtml`, `_Header.cshtml`, `_Footer.cshtml`, `_Flash.cshtml` — shared
  chrome (was `views/partials/*.ejs`, merged into one layout the way ASP.NET Core expects)

**Controller — `Controllers/`**

- `ProjectController.cs` — handles requests for the TOC and detail pages, pulls data
  from the repositories, picks a view to render (was `controllers/projectController.js`)
- `AuthController.cs` — handles login/logout (was `controllers/authController.js`)
- `CommentController.cs` — handles posting/deleting comments (was `controllers/commentController.js`)
- `ErrorController.cs` — renders the shared error page for 404s and unhandled exceptions

`Program.cs` is the entry point: it's `app.js`'s two halves (service wiring, then
middleware pipeline) combined into ASP.NET Core's builder/app pattern. Routing is
attribute-based (`[HttpGet("/projects/{id}")]` on each action) instead of a separate
`routes/index.js`, so the URL each action responds to sits right next to the code
that handles it.

## What changed under the hood (same behavior, different building blocks)

| Node version | ASP.NET Core version |
|---|---|
| `express-session` + `req.session.user` | Cookie authentication (`[Authorize]`, `User.Identity`) |
| hand-rolled `middleware/csrf.js` | built-in antiforgery (`@Html.AntiForgeryToken()` / `[ValidateAntiForgeryToken]`) |
| `connect-flash` | `TempData["FlashSuccess"/"FlashError"]` |
| `bcryptjs` hash | PBKDF2-HMAC-SHA256 via `System.Security.Cryptography` (no extra package needed) |
| `helmet()` CSP headers | a small custom middleware in `Program.cs` doing the same headers |
| `express-rate-limit` on `/login` | built-in `Microsoft.AspNetCore.RateLimiting` fixed-window limiter |
| EJS auto-escaping (`<%= %>`) | Razor auto-escaping (`@value`) — same protection against stored XSS in comments |

No third-party NuGet packages are required — cookie auth, antiforgery, and rate
limiting all ship in the ASP.NET Core shared framework.

## Data

- `Data/projects.json` — the twelve tracked repos, unchanged from the Node version
- `Data/comments.json` — starts as `{}`; the app writes to it when someone comments
