using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadMonitoringSystem.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSensorIdFromAlerts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SensorID",
                table: "Alerts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SensorID",
                table: "Alerts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
