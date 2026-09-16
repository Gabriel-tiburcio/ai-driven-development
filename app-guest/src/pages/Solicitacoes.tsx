import { useEffect, useState } from "react";
import { Navigate, useNavigate } from "react-router-dom";
import { api } from "../api/client";
import { useGuest } from "../context/GuestContext";
import type { GuestRequest } from "../types";

const REQUEST_TYPES = ["Toalhas", "Amenities", "Manutenção", "Limpeza", "Outro"];

export default function Solicitacoes() {
  const { hotel, roomNumber, guestName, setGuestInfo } = useGuest();
  const navigate = useNavigate();

  const [requests, setRequests] = useState<GuestRequest[]>([]);
  const [type, setType] = useState(REQUEST_TYPES[0]);
  const [details, setDetails] = useState("");
  const [room, setRoom] = useState(roomNumber);
  const [name, setName] = useState(guestName);

  useEffect(() => {
    if (hotel && roomNumber) {
      api.getGuestRequests(hotel.id, roomNumber).then(setRequests);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [hotel]);

  if (!hotel) return <Navigate to="/" replace />;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setGuestInfo(room, name);

    await api.createGuestRequest(hotel.id, { type, details, guestName: name, roomNumber: room });
    const updated = await api.getGuestRequests(hotel.id, room);
    setRequests(updated);
    setDetails("");
  };

  return (
    <div className="screen">
      <button className="link-back" onClick={() => navigate(-1)}>
        &larr; Voltar
      </button>

      <header className="page-header">
        <h1>Solicitações</h1>
        <p className="subtitle">Peça itens ou atendimento sem precisar ligar para a recepção.</p>
      </header>

      <form onSubmit={handleSubmit} className="reserve-form">
        <div className="field">
          <label>Tipo de solicitação</label>
          <select value={type} onChange={(e) => setType(e.target.value)}>
            {REQUEST_TYPES.map((t) => (
              <option key={t} value={t}>
                {t}
              </option>
            ))}
          </select>
        </div>
        <div className="field">
          <label>Detalhes</label>
          <input value={details} onChange={(e) => setDetails(e.target.value)} placeholder="Ex: 2 toalhas extras" required />
        </div>
        <div className="field">
          <label>Seu nome</label>
          <input value={name} onChange={(e) => setName(e.target.value)} required />
        </div>
        <div className="field">
          <label>Número do quarto</label>
          <input value={room} onChange={(e) => setRoom(e.target.value)} required />
        </div>
        <button type="submit">Enviar solicitação</button>
      </form>

      {requests.length > 0 && (
        <>
          <h3>Suas solicitações</h3>
          <div className="card-list">
            {requests.map((r) => {
              const done = r.status === "Done" || r.status === 2;
              const label = done ? "Concluída" : r.status === "InProgress" || r.status === 1 ? "Em andamento" : "Pendente";
              return (
                <div className="activity-card ticket-stub" key={r.id}>
                  <div className="activity-card-body">
                    <span className={`ticket-stub-status ${done ? "is-confirmed" : ""}`}>{label}</span>
                    <h3>{r.type}</h3>
                    <p className="muted">{r.details}</p>
                    <p className="muted">Quarto {r.roomNumber}</p>
                  </div>
                </div>
              );
            })}
          </div>
        </>
      )}
    </div>
  );
}
