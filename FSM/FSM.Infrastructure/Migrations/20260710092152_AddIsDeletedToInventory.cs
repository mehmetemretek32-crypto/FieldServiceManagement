using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InventoryItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InventoryItems");
        }
    }
}
