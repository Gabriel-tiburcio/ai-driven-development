import { useEffect, useState } from "react";
import { Navigate, useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";
import CategoryThumb from "../components/CategoryThumb";
import { useGuest } from "../context/GuestContext";
import type { KidsActivity } from "../types";

export default function RecreacaoInfantilDetail() {
  const { id } = useParams<{ id: string }>();
  const { hotel, roomNumber, setGuestInfo, guestName } = useGuest();
  const navigate = useNavigate();

  const [activity, setActivity] = useState<KidsActivity | null>(null);
  const [notFound, setNotFound] = useState(false);
  const [childName, setChildName] = useState("");
  const [childAge, setChildAge] = useState("");
  const [room, setRoom] = useState(roomNumber);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    if (!hotel || !id) return;
    api
      .getKidsActivity(hotel.id, id)
      .then(setActivity)
      .catch(() => setNotFound(true));
  }, [hotel, id]);

  if (!hotel) return <Navigate to="/" replace />;
  if (notFound) return <Navigate to="/recreacao-infantil" replace />;
  if (!activity) return <div className="screen">Carregando...</div>;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setGuestInfo(room, guestName);
    await api.enrollKidsActivity(hotel.id, activity.id, {
      childName,
      childAge,
      guardianRoomNumber: room,
      guardianName: guestName || undefined,
    });
    setSuccess(true);
  };

  if (success) {
    return (
      <div className="screen center">
        <div className="success-box">
          <h2>Inscrição enviada!</h2>
          <p>A equipe de recreação vai confirmar a vaga de {childName} com o quarto {room}.</p>
          <button className="secondary" onClick={() => navigate("/recreacao-infantil")}>
            Voltar à recreação infantil
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
        <CategoryThumb imageUrl={activity.imageUrl} kind="kids" />
      </div>

      <h1>{activity.name}</h1>
      <span className="badge">{activity.ageRange}</span>
      <p className="muted">{activity.schedule} · {activity.location}</p>
      <p>{activity.description}</p>

      <h3>Inscrever criança</h3>
      <form onSubmit={handleSubmit} className="reserve-form">
        <div className="field">
          <label>Nome da criança</label>
          <input value={childName} onChange={(e) => setChildName(e.target.value)} required />
        </div>
        <div className="field">
          <label>Idade</label>
          <input value={childAge} onChange={(e) => setChildAge(e.target.value)} required />
        </div>
        <div className="field">
          <label>Número do quarto do responsável</label>
          <input value={room} onChange={(e) => setRoom(e.target.value)} required />
        </div>
        <button type="submit">Inscrever criança</button>
      </form>
    </div>
  );
}
