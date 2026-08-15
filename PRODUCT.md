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

AllStay is an in-house hotel guest-activities platform. Guests discover and book on-property activities (tours, experiences, amenities) during their stay; hotel staff manage the activity catalog and incoming reservations; hotels themselves are sold on adopting the platform via the institutional site. Confirmed as the complete core purpose — no broader guest-services/upsell/concierge scope beyond activities booking.

## Positioning

*Inferred from repository evidence, not explicitly confirmed:* instant, code/QR-based access to a hotel's activity catalog with no app install (PWA) for guests, paired with a lightweight staff-side tool to manage that catalog and its reservations. Differentiating claim not yet articulated by the user — do not invent competitive positioning language beyond this mechanism.

## Operating Context

- Guests scan a hotel-specific code at the property, land on their hotel's catalog, browse activities, view detail, book, and check `My Reservations` — all in a mobile PWA (app-guest, React + TypeScript + Vite + React Router).
- Hotel staff log into `web-hotel-portal` (ASP.NET Core Razor Pages) to manage `Activities` and `Reservations`.
- Hotel prospects land on `web-institutional` (ASP.NET Core Razor Pages: `Index`, `Contact`) to learn about AllStay and get in touch — B2B, not guest-facing.
- Backend: ASP.NET Core (.NET 8), layered Domain/Application/Infrastructure/Api, JWT auth for staff, a key-gated admin API for onboarding hotels/staff (no super-admin UI — ops-only).
- Deployment: self-contained win-x64 builds to HostAzul (Plesk/IIS); Brazilian market, Portuguese (pt-BR) content; subdomains per app (see `DEPLOYMENT.md`).
- This project is also an MBA capstone (TCC) deliverable — supporting docs (financial model, user guide, pitch deck) live at the repo root and in `docs/`.

## Capabilities and Constraints

- **app-guest**: pages are `Home`, `HotelEntry`, `Catalog`, `ActivityDetail`, `MyReservations`. No native app install; PWA only.
- **web-hotel-portal**: pages are `Login`, `Activities`, `ActivityDetail`, `Reservations`. Staff-authenticated.
- **web-institutional**: pages are `Index`, `Contact`. No auth; public marketing site.
- `web-backoffice` (Blazor) duplicates staff-portal functionality; relationship to `web-hotel-portal` is an open/undecided fact (see Users).
- No customer testimonials, case studies, or press exist yet — do not fabricate any for the redesign.

## Brand Commitments

- Product name: **AllStay**.
- Logo asset exists at repo root (`logo.png`) — reuse rather than replace unless the user asks for a rebrand.

## Evidence on Hand

- `logo.png` (repo root) — brand mark to reuse.
- `docs/AllStay_Guia_do_Usuario.pdf` / `.docx` — end-user guide; may contain real feature/copy detail worth mining before writing UI copy.
- `All Stay Entrega 2.pdf`, `AllStay_Pagamentos.pptx`, `All_Stay_planilha_preenchida.xlsx`, `FIAP_Modelo_Projecao_Financeira_MVP.xlsx`, `Capitulo_6_Modelo_de_Monetizacao_e_Viabilidade_Financeira_AllStay_final.docx` — academic/business deliverables (financial model, monetization chapter, pitch deck), not necessarily source for UI content, but may hold real positioning/monetization language worth checking before inventing marketing copy for `web-institutional`.
- No testimonials, benchmarks, or pricing claims are confirmed — none should be fabricated for the institutional site.

## Product Principles

1. Guests should reach their hotel's activity catalog with minimal friction — scan, browse, book, no install.
2. Hotel staff need a fast, low-training tool to keep a live activity/reservation catalog current.
3. The institutional site exists to build B2B trust with hotel decision-makers and convert interest into contact — treat it as Persuade, not just informational.
4. As an MBA capstone deliverable, visual and interaction polish is itself part of what's being evaluated, not incidental — craft matters beyond "it works."
