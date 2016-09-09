# Post a Score

Gives information on where and how to send game events to the analytics engine in Nether.

## Request

See Common parameters and headers that are used by all requests related to the Analytics Building Block

Method  | Request URI
------- | -----------
GET     | /api/endpoint?api-version={api-version}

### JSON Body

Emtpy Body

## Response

Status code: 200 - Success

### JSON Body


```json
{
  "httpVerb": "POST",
  "url": "https://netheranalytics-ns.servicebus.windows.net/gameevents/messages",
  "contentType": "application/json",
  "authorization": "SharedAccessSignature sr=https%3A%2F%2Fnetheranalytics-ns.servicebus.windows.net%2Fgameevents%2Fmessages&sig=Fs6DMqsKqfLokZ%2FAMVJ2GGFsVnPt7p9XOe1J2sfhL28%3D&se=1473518062&skn=test",
  "validUntilUtc": "2016-09-10T14:34:22.3424851Z"
}
```

Element name        | Required  | Type      | Description
------------------- | --------- | --------- | -----------
httpVerb            | Yes       | String    | The HTTP Verb to use when sending game events
url                 | Yes       | String    | The URL to sent the game events to
contentType         | Yes       | String    | Content type to set in the header of the request to the game event endpoint
authorization       | Yes       | String    | Value of the authorization token to set in the header of request to the game event endpoint
validUntilUtc       | Yes       | String    | The Universal Coordinated Time, UTC, until this information is validUntilUtc


