import { useEffect, useState } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { api } from "../api/client";
import { useGuest } from "../context/GuestContext";
import type { Restaurant } from "../types";

export default function Restaurante() {
  const { hotel } = useGuest();
  const navigate = useNavigate();
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!hotel) return;
    api
      .getRestaurants(hotel.id)
      .then(setRestaurants)
      .finally(() => setLoading(false));
  }, [hotel]);

  if (!hotel) return <Navigate to="/" replace />;

  return (
    <div className="screen">
      <button className="link-back" onClick={() => navigate(-1)}>
        &larr; Voltar
      </button>

      <header className="page-header">
        <h1>Restaurante</h1>
        <p className="subtitle">Opções de alimentação em {hotel.name}.</p>
      </header>

      {loading ? (
        <p>Carregando restaurantes...</p>
      ) : restaurants.length === 0 ? (
        <p>Nenhum restaurante disponível no momento.</p>
      ) : (
        <div className="card-list">
          {restaurants.map((r) => (
            <Link to={`/restaurante/${r.id}`} key={r.id} className="activity-card">
              <div className="activity-card-body">
                <div className="activity-card-top">
                  <span className="badge">{r.cuisineType}</span>
                </div>
                <h3>{r.name}</h3>
                <p className="muted">{r.hours}</p>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
