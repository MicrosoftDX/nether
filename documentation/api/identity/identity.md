# Identity API

Notes on the docs to write up:

* custom fb grant (http://docs.identityserver.io/en/release/topics/extension_grants.html)
  * convert fb user access token to nether token (unity, Facebook JavaScript SDK)
* urls
  * /identity/.well-known/openid-configuration
    * /identity/account/logout
  * /api/identity-test (to echo claims back)
* document the auth flow
  * authn
    * fb-usertoken custom grant
    * implicit flow in browser
    * username/password flow
  * authn, and then checking/creating gamertag
