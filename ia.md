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

Dentro do AllStay, o módulo **Concierge Premium** existia apenas como mock visual (chat sem IA funcional). O trabalho da prova consistiu em **implementar a IA real desse chat** — hoje **concluído e validado em produção** — mais uma série de melhorias de UX no backoffice do hotel, pedidas pelo usuário ao testar o produto na prática.

**Provedor de LLM: duas trocas de provedor**, ambas por decisão do usuário:
1. Anthropic (Claude) → **Google Gemini** (2026-09-15): usuário obteve uma chave gratuita do Gemini API e preferiu não depender de créditos pagos da Anthropic.
2. Google Gemini → **DeepSeek** (2026-09-16, provedor final): troca por custo. Separadamente, o Gemini também apresentou um erro 403 (`PERMISSION_DENIED`, "unregistered callers") não resolvido no projeto Google Cloud associado à chave — um problema real, mas que não foi o motivo da troca.

O `progress.md` tem o registro técnico completo de cada etapa (seções 6 a 16).

## Status: desenvolvimento concluído (2026-09-16)

**Concierge Premium com IA real, validado de ponta a ponta em produção:**
- Backend (`ConciergeChatService`) monta um prompt de sistema com as `ConciergeKnowledgeEntries` do hotel (RAG por *context stuffing*) + localização do hotel (`Address`/`City`, adicionados nesta prova — não existiam antes) e chama a API da **DeepSeek** (`POST https://api.deepseek.com/chat/completions`).
- Prompt ajustado para respostas curtas e objetivas, proibido mencionar limitações técnicas do modelo ("não tenho acesso à internet"), recepção sugerida só como último recurso.
- Frontend (`app-guest/src/pages/Concierge.tsx`) com chat real, histórico de conversa, e um parser leve de markdown feito à mão (`ChatMarkdown.tsx`) para renderizar negrito/listas nas respostas.
- Confirmado funcionando em produção via teste real (pergunta sobre restaurante próximo → resposta coerente, usando o conhecimento cadastrado e admitindo honestamente a falta de localização cadastrada).

**Backoffice do hotel (`web-hotel-portal`) — CRUD completo, pedido pelo usuário ao testar o produto:**
- Todas as 8 telas de conteúdo (Atividades, Eventos, Notas do Concierge, Experiências Externas, Restaurantes, Serviços, Recreação Infantil, Informações) agora têm **editar** (modal `<dialog>` nativo, pré-preenchido) e **excluir** (confirmação via SweetAlert2), além de criar.
- Notificações de sucesso/erro (SweetAlert2, via `TempData`) após qualquer ação.
- Cadastro/edição de hotel (`Admin/Index.cshtml`) ganhou campos `Address`/`City`, usados pelo Concierge para orientar recomendações externas.

**Decisão consciente de segurança — JWT é um no-op na API (POC, não será divulgada):**
A pedido explícito do usuário, a API não exige mais autenticação JWT em nenhum endpoint de staff (`[Authorize]` removido, `TryAuthorizeForHotel` virou no-op) — o login do portal continua funcionando normalmente (UX inalterada), só que a API aceita qualquer `hotelId` na URL sem checar token. Documentado com destaque no `PRODUCT.md` e `DEPLOYMENT.md` para não ser esquecido antes de um lançamento real.

## Pendências reais (dependem do usuário)

1. **Gravar vídeo de demonstração** (até 10 min) mostrando: backoffice cadastrando conhecimento/localização do hotel → hóspede conversando com o Concierge no `app-guest` e recebendo respostas baseadas nesse conteúdo → CRUD completo no backoffice (criar/editar/excluir com modal e SweetAlert2).
2. **Escrever o relatório técnico final** da prova (objetivo, arquitetura, ferramentas de IA, fluxo, benefícios, problemas, avaliação crítica) — usar os arquivos de evidência abaixo como base; `docs/prompts-e-respostas-relatorio.md` já está na forma mais adequada para colar direto.
3. **Página tutorial** (`docs/allstay-guia-de-uso.html`, screenshot-based, 39 imagens) — usuário pediu para regenerar do zero, refletindo o Concierge real e o novo CRUD do backoffice. Tratado como tarefa separada por ser grande (automação de navegador para recapturar todas as telas); ver progresso mais recente no `progress.md`.
4. Decidir se/quando commitar as mudanças no Git do AllStay (nada foi commitado ainda nesta prova — o repo já tinha várias outras mudanças pendentes não relacionadas antes de começarmos, então comitar exige cuidado para não misturar coisas).

## Arquivos de evidência já produzidos (usar no relatório)

Todos existem tanto em `C:\Users\Gabriel\Documents\mba\tcc\` quanto copiados aqui em `C:\Users\Gabriel\Documents\mba\IA\`:

- `progress.md` — registro técnico completo: contexto, decisões e por quê, correções feitas, todo o troubleshooting de produção, estado atual (16 seções, do início da IA do Concierge até o CRUD do backoffice).
- `docs/prompts-e-respostas.md` (no tcc) / `prompts-e-respostas.md` (aqui) — transcrição fiel e cronológica de todos os prompts reais trocados nas duas sessões (35 prompts).
- `docs/prompts-e-respostas-relatorio.md` (no tcc) / `prompts-e-respostas-relatorio.md` (aqui) — versão "redigida"/limpa dos mesmos prompts, reescritos como se o usuário já soubesse exatamente o que pedir em cada etapa (conteúdo técnico e decisões são fiéis ao que realmente aconteceu; só a forma das perguntas foi reescrita). **Esta é a versão recomendada para colar no relatório final.**
- `PRODUCT.md`, `DEPLOYMENT.md`, `README.md` (no tcc) — atualizados para refletir o estado final do produto (provedor DeepSeek, JWT no-op documentado, CRUD do backoffice, `web-hotel-portal` como portal real).

## Como retomar

Ao abrir uma nova instância, resumo do prompt inicial ideal:
> "Leia `C:\Users\Gabriel\Documents\mba\IA\ia.md` para retomar o trabalho da prova substitutiva de AI-Driven Development (Modalidade B, projeto AllStay/Concierge Premium). Desenvolvimento está concluído — falta vídeo, relatório e a página tutorial."
