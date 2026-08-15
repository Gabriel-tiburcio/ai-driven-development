import { useEffect, useState } from "react";
import { Navigate, useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";
import { useGuest } from "../context/GuestContext";
import type { Activity, ActivitySlot } from "../types";

export default function ActivityDetailPage() {
  const { activityId } = useParams<{ activityId: string }>();
  const { hotel, roomNumber, guestName, setGuestInfo } = useGuest();
  const navigate = useNavigate();

  const [activity, setActivity] = useState<Activity | null>(null);
  const [selectedSlot, setSelectedSlot] = useState<ActivitySlot | null>(null);
  const [room, setRoom] = useState(roomNumber);
  const [name, setName] = useState(guestName);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    if (!hotel || !activityId) return;
    api.getActivity(hotel.id, activityId).then(setActivity);
  }, [hotel, activityId]);

  if (!hotel) return <Navigate to="/" replace />;

  const handleReserve = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedSlot) return;

    setSubmitting(true);
    setError(null);

    try {
      await api.createReservation(hotel.id, { slotId: selectedSlot.id, guestName: name, roomNumber: room });
      setGuestInfo(room, name);
      setSuccess(true);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Não foi possível concluir a reserva.");
    } finally {
      setSubmitting(false);
    }
  };

  if (success) {
    return (
      <div className="screen center">
        <div className="success-box">
          <h2>Reserva confirmada!</h2>
          <p>Você vai receber lembretes sobre esta atividade.</p>
          <button onClick={() => navigate("/reservations")}>Ver minhas reservas</button>
          <button className="secondary" onClick={() => navigate("/catalog")}>
            Voltar ao catálogo
          </button>
        </div>
      </div>
    );
  }

  if (!activity) {
    return (
      <div className="screen center">
        <p>Carregando...</p>
      </div>
    );
  }

  return (
    <div className="screen">
      <button className="link-back" onClick={() => navigate(-1)}>
        &larr; Voltar
      </button>

      <h1>{activity.name}</h1>
      <span className="badge">{activity.category}</span>
      <p className="muted">{activity.durationMinutes} min · {activity.price === 0 ? "Incluso na estadia" : `R$ ${activity.price.toFixed(2)}`}</p>
      <p>{activity.description}</p>

      <h3>Horários disponíveis</h3>
      <div className="slot-list">
        {activity.slots.length === 0 && <p className="muted">Nenhum horário disponível.</p>}
        {activity.slots.map((slot) => {
          const dt = new Date(slot.startTime);
          const disabled = slot.availableSpots <= 0;
          return (
            <button
              key={slot.id}
              disabled={disabled}
              className={`slot-chip ${selectedSlot?.id === slot.id ? "active" : ""}`}
              onClick={() => setSelectedSlot(slot)}
            >
              {dt.toLocaleDateString("pt-BR", { day: "2-digit", month: "2-digit" })}{" "}
              {dt.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" })}
              <br />
              <small>{disabled ? "Esgotado" : `${slot.availableSpots} vagas`}</small>
            </button>
          );
        })}
      </div>

      {selectedSlot && (
        <form onSubmit={handleReserve} className="reserve-form">
          <div className="field">
            <label>Seu nome</label>
            <input value={name} onChange={(e) => setName(e.target.value)} required />
          </div>
          <div className="field">
            <label>Número do quarto</label>
            <input value={room} onChange={(e) => setRoom(e.target.value)} required />
          </div>
          {error && <p className="error">{error}</p>}
          <button type="submit" disabled={submitting}>
            {submitting ? "Reservando..." : "Confirmar reserva"}
          </button>
          <p className="muted small">Cobrado na conta do quarto no checkout.</p>
        </form>
      )}
    </div>
  );
}
