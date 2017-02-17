var map;
var endpointInfo;
var netherClient;

function loadMapScenario() {
    map = new Microsoft.Maps.Map(document.getElementById('myMap'), {
        credentials: 'PUT_YOUR_BING_MAPS_KEY_HERE'
    });

    Microsoft.Maps.Events.addHandler(map, 'click', mapClick);
    netherClient = new NetherClient("http://localhost:5000");
}

function mapClick(e) {

    console.log('Click recorded at:');
    console.log('  Longitude: ' + e.location.longitude);
    console.log('  Latitude : ' + e.location.latitude);

    netherClient.sendEvent(
        {
            "type": "location",
            "version": "1.0.0",
            "clientUtcTime": "2017-02-17 14:00:00",
            "gameSessionId": "session1",
            "longitude": e.location.longitude,
            "latitude": e.location.latitude,
            "geohash": ""
        }, function(){
            console.log("done!");
        });
}
