using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Nether.Data.EntityFramework.PlayerManagement;

namespace Nether.Data.Sql.PlayerManagement.Migrations
{
    [DbContext(typeof(SqlPlayerManagementContext))]
    [Migration("20170222204052_InitialPlayerManagementContextMigration")]
    partial class InitialPlayerManagementContextMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Nether.Data.EntityFramework.PlayerManagement.PlayerEntity", b =>
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

            modelBuilder.Entity("Nether.Data.EntityFramework.PlayerManagement.PlayerExtendedEntity", b =>
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
