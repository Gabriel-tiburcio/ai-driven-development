import { BrowserRouter, Route, Routes } from "react-router-dom";
import Layout from "./Layout";
import { GuestProvider } from "./context/GuestContext";
import Home from "./pages/Home";
import HotelEntry from "./pages/HotelEntry";
import Hub from "./pages/Hub";
import Catalog from "./pages/Catalog";
import ActivityDetailPage from "./pages/ActivityDetail";
import MyReservations from "./pages/MyReservations";
import Informacoes from "./pages/Informacoes";
import Restaurante from "./pages/Restaurante";
import RestauranteDetail from "./pages/RestauranteDetail";
import Eventos from "./pages/Eventos";
import EventoDetail from "./pages/EventoDetail";
import Servicos from "./pages/Servicos";
import ServicoDetail from "./pages/ServicoDetail";
import ExperienciasExternas from "./pages/ExperienciasExternas";
import ExperienciaExternaDetail from "./pages/ExperienciaExternaDetail";
import RecreacaoInfantil from "./pages/RecreacaoInfantil";
import RecreacaoInfantilDetail from "./pages/RecreacaoInfantilDetail";
import Solicitacoes from "./pages/Solicitacoes";
import Concierge from "./pages/Concierge";

export default function App() {
  return (
    <GuestProvider>
      <BrowserRouter>
        <Routes>
          <Route element={<Layout />}>
            <Route path="/" element={<Home />} />
            <Route path="/h/:code" element={<HotelEntry />} />
            <Route path="/hub" element={<Hub />} />
            <Route path="/atividades" element={<Catalog />} />
            <Route path="/activity/:activityId" element={<ActivityDetailPage />} />
            <Route path="/reservations" element={<MyReservations />} />
            <Route path="/informacoes" element={<Informacoes />} />
            <Route path="/restaurante" element={<Restaurante />} />
            <Route path="/restaurante/:id" element={<RestauranteDetail />} />
            <Route path="/eventos" element={<Eventos />} />
            <Route path="/eventos/:id" element={<EventoDetail />} />
            <Route path="/servicos" element={<Servicos />} />
            <Route path="/servicos/:id" element={<ServicoDetail />} />
            <Route path="/experiencias-externas" element={<ExperienciasExternas />} />
            <Route path="/experiencias-externas/:id" element={<ExperienciaExternaDetail />} />
            <Route path="/recreacao-infantil" element={<RecreacaoInfantil />} />
            <Route path="/recreacao-infantil/:id" element={<RecreacaoInfantilDetail />} />
            <Route path="/solicitacoes" element={<Solicitacoes />} />
            <Route path="/concierge" element={<Concierge />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </GuestProvider>
  );
}
