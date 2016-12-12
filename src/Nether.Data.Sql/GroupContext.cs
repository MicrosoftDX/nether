// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nether.Data.PlayerManagement;

namespace Nether.Data.Sql.PlayerManagement
{
    public class GroupContext : DbContext
    {
        private readonly string _connectionString;
        private readonly string _table;

        public DbSet<GroupEntity> Groups { get; set; }

        public GroupContext(string connectionString, string table)
        {
            _connectionString = connectionString;
            _table = table;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<GroupEntity>()
            .Property(g => g.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<GroupEntity>().ForSqlServerToTable(_table)
                .HasKey(g => g.Name);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_connectionString);
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            return await Groups.Select(g => new Group
            {
                Name = g.Name,
                CustomType = g.CustomType,
                Description = g.Description
            }).ToListAsync();
        }

        public async Task<byte[]> GetGroupImageAsync(string name)
        {
            var group = await Groups.SingleAsync(g => g.Name.Equals(name));
            return group.Image;
        }

        public async Task<Group> GetGroupDetailsAsync(string groupname)
        {
            var group = await Groups.SingleAsync(g => g.Name.Equals(groupname));
            return new Group
            {
                Name = group.Name,
                CustomType = group.CustomType,
                Description = group.Description
            };
        }

        public async Task SaveGroupAsync(Group group)
        {
            // add new group only if it does not exist
            GroupEntity entity = await Groups.FindAsync(group.Name);
            if (entity == null)
            {
                await Groups.AddAsync(new GroupEntity
                {
                    Name = group.Name,
                    CustomType = group.CustomType,
                    Description = group.Description
                });
                await SaveChangesAsync();
            }
        }

        public async Task UploadGroupImageAsync(string groupname, byte[] image)
        {
            var group = await Groups.SingleAsync(g => g.Name.Equals(groupname));
            group.Image = image;
            Groups.Update(group);
            await SaveChangesAsync();
        }
    }

    public class GroupEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CustomType { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
    }
}
