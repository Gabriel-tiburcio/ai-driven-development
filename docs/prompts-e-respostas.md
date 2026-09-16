# Prompts e Respostas Utilizados — Evidência de Uso de IA

Documento de evidência para a Prova Substitutiva (Modalidade B — Projeto Prático), disciplina AI-Driven Development. Reproduz, em ordem cronológica, os prompts reais enviados à ferramenta de IA (Claude Code / Anthropic Claude) e um resumo fiel das respostas/ações tomadas, durante a implementação da IA real do módulo Concierge Premium do AllStay.

---

### Prompt 1
**Usuário:** "read the file trabalho.txt"

**Resposta da IA:** Leu `trabalho.txt` e resumiu o enunciado da prova: duas modalidades (A — artigo técnico; B — projeto prático), com critérios de avaliação (domínio conceitual 30%, capacidade crítica 20%, qualidade técnica 20%, profundidade de análise do uso de IA 20%, organização/documentação 10%).

---

### Prompt 2
**Usuário:** "What you think more easlly for you?"

**Resposta da IA:** Recomendou a Modalidade B, argumentando que um projeto prático gera evidências de uso de IA (prompts, decisões, correções) naturalmente durante o próprio desenvolvimento, enquanto a Modalidade A exigiria pesquisa bibliográfica com fontes acadêmicas verificáveis (risco de alucinação de referências).

---

### Prompt 3
**Usuário:** "mas a B precisa de video, você conseguiria gerar?"

**Resposta da IA:** Esclareceu que não tem capacidade de gravar vídeo — isso precisa ser feito pelo usuário (ex.: OBS, Loom, gravador de tela do Windows). Ofereceu preparar um roteiro de demonstração para facilitar a gravação.

---

### Prompt 4
**Usuário:** "Beleza, vamos pela modalidade B, uma dúvida o projeto é livre? pode ser qualquer coisa?"

**Resposta da IA:** Confirmou que o projeto é livre, desde que cumpra os requisitos do enunciado (aplicação funcional, uso de ao menos uma ferramenta de IA, evidências de uso, relatório técnico). Alertou que um projeto trivial demais prejudicaria os critérios de "qualidade técnica" e "profundidade da análise".

---

### Prompt 5
**Usuário:** "Pensei em um app delivery de lavanderias, funcionaria como um uber/ifood para lavanderia, o cliente solicita lavagem sacou?"

**Resposta da IA:** Deu retorno crítico: a ideia é boa mas tem escopo grande demais para o prazo da prova (apps completos tipo Uber/iFood exigem múltiplos perfis, geolocalização, pagamentos). Sugeriu um MVP recortado (web único, dois perfis, fluxo de status simplificado) e perguntou se o usuário queria incluir IA como recurso do próprio produto (não só como ferramenta de desenvolvimento) e qual stack preferia.

---

### Prompt 6
**Usuário (respostas às perguntas):** "Sim, incluir um recurso de IA no app" / "Algo que você já usa no trabalho/estudos"

**Resposta da IA:** Registrou as escolhas e pediu que o usuário informasse a stack já conhecida antes de detalhar o projeto.

---

### Prompt 7
**Usuário:** "tenho um app pronto, que é o meu TCC, está online, quer dar uma olhada? C:\Users\Gabriel\Documents\mba\tcc"

**Resposta da IA:** Explorou o repositório do projeto AllStay (`README.md`, `PRODUCT.md`): plataforma de concierge digital para hotéis (backend ASP.NET Core/.NET 8, PWA React para hóspede, portal admin em Razor Pages), já em produção como parte do TCC de MBA do usuário. Identificou que o módulo **Concierge Premium** é hoje só um mock visual (chat sem IA funcional), com o backoffice de "treinar o agente" já implementado (`ConciergeKnowledgeEntry`). Propôs duas opções: (1) aproveitar o AllStay implementando a IA real do Concierge; (2) seguir com a lavanderia do zero.

---

### Prompt 8
**Usuário (escolha):** "AllStay — implementar o Concierge com IA de verdade (Recomendado)"

**Resposta da IA:** Confirmou a escolha e anunciou que iria explorar a estrutura do Concierge (backend e frontend) antes de propor o plano de implementação.

---

### Prompt 9
**Usuário (respostas):** "Não tenho, preciso criar uma conta/chave" (API key Anthropic) / "Resposta completa (mais simples)" (sem streaming)

**Resposta da IA:** Registrou as decisões: chave de API pendente (usuário criaria depois em console.anthropic.com), endpoint de chat retornaria resposta completa, sem streaming, para simplificar a implementação.

---

### Prompt 10 (delegação interna — subagente de exploração)
**Instrução dada pela IA a um subagente (`Explore`):** investigar, no repositório AllStay, os padrões de código do backend (Clean Architecture: Domain/Application/Infrastructure/Api), o funcionamento de `HotelScopedControllerBase`, o padrão de autorização para endpoints guest-facing (`GuestRequestsController`), o padrão de configuração via `IOptions<T>` (`JwtOptions`), o `DependencyInjection.cs`, a estrutura de `ConciergeKnowledgeEntry`/`ConciergeKnowledgeController`, e no frontend (`app-guest`) o cliente HTTP (`api/client.ts`), o `GuestContext`, e a estrutura de `conciergeSeedMessages`/`ConciergeMessage` usados no mock do chat.

**Resposta do subagente:** Relatório detalhado com trechos de código de referência (`GuestRequestsController`, `JwtOptions`, `DependencyInjection.cs`, `AllStayDbContext`, `client.ts`, `GuestContext.tsx`, `mockContent.ts`), confirmando: guest nunca autentica (endpoints `[AllowAnonymous]`, hotelId vindo da rota); `.NET 8`; `System.Text.Json` como único stack de serialização; nenhuma integração HTTP externa prévia (seria a primeira `HttpClient` registrada); `appsettings.json` com seções planas por integração.

---

### Prompt 11
**Usuário:** implicitamente aprovado o plano ao aguardar a implementação (sem novo texto — a IA seguiu direto para o código com base nas decisões já tomadas)

**Resposta da IA:** Implementou, seguindo os padrões levantados:
- `backend/src/AllStay.Application/DTOs/ConciergeChatDtos.cs` (`ConciergeChatMessage`, `ConciergeChatRequest`, `ConciergeChatResponse`)
- `backend/src/AllStay.Application/Interfaces/IConciergeChatService.cs`
- `backend/src/AllStay.Infrastructure/Services/AnthropicOptions.cs`
- `backend/src/AllStay.Infrastructure/Services/ConciergeChatService.cs` — monta o prompt de sistema a partir de `ConciergeKnowledgeEntries` do hotel (RAG por *context stuffing*) e chama `POST https://api.anthropic.com/v1/messages`
- `backend/src/AllStay.Api/Controllers/ConciergeChatController.cs` — endpoint `POST /api/hotels/{hotelId}/concierge/chat`, `[AllowAnonymous]`
- Registro no DI: `services.Configure<AnthropicOptions>(...)`, `services.AddHttpClient<IConciergeChatService, ConciergeChatService>()`
- Seção `"Anthropic"` adicionada em `appsettings.json` e `appsettings.Production.json` (chave em branco, a preencher pelo usuário)

---

### Prompt 12
**Usuário (ao rodar o build):** erro de compilação surgiu (`AddHttpClient` não encontrado)

**Resposta da IA:** Diagnosticou que `AllStay.Infrastructure.csproj` não tinha o pacote `Microsoft.Extensions.Http` referenciado (só `Configuration.Abstractions` e `Options.ConfigurationExtensions`). Adicionou `<PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />` ao `.csproj`; a build passou a compilar com 0 erros/0 avisos. **Correção realizada após erro apontado pela IA**, registrada aqui como exigido pelo enunciado da prova (item "Correções realizadas após sugestões da IA").

---

### Prompt 13
**Usuário:** "por favor, enquanto trabalha por favor registre tudo em um progress.md para eu ter os 'prompets e respostas' que preciso no trabalho"

**Resposta da IA:** Criou `tcc/progress.md`, consolidando contexto, tabela-resumo de prompts, decisões técnicas (com justificativas), correções feitas e estado atual/próximos passos.

---

### Prompt 14
**Usuário:** "lembre de criar o progress.md aqui na pasta de IA ou pelo menos uma cópia"

**Resposta da IA:** Copiou `progress.md` também para `C:\Users\Gabriel\Documents\mba\IA\progress.md`.

---

### Prompt 15
**Usuário:** "registre também os prompets ou simule isso em um arquivo para eu enviar depois, se não me engano preciso disso para o trabalho"

**Resposta da IA:** Criou este arquivo (`docs/prompts-e-respostas.md`), com a transcrição fiel de todos os prompts reais trocados nesta sessão até aqui, mais uma cópia em `mba/IA/`.

---

### Prompt 16
**Contexto:** implementação do frontend do chat, na sequência da implementação do backend (prompt 11).

**Resposta da IA:** Explorou os padrões do frontend (`api/client.ts`, `GuestContext`, `ConciergeMessage`, padrão de loading/erro em `ActivityDetail.tsx`) e reescreveu `app-guest/src/pages/Concierge.tsx`: estado real de mensagens, input habilitado, chamada a `api.askConcierge` (novo método adicionado em `client.ts`) com histórico da conversa, indicador "Digitando...", tratamento de erro. Removeu `conciergeSeedMessages` (mock) de `data/mockContent.ts`, já não utilizado. Validou com `npx tsc --noEmit` (sem erros).

---

### Prompt 17 (notificação automática, não é entrada do usuário)
Uma tarefa em segundo plano (mesmo agente do prompt 13) concluiu de forma assíncrona, criando `progress.md` e depois este próprio arquivo (`docs/prompts-e-respostas.md`). Ao revisar o conteúdo gerado, a IA (nesta sessão principal) corrigiu uma imprecisão no relato do Prompt 12 (a causa real do erro de build era pacote ausente no `.csproj`, não cache de restore) e completou o histórico com o prompt 16, que não constava na versão gerada pelo agente em segundo plano.

---

## Observação sobre decisões do desenvolvedor (não geradas pela IA)

- Escolha da Modalidade B em vez de A: decisão do usuário, após ponderar o trade-off apresentado pela IA.
- Escolha de aproveitar o AllStay (TCC já existente) em vez de construir a lavanderia do zero: decisão do usuário.
- Escolha de não usar streaming na resposta do chat: decisão do usuário, priorizando simplicidade sobre efeito visual.
- Decisão de não commitar a chave de API real no repositório: alinhada por segurança (mesmo o projeto já tendo outros segredos em texto plano no `appsettings.json`, optou-se por não repetir esse padrão para uma chave nova).

Este documento deve ser atualizado conforme novos prompts forem trocados até a finalização do projeto (frontend do chat, testes end-to-end, gravação do vídeo).
