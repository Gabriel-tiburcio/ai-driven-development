using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllStay.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestsAndKidsActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuestRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuestName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuestRequests_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KidsActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgeRange = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Schedule = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KidsActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KidsActivities_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KidsEnrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KidsActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChildAge = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GuardianRoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GuardianName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KidsEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KidsEnrollments_KidsActivities_KidsActivityId",
                        column: x => x.KidsActivityId,
                        principalTable: "KidsActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuestRequests_HotelId",
                table: "GuestRequests",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_KidsActivities_HotelId",
                table: "KidsActivities",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_KidsEnrollments_KidsActivityId",
                table: "KidsEnrollments",
                column: "KidsActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuestRequests");

            migrationBuilder.DropTable(
                name: "KidsEnrollments");

            migrationBuilder.DropTable(
                name: "KidsActivities");
        }
    }
}
