using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DjProfiles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    StageName = table.Column<string>(type: "TEXT", nullable: false),
                    RealName = table.Column<string>(type: "TEXT", nullable: true),
                    Tagline = table.Column<string>(type: "TEXT", nullable: false),
                    Bio = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    BannerImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Genres = table.Column<string>(type: "TEXT", nullable: false),
                    Equipment = table.Column<string>(type: "TEXT", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "TEXT", nullable: true),
                    Featured = table.Column<bool>(type: "INTEGER", nullable: false),
                    Rating = table.Column<double>(type: "REAL", nullable: false),
                    ReviewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    SocialLinks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DjProfiles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DjProfiles");
        }
    }
}
