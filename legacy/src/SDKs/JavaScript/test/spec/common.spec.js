var exampleConfig = {
        netherBaseUrl: '<nether url>',
        providers: nether.player.identity.providers.facebook | nether.player.identity.providers.nether,
        providerConfig: [{
                provider: nether.player.identity.providers.facebook,
                netherClientId: '<client Id>',
                netherClientSecret: '<client secret>',
                facebookAppId: '<facebook app Id>',
            },
            {
                provider: nether.player.identity.providers.nether,
                netherClientId: '<client Id>',
                netherClientSecret: '<client secret>' 
            }]
    }