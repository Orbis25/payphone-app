using System.Reflection;
using Payphone.Application.Services.Core;

namespace Payphone.Infrastructure.EF.Seeds;

public static class SeedConfiguration
{
    public static void ApplySeedConfiguration(this IApplicationBuilder applicationBuilder)
    {
        using var scope = applicationBuilder.ApplicationServices.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var seedTypes = Assembly.GetAssembly(typeof(ISeed))?
            .GetTypes()
            .Where(t => typeof(ISeed).IsAssignableFrom(t) && t.IsInterface) ?? [];

        try
        {
            foreach (var service in seedTypes)
            {
                var serviceInstance = serviceProvider.GetRequiredService(service) as ISeed;
                serviceInstance?.SeedAsync().GetAwaiter().GetResult();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred while seeding the database : "+e?.Message);
            
        }
    }
}