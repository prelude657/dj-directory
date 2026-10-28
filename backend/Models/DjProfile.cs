using System;
using System.Collections.Generic;

namespace backend.Models
{
    public class DjProfile
    {
        public int Id { get; set; }

        // ASP.NET Core Identity User GUID string
        public string UserId { get; set; } = string.Empty;

        public string StageName { get; set; } = string.Empty;
        public string Tagline { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;

        public bool Featured { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }

        public List<string> Genres { get; set; } = new();
        public List<string> Equipment { get; set; } = new();

        public SocialLinks? SocialLinks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class SocialLinks
    {
        public string? Instagram { get; set; }
        public string? Youtube { get; set; }
        public string? Soundcloud { get; set; }
        public string? Spotify { get; set; }
    }
}