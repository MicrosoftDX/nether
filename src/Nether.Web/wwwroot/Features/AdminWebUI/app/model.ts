// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

export class Player {
    userId: string;
    gamertag: string;
    country: string;
    customTag: string;
}

export class Group {
    name: string;
    customType: string;
    description: string;
}

export class LeaderboardScore {
    gamertag: string;
    score: number;
}