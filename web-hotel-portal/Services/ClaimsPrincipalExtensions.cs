using System.Security.Claims;

namespace AllStay.Web.HotelPortal.Services;

public static class AllStayClaimTypes
{
    public const string Jwt = "allstay_jwt";
    public const string HotelId = "allstay_hotel_id";
    public const string HotelName = "allstay_hotel_name";
}

public static class ClaimsPrincipalExtensions
{
    public static string? GetJwt(this ClaimsPrincipal user) => user.FindFirst(AllStayClaimTypes.Jwt)?.Value;

    public static Guid GetHotelId(this ClaimsPrincipal user)
        => Guid.TryParse(user.FindFirst(AllStayClaimTypes.HotelId)?.Value, out var id) ? id : Guid.Empty;

    public static string? GetHotelName(this ClaimsPrincipal user) => user.FindFirst(AllStayClaimTypes.HotelName)?.Value;

    public static string? GetFullName(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.Name)?.Value;
}
