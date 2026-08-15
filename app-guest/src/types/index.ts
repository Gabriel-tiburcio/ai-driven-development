export interface Hotel {
  id: string;
  name: string;
  code: string;
  tier: number;
  isActive: boolean;
}

export interface ActivitySlot {
  id: string;
  startTime: string;
  capacity: number;
  bookedCount: number;
  availableSpots: number;
}

export interface Activity {
  id: string;
  hotelId: string;
  name: string;
  description: string | null;
  category: string;
  price: number;
  durationMinutes: number;
  imageUrl: string | null;
  isActive: boolean;
  slots: ActivitySlot[];
}

export interface Reservation {
  id: string;
  slotId: string;
  activityId: string;
  activityName: string;
  slotStartTime: string;
  guestName: string;
  roomNumber: string;
  status: "Confirmed" | "Cancelled" | "CompletedNoShow" | "Completed" | number;
  createdAt: string;
}
