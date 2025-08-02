using System.Collections.Immutable;

namespace Payphone.Application.Options;

public class CorsConfigOption
{
    public IImmutableList<string> OriginsAllowed { get; set; } = [];
    public IImmutableList<string> MethodsAllowed { get; set; } = [];

}