import { useEffect, useState } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { api } from "../api/client";
import CategoryThumb from "../components/CategoryThumb";
import { useGuest } from "../context/GuestContext";
import type { KidsActivity } from "../types";

export default function RecreacaoInfantil() {
  const { hotel } = useGuest();
  const navigate = useNavigate();
  const [kidsActivities, setKidsActivities] = useState<KidsActivity[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!hotel) return;
    api
      .getKidsActivities(hotel.id)
      .then(setKidsActivities)
      .finally(() => setLoading(false));
  }, [hotel]);

  if (!hotel) return <Navigate to="/" replace />;

  return (
    <div className="screen">
      <button className="link-back" onClick={() => navigate(-1)}>
        &larr; Voltar
      </button>

      <header className="page-header">
        <h1>Recreação Infantil</h1>
        <p className="subtitle">Programação para os pequenos em {hotel.name}.</p>
      </header>

      {loading ? (
        <p>Carregando atividades...</p>
      ) : kidsActivities.length === 0 ? (
        <p>Nenhuma atividade infantil disponível no momento.</p>
      ) : (
        <div className="card-list">
          {kidsActivities.map((k) => (
            <Link to={`/recreacao-infantil/${k.id}`} key={k.id} className="explore-row">
              <div className="explore-row-thumb">
                <CategoryThumb imageUrl={k.imageUrl} kind="kids" />
              </div>
              <div className="explore-row-body">
                <h3>{k.name}</h3>
                <p className="muted small">
                  {k.ageRange} · {k.schedule} · {k.location}
                </p>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
