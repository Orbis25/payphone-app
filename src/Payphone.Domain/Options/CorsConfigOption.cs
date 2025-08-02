namespace Payphone.Domain.Options;

public class CorsConfigOption
{
    public IImmutableList<string> OriginsAllowed { get; set; } = [];
    public IImmutableList<string> MethodsAllowed { get; set; } = [];

}