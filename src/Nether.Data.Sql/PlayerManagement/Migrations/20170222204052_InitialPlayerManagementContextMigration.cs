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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayersExtended");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
