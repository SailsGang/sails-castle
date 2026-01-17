using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SailsEnergy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    PerformedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Details = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PerformedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LicensePlate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Manufacturer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BatteryCapacityKwh = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnergyLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GangId = table.Column<Guid>(type: "uuid", nullable: false),
                    GangCarId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoggedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnergyKwh = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    ChargingDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    AppliedTariffId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnergyLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gang_cars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GangId = table.Column<Guid>(type: "uuid", nullable: false),
                    CarId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gang_cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gang_members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GangId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gang_members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gangs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gangs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Periods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GangId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ClosedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ClosedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Periods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tariffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GangId = table.Column<Guid>(type: "uuid", nullable: false),
                    PricePerKwh = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    EffectiveFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SetByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tariffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdentityId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AvatarUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_EntityId",
                table: "audit_logs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_EntityType_EntityId",
                table: "audit_logs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_PerformedAt",
                table: "audit_logs",
                column: "PerformedAt");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_PerformedByUserId",
                table: "audit_logs",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_cars_OwnerId",
                table: "cars",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_EnergyLogs_GangId",
                table: "EnergyLogs",
                column: "GangId");

            migrationBuilder.CreateIndex(
                name: "IX_EnergyLogs_GangId_PeriodId",
                table: "EnergyLogs",
                columns: new[] { "GangId", "PeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_EnergyLogs_LoggedByUserId",
                table: "EnergyLogs",
                column: "LoggedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EnergyLogs_PeriodId",
                table: "EnergyLogs",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_gang_cars_CarId",
                table: "gang_cars",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_gang_cars_GangId",
                table: "gang_cars",
                column: "GangId");

            migrationBuilder.CreateIndex(
                name: "IX_gang_cars_GangId_CarId",
                table: "gang_cars",
                columns: new[] { "GangId", "CarId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_gang_members_GangId",
                table: "gang_members",
                column: "GangId");

            migrationBuilder.CreateIndex(
                name: "IX_gang_members_GangId_UserId",
                table: "gang_members",
                columns: new[] { "GangId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_gang_members_UserId",
                table: "gang_members",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_gangs_OwnerId",
                table: "gangs",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Periods_GangId",
                table: "Periods",
                column: "GangId");

            migrationBuilder.CreateIndex(
                name: "IX_Periods_GangId_Status",
                table: "Periods",
                columns: new[] { "GangId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Tariffs_GangId",
                table: "Tariffs",
                column: "GangId");

            migrationBuilder.CreateIndex(
                name: "IX_Tariffs_GangId_EffectiveFrom",
                table: "Tariffs",
                columns: new[] { "GangId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_IdentityId",
                table: "user_profiles",
                column: "IdentityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "cars");

            migrationBuilder.DropTable(
                name: "EnergyLogs");

            migrationBuilder.DropTable(
                name: "gang_cars");

            migrationBuilder.DropTable(
                name: "gang_members");

            migrationBuilder.DropTable(
                name: "gangs");

            migrationBuilder.DropTable(
                name: "Periods");

            migrationBuilder.DropTable(
                name: "Tariffs");

            migrationBuilder.DropTable(
                name: "user_profiles");
        }
    }
}
