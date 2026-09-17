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

---

### Prompt 18
**Usuário:** "any LLM free that I can use?"

**Resposta da IA:** Sugeriu alternativas gratuitas/mais baratas à Anthropic (Google Gemini, Groq, OpenRouter, Ollama local), recomendando não trocar de provedor sem necessidade real, já que a integração com Anthropic já estava pronta.

---

### Prompt 19
**Usuário:** colou uma chave de API do Gemini (Google AI Studio) diretamente no chat.

**Resposta da IA:** Avisou sobre o risco de ter colado uma chave real em texto no chat (sugeriu revogar/regerar caso não quisesse que ficasse registrada) e perguntou a intenção de uso antes de agir. Usuário confirmou: trocar a Anthropic pela Gemini. A IA reescreveu `ConciergeChatService` para o formato de API do Gemini (`generateContent`), renomeou `AnthropicOptions` → `GeminiOptions`, e tentou inicialmente guardar a chave via `dotnet user-secrets` (mais seguro) — o usuário pediu para manter em texto plano no `appsettings.json`, seguindo o padrão já existente no projeto para outros segredos; a IA reverteu para esse formato a pedido explícito.

---

### Prompt 20
**Usuário (pergunta exploratória):** "existe uma forma de configurar um rag dinâmico? para o concierge dar dicas de locais para almoçar ou jantar e até mesmo passeios?"

**Resposta da IA:** Recomendou usar o *grounding* nativo do Gemini (ferramenta de busca do Google embutida na API), citando como alternativa mais trabalhosa integrar a Google Places API separadamente. Usuário escolheu a opção mais rápida (grounding).

---

### Prompt 21
**Usuário:** "opção mais rápida"

**Resposta da IA:** Adicionou `tools: [{ "google_search": {} }]` à chamada do Gemini e ajustou o prompt de sistema para permitir recomendações externas via busca, mantendo a regra de nunca inventar lugares.

---

### Prompt 22
**Usuário:** "inclua o address/city no fluxo, lembre que precisa rever o cadastro do hotel para transmitir essa informação da origem até o momento que o hóspede aciona o concierge"

**Resposta da IA:** Antes de codar, rodou um subagente de exploração para mapear o fluxo real de cadastro/edição de hotel (achou que **não existia nenhum endpoint de edição de hotel**, só criação e ativar/desativar). Implementou de ponta a ponta: campos `Address`/`City` na entidade `Hotel`, migration EF Core, DTOs, novo endpoint `PATCH /api/admin/hotels/{id}/location`, UI de edição inline no `web-hotel-portal/Pages/Admin/Index.cshtml` (cobrindo hotéis já cadastrados), e injeção da localização no prompt do `ConciergeChatService`.

---

### Prompt 23
**Usuário:** "acabei de publicar em produção, poderia testar? se precisar novamente dos links ou de alguma outra informação me avise"

**Resposta da IA:** Testou via `curl` e automação de navegador (Claude in Chrome). Descobriu, em sequência, três problemas reais de produção: (1) `Invalid column name 'Address'/'City'` — migration nunca aplicada em produção porque migrations só rodam automaticamente em ambiente Development; (2) depois de corrigido, `Execution Timeout Expired` na mesma query — não era lock (uma consulta direta do usuário funcionava), e sim um commit pendente numa sessão externa ao banco; (3) erro 403 do Gemini (`PERMISSION_DENIED`, "unregistered callers") mesmo com a chave configurada.

---

### Prompt 24
**Usuário:** "pode aplicar" (a migration)

**Resposta da IA:** Tentou rodar `dotnet ef database update` direto contra o banco de produção — **bloqueado pelo classificador de segurança do Claude Code** (ação de produção via terminal). Gerou em vez disso um script SQL idempotente (`dotnet ef migrations script --idempotent`, que não toca no banco) para o usuário rodar manualmente.

---

### Prompt 25
**Usuário:** "script executado" / depois "realmente havia um block de um commit não efetuado, tente agora"

**Resposta da IA:** Retestou o endpoint (voltou a responder em <1s) e depois o chat do Concierge, chegando ao erro 403 do Gemini — um problema separado, não relacionado à decisão de troca de provedor tomada no Prompt 26 (que foi por custo).

---

### Prompt 26
**Usuário:** "vamos trocar o gemini pelo deepseek, segue chave: sk-71be..."

**Resposta da IA:** Perguntou como o usuário queria lidar com a perda do grounding do Gemini (DeepSeek não tem busca nativa) — usuário escolheu manter as recomendações limitadas ao conhecimento geral do modelo, com aviso no prompt para nunca inventar lugares e sempre recomendar confirmação. Reescreveu `ConciergeChatService` para o formato OpenAI-compatible da DeepSeek (`POST https://api.deepseek.com/chat/completions`).

---

### Prompt 27
**Usuário:** "to tomando 401 ainda" / "tentei outro methodo e estou tomando 401" / "testei outro endpoint e ainda estou tomando 401"

**Resposta da IA:** Diagnosticou repetidamente que o erro 401 vinha da própria DeepSeek (`Authentication Fails`), não da API do AllStay — apontou como causa provável a chave não estar configurada corretamente em produção (nome errado de variável de ambiente, `DeepSeek__ApiKey` com dois underscores é o formato exigido pelo ASP.NET Core).

---

### Prompt 28
**Usuário:** "pode reabilitar o jwt, o problema era apenas localhost"

**Resposta da IA:** Havia removido a autenticação JWT da API (pedido anterior do usuário, ver Prompt 29 abaixo, fora de ordem cronológica no rascunho original) e a restaurou integralmente: usou `git checkout` para os arquivos que já estavam commitados com JWT (fonte de verdade mais confiável que "lembrar" o código), e reconstruiu manualmente os 8 controllers que tinham a proteção como mudança local não commitada, conferindo que as anotações `[Authorize]` voltaram exatamente nas mesmas linhas de antes.

---

### Prompt 29 (fora de ordem — o pedido de remoção do JWT antecede o Prompt 28)
**Usuário:** "remova a autenticação jwt dos projetos" → confirmado com "sim, é apenas uma POC que não será divulgada"

**Resposta da IA:** Antes de agir, rodou um subagente de exploração para mapear toda dependência de JWT (backend + portal), para não quebrar o login do staff sem querer. Optou por uma remoção de escopo mínimo: desativou a validação JWT na API (`Program.cs`, `TryAuthorizeForHotel` virou no-op, `[Authorize]` removido de 10 controllers) mas manteve o login do portal funcionando sem mudanças, evitando reescrever as ~17 páginas que dependiam do claim do JWT para saber o hotelId.

---

### Prompt 30
**Usuário:** relatou uma resposta do Concierge muito longa, mencionando "não tenho acesso à internet" e sugerindo a recepção com frequência excessiva; pediu para "preparar a página para aceitar essa formatação, e limitar as respostas para serem objetivas e não avisar que o deepseek não tem acesso a internet e evitar ao máximo indicar que o hóspede fale com a recepção"

**Resposta da IA:** Reescreveu o prompt de sistema (respostas de 2-4 frases, proibido mencionar limitações técnicas, recepção só como último recurso) e reduziu `MaxTokens` de 1024 para 400. Criou `ChatMarkdown.tsx`, um parser leve de markdown feito à mão (negrito, parágrafos, listas), para o app do hóspede parar de mostrar `**asteriscos**` literais.

---

### Prompt 31
**Usuário:** "quais os próximos passos?"

**Resposta da IA:** Listou pendências (publicar as mudanças, testar, cadastrar endereço de um hotel real, commitar, gravar vídeo, escrever relatório).

---

### Prompt 32
**Usuário:** "estou testando, a única coisa que senti falta foi um botão para excluir ou editar as coisas cadastradas em [...], nada tem opção de exclusão, poderia incluir para tudo? digo para eventos, as notas do concierge, experiência, restaurantes..."

**Resposta da IA:** Rodou exploração para confirmar que o backend já tinha endpoints `DELETE` prontos para todas as 8 entidades do backoffice (faltava só a UI). Adicionou botão "Excluir" com confirmação em Atividades, Eventos, Notas do Concierge, Experiências Externas, Restaurantes, Serviços, Recreação Infantil e Informações.

---

### Prompt 33
**Usuário:** "você preparou o botão de edição também?" → "sim"

**Resposta da IA:** Adicionou edição para as mesmas 8 entidades. Só as Notas do Concierge não tinham endpoint de atualização no backend — implementado do zero (`UpdateConciergeKnowledgeEntryRequest`, `UpdateAsync`, `PUT /concierge-knowledge/{id}`). Nas outras 7, o backend já suportava `PUT`.

---

### Prompt 34
**Usuário:** "gostei, mas os botões ficaram muito soltos, a edição não poderia ser por um modal? e utilize o sweetalert para confirmar as mensagem de confirmação, sucesso e erro"

**Resposta da IA:** Converteu os formulários de edição de `<details>` inline para `<dialog>` nativo (modal). Adicionou SweetAlert2 via CDN: confirmação de exclusão substitui o `confirm()` nativo, e toast de sucesso/erro aparece após qualquer criação/edição/exclusão (via `TempData` lido pelo `_Layout.cshtml`).

---

### Prompt 35
**Usuário:** "Perfeito, concluímos o desenvolvimento, por favor atualize todos os .md e depois a documentação inclusive a página tutorial"

**Resposta da IA:** Atualizou `progress.md`, `ia.md`, `README.md`, `PRODUCT.md`, `DEPLOYMENT.md` e os dois arquivos de prompts/respostas (este e sua versão para relatório) para refletir o estado final do projeto. A regeneração da página tutorial (`docs/allstay-guia-de-uso.html`, com 39 screenshots) foi tratada à parte, dado o tamanho da tarefa.

---

## Observação sobre decisões do desenvolvedor (não geradas pela IA)

- Escolha da Modalidade B em vez de A: decisão do usuário, após ponderar o trade-off apresentado pela IA.
- Escolha de aproveitar o AllStay (TCC já existente) em vez de construir a lavanderia do zero: decisão do usuário.
- Escolha de não usar streaming na resposta do chat: decisão do usuário, priorizando simplicidade sobre efeito visual.
- Decisão de não commitar a chave de API real no repositório: alinhada por segurança (mesmo o projeto já tendo outros segredos em texto plano no `appsettings.json`, optou-se por não repetir esse padrão para uma chave nova) — **revisitada depois**: o usuário pediu explicitamente para manter a chave em texto plano no `appsettings.json`, seguindo o padrão já existente no restante do projeto, tanto para o Gemini quanto para a DeepSeek.
- Escolha de trocar Anthropic → Gemini → DeepSeek: decisões do usuário, motivadas por custo/disponibilidade de free tier nas duas trocas. O erro de permissão (403) do Gemini (não resolvido no projeto Google Cloud associado à chave) foi um problema real encontrado na mesma época, mas não foi o motivo da segunda troca.
- Escolha de manter as recomendações externas do Concierge limitadas ao conhecimento geral do modelo (sem busca ao vivo) após a troca para DeepSeek: decisão do usuário, ciente do risco de desatualização.
- Remoção (temporária) e depois restauração da autenticação JWT da API: decisões do usuário — primeiro para simplificar testes de uma POC que não seria divulgada, depois revertida ao descobrir que o problema original era local (ambiente de teste), não da autenticação em si.
- Pedido de botões de excluir/editar, modal e SweetAlert2 no backoffice: melhorias de UX apontadas pelo usuário ao testar o produto na prática, não solicitadas no escopo original da prova.

Este documento reflete o estado final do projeto para fins de entrega da prova substitutiva (2026-09-16).
