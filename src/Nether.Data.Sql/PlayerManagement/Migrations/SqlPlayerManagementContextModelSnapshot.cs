// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Nether.Data.Sql.PlayerManagement;

namespace Nether.Data.Sql.PlayerManagement.Migrations
{
    [DbContext(typeof(SqlPlayerManagementContext))]
    internal partial class SqlPlayerManagementContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Nether.Data.Sql.PlayerManagement.PlayerEntity", b =>
                {
                    b.Property<string>("Gamertag")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("Country");

                    b.Property<string>("CustomTag");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Gamertag");

                    b.HasAlternateKey("UserId");

                    b.ToTable("Players");

                    b.HasAnnotation("SqlServer:TableName", "Players");
                });

            modelBuilder.Entity("Nether.Data.Sql.PlayerManagement.PlayerExtendedEntity", b =>
                {
                    b.Property<string>("Gamertag")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("State");

                    b.HasKey("Gamertag");

                    b.ToTable("PlayersExtended");

                    b.HasAnnotation("SqlServer:TableName", "PlayersExtended");
                });
        }
    }
}
