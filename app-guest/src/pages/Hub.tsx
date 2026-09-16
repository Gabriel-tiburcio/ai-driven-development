import { Link, Navigate } from "react-router-dom";
import { useGuest } from "../context/GuestContext";

const MODULES = [
  { to: "/restaurante", icon: "🍽️", label: "Restaurante", blurb: "Onde comer no hotel" },
  { to: "/concierge", icon: "💬", label: "Concierge Premium", blurb: "Tire dúvidas e receba dicas" },
  { to: "/informacoes", icon: "ℹ️", label: "Informações", blurb: "Wi-Fi, horários e regras" },
  { to: "/eventos", icon: "🎉", label: "Eventos", blurb: "Programação especial" },
  { to: "/atividades", icon: "🏓", label: "Atividades", blurb: "Grátis e inclusas na estadia" },
  { to: "/servicos", icon: "💆", label: "Serviços", blurb: "Experiências pagas no hotel" },
  { to: "/experiencias-externas", icon: "🧭", label: "Experiências Externas", blurb: "Passeios fora do hotel" },
  { to: "/solicitacoes", icon: "🛎️", label: "Solicitações", blurb: "Peça itens e atendimento" },
  { to: "/recreacao-infantil", icon: "🧸", label: "Recreação Infantil", blurb: "Programação para crianças" },
];

export default function Hub() {
  const { hotel } = useGuest();

  if (!hotel) return <Navigate to="/" replace />;

  return (
    <div className="screen">
      <header className="page-header">
        <h1>{hotel.name}</h1>
        <p className="subtitle">O que você precisa durante a sua estadia?</p>
      </header>

      <div className="module-grid">
        {MODULES.map((m) => (
          <Link to={m.to} key={m.to} className="module-card">
            <span className="module-card-icon">{m.icon}</span>
            <h3>{m.label}</h3>
            <p className="muted small">{m.blurb}</p>
          </Link>
        ))}
      </div>
    </div>
  );
}
