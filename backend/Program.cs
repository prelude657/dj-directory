using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context Setup
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=djdirectory.db"));

// Identity Setup
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication Setup
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "SuperSecretKeyForDJDirectoryApp2026!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "DJDirectoryApi",
        ValidAudience = jwtSettings["Audience"] ?? "DJDirectoryClient",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularDev");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    if (!context.DjProfiles.Any())
    {
        context.DjProfiles.AddRange(
            new DjProfile
            {
                UserId = "seed-user-1",
                StageName = "DJ Hendo",
                Tagline = "Open Format & House Specialist",
                Bio = "Bringing high energy to sets worldwide.",
                City = "Salvador",
                Country = "Brazil",
                Location = "Salvador, Brazil",
                ProfileImageUrl = "https://images.unsplash.com/photo-1516450360452-9312f5e86fc7",
                Featured = true,
                Rating = 4.9,
                ReviewCount = 28,
                Genres = new List<string> { "Hip Hop", "Old School R&B", "Baile Charme", "Amapiano" },
                Equipment = new List<string> { "Pioneer DDJ-FLX10", "Pioneer DDJ-FLX4" }
            },
            new DjProfile
            {
                UserId = "seed-user-2",
                StageName = "DJ BeatMaker",
                Tagline = "Afrobeats & Dancehall Enthusiast",
                Bio = "Crafting tropical vibes.",
                City = "Rio de Janeiro",
                Country = "Brazil",
                Location = "Rio de Janeiro, Brazil",
                ProfileImageUrl = "https://images.unsplash.com/photo-1571266028243-3716f02d2d2e",
                Featured = false,
                Rating = 4.7,
                ReviewCount = 15,
                Genres = new List<string> { "Afrobeats", "Dancehall", "Brazilian Funk" },
                Equipment = new List<string> { "Pioneer DDJ-800" }
            }
        );
        context.SaveChanges();
    }
}

app.Run();