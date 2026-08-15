import { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";
import { api } from "../api/client";
import { useGuest } from "../context/GuestContext";
import type { Reservation } from "../types";

export default function MyReservations() {
  const { hotel, roomNumber, setGuestInfo, guestName } = useGuest();
  const [room, setRoom] = useState(roomNumber);
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [loading, setLoading] = useState(false);
  const [searched, setSearched] = useState(false);

  useEffect(() => {
    if (roomNumber) load(roomNumber);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (!hotel) return <Navigate to="/" replace />;

  const load = async (roomToSearch: string) => {
    if (!roomToSearch) return;
    setLoading(true);
    setSearched(true);
    try {
      const results = await api.getMyReservations(hotel.id, roomToSearch);
      setReservations(results);
      setGuestInfo(roomToSearch, guestName);
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = async (reservationId: string) => {
    await api.cancelReservation(hotel.id, reservationId);
    load(room);
  };

  return (
    <div className="screen">
      <header className="page-header">
        <h1>Minhas reservas</h1>
      </header>

      <form
        className="reserve-form"
        onSubmit={(e) => {
          e.preventDefault();
          load(room);
        }}
      >
        <div className="field">
          <label>Número do quarto</label>
          <input value={room} onChange={(e) => setRoom(e.target.value)} required />
        </div>
        <button type="submit">Buscar</button>
      </form>

      {loading && <p>Carregando...</p>}

      {!loading && searched && reservations.length === 0 && <p>Nenhuma reserva encontrada para este quarto.</p>}

      <div className="card-list">
        {reservations.map((r) => {
          const dt = new Date(r.slotStartTime);
          const cancelled = r.status === "Cancelled" || r.status === 1;
          return (
            <div className="activity-card ticket-stub" key={r.id}>
              <div className="activity-card-body">
                <span className={`ticket-stub-status ${cancelled ? "is-cancelled" : "is-confirmed"}`}>
                  {cancelled ? "Cancelada" : "Confirmada"}
                </span>
                <h3>{r.activityName}</h3>
                <p className="muted">
                  {dt.toLocaleDateString("pt-BR")} às {dt.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" })}
                </p>
                <p className="muted">Hóspede: {r.guestName}</p>
                {!cancelled && (
                  <button className="danger" onClick={() => handleCancel(r.id)}>
                    Cancelar
                  </button>
                )}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
