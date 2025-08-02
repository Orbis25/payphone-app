using Microsoft.AspNetCore.Identity;

namespace Payphone.Domain.Models;

public class User : IdentityUser
{
    public string? FullName { get; set; }
    
}