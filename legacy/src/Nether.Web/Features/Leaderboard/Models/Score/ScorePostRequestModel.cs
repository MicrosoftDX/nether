// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace Nether.Web.Features.Leaderboard.Models.Score
{
    public class ScorePostRequestModel
    {
        /// <summary>
        /// Country code
        /// </summary>
        [Required]
        public string Country { get; set; }

        /// <summary>
        /// Achieved score
        /// </summary>
        [Required]
        public int Score { get; set; }
    }
}
