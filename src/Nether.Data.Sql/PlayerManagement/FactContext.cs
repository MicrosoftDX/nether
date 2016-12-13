// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using System;
using Nether.Data.PlayerManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Nether.Data.Sql.PlayerManagement
{
    public class FactContext : DbContext
    {
        private readonly string _connectionString;
        private readonly string _table;

        private static readonly string s_groupPlayersSql = "select * from {0} where GroupId = '{1}'";
        private static readonly string s_playerGroupsSql = "select * from {0} where PlayerId = '{1}'";

        public DbSet<FactEntity> PlayerManagementFact { get; set; }

        public FactContext(string connectionString, string table)
        {
            _connectionString = connectionString;
            _table = table;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FactEntity>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();

            builder.Entity<FactEntity>().ForSqlServerToTable(_table);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_connectionString);
        }

        public async Task AddPlayerToGroupAsync(Group group, string playerId)
        {
            await PlayerManagementFact.AddAsync(new FactEntity
            {
                PlayerId = playerId,
                GroupId = group.Name
            });
            await SaveChangesAsync();
        }

        public async Task<List<string>> getGroupPlayersAsync(string groupname)
        {
            string sql = String.Format(s_groupPlayersSql, _table, groupname);
            var players = await PlayerManagementFact.FromSql(sql).ToListAsync();
            return players.Select(p => p.PlayerId).ToList();
        }

        public async Task<List<string>> GetPlayerGroupsAsync(string playerId)
        {
            string sql = String.Format(s_playerGroupsSql, _table, playerId);
            var groups = await PlayerManagementFact.FromSql(sql).ToListAsync();
            return groups.Select(g => g.GroupId).ToList();
        }

        public async Task RemovePlayerFromGroupAsync(string groupName, string playerId)
        {
            List<FactEntity> facts = await PlayerManagementFact.Where(f => (f.GroupId == groupName) && (f.PlayerId == playerId)).ToListAsync();
            RemoveRange(facts);
            await SaveChangesAsync();
        }
    }

    public class FactEntity
    {
        public Guid Id { get; set; }
        public string PlayerId { get; set; }
        public string GroupId { get; set; }
    }
}