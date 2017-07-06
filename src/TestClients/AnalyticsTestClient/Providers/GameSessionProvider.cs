// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Nether.Test.ConsoleClient
{
    internal class GameSessionProvider : IGameSessionProvider
    {
        public string GetGameSession()
        {
            return RandomEx.GetUniqueShortId();
        }
    }
}
