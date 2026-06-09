using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoffeeShopRush.Data;
using CoffeeShopRush.Models;
using System.Security.Claims;

namespace CoffeeShopRush.Controllers;

public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ApplicationDbContext context, ILogger<AuthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var redirectUrl = Url.Action("GoogleResponse", "Auth");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public async Task<IActionResult> GoogleResponse()
    {
        try
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Google authentication failed");
                return RedirectToAction("Index", "Home");
            }

            var claims = result.Principal?.Identities?.FirstOrDefault()?.Claims;
            if (claims == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("No email received from Google");
                return RedirectToAction("Index", "Home");
            }

            // Find or create user in database
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                // Create new user
                user = new User
                {
                    Email = email,
                    DisplayName = name ?? email.Split('@')[0],
                    GoogleId = googleId,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow,
                    TotalScore = 0,
                    GamesPlayed = 0
                };
                _context.Users.Add(user);
                _logger.LogInformation($"New user created: {email}");
            }
            else
            {
                // Update existing user
                user.LastLoginAt = DateTime.UtcNow;
                user.GoogleId = googleId;
                _logger.LogInformation($"User logged in: {email}");
            }

            await _context.SaveChangesAsync();

            // Create application cookie
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("DisplayName", user.DisplayName)
            };

            var identity = new ClaimsIdentity(userClaims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            });

            // Redirect to game page
            return RedirectToAction("Play", "Game");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google authentication");
            return RedirectToAction("Index", "Home");
        }
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        await HttpContext.SignOutAsync(GoogleDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        var userId = int.Parse(userIdClaim);
        
        var user = await _context.Users
            .Include(u => u.GameSessions)
            .Include(u => u.HighScores)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return NotFound();

        return View(user);
    }
}