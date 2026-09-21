import { useEffect, useState } from "react";
import { Link, Navigate } from "react-router-dom";
import { api } from "../api/client";
import CategoryThumb from "../components/CategoryThumb";
import { useGuest } from "../context/GuestContext";
import type { Activity, EventItem, ExternalExperience } from "../types";

type Tab = "Todas" | "Atividades" | "Eventos" | "Experiências";
const TABS: Tab[] = ["Todas", "Atividades", "Eventos", "Experiências"];

export default function Explorar() {
  const { hotel } = useGuest();
  const [tab, setTab] = useState<Tab>("Todas");
  const [activities, setActivities] = useState<Activity[]>([]);
  const [events, setEvents] = useState<EventItem[]>([]);
  const [experiences, setExperiences] = useState<ExternalExperience[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!hotel) return;
    Promise.all([
      api.getActivities(hotel.id),
      api.getEvents(hotel.id),
      api.getExternalExperiences(hotel.id),
    ])
      .then(([a, e, x]) => {
        setActivities(a);
        setEvents(e);
        setExperiences(x);
      })
      .finally(() => setLoading(false));
  }, [hotel]);

  if (!hotel) return <Navigate to="/" replace />;

  const showActivities = tab === "Todas" || tab === "Atividades";
  const showEvents = tab === "Todas" || tab === "Eventos";
  const showExperiences = tab === "Todas" || tab === "Experiências";

  const nothingToShow =
    !loading &&
    (!showActivities || activities.length === 0) &&
    (!showEvents || events.length === 0) &&
    (!showExperiences || experiences.length === 0);

  return (
    <div className="screen">
      <header className="page-header">
        <h1>Explorar</h1>
      </header>

      <div className="chip-row">
        {TABS.map((t) => (
          <button key={t} className={`chip ${tab === t ? "active" : ""}`} onClick={() => setTab(t)}>
            {t}
          </button>
        ))}
      </div>

      {loading ? (
        <p className="muted">Carregando...</p>
      ) : nothingToShow ? (
        <p className="muted">Nada disponível nesta categoria no momento.</p>
      ) : (
        <>
          {showActivities && activities.length > 0 && (
            <>
              <h2 className="section-title">Atividades</h2>
              {activities.map((a) => {
                const nextSlot = a.slots[0];
                return (
                  <Link to={`/activity/${a.id}`} key={a.id} className="explore-row">
                    <div className="explore-row-thumb">
                      <CategoryThumb imageUrl={a.imageUrl} category={a.category} kind="activity" />
                    </div>
                    <div className="explore-row-body">
                      <h3>{a.name}</h3>
                      <p className="muted small">
                        {a.category}
                        {nextSlot && ` · Hoje, ${new Date(nextSlot.startTime).toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" })}`}
                      </p>
                    </div>
                  </Link>
                );
              })}
            </>
          )}

          {showEvents && events.length > 0 && (
            <>
              <h2 className="section-title">Eventos</h2>
              {events.map((e) => {
                const dt = new Date(`${e.eventDate}T00:00:00`);
                return (
                  <Link to={`/eventos/${e.id}`} key={e.id} className="explore-row">
                    <div className="explore-row-thumb">
                      <CategoryThumb imageUrl={e.imageUrl} category={e.category} kind="event" />
                    </div>
                    <div className="explore-row-body">
                      <h3>{e.name}</h3>
                      <p className="muted small">
                        {e.category} · {dt.toLocaleDateString("pt-BR", { day: "2-digit", month: "2-digit" })}, {e.startTime.slice(0, 5)}
                      </p>
                    </div>
                  </Link>
                );
              })}
            </>
          )}

          {showExperiences && experiences.length > 0 && (
            <>
              <h2 className="section-title">Experiências</h2>
              {experiences.map((x) => (
                <Link to={`/experiencias-externas/${x.id}`} key={x.id} className="explore-row">
                  <div className="explore-row-thumb">
                    <CategoryThumb imageUrl={x.imageUrl} category={x.category} kind="experience" />
                  </div>
                  <div className="explore-row-body">
                    <h3>{x.name}</h3>
                    <p className="muted small">
                      {x.category} · {x.durationLabel}
                    </p>
                  </div>
                </Link>
              ))}
            </>
          )}
        </>
      )}
    </div>
  );
}
