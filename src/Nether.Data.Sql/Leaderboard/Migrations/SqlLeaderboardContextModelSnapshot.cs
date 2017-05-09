// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Nether.Data.Sql.Leaderboard;

namespace Nether.Data.Sql.Leaderboard.Migrations
{
    [DbContext(typeof(SqlLeaderboardContext))]
    internal partial class SqlLeaderboardContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Nether.Data.Sql.Leaderboard.QueriedGamerScore", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("Ranking");

                    b.Property<int>("Score");

                    b.HasKey("UserId");

                    b.ToTable("Ranks");
                });

            modelBuilder.Entity("Nether.Data.Sql.Leaderboard.SavedGamerScore", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateAchieved");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("Score");

                    b.HasKey("Id");

                    b.HasIndex("DateAchieved", "UserId", "Score");

                    b.ToTable("Scores");

                    b.HasAnnotation("SqlServer:TableName", "Scores");
                });
        }
    }
}
