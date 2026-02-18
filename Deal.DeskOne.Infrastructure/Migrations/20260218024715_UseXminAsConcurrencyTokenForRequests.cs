using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deal.DeskOne.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseXminAsConcurrencyTokenForRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Requests",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Requests");
        }
    }
}
