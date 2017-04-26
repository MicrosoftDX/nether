describe("Initialisation", function() {

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
