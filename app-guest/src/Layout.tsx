import { useState } from "react";
import { NavLink, Outlet } from "react-router-dom";
import Drawer from "./components/Drawer";
import Icon from "./components/Icon";
import { useGuest } from "./context/GuestContext";

export default function Layout() {
  const { hotel, roomNumber } = useGuest();
  const [drawerOpen, setDrawerOpen] = useState(false);
  const today = new Date().getDate();

  return (
    <div className="app-shell">
      {hotel && (
        <header className="app-header">
          <button className="icon-btn" onClick={() => setDrawerOpen(true)} aria-label="Abrir menu">
            <Icon name="menu" />
          </button>
          {roomNumber && (
            <span className="room-pill">
              <Icon name="clipboard-list" />
              Quarto {roomNumber}
            </span>
          )}
        </header>
      )}

      {drawerOpen && <Drawer onClose={() => setDrawerOpen(false)} />}

      <div className="app-content">
        <Outlet />
      </div>

      {hotel && (
        <nav className="bottom-nav">
          <NavLink to="/hoje" className={({ isActive }) => (isActive ? "active" : "")} aria-label="Hoje">
            <span className="nav-day-badge">{today}</span>
          </NavLink>
          <NavLink to="/explorar" className={({ isActive }) => (isActive ? "active" : "")} aria-label="Explorar">
            <Icon name="compass" />
          </NavLink>
          <NavLink to="/solicitacoes" className={({ isActive }) => (isActive ? "active" : "")} aria-label="Solicitações">
            <Icon name="receipt" />
          </NavLink>
          <NavLink to="/concierge" className={({ isActive }) => (isActive ? "active" : "")} aria-label="Concierge">
            <Icon name="headset" />
          </NavLink>
        </nav>
      )}
    </div>
  );
}
