import { useState } from "react";
import { Navigate } from "react-router-dom";
import { api } from "../api/client";
import ChatMarkdown from "../components/ChatMarkdown";
import Icon from "../components/Icon";
import { useGuest } from "../context/GuestContext";
import type { ConciergeMessage } from "../types";

const SUGGESTIONS = ["Quais restaurantes estão abertos?", "O que posso fazer hoje?", "Horário do check-out?"];

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

  const [messages, setMessages] = useState<ConciergeMessage[]>([
    {
      id: newMessageId(),
      sender: "concierge",
      text: `Olá! Sou o HotelarIA, assistente digital do ${hotel?.name ?? "hotel"}. Como posso ajudar durante sua estadia?`,
      timestamp: now(),
    },
  ]);
  const [draft, setDraft] = useState("");
  const [sending, setSending] = useState(false);
  const [error, setError] = useState<string | null>(null);

  if (!hotel) return <Navigate to="/" replace />;

  const sendText = async (text: string) => {
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

  const handleSend = (e: React.FormEvent) => {
    e.preventDefault();
    sendText(draft.trim());
  };

  return (
    <div className="screen">
      <div className="concierge-header">
        <span className="concierge-avatar">
          <Icon name="headset" />
        </span>
        <div>
          <h1>HotelarIA</h1>
          <p className="subtitle">Assistente Virtual</p>
        </div>
      </div>

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

      {messages.length === 1 && !sending && (
        <div className="suggestion-chip-row">
          {SUGGESTIONS.map((s) => (
            <button key={s} className="suggestion-chip" onClick={() => sendText(s)}>
              {s}
            </button>
          ))}
        </div>
      )}

      {error && <p className="error">{error}</p>}

      <form className="chat-input-row" onSubmit={handleSend}>
        <input
          placeholder="Escreva sua mensagem"
          value={draft}
          onChange={(e) => setDraft(e.target.value)}
          disabled={sending}
        />
        <button type="submit" className="send-btn" disabled={sending || !draft.trim()} aria-label="Enviar">
          <Icon name="send" />
        </button>
      </form>
    </div>
  );
}
