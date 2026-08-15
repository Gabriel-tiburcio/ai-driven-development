using AllStay.Application.DTOs;

namespace AllStay.Application.Interfaces;

public interface IReservationService
{
    Task<IReadOnlyList<ReservationDto>> ListForHotelAsync(Guid hotelId, CancellationToken ct = default);
    Task<IReadOnlyList<ReservationDto>> ListForGuestAsync(Guid hotelId, string roomNumber, CancellationToken ct = default);

    /// <returns>Null if the slot doesn't belong to the hotel; throws InvalidOperationException if the slot is full.</returns>
    Task<ReservationDto?> CreateAsync(Guid hotelId, CreateReservationRequest request, CancellationToken ct = default);
    Task<bool> CancelAsync(Guid hotelId, Guid reservationId, CancellationToken ct = default);
}
