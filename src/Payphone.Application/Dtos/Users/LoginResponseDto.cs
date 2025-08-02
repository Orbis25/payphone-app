namespace Payphone.Application.Dtos.Users;

public class LoginResponseDto
{
    public string? UserId { get; set; }
    public string? Jwt { get; set; }
}