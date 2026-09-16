import { useEffect, useState } from "react";
import { Navigate, useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";
import { useGuest } from "../context/GuestContext";
import type { ExternalExperience } from "../types";

export default function ExperienciaExternaDetail() {
  const { id } = useParams<{ id: string }>();
  const { hotel, roomNumber, guestName, setGuestInfo } = useGuest();
  const navigate = useNavigate();

  const [experience, setExperience] = useState<ExternalExperience | null>(null);
  const [loading, setLoading] = useState(true);
  const [room, setRoom] = useState(roomNumber);
  const [name, setName] = useState(guestName);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    if (!hotel || !id) return;
    api
      .getExternalExperience(hotel.id, id)
      .then(setExperience)
      .catch(() => setExperience(null))
      .finally(() => setLoading(false));
  }, [hotel, id]);

  if (!hotel) return <Navigate to="/" replace />;
  if (loading) return <div className="screen center">Carregando...</div>;
  if (!experience) return <Navigate to="/experiencias-externas" replace />;

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setGuestInfo(room, name);
    api
      .requestExternalExperience(hotel.id, experience.id, { guestName: name, roomNumber: room })
      .then(() => setSuccess(true));
  };

  if (success) {
    return (
      <div className="screen center">
        <div className="success-box">
          <h2>Solicitação enviada!</h2>
          <p>A recepção vai confirmar disponibilidade, horário de saída e pagamento com você.</p>
          <button className="secondary" onClick={() => navigate("/experiencias-externas")}>
            Voltar às experiências
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="screen">
      <button className="link-back" onClick={() => navigate(-1)}>
        &larr; Voltar
      </button>

      <h1>{experience.name}</h1>
      <span className="badge">{experience.category}</span>
      <p className="muted">
        {experience.durationLabel} · {experience.location} · R$ {experience.price.toFixed(2)}
      </p>
      <p>{experience.description}</p>

      <h3>Solicitar reserva</h3>
      <form onSubmit={handleSubmit} className="reserve-form">
        <div className="field">
          <label>Seu nome</label>
          <input value={name} onChange={(e) => setName(e.target.value)} required />
        </div>
        <div className="field">
          <label>Número do quarto</label>
          <input value={room} onChange={(e) => setRoom(e.target.value)} required />
        </div>
        <button type="submit">Solicitar reserva</button>
      </form>
    </div>
  );
}
