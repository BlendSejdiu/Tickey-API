using Microsoft.AspNetCore.Identity;
using Tickey.Sevices;

namespace Tickey.Endpoints;

public static class UserEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/users/register", async (UserDTO userDto, AppDbContext db, ILogger<UserEndpointsLogger> logger) =>
        {
            if (await db.Users.AnyAsync(u => u.Email == userDto.Email))
                return Results.BadRequest(new { Message = "Email already exists." });

            if (string.IsNullOrWhiteSpace(userDto.Password))
                return Results.BadRequest(new { Message = "Password is required." });

            var user = new User();
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, userDto.Password);

            user.Id = new Guid();
            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.PasswordHash = hashedPassword;

            user.Role = user.Email == "admin@gmail.com" ? "Admin" : "User";

            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            logger.LogInformation("New user registered: {Username} ({Email})", user.Username, user.Email);

            return Results.Created($"/users/{user.Id}", new { user.Id, user.Username, user.Email });

        }).WithName("RegisterUser").WithTags("Users").WithSummary("Register a new user");

        app.MapPost("/users/login", async (UserDTO userDto, AppDbContext db, TokenService tokenService) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (user == null)
                return Results.BadRequest(new { Message = "Invalid email or password." });

            var passwordVerificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, userDto.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
                return Results.BadRequest(new { Message = "Invalid email or password." });

            var response = new TokenResponseDTO
            {
                AccessToken = tokenService.CreateToken(user),
                RefreshToken = await tokenService.GenerateAndSaveRefreshTokenAsync(user),
            };

            return Results.Ok(new { Message = "Login successful.", response });
        }).WithName("LoginUser").WithTags("Users").WithSummary("Login a user");
    }

    internal sealed class UserEndpointsLogger
    {
    }
}