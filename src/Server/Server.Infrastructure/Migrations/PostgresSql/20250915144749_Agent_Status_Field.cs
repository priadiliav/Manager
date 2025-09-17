using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Agent_Status_Field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSynchronized",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "LastSynchronizedAt",
                table: "Agents");

            migrationBuilder.RenameColumn(
                name: "LastUnsynchronizedAt",
                table: "Agents",
                newName: "LastStatusChangeAt");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Agents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Agents");

            migrationBuilder.RenameColumn(
                name: "LastStatusChangeAt",
                table: "Agents",
                newName: "LastUnsynchronizedAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsSynchronized",
                table: "Agents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastSynchronizedAt",
                table: "Agents",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
