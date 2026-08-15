import { BrowserRouter, Route, Routes } from "react-router-dom";
import Layout from "./Layout";
import { GuestProvider } from "./context/GuestContext";
import Home from "./pages/Home";
import HotelEntry from "./pages/HotelEntry";
import Catalog from "./pages/Catalog";
import ActivityDetailPage from "./pages/ActivityDetail";
import MyReservations from "./pages/MyReservations";

export default function App() {
  return (
    <GuestProvider>
      <BrowserRouter>
        <Routes>
          <Route element={<Layout />}>
            <Route path="/" element={<Home />} />
            <Route path="/h/:code" element={<HotelEntry />} />
            <Route path="/catalog" element={<Catalog />} />
            <Route path="/activity/:activityId" element={<ActivityDetailPage />} />
            <Route path="/reservations" element={<MyReservations />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </GuestProvider>
  );
}
