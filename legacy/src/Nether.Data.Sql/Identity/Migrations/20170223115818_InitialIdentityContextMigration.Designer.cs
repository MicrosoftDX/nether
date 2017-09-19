using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Nether.Data.EntityFramework.Identity;

namespace Nether.Data.Sql.Identity.Migrations
{
    [DbContext(typeof(SqlIdentityContext))]
    [Migration("20170223115818_InitialIdentityContextMigration")]
    partial class InitialIdentityContextMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Nether.Data.EntityFramework.Identity.LoginEntity", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(50);

                    b.Property<string>("ProviderType")
                        .HasMaxLength(50);

                    b.Property<string>("ProviderId")
                        .HasMaxLength(50);

                    b.Property<string>("ProviderData");

                    b.HasKey("UserId", "ProviderType");

                    b.ToTable("Logins");

                    b.HasAnnotation("SqlServer:TableName", "UserLogins");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Identity.UserEntity", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<bool>("IsActive");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("UserId");

                    b.ToTable("Users");

                    b.HasAnnotation("SqlServer:TableName", "Users");
                });

            modelBuilder.Entity("Nether.Data.EntityFramework.Identity.LoginEntity", b =>
                {
                    b.HasOne("Nether.Data.Sql.Identity.UserEntity", "User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
