using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadMonitoringSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAlertModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "DataValue",
                table: "SensorData",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "DataValue",
                table: "SensorData",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
