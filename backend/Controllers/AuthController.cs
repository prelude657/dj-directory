using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            AppDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                {
                    return BadRequest(new { message = "Email and password are required." });
                }

                var userExists = await _userManager.FindByEmailAsync(dto.Email);
                if (userExists != null)
                {
                    return BadRequest(new { message = "User with this email already exists." });
                }

                var user = new IdentityUser
                {
                    Email = dto.Email,
                    UserName = dto.Email
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new { message = "Registration failed.", errors });
                }

                // Automatically create a linked DjProfile
                var djProfile = new DjProfile
                {
                    UserId = user.Id,
                    StageName = string.IsNullOrWhiteSpace(dto.StageName)
                        ? dto.Email.Split('@')[0]
                        : dto.StageName,
                    Bio = string.Empty,
                    Location = string.Empty,
                    City = string.Empty,
                    Country = string.Empty,
                    ProfileImageUrl = string.Empty,
                    Genres = new List<string>(),
                    Equipment = new List<string>(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.DjProfiles.Add(djProfile);
                await _context.SaveChangesAsync();

                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        stageName = djProfile.StageName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An internal server error occurred.", detail = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                {
                    return BadRequest(new { message = "Email and password are required." });
                }

                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                var djProfile = await _context.DjProfiles
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        stageName = djProfile?.StageName ?? string.Empty
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An internal server error occurred.", detail = ex.Message });
            }
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"] ?? "SuperSecretKeyForDJDirectoryApp2026!";
            var key = Encoding.UTF8.GetBytes(keyString);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"] ?? "DJDirectoryApi",
                Audience = jwtSettings["Audience"] ?? "DJDirectoryClient"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}