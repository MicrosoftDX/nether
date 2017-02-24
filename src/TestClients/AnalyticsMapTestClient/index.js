var map;
var endpointInfo;
var netherClient;

function main() {
    map = new Microsoft.Maps.Map(document.getElementById('myMap'), {
        credentials: 'YOUR_KEY_HERE'
    });

    Microsoft.Maps.Events.addHandler(map, 'click', mapClick);
    netherClient = new NetherClient("http://localhost:5000");
}


function padLeft(nr, n, str){
    return Array(n-String(nr).length+1).join(str||'0')+nr;
}

function mapClick(e) {

    var gameSessionId = $('#gameSessionId').val();

    var d = new Date();

    var clientUtcTime = 
        d.getUTCFullYear() + '-' +
        padLeft((d.getUTCMonth() + 1), 2) + '-' +
        padLeft(d.getUTCDate(), 2) + ' ' +
        padLeft(d.getUTCHours(), 2) + ':' +
        padLeft(d.getUTCMinutes(), 2) + ':' +
        padLeft(d.getUTCSeconds(), 2);

// console.log(e.location.latitude + ', ' + e.location.longitude);
// return;

    console.log('At ' + clientUtcTime + '(UTC) a click recorded at:');
    console.log('  lat : ' + e.location.latitude);
    console.log('  lon: ' + e.location.longitude);
    console.log('  sessionId: ' + gameSessionId);

    netherClient.sendEvent({
            "type": "location",
            "version": "1.0.0",
            "clientUtcTime": clientUtcTime,
            "gameSessionId": gameSessionId,
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
