import { NavLink, Outlet } from "react-router-dom";
import { useGuest } from "./context/GuestContext";

export default function Layout() {
  const { hotel } = useGuest();

  return (
    <div className="app-shell">
      <div className="app-content">
        <Outlet />
      </div>
      {hotel && (
        <nav className="bottom-nav">
          <NavLink to="/hub" className={({ isActive }) => (isActive ? "active" : "")}>
            Início
          </NavLink>
          <NavLink to="/concierge" className={({ isActive }) => (isActive ? "active" : "")}>
            Concierge
          </NavLink>
          <NavLink to="/reservations" className={({ isActive }) => (isActive ? "active" : "")}>
            Reservas
          </NavLink>
        </nav>
      )}
    </div>
  );
}
