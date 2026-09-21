import { useEffect, useState } from "react";
import { Navigate, useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";
import CategoryThumb from "../components/CategoryThumb";
import Icon from "../components/Icon";
import { useGuest } from "../context/GuestContext";
import type { Activity, Reservation } from "../types";

export default function ReservaDetail() {
  const { reservationId } = useParams<{ reservationId: string }>();
  const { hotel, roomNumber } = useGuest();
  const navigate = useNavigate();

  const [reservation, setReservation] = useState<Reservation | null>(null);
  const [activity, setActivity] = useState<Activity | null>(null);
  const [loading, setLoading] = useState(true);
  const [cancelling, setCancelling] = useState(false);

  useEffect(() => {
    if (!hotel || !roomNumber || !reservationId) {
      setLoading(false);
      return;
    }
    api
      .getMyReservations(hotel.id, roomNumber)
      .then(async (all) => {
        const found = all.find((r) => r.id === reservationId) ?? null;
        setReservation(found);
        if (found) {
          const a = await api.getActivity(hotel.id, found.activityId).catch(() => null);
          setActivity(a);
        }
      })
      .finally(() => setLoading(false));
  }, [hotel, roomNumber, reservationId]);

  if (!hotel) return <Navigate to="/" replace />;
  if (loading) return <div className="screen center">Carregando...</div>;
  if (!reservation) return <Navigate to="/hoje" replace />;

  const cancelled = reservation.status === "Cancelled" || reservation.status === 1;
  const dt = new Date(reservation.slotStartTime);

  const handleCancel = async () => {
    setCancelling(true);
    try {
      await api.cancelReservation(hotel.id, reservation.id);
      navigate("/hoje", { replace: true });
    } finally {
      setCancelling(false);
    }
  };

  return (
    <div className="screen">
      <div className="detail-hero">
        <CategoryThumb imageUrl={activity?.imageUrl} category={activity?.category} kind="activity" />
        <button className="icon-btn" onClick={() => navigate(-1)} aria-label="Voltar">
          <Icon name="arrow-left" />
        </button>
        {!cancelled && (
          <span className="status-pill">
            <Icon name="check" /> Reservado
          </span>
        )}
      </div>

      <h1>{reservation.activityName}</h1>
      {activity && <p className="muted">{activity.category}</p>}

      <div className="info-grid">
        <div className="info-tile">
          <span className="info-tile-label">
            <Icon name="clock" /> Horário
          </span>
          <span className="info-tile-value">{dt.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" })}</span>
        </div>
        {activity && (
          <div className="info-tile">
            <span className="info-tile-label">
              <Icon name="timer" /> Duração
            </span>
            <span className="info-tile-value">{activity.durationMinutes} minutos</span>
          </div>
        )}
      </div>

      {activity?.description && (
        <>
          <h3>Sobre a atividade</h3>
          <p className="muted">{activity.description}</p>
        </>
      )}

      {!cancelled && (
        <button className="outline-accent" style={{ width: "100%", marginTop: "1.5rem" }} onClick={handleCancel} disabled={cancelling}>
          {cancelling ? "Cancelando..." : "Cancelar"}
        </button>
      )}
    </div>
  );
}
