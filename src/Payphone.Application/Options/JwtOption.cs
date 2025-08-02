namespace Payphone.Application.Options;

public class JwtOption
{
    public string? Audience { get; set; }
    public string? Issuer { get; set; }
    public string? Key { get; set; }
    public string? DefaultUser { get; set; }
    public string? DefaultPassword { get; set; }
}