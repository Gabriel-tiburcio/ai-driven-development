import { useEffect, useState } from "react";
import { Link, Navigate } from "react-router-dom";
import { api } from "../api/client";
import CategoryThumb from "../components/CategoryThumb";
import Icon from "../components/Icon";
import { useGuest } from "../context/GuestContext";
import type { Activity, Reservation } from "../types";

function isToday(iso: string) {
  const d = new Date(iso);
  const now = new Date();
  return d.toDateString() === now.toDateString();
}

export default function Hoje() {
  const { hotel, roomNumber } = useGuest();
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [activities, setActivities] = useState<Record<string, Activity>>({});
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!hotel || !roomNumber) {
      setLoading(false);
      return;
    }
    api
      .getMyReservations(hotel.id, roomNumber)
      .then(async (all) => {
        const confirmed = all.filter((r) => (r.status === "Confirmed" || r.status === 0) && isToday(r.slotStartTime));
        setReservations(confirmed);
        const uniqueIds = Array.from(new Set(confirmed.map((r) => r.activityId)));
        const fetched = await Promise.all(uniqueIds.map((id) => api.getActivity(hotel.id, id).catch(() => null)));
        const map: Record<string, Activity> = {};
        fetched.forEach((a) => {
          if (a) map[a.id] = a;
        });
        setActivities(map);
      })
      .finally(() => setLoading(false));
  }, [hotel, roomNumber]);

  if (!hotel) return <Navigate to="/" replace />;

  return (
    <div className="screen">
      <header className="page-header">
        <h1>Sua agenda de hoje</h1>
      </header>

      {loading ? (
        <p className="muted">Carregando...</p>
      ) : reservations.length === 0 ? (
        <div className="empty-state">
          <p>Você ainda não tem nada reservado para hoje.</p>
          <Link to="/explorar">
            <button>Explorar atividades</button>
          </Link>
        </div>
      ) : (
        <div className="agenda-scroll">
          {reservations.map((r) => {
            const activity = activities[r.activityId];
            const dt = new Date(r.slotStartTime);
            return (
              <Link to={`/reserva/${r.id}`} key={r.id} className="agenda-card">
                <div className="agenda-card-photo">
                  <CategoryThumb imageUrl={activity?.imageUrl} category={activity?.category} kind="activity" />
                  <span className="agenda-card-time">
                    <Icon name="clock" />
                    {dt.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" })}
                  </span>
                </div>
                <h3>{r.activityName}</h3>
                {activity && <p className="muted small">{activity.category}</p>}
              </Link>
            );
          })}
        </div>
      )}
    </div>
  );
}
