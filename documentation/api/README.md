# Nether REST API

## API Design Philosopy

The REST API for Nether is and should be designed to be easy to use and conform to the "normal" guidance of how a good REST API should look like. The Azure REST API (specifically the Azure Resource Manager, ARM, REST API) has and should influence the design and documentation of Nether's API. While still conforming to "best practices" of REST APIs, Nether's API has to take special consideration of beeing efficient "enough" for the different gaming scenarios it tries to solve.

## HTTP Response Codes

The API should conform to the "normal" guidance of returning HTTP Response Codes that conform to the [HTTP Standard](https://tools.ietf.org/html/rfc7231).

Code  | Reason-Phrase                  | Cacheable 
----- | ------------------------------ | ----------
100   | Continue                       | False     
101   | Switching Protocols            | False     
200   | OK                             | True      
201   | Created                        | False     
202   | Accepted                       | False     
203   | Non-Authoritative Information  | True      
204   | No Content                     | True      
205   | Reset Content                  | False     
206   | Partial Content                | True      
300   | Multiple Choices               | True      
301   | Moved Permanently              | True      
302   | Found                          | False     
303   | See Other                      | False     
304   | Not Modified                   | False     
305   | Use Proxy                      | False     
307   | Temporary Redirect             | False     
400   | Bad Request                    | False     
401   | Unauthorized                   | False     
402   | Payment Required               | False     
403   | Forbidden                      | False     
404   | Not Found                      | True      
405   | Method Not Allowed             | True      
406   | Not Acceptable                 | False     
407   | Proxy Authentication Required  | False     
408   | Request Timeout                | False     
409   | Conflict                       | False     
410   | Gone                           | True      
411   | Length Required                | False     
412   | Precondition Failed            | False     
413   | Payload Too Large              | False     
414   | URI Too Long                   | True      
415   | Unsupported Media Type         | False     
416   | Range Not Satisfiable          | False     
417   | Expectation Failed             | False     
426   | Upgrade Required               | False     
500   | Internal Server Error          | False     
501   | Not Implemented                | True      
502   | Bad Gateway                    | False     
503   | Service Unavailable            | False     
504   | Gateway Timeout                | False     
505   | HTTP Version Not Supported     | False     

### 200 OK

Default Response Code for GET Operations in Nether.

### 201 Created

Default Response Code for any API call that synchronously create a resource. If a corresponding request is made to retrieve the same resource, the caller should expect to retrieve the just created resource.

### 202 Accepted

Default Response Code for aynchronous APIs. The response signals that the request has been received by the server, but perhaps not yet processed. A caller should not expect to see any imediate updated state in the service after calling an API that responds with this Response Code.

