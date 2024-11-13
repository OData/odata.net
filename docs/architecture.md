## goal

The goal is that an api producer can implement their service by handling requests using something as simple as `new OdataService(edmModel, dataProviders).HandleRequest(request)`.

Note that threading is left as a decision for the api producer *outside* of the context of implementing odata. They can use ASP.NET, azure functions, etc. to handle this; it should be orthogonal to the use of odata.

Also note that it is very useful for dataProviders to be modeled as OdataServices themselves because then an OdataService can be composed of multiple other OdataServices. A service can also be composed of multiple non-odata services that have been adapted into odata services.

Once data providers are seen as odata services themselves, we can also see that there will be at least one odata service implementation that is an odata client. This means that odata clients and odata services will have a unified interface and not differ from each other.

## architecture

The handling of an odata request involves 5 components:
1. csdl
2. http verb
3. uri
4. request body
5. response body

I want to take a moment now to convince why we should avoid dicsussing C# models for the time being and expect customers to rely strictly on odata types. This can be improved and built on top of in the future, and we **must** make certain that doing so is possible, or else our architecture will severely limit what customers can reasonably migrate to. Now for the convincing, consider the following CSDL:

tODO https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_DeepInsert
```xml
<?xml version="1.0" encoding="utf-8"?>
<edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
  <edmx:DataServices>
    <Schema Namespace="self" xmlns="http://docs.oasis-open.org/odata/ns/edm">
      
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
```

## follow-ups
1. being able to stream payloads
2. handling serialization of customer-defined types







TODO avoid the c# stuff for now?

serialize and deserialize


odata structure in c#
.net structure in c#
