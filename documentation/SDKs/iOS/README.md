# iOS SDK for Nether

## Work in progress

There is currently no iOS SDK for Nether. We have it on the roadmap, but no planned dates set yet.  We welcome contributions from the community, so please help us if you have time.

If you want to integrate your game with Nether at this moment you can do so by manually calling the REST API.

(The below instructions are only to be seen as design discussions since the SDK doesn't exist yet.)

## Downloading the Nether SDK for iOS
(work in progress)

Nether SDK for iOS is delivered as source code from https://github.com/microsoftdx/nether or as a CocoaPod

### Installing the Nether Pod
(work in progress)

This area assumes you are have knowledge on how to use (CocoaPods)[https://cocoapods.org/].

Create a Podfile or add to your existing one in the Xcode project directory:

```
platform :ios, '8.0'
use_frameworks!

target 'MyNetherGame' do
  pod 'Nether', '~> 1.0'
end
```

Download and install your dependencies by executing

```
$ pod install
```

## Configuring the client
(work in progress)

Swift
```
// Pseudocode

#import ...

nether = new NetherClient("http://yoururl.com")
nether.Config.Authentication = AuthenticationProviders.Anonymous // default value, can be omitted
nether.Config.SendAutomaticHeartbeats = true // default value, can be omitted
```

Objective-C
```
// Pseudocode

#import ...

nether = new NetherClient("http://yoururl.com")
nether.Config.Authentication = AuthenticationProviders.Anonymous // default value, can be omitted
nether.Config.SendAutomaticHeartbeats = true // default value, can be omitted
```

## Initializing a connection to Nether
(Describe why, when and how)

Swift
```
// Pseudocode

nether.Connect()
```

Objective-C
```
// Pseudocode

nether.Connect()
```
## Integrating with Facebook Authentication
(Describe why, when and how)

```
nether.AuthenticateWithFacebook(...)
```

## Sending Game Events for Analytic Evaluation
(Describe what sending Game Events will give you as a game developer)

### Send Client Device Capabilities
(Describe why, when and how)

```
nether.Analytics.SendClientDeviceCapabilities(...optionstosend...)
```

### Send Generic Game Event
(Describe why, when and how)

```
var event = { "type":"GameStartEvent", "version":"1.0.0", ...}
nether.Analytics.SendGenericGameEvent(event)
```
