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
    members: string[];
}

export class LeaderboardScore {
    gamertag: string;
    score: number;
}