using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadMonitoringSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRoleToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoadSections",
                columns: table => new
                {
                    RoadSectionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadSections", x => x.RoadSectionID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    SensorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadSectionID = table.Column<int>(type: "int", nullable: false),
                    SensorType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InstallationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.SensorID);
                    table.ForeignKey(
                        name: "FK_Sensors_RoadSections_RoadSectionID",
                        column: x => x.RoadSectionID,
                        principalTable: "RoadSections",
                        principalColumn: "RoadSectionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    AlertID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadSectionID = table.Column<int>(type: "int", nullable: false),
                    AlertType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    SensorID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.AlertID);
                    table.ForeignKey(
                        name: "FK_Alerts_RoadSections_RoadSectionID",
                        column: x => x.RoadSectionID,
                        principalTable: "RoadSections",
                        principalColumn: "RoadSectionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Alerts_Sensors_SensorID",
                        column: x => x.SensorID,
                        principalTable: "Sensors",
                        principalColumn: "SensorID");
                });

            migrationBuilder.CreateTable(
                name: "SensorData",
                columns: table => new
                {
                    SensorDataID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SensorID = table.Column<int>(type: "int", nullable: false),
                    Parameter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DataValue = table.Column<float>(type: "real", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorData", x => x.SensorDataID);
                    table.ForeignKey(
                        name: "FK_SensorData_Sensors_SensorID",
                        column: x => x.SensorID,
                        principalTable: "Sensors",
                        principalColumn: "SensorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_RoadSectionID",
                table: "Alerts",
                column: "RoadSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_SensorID",
                table: "Alerts",
                column: "SensorID");

            migrationBuilder.CreateIndex(
                name: "IX_SensorData_SensorID",
                table: "SensorData",
                column: "SensorID");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_RoadSectionID",
                table: "Sensors",
                column: "RoadSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "SensorData");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "RoadSections");
        }
    }
}
