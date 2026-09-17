# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

- **Hotel guests** — travelers staying at a participating hotel. They reach the guest app by scanning a hotel-specific code/QR at the property (`app.allstay.eupanda.com.br/h/<code>`), then browse and book on-property activities during their stay, with no app install required (React PWA).
- **Hotel staff** — property employees who manage the activity catalog and reservations day to day, via `web-hotel-portal` (the in-scope staff surface for this redesign). Roles recorded in the backend: Manager, FrontDesk, ActivityCoordinator.
- **Hotel decision-makers (B2B prospects)** — hotel owners/managers evaluating whether to adopt AllStay for their property. They land on `web-institutional`, the marketing/sales site.

*Resolved (2026-09-16):* `web-hotel-portal` (Razor Pages) is the real, deployed staff portal (`hotel.allstay.eupanda.com.br`). `web-backoffice` (Blazor) is a stale near-duplicate, not deployed — treat `web-hotel-portal` as the single staff surface going forward.

## Product Purpose

AllStay is an in-house hotel guest-experience platform. Guests use it to discover and, where applicable, book everything relevant to their stay — dining, hotel information, events, activities, paid services, external experiences, requests to staff, kids' recreation, and an AI concierge; hotel staff manage the underlying catalogs, content, and incoming reservations/requests; hotels themselves are sold on adopting the platform via the institutional site. The POC scope is the 9 modules below (see POC Modules) — all 9 are implemented full-stack (backend + `web-hotel-portal` admin + `app-guest`), including a **working AI Concierge** (added 2026-09-15/16, originally a mock — see below).

## Positioning

*Inferred from repository evidence, not explicitly confirmed:* instant, code/QR-based access to a hotel's activity catalog with no app install (PWA) for guests, paired with a lightweight staff-side tool to manage that catalog and its reservations. Differentiating claim not yet articulated by the user — do not invent competitive positioning language beyond this mechanism.

## Operating Context

- Guests scan a hotel-specific code at the property, land on a hub (`/hub`) listing all 9 modules, and browse/book/request within whichever they need — all in a mobile PWA (app-guest, React + TypeScript + Vite + React Router).
- Hotel staff log into `web-hotel-portal` (ASP.NET Core Razor Pages) to manage each module's catalog/content, plus incoming reservations, hire requests, guest requests, and kids-activity enrollments.
- Hotel prospects land on `web-institutional` (ASP.NET Core Razor Pages: `Index`, `Contact`) to learn about AllStay and get in touch — B2B, not guest-facing.
- Backend: ASP.NET Core (.NET 8), layered Domain/Application/Infrastructure/Api, JWT auth for staff, a key-gated admin API for onboarding hotels/staff (no super-admin UI — ops-only).
- Deployment: self-contained win-x64 builds to HostAzul (Plesk/IIS); Brazilian market, Portuguese (pt-BR) content; subdomains per app (see `DEPLOYMENT.md`).
- This project is also an MBA capstone (TCC) deliverable — supporting docs (financial model, user guide, pitch deck) live at the repo root and in `docs/`.

## POC Modules

*User-defined scope (2026-08-30). All 9 implemented full-stack as of 2026-08-30, except the noted Concierge Premium exception. Order reflects the user's numbering, not build priority.*

1. **Restaurante** — hotel's dining options: restaurants, hours, description, menu, other relevant info. View-only in the POC (per original scope, "pode evoluir para reservas/pedidos" later). Backend `Restaurant`; guest pages `Restaurante`/`RestauranteDetail`; admin `web-hotel-portal/Pages/Restaurante`.
2. **Concierge Premium** — AI-based concierge, real as of 2026-09-16. Guest-facing chat (`app-guest/src/pages/Concierge.tsx`) calls `POST /api/hotels/{hotelId}/concierge/chat` (`ConciergeChatController` → `ConciergeChatService`), which builds a system prompt from the hotel's `ConciergeKnowledgeEntry` records (simple RAG by context stuffing — no vector store, justified by the small per-hotel content volume) and calls an external LLM. **Provider migrated twice, both times for cost**: Anthropic (Claude) → Google Gemini → **DeepSeek** (current, OpenAI-compatible `chat/completions` API). Gemini separately hit an unresolved 403 `PERMISSION_DENIED` on its associated Google Cloud project around the same time, but that was not the reason for the switch. External recommendations (nearby restaurants/tours) rely on the model's own general knowledge — Gemini's Google Search grounding was tried and dropped along with the provider switch; the system prompt explicitly forbids inventing places and caps response length/tone (short, no mention of the model lacking internet access, front-desk suggested only as a last resort). Backoffice "training content" area (`web-hotel-portal/Pages/ConciergeKnowledge`) supports create/edit/delete.
3. **Informações** — centralizes general hotel info guests need during their stay: hours, Wi-Fi, rules, check-in/check-out, parking, contacts, facilities, etc. Backend `HotelInfoSection`; guest page `Informacoes`; admin `web-hotel-portal/Pages/Informacoes`.
4. **Eventos** — one-off happenings promoted by the hotel. Backend `EventItem`; guest pages `Eventos`/`EventoDetail`; admin `web-hotel-portal/Pages/Eventos`.
5. **Atividades** — free/internal activities offered by the hotel. The original reference implementation — `app-guest` (Catalog/ActivityDetail/MyReservations) and `web-hotel-portal` (Activities/Reservations) — that all other modules' vertical slices (entity → DTOs → service → controller → admin page → guest page) were modeled on.
6. **Serviços** — paid in-hotel services (massage, spa, private lesson, etc.). Backend `Service` + `ServiceRequest` (hire request, no time-slot booking); guest pages `Servicos`/`ServicoDetail`; admin `web-hotel-portal/Pages/Servicos` (catalog + requests list).
7. **Experiências Externas** — paid off-property experiences brokered by the hotel. Backend `ExternalExperience` + `ExperienceRequest`; guest pages `ExperienciasExternas`/`ExperienciaExternaDetail`; admin `web-hotel-portal/Pages/ExperienciasExternas`.
8. **Solicitações** — guest requests items/service directly instead of calling the front desk. Backend `GuestRequest` (status: Pending/InProgress/Done); guest page `Solicitacoes` (create + own-room list); admin `web-hotel-portal/Pages/Solicitacoes` (list + status updates).
9. **Recreação Infantil** — kids' programming with enrollment. Backend `KidsActivity` + `KidsEnrollment`; guest pages `RecreacaoInfantil`/`RecreacaoInfantilDetail`; admin `web-hotel-portal/Pages/RecreacaoInfantil` (catalog + create + enrollments list).

## Capabilities and Constraints

- **app-guest**: post-QR/code entry now lands on `/hub`, a grid linking to all 9 modules. Pages: `Home`, `HotelEntry`, `Hub`, `Catalog`(`/atividades`)/`ActivityDetail`/`MyReservations`, `Restaurante`/`RestauranteDetail`, `Concierge` (mock chat), `Informacoes`, `Eventos`/`EventoDetail`, `Servicos`/`ServicoDetail`, `ExperienciasExternas`/`ExperienciaExternaDetail`, `Solicitacoes`, `RecreacaoInfantil`/`RecreacaoInfantilDetail`. Bottom nav: Início / Concierge / Reservas. No native app install; PWA only.
- **web-hotel-portal**: pages are `Login`, `Activities`/`ActivityDetail`, `Reservations`, plus one admin page per module above (`Informacoes`, `Eventos`, `Restaurante`, `Servicos`, `ExperienciasExternas`, `Solicitacoes`, `RecreacaoInfantil`, `ConciergeKnowledge`). All 8 content pages support create/edit/delete (edit opens a native `<dialog>` modal; delete/success/error feedback via SweetAlert2). `Admin/Index.cshtml` (super-admin, `AdminApiKey`-gated) manages hotels including `Address`/`City` (added 2026-09-15, used by the Concierge prompt).
- **Auth is a deliberate POC no-op (2026-09-16)**: JWT is still issued at login and the staff UX/session is unchanged, but the API no longer validates the token or enforces `[Authorize]` on any staff endpoint (`HotelScopedControllerBase.TryAuthorizeForHotel` is a no-op) — any `hotelId` in the URL works for any caller. Explicit, confirmed user decision ("é apenas uma POC que não será divulgada"); revisit before any real launch.
- **Database**: EF Core migrations for all new entities have been generated; confirm with the user whether `dotnet ef database update` has been run against the production DB (`backend/src/AllStay.Api/appsettings.json`'s `ConnectionStrings:Default`) before assuming a given module's tables exist — this repo has no separate local/dev database, so migrations are applied directly to production and require explicit user approval each time. Migrations do **not** run automatically outside `Development` — apply them manually (idempotent script via `dotnet ef migrations script --idempotent`) after every deploy that adds one.
- **web-institutional**: pages are `Index`, `Contact`. No auth; public marketing site.
- `web-backoffice` (Blazor) is a stale, undeployed duplicate of `web-hotel-portal` — not in use (see Users).
- No customer testimonials, case studies, or press exist yet — do not fabricate any for the redesign.

## Brand Commitments

- Product name: **AllStay**.
- Logo asset exists at repo root (`logo.png`) — reuse rather than replace unless the user asks for a rebrand.

## Evidence on Hand

- `logo.png` (repo root) — brand mark to reuse.
- `docs/AllStay_Guia_de_Uso_Tutorial.pdf` / `docs/allstay-guia-de-uso.html` — screenshot-based end-user guide (guest + backoffice), built from `docs/allstay-guia-de-uso.template.html`; being updated to cover all 9 modules.
- `All Stay Entrega 2.pdf`, `AllStay_Pagamentos.pptx`, `All_Stay_planilha_preenchida.xlsx`, `FIAP_Modelo_Projecao_Financeira_MVP.xlsx`, `Capitulo_6_Modelo_de_Monetizacao_e_Viabilidade_Financeira_AllStay_final.docx` — academic/business deliverables (financial model, monetization chapter, pitch deck), not necessarily source for UI content, but may hold real positioning/monetization language worth checking before inventing marketing copy for `web-institutional`.
- No testimonials, benchmarks, or pricing claims are confirmed — none should be fabricated for the institutional site.

## Product Principles

1. Guests should reach their hotel's stay-relevant content and services with minimal friction — scan, browse, book/request, no install.
2. Hotel staff need a fast, low-training tool to keep each module's catalog/content and reservations/requests current.
3. The institutional site exists to build B2B trust with hotel decision-makers and convert interest into contact — treat it as Persuade, not just informational.
4. As an MBA capstone deliverable, visual and interaction polish is itself part of what's being evaluated, not incidental — craft matters beyond "it works."
