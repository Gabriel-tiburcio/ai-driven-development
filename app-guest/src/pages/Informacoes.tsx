import { useEffect, useMemo, useState } from "react";
import { Navigate } from "react-router-dom";
import { api } from "../api/client";
import Icon from "../components/Icon";
import { useGuest } from "../context/GuestContext";
import type { HotelInfoSection } from "../types";

export default function Informacoes() {
  const { hotel } = useGuest();
  const [sections, setSections] = useState<HotelInfoSection[]>([]);
  const [loading, setLoading] = useState(true);
  const [query, setQuery] = useState("");
  const [activeChip, setActiveChip] = useState<string | null>(null);
  const [openId, setOpenId] = useState<string | null>(null);
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    if (!hotel) return;
    api
      .getInfoSections(hotel.id)
      .then(setSections)
      .finally(() => setLoading(false));
  }, [hotel]);

  const wifiSection = useMemo(
    () => sections.find((s) => /wi-?fi|internet/i.test(s.title)) ?? null,
    [sections]
  );
  const otherSections = sections.filter((s) => s.id !== wifiSection?.id);

  const filtered = otherSections.filter((s) => {
    const matchesQuery = !query || `${s.title} ${s.content}`.toLowerCase().includes(query.toLowerCase());
    const matchesChip = !activeChip || s.title === activeChip;
    return matchesQuery && matchesChip;
  });

  if (!hotel) return <Navigate to="/" replace />;

  const handleCopyWifi = async () => {
    if (!wifiSection) return;
    try {
      await navigator.clipboard.writeText(wifiSection.content);
      setCopied(true);
      setTimeout(() => setCopied(false), 1500);
    } catch {
      // clipboard unavailable — silently ignore
    }
  };

  return (
    <div className="screen">
      <header className="page-header">
        <h1>Informações do hotel</h1>
      </header>

      <div className="search-bar">
        <Icon name="search" />
        <input placeholder="Buscar informações..." value={query} onChange={(e) => setQuery(e.target.value)} />
      </div>

      {otherSections.length > 0 && (
        <div className="chip-row">
          {otherSections.slice(0, 6).map((s) => (
            <button
              key={s.id}
              className={`chip ${activeChip === s.title ? "active" : ""}`}
              onClick={() => setActiveChip(activeChip === s.title ? null : s.title)}
            >
              {s.title}
            </button>
          ))}
        </div>
      )}

      {loading ? (
        <p className="muted">Carregando informações...</p>
      ) : (
        <>
          {wifiSection && (
            <div className="highlight-card">
              <div>
                <h3>
                  <Icon name="wifi" style={{ width: 16, height: 16, verticalAlign: "-2px", marginRight: 4 }} />
                  {wifiSection.title}
                </h3>
                <p className="muted small">{wifiSection.content}</p>
              </div>
              <button className="copy-btn" onClick={handleCopyWifi}>
                {copied ? "Copiado!" : "Copiar"}
              </button>
            </div>
          )}

          {filtered.length === 0 ? (
            <p className="muted">Nenhuma informação encontrada.</p>
          ) : (
            filtered.map((section) => {
              const open = openId === section.id;
              return (
                <div className="accordion-item" key={section.id}>
                  <button className={`accordion-row ${open ? "open" : ""}`} onClick={() => setOpenId(open ? null : section.id)}>
                    {section.title}
                    <Icon name="chevron-right" />
                  </button>
                  {open && <div className="accordion-body">{section.content}</div>}
                </div>
              );
            })
          )}
        </>
      )}
    </div>
  );
}
