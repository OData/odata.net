## pattern overview

The general pattern is that the CSTs will be a discriminated union that corresponds directly to the [OData ABNF](https://docs.oasis-open.org/odata/odata/v4.01/cs01/abnf/odata-abnf-construction-rules.txt). The ASTs will be a discriminated union that corresponds as directly as possible to the [OData URL conventions](https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part2-url-conventions.html) and the [OData protocol](https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html) documents; note that these documents will, from time to time, begin to define concepts without given them names: in these cases, we must create our own name, and we should try to consistently use those names.

We should also stick to the naming conventions laid out in the [architecture](./architecture.md). So, we should have "parser"s to create CSTs from strings, "converter"s to move between CSTs and ASTs, "translator"s to move between different types of ASTs, "transcriber"s to create strings from CSTs, "serializer"s to go from user defined types to ASTs, and "deserializer"s to go from ASTs to user defined types. 

## odata request

An OData request has a verb, a URL, and a request body. Taking a bottom up approach, let's start with the URL and request body and build into an odata request type.

### odata uri

#### odata resource path

##### CST

```csharp
// pull from `odataRelativeUri` definition in https://docs.oasis-open.org/odata/odata/v4.01/cs01/abnf/odata-abnf-construction-rules.txt
public abstract class OdataRelativeUri
{
  private OdataRelativeUri()
  {
  }

  public sealed class 
}
```
