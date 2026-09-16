import type {
  Activity,
  EventItem,
  ExternalExperience,
  GuestRequest,
  Hotel,
  HotelInfoSection,
  KidsActivity,
  Reservation,
  Restaurant,
  Service,
} from "../types";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "https://localhost:7001";

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...(init?.headers ?? {}),
    },
  });

  if (!response.ok) {
    const body = await response.json().catch(() => null);
    throw new Error(body?.message ?? `Request failed with status ${response.status}`);
  }

  if (response.status === 204) return undefined as T;
  return (await response.json()) as T;
}

export const api = {
  getHotelByCode: (code: string) => request<Hotel>(`/api/hotels/by-code/${encodeURIComponent(code)}`),

  getActivities: (hotelId: string, category?: string) => {
    const query = category ? `?category=${encodeURIComponent(category)}` : "";
    return request<Activity[]>(`/api/hotels/${hotelId}/activities${query}`);
  },

  getActivity: (hotelId: string, activityId: string) =>
    request<Activity>(`/api/hotels/${hotelId}/activities/${activityId}`),

  createReservation: (hotelId: string, payload: { slotId: string; guestName: string; roomNumber: string }) =>
    request<Reservation>(`/api/hotels/${hotelId}/reservations`, {
      method: "POST",
      body: JSON.stringify(payload),
    }),

  getMyReservations: (hotelId: string, room: string) =>
    request<Reservation[]>(`/api/hotels/${hotelId}/reservations/mine?room=${encodeURIComponent(room)}`),

  cancelReservation: (hotelId: string, reservationId: string) =>
    request<void>(`/api/hotels/${hotelId}/reservations/${reservationId}/cancel`, { method: "POST" }),

  getInfoSections: (hotelId: string) => request<HotelInfoSection[]>(`/api/hotels/${hotelId}/info-sections`),

  getEvents: (hotelId: string) => request<EventItem[]>(`/api/hotels/${hotelId}/events`),

  getEvent: (hotelId: string, eventId: string) =>
    request<EventItem>(`/api/hotels/${hotelId}/events/${eventId}`),

  getGuestRequests: (hotelId: string, room: string) =>
    request<GuestRequest[]>(`/api/hotels/${hotelId}/requests/mine?room=${encodeURIComponent(room)}`),

  createGuestRequest: (hotelId: string, payload: { type: string; details: string; guestName: string; roomNumber: string }) =>
    request<GuestRequest>(`/api/hotels/${hotelId}/requests`, {
      method: "POST",
      body: JSON.stringify(payload),
    }),

  getKidsActivities: (hotelId: string) => request<KidsActivity[]>(`/api/hotels/${hotelId}/kids-activities`),

  getKidsActivity: (hotelId: string, kidsActivityId: string) =>
    request<KidsActivity>(`/api/hotels/${hotelId}/kids-activities/${kidsActivityId}`),

  enrollKidsActivity: (
    hotelId: string,
    kidsActivityId: string,
    payload: { childName: string; childAge: string; guardianRoomNumber: string; guardianName?: string }
  ) =>
    request(`/api/hotels/${hotelId}/kids-activities/${kidsActivityId}/enrollments`, {
      method: "POST",
      body: JSON.stringify(payload),
    }),

  getRestaurants: (hotelId: string) => request<Restaurant[]>(`/api/hotels/${hotelId}/restaurants`),

  getRestaurant: (hotelId: string, restaurantId: string) =>
    request<Restaurant>(`/api/hotels/${hotelId}/restaurants/${restaurantId}`),

  getServices: (hotelId: string) => request<Service[]>(`/api/hotels/${hotelId}/services`),

  getService: (hotelId: string, serviceId: string) =>
    request<Service>(`/api/hotels/${hotelId}/services/${serviceId}`),

  requestService: (hotelId: string, serviceId: string, payload: { guestName: string; roomNumber: string }) =>
    request(`/api/hotels/${hotelId}/services/${serviceId}/requests`, {
      method: "POST",
      body: JSON.stringify(payload),
    }),

  getExternalExperiences: (hotelId: string) =>
    request<ExternalExperience[]>(`/api/hotels/${hotelId}/external-experiences`),

  getExternalExperience: (hotelId: string, experienceId: string) =>
    request<ExternalExperience>(`/api/hotels/${hotelId}/external-experiences/${experienceId}`),

  requestExternalExperience: (hotelId: string, experienceId: string, payload: { guestName: string; roomNumber: string }) =>
    request(`/api/hotels/${hotelId}/external-experiences/${experienceId}/requests`, {
      method: "POST",
      body: JSON.stringify(payload),
    }),

  askConcierge: (
    hotelId: string,
    payload: { message: string; history: { role: "guest" | "concierge"; text: string }[] }
  ) =>
    request<{ reply: string }>(`/api/hotels/${hotelId}/concierge/chat`, {
      method: "POST",
      body: JSON.stringify(payload),
    }),
};
