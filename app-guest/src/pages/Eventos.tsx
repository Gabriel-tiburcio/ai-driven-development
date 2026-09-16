import { useEffect, useState } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { api } from "../api/client";
import { useGuest } from "../context/GuestContext";
import type { EventItem } from "../types";

export default function Eventos() {
  const { hotel } = useGuest();
  const navigate = useNavigate();
  const [events, setEvents] = useState<EventItem[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!hotel) return;
    api
      .getEvents(hotel.id)
      .then(setEvents)
      .finally(() => setLoading(false));
  }, [hotel]);

  if (!hotel) return <Navigate to="/" replace />;

  return (
    <div className="screen">
      <button className="link-back" onClick={() => navigate(-1)}>
        &larr; Voltar
      </button>

      <header className="page-header">
        <h1>Eventos</h1>
        <p className="subtitle">Programação especial em {hotel.name}.</p>
      </header>

      {loading ? (
        <p>Carregando eventos...</p>
      ) : events.length === 0 ? (
        <p>Nenhum evento programado no momento.</p>
      ) : (
        <div className="card-list">
          {events.map((e) => {
            const dt = new Date(`${e.eventDate}T00:00:00`);
            return (
              <Link to={`/eventos/${e.id}`} key={e.id} className="activity-card">
                <div className="activity-card-body">
                  <div className="activity-card-top">
                    <span className="badge">{e.category}</span>
                  </div>
                  <h3>{e.name}</h3>
                  <p className="muted">
                    {dt.toLocaleDateString("pt-BR", { day: "2-digit", month: "2-digit" })} · {e.startTime.slice(0, 5)} · {e.location}
                  </p>
                </div>
              </Link>
            );
          })}
        </div>
      )}
    </div>
  );
}
