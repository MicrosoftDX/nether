using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Nether.Data.Sql.PlayerManagement;

namespace Nether.Data.Sql.PlayerManagement.Migrations
{
    [DbContext(typeof(SqlPlayerManagementContext))]
    partial class SqlPlayerManagementContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Nether.Data.Sql.PlayerManagement.GroupEntity", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CustomType");

                    b.Property<string>("Description");

                    b.Property<byte[]>("Image");

                    b.HasKey("Name");

                    b.ToTable("Groups");

                    b.HasAnnotation("SqlServer:TableName", "Groups");
                });

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

            modelBuilder.Entity("Nether.Data.Sql.PlayerManagement.PlayerGroupEntity", b =>
                {
                    b.Property<string>("GroupName");

                    b.Property<string>("Gamertag")
                        .HasMaxLength(50);

                    b.HasKey("GroupName", "Gamertag");

                    b.HasIndex("Gamertag");

                    b.ToTable("PlayerGroups");

                    b.HasAnnotation("SqlServer:TableName", "PlayerGroups");
                });

            modelBuilder.Entity("Nether.Data.Sql.PlayerManagement.PlayerGroupEntity", b =>
                {
                    b.HasOne("Nether.Data.Sql.PlayerManagement.PlayerEntity", "Player")
                        .WithMany("PlayerGroups")
                        .HasForeignKey("Gamertag")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Nether.Data.Sql.PlayerManagement.GroupEntity", "Group")
                        .WithMany("PlayerGroups")
                        .HasForeignKey("GroupName")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
