import { useEffect, useState } from "react";
import { Navigate, useNavigate } from "react-router-dom";
import { api } from "../api/client";
import { useGuest } from "../context/GuestContext";
import type { HotelInfoSection } from "../types";

export default function Informacoes() {
  const { hotel } = useGuest();
  const navigate = useNavigate();
  const [sections, setSections] = useState<HotelInfoSection[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!hotel) return;
    api
      .getInfoSections(hotel.id)
      .then(setSections)
      .finally(() => setLoading(false));
  }, [hotel]);

  if (!hotel) return <Navigate to="/" replace />;

  return (
    <div className="screen">
      <button className="link-back" onClick={() => navigate(-1)}>
        &larr; Voltar
      </button>

      <header className="page-header">
        <h1>Informações</h1>
        <p className="subtitle">Tudo o que você precisa saber sobre {hotel.name}.</p>
      </header>

      {loading ? (
        <p>Carregando informações...</p>
      ) : sections.length === 0 ? (
        <p>Nenhuma informação disponível no momento.</p>
      ) : (
        <div className="card-list">
          {sections.map((section) => (
            <div className="activity-card" key={section.id}>
              <div className="activity-card-body">
                <div className="activity-card-top">
                  <h3>
                    {section.icon} {section.title}
                  </h3>
                </div>
                <p className="muted">{section.content}</p>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
