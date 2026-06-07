using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspireApp.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class AddDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Date2",
                table: "WeatherForecasts",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date2",
                table: "WeatherForecasts");
        }
    }
}
