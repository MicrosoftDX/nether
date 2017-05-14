using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Nether.Data.Sql.PlayerManagement;

namespace Nether.Data.Sql.PlayerManagement.Migrations
{
    [DbContext(typeof(SqlPlayerManagementContext))]
    [Migration("20170427115004_InitialSqlPlayerManagementContextMigration")]
    partial class InitialSqlPlayerManagementContextMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Nether.Data.EntityFramework.PlayerManagement.PlayerEntity", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("Country");

                    b.Property<string>("CustomTag");

                    b.Property<string>("Gamertag")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("UserId");

                    b.HasAlternateKey("Gamertag");

                    b.ToTable("Players");

                    b.HasAnnotation("SqlServer:TableName", "Players");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.PlayerManagement.PlayerExtendedEntity", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("State");

                    b.HasKey("UserId");

                    b.ToTable("PlayersExtended");

                    b.HasAnnotation("SqlServer:TableName", "PlayersExtended");
                });
        }
    }
}
