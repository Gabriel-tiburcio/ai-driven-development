import { useEffect, useState } from "react";
import { Link, Navigate } from "react-router-dom";
import { api } from "../api/client";
import CategoryThumb from "../components/CategoryThumb";
import { useGuest } from "../context/GuestContext";
import type { Restaurant } from "../types";

export default function Restaurante() {
  const { hotel } = useGuest();
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
      <header className="page-header">
        <h1>Restaurantes</h1>
      </header>

      {loading ? (
        <p className="muted">Carregando restaurantes...</p>
      ) : restaurants.length === 0 ? (
        <p className="muted">Nenhum restaurante disponível no momento.</p>
      ) : (
        restaurants.map((r) => (
          <Link to={`/restaurante/${r.id}`} key={r.id} className="restaurant-row">
            <div className="hero-photo">
              <CategoryThumb imageUrl={r.imageUrl} category={r.cuisineType} kind="restaurant" />
            </div>
            <h3>{r.name}</h3>
            <p className="muted small">
              {r.cuisineType} · {r.hours}
            </p>
          </Link>
        ))
      )}
    </div>
  );
}
