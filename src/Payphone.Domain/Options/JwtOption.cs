namespace Payphone.Domain.Options;

public class JwtOption
{
    public string? Audience { get; set; }
    public string? Issuer { get; set; }
    public string? Key { get; set; }
}