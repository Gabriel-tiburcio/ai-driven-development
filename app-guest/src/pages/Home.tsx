import { useState } from "react";
import { Navigate, useNavigate } from "react-router-dom";
import { useGuest } from "../context/GuestContext";

export default function Home() {
  const { hotel } = useGuest();
  const navigate = useNavigate();
  const [code, setCode] = useState("");

  if (hotel) return <Navigate to="/catalog" replace />;

  return (
    <div className="screen center">
      <img src="/logo.png" alt="AllStay" className="brand-logo" />
      <h1>AllStay</h1>
      <p className="muted">Escaneie o QR code no seu quarto para começar.</p>

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
