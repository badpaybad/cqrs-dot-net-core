using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IotHub.DbMigration.Migrations
{
    public partial class initIothubDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommandEventStorage",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DataType = table.Column<string>(nullable: true),
                    DataJson = table.Column<string>(nullable: true),
                    IsCommand = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandEventStorage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommandEventStorageHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CommandEventId = table.Column<Guid>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandEventStorageHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventSourcingDescription",
                columns: table => new
                {
                    EsdId = table.Column<Guid>(nullable: false),
                    AggregateId = table.Column<Guid>(nullable: false),
                    Version = table.Column<long>(nullable: false),
                    AggregateType = table.Column<string>(nullable: true),
                    EventType = table.Column<string>(nullable: true),
                    EventData = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSourcingDescription", x => x.EsdId);
                });

            migrationBuilder.CreateTable(
                name: "SampleEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Version = table.Column<string>(nullable: true),
                    Published = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Fullname = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Actived = table.Column<bool>(nullable: false),
                    TokenSession = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommandEventStorage");

            migrationBuilder.DropTable(
                name: "CommandEventStorageHistory");

            migrationBuilder.DropTable(
                name: "EventSourcingDescription");

            migrationBuilder.DropTable(
                name: "SampleEntities");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
