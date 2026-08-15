import type { Activity, Hotel, Reservation } from "../types";

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
};
