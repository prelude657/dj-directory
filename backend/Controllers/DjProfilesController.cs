using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    public class UpdateDjProfileDto
    {
        public string StageName { get; set; } = string.Empty;
        public string Tagline { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public List<string> Genres { get; set; } = new();
        public List<string> Equipment { get; set; } = new();

        // Fix: Use backend.Models.SocialLinks instead of List<string>
        public SocialLinks? SocialLinks { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class DjProfilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DjProfilesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DjProfile>>> GetProfiles()
        {
            return await _context.DjProfiles.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DjProfile>> GetProfile(int id)
        {
            var profile = await _context.DjProfiles.FindAsync(id);

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<DjProfile>> GetProfileByUserId(string userId)
        {
            var profile = await _context.DjProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        [HttpGet("my-profile")]
        [Authorize]
        public async Task<ActionResult<DjProfile>> GetMyProfile()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var profile = await _context.DjProfiles
                .FirstOrDefaultAsync(p => p.UserId == currentUserId);

            if (profile == null)
            {
                return NotFound("Profile not found for current user.");
            }

            return Ok(profile);
        }

        [HttpPut("my-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateDjProfileDto dto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var profile = await _context.DjProfiles
                .FirstOrDefaultAsync(p => p.UserId == currentUserId);

            if (profile == null)
            {
                return NotFound("Profile not found for current user.");
            }

            profile.StageName = dto.StageName;
            profile.Tagline = dto.Tagline;
            profile.Bio = dto.Bio;
            profile.City = dto.City;
            profile.Country = dto.Country;
            profile.Location = dto.Location;
            profile.ProfileImageUrl = dto.ProfileImageUrl;
            profile.Genres = dto.Genres;
            profile.Equipment = dto.Equipment;
            profile.SocialLinks = dto.SocialLinks;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully." });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<DjProfile>> CreateProfile([FromBody] DjProfile profile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            profile.UserId = userId;
            _context.DjProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] DjProfile updatedProfile)
        {
            if (id != updatedProfile.Id)
            {
                return BadRequest("ID mismatch.");
            }

            var existingProfile = await _context.DjProfiles.FindAsync(id);
            if (existingProfile == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingProfile.UserId != currentUserId)
            {
                return Forbid();
            }

            existingProfile.StageName = updatedProfile.StageName;
            existingProfile.Tagline = updatedProfile.Tagline;
            existingProfile.Bio = updatedProfile.Bio;
            existingProfile.City = updatedProfile.City;
            existingProfile.Country = updatedProfile.Country;
            existingProfile.Location = updatedProfile.Location;
            existingProfile.ProfileImageUrl = updatedProfile.ProfileImageUrl;
            existingProfile.Featured = updatedProfile.Featured;
            existingProfile.Rating = updatedProfile.Rating;
            existingProfile.ReviewCount = updatedProfile.ReviewCount;
            existingProfile.Genres = updatedProfile.Genres;
            existingProfile.Equipment = updatedProfile.Equipment;
            existingProfile.SocialLinks = updatedProfile.SocialLinks;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            var profile = await _context.DjProfiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (profile.UserId != currentUserId)
            {
                return Forbid();
            }

            _context.DjProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}