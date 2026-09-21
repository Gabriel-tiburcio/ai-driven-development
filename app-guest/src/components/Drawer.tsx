import { Link } from "react-router-dom";
import Icon, { type IconName } from "./Icon";

interface DrawerLink {
  to: string;
  label: string;
  icon: IconName;
}

const LINKS: DrawerLink[] = [
  { to: "/informacoes", label: "Informações do hotel", icon: "info" },
  { to: "/restaurante", label: "Restaurantes", icon: "restaurant" },
  { to: "/servicos", label: "Serviços", icon: "service" },
  { to: "/recreacao-infantil", label: "Recreação infantil", icon: "kids" },
  { to: "/reservations", label: "Minhas reservas", icon: "clipboard-list" },
];

export default function Drawer({ onClose }: { onClose: () => void }) {
  return (
    <>
      <div className="drawer-overlay" onClick={onClose} />
      <nav className="drawer-panel">
        {LINKS.map((link) => (
          <Link key={link.to} to={link.to} className="drawer-link" onClick={onClose}>
            <Icon name={link.icon} />
            {link.label}
          </Link>
        ))}
      </nav>
    </>
  );
}
