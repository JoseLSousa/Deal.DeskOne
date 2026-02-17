using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deal.DeskOne.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedByToRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Requests",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Requests");
        }
    }
}
