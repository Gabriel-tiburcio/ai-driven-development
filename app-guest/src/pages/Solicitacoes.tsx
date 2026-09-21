import { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";
import { api } from "../api/client";
import Icon, { type IconName } from "../components/Icon";
import { useGuest } from "../context/GuestContext";
import type { GuestRequest } from "../types";

const REQUEST_TYPES: { type: string; icon: IconName }[] = [
  { type: "Toalhas", icon: "towel" },
  { type: "Travesseiros", icon: "pillow" },
  { type: "Amenities", icon: "bottle" },
  { type: "Limpeza", icon: "spray" },
  { type: "Manutenção", icon: "wrench" },
  { type: "Outro pedido", icon: "clipboard-list" },
];

function statusStep(status: GuestRequest["status"]) {
  if (status === "Done" || status === 2) return 2;
  if (status === "InProgress" || status === 1) return 1;
  return 0;
}

const STEPS = ["Solicitado", "Preparando", "Concluído"];

function Stepper({ step }: { step: number }) {
  return (
    <div className="stepper">
      {STEPS.map((label, i) => {
        const done = i < step;
        const current = i === step;
        return (
          <div key={label} className={`stepper-step ${done ? "done" : ""} ${current ? "current" : ""}`}>
            <span className="stepper-line" />
            <span className="stepper-dot">{done ? <Icon name="check" /> : i + 1}</span>
            <span className="stepper-label">{label}</span>
          </div>
        );
      })}
    </div>
  );
}

export default function Solicitacoes() {
  const { hotel, roomNumber, guestName, setGuestInfo } = useGuest();

  const [tab, setTab] = useState<"Minhas" | "Novas">("Minhas");
  const [requests, setRequests] = useState<GuestRequest[]>([]);
  const [type, setType] = useState<string | null>(null);
  const [details, setDetails] = useState("");
  const [room, setRoom] = useState(roomNumber);
  const [name, setName] = useState(guestName);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (hotel && roomNumber) {
      api.getGuestRequests(hotel.id, roomNumber).then(setRequests);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [hotel]);

  if (!hotel) return <Navigate to="/" replace />;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!type) return;
    setSubmitting(true);
    setGuestInfo(room, name);

    await api.createGuestRequest(hotel.id, { type, details, guestName: name, roomNumber: room });
    const updated = await api.getGuestRequests(hotel.id, room);
    setRequests(updated);
    setDetails("");
    setType(null);
    setSubmitting(false);
    setTab("Minhas");
  };

  const open = requests.filter((r) => statusStep(r.status) < 2);
  const done = requests.filter((r) => statusStep(r.status) === 2);

  return (
    <div className="screen">
      <header className="page-header">
        <h1>Solicitações</h1>
      </header>

      <div className="tab-switch">
        <button className={tab === "Minhas" ? "active" : ""} onClick={() => setTab("Minhas")}>
          Minhas
        </button>
        <button className={tab === "Novas" ? "active" : ""} onClick={() => setTab("Novas")}>
          Novas
        </button>
      </div>

      {tab === "Minhas" && (
        <>
          {requests.length === 0 && <p className="muted">Você ainda não fez nenhuma solicitação.</p>}

          {open.length > 0 && (
            <>
              <h2 className="section-title">Em aberto</h2>
              <div className="card-list">
                {open.map((r) => (
                  <div className="activity-card" key={r.id} style={{ display: "block" }}>
                    <div className="activity-card-top">
                      <h3 style={{ margin: 0 }}>
                        {r.type} <span className="muted small">#{r.id.slice(0, 4)}</span>
                      </h3>
                      <span className="badge">{STEPS[statusStep(r.status)]}</span>
                    </div>
                    <Stepper step={statusStep(r.status)} />
                  </div>
                ))}
              </div>
            </>
          )}

          {done.length > 0 && (
            <>
              <h2 className="section-title">Concluídos</h2>
              <div className="card-list">
                {done.map((r) => (
                  <div className="activity-card" key={r.id}>
                    <div className="activity-card-body">
                      <div className="activity-card-top">
                        <h3 style={{ margin: 0 }}>
                          {r.type} <span className="muted small">#{r.id.slice(0, 4)}</span>
                        </h3>
                        <span className="badge" style={{ background: "rgba(28,122,52,.12)", color: "var(--ok-fg)" }}>
                          <Icon name="check" style={{ width: 11, height: 11, verticalAlign: "-1px" }} /> Concluído
                        </span>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </>
          )}
        </>
      )}

      {tab === "Novas" && (
        <>
          <div className="request-grid">
            {REQUEST_TYPES.map((rt) => (
              <button
                key={rt.type}
                className={`request-tile ${type === rt.type ? "active" : ""}`}
                onClick={() => setType(type === rt.type ? null : rt.type)}
              >
                <span className="request-tile-icon">
                  <Icon name={rt.icon} />
                </span>
                {rt.type}
              </button>
            ))}
          </div>

          {type && (
            <form onSubmit={handleSubmit} className="reserve-form" style={{ marginTop: "1.25rem" }}>
              <div className="field">
                <label>Detalhes</label>
                <input value={details} onChange={(e) => setDetails(e.target.value)} placeholder={`Ex: 2 ${type.toLowerCase()} extras`} required />
              </div>
              <div className="field">
                <label>Seu nome</label>
                <input value={name} onChange={(e) => setName(e.target.value)} required />
              </div>
              <div className="field">
                <label>Número do quarto</label>
                <input value={room} onChange={(e) => setRoom(e.target.value)} required />
              </div>
              <button type="submit" disabled={submitting}>
                {submitting ? "Enviando..." : "Enviar solicitação"}
              </button>
            </form>
          )}
        </>
      )}
    </div>
  );
}
