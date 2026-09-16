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

export interface HotelInfoSection {
  id: string;
  title: string;
  icon: string;
  content: string;
}

export interface Restaurant {
  id: string;
  name: string;
  description: string;
  cuisineType: string;
  hours: string;
  imageUrl: string | null;
  menuHighlights: string[];
}

export interface EventItem {
  id: string;
  name: string;
  description: string | null;
  eventDate: string;
  startTime: string;
  location: string;
  category: string;
  imageUrl: string | null;
}

export interface Service {
  id: string;
  name: string;
  description: string;
  category: string;
  price: number;
  durationMinutes: number;
  imageUrl: string | null;
}

export interface ExternalExperience {
  id: string;
  name: string;
  description: string;
  category: string;
  price: number;
  durationLabel: string;
  location: string;
  imageUrl: string | null;
}

export interface KidsActivity {
  id: string;
  name: string;
  description: string;
  ageRange: string;
  schedule: string;
  location: string;
  imageUrl: string | null;
}

export interface GuestRequest {
  id: string;
  type: string;
  details: string;
  roomNumber: string;
  guestName: string;
  status: "Pending" | "InProgress" | "Done" | number;
  createdAt: string;
}

export interface ConciergeMessage {
  id: string;
  sender: "guest" | "concierge";
  text: string;
  timestamp: string;
}
