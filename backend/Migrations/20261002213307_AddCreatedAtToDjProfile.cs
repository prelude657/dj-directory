using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToDjProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerImageUrl",
                table: "DjProfiles");

            migrationBuilder.DropColumn(
                name: "HourlyRate",
                table: "DjProfiles");

            migrationBuilder.DropColumn(
                name: "RealName",
                table: "DjProfiles");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "DjProfiles",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DjProfiles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DjProfiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DjProfiles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DjProfiles");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "DjProfiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "BannerImageUrl",
                table: "DjProfiles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HourlyRate",
                table: "DjProfiles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RealName",
                table: "DjProfiles",
                type: "TEXT",
                nullable: true);
        }
    }
}
