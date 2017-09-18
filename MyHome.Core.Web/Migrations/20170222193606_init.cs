using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHome.Core.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Elements",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ExternalId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ElementItemId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Group = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElementItems",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AllowSchedule = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ElementId = table.Column<string>(nullable: true),
                    ExternalId = table.Column<string>(nullable: true),
                    IsEnum = table.Column<bool>(nullable: false),
                    ModeId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NotCollectChanges = table.Column<bool>(nullable: false),
                    RefreshTime = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementItems_Elements_ElementId",
                        column: x => x.ElementId,
                        principalTable: "Elements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElementItems_Schedules_ModeId",
                        column: x => x.ModeId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElementItemEnums",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ElementItemId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementItemEnums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementItemEnums_ElementItems_ElementItemId",
                        column: x => x.ElementItemId,
                        principalTable: "ElementItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElementItemValues",
                columns: table => new
                {
                    DateTime = table.Column<DateTime>(nullable: false),
                    ElementItemId = table.Column<string>(nullable: false),
                    RawValue = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: false),
                    ValueId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementItemValues", x => new { x.DateTime, x.ElementItemId });
                    table.ForeignKey(
                        name: "FK_ElementItemValues_ElementItems_ElementItemId",
                        column: x => x.ElementItemId,
                        principalTable: "ElementItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElementItemValues_ElementItemEnums_ValueId",
                        column: x => x.ValueId,
                        principalTable: "ElementItemEnums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleHours",
                columns: table => new
                {
                    ScheduleId = table.Column<string>(nullable: false),
                    DayOfWeek = table.Column<int>(nullable: false),
                    Hour = table.Column<int>(nullable: false),
                    RawValue = table.Column<string>(nullable: true),
                    ValueId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleHours", x => new { x.ScheduleId, x.DayOfWeek, x.Hour });
                    table.ForeignKey(
                        name: "FK_ScheduleHours_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleHours_ElementItemEnums_ValueId",
                        column: x => x.ValueId,
                        principalTable: "ElementItemEnums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElementItemEnums_ElementItemId",
                table: "ElementItemEnums",
                column: "ElementItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementItems_ElementId",
                table: "ElementItems",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementItems_ModeId",
                table: "ElementItems",
                column: "ModeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElementItemValues_ElementItemId",
                table: "ElementItemValues",
                column: "ElementItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementItemValues_ValueId",
                table: "ElementItemValues",
                column: "ValueId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleHours_ValueId",
                table: "ScheduleHours",
                column: "ValueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElementItemValues");

            migrationBuilder.DropTable(
                name: "ScheduleHours");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "ElementItemEnums");

            migrationBuilder.DropTable(
                name: "ElementItems");

            migrationBuilder.DropTable(
                name: "Elements");

            migrationBuilder.DropTable(
                name: "Schedules");
        }
    }
}
