describe("nether player getPlayer", function() {
  beforeEach(function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(200, "{\"player\": { \"gamerTag\":\"BeautifulStranger27\" } }");
      });
  });

  it("exposes getPlayer", function() {
    expect(nether.player.getPlayer).toBeDefined();
  });

  it("sends an ajax message", function() {    
    nether.player.getPlayer(jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalled();
  });

  it("sends an ajax message using default (HTTP GET) method", function() {    
    nether.player.getPlayer(jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).not.toHaveBeenCalledWith(jasmine.objectContaining({"method":"GET"}));
  });

  it("sends an ajax message to a sensible place", function() {    
    //reinit to blank baseurl
    nether.init({ 
      "netherClientId" : "Kristofer",
      "netherClientSecret" : "Anko",
      "facebookAppId" : "Stuart",
      "netherBaseUrl" : ""
    }, function(){}, function(){}, document);
    nether.player.getPlayer(jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url":"/api/player"}));
  });

  it("sends an ajax message to a configurable place", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.getPlayer(jasmine.createSpy("successCallback"));    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url": exampleConfig.netherBaseUrl + "/api/player"}));
  });

  it("calls the supplied callback", function() {    
    var callback = jasmine.createSpy("successCallback");
    nether.player.getPlayer(callback);
    
    expect(callback).toHaveBeenCalled();
  });


  it("calls the supplied callback with true in success case", function() {    
    var callback = jasmine.createSpy("successCallback");
    nether.player.getPlayer(callback);
    
    expect(callback).toHaveBeenCalledWith(jasmine.objectContaining({ "gamerTag" : "BeautifulStranger27" }));
  });

  it("handles failed http responses gracefully", function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(500, {});
      });

    var genericCallback = jasmine.createSpy("genericCallback");
    nether.player.getPlayer(genericCallback);
    expect(genericCallback).toHaveBeenCalledWith(null); //todo: assert that nullity is correct here

  });
});

describe("nether player setPlayer", function() {
  beforeEach(function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(204, "");
      });
  });

  it("exposes setPlayer", function() {
    expect(nether.player.setPlayer).toBeDefined();
  });

  it("sends an ajax message", function() {    
    nether.player.setPlayer("country", "gamerTag", "customTag", jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalled();
  });

  it("sends an ajax message using HTTP PUT method", function() {    
    nether.player.setPlayer("country", "gamerTag", "customTag", jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"method":"PUT"}));
  });

  it("sends an ajax message to a sensible place", function() {    
    //reinit to blank baseurl
    nether.init({ 
      "netherClientId" : "Kristofer",
      "netherClientSecret" : "Anko",
      "facebookAppId" : "Stuart",
      "netherBaseUrl" : ""
    }, function(){}, function(){}, document);
    nether.player.setPlayer("country", "gamerTag", "customTag", jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url":"/api/player"}));
  });

  it("sends an ajax message to a configurable place", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.setPlayer("country", "gamerTag", "customTag", jasmine.createSpy("successCallback"));    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url": exampleConfig.netherBaseUrl + "/api/player"}));
  });

  it("sends an ajax message containing the country", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.setPlayer("country", "gamerTag", "customTag", jasmine.createSpy("successCallback"));    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"data": 
      jasmine.objectContaining({ "country" : "country"})
    }));
  });

  it("sends an ajax message containing the gamerTag", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.setPlayer("country", "gamerTag", "customTag", jasmine.createSpy("successCallback"));    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"data": 
      jasmine.objectContaining({ "gamerTag" : "gamerTag"})
    }));
  });

  it("sends an ajax message containing the customTag", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.setPlayer("country", "gamerTag", "customTag", jasmine.createSpy("successCallback"));    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"data": 
      jasmine.objectContaining({ "customTag" : "customTag"})
    }));
  });

  it("assigns the gamerTag", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.setPlayer("country", "gamerTag", "customTag", jasmine.createSpy("successCallback"));    
    expect(nether.player.gamertag).toBe("gamerTag");
  });

  it("assigns the country", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.setPlayer("country", "gamerTag", "customTag", jasmine.createSpy("successCallback"));    
    expect(nether.player.country).toBe("country");
  });

  it("calls the supplied callback", function() {    
    var callback = jasmine.createSpy("successCallback");
    nether.player.setPlayer("country", "gamerTag", "customTag", callback);
    
    expect(callback).toHaveBeenCalled();
  });

  it("handles failed http responses with returned Error class", function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(500, {});
      });

    var genericCallback = jasmine.createSpy("genericCallback");
    nether.player.setPlayer("country", "gamerTag", "customTag", genericCallback);
    expect(genericCallback).toHaveBeenCalledWith(jasmine.any(Error)); //todo: assert that nullity is correct here

  });
});

describe("nether player deletePlayer", function() {
  beforeEach(function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(204, "");
      });
  });

  it("exposes deletePlayer", function() {
    expect(nether.player.deletePlayer).toBeDefined();
  });

  it("sends an ajax message", function() {    
    nether.player.deletePlayer(jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalled();
  });

  it("sends an ajax message using HTTP DELETE method", function() {    
    nether.player.deletePlayer(jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"method":"DELETE"}));
  });


  it("sends an ajax message to a sensible place", function() {    
    //reinit to blank baseurl
    nether.init({ 
      "netherClientId" : "Kristofer",
      "netherClientSecret" : "Anko",
      "facebookAppId" : "Stuart",
      "netherBaseUrl" : ""
    }, function(){}, function(){}, document);
    nether.player.deletePlayer(jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url":"/api/player"}));
  });

  it("sends an ajax message to a configurable place", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.deletePlayer(jasmine.createSpy("successCallback"));    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url": exampleConfig.netherBaseUrl + "/api/player"}));
  });

  it("assigns the gamerTag", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.deletePlayer(jasmine.createSpy("successCallback"));    
    expect(nether.player.gamertag).toBe("");
  });

  it("assigns the country", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.deletePlayer(jasmine.createSpy("successCallback"));    
    expect(nether.player.country).toBe("");
  });

  it("calls the supplied callback", function() {    
    var callback = jasmine.createSpy("successCallback");
    nether.player.deletePlayer(callback);
    
    expect(callback).toHaveBeenCalled();
  });

  it("handles failed http responses with returned Error class", function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(500, {});
      });

    var genericCallback = jasmine.createSpy("genericCallback");
    nether.player.deletePlayer(genericCallback);
    expect(genericCallback).toHaveBeenCalledWith(false); //todo: assert that nullity is correct here

  });
});

describe("nether player getPlayerState", function() {
  beforeEach(function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(200, "{ \"gamertag\":\"BeautifulStranger27\", \"state\":{ \"state\":123 } }");
      });
  });

  it("exposes getState", function() {
    expect(nether.player.getState).toBeDefined();
  });

  it("sends an ajax message", function() {    
    nether.player.getState(jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalled();
  });

  it("sends an ajax message using defautl (HTTP DELETE) method", function() {    
    nether.player.deletePlayer(jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).not.toHaveBeenCalledWith(jasmine.objectContaining({"method":"GET"}));
  });

  it("sends an ajax message to a sensible place", function() {    
    //reinit to blank baseurl
    nether.init({ 
      "netherClientId" : "Kristofer",
      "netherClientSecret" : "Anko",
      "facebookAppId" : "Stuart",
      "netherBaseUrl" : ""
    }, function(){}, function(){}, document);
    nether.player.getState(jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url":"/api/player/state"}));
  });

  it("sends an ajax message to a configurable place", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.getState(jasmine.createSpy("successCallback"));    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url": exampleConfig.netherBaseUrl + "/api/player/state"}));
  });

  it("assigns the gamerTag", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.deletePlayer(jasmine.createSpy("successCallback"));    
    expect(nether.player.gamertag).toBe("BeautifulStranger27");
  });

  it("assigns the state", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.deletePlayer(jasmine.createSpy("successCallback"));    
    expect(nether.player.state).toBe(123);
  });

  it("calls the supplied callback", function() {    
    var callback = jasmine.createSpy("successCallback");
    nether.player.deletePlayer(callback);
    
    expect(callback).toHaveBeenCalled();
  });

  it("handles failed http responses with returned Error class", function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(500, {});
      });

    var genericCallback = jasmine.createSpy("genericCallback");
    nether.player.deletePlayer(genericCallback);
    expect(genericCallback).toHaveBeenCalledWith(false); //todo: assert that nullity is correct here

  });
});

describe("nether player setPlayerState", function() {
  beforeEach(function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(204, "");
      });
  });

  it("exposes setState", function() {
    expect(nether.player.setState).toBeDefined();
  });

  it("sends an ajax message", function() {    
    nether.player.setState({}, jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalled();
  });

  it("sends an ajax message to a sensible place", function() {    
    //reinit to blank baseurl
    nether.init({ 
      "netherClientId" : "Kristofer",
      "netherClientSecret" : "Anko",
      "facebookAppId" : "Stuart",
      "netherBaseUrl" : ""
    }, function(){}, function(){}, document);
    nether.player.setState({}, jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url":"/api/player/state"}));
  });

  it("sends an ajax message using HTTP PUT method", function() {    
    nether.player.setState({}, jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"method":"PUT"}));
  });

  it("sends an ajax message to a configurable place", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.setState({}, jasmine.createSpy("successCallback"));    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url": exampleConfig.netherBaseUrl + "/api/player/state"}));
  });

  it("sends an ajax message containing the state", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.player.setState({ "someState":"yesItIs"}, jasmine.createSpy("successCallback"));    
    expect(nether.common.ajax).toHaveBeenCalledWith(
      jasmine.objectContaining({"data": 
        jasmine.objectContaining({ "state" : 
          jasmine.objectContaining({ "someState" : "yesItIs"})
           })
    }));
  });

  it("calls the supplied callback", function() {    
    var callback = jasmine.createSpy("successCallback");
    nether.player.setState({}, callback);
    
    expect(callback).toHaveBeenCalled();
  });

  it("handles failed http responses with returned Error class", function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(500, {});
      });

    var genericCallback = jasmine.createSpy("genericCallback");
    nether.player.setState({}, genericCallback);
    expect(genericCallback).toHaveBeenCalledWith(jasmine.any(Error)); //todo: assert that nullity is correct here

  });
});