// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Newtonsoft.Json;

namespace Nether.Web.Features.Identity.Models.UserLoginAdmin
{
    public class UserLoginModel
    {
        public string ProviderType { get; set; }
        public string ProviderId { get; set; }
        // Don't include ProviderData as that may be sensitive (e.g. password hash!)
        [JsonProperty(PropertyName = "_link")]
        public string _Link { get; set; }
    }
}
