var map;
var endpointInfo;
var netherClient;

function loadMapScenario() {
    map = new Microsoft.Maps.Map(document.getElementById('myMap'), {
        credentials: 'YOUR-KEY-HERE'
    });

    Microsoft.Maps.Events.addHandler(map, 'click', mapClick);
    netherClient = new NetherClient("http://localhost:5000");
}

function padLeft(nr, n, str){
    return Array(n-String(nr).length+1).join(str||'0')+nr;
}

function mapClick(e) {

    var d = new Date();

    var clientUtcTime = 
        d.getUTCFullYear() + '-' +
        padLeft((d.getUTCMonth() + 1), 2) + '-' +
        padLeft(d.getUTCDate(), 2) + ' ' +
        padLeft(d.getUTCHours(), 2) + ':' +
        padLeft(d.getUTCMinutes(), 2) + ':' +
        padLeft(d.getUTCSeconds(), 2);


    console.log('At ' + clientUtcTime + '(UTC) a click recorded at:');
    console.log('  lat : ' + e.location.latitude);
    console.log('  lon: ' + e.location.longitude);

    netherClient.sendEvent({
            "type": "location",
            "version": "1.0.0",
            "clientUtcTime": clientUtcTime,
            "gameSessionId": "sessionId123",
            "lat": e.location.latitude,
            "lon": e.location.longitude,
            "properties": {
                "testProp1": "true",
                "testProp2": "sthlm"
            }
        }, function(){
            console.log("done!");
        });
}
