import { useEffect, useState } from "react";
import { Navigate, useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";
import CategoryThumb from "../components/CategoryThumb";
import { useGuest } from "../context/GuestContext";
import type { Service } from "../types";

export default function ServicoDetail() {
  const { id } = useParams<{ id: string }>();
  const { hotel, roomNumber, guestName, setGuestInfo } = useGuest();
  const navigate = useNavigate();

  const [service, setService] = useState<Service | null>(null);
  const [loading, setLoading] = useState(true);
  const [room, setRoom] = useState(roomNumber);
  const [name, setName] = useState(guestName);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    if (!hotel || !id) return;
    api
      .getService(hotel.id, id)
      .then(setService)
      .catch(() => setService(null))
      .finally(() => setLoading(false));
  }, [hotel, id]);

  if (!hotel) return <Navigate to="/" replace />;
  if (loading) return <div className="screen center">Carregando...</div>;
  if (!service) return <Navigate to="/servicos" replace />;

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setGuestInfo(room, name);
    api.requestService(hotel.id, service.id, { guestName: name, roomNumber: room }).then(() => setSuccess(true));
  };

  if (success) {
    return (
      <div className="screen center">
        <div className="success-box">
          <h2>Solicitação enviada!</h2>
          <p>A recepção vai confirmar horário e pagamento com você.</p>
          <button className="secondary" onClick={() => navigate("/servicos")}>
            Voltar aos serviços
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

      <div className="hero-photo">
        <CategoryThumb imageUrl={service.imageUrl} category={service.category} kind="service" />
      </div>

      <h1>{service.name}</h1>
      <span className="badge">{service.category}</span>
      <p className="muted">
        {service.durationMinutes} min · R$ {service.price.toFixed(2)}
      </p>
      <p>{service.description}</p>

      <h3>Solicitar contratação</h3>
      <form onSubmit={handleSubmit} className="reserve-form">
        <div className="field">
          <label>Seu nome</label>
          <input value={name} onChange={(e) => setName(e.target.value)} required />
        </div>
        <div className="field">
          <label>Número do quarto</label>
          <input value={room} onChange={(e) => setRoom(e.target.value)} required />
        </div>
        <button type="submit">Solicitar contratação</button>
      </form>
    </div>
  );
}
