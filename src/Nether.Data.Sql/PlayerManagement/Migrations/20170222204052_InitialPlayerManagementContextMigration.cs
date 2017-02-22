// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nether.Data.Sql.PlayerManagement.Migrations
{
    public partial class InitialPlayerManagementContextMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    CustomType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Image = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Gamertag = table.Column<string>(maxLength: 50, nullable: false),
                    Country = table.Column<string>(nullable: true),
                    CustomTag = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Gamertag);
                    table.UniqueConstraint("AK_Players_UserId", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "PlayersExtended",
                columns: table => new
                {
                    Gamertag = table.Column<string>(maxLength: 50, nullable: false),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayersExtended", x => x.Gamertag);
                });

            migrationBuilder.CreateTable(
                name: "PlayerGroups",
                columns: table => new
                {
                    GroupName = table.Column<string>(nullable: false),
                    Gamertag = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGroups", x => new { x.GroupName, x.Gamertag });
                    table.ForeignKey(
                        name: "FK_PlayerGroups_Players_Gamertag",
                        column: x => x.Gamertag,
                        principalTable: "Players",
                        principalColumn: "Gamertag",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerGroups_Groups_GroupName",
                        column: x => x.GroupName,
                        principalTable: "Groups",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGroups_Gamertag",
                table: "PlayerGroups",
                column: "Gamertag");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayersExtended");

            migrationBuilder.DropTable(
                name: "PlayerGroups");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
