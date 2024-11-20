## pattern overview

The general pattern is that the CSTs will be a discriminated union that corresponds directly to the [OData ABNF](https://docs.oasis-open.org/odata/odata/v4.01/cs01/abnf/odata-abnf-construction-rules.txt). The ASTs will be a discriminated union that corresponds to the same ABNF but with the string literals removed, as well as "overhead" from the CST like aliases. 

We should also stick to the naming conventions laid out in the [architecture](./architecture.md). So, we should have "parser"s to create CSTs from strings, "converter"s to move between CSTs and ASTs, "translator"s to move between different types of ASTs, "transcriber"s to create strings from CSTs, "serializer"s to go from user defined types to ASTs, and "deserializer"s to go from ASTs to user defined types. 

## odata request

An OData request has a verb, a URL, and a request body. Taking a bottom up approach, let's start with the URL and request body and build into an odata request type.

### odata uri

#### odata resource path

##### CST

A sample implementation of the CST is [here](../odata/Root/OdataResourcePath/ConcreteSyntaxTreeNodes/OdataRelativeUri.cs).

##### AST



TODO link to the repo instead of having markdown code blocks
TODO split classes into individual files
TODO do a demo of implementing a discriminated union
TODO do a demo of a "terminal" tree that parses and transcribes; try using /entityset/key/property for this (i think you can't actually use combinators because odata is not a context free grammar)
TODO try ref struct

#### CST transcriber

TODO

#### CST to AST translator

TODO

#### AST to CST translator

TODO

#### uri parser

TODO
