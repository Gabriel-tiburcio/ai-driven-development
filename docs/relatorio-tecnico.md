# Relatório Técnico — Prova Substitutiva AI-Driven Development

**Modalidade B — Projeto Prático**
**Autor:** Gabriel
**Projeto:** AllStay — Concierge Premium com IA real + evolução do backoffice
**Ferramenta de IA principal:** Claude Code (Anthropic), modelo Claude Sonnet 5, uso agentic via terminal com acesso de leitura/escrita ao repositório, subagentes de exploração e automação de navegador (Claude in Chrome / Playwright)
**Repositório:** `C:\Users\Gabriel\Documents\mba\tcc` (Git)
**Evidências completas de prompts:** `docs/prompts-e-respostas.md` (transcrição fiel) e `docs/prompts-e-respostas-relatorio.md` (versão redigida, recomendada para leitura corrida); registro técnico completo em `progress.md`.

---

## 1. Objetivo do sistema

O AllStay é uma plataforma de concierge digital para hotéis que desenvolvi como Trabalho de Conclusão de Curso do MBA (FIAP), já em produção. O hóspede acessa via QR code/código do hotel, sem instalação de app, e navega por nove módulos de conteúdo e serviços da estadia (restaurante, informações, eventos, atividades, serviços pagos, experiências externas, solicitações, recreação infantil e concierge); a equipe do hotel gerencia todo esse conteúdo por um painel administrativo; hotéis em potencial conhecem o produto por um site institucional.

Dos nove módulos, o **Concierge Premium** era o único que eu tinha deixado incompleto no TCC original: existia apenas como um mock visual — um chat estático, sem inteligência artificial funcional —, embora a área de backoffice para o hotel cadastrar "conhecimento" (`ConciergeKnowledgeEntry`) já estivesse pronta.

Ao escolher o tema desta prova, optei deliberadamente por **não construir um projeto descartável do zero**, e sim terminar uma funcionalidade real de um produto que já está em produção — isso me pareceu um teste mais honesto da minha capacidade de usar IA em um contexto de engenharia real, com legado, convenções já estabelecidas e usuários reais, em vez de um cenário artificial "greenfield" onde qualquer decisão de arquitetura é trivial. Os objetivos específicos que defini para a prova foram:

1. Implementar um chat funcional em que o hóspede conversa com um LLM que responde com base no conteúdo específico do hotel (RAG), sem alucinar informações que o hotel não forneceu.
2. Manter essa implementação coerente com a arquitetura e as convenções que eu já havia estabelecido no projeto (Clean Architecture no backend, padrões de autenticação/autorização, estilo de código do frontend).
3. Usar o próprio produto em produção como ambiente de teste e deixar que o uso real guiasse melhorias adicionais no backoffice — em vez de travar o escopo no que foi planejado no primeiro dia.

## 2. Arquitetura utilizada

O AllStay é composto por quatro aplicações e uma API central. Escolhi essa stack — **.NET/ASP.NET Core no backend e nos dois portais Razor Pages, React no app do hóspede** — porque é a stack em que tenho mais anos de experiência profissional; isso foi uma decisão deliberada de reduzir o risco de arquitetura em um projeto que já ia envolver risco suficiente na parte de IA, e me permitiu revisar criticamente cada sugestão da IA com autoridade técnica, em vez de aceitar às cegas por não conhecer a tecnologia a fundo.

| Camada | Tecnologia | Papel |
|---|---|---|
| `backend` | ASP.NET Core .NET 8, Clean Architecture (Domain / Application / Infrastructure / Api), Entity Framework Core, SQL Server | API central: regras de negócio, persistência, integração com o LLM |
| `app-guest` | React 19 + TypeScript + Vite + React Router, PWA | Aplicativo do hóspede — inclui a tela do Concierge |
| `web-hotel-portal` | ASP.NET Core Razor Pages | Backoffice da equipe do hotel — inclui o cadastro de conhecimento do Concierge e o CRUD completo dos demais módulos |
| `web-institutional` | ASP.NET Core Razor Pages | Site B2B de captação de hotéis |

A funcionalidade implementada nesta prova segue o padrão de Clean Architecture que eu já havia estabelecido no backend:

- **Domain**: entidade `ConciergeKnowledgeEntry` (já existente) e o campo `Address`/`City`, que adicionei à entidade `Hotel` para dar contexto geográfico ao concierge.
- **Application**: DTOs (`ConciergeChatRequest`/`ConciergeChatResponse`) e a interface `IConciergeChatService`.
- **Infrastructure**: `ConciergeChatService`, responsável por (1) buscar as entradas de conhecimento do hotel no banco, (2) montar um *prompt* de sistema a partir delas — optei por uma forma simples de RAG por **context stuffing**, sem vetorização, por avaliar que o volume de conteúdo por hotel não justificava a complexidade operacional de um vector store — e (3) chamar a API do provedor de LLM configurado via `HttpClient`.
- **Api**: `ConciergeChatController`, expondo `POST /api/hotels/{hotelId}/concierge/chat`, endpoint público (`[AllowAnonymous]`), seguindo o mesmo padrão que eu já usava para os demais endpoints do hóspede (que nunca autentica — sua identidade vem do código do hotel na rota, não de um token).

No frontend, reescrevi `app-guest/src/pages/Concierge.tsx` para manter estado real de mensagens, exibir indicador de digitação, tratar erros no mesmo padrão das outras telas do app, e renderizar a formatação Markdown simples (negrito, listas) que o modelo retorna — via um pequeno componente (`ChatMarkdown.tsx`) que optei por escrever à mão em vez de adicionar mais uma dependência ao bundle, já que o app do hóspede é uma PWA e eu priorizo manter o payload inicial pequeno.

Um ponto de arquitetura que decidi conscientemente foi **desativar temporariamente a imposição de autenticação JWT** nos endpoints do backoffice durante a fase final de testes da POC — para acelerar a validação ponta a ponta sem depender de renovação de token durante sessões longas de teste manual. Restaurei a proteção assim que confirmei que ela não era, de fato, a causa do problema que eu estava investigando (seção 6).

## 3. Ferramentas de IA empregadas

### 3.1 Ferramenta de desenvolvimento

Usei o **Claude Code** (Anthropic), executando o modelo **Claude Sonnet 5**, como agente principal de desenvolvimento durante toda a prova, com acesso de leitura/escrita ao repositório, execução de comandos de terminal (build, testes, `curl`, `dotnet ef`), e, em pontos específicos, subagentes especializados:

- **Subagentes de exploração**, que direcionei para mapear convenções existentes no código *antes* de qualquer implementação nova — por exemplo, antes de implementar o campo de localização do hotel, pedi uma investigação que confirmou que eu ainda não tinha construído nenhum fluxo de edição de hotel no admin, só criação — o que me permitiu desenhar a funcionalidade completa de uma vez, em vez de descobrir a lacuna no meio da implementação.
- **Automação de navegador** (Claude in Chrome, e depois Playwright headless) para eu testar o produto de ponta a ponta em produção e para regenerar a documentação de uso com screenshots reais.
- **Múltiplos agentes em paralelo**, que usei tanto para acelerar tarefas independentes (captura de screenshots em três frentes simultâneas) quanto para aprender, na prática, o limite dessa técnica quando os agentes disputam o mesmo recurso compartilhado (discutido na seção 6).

### 3.2 Ferramentas de IA no próprio produto (o recurso construído)

Decidi migrar o provedor de LLM do Concierge Premium duas vezes ao longo da prova, sempre por uma análise de custo-benefício de operação em produção, não por limitação técnica de nenhum dos modelos:

1. **Anthropic (Claude)** — escolha inicial, mesma família de modelo que eu já estava usando como ferramenta de desenvolvimento, o que simplificou a integração inicial.
2. **Google Gemini** — troquei ao identificar uma oferta de free tier vantajosa para o estágio inicial do produto. Nessa fase, também aproveitei o recurso de ***grounding* com busca do Google**, nativo da API do Gemini, para permitir recomendações externas (restaurantes, passeios) com informação atualizada — uma capacidade que avaliei como diferencial real para a experiência do hóspede.
3. **DeepSeek** (provedor final, em produção) — nova troca por custo, já pensando em escala: para o volume de mensagens que projeto para múltiplos hotéis, a DeepSeek tem um custo por token bem mais competitivo. Como esse provedor não oferece busca ao vivo, optei por manter as recomendações externas limitadas ao conhecimento geral do próprio modelo, com salvaguardas explícitas no prompt contra invenção de estabelecimentos inexistentes — uma troca consciente de uma funcionalidade "nice-to-have" (busca em tempo real) por previsibilidade de custo em escala, que considero a decisão correta para este estágio do produto.

Essa sequência de trocas foi, na prática, um exercício direto de **prompt/context engineering**: precisei reescrever o prompt de sistema do Concierge a cada mudança de capacidade do modelo (com ou sem busca), e novamente após avaliar criticamente as primeiras respostas em uso real (identifiquei que estavam longas demais, mencionavam limitações técnicas do modelo de forma pouco profissional para o produto, e sugeriam a recepção com frequência excessiva) — processo detalhado na seção 5.

## 4. Fluxo de desenvolvimento

O desenvolvimento seguiu um ciclo iterativo de trabalho assistido por IA, em duas sessões principais (2026-09-09 e 2026-09-15/16), documentado prompt a prompt em `docs/prompts-e-respostas.md`. Em linhas gerais:

1. **Definição de escopo.** Descartei uma ideia inicial (um app de delivery de lavanderias) por avaliar, com apoio da análise crítica da IA, que o escopo era grande demais para o prazo — e decidi reaproveitar o AllStay, identificando qual módulo estava incompleto e por quê.
2. **Exploração antes da implementação.** Antes de cada mudança não trivial, usei um subagente para mapear os padrões já existentes no código, garantindo que a implementação nova ficasse indistinguível, em estilo, do restante do projeto que eu já havia escrito.
3. **Implementação incremental com validação de build.** Cada mudança foi seguida de `dotnet build`/`npx tsc --noEmit`, capturando erros reais (por exemplo, um pacote NuGet que faltava referenciar) e corrigindo antes de prosseguir.
4. **Teste em produção e resolução de problemas de infraestrutura.** Como o projeto não tem ambiente de homologação — uma decisão de custo que tomei desde o TCC original, dado que é um produto ainda pré-comercial —, todo teste de ponta a ponta ocorreu direto em produção, o que me permitiu (com apoio da IA no diagnóstico) encontrar e corrigir três problemas operacionais reais, nenhum deles relacionado ao código da feature em si.
5. **Iteração guiada por uso real.** Depois que o Concierge funcionou, usei o próprio produto como um usuário real usaria, e isso revelou necessidades que eu não tinha previsto no escopo original (calibragem das respostas do modelo, ausência de edição/exclusão no backoffice) — tratei essas descobertas como parte legítima do mesmo ciclo de desenvolvimento, não como "escopo extra".
6. **Consolidação e documentação.** Ao final, atualizei toda a documentação do projeto (`README.md`, `PRODUCT.md`, `DEPLOYMENT.md`) para refletir o estado real do sistema, e regenerei a documentação de uso do produto (guia com screenshots) inteiramente.

## 5. Benefícios observados

- **Velocidade em tarefas mecânicas e repetitivas, sem abrir mão de padrão de qualidade.** A adição de exclusão e edição a oito telas praticamente idênticas do backoffice — endpoint, handler, formulário — foi possível em uma única sessão porque, depois de eu revisar e aprovar o padrão na primeira tela, consegui pedir a replicação consistente nas outras sete com confiança.
- **Redução de retrabalho por suposições erradas.** Insistir em explorar o código antes de implementar evitou, pelo menos duas vezes de forma documentada, que uma funcionalidade fosse construída sobre uma premissa falsa sobre o que já existia no projeto.
- **Diagnóstico eficiente de problemas de produção fora do escopo original da feature.** Identifiquei, com apoio da IA, três causas-raiz distintas de erros em produção (coluna de banco inexistente por migration pendente, transação de banco presa, erro de configuração de permissão em um provedor externo) — nenhuma delas um bug no código que eu escrevi nesta prova, mas sim lacunas operacionais que só um teste real em produção revelaria.
- **Adaptação de ferramenta perante uma limitação real.** Quando a automação de navegador dependente da minha própria sessão do Chrome se mostrou frágil para rodar em paralelo, troquei a abordagem para Playwright headless, independente do meu navegador — prefiro trocar de ferramenta a insistir em uma abordagem que já demonstrou ser instável.
- **Documentação viva como ativo, não como tarefa burocrática.** Mantive `progress.md` atualizado a cada etapa relevante, o que me permitiu reconstruir este relatório com precisão, sem depender da minha memória sobre decisões tomadas dias antes.

## 6. Problemas encontrados

Esta seção documenta os problemas reais que enfrentei — inclusive os que não foram causados pela IA, mas que ela ajudou a diagnosticar, e os pontos em que precisei corrigir o rumo.

1. **Erro de build por dependência faltante.** `AddHttpClient` não compilava porque eu não tinha referenciado `Microsoft.Extensions.Http` no `AllStay.Infrastructure.csproj`. Diagnosticado e corrigido no mesmo ciclo, sem impacto no cronograma.
2. **Migration de banco não aplicada em produção.** Como o projeto não roda migrations automaticamente fora do ambiente de desenvolvimento — escolha que fiz para evitar alterações de schema acidentais em produção —, precisei aplicar a migration manualmente. Pedi para a IA automatizar isso via terminal e ela foi corretamente bloqueada pelo próprio classificador de segurança do Claude Code, o que na prática validou uma salvaguarda que considero correta: mudanças de schema em produção devem passar por revisão humana. Resolvi gerando um script SQL idempotente e rodando eu mesmo.
3. **Transação de banco presa, um "falso positivo" de bug de código.** Depois da migration aplicada, a mesma consulta simples passou a expirar por timeout — o tipo de sintoma que, sem investigação cuidadosa, eu poderia ter atribuído erroneamente a um problema de performance na aplicação. O diagnóstico correto (uma transação que eu mesmo tinha deixado aberta em outra ferramenta de acesso ao banco) só apareceu comparando o comportamento da API com uma consulta direta minha no mesmo banco.
4. **Erro de configuração de permissão no Google Cloud.** O Gemini passou a retornar 403 (`PERMISSION_DENIED`) no projeto associado à minha chave — um ajuste de configuração do lado do provedor que não priorizei investigar a fundo porque já estava avaliando trocar de provedor por custo de qualquer forma.
5. **Ajuste de segurança temporário no backoffice, revertido com rapidez.** Durante uma rodada intensa de testes manuais, simplifiquei a validação de autenticação da API para acelerar o ciclo de testes, e horas depois, ao perceber que essa simplificação não era o que estava resolvendo o problema que eu investigava, revertei imediatamente com apoio da IA — que usou o histórico do Git como fonte de verdade para restaurar exatamente o estado anterior, com verificação linha a linha. Foi um ajuste de escopo bem contido, corrigido no mesmo dia, mas que me lembrou a importância de isolar variáveis com mais disciplina antes de mudar controles de segurança, mesmo em ambiente de teste.
6. **Contenção entre agentes em paralelo.** Três subagentes de automação de navegador, rodando simultaneamente, disputaram a mesma sessão do meu Chrome — o que me ensinou, na prática, que paralelizar automação de UI dependente de um recurso compartilhado e com estado (uma única sessão de navegador) não escala da mesma forma que paralelizar tarefas de leitura/análise independentes.
7. **Respostas do LLM mal calibradas para o produto, na primeira versão do prompt.** Sem ajuste fino, o Concierge produzia respostas mais longas do que eu gostaria e mencionava detalhes técnicos do próprio modelo que não fazem sentido para um hóspede de hotel. Corrigi isso com instruções explícitas e negativas no prompt de sistema (o que **não** dizer) — um refinamento normal de qualquer produto conversacional, que eu já esperava precisar fazer depois de ver respostas reais.

## 7. Avaliação crítica da experiência

**Sobre os limites da IA neste projeto.** A IA se comportou de forma responsável em todos os pontos de decisão sensível: quando pedi uma simplificação temporária de segurança, ela pediu confirmação explícita antes de agir e documentou a decisão com destaque, para que não fosse esquecida. Quando pedi para aplicar uma migration direto em produção via terminal, ela recusou corretamente e propôs uma alternativa mais segura. Isso reforça algo que já esperava desta disciplina: a IA é uma executora extremamente competente, mas a responsabilidade de julgamento sobre trade-offs de produção continua sendo minha — e prefiro que seja assim.

**Sobre RAG e Context Engineering.** Escolhi deliberadamente um RAG simplíssimo (*context stuffing*, sem vetorização) para o Concierge. Com poucas dezenas de entradas de conhecimento por hotel, uma arquitetura de recuperação vetorial (embeddings, busca por similaridade) adicionaria complexidade e latência sem ganho de qualidade perceptível neste estágio do produto — mas já tenho mapeado o ponto de migração para RAG vetorial caso o volume de conteúdo por hotel cresça (por exemplo, hotéis com dezenas de páginas de manual interno). Essa decisão reforça uma lição prática da disciplina: a técnica mais sofisticada nem sempre é a certa — a engenharia de contexto adequada depende do volume e da natureza real dos dados, e eu prefiro adicionar complexidade só quando o dado justifica.

**Sobre dependência de provedores externos.** As duas trocas de provedor de LLM (Anthropic → Gemini → DeepSeek), ambas por análise de custo, confirmam algo que eu já suspeitava antes de começar: mesmo isolando bem a integração atrás de uma interface (`IConciergeChatService`), trocar de provedor de LLM ainda exige reescrever a lógica de chamada HTTP e o formato de mensagens, porque não existe um padrão universal entre APIs (o Gemini usa `generateContent` com `system_instruction`; a DeepSeek usa um formato compatível com OpenAI). Para este estágio do produto, decidi que não valia a pena investir numa camada de abstração mais genérica antes da segunda troca real acontecer — otimização prematura teria sido um desperdício de tempo se eu tivesse ficado só no Gemini.

**Sobre avaliação crítica das sugestões da IA.** Não aceitei todas as sugestões sem questionar. Por exemplo, mantive o padrão de configuração já usado no projeto (segredos em `appsettings.json`) mesmo quando a IA sugeriu uma alternativa mais moderna, porque avaliei que a consistência com o resto da base de código, num produto que ainda estou evoluindo sozinho, valia mais do que a "melhor prática" isolada naquele ponto específico.

**Conclusão.** O maior aprendizado desta prova não foi sobre qualidade de código gerado pela IA — que na minha experiência já é alta e continua sendo a parte mais fácil do processo —, mas sobre governança das decisões ao redor dela: quando confirmar antes de agir, quando reverter rápido em vez de insistir, e quando a "melhor prática" genérica perde para o contexto específico do produto. É exatamente esse tipo de julgamento que a observação final do enunciado da prova destaca como o critério mais importante: não a mera utilização da IA, mas a capacidade de avaliar criticamente suas respostas e usá-la de forma responsável no processo de desenvolvimento de software.

---

## Entregáveis desta prova

- **Código-fonte:** repositório Git em `C:\Users\Gabriel\Documents\mba\tcc`.
- **Relatório técnico:** este documento, com prompts detalhados em `docs/prompts-e-respostas-relatorio.md` (versão recomendada) e `docs/prompts-e-respostas.md` (transcrição fiel).
- **Vídeo de demonstração:** cobrindo o cadastro de conhecimento/localização no backoffice → conversa real com o Concierge no app do hóspede → CRUD completo (criar, editar, excluir) das oito telas de conteúdo do backoffice.
- **Página tutorial (material de apoio):** `docs/allstay-guia-de-uso.html`, com 46 capturas de tela reais de produção, cobrindo os quatro aplicativos do sistema.
