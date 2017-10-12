# Nether SDK for Android #

Current features:
* User authentication (OAuth)
* Nether APIs:
  * Post score
  * Get leaderboard by name

What's not here currently:
* The Nether client package is only available in this project and is bundled
  with the sample - if you want to take the code into use in your own project,
  you have to copy the source files
* The HTTP operations are not managed i.e. there's no queue for the requests

## Getting started ##

### Permissions ###

The Nether client code requires only networking permission. If you have your
own app for the Nether client, make sure you have the permission in your
`AndroidManifest.xml` file:

```xml
<uses-permission android:name="android.permission.INTERNET" />
```

See the [AndroidManifest.xml](/src/SDKs/Android/app/src/main/AndroidManifest.xml)
file of the sample.

### Dependencies ###

The Nether client uses [OkHttp](http://square.github.io/okhttp/). You can add it
to your project using [Gradle](https://gradle.org/). In your
`build.gradle` file add:

```
compile 'com.squareup.okhttp3:okhttp:3.8.0'
```

See the [build.gradle](/src/SDKs/Android/app/build.gradle) file of the sample.

### The sample ###

This project comes with a sample for testing the Nether client features.
To open the project in [Intellij IDEA](https://www.jetbrains.com/idea/), select
**Open** or **File -> Open**, browse to `/src/SDKs/Android` (see the image
below) and click **OK**.

![Opening the project in IntelliJ IDEA](/src/SDKs/Android/doc/opening_project_in_intellij.png)

### Implementation ###

The Nether client code is in
[/src/SDKs/Android/app/src/main/java/org/microsoftdx/netherclient](/src/SDKs/Android/app/src/main/java/org/microsoftdx/netherclient)

The classes intended to be used by the application (i.e. the main interfaces)
are:
* [NetherClient](/src/SDKs/Android/app/src/main/java/org/microsoftdx/netherclient/NetherClient.java) and
* [NetherApiCallResponse](/src/SDKs/Android/app/src/main/java/org/microsoftdx/netherclient/NetherApiCallResponse.java)

Here's an example how to authenticate a user and do a Nether API call:

```java
NetherClient netherClient = new NetherClient(
        /* listener */ this,
        "https://netherweb0123456789abc.azurewebsites.net",
        "myNetherClientId",
        "myNetherClientSecret");

netherClient.login("myPlayerId", "myPlayerPassword");

...

netherClient.postScoreForCurrentPlayer("Finland", 1000000);
```

The results of the API calls are delivered via callbacks - see the `Listener`
interface in
[NetherClient.java](/src/SDKs/Android/app/src/main/java/org/microsoftdx/netherclient/NetherClient.java).
