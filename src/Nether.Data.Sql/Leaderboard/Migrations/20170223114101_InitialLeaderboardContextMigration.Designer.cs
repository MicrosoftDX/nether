using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Nether.Data.EntityFramework.Leaderboard;

namespace Nether.Data.Sql.Leaderboard.Migrations
{
    [DbContext(typeof(SqlLeaderboardContext))]
    [Migration("20170223114101_InitialLeaderboardContextMigration")]
    partial class InitialLeaderboardContextMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Nether.Data.EntityFramework.Leaderboard.QueriedGamerScore", b =>
                {
                    b.Property<string>("Gamertag")
                        .ValueGeneratedOnAdd();                    

                    b.Property<long>("Ranking");

                    b.Property<int>("Score");

                    b.HasKey("Gamertag");

                    b.ToTable("Ranks");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Leaderboard.SavedGamerScore", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateAchieved");

                    b.Property<string>("Gamertag")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("Score");

                    b.HasKey("Id");

                    b.HasIndex("DateAchieved", "Gamertag", "Score");

                    b.ToTable("Scores");

                    b.HasAnnotation("SqlServer:TableName", "Scores");
                });
        }
    }
}
