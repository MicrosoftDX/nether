const exampleConfig = { 
      "netherClientId" : "Kristofer",
      "netherClientSecret" : "Anko",
      "facebookAppId" : "Stuart",
      "netherBaseUrl" : "https://andy.and.james/from/elastacloud"
    };
//const host = (new JSDOM('<html><head><script>/*usedtofindfirstscriptTag*/</head><body></body></html>')).window;
    
describe("Initialisation", function() {
  //var nether = require("../../src/nether.js");

  it("is a thing", function() {
    expect(nether).toBeDefined();
  });

  it("can be initialized", function() {
    expect(nether.init).toBeDefined();
  });

  it("can be configured", function() {
  
    nether.init(exampleConfig, function(){}, function(){}, document);
    expect(nether.player.identity.netherClientId).toBe(exampleConfig.netherClientId);
    expect(nether.player.identity.netherClientSecret).toBe(exampleConfig.netherClientSecret);
    expect(nether.player.identity.facebookAppId).toBe(exampleConfig.facebookAppId);
    expect(nether.netherBaseUrl).toBe(exampleConfig.netherBaseUrl);
  });
});

describe("nether networking", function() {
  beforeEach(function() {
    nether.common.ajax = jasmine.createSpy("ajaxWrapper");
  });

  afterEach(function() {

  });


  it("netherCallback not manually invoked", function() {    
    var doneFn = jasmine.createSpy("success");
    nether.init(exampleConfig, function(){}, doneFn, document);
    
    expect(doneFn).not.toHaveBeenCalled();
  });

  it("remote not manaully invoked", function() {
        nether.init(exampleConfig, function(){}, function(){}, document);
        expect(nether.common.ajax).not.toHaveBeenCalled();
  });
});

describe("nether getAllLeaderboards", function() {
  beforeEach(function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(200, "{\"leaderboards\": [1,2,3]}");
      });
  });

  it("exposes getAllLeaderboards", function() {
    expect(nether.leaderboard.getAllLeaderboards).toBeDefined();
  });

  it("sends an ajax message", function() {    
    nether.leaderboard.getAllLeaderboards(jasmine.createSpy("successCallback"));
    
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
    nether.leaderboard.getAllLeaderboards(jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url":"/api/leaderboards"}));
  });

  it("sends an ajax message to a configurable place", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.leaderboard.getAllLeaderboards(jasmine.createSpy("successCallback"));    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url": exampleConfig.netherBaseUrl + "/api/leaderboards"}));
  });

  it("calls the supplied callback", function() {    
    var callback = jasmine.createSpy("successCallback");
    nether.leaderboard.getAllLeaderboards(callback);
    
    expect(callback).toHaveBeenCalled();
  });

  it("handles failed http responses gracefully", function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(500, {});
      });

    var genericCallback = jasmine.createSpy("genericCallback");
    nether.leaderboard.getAllLeaderboards(genericCallback);
    expect(genericCallback).toHaveBeenCalledWith(500, jasmine.any(Object));

  })
});

describe("nether getLeaderboard", function() {
  beforeEach(function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(200, "{\"positions\": [1,2,3]}");
      });
  });

  it("exposes getLeaderboard", function() {
    expect(nether.leaderboard.getLeaderboard).toBeDefined();
  });

  it("sends an ajax message", function() {    
    nether.leaderboard.getLeaderboard(1, jasmine.createSpy("successCallback"));
    
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
    nether.leaderboard.getLeaderboard(1, jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url":"/api/leaderboards/1"}));
  });

  it("sends an ajax message to a configurable place", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.leaderboard.getLeaderboard(1, jasmine.createSpy("successCallback"));    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url": exampleConfig.netherBaseUrl + "/api/leaderboards/1"}));
  });

  it("calls the supplied callback", function() {    
    var callback = jasmine.createSpy("successCallback");
    nether.leaderboard.getLeaderboard(1, callback);
    
    expect(callback).toHaveBeenCalled();
  });

  it("handles failed http responses gracefully", function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(500, {});
      });

    var genericCallback = jasmine.createSpy("genericCallback");
    nether.leaderboard.getLeaderboard(1, genericCallback);
    expect(genericCallback).toHaveBeenCalledWith(500, jasmine.any(Object));

  })
});