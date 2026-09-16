# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

- **Hotel guests** — travelers staying at a participating hotel. They reach the guest app by scanning a hotel-specific code/QR at the property (`app.allstay.eupanda.com.br/h/<code>`), then browse and book on-property activities during their stay, with no app install required (React PWA).
- **Hotel staff** — property employees who manage the activity catalog and reservations day to day, via `web-hotel-portal` (the in-scope staff surface for this redesign). Roles recorded in the backend: Manager, FrontDesk, ActivityCoordinator.
- **Hotel decision-makers (B2B prospects)** — hotel owners/managers evaluating whether to adopt AllStay for their property. They land on `web-institutional`, the marketing/sales site.

*Open item:* a second staff-facing implementation (`web-backoffice`, Blazor) exists with near-duplicate pages (Activities, Reservations, Login) to `web-hotel-portal` (Razor Pages). The user did not resolve whether these are redundant, split by role, or one is being retired. Not blocking — `web-backoffice` is out of scope for the current redesign request; revisit if it needs its own pass later.

## Product Purpose

AllStay is an in-house hotel guest-experience platform. Guests use it to discover and, where applicable, book everything relevant to their stay — dining, hotel information, events, activities, paid services, external experiences, requests to staff, kids' recreation, and an AI concierge; hotel staff manage the underlying catalogs, content, and incoming reservations/requests; hotels themselves are sold on adopting the platform via the institutional site. The POC scope is the 9 modules below (see POC Modules) — all 9 are now implemented full-stack (backend + `web-hotel-portal` admin + `app-guest`), with one deliberate exception: Concierge Premium's guest-facing chat stays a static visual mock (no working AI), per the module's original scope.

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
2. **Concierge Premium** — AI-based concierge with context from all of AllStay plus hotel-supplied info. **Guest-facing chat is intentionally still a static visual mock (`app-guest/src/pages/Concierge.tsx`), no working AI** — this was the specified scope, not a gap. What's real: the backoffice "training content" area (`ConciergeKnowledgeEntry` backend entity, `web-hotel-portal/Pages/ConciergeKnowledge`) where hotels can already feed information/instructions for the agent, ahead of real AI wiring.
3. **Informações** — centralizes general hotel info guests need during their stay: hours, Wi-Fi, rules, check-in/check-out, parking, contacts, facilities, etc. Backend `HotelInfoSection`; guest page `Informacoes`; admin `web-hotel-portal/Pages/Informacoes`.
4. **Eventos** — one-off happenings promoted by the hotel. Backend `EventItem`; guest pages `Eventos`/`EventoDetail`; admin `web-hotel-portal/Pages/Eventos`.
5. **Atividades** — free/internal activities offered by the hotel. The original reference implementation — `app-guest` (Catalog/ActivityDetail/MyReservations) and `web-hotel-portal` (Activities/Reservations) — that all other modules' vertical slices (entity → DTOs → service → controller → admin page → guest page) were modeled on.
6. **Serviços** — paid in-hotel services (massage, spa, private lesson, etc.). Backend `Service` + `ServiceRequest` (hire request, no time-slot booking); guest pages `Servicos`/`ServicoDetail`; admin `web-hotel-portal/Pages/Servicos` (catalog + requests list).
7. **Experiências Externas** — paid off-property experiences brokered by the hotel. Backend `ExternalExperience` + `ExperienceRequest`; guest pages `ExperienciasExternas`/`ExperienciaExternaDetail`; admin `web-hotel-portal/Pages/ExperienciasExternas`.
8. **Solicitações** — guest requests items/service directly instead of calling the front desk. Backend `GuestRequest` (status: Pending/InProgress/Done); guest page `Solicitacoes` (create + own-room list); admin `web-hotel-portal/Pages/Solicitacoes` (list + status updates).
9. **Recreação Infantil** — kids' programming with enrollment. Backend `KidsActivity` + `KidsEnrollment`; guest pages `RecreacaoInfantil`/`RecreacaoInfantilDetail`; admin `web-hotel-portal/Pages/RecreacaoInfantil` (catalog + create + enrollments list).

## Capabilities and Constraints

- **app-guest**: post-QR/code entry now lands on `/hub`, a grid linking to all 9 modules. Pages: `Home`, `HotelEntry`, `Hub`, `Catalog`(`/atividades`)/`ActivityDetail`/`MyReservations`, `Restaurante`/`RestauranteDetail`, `Concierge` (mock chat), `Informacoes`, `Eventos`/`EventoDetail`, `Servicos`/`ServicoDetail`, `ExperienciasExternas`/`ExperienciaExternaDetail`, `Solicitacoes`, `RecreacaoInfantil`/`RecreacaoInfantilDetail`. Bottom nav: Início / Concierge / Reservas. No native app install; PWA only.
- **web-hotel-portal**: pages are `Login`, `Activities`/`ActivityDetail`, `Reservations`, plus one admin page per module above (`Informacoes`, `Eventos`, `Restaurante`, `Servicos`, `ExperienciasExternas`, `Solicitacoes`, `RecreacaoInfantil`, `ConciergeKnowledge`). Staff-authenticated.
- **Database**: EF Core migrations for all new entities have been generated; confirm with the user whether `dotnet ef database update` has been run against the production DB (`backend/src/AllStay.Api/appsettings.json`'s `ConnectionStrings:Default`) before assuming a given module's tables exist — this repo has no separate local/dev database, so migrations are applied directly to production and require explicit user approval each time.
- **web-institutional**: pages are `Index`, `Contact`. No auth; public marketing site.
- `web-backoffice` (Blazor) duplicates staff-portal functionality; relationship to `web-hotel-portal` is an open/undecided fact (see Users).
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
