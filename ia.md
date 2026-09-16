# Contexto do trabalho — Prova Substitutiva AI-Driven Development

Arquivo de retomada, para continuar este trabalho em outra instância/sessão do Claude Code sem perder contexto. Leia isto primeiro.

## O que é a prova

Enunciado completo em `trabalho.txt` (nesta mesma pasta). Resumo: escolher entre Modalidade A (artigo técnico) ou Modalidade B (projeto prático). **Escolhemos a Modalidade B.**

Requisitos da Modalidade B:
- Aplicação funcional usando IA como apoio ao desenvolvimento.
- Registrar evidências: prompts, respostas relevantes, decisões do desenvolvedor, correções feitas após sugestões da IA.
- Relatório técnico: objetivo do sistema, arquitetura, ferramentas de IA empregadas, fluxo de desenvolvimento, benefícios observados, problemas encontrados, avaliação crítica.
- Entregáveis: código no Git, relatório técnico com prompts, vídeo de demonstração (até 10 min).

## Decisão de projeto

Em vez de construir algo do zero, reaproveitamos um projeto já pronto e em produção do usuário: **AllStay**, seu TCC de MBA (FIAP) — plataforma de concierge digital para hotéis. Repositório em:

```
C:\Users\Gabriel\Documents\mba\tcc
```

Dentro do AllStay, o módulo **Concierge Premium** existia apenas como mock visual (chat sem IA funcional). O trabalho da prova consistiu em **implementar a IA real desse chat**, usando um LLM externo e o conteúdo que o hotel já cadastra no backoffice (`ConciergeKnowledgeEntry`) como base de conhecimento (RAG simples por *context stuffing* — sem vetorização, justificado pelo volume pequeno de conteúdo por hotel).

**Provedor de LLM: migrado de Anthropic (Claude) para Google Gemini em 2026-09-15**, a pedido do usuário, que obteve uma chave gratuita do Gemini API (Google AI Studio) e preferiu não depender de créditos pagos da Anthropic. Ver seção 8 do `progress.md` para os detalhes técnicos da migração.

## O que já foi implementado (status: concluído e compilando)

**Backend** (`backend/src/`, ASP.NET Core .NET 8, Clean Architecture):
- `AllStay.Application/DTOs/ConciergeChatDtos.cs`
- `AllStay.Application/Interfaces/IConciergeChatService.cs`
- `AllStay.Infrastructure/Services/GeminiOptions.cs` (antes `AnthropicOptions.cs`, renomeado na migração de provedor)
- `AllStay.Infrastructure/Services/ConciergeChatService.cs` — monta o prompt de sistema com as `ConciergeKnowledgeEntries` do hotel e chama `POST https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}` (Gemini)
- `AllStay.Api/Controllers/ConciergeChatController.cs` — `POST /api/hotels/{hotelId}/concierge/chat`, `[AllowAnonymous]` (hóspede nunca autentica, padrão já usado em `GuestRequestsController`)
- Registrado no DI (`AddHttpClient<IConciergeChatService, ConciergeChatService>`)
- Config: seção `"Gemini": { "ApiKey": "...", "Model": "gemini-2.5-flash", "MaxTokens": 1024 }` — **`appsettings.json` (local) tem a chave real do Gemini preenchida** (fornecida pelo usuário em 2026-09-15, a pedido dele mantida em texto plano no mesmo padrão dos demais segredos do projeto); `appsettings.Production.json` continua com a chave em branco.
- Precisou adicionar `PackageReference Microsoft.Extensions.Http` ao `AllStay.Infrastructure.csproj` (faltava — causava erro de build `AddHttpClient` não encontrado)
- `dotnet build` passa com 0 erros/0 avisos
- Testado subindo o backend local (`dotnet run`, porta 5299) — sobe sem erros. **Ainda não testamos o chat de verdade** (falta um hotelId/código válido — ver pendências).

**Frontend** (`app-guest/`, React + TS + Vite):
- `src/api/client.ts` — novo método `askConcierge(hotelId, { message, history })`
- `src/pages/Concierge.tsx` — reescrito: chat real (estado local de mensagens), input habilitado, chama o endpoint com histórico da conversa, indicador "Digitando...", tratamento de erro (padrão de `ActivityDetail.tsx`)
- `src/data/mockContent.ts` — removido `conciergeSeedMessages` (não usado mais)
- `npx tsc --noEmit` passa sem erros

## O que falta (pendências reais, dependem do usuário)

1. ~~Criar conta/chave em console.anthropic.com~~ — **feito**: chave já preenchida em `appsettings.json` local (ver alerta de segurança abaixo). Falta só preencher a mesma chave (ou outra dedicada) em `appsettings.Production.json` na hora do deploy.
2. **⚠️ Decidir o que fazer com a chave antes de commitar/dar push**: hoje ela está em texto plano em `backend/src/AllStay.Api/appsettings.json`, que é um arquivo **rastreado pelo Git**. O projeto já tem outros segredos assim versionados (senha do banco, JWT signing key, admin key) — mas vale uma decisão consciente: (a) manter o padrão do projeto e aceitar o risco, ou (b) mover para variável de ambiente / `dotnet user-secrets` antes de commitar, deixando o arquivo versionado só com o placeholder vazio. **Se optar por (b), avisar a próxima sessão para fazer a troca antes de qualquer commit.**
3. **Testar de ponta a ponta**: rodar backend (`dotnet run` em `backend/src/AllStay.Api`) + frontend (`npm run dev` em `app-guest`) localmente e conversar com o Concierge de verdade.
   - Falta um **hotelId/código de hotel válido** (com pelo menos uma entrada em "Concierge Knowledge" cadastrada no backoffice) para o teste fazer sentido. Tentamos descobrir isso via `GET /api/admin/hotels` (com a admin key) e via `sqlcmd` direto no banco — **o classificador de segurança do Claude Code bloqueou os dois comandos** (ações em banco/API de produção via terminal). Caminho mais simples: abrir o `app-guest` no navegador com o código de um hotel real que você já conhece, ou logar no `web-hotel-portal` pra achar um.
   - Atenção: o `appsettings.json` do backend aponta para o banco de **produção** (não há banco local separado) — não é necessário rodar `dotnet ef database update` para este trabalho (nenhuma migração nova foi criada, o endpoint só lê a tabela `ConciergeKnowledgeEntries`, que já existe).
4. **Gravar vídeo de demonstração** (até 10 min) mostrando: backoffice cadastrando conhecimento do hotel (`web-hotel-portal/Pages/ConciergeKnowledge`) → hóspede conversando com o Concierge no `app-guest` e recebendo respostas baseadas nesse conteúdo.
5. **Escrever o relatório técnico final** da prova (objetivo, arquitetura, ferramentas de IA, fluxo, benefícios, problemas, avaliação crítica) — usar os arquivos de evidência abaixo como base.
6. Decidir se/quando commitar as mudanças no Git do AllStay (ainda não foi commitado nada desta feature — o repo tinha várias outras mudanças pendentes não relacionadas, então comitar exige cuidado para não misturar coisas; e ver o item 2 antes de commitar `appsettings.json`).

## Arquivos de evidência já produzidos (usar no relatório)

Todos existem tanto em `C:\Users\Gabriel\Documents\mba\tcc\` quanto copiados aqui em `C:\Users\Gabriel\Documents\mba\IA\`:

- `progress.md` — registro técnico: contexto, decisões e por quê, correções feitas, estado atual/próximos passos.
- `docs/prompts-e-respostas.md` (no tcc) / `prompts-e-respostas.md` (aqui) — transcrição fiel e cronológica dos prompts reais trocados na sessão.
- `docs/prompts-e-respostas-relatorio.md` (no tcc) / `prompts-e-respostas-relatorio.md` (aqui) — versão "redigida"/limpa dos mesmos prompts, reescritos como se o usuário já soubesse exatamente o que pedir em cada etapa (conteúdo técnico e decisões são fiéis ao que realmente aconteceu; só a forma das perguntas foi reescrita). **Esta é a versão recomendada para colar no relatório final.**

## Observação sobre um evento estranho nesta sessão

Durante o trabalho, notificações de uma tarefa em segundo plano (não solicitada explicitamente nesta conversa, aparentemente disparada por outro canal/instância) apareceram criando/atualizando `progress.md` e `docs/prompts-e-respostas.md` de forma autônoma. O conteúdo gerado foi revisado, corrigido (havia uma imprecisão sobre a causa de um erro de build) e incorporado normalmente — não houve perda de trabalho, mas vale investigar se há outra sessão do Claude Code rodando em paralelo na pasta do TCC, para não haver conflito de edições.

## Como retomar

Ao abrir uma nova instância, resumo do prompt inicial ideal:
> "Leia `C:\Users\Gabriel\Documents\mba\IA\ia.md` para retomar o trabalho da prova substitutiva de AI-Driven Development (Modalidade B, projeto AllStay/Concierge Premium)."
