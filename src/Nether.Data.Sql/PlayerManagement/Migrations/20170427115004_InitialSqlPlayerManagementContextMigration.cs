// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nether.Data.Sql.PlayerManagement.Migrations
{
    public partial class InitialSqlPlayerManagementContextMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 50, nullable: false),
                    Country = table.Column<string>(nullable: true),
                    CustomTag = table.Column<string>(nullable: true),
                    Gamertag = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.UserId);
                    table.UniqueConstraint("AK_Players_Gamertag", x => x.Gamertag);
                });

            migrationBuilder.CreateTable(
                name: "PlayersExtended",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 50, nullable: false),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayersExtended", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "PlayersExtended");
        }
    }
}
