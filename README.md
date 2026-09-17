# AllStay

AllStay é uma plataforma de concierge digital para hotéis. O hóspede acessa via QR code/código do hotel, sem instalar app, e navega por 9 módulos de conteúdo e serviços da estadia; a equipe do hotel gerencia tudo isso por um painel administrativo; hotéis em potencial conhecem o produto pelo site institucional.

Este repositório também é o entregável de um TCC de MBA (FIAP) — os documentos de suporte (modelo financeiro, guia de uso, pitch) ficam na raiz e em `docs/`.

## Módulos do POC

Todos os 9 módulos abaixo estão implementados ponta a ponta (backend + admin no `web-hotel-portal` + `app-guest`). O **Concierge Premium** tem IA real: o chat do hóspede conversa de verdade com um LLM (DeepSeek), usando o conteúdo cadastrado pelo hotel (`ConciergeKnowledgeEntry`) como contexto — RAG simples por *context stuffing*, sem vetorização.

| # | Módulo | Guest (`app-guest`) | Admin (`web-hotel-portal`) |
|---|---|---|---|
| 1 | Restaurante | `Restaurante` / `RestauranteDetail` | `Restaurante` |
| 2 | Concierge Premium | `Concierge` (chat com IA real) | `ConciergeKnowledge` |
| 3 | Informações | `Informacoes` | `Informacoes` |
| 4 | Eventos | `Eventos` / `EventoDetail` | `Eventos` |
| 5 | Atividades | `Catalog` (`/atividades`) / `ActivityDetail` / `MyReservations` | `Activities` / `ActivityDetail` / `Reservations` |
| 6 | Serviços | `Servicos` / `ServicoDetail` | `Servicos` |
| 7 | Experiências Externas | `ExperienciasExternas` / `ExperienciaExternaDetail` | `ExperienciasExternas` |
| 8 | Solicitações | `Solicitacoes` | `Solicitacoes` |
| 9 | Recreação Infantil | `RecreacaoInfantil` / `RecreacaoInfantilDetail` | `RecreacaoInfantil` |

Detalhes de escopo, usuários e princípios de produto em [`PRODUCT.md`](./PRODUCT.md).

## Estrutura do repositório

- **`backend/`** — API ASP.NET Core (.NET 8/9), Clean Architecture (`Domain` / `Application` / `Infrastructure` / `Api`). SQL Server, EF Core, JWT para staff, integração com a DeepSeek para o Concierge Premium.
- **`app-guest/`** — PWA do hóspede (React + TypeScript + Vite + React Router). Acesso via `/h/<código-do-hotel>`, sem login.
- **`web-hotel-portal/`** — painel administrativo da equipe do hotel (ASP.NET Core Razor Pages).
- **`web-institutional/`** — site institucional B2B (ASP.NET Core Razor Pages).
- **`docs/`** — guia de uso (screenshot-based), radar tecnológico e demais materiais do TCC.

## Rodando localmente

```
# Backend
cd backend/src/AllStay.Api
dotnet run

# App do hóspede
cd app-guest
npm install
npm run dev

# Portal do hotel
cd web-hotel-portal
dotnet run
```

⚠️ O backend não tem uma configuração de banco local separada — `appsettings.json` aponta para o banco de produção (HostAzul). Qualquer `dotnet ef database update` ou dado de teste criado localmente afeta esse banco; aplique migrações com cuidado. Veja [`DEPLOYMENT.md`](./DEPLOYMENT.md) para o processo de deploy.
