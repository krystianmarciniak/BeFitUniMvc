using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeFitUniMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddStartEndAndUserIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "TrainingSessions",
                newName: "StartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "TrainingSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "PerformedExercises",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PerformedExercises",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PerformedExercises");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "TrainingSessions",
                newName: "Date");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "PerformedExercises",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
