using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Technicians",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Technicians");
        }
    }
}
