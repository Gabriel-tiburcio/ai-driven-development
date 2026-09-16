using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using AllStay.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AllStay.Infrastructure.Services;

/// <summary>
/// Answers guest questions for the Concierge Premium chat. Retrieval is intentionally simple:
/// with only a handful of short knowledge entries per hotel, stuffing all of them into the
/// system prompt is a valid, low-latency form of RAG at this scale — no vector store needed.
/// </summary>
public class ConciergeChatService(AllStayDbContext db, HttpClient httpClient, IOptions<DeepSeekOptions> options) : IConciergeChatService
{
    private readonly DeepSeekOptions _options = options.Value;

    public async Task<ConciergeChatResponse> AskAsync(Guid hotelId, ConciergeChatRequest request, CancellationToken ct = default)
    {
        var hotel = await db.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId, ct)
            ?? throw new InvalidOperationException($"Hotel {hotelId} not found");

        var knowledge = await db.ConciergeKnowledgeEntries
            .Where(e => e.HotelId == hotelId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(ct);

        var systemPrompt = BuildSystemPrompt(hotel.Name, hotel.Address, hotel.City, knowledge);

        var messages = new List<object> { new { role = "system", content = systemPrompt } };
        foreach (var turn in request.History ?? [])
        {
            messages.Add(new { role = turn.Role == "guest" ? "user" : "assistant", content = turn.Text });
        }
        messages.Add(new { role = "user", content = request.Message });

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.deepseek.com/chat/completions")
        {
            Content = JsonContent.Create(new
            {
                model = _options.Model,
                max_tokens = _options.MaxTokens,
                messages
            })
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        using var httpResponse = await httpClient.SendAsync(httpRequest, ct);
        var body = await httpResponse.Content.ReadAsStringAsync(ct);

        if (!httpResponse.IsSuccessStatusCode)
            throw new InvalidOperationException($"DeepSeek API error ({(int)httpResponse.StatusCode}): {body}");

        var reply = ExtractReplyText(body);
        return new ConciergeChatResponse(reply);
    }

    private static string BuildSystemPrompt(string hotelName, string? address, string? city, IReadOnlyList<Domain.Entities.ConciergeKnowledgeEntry> knowledge)
    {
        var knowledgeBlock = knowledge.Count == 0
            ? "Nenhuma informação específica foi cadastrada pelo hotel ainda."
            : string.Join("\n\n", knowledge.Select(e => $"## {e.Title}\n{e.Content}"));

        var locationLine = (address, city) switch
        {
            (null or "", null or "") => "Localização não cadastrada pelo hotel.",
            (var a, null or "") => a!,
            (null or "", var c) => c!,
            var (a, c) => $"{a}, {c}"
        };

        return $"""
            Você é o Concierge Premium do hotel "{hotelName}", parte do app AllStay.
            Localização do hotel: {locationLine}

            Estilo de resposta: português (pt-BR), cordial mas direto ao ponto. Seja BREVE — a maioria
            das respostas deve caber em 2-4 frases curtas. Vá direto à recomendação ou informação pedida,
            sem introduções longas, sem repetir ressalvas a cada mensagem, sem parágrafos separados para
            "dentro do hotel" e "fora do hotel" a menos que o hóspede peça as duas coisas. Use **negrito**
            só para nomes de lugares ou informações-chave, não para frases inteiras.

            Sobre o hotel (políticas, horários, comodidades, serviços): use APENAS as informações abaixo,
            fornecidas pelo hotel. Se não tiver a informação, diga isso em uma frase curta — não invente
            dados sobre o hotel.

            Sobre recomendações externas (restaurantes, bares, passeios, pontos turísticos nas
            proximidades): use seu conhecimento geral sobre a região para sugerir lugares conhecidos e
            estabelecidos perto da localização do hotel acima, com naturalidade, como uma recomendação
            normal. Priorize qualquer indicação já cadastrada nas informações abaixo. Nunca invente nomes
            de lugares que você não tenha certeza que existam de verdade — se não tiver uma sugestão
            confiável, diga isso em uma frase, sem rodeios.

            Não mencione que você não tem acesso à internet ou que é uma IA. Só sugira que o hóspede fale
            com a recepção como último recurso, quando genuinamente não houver nada mais a oferecer — não
            use isso como resposta padrão nem repita em toda mensagem.

            Informações do hotel:
            {knowledgeBlock}
            """;
    }

    private static string ExtractReplyText(string responseBody)
    {
        var json = JsonNode.Parse(responseBody);
        var text = json?["choices"]?[0]?["message"]?["content"]?.GetValue<string>();

        return string.IsNullOrEmpty(text)
            ? throw new InvalidOperationException($"Unexpected DeepSeek API response shape: {responseBody}")
            : text;
    }
}
