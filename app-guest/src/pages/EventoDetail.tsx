import { useEffect, useState } from "react";
import { Navigate, useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";
import CategoryThumb from "../components/CategoryThumb";
import { useGuest } from "../context/GuestContext";
import type { EventItem } from "../types";

export default function EventoDetail() {
  const { id } = useParams<{ id: string }>();
  const { hotel } = useGuest();
  const navigate = useNavigate();
  const [event, setEvent] = useState<EventItem | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!hotel || !id) return;
    api
      .getEvent(hotel.id, id)
      .then(setEvent)
      .finally(() => setLoading(false));
  }, [hotel, id]);

  if (!hotel) return <Navigate to="/" replace />;
  if (!loading && !event) return <Navigate to="/explorar" replace />;

  return (
    <div className="screen">
      <button className="link-back" onClick={() => navigate(-1)}>
        &larr; Voltar
      </button>

      {loading || !event ? (
        <p>Carregando evento...</p>
      ) : (
        <>
          <div className="hero-photo">
            <CategoryThumb imageUrl={event.imageUrl} category={event.category} kind="event" />
          </div>

          <h1>{event.name}</h1>
          <span className="badge">{event.category}</span>
          <p className="muted">
            {new Date(`${event.eventDate}T00:00:00`).toLocaleDateString("pt-BR", {
              day: "2-digit",
              month: "2-digit",
              year: "numeric",
            })}{" "}
            · {event.startTime.slice(0, 5)} · {event.location}
          </p>
          {event.description && <p>{event.description}</p>}
        </>
      )}
    </div>
  );
}
