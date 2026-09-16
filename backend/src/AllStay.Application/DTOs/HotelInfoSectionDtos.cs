namespace AllStay.Application.DTOs;

public record HotelInfoSectionDto(
    Guid Id,
    Guid HotelId,
    string Title,
    string Icon,
    string Content,
    int SortOrder,
    bool IsActive);

public record CreateHotelInfoSectionRequest(
    string Title,
    string Icon,
    string Content,
    int SortOrder);

public record UpdateHotelInfoSectionRequest(
    string Title,
    string Icon,
    string Content,
    int SortOrder,
    bool IsActive);
