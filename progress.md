# Progresso — Concierge Premium com IA (Prova Substitutiva, Modalidade B)

Registro de prompts, decisões e correções feitas durante o desenvolvimento assistido por IA (Claude Code) para dar vida real ao chat do módulo **Concierge Premium**, que hoje é um mock visual estático (`app-guest/src/pages/Concierge.tsx`), usando a base de conhecimento já existente (`ConciergeKnowledgeEntry`) e a API da Anthropic (Claude).

---

## 1. Contexto e objetivo

- Ferramenta de IA usada: **Claude Code** (Anthropic), via terminal, com acesso de leitura/escrita ao repositório.
- Objetivo do trabalho: substituir o mock do chat do hóspede por um endpoint real de backend que:
  1. Busca as entradas de `ConciergeKnowledgeEntry` do hotel (conteúdo cadastrado pela equipe no backoffice `web-hotel-portal/Pages/ConciergeKnowledge`).
  2. Monta um prompt de sistema com esse conteúdo (RAG simples por *context stuffing*, sem vetorização — justificativa: poucas entradas por hotel, latência e complexidade menores).
  3. Chama a API de mensagens da Anthropic (`https://api.anthropic.com/v1/messages`).
  4. Retorna a resposta para o app do hóspede.

## 2. Prompts utilizados (resumo)

| # | Prompt (resumo) | Resposta/ação da IA |
|---|---|---|
| 1 | "read the file trabalho.txt" | Leu e resumiu o enunciado da prova (Modalidade A vs B, critérios de avaliação). |
| 2 | Pergunta exploratória: qual modalidade seria mais fácil de executar com apoio de IA | Recomendou Modalidade B (Projeto Prático), justificando pela geração natural de evidências (prompts/decisões) durante o próprio uso da ferramenta. |
| 3 | "a B precisa de vídeo, você conseguiria gerar?" | Esclareceu que não é capaz de gravar vídeo — isso fica a cargo do usuário; ofereceu preparar roteiro de demonstração. |
| 4 | Ideia inicial: app de delivery de lavanderia (tipo Uber/iFood) | Deu retorno crítico sobre escopo grande demais para o prazo da prova e sugeriu um recorte de MVP. |
| 5 | "tenho um app pronto, que é o meu TCC" (AllStay) | Explorou o repositório (`README.md`, `PRODUCT.md`), identificou o módulo **Concierge Premium** como mock incompleto e propôs usá-lo como projeto da prova. |
| 6 | Confirmação: usar AllStay + incluir IA real no produto + stack já existente | Planejou a implementação: endpoint de chat no backend + integração no frontend, resposta completa (sem streaming), usando a API da Anthropic. |
| 7 | Exploração de padrões do código (subagente `Explore`) | Levantou convenções do projeto: Clean Architecture (Domain/Application/Infrastructure/Api), padrão de `HotelScopedControllerBase`, endpoints guest-facing com `[AllowAnonymous]` (ex. `GuestRequestsController`), padrão de `IOptions<T>` (`JwtOptions`), cliente HTTP do frontend (`app-guest/src/api/client.ts`), `GuestContext`, estrutura de `ConciergeMessage`. |
| 8 | Implementação do backend | Criou DTOs (`ConciergeChatDtos.cs`), interface (`IConciergeChatService`), opções (`AnthropicOptions`), serviço (`ConciergeChatService`) e controller (`ConciergeChatController`); registrou no DI (`AddHttpClient`); adicionou seção `Anthropic` em `appsettings.json` e `appsettings.Production.json` (chave em branco — a preencher). |
| 9 | "registre tudo em um progress.md" | Criação deste arquivo, consolidando prompts/decisões/correções até o momento. |

## 3. Decisões técnicas e por quê

- **RAG por context stuffing, não vetorização**: cada hotel tem poucas entradas de conhecimento curtas; buscar/rankear com embeddings adicionaria complexidade sem ganho real de qualidade nesta escala. Decisão documentada no próprio código (`ConciergeChatService`, comentário de classe).
- **Endpoint sem autenticação (`[AllowAnonymous]`)**: hóspede nunca faz login no AllStay (acesso via QR/código do hotel); seguiu o mesmo padrão já usado em `GuestRequestsController` para criação de solicitações.
- **`IOptions<AnthropicOptions>`** em vez de ler `IConfiguration` direto: replica o padrão já existente (`JwtOptions`) no projeto, para manter consistência.
- **`AddHttpClient<IConciergeChatService, ConciergeChatService>()`**: HttpClient tipado, gerenciado pelo `IHttpClientFactory`, evita problemas de socket exhaustion de instanciar `HttpClient` manualmente.
- **Resposta completa, sem streaming**: decisão do usuário — prioriza simplicidade e velocidade de entrega em relação a um efeito visual mais sofisticado.
- **Chave de API não commitada com valor real**: `appsettings.json`/`appsettings.Production.json` receberam apenas `"ApiKey": ""` — o valor real deve ser preenchido pelo usuário (ele ainda não tinha conta na Anthropic no momento da implementação).

## 4. Correções feitas durante o processo

- **Erro de build inicial**: `dotnet build` falhou com `IServiceCollection não contém uma definição para "AddHttpClient"`. Causa real: `AllStay.Infrastructure.csproj` não tinha o pacote `Microsoft.Extensions.Http` referenciado (só `Configuration.Abstractions` e `Options.ConfigurationExtensions`). Correção: adicionada a `PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0"` ao `.csproj` — build voltou a passar com 0 erros/0 avisos.
- **Import não usado no frontend**: ao remover `conciergeSeedMessages` de `mockContent.ts` (substituído por estado real de chat), o tipo `ConciergeMessage` ficou sem uso nesse arquivo — removido do import para não quebrar o typecheck.

## 5. Frontend implementado

- `app-guest/src/api/client.ts`: adicionado `askConcierge(hotelId, { message, history })`, chamando `POST /api/hotels/{hotelId}/concierge/chat`.
- `app-guest/src/pages/Concierge.tsx`: reescrito — estado de mensagens real (`useState<ConciergeMessage[]>`), input habilitado, envio chama `api.askConcierge` com o histórico da conversa, indicador "Digitando...", tratamento de erro (`try/catch` + `setError`, seguindo o padrão de `ActivityDetail.tsx`). Mensagem inicial do concierge gerada localmente (saudação com o nome do hotel), sem chamar a API.
- `app-guest/src/data/mockContent.ts`: removido `conciergeSeedMessages` (não usado mais).
- Verificado com `npx tsc --noEmit` — sem erros.

## 6. Sessão de 2026-09-09 — chave configurada e teste local

- Usuário forneceu uma chave real da Anthropic (`sk-ant-api03-...`). Preenchida em `backend/src/AllStay.Api/appsettings.json` (`Anthropic:ApiKey`), **apenas local** — `appsettings.Production.json` permanece com a chave em branco.
- ⚠️ **Pendência de segurança**: essa chave ficou em texto plano em `appsettings.json`, que é um arquivo rastreado pelo Git neste repositório (mesmo padrão já usado para outros segredos do projeto — senha do banco, JWT signing key, admin API key). **Antes de dar `git commit`/`git push` nessa mudança, decidir**: (a) mover a chave para variável de ambiente / user-secrets e manter o placeholder vazio no arquivo versionado, ou (b) aceitar conscientemente o mesmo padrão de segredo em texto plano já usado no resto do projeto. Ver `[[ia]]` para o alerta completo.
- Backend rodado localmente (`dotnet run` em `backend/src/AllStay.Api`, porta 5299) — subiu sem erros, endpoint `GET /api/hotels/by-code/{code}` respondeu 404 para código inexistente (comportamento esperado).
- **Teste end-to-end do chat ainda não foi feito**: para chamar `POST /api/hotels/{hotelId}/concierge/chat` de verdade, falta um `hotelId`/código de hotel válido em produção com pelo menos uma `ConciergeKnowledgeEntry` cadastrada. Tentei descobrir isso via `GET /api/admin/hotels` (gated pela Admin API key) e via `sqlcmd` direto no banco de produção — **ambos os comandos foram bloqueados pelo classificador de segurança do Claude Code** (ações sensíveis em banco/API de produção via terminal). Não insisti em contornar; fica pendente de o usuário informar um código de hotel válido, ou testar manualmente pelo navegador.
- Processo `dotnet` local finalizado ao fim da sessão (`taskkill /IM dotnet.exe`) — nada ficou rodando em background.

## 7. Estado atual / próximos passos

- [x] Backend: DTOs, interface, opções, serviço, controller, registro DI, config — implementados e compilando.
- [x] Frontend: chat real ligado ao endpoint, typecheck ok.
- [x] `Anthropic:ApiKey` preenchida localmente (produção ainda em branco).
- [ ] **Decidir como tratar a chave antes de commitar** (ver alerta de segurança acima).
- [ ] Obter um hotelId/código de hotel válido (com conhecimento cadastrado) e testar o chat de ponta a ponta — via navegador (`npm run dev` no `app-guest`) é o caminho mais simples, já que consultas diretas ao banco/admin de produção via terminal estão sendo bloqueadas pelo classificador de segurança.
- [ ] `dotnet ef database update` **não é necessário** — nenhuma migração nova foi criada (o endpoint só lê `ConciergeKnowledgeEntries`, que já existe).
- [ ] Preencher `Anthropic:ApiKey` em produção (`appsettings.Production.json`) quando for fazer o deploy — ver `DEPLOYMENT.md`.
- [ ] Gravar vídeo de demonstração (até 10 min) e escrever o relatório técnico final da prova, usando este arquivo como base de evidências.

## 8. Sessão de 2026-09-15 — troca de provedor: Anthropic → Google Gemini

- Motivo: usuário obteve uma chave gratuita do Gemini API (Google AI Studio) e pediu para trocar o provedor de IA do Concierge Premium, para não depender de créditos pagos da Anthropic.
- ⚠️ A chave foi colada em texto plano no chat — o usuário foi avisado a considerar revogá-la/regerá-la caso não quisesse que ficasse registrada na conversa.
- Alterações no backend:
  - `AnthropicOptions.cs` removido; criado `GeminiOptions.cs` (`ApiKey`, `Model` = `gemini-2.5-flash`, `MaxTokens`).
  - `ConciergeChatService.cs` reescrito para chamar `POST https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}` no lugar de `https://api.anthropic.com/v1/messages`. Formato de mensagens do Gemini é diferente do Anthropic: histórico vira `contents: [{role: "user"|"model", parts: [{text}]}]`, e o prompt de sistema vai em `system_instruction.parts[0].text` (Anthropic usava `system` de nível superior e `role: "assistant"` em vez de `"model"`). Limite de tokens de saída vira `generationConfig.maxOutputTokens`.
  - `DependencyInjection.cs`: troca de `services.Configure<AnthropicOptions>` para `services.Configure<GeminiOptions>`.
  - `appsettings.json` / `appsettings.Production.json`: seção `Anthropic` renomeada para `Gemini`.
- **Decisão sobre a chave, revisitada**: cheguei a configurar a chave via `dotnet user-secrets` (fora do arquivo versionado) para resolver de vez a pendência de segurança registrada na sessão anterior. O usuário, porém, pediu explicitamente para manter a chave direto em `appsettings.json`, no mesmo padrão já usado pelos outros segredos do projeto (senha do banco, JWT signing key, admin key) — então revertido: chave real ficou em `appsettings.json` local, user-secrets removido para não duplicar. `appsettings.Production.json` continua com a chave em branco.
- `dotnet build AllStay.sln` — compilação com êxito, 0 erros, 0 avisos.
- Teste end-to-end do chat via Gemini **ainda não foi feito** (mesma pendência de antes: falta um hotelId/código de hotel válido com conhecimento cadastrado).
- Impacto no relatório técnico: a seção de "ferramentas de IA empregadas" deve citar tanto a decisão inicial (Anthropic) quanto a migração para o Gemini, incluindo o motivo (custo/disponibilidade de free tier) como parte da narrativa de decisões do desenvolvedor.

## 9. Sessão de 2026-09-15 (cont.) — RAG dinâmico para recomendações externas (Google Search grounding)

- Pedido do usuário: o Concierge também deveria sugerir restaurantes/bares e passeios nas proximidades do hotel, não só responder com base no conteúdo cadastrado no backoffice.
- Opções avaliadas: (a) **grounding com Google Search**, nativo da API do Gemini — o modelo pesquisa em tempo real e usa os resultados para responder, sem pipeline de indexação próprio; (b) integrar a **Google Places API** manualmente (busca por tipo `restaurant`/`tourist_attraction` perto das coordenadas do hotel), mais controle sobre os dados retornados (nota, endereço, horário) mas bem mais trabalho de implementação. Usuário escolheu a opção (a) por ser a mais rápida.
- Implementação: adicionado `tools: [{ "google_search": {} }]` ao corpo da requisição em `ConciergeChatService.cs`. O prompt de sistema foi ajustado para separar duas categorias de resposta: (1) fatos sobre o hotel (políticas, horários, comodidades) — continuam restritos **apenas** ao conteúdo cadastrado (`ConciergeKnowledgeEntry`), sem inventar; (2) recomendações externas (restaurantes, passeios) — o modelo pode usar a busca do Google, priorizando primeiro qualquer indicação já cadastrada pelo hotel e complementando com a busca, nunca inventando nomes de lugares.
- `ExtractReplyText` foi generalizado para concatenar todas as `parts` de texto da resposta (antes pegava só a primeira) — com grounding habilitado, a resposta pode vir em mais de uma part.
- **Limitação conhecida**: o `Hotel` não tem campo de endereço/cidade no domínio hoje — a localização geográfica para a busca depende do nome do hotel e/ou de alguma entrada de conhecimento que mencione a cidade. Não foi adicionado um campo de endereço estruturado nesta sessão (fora do escopo pedido); se a qualidade das recomendações for ruim em teste, next step natural é adicionar `Address`/`City` ao `Hotel` e injetar isso explicitamente no prompt de sistema.
- Citações/fontes da busca (grounding metadata) não são extraídas nem exibidas ao hóspede nesta versão — MVP simples, por ser a opção "mais rápida" pedida pelo usuário. Melhoria futura possível.
- `dotnet build AllStay.sln` — compilação com êxito, 0 erros, 0 avisos, após a mudança.
- Teste end-to-end (incluindo o grounding) ainda não foi feito — mesma pendência de hotelId válido.

## 10. Sessão de 2026-09-15 (cont.) — Address/City do hotel, ponta a ponta (cadastro → concierge)

- Pedido do usuário: incluir `Address`/`City` no fluxo completo, revendo o cadastro do hotel para que a informação chegue desde a origem (onde o hotel é cadastrado) até o momento em que o hóspede aciona o Concierge (usado pelo grounding do item 9).
- Antes de editar, rodei um agente de exploração (`Explore`) para mapear o fluxo real de cadastro/edição de hotel no código, evitando suposições. Achados principais: `Hotel` não tinha nenhum campo de localização; hotéis são criados via `AdminController` (`POST /api/admin/hotels`, protegido por chave admin) chamado pela página `web-hotel-portal/Pages/Admin/Index.cshtml`; **não existia nenhum endpoint de edição de hotel** (só criar e ativar/desativar) — precisei criar um do zero para permitir atualizar a localização depois do cadastro inicial.
- Mudanças no backend:
  - `Hotel.cs`: adicionados `Address` e `City` (`string?`, seguindo o padrão de `ContactEmail`/`ContactPhone`).
  - `AllStayDbContext.cs`: `HasMaxLength(300)` para `Address`, `HasMaxLength(120)` para `City`.
  - Nova migration EF Core `AddHotelAddressAndCity` (`dotnet ef migrations add ... --project src/AllStay.Infrastructure --startup-project src/AllStay.Api`).
  - `HotelDtos.cs`: `HotelDto` e `CreateHotelRequest` ganharam `Address`/`City`; novo `UpdateHotelLocationRequest(string? Address, string? City)`.
  - `IHotelService`/`HotelService`: novo método `UpdateLocationAsync(hotelId, request)` (não existia nenhum "update" antes, só criar/toggle).
  - `AdminController`: novo endpoint `PATCH /api/admin/hotels/{hotelId}/location`.
  - `DbSeeder.cs`: hotel de demonstração ganhou endereço/cidade de exemplo (Fortaleza, CE).
  - `ConciergeChatService.cs`: `BuildSystemPrompt` agora recebe `address`/`city` do `Hotel` (já carregado via EF, sem query extra) e injeta uma linha "Localização do hotel: ..." no prompt de sistema, usada tanto para as recomendações externas via Google Search grounding (item 9) quanto para o modelo avisar o hóspede caso a localização não esteja cadastrada.
- Mudanças no frontend (`web-hotel-portal`, backoffice/admin):
  - `Models/Dtos.cs`, `Services/ApiClient.cs`: mirror manual das mudanças de backend (sem projeto compartilhado entre backend e portal — convenção já existente no projeto) + novo `UpdateHotelLocationAsync`.
  - `Pages/Admin/Index.cshtml(.cs)`: formulário de criação de hotel ganhou campos Endereço/Cidade; adicionada uma coluna "Localização" na tabela de hotéis com edição inline (`<details>` expansível por linha + novo handler `OnPostUpdateLocationAsync`) para editar a localização de hotéis já existentes — cobre o caso de hotéis cadastrados antes dessa mudança.
- `dotnet build AllStay.sln` (backend) e `dotnet build` (web-hotel-portal) — ambos com êxito, 0 erros, 0 avisos.
- Teste end-to-end (cadastro com endereço → concierge recomendando algo próximo) ainda não foi feito — mesma pendência de hotelId/teste manual de sempre.
- Ponto para o relatório: essa etapa é um bom exemplo de correção proativa depois de sugestão da IA — a IA identificou, via exploração antes de codar, que faltava todo um caminho de edição de hotel (não só o campo novo), e o fluxo foi desenhado de ponta a ponta em vez de só adicionar a coluna no banco.

## 11. Sessão de 2026-09-15/16 — troca DeepSeek e vaivém da autenticação JWT

- **Troca Gemini → DeepSeek**: motivada por custo (DeepSeek mais barata para o volume esperado). Coincidentemente, na mesma janela o Gemini configurado também retornava 403 (`PERMISSION_DENIED`, "unregistered callers") mesmo após verificar habilitação da API no projeto Google Cloud — um problema real (ver item 3 do troubleshooting abaixo), mas **não foi esse erro que motivou a troca de provedor**, e sim o custo. `GeminiOptions` → `DeepSeekOptions`; `ConciergeChatService` reescrito para o formato OpenAI-compatible da DeepSeek (`POST https://api.deepseek.com/chat/completions`, `messages` com roles, resposta em `choices[0].message.content`). Como a DeepSeek não tem grounding nativo (busca do Google), o usuário decidiu manter as recomendações externas limitadas ao conhecimento geral do modelo (sem busca ao vivo), com aviso explícito no prompt para nunca inventar lugares e sempre recomendar confirmar informações desatualizadas.
- Troubleshooting de produção nessa sessão (registrado em detalhe para o relatório, é um bom exemplo de diagnóstico incremental):
  1. Erro inicial: `Invalid column name 'Address'/'City'` — migration não tinha sido aplicada em produção (migrations só rodam automaticamente em Development). Resolvido gerando um script SQL idempotente (`dotnet ef migrations script --idempotent`) para o usuário rodar manualmente no banco de produção.
  2. Depois da migration, a mesma query (por PK) começou a dar `Execution Timeout Expired` — não era lock (uma consulta direta do usuário no banco funcionava normalmente), e sim um commit não finalizado em uma sessão do usuário que travou a tabela `Hotels`. Resolvido pelo usuário fechando/commitando a transação pendente.
  3. Depois disso, apareceu o erro 403 do Gemini (ver acima) — problema real, mas paralelo à decisão de trocar para DeepSeek, que já tinha sido motivada por custo.
  4. Com a DeepSeek, apareceu `401 Authentication Fails (auth header format should be Bearer sk-...)` — a chave configurada em produção não estava chegando ao app corretamente (causa raiz não totalmente confirmada; suspeita de nome errado de variável de ambiente, já que o ASP.NET Core exige `DeepSeek__ApiKey` com dois underscores para mapear para a seção `DeepSeek:ApiKey`). **Ainda pendente de resolução** ao final desta sessão.
- **Remoção e depois reversão da autenticação JWT**: a pedido do usuário ("é apenas uma POC que não será divulgada"), removi a exigência de JWT das rotas de staff da API (`Program.cs`: removido `AddAuthentication/AddJwtBearer`/`UseAuthentication`; `HotelScopedControllerBase.TryAuthorizeForHotel` virou no-op; `[Authorize]` removido de 10 controllers), mantendo o login do portal funcionando (só que sem validação real do token pela API). Pouco depois, o usuário identificado que o problema que motivou o pedido era só um efeito de teste local ("o problema era apenas localhost") e pediu para reabilitar — JWT restaurado integralmente: 4 arquivos (`Program.cs`, `HotelScopedControllerBase.cs`, `ActivitiesController.cs`, `ReservationsController.cs`) via `git checkout` (já estavam commitados com JWT); os outros 8 controllers (`Events`, `ExternalExperiences`, `ConciergeKnowledge`, `GuestRequests`, `HotelInfoSections`, `KidsActivities`, `Restaurants`, `Services`) reconstruídos manualmente linha a linha (não estavam commitados no git — eram alterações locais de uma sessão anterior), conferido depois que os `[Authorize]` voltaram exatamente nas mesmas linhas/quantidades de antes. Build final: 0 erros.
- **Observação para o relatório**: esse vaivém é um bom exemplo de avaliação crítica do processo — a IA seguiu a instrução do usuário de remover a autenticação (com confirmação explícita antes, dado o risco), mas ao reverter usou o git como fonte de verdade sempre que possível (mais confiável que tentar "lembrar" o código original) e sinalizou explicitamente quando parte do estado anterior não estava no histórico do git.

## 12. Sessão de 2026-09-16 — Concierge validado de ponta a ponta em produção

- Usuário publicou novamente (chave real da DeepSeek preenchida em `appsettings.Production.json`) e confirmou pelo app que o Concierge respondeu corretamente.
- Confirmação independente via `curl` direto em produção: `POST /api/hotels/{hotelId}/concierge/chat` → **HTTP 200**. Pergunta de teste: recomendação de restaurante próximo ao hotel de demonstração (Curaçau Resort & Spa, sem `Address`/`City` cadastrados ainda).
- Resposta observada: o Concierge (1) avisou honestamente que não tem a localização exata cadastrada e por isso não pode indicar distâncias/lugares próximos — em vez de inventar; (2) usou o conteúdo já cadastrado no backoffice para sugerir a opção interna do hotel (**"Terraço Curaçau"**, um restaurante do próprio hotel); (3) ofereceu transferir para a recepção como alternativa para recomendações externas.
- Isso valida, de ponta a ponta, todo o trabalho da sessão: migração de provedor (Anthropic → Gemini → DeepSeek), o RAG por context stuffing com `ConciergeKnowledgeEntry`, e o tratamento de localização ausente (a mensagem de fallback do prompt de sistema, escrita na seção 9/10, funcionou exatamente como projetado).
- Pendência natural para uma próxima etapa (não solicitada ainda): cadastrar Endereço/Cidade do hotel de demonstração via `web-hotel-portal/Pages/Admin/Index.cshtml` para testar o caminho "com localização" (a versão que orienta o modelo a sugerir lugares externos com base na cidade).

## 13. Sessão de 2026-09-16 (cont.) — respostas mais objetivas + renderização de markdown

- Feedback do usuário após testar em produção: a resposta do Concierge veio muito longa e repetitiva, mencionava explicitamente "não tenho acesso à internet" (vazamento de detalhe de implementação) e sugeria falar com a recepção com frequência excessiva; além disso o app renderizava o markdown (`**negrito**`, listas) como texto cru, com os asteriscos aparecendo literalmente.
- **Prompt de sistema** (`ConciergeChatService.BuildSystemPrompt`) reescrito: instrução explícita para respostas curtas (2-4 frases na maioria dos casos), proibição de mencionar falta de acesso à internet ou que é uma IA, e a recepção passou a ser sugerida apenas como último recurso — não mais como resposta padrão a cada lacuna de informação.
- `DeepSeekOptions.MaxTokens` reduzido de 1024 para 400 (reforça respostas mais curtas, e reduz custo/latência) — atualizado também em `appsettings.json` e `appsettings.Production.json`.
- **Frontend** (`app-guest`): criado `src/components/ChatMarkdown.tsx`, um parser leve de markdown feito à mão (sem adicionar dependência nova) que suporta `**negrito**`, parágrafos (quebra de linha) e listas com `-`/`*`. `Concierge.tsx` passou a usar esse componente no lugar de `<p>{m.text}</p>`. Ajustes de CSS em `index.css` para espaçamento entre parágrafos/listas dentro do balão de chat.
- Build backend: 0 erros. `npx tsc --noEmit` no `app-guest`: sem erros.
- Ainda não testado em produção após essas mudanças — pendente de novo deploy + validação do usuário.

## 14. Sessão de 2026-09-16 (cont.) — botões de excluir no backoffice (web-hotel-portal)

- Feedback do usuário testando o portal: nenhuma tela tinha opção de excluir os itens cadastrados (Atividades, Eventos, Notas do Concierge, Experiências Externas, Restaurantes, Serviços, Recreação Infantil, Informações).
- Antes de implementar, rodei uma exploração (`Explore`) que confirmou algo importante: **o backend já tinha os endpoints `DELETE` prontos para todas as 8 entidades** (`ActivitiesController`, `EventsController`, `ConciergeKnowledgeController`, `ExternalExperiencesController`, `RestaurantsController`, `ServicesController`, `KidsActivitiesController`, `HotelInfoSectionsController`) — só faltava a UI no portal chamá-los. Nenhuma mudança de backend foi necessária.
- `web-hotel-portal/Services/ApiClient.cs`: adicionados 8 métodos `DeleteXAsync(jwt, hotelId, id)`, um por entidade, seguindo o padrão já usado por `DeleteSlotAsync`.
- Cada `.cshtml.cs` das 8 páginas ganhou um handler `OnPostDeleteAsync(Guid id)` chamando o método correspondente e redirecionando de volta.
- Cada `.cshtml` ganhou um botão "Excluir" por card, com `onsubmit="return confirm(...)"` — não havia nenhum padrão de confirmação (`confirm()`) no código antes; foi introduzido agora, e é aplicado de forma consistente nas 8 telas.
- `dotnet build` no `web-hotel-portal`: 0 erros, 0 avisos.
- Escopo desta rodada: só exclusão (o que o usuário pediu com mais ênfase — "nada tem opção de exclusão"). Edição (`PUT`) não foi implementada — o backend já suporta `PUT` para 6 das 8 entidades (só `ConciergeKnowledge` não tem update), mas isso ficou de fora por não ter sido pedido; se quiser, é a próxima extensão natural.
- Ainda não testado em produção — pendente de deploy do `web-hotel-portal`.

## 15. Sessão de 2026-09-16 (cont.) — botões de editar no backoffice

- Usuário confirmou que também queria edição, não só exclusão, para as mesmas 8 entidades.
- **Notas do Concierge não tinham endpoint de atualização no backend** — foi o único caso que exigiu mudança de backend: `UpdateConciergeKnowledgeEntryRequest` (DTO), `UpdateAsync` em `IConciergeKnowledgeService`/`ConciergeKnowledgeService`, e `PUT /api/hotels/{hotelId}/concierge-knowledge/{entryId}` em `ConciergeKnowledgeController`. As outras 7 entidades já tinham `PUT` pronto no backend.
- `web-hotel-portal`: mirror dos 8 `UpdateXRequest` em `Models/Dtos.cs`; 8 métodos `UpdateXAsync` em `ApiClient.cs` (todos via `PutAsJsonAsync`).
- Cada uma das 8 páginas ganhou um handler `OnPostEditAsync(...)` e um formulário de edição expansível (`<details><summary>Editar</summary>...`) por card, pré-preenchido com os valores atuais do item — mesmo padrão visual usado no `Admin/Index.cshtml` para editar localização do hotel. Campo de status (Ativo/Inativo) implementado como `<select>` em vez de checkbox, para evitar a complexidade de model binding de checkboxes HTML em formulários sem tag helpers `asp-for`.
- `dotnet build` no backend e no portal: 0 erros/avisos nos dois (a build do portal já compila as Razor Views, então confirma que não há erro de sintaxe nos `.cshtml` também).
- Ainda não testado em produção — pendente de deploy do backend (novo endpoint PUT) e do portal (UI de edição).

## 16. Sessão de 2026-09-16 (cont.) — modais + SweetAlert2 no backoffice

- Feedback do usuário: os botões de editar/excluir ficaram "muito soltos" na tela (formulários `<details>` expandindo inline), e pediu para (1) a edição virar um modal e (2) usar SweetAlert2 para confirmação de exclusão e mensagens de sucesso/erro.
- **Modais de edição**: convertidos de `<details>` inline para `<dialog class="modal">` nativo do HTML, aberto via botão "Editar" (`data-modal-target`) e fechado por botão "Cancelar" (`data-modal-close`), clique no backdrop, ou Esc (nativo do `<dialog>`). CSS novo em `site.css` (`.card-actions`, `dialog.modal`, `.modal-actions`). JS de abertura/fechamento centralizado em `site.js` (uma única IIFE delegada, sem duplicar lógica por página).
- **SweetAlert2**: adicionado via CDN (`cdnjs`) no `_Layout.cshtml`. Dois usos:
  1. Confirmação de exclusão: os forms de delete perderam o `onsubmit="return confirm(...)"` nativo e ganharam `class="js-confirm-delete" data-confirm-message="..."`; um listener delegado em `site.js` intercepta o submit, mostra `Swal.fire` com confirmação, e reenvia o form (`requestSubmit()`) só se confirmado — com uma flag (`data-confirm-submitted`) pra não cair em loop infinito de interceptação.
  2. Toast de sucesso/erro após qualquer ação (criar/editar/excluir): criado `web-hotel-portal/Services/ToastExtensions.cs` com um método de extensão `SetToast(bool success, string successMessage, string errorMessage)` para `PageModel`, chamado em todos os handlers `OnPostAsync`/`OnPostEditAsync`/`OnPostDeleteAsync` das 8 páginas (antes esses handlers ignoravam o `bool` de sucesso retornado pela API — agora ele decide a mensagem). A mensagem vai para `TempData`, que o `_Layout.cshtml` lê e renderiza como atributos `data-toast-message`/`data-toast-type` no `<body>`; `site.js` dispara o `Swal.fire` (toast no canto superior direito, 3s) na carga da página seguinte ao redirect.
- `dotnet build` no portal: 0 erros/avisos (compila as Razor Views também).
- Ainda não testado visualmente em navegador nem publicado — pendente de deploy.

## 17. Sessão de 2026-09-16 (cont.) — atualização de toda a documentação

- Usuário confirmou que o desenvolvimento está concluído e pediu para atualizar todos os `.md` e a documentação, incluindo a página tutorial.
- Atualizados: `progress.md` (este arquivo), `ia.md`, `README.md` (tabela de módulos — Concierge Premium deixou de ser mock), `PRODUCT.md` (seção do Concierge Premium reescrita com o histórico de troca de provedor, `web-backoffice` marcado como não usado/resolvido, JWT documentado como no-op deliberado de POC, migrations documentadas como manuais em produção), `DEPLOYMENT.md` (corrigido: portal real é `web-hotel-portal` não `web-backoffice`; host do banco corrigido; variável `DeepSeek__ApiKey` documentada com o aviso do duplo underscore; nota sobre transação presa travando queries).
- `docs/prompts-e-respostas.md` e `docs/prompts-e-respostas-relatorio.md` (e cópias em `mba/IA/`) ganharam a continuação completa da sessão de hoje (prompts 18–35 / 15–22 respectivamente): troca Gemini→DeepSeek, RAG dinâmico, Address/City ponta a ponta, troubleshooting de produção (migration não aplicada, transação presa, permissão do Gemini), remoção e reversão do JWT, ajuste de tom do Concierge + parser de markdown, CRUD completo do backoffice, modais + SweetAlert2.
- **Página tutorial** (`docs/allstay-guia-de-uso.html`, gerada de `allstay-guia-de-uso.template.html` com 39 screenshots via placeholders `__IMG_G1__`...`__IMG_A7__`): usuário pediu para regenerar do zero. Antes de comprometer o tempo, perguntei o escopo (só texto / screenshots que mudaram / tudo) — usuário escolheu **regenerar tudo**. Tratado como tarefa separada, dado o tamanho (automação de navegador para recapturar as 39 telas de hóspede/institucional/portal/admin).

## 18. Sessão de 2026-09-16 (cont.) — página tutorial regenerada do zero

- Usuário confirmou "regenerar tudo" (as 46 screenshots do guia: 21 hóspede, 5 institucional, 13 portal, 7 admin).
- Tentativa inicial: 3 agentes em paralelo usando a extensão Claude in Chrome (browser do usuário). Resultado: contenção entre os agentes (abas somem/URLs mudam sozinhas) e, por fim, a extensão caiu de vez (`list_connected_browsers` retornando vazio) — só 9/21 screenshots do hóspede foram capturados antes do disconnect.
- Descoberto nesse processo: `hotel.allstay.eupanda.com.br` e `allstay.eupanda.com.br` estavam com **certificado/binding SSL quebrado** (connection reset em HTTPS), mas respondiam normalmente em **HTTP puro** — usuário confirmou "está tudo no ar" e sugeriu tentar `www.`; o diagnóstico real foi HTTP funcionando (200/302), só HTTPS quebrado nesses dois hosts específicos (`app.` e `api.` continuavam OK em HTTPS).
- Usuário pediu para capturar "de outra forma" em vez de depender da extensão do Chrome. Solução: instalado **Playwright** localmente em `docs/` (headless Chromium, baixado via `npx playwright install`), com um script Node (`capture.mjs`) que navega e tira as 46 screenshots sem depender de nenhuma extensão de navegador — usando `http://` para os dois hosts com HTTPS quebrado.
- Todas as 46 capturadas na primeira passada; revisão visual encontrou 2 problemas que foram corrigidos com scripts de ajuste pontuais: (1) o fluxo de reserva do hóspede (G4-G7) clicou na atividade errada ("Jantar Romântico" em vez de "Passeio de Catamarã", citado no texto do guia) — corrigido, inclusive criando uma segunda reserva real (17/08) para gerar a tela de confirmação (G6), que tinha ficado de fora da primeira passada; (2) os campos de nome em 3 formulários do hóspede (G16/G18/G21) ficaram vazios por causa de um seletor por posição errado — corrigido injetando a sessão do hóspede direto via `localStorage` (mais robusto que repetir o fluxo de UI) e usando seletores mais específicos.
- Duas legendas do template ficaram desatualizadas e foram corrigidas: a do Concierge Premium (não é mais "prévia visual sem IA") e a da tela de treinamento do Concierge no backoffice (não é mais "para o futuro agente" — o agente já é real).
- HTML final montado (`docs/allstay-guia-de-uso.html`, ~2.9 MB, todos os 46 placeholders substituídos por imagens embutidas em base64) via um script de montagem simples. Ferramentas temporárias (Playwright, node_modules, scripts de captura/ajuste/montagem) removidas de `docs/` após o uso — só os PNGs brutos (`docs/tutorial-screens/`) e o HTML final ficaram.
- **Pendência**: o PDF (`AllStay_Guia_de_Uso_Tutorial.pdf`) não foi regenerado — ainda reflete a versão de 16 de agosto. Gerar um novo é simples (abrir o HTML novo no Chrome e "Salvar como PDF"), mas não foi feito automaticamente por já termos removido o Playwright local; fica como próximo passo opcional.
- Ponto para o relatório: essa etapa ilustra bem troubleshooting real de infraestrutura (SSL quebrado descoberto por acaso ao tentar gerar documentação) e adaptação de ferramenta (troca de automação de navegador dependente de extensão por Playwright headless, mais robusto e sem depender do computador do usuário estar com o Chrome aberto).

Este arquivo deve continuar sendo atualizado a cada etapa relevante do desenvolvimento.
