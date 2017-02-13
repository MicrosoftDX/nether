// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations;

namespace Nether.Web.Features.IdentityUi
{
    public class RegisterViewModel
    {
        public string ReturnUrl { get; set; }
        [Required]
        public string Gamertag { get; set; }
    }
}