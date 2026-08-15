import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import type { Hotel } from "../types";

interface GuestSession {
  hotel: Hotel | null;
  roomNumber: string;
  guestName: string;
  setHotel: (hotel: Hotel) => void;
  setGuestInfo: (roomNumber: string, guestName: string) => void;
  clear: () => void;
}

const STORAGE_KEY = "allstay.guest-session";

const GuestContext = createContext<GuestSession | undefined>(undefined);

function loadStoredSession(): { hotel: Hotel | null; roomNumber: string; guestName: string } {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return { hotel: null, roomNumber: "", guestName: "" };
    return JSON.parse(raw);
  } catch {
    return { hotel: null, roomNumber: "", guestName: "" };
  }
}

export function GuestProvider({ children }: { children: ReactNode }) {
  const stored = loadStoredSession();
  const [hotel, setHotelState] = useState<Hotel | null>(stored.hotel);
  const [roomNumber, setRoomNumber] = useState(stored.roomNumber);
  const [guestName, setGuestName] = useState(stored.guestName);

  useEffect(() => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify({ hotel, roomNumber, guestName }));
  }, [hotel, roomNumber, guestName]);

  const setHotel = (h: Hotel) => setHotelState(h);
  const setGuestInfo = (room: string, name: string) => {
    setRoomNumber(room);
    setGuestName(name);
  };
  const clear = () => {
    setHotelState(null);
    setRoomNumber("");
    setGuestName("");
    localStorage.removeItem(STORAGE_KEY);
  };

  return (
    <GuestContext.Provider value={{ hotel, roomNumber, guestName, setHotel, setGuestInfo, clear }}>
      {children}
    </GuestContext.Provider>
  );
}

export function useGuest() {
  const ctx = useContext(GuestContext);
  if (!ctx) throw new Error("useGuest must be used within GuestProvider");
  return ctx;
}
