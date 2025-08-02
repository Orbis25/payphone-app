namespace Payphone.Application.Services.Users;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtOption _jwtOption;

    public UserService(ILogger<UserService> logger,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IOptions<JwtOption> jwtOption)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtOption = jwtOption.Value;
    }

    public async Task<Response<LoginResponseDto>> LoginAsync(LoginDto input,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("login user with email {email}", input.Email);
        var user = await _userManager.FindByEmailAsync(input.Email!).ConfigureAwait(false);

        if (user == null)
        {
            _logger.LogInformation("user not found with email {email}", input.Email);
            return new() { Message = "invalid email/password" };
        }
        
        
        var result = await _signInManager.PasswordSignInAsync(user, input.Password!, false, false)
            .ConfigureAwait(false);

        if (!result.Succeeded)
        {
            _logger.LogInformation("invalid email/password for user with email {email}", input.Email);
            return new() { Message = "Invalid email/password" };
        }
        
        
        _logger.LogInformation("user logged in with email {email}", input.Email);
        
        var jwt = BuildToken(user);
        

        _logger.LogInformation("returning jwt for user with email {email}", input.Email);
        
        return new()
        {
            Data = new()
            {
                Jwt = jwt,
                UserId = user.Id
            }
        };
    }


    private string BuildToken(User user)
    {
        var claims = new List<Claim>
        {
            new("jti", Guid.NewGuid().ToString()),
            new("id", user.Id),
            new("email", user.Email ?? ""),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.Key ?? ""));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            _jwtOption.Issuer,
            _jwtOption.Audience,
            claims,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}