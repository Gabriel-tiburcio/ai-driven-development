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
          <NavLink to="/catalog" className={({ isActive }) => (isActive ? "active" : "")}>
            Catálogo
          </NavLink>
          <NavLink to="/reservations" className={({ isActive }) => (isActive ? "active" : "")}>
            Minhas reservas
          </NavLink>
        </nav>
      )}
    </div>
  );
}
