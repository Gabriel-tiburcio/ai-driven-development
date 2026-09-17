# AllStay — Deploy to HostAzul (Plesk/IIS)

Subdomains:
| App | Subdomain | Publish output |
|---|---|---|
| Institutional website | `allstay.eupanda.com.br` | `web-institutional/bin/Release/net8.0/win-x64/publish/` |
| Guest PWA (static) | `app.allstay.eupanda.com.br` | `app-guest/dist/` |
| Hotel backoffice | `hotel.allstay.eupanda.com.br` | `web-hotel-portal/bin/Release/net8.0/win-x64/publish/` |
| Backend API | `api.allstay.eupanda.com.br` | `backend/src/AllStay.Api/bin/Release/net8.0/win-x64/publish/` |

*(`web-backoffice`, Blazor, is a stale duplicate — not deployed. `web-hotel-portal`, Razor Pages, is the real staff portal.)*

All three .NET apps target **.NET 8 (LTS)** and publish **self-contained** (`win-x64`) — the server doesn't need the .NET runtime installed ("autosuficiente"). Originally built on .NET 9, but retargeted to 8 after hitting `HTTP 500.32 — Failed to load .NET Core host` on HostAzul, which only has the .NET 6/8 hosting bundle.

**If you hit 500.32 again:** it usually means the IIS app pool is running 32-bit (`Enable 32-Bit Applications = True` in Plesk's hosting settings). A `win-x86` build of the API is pre-built at `backend/publish-x86/` — try that instead of `backend/publish/` if the 64-bit one fails.

## 0) Database (one-time, and again after every new migration)

The production DB is `202_allstay` on `bd.hulk.hostazul.com.br,3533`. Migrations **do not run automatically** in Production — `Program.cs` only calls `DbSeeder.SeedAsync` (which includes `Database.MigrateAsync`) when `ASPNETCORE_ENVIRONMENT=Development`. Every time a new EF migration is added, generate and run an idempotent script manually:

```
dotnet ef migrations script --idempotent -p src/AllStay.Infrastructure -s src/AllStay.Api -o deploy/<name>.sql
```
(`--idempotent` makes it safe to re-run — it skips migrations already applied, checked against `__EFMigrationsHistory`.)

Run the generated `.sql` against the production DB via whatever SQL client/panel HostAzul provides (SSMS, Azure Data Studio, etc.). **Close the client's transaction/session when done** — an open uncommitted transaction on a table (even from just running a `SELECT`) blocks every query the API makes against it, surfacing as `Execution Timeout Expired` errors in the app with no obvious cause (this happened once — see `progress.md` §12).

This creates/updates tables but does **not** seed demo data — the seeder only runs in `Development`. Create real hotels/staff via the admin API (see below).

## 1) Backend API → `api.allstay.eupanda.com.br`

```
cd backend
dotnet publish src/AllStay.Api -p:PublishProfile=HostAzul
```
Upload the contents of `src/AllStay.Api/bin/Release/net8.0/win-x64/publish/` to the `api.allstay.eupanda.com.br` site root in Plesk.

**Set these as environment variables** on the Plesk site (Websites & Domains → api.allstay... → Environment Variables, or PHP/ASP.NET settings panel — exact location depends on your Plesk skin), or by editing `web.config` on the server after upload:

```
ConnectionStrings__Default = Data Source=bd.hulk.hostazul.com.br,3533;Initial Catalog=202_allstay;User ID=202_allstay;Password=<the real password>;TrustServerCertificate=True
Jwt__SigningKey             = <a long random secret — still issued at login even though the API no longer validates it, see note below>
Admin__ApiKey               = ecdc688b06bc7c07419b8f7ac9adb55210abd1d3679a2974
DeepSeek__ApiKey            = <DeepSeek API key — note the DOUBLE underscore; DeepSeek:ApiKey or DEEPSEEK_APIKEY will silently fail>
```

Never commit real values for these into the repo — `appsettings.Production.json` intentionally leaves the secrets blank (this project keeps them in plain text in `appsettings.json` alongside the DB password/admin key, matching an existing project convention — a deliberate, discussed trade-off for a school POC, not an oversight).

⚠️ **JWT auth is a deliberate POC no-op as of 2026-09-16.** `Program.cs` still issues a JWT at staff login (so the portal session/UX is unchanged), but no longer registers `AddAuthentication/AddJwtBearer` or enforces `[Authorize]` on any staff endpoint — any `hotelId` in the URL works for any caller. Confirmed, explicit user decision, not a bug. Revisit before any real launch.

Verify: `https://api.allstay.eupanda.com.br/swagger` should load, and `GET /api/hotels/by-code/<some-code>` should hit the real DB.

### Onboarding the first real hotel + staff login

The demo seeder only runs in `Development`. In production, create hotels and their staff logins via the admin API (gated by the `Admin__ApiKey` header, not staff JWT — there's no super-admin UI, this is meant for you/ops, not hotel staff):

```
curl https://api.allstay.eupanda.com.br/api/admin/hotels \
  -X POST -H "X-Admin-Key: <Admin__ApiKey>" -H "Content-Type: application/json" \
  -d '{"name":"Nome do Hotel","code":"codigo-do-hotel","tier":0,"contactEmail":"...","contactPhone":null}'

curl https://api.allstay.eupanda.com.br/api/admin/hotels/<hotelId returned above>/staff \
  -X POST -H "X-Admin-Key: <Admin__ApiKey>" -H "Content-Type: application/json" \
  -d '{"email":"gerente@hotel.com","password":"...","fullName":"...","role":0}'
```
`code` is what guests type/scan (`app.allstay.eupanda.com.br/h/<code>`). `role`: `0`=Manager, `1`=FrontDesk, `2`=ActivityCoordinator.

## 2) Institutional website → `allstay.eupanda.com.br`

```
cd web-institutional
dotnet publish -p:PublishProfile=HostAzul
```
Upload `bin/Release/net8.0/win-x64/publish/` to the `allstay.eupanda.com.br` site root. `appsettings.Production.json` already points `Api:BaseUrl` at `https://api.allstay.eupanda.com.br` — no secrets here, nothing else to configure.

## 3) Hotel backoffice → `hotel.allstay.eupanda.com.br`

```
cd web-hotel-portal
dotnet publish -p:PublishProfile=HostAzul
```
Upload `bin/Release/net8.0/win-x64/publish/` to the `hotel.allstay.eupanda.com.br` site root. `Api:BaseUrl` and `Admin:ApiKey` are already set in `appsettings.Production.json` — nothing secret to configure here (the admin key used by super-admin pages is the same one set on the API above).

## 4) Guest PWA → `app.allstay.eupanda.com.br`

This one is a static build, not a .NET publish:
```
cd app-guest
npm install
npm run build
```
Upload the **contents** of `dist/` (not the folder itself) to the `app.allstay.eupanda.com.br` site root. `dist/web.config` handles SPA client-side routing and PWA MIME types automatically — it needs the IIS **URL Rewrite** module on the server (standard on most Plesk/Windows hosting plans; ask HostAzul support to confirm if routes 404 after deploy).

`.env.production` already points the build at `https://api.allstay.eupanda.com.br`.

## Order to deploy in

1. DB schema (step 0)
2. API (step 1) — the other three depend on it being reachable
3. Institutional site, backoffice, guest PWA (steps 2–4, any order)

## After deploy — smoke test

- `https://allstay.eupanda.com.br` loads, contact form submits successfully (creates a Lead via the API)
- `https://api.allstay.eupanda.com.br/swagger` loads
- `https://hotel.allstay.eupanda.com.br/login` loads (create a real hotel + staff user in the DB first — see step 0 note)
- `https://app.allstay.eupanda.com.br/h/<real-hotel-code>` resolves the hotel and shows its catalog
