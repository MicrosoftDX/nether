var map;
var endpointInfo;
var netherClient;

function loadMapScenario() {
    map = new Microsoft.Maps.Map(document.getElementById('myMap'), {
        credentials: 'AkSg8WKvYlSfF0u9-CSclN2m-v1SU9WkI4iwV9szphXqU6htZpcN4JWY4-pEbjTr'
    });

    Microsoft.Maps.Events.addHandler(map, 'click', mapClick);
    netherClient = new NetherClient("http://localhost:5000");
}

function mapClick(e) {

    console.log(e);
    console.log('Longitude: ' + e.location.longitude);
    console.log('Latitude:  ' + e.location.latitude);

    netherClient.sendEvent({"type":"location"});
}
