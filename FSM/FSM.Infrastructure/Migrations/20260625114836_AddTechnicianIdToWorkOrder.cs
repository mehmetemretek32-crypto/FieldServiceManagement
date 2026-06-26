using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianIdToWorkOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignedTechnicianId",
                table: "WorkOrders",
                newName: "TechnicianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TechnicianId",
                table: "WorkOrders",
                newName: "AssignedTechnicianId");
        }
    }
}
