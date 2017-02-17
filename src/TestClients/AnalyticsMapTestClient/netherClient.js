function NetherClient(netherBaseUrl) {

    this.netherBaseUrl = netherBaseUrl;

    this.authorization = "";
    this.contentType = "";
    this.httpVerb = "";
    this.url = "";
    this.validUntil = new Date(-8640000000000000);

    this.sendEvent = function (gameEvent, success = null) {
        var self = this;
        var now = new Date();

        if (this.validUntil < now) {
            this._updateEndpointInfo(function () {
                self._sendToEventHub(gameEvent, success)
            });
        }
        else {
            self._sendToEventHub(gameEvent, success);
        }
    }

    this._updateEndpointInfo = function (success) {
        var self = this;

        $.getJSON(this.netherBaseUrl + "/api/endpoint", function (info) {
            console.log('Nether Analytics Endpoint Information Recieved and Updated');
            console.log(info);

            self.authorization = info.authorization;
            self.contentType = info.contentType;
            self.httpVerb = info.httpVerb;
            self.url = info.url;
            self.validUntil = new Date(info.validUntilUtc);

            success();
        });
    }

    this._sendToEventHub = function (gameEvent, success) {
        var self = this;

        var json = JSON.stringify(gameEvent);
        console.log('Sending event:');
        console.log(json);

        $.ajax({
            type: self.httpVerb,
            url: self.url,
            data: json,
            dataType: self.contentType,

            beforeSend: function (request) {
                request.setRequestHeader("ContentType", self.contentType);
                request.setRequestHeader("Authorization", self.authorization);
            },
            success: function () {
                if (success != null)
                    success();
            }
        });
    }
}
