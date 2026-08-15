import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";
import { useGuest } from "../context/GuestContext";

export default function HotelEntry() {
  const { code } = useParams<{ code: string }>();
  const navigate = useNavigate();
  const { setHotel } = useGuest();
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!code) return;

    api
      .getHotelByCode(code)
      .then((hotel) => {
        setHotel(hotel);
        navigate("/catalog", { replace: true });
      })
      .catch(() => setError("Não foi possível encontrar este hotel. Verifique o QR code ou fale com a recepção."));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [code]);

  if (error) {
    return (
      <div className="screen center">
        <p className="error">{error}</p>
      </div>
    );
  }

  return (
    <div className="screen center">
      <p>Conectando ao seu hotel...</p>
    </div>
  );
}
