using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoPulse.Fleet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DomainLayerEntitiesV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LicenseNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    HomeLatitude = table.Column<double>(type: "double precision", nullable: false),
                    HomeLongitude = table.Column<double>(type: "double precision", nullable: false),
                    HomeHubId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hubs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RouteName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    EstimatedDistanceKm = table.Column<double>(type: "double precision", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LicensePlate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MaxSpeedLimit = table.Column<int>(type: "integer", nullable: false),
                    CapacityTonnage = table.Column<double>(type: "double precision", nullable: false),
                    CurrentLatitude = table.Column<double>(type: "double precision", nullable: false),
                    CurrentLongitude = table.Column<double>(type: "double precision", nullable: false),
                    CurrentHubId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouteStops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    StopSequence = table.Column<int>(type: "integer", nullable: false),
                    StopType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    HubId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteStops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteStops_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    DriverId = table.Column<Guid>(type: "uuid", nullable: false),
                    RouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReleasedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CargoType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CargoTonnage = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assignments_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assignments_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParkingSpaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HubId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpaceDesignation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OccupiedByVehicleId = table.Column<Guid>(type: "uuid", nullable: true),
                    OccupiedFromUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OccupiedUntilUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingSpaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParkingSpaces_Hubs_HubId",
                        column: x => x.HubId,
                        principalTable: "Hubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParkingSpaces_Vehicles_OccupiedByVehicleId",
                        column: x => x.OccupiedByVehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentStops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    RouteStopId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstimatedArrivalTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualArrivalTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ScheduledStayDurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    ActualDepartureTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentStops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentStops_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignmentStops_RouteStops_RouteStopId",
                        column: x => x.RouteStopId,
                        principalTable: "RouteStops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_DriverId",
                table: "Assignments",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_RouteId",
                table: "Assignments",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_VehicleId",
                table: "Assignments",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentStops_AssignmentId",
                table: "AssignmentStops",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentStops_RouteStopId",
                table: "AssignmentStops",
                column: "RouteStopId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpaces_HubId",
                table: "ParkingSpaces",
                column: "HubId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpaces_OccupiedByVehicleId",
                table: "ParkingSpaces",
                column: "OccupiedByVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_RouteId",
                table: "RouteStops",
                column: "RouteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentStops");

            migrationBuilder.DropTable(
                name: "ParkingSpaces");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "RouteStops");

            migrationBuilder.DropTable(
                name: "Hubs");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Routes");
        }
    }
}
