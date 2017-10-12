// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;

public static HttpResponseMessage Run(HttpRequestMessage req, TraceWriter log)
{
    var now = DateTime.UtcNow;

    string msg;

    switch (now.DayOfWeek)
    {
        case DayOfWeek.Monday:
            msg = "It's Monday, don't forget to be aweseome. -Anonymous";
            break;
        case DayOfWeek.Tuesday:
            msg = "Tuesday isn't so bad...It's a sign that I've somehow survived Monday. -Anonymous";
            break;
        case DayOfWeek.Wednesday:
            msg = "Wednesday is like middle finger of the week. -Anonymous";
            break;
        case DayOfWeek.Thursday:
            msg = "Thursday is perhaps the worst day of the week. It's nothing in itself; it just reminds you that the week has been going on too long. -Nicci French";
            break;
        case DayOfWeek.Friday:
            msg = "My boss yelled at me yesterday 'It's the fifth time you've been late to work this week! Do you know what that means!?' I said, 'Probably that it's Friday?'. -Anonymous";
            break;
        case DayOfWeek.Saturday:
            msg = "I didn't always have 14,000 people wanting to hang out with me on a Saturday night. -Taylor Swift";
            break;
        case DayOfWeek.Sunday:
            msg = "I love to go to the zoo. But not on Sunday. I don't like to see the people making fun of the animals, when it should be the other way around. -Ernest Hemingway";
            break;
        default:
            msg = "Somehow, today is a day, that doesn't exists";
            break;
    }

    log.Info($"The following message will be returned: {0}");

    return req.CreateResponse(HttpStatusCode.OK, msg);
}
