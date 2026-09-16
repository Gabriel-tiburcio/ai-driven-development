import { useEffect, useState } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { api } from "../api/client";
import { useGuest } from "../context/GuestContext";
import type { ExternalExperience } from "../types";

export default function ExperienciasExternas() {
  const { hotel } = useGuest();
  const navigate = useNavigate();
  const [experiences, setExperiences] = useState<ExternalExperience[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!hotel) return;
    api
      .getExternalExperiences(hotel.id)
      .then(setExperiences)
      .finally(() => setLoading(false));
  }, [hotel]);

  if (!hotel) return <Navigate to="/" replace />;

  return (
    <div className="screen">
      <button className="link-back" onClick={() => navigate(-1)}>
        &larr; Voltar
      </button>

      <header className="page-header">
        <h1>Experiências Externas</h1>
        <p className="subtitle">Passeios e experiências fora de {hotel.name}, com a curadoria do hotel.</p>
      </header>

      {loading ? (
        <p>Carregando experiências...</p>
      ) : experiences.length === 0 ? (
        <p>Nenhuma experiência disponível no momento.</p>
      ) : (
        <div className="card-list">
          {experiences.map((x) => (
            <Link to={`/experiencias-externas/${x.id}`} key={x.id} className="activity-card">
              <div className="activity-card-body">
                <div className="activity-card-top">
                  <span className="badge">{x.category}</span>
                </div>
                <h3>{x.name}</h3>
                <p className="muted">{x.durationLabel} · {x.location}</p>
                <p className="price">R$ {x.price.toFixed(2)}</p>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
