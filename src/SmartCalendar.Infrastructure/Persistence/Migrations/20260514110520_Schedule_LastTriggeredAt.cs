using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCalendar.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Schedule_LastTriggeredAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastTriggeredAt",
                table: "Schedules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_SceneId",
                table: "Schedules",
                column: "SceneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Scenes_SceneId",
                table: "Schedules",
                column: "SceneId",
                principalTable: "Scenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Scenes_SceneId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_SceneId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "LastTriggeredAt",
                table: "Schedules");
        }
    }
}
