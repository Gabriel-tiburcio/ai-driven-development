# AllStay — Deploy to HostAzul (Plesk/IIS)

Subdomains:
| App | Subdomain | Publish output |
|---|---|---|
| Institutional website | `allstay.eupanda.com.br` | `web-institutional/bin/Release/net8.0/win-x64/publish/` |
| Guest PWA (static) | `app.allstay.eupanda.com.br` | `app-guest/dist/` |
| Hotel backoffice | `hotel.allstay.eupanda.com.br` | `web-backoffice/bin/Release/net8.0/win-x64/publish/` |
| Backend API | `api.allstay.eupanda.com.br` | `backend/src/AllStay.Api/bin/Release/net8.0/win-x64/publish/` |

All three .NET apps target **.NET 8 (LTS)** and publish **self-contained** (`win-x64`) — the server doesn't need the .NET runtime installed ("autosuficiente"). Originally built on .NET 9, but retargeted to 8 after hitting `HTTP 500.32 — Failed to load .NET Core host` on HostAzul, which only has the .NET 6/8 hosting bundle.

**If you hit 500.32 again:** it usually means the IIS app pool is running 32-bit (`Enable 32-Bit Applications = True` in Plesk's hosting settings). A `win-x86` build of the API is pre-built at `backend/publish-x86/` — try that instead of `backend/publish/` if the 64-bit one fails.

## 0) Database (one-time)

The production DB (`202_allstay` on `192.168.20.3,3533`) needs its schema created once. From a machine that can reach that host (likely has to be done on/near the HostAzul server, since it's a private IP):

```
sqlcmd -S 192.168.20.3,3533 -d 202_allstay -U 202_allstay -P <password> -i backend/deploy/InitialCreate.sql
```

Or run it through whatever DB management panel HostAzul provides (paste the contents of `backend/deploy/InitialCreate.sql`). This creates all tables but does **not** seed demo data — the seeder only runs in the `Development` environment. Create the first hotel/staff user for real either via a one-off script or by temporarily adding an admin endpoint (not included in this build — flag if you want one).

If you ever add new EF migrations later, regenerate the script instead of re-running the old one:
```
dotnet ef migrations script --idempotent -p src/AllStay.Infrastructure -s src/AllStay.Api -o deploy/<name>.sql
```
(`--idempotent` makes it safe to re-run — it skips migrations already applied.)

## 1) Backend API → `api.allstay.eupanda.com.br`

```
cd backend
dotnet publish src/AllStay.Api -p:PublishProfile=HostAzul
```
Upload the contents of `src/AllStay.Api/bin/Release/net8.0/win-x64/publish/` to the `api.allstay.eupanda.com.br` site root in Plesk.

**Set these as environment variables** on the Plesk site (Websites & Domains → api.allstay... → Environment Variables, or PHP/ASP.NET settings panel — exact location depends on your Plesk skin), or by editing `web.config` on the server after upload:

```
ConnectionStrings__Default = Server=192.168.20.3,3533;Database=202_allstay;User Id=202_allstay;Password=<the real password>;TrustServerCertificate=True
Jwt__SigningKey             = <a long random secret — generate a new one, don't reuse the dev one>
Admin__ApiKey               = ecdc688b06bc7c07419b8f7ac9adb55210abd1d3679a2974
```

Never commit real values for these into the repo — `appsettings.Production.json` intentionally leaves them blank. `Admin__ApiKey` above is a freshly generated value for this deploy — treat it like a password (keep it out of chat history/screenshots once you've copied it into Plesk).

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
cd web-backoffice
dotnet publish -p:PublishProfile=HostAzul
```
Upload `bin/Release/net8.0/win-x64/publish/` to the `hotel.allstay.eupanda.com.br` site root. Same as above — `Api:BaseUrl` is already set, nothing secret to configure.

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
