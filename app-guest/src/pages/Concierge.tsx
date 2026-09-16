import { useState } from "react";
import { Navigate, useNavigate } from "react-router-dom";
import { api } from "../api/client";
import ChatMarkdown from "../components/ChatMarkdown";
import { useGuest } from "../context/GuestContext";
import type { ConciergeMessage } from "../types";

let nextMessageId = 0;
function newMessageId() {
  nextMessageId += 1;
  return `m${nextMessageId}`;
}

function now() {
  return new Date().toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" });
}

export default function Concierge() {
  const { hotel } = useGuest();
  const navigate = useNavigate();

  const [messages, setMessages] = useState<ConciergeMessage[]>([
    {
      id: newMessageId(),
      sender: "concierge",
      text: `Olá! Sou o Concierge Premium do ${hotel?.name ?? "hotel"}. Como posso ajudar na sua estadia?`,
      timestamp: now(),
    },
  ]);
  const [draft, setDraft] = useState("");
  const [sending, setSending] = useState(false);
  const [error, setError] = useState<string | null>(null);

  if (!hotel) return <Navigate to="/" replace />;

  const handleSend = async (e: React.FormEvent) => {
    e.preventDefault();
    const text = draft.trim();
    if (!text || sending) return;

    const guestMessage: ConciergeMessage = { id: newMessageId(), sender: "guest", text, timestamp: now() };
    const history = messages.map((m) => ({ role: m.sender, text: m.text }));

    setMessages((prev) => [...prev, guestMessage]);
    setDraft("");
    setSending(true);
    setError(null);

    try {
      const { reply } = await api.askConcierge(hotel.id, { message: text, history });
      setMessages((prev) => [...prev, { id: newMessageId(), sender: "concierge", text: reply, timestamp: now() }]);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Não foi possível falar com o concierge agora.");
    } finally {
      setSending(false);
    }
  };

  return (
    <div className="screen">
      <button className="link-back" onClick={() => navigate(-1)}>
        &larr; Voltar
      </button>

      <header className="page-header">
        <h1>Concierge Premium</h1>
        <p className="subtitle">Converse e receba recomendações sobre {hotel.name}.</p>
      </header>

      <div className="chat-thread">
        {messages.map((m) => (
          <div key={m.id} className={`chat-bubble ${m.sender}`}>
            <ChatMarkdown text={m.text} />
            <span className="chat-bubble-time">{m.timestamp}</span>
          </div>
        ))}
        {sending && (
          <div className="chat-bubble concierge">
            <p>Digitando...</p>
          </div>
        )}
      </div>

      {error && <p className="error">{error}</p>}

      <form className="chat-input-row" onSubmit={handleSend}>
        <input
          placeholder="Escreva sua mensagem..."
          value={draft}
          onChange={(e) => setDraft(e.target.value)}
          disabled={sending}
        />
        <button type="submit" disabled={sending || !draft.trim()}>
          Enviar
        </button>
      </form>
    </div>
  );
}
