
describe("nether scores addScore", function() {
  beforeEach(function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(200, "{\"scores\": [1,2,3]}");
      });
  });

  it("exposes addScore", function() {
    expect(nether.scores.addScore).toBeDefined();
  });

  it("sends an ajax message", function() {    
    nether.scores.addScore(1, jasmine.createSpy("successCallback"));
    
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
    nether.scores.addScore(1, jasmine.createSpy("successCallback"));
    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url":"/api/scores"}));
  });

  it("sends an ajax message to a configurable place", function() {    
    nether.init(exampleConfig, function(){}, function(){}, document);
    nether.scores.addScore(1, jasmine.createSpy("successCallback"));    
    expect(nether.common.ajax).toHaveBeenCalledWith(jasmine.objectContaining({"url": exampleConfig.netherBaseUrl + "/api/scores"}));
  });

  it("calls the supplied callback", function() {    
    var callback = jasmine.createSpy("successCallback");
    nether.scores.addScore(1, callback);
    
    expect(callback).toHaveBeenCalled();
  });


  it("calls the supplied callback with true in success case", function() {    
    var callback = jasmine.createSpy("successCallback");
    nether.scores.addScore(1, callback);
    
    expect(callback).toHaveBeenCalledWith(true);
  });

  it("handles failed http responses gracefully", function() {
    nether.common.ajax = jasmine.createSpy().and.callFake(function(args) {
        args.callback(500, {});
      });

    var genericCallback = jasmine.createSpy("genericCallback");
    nether.scores.addScore(1, genericCallback);
    expect(genericCallback).toHaveBeenCalledWith(false);

  });

  it("guards against malformed score", function() { 
    var callback = jasmine.createSpy("successCallback");
    nether.scores.addScore({ "whatIsThis" : "thisIsNotAScore"}, callback);
    
    expect(callback).toHaveBeenCalledWith(false);

  });

  it("guards against a negative score", function() { 
    var callback = jasmine.createSpy("successCallback");
    nether.scores.addScore(-1, callback);
    
    expect(callback).toHaveBeenCalledWith(false);

  });
});