import { useEffect, useMemo, useState } from "react";
import { Link, Navigate } from "react-router-dom";
import { api } from "../api/client";
import { useGuest } from "../context/GuestContext";
import type { Activity } from "../types";

export default function Catalog() {
  const { hotel } = useGuest();
  const [activities, setActivities] = useState<Activity[]>([]);
  const [loading, setLoading] = useState(true);
  const [category, setCategory] = useState<string | null>(null);

  useEffect(() => {
    if (!hotel) return;
    api
      .getActivities(hotel.id)
      .then(setActivities)
      .finally(() => setLoading(false));
  }, [hotel]);

  const categories = useMemo(
    () => Array.from(new Set(activities.map((a) => a.category))),
    [activities]
  );

  const filtered = category ? activities.filter((a) => a.category === category) : activities;

  if (!hotel) return <Navigate to="/" replace />;

  return (
    <div className="screen">
      <header className="page-header">
        <h1>{hotel.name}</h1>
        <p className="subtitle">O que você quer aproveitar hoje?</p>
      </header>

      {categories.length > 0 && (
        <div className="chip-row">
          <button className={`chip ${category === null ? "active" : ""}`} onClick={() => setCategory(null)}>
            Tudo
          </button>
          {categories.map((c) => (
            <button
              key={c}
              className={`chip ${category === c ? "active" : ""}`}
              onClick={() => setCategory(c)}
            >
              {c}
            </button>
          ))}
        </div>
      )}

      {loading ? (
        <p>Carregando atividades...</p>
      ) : filtered.length === 0 ? (
        <p>Nenhuma atividade disponível no momento.</p>
      ) : (
        <div className="card-list">
          {filtered.map((activity) => {
            const spots = activity.slots.reduce((sum, s) => sum + s.availableSpots, 0);
            return (
              <Link to={`/activity/${activity.id}`} key={activity.id} className="activity-card">
                <div className="activity-card-body">
                  <div className="activity-card-top">
                    <span className="badge">{activity.category}</span>
                    {spots === 0 && <span className="badge badge-muted">Sem vagas</span>}
                  </div>
                  <h3>{activity.name}</h3>
                  <p className="muted">{activity.durationMinutes} min</p>
                  <p className="price">{activity.price === 0 ? "Incluso na estadia" : `R$ ${activity.price.toFixed(2)}`}</p>
                </div>
              </Link>
            );
          })}
        </div>
      )}
    </div>
  );
}
