using Microsoft.EntityFrameworkCore.Migrations;
using System;
using TaskManagement.Server.Api.Models;


#nullable disable


namespace TaskManagement.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
            name: "Tasks",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
            .Annotation("Sqlite:Autoincrement", true),
                UserId = table.Column<bool>(type: "INTEGER", nullable: true),
                UserName = table.Column<string>(type: "TEXT", nullable: true),
                Title = table.Column<string>(type: "TEXT", nullable: false),
                Description = table.Column<string>(type: "TEXT", nullable: true),
                DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                CreatedAtDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAtDate = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tasks", x => x.Id);
                table.ForeignKey("FK_Tasks", x => x.UserId, "Users");
            });


            migrationBuilder.CreateIndex(
            name: "IX_Tasks_IsCompleted",
            table: "Tasks",
            column: "IsCompleted");

            migrationBuilder.CreateTable(
               name: "Users",
               columns: table => new
               {
                   Id = table.Column<int>(type: "INTEGER", nullable: false)
               .Annotation("Sqlite:Autoincrement", true),
                   Name = table.Column<string>(type: "TEXT", nullable: false),
                   Email = table.Column<string>(type: "TEXT", nullable: true),
                   IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                   CreatedAtDate = table.Column<DateTime>(type: "TEXT", nullable: false),
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Users", x => x.Id);
               });


            migrationBuilder.CreateIndex(
            name: "IX_Users_IsActive",
            table: "Users",
            column: "IsActive");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
            name: "Tasks");
            migrationBuilder.DropTable(
            name: "Users");
        }
    }
}