## goal

The goal is that an api producer can implement their service by handling requests using something as simple as `new OdataService(edmModel, dataProviders).HandleRequest(request)`.

Note that threading is left as a decision for the api producer *outside* of the context of implementing odata. They can use ASP.NET, azure functions, etc. to handle this; it should be orthogonal to the use of odata.

Also note that it is very useful for dataProviders to be modeled as OdataServices themselves because then an OdataService can be composed of multiple other OdataServices. A service can also be composed of multiple non-odata services that have been adapted into odata services.

Once data providers are seen as odata services themselves, we can also see that there will be at least one odata service implementation that is an odata client. This means that odata clients and odata services will have a unified interface and not differ from each other.

## architecture



read and write requests
4 http verbs
parse and transcribe


uri
request
response
csdl






serialize and deserialize


odata structure in c#
.net structure in c#
