import { useState } from "react";
import { Navigate, useNavigate } from "react-router-dom";
import Icon from "../components/Icon";
import { useGuest } from "../context/GuestContext";

export default function Home() {
  const { hotel } = useGuest();
  const navigate = useNavigate();
  const [code, setCode] = useState("");

  if (hotel) return <Navigate to="/hoje" replace />;

  return (
    <div className="screen center qr-entry">
      <Icon name="qrcode" className="qr-icon" />
      <h1>AllStay</h1>
      <p className="subtitle">Escaneie o QR Code disponível no seu quarto para iniciar sua experiência.</p>

      <form
        className="reserve-form"
        onSubmit={(e) => {
          e.preventDefault();
          if (code.trim()) navigate(`/h/${code.trim()}`);
        }}
      >
        <div className="field">
          <label>Ou digite o código do hotel</label>
          <input value={code} onChange={(e) => setCode(e.target.value)} placeholder="ex: curacau-resort" />
        </div>
        <button type="submit">Entrar</button>
      </form>
    </div>
  );
}
