import { useEffect, useState } from "react";
import { Navigate, useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";
import CategoryThumb from "../components/CategoryThumb";
import { useGuest } from "../context/GuestContext";
import type { Restaurant } from "../types";

export default function RestauranteDetail() {
  const { id } = useParams<{ id: string }>();
  const { hotel } = useGuest();
  const navigate = useNavigate();
  const [restaurant, setRestaurant] = useState<Restaurant | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!hotel || !id) return;
    api
      .getRestaurant(hotel.id, id)
      .then(setRestaurant)
      .catch(() => setRestaurant(null))
      .finally(() => setLoading(false));
  }, [hotel, id]);

  if (!hotel) return <Navigate to="/" replace />;
  if (loading) return <div className="screen center">Carregando...</div>;
  if (!restaurant) return <Navigate to="/restaurante" replace />;

  return (
    <div className="screen">
      <button className="link-back" onClick={() => navigate(-1)}>
        &larr; Voltar
      </button>

      <div className="hero-photo">
        <CategoryThumb imageUrl={restaurant.imageUrl} category={restaurant.cuisineType} kind="restaurant" />
      </div>

      <h1>{restaurant.name}</h1>
      <p className="muted">
        {restaurant.cuisineType} · {restaurant.hours}
      </p>
      <p>{restaurant.description}</p>

      {restaurant.menuHighlights.length > 0 && (
        <>
          <h3 className="section-title">Destaques do cardápio</h3>
          <div className="card-list">
            {restaurant.menuHighlights.map((item) => (
              <div className="activity-card" key={item}>
                <div className="activity-card-body">
                  <p style={{ margin: 0 }}>{item}</p>
                </div>
              </div>
            ))}
          </div>
        </>
      )}
    </div>
  );
}
