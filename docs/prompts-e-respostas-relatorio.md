# Prompts e Respostas — Evidência de Uso de IA (versão para o relatório)

Documento de evidência para a Prova Substitutiva (Modalidade B — Projeto Prático), disciplina AI-Driven Development. Reproduz, em ordem cronológica, os prompts utilizados com a ferramenta de IA (Claude Code / Anthropic Claude) e um resumo das respostas/ações tomadas, na implementação da IA real do módulo Concierge Premium do AllStay.

*Nota: os prompts abaixo foram reescritos de forma mais direta/objetiva em relação à conversa original, para clareza do relatório — o conteúdo técnico, as decisões e a ordem dos eventos são fiéis ao que de fato ocorreu (ver `progress.md` para o registro bruto).*

---

### Prompt 1 — Entendimento do enunciado
**Eu:** "Leia o arquivo `trabalho.txt` e me diga o que a prova substitutiva está pedindo."

**IA:** Leu o arquivo e resumiu as duas modalidades disponíveis — Modalidade A (artigo técnico de revisão de literatura) e Modalidade B (projeto prático) — junto com os requisitos de cada uma e os critérios de avaliação (domínio conceitual 30%, capacidade crítica 20%, qualidade técnica 20%, profundidade da análise sobre uso de IA 20%, organização/documentação 10%).

---

### Prompt 2 — Escolha da modalidade
**Eu:** "Considerando que vou usar você como ferramenta de apoio, qual modalidade faz mais sentido eu escolher?"

**IA:** Recomendou a Modalidade B, argumentando que um projeto prático gera evidências de uso de IA (prompts, decisões, correções) naturalmente durante o próprio desenvolvimento, enquanto a Modalidade A exigiria pesquisa bibliográfica com fontes acadêmicas verificáveis — mais lenta e com risco de citações incorretas se não checadas com cuidado.

---

### Prompt 3 — Escopo do entregável de vídeo
**Eu:** "A Modalidade B pede um vídeo de demonstração. Você consegue gerar esse vídeo pra mim?"

**IA:** Esclareceu que gravação de vídeo não é uma capacidade da ferramenta — isso precisaria ser feito por mim (ex.: OBS, Loom, gravador de tela do Windows). Se ofereceu para preparar um roteiro de demonstração para facilitar a gravação.

---

### Prompt 4 — Confirmação da modalidade e liberdade de tema
**Eu:** "Vamos de Modalidade B. O tema do projeto é livre, ou preciso escolher entre as sugestões do enunciado?"

**IA:** Confirmou que o tema é livre — as sugestões do enunciado (chatbot, agente com MCP, etc.) são exemplos, não obrigatórios — desde que o projeto cumpra os requisitos formais: aplicação funcional, uso de pelo menos uma ferramenta de IA, evidências de uso registradas, e relatório técnico. Alertou que um projeto trivial demais prejudicaria os critérios de "qualidade técnica" e "profundidade da análise".

---

### Prompt 5 — Proposta inicial de projeto
**Eu:** "Pensei em desenvolver um app de delivery de lavanderias, no estilo Uber/iFood: o cliente solicita a coleta da roupa suja e a lavanderia parceira aceita o pedido. O que acha?"

**IA:** Validou a ideia, mas alertou sobre o escopo: um app completo estilo Uber/iFood normalmente envolve múltiplos perfis de usuário, geolocalização em tempo real e integração de pagamentos — inviável no prazo de uma prova. Sugeriu recortar um MVP (aplicação web única, dois perfis, fluxo de status simplificado, sem pagamento real) e perguntou se eu queria incluir IA como recurso do próprio produto (não só como ferramenta de desenvolvimento), e qual stack eu preferia usar.

---

### Prompt 6 — Direcionamento técnico
**Eu:** "Quero incluir um recurso de IA de verdade dentro do app, não só usar IA para programar. E prefiro usar uma stack que eu já domino, em vez de começar algo do zero."

**IA:** Registrou as duas decisões e pediu que eu indicasse qual stack/projeto eu já tinha disponível, para aproveitar em vez de começar do zero.

---

### Prompt 7 — Reaproveitamento do TCC
**Eu:** "Na verdade eu já tenho um app pronto e em produção, que é o meu TCC de MBA — o AllStay, uma plataforma de concierge digital para hotéis. Dá uma olhada no repositório em `C:\Users\Gabriel\Documents\mba\tcc` e me diz se dá pra aproveitar pra essa prova."

**IA:** Explorou o repositório (`README.md`, `PRODUCT.md`) e identificou a arquitetura completa: backend ASP.NET Core (.NET 8, Clean Architecture), PWA React para o hóspede (`app-guest`), portal administrativo em Razor Pages (`web-hotel-portal`), site institucional. Identificou que, dos 9 módulos do produto, o **Concierge Premium** era o único incompleto: o chat do hóspede era um mock visual estático, sem IA funcional, embora o backoffice para cadastrar "conhecimento" do hotel (`ConciergeKnowledgeEntry`) já existisse pronto. Propôs duas alternativas: (1) aproveitar o AllStay implementando a IA real do Concierge, encaixando perfeitamente no requisito da prova; ou (2) seguir com a ideia da lavanderia, do zero.

---

### Prompt 8 — Decisão final de escopo
**Eu:** "Vamos aproveitar o AllStay — faz mais sentido terminar um recurso que já estava planejado do que começar outro projeto do zero. Bora implementar a IA real do Concierge."

**IA:** Confirmou a escolha e anunciou que antes de codar iria mapear a estrutura atual do módulo Concierge (entidade de conhecimento, controller de backoffice, tela mockada do hóspede) para desenhar um plano de implementação compatível com o que já existe.

---

### Prompt 9 — Definições de implementação
**Eu:** "Ainda não tenho uma chave de API da Anthropic configurada, vou precisar criar uma conta. E para a resposta do chat, prefiro começar simples: resposta completa de uma vez, sem streaming — dá pra evoluir depois."

**IA:** Registrou as duas decisões: (1) a integração ficaria pronta no código, com a chave de API a ser preenchida por mim após criar a conta em console.anthropic.com; (2) o endpoint retornaria a resposta completa do modelo, sem streaming (SSE), priorizando simplicidade de implementação e de teste dentro do prazo da prova.

---

### Prompt 10 — Levantamento de padrões do código
**Eu:** "Antes de implementar, mapeia os padrões que o projeto já usa — arquitetura em camadas, como os controllers tratam autenticação/autorização para hóspede vs. staff, como configurações e chaves externas são lidas, e como o frontend do hóspede chama a API — pra você seguir exatamente o mesmo estilo."

**IA:** Delegou a um subagente de exploração, que levantou: o padrão de Clean Architecture (Domain/Application/Infrastructure/Api); o funcionamento de `HotelScopedControllerBase` e do padrão `[AllowAnonymous]` para endpoints guest-facing (hóspede nunca autentica — identidade vem da rota, não de JWT), usado como referência o `GuestRequestsController`; o padrão de configuração via `IOptions<T>` (`JwtOptions`); a ausência de qualquer integração HTTP externa prévia no projeto (seria a primeira); e no frontend, o cliente HTTP (`api/client.ts`), o `GuestContext` e a estrutura de dados do mock do chat (`ConciergeMessage`, `conciergeSeedMessages`).

---

### Prompt 11 — Implementação do backend
**Eu:** "Beleza, implementa o endpoint de chat do Concierge seguindo esses padrões: busca o conhecimento cadastrado do hotel, monta o contexto pro modelo, chama a API da Anthropic e devolve a resposta."

**IA:** Implementou, ponta a ponta:
- `ConciergeChatDtos.cs` (request/response do chat)
- `IConciergeChatService.cs`
- `AnthropicOptions.cs` (chave, modelo, max tokens — padrão `IOptions<T>`)
- `ConciergeChatService.cs` — busca as `ConciergeKnowledgeEntries` do hotel e monta um prompt de sistema com todo o conteúdo cadastrado (abordagem de RAG por *context stuffing*, justificada pelo volume pequeno de conteúdo por hotel), chama `POST https://api.anthropic.com/v1/messages`
- `ConciergeChatController.cs` — endpoint `POST /api/hotels/{hotelId}/concierge/chat`, `[AllowAnonymous]`, seguindo o mesmo padrão de autorização guest usado em outros módulos
- Registro no DI (`AddHttpClient`, `Configure<AnthropicOptions>`) e nova seção `Anthropic` em `appsettings.json`/`appsettings.Production.json`, com a chave deixada em branco propositalmente

---

### Prompt 12 — Correção de erro de build
**Eu:** "Rodei o build e deu erro: `IServiceCollection não contém uma definição para AddHttpClient`. Investiga e corrige."

**IA:** Diagnosticou que `AllStay.Infrastructure.csproj` não tinha o pacote `Microsoft.Extensions.Http` referenciado. Adicionou a referência ao pacote; o build voltou a compilar com 0 erros/0 avisos.

---

### Prompt 13 — Implementação do frontend
**Eu:** "Agora conecta o front — troca o mock do `Concierge.tsx` pelo chat de verdade, chamando esse endpoint, e trata os estados de carregando/erro do mesmo jeito que as outras telas do app já fazem."

**IA:** Adicionou `askConcierge` em `api/client.ts`; reescreveu `Concierge.tsx` com estado real de mensagens, input habilitado, chamada ao endpoint com o histórico da conversa, indicador de "Digitando..." e tratamento de erro no mesmo padrão de `ActivityDetail.tsx`. Removeu o mock `conciergeSeedMessages`, que ficou sem uso. Validou com `npx tsc --noEmit`, sem erros.

---

### Prompt 14 — Organização das evidências
**Eu:** "Registra tudo isso — prompts, decisões e correções — em um arquivo de progresso, e também guarda uma cópia na pasta onde estou fazendo o trabalho da disciplina."

**IA:** Criou `progress.md` (contexto, tabela de prompts, decisões técnicas com justificativas, correções, estado atual e próximos passos) e uma cópia deste documento de prompts/respostas, ambos também replicados na pasta da disciplina (`mba/IA/`).

---

## Decisões que foram minhas, não da IA

- Escolha da Modalidade B em vez da A.
- Escolha de aproveitar o AllStay (TCC já existente) em vez de construir um projeto novo do zero.
- Escolha de não usar streaming na resposta do chat, priorizando simplicidade.
- Decisão de não commitar a chave de API real no repositório (mesmo havendo outros segredos em texto plano no projeto, optei por não repetir esse padrão para uma chave nova).

### Prompt 15 — Troca de provedor de IA (Anthropic → Gemini)
**Eu:** "Consegui uma chave gratuita do Google Gemini. Pode trocar o provedor de IA do Concierge da Anthropic para o Gemini?"

**IA:** Confirmou o objetivo antes de agir (avisando sobre o risco de ter compartilhado a chave em texto no chat) e reescreveu a integração: `AnthropicOptions` → `GeminiOptions`, `ConciergeChatService` adaptado para o formato de requisição/resposta do endpoint `generateContent` do Gemini. Sugeriu inicialmente guardar a chave via `dotnet user-secrets` (mais seguro que texto plano); segui o padrão já usado no resto do projeto e pedi para manter em `appsettings.json` mesmo, o que a IA aplicou.

---

### Prompt 16 — Recomendações dinâmicas de restaurantes e passeios
**Eu:** "O Concierge também deveria conseguir recomendar restaurantes e passeios nas proximidades do hotel, não só responder com o que está cadastrado. Dá pra fazer isso de forma dinâmica?"

**IA:** Apresentou duas opções: usar o *grounding* com busca do Google, nativo da API do Gemini (mais rápido de implementar), ou integrar a Google Places API manualmente (mais controle sobre os dados, mais trabalho). Optei pela primeira. A IA habilitou a ferramenta de busca na chamada do Gemini e ajustou o prompt para priorizar o conteúdo cadastrado pelo hotel e nunca inventar lugares.

---

### Prompt 17 — Localização do hotel de ponta a ponta
**Eu:** "Quero incluir o endereço/cidade do hotel nesse fluxo. Revise como o hotel é cadastrado hoje para essa informação chegar desde o cadastro até o momento em que o hóspede usa o Concierge."

**IA:** Investigou o fluxo de cadastro de hotel antes de implementar e descobriu que **não existia nenhuma forma de editar um hotel já cadastrado** (só criar e ativar/desativar). Implementou a funcionalidade completa: campos de endereço/cidade na entidade, migration de banco, endpoint de atualização, tela de edição no painel admin (cobrindo hotéis já existentes), e a informação sendo injetada no prompt do Concierge para orientar as recomendações externas.

---

### Prompt 18 — Teste em produção e troubleshooting
**Eu:** "Acabei de publicar em produção, poderia testar?"

**IA:** Testou via terminal e navegador, e diagnosticou em sequência três problemas reais do ambiente de produção, não do código em si: uma migration de banco que nunca tinha sido aplicada (migrations só rodam automaticamente em ambiente de desenvolvimento), uma transação de banco deixada aberta por engano numa ferramenta externa (bloqueando consultas), e um erro de permissão (403) da API do Gemini no projeto Google Cloud associado à chave. Para o primeiro problema, como o Claude Code bloqueia ações diretas em banco de produção via terminal, a IA gerou um script SQL seguro (idempotente) para eu rodar manualmente.

---

### Prompt 19 — Segunda troca de provedor (Gemini → DeepSeek)
**Eu:** "Vamos trocar o Gemini pela DeepSeek, aqui está a chave."

**IA:** Perguntou como eu queria lidar com a perda da busca ao vivo do Gemini (a DeepSeek não tem essa ferramenta nativa) — optei por manter as recomendações baseadas apenas no conhecimento geral do modelo, com aviso para nunca inventar lugares. A IA reescreveu a integração para o formato de API da DeepSeek (compatível com o padrão OpenAI) e me ajudou a diagnosticar um erro de autenticação (401) causado pela chave não estar chegando corretamente à aplicação em produção.

---

### Prompt 20 — Ajuste fino do comportamento do Concierge
**Eu:** "As respostas do Concierge estão muito longas, ele fica avisando que não tem acesso à internet e manda o hóspede falar com a recepção o tempo todo. Também preciso que o app renderize a formatação em negrito/listas que o modelo está mandando."

**IA:** Reescreveu o prompt de sistema para respostas curtas e objetivas, proibiu menções a limitações técnicas do modelo, e limitou a sugestão de recepção a casos sem alternativa real. Criou um pequeno renderizador de markdown no app do hóspede (sem adicionar nenhuma biblioteca nova) para exibir negrito e listas corretamente em vez de asteriscos literais.

---

### Prompt 21 — Botões de excluir e editar no backoffice
**Eu:** "Testando o painel percebi que nada tem opção de excluir os itens cadastrados — dá pra incluir isso pra tudo? E também a edição, não só exclusão."

**IA:** Verificou primeiro que o backend já tinha endpoints prontos para excluir e editar quase todas as entidades (só faltava a interface) — exceto as notas do Concierge, que não tinham endpoint de edição, implementado nessa mesma etapa. Adicionou botões de excluir e editar nas 8 telas do painel administrativo.

---

### Prompt 22 — Refinamento de UX (modal + SweetAlert2)
**Eu:** "Gostei, mas os botões ficaram muito soltos visualmente. A edição não poderia abrir em um modal? E usa o SweetAlert para as confirmações e mensagens de sucesso/erro, em vez do alerta padrão do navegador."

**IA:** Converteu os formulários de edição para modais nativos do navegador, trocou a confirmação de exclusão pelo SweetAlert2, e adicionou notificações de sucesso/erro após qualquer ação no painel (antes essas ações não avisavam se tinham realmente funcionado).

---

## Decisões que foram minhas, não da IA

- Escolha da Modalidade B em vez da A.
- Escolha de aproveitar o AllStay (TCC já existente) em vez de construir um projeto novo do zero.
- Escolha de não usar streaming na resposta do chat, priorizando simplicidade.
- Decisão de manter a chave de API em texto plano no `appsettings.json`, seguindo o mesmo padrão já usado no restante do projeto para outros segredos — inclusive depois de a IA sugerir uma alternativa mais segura (`dotnet user-secrets`).
- Duas trocas sucessivas de provedor de IA (Anthropic → Gemini → DeepSeek), ambas motivadas por custo — não pelo erro de permissão do Gemini encontrado na mesma época, que foi um problema à parte.
- Escolha de manter as recomendações externas do Concierge limitadas ao conhecimento geral do modelo após trocar para a DeepSeek, em vez de integrar uma API de busca separada.
- Pedido para remover a autenticação JWT da API (por ser uma POC não divulgada) e, pouco depois, pedido para restaurá-la ao perceber que o problema original era do ambiente de teste local, não da autenticação.
- Todos os ajustes de UX do backoffice (botões de excluir/editar, modal, SweetAlert2) nasceram de uso real do produto, não do escopo original planejado — evidência de um ciclo iterativo típico de desenvolvimento assistido por IA.

## Estado final (2026-09-16)

- Concierge Premium com IA real (DeepSeek), RAG por context stuffing com o conteúdo cadastrado pelo hotel, validado de ponta a ponta em produção.
- Backoffice do hotel com CRUD completo (criar/editar/excluir) em todas as 8 telas de conteúdo, com modais e feedback visual (SweetAlert2).
- Pendências remanescentes: gravar o vídeo de demonstração e finalizar a redação do relatório técnico da prova (este documento e `progress.md` servem de base).
