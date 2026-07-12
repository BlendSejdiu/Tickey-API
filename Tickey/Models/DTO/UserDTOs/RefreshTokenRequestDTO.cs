namespace Tickey.Models.DTO.UserDTOs;

public class RefreshTokenRequestDTO
{
    public Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
}