using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickey.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedEventModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventDate",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "EventTime",
                table: "Events",
                newName: "EventDateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventDateTime",
                table: "Events",
                newName: "EventTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EventDate",
                table: "Events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
