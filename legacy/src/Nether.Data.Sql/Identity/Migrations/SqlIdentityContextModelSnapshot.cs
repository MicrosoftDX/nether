// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Nether.Data.Sql.Identity;

namespace Nether.Data.Sql.Identity.Migrations
{
    [DbContext(typeof(SqlIdentityContext))]
    internal partial class SqlIdentityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Nether.Data.Sql.Identity.LoginEntity", b =>
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

            modelBuilder.Entity("Nether.Data.Sql.Identity.UserEntity", b =>
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

            modelBuilder.Entity("Nether.Data.Sql.Identity.LoginEntity", b =>
                {
                    b.HasOne("Nether.Data.Sql.Identity.UserEntity", "User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
