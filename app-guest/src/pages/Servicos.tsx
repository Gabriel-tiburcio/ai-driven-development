import { useEffect, useMemo, useState } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { api } from "../api/client";
import { useGuest } from "../context/GuestContext";
import type { Service } from "../types";

export default function Servicos() {
  const { hotel } = useGuest();
  const navigate = useNavigate();
  const [services, setServices] = useState<Service[]>([]);
  const [loading, setLoading] = useState(true);
  const [category, setCategory] = useState<string | null>(null);

  useEffect(() => {
    if (!hotel) return;
    api
      .getServices(hotel.id)
      .then(setServices)
      .finally(() => setLoading(false));
  }, [hotel]);

  const categories = useMemo(() => Array.from(new Set(services.map((s) => s.category))), [services]);

  if (!hotel) return <Navigate to="/" replace />;

  const filtered = category ? services.filter((s) => s.category === category) : services;

  return (
    <div className="screen">
      <button className="link-back" onClick={() => navigate(-1)}>
        &larr; Voltar
      </button>

      <header className="page-header">
        <h1>Serviços</h1>
        <p className="subtitle">Experiências pagas dentro de {hotel.name}.</p>
      </header>

      {categories.length > 0 && (
        <div className="chip-row">
          <button className={`chip ${category === null ? "active" : ""}`} onClick={() => setCategory(null)}>
            Tudo
          </button>
          {categories.map((c) => (
            <button key={c} className={`chip ${category === c ? "active" : ""}`} onClick={() => setCategory(c)}>
              {c}
            </button>
          ))}
        </div>
      )}

      {loading ? (
        <p>Carregando serviços...</p>
      ) : filtered.length === 0 ? (
        <p>Nenhum serviço disponível no momento.</p>
      ) : (
        <div className="card-list">
          {filtered.map((s) => (
            <Link to={`/servicos/${s.id}`} key={s.id} className="activity-card">
              <div className="activity-card-body">
                <div className="activity-card-top">
                  <span className="badge">{s.category}</span>
                </div>
                <h3>{s.name}</h3>
                <p className="muted">{s.durationMinutes} min</p>
                <p className="price">R$ {s.price.toFixed(2)}</p>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
