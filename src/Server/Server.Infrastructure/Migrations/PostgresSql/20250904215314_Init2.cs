using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_Hardware_HardwareId",
                table: "Agents");

            migrationBuilder.DropIndex(
                name: "IX_Agents_HardwareId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "HardwareId",
                table: "Agents");

            migrationBuilder.AddColumn<Guid>(
                name: "AgentId",
                table: "Hardware",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Hardware_AgentId",
                table: "Hardware",
                column: "AgentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Hardware_Agents_AgentId",
                table: "Hardware",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hardware_Agents_AgentId",
                table: "Hardware");

            migrationBuilder.DropIndex(
                name: "IX_Hardware_AgentId",
                table: "Hardware");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "Hardware");

            migrationBuilder.AddColumn<long>(
                name: "HardwareId",
                table: "Agents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Agents_HardwareId",
                table: "Agents",
                column: "HardwareId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_Hardware_HardwareId",
                table: "Agents",
                column: "HardwareId",
                principalTable: "Hardware",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
