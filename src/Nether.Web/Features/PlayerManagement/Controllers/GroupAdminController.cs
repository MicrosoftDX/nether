// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Nether.Data.PlayerManagement;
using Nether.Web.Utilities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using Nether.Web.Features.PlayerManagement.Models.GroupAdmin;

//TO DO: The group and player Image type is not yet implemented. Seperate methods need to be implemented to upload a player or group image
//TODO: Add versioning support

namespace Nether.Web.Features.PlayerManagement
{
    /// <summary>
    /// Player management admin API
    /// </summary>
    [Authorize(Roles = RoleNames.Admin)]
    [Route("admin/groups")]
    public class GroupAdminController : Controller
    {
        private readonly IPlayerManagementStore _store;
        private readonly ILogger _logger;

        public GroupAdminController(IPlayerManagementStore store, ILogger<GroupAdminController> logger)
        {
            _store = store;
            _logger = logger;
        }

        /// <summary>
        /// Get list of all groups. You must be an administrator to perform this action.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GroupListResponseModel))]
        [HttpGet("")]
        public async Task<ActionResult> GetGroupsAsync()
        {
            // Call data store
            var groups = await _store.GetGroupsAsync();

            // Return result
            return Ok(GroupListResponseModel.FromGroups(groups));
        }

        /// <summary>
        /// Gets group by name.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GroupGetResponseModel))]
        [HttpGet("{groupName}", Name = nameof(GetGroup))]
        public async Task<ActionResult> GetGroup(string groupName)
        {
            // Call data store
            var group = await _store.GetGroupDetailsAsync(groupName);
            if (group == null)
            {
                return NotFound();
            }

            // Format response model
            var resultModel = new GroupGetResponseModel
            {
                Group = group
            };

            // Return result
            return Ok(resultModel);
        }

        /// <summary>
        /// Creates or updates the speicifed group
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="group"></param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [HttpPut("{groupName}")]
        public async Task<ActionResult> PutGroup([FromRoute] string groupName, [FromBody]GroupPutRequestModel group)
        {
            // Update group
            await _store.SaveGroupAsync(
                    new Group
                    {
                        Name = groupName,
                        CustomType = group.CustomType,
                        Description = group.Description,
                        Members = group.Members
                    }
                );

            // Return result
            return NoContent();
        }

        /// <summary>
        /// Gets the members of the group as player objects.
        /// </summary>
        /// <param name="groupName">Name of the group</param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GroupMemberResponseModel))]
        [HttpGet("{groupName}/players")]
        public async Task<ActionResult> GetGroupPlayers(string groupName)
        {
            // Call data store
            List<string> gamertags = await _store.GetGroupPlayersAsync(groupName);

            // Format response model
            var resultModel = new GroupMemberResponseModel
            {
                Gamertags = gamertags
            };

            // Return result
            return Ok(resultModel);
        }
    }
}
