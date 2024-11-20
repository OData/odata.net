## pattern overview

We should stick to the naming conventions laid out in the [architecture](./architecture.md). So, we should have "parser"s to create CSTs from strings, "converter"s to move between CSTs and ASTs, "translator"s to move between different types of ASTs, "transcriber"s to create strings from CSTs, "serializer"s to go from user defined types to ASTs, and "deserializer"s to go from ASTs to user defined types. 

The CSTs will be a discriminated union that corresponds directly to the [OData ABNF](https://docs.oasis-open.org/odata/odata/v4.01/cs01/abnf/odata-abnf-construction-rules.txt).

The ASTs will be a discriminated union that corresponds to the same ABNF but with the string literals removed, as well as "overhead" from the CST like aliases. 

The transcribers will be implemented as visitors on the CST nodes to convert them to strings using an intermediate `StringBuilder`.

The converters will be implemented as visitors on the AST or CST nodes to produce instances of the other.

TODO parsers will be...

TODO translators will be...

## odata resource path example

Let's use the odata resource path as an example of the above patterns.

### CST

A sample implementation of the CST is [here](../odata/Root/OdataResourcePath/ConcreteSyntaxTreeNodes/OdataRelativeUri.cs).

### AST

A sample implementation of the AST is [here](../odata/Root/OdataResourcePath/AbstractSyntaxTreeNodes/OdataRelativeUri.cs).

### CST transcriber

A sample implementation of the transcriber is [here](../odata/Root/OdataResourcePath/Transcribers/OdataRelativeUriTranscriber.cs).

### CST to AST converter

A sample implementation of the CST to AST translator is [here](../odata/Root/OdataResourcePath/CstToAstTranslators/OdataRelativeUriTranslator.cs).

### AST to CST converter

A sample implementation of the CST to AST translator is [here](../odata/Root/OdataResourcePath/AstToCstTranslators/OdataRelativeUriTranslator.cs).

### uri parser

TODO

### translator

TODO

TODO do a demo of implementing a discriminated union
TODO do a demo of a "terminal" tree that parses and transcribes; try using /entityset/key/property for this (i think you can't actually use combinators because odata is not a context free grammar)
TODO try ref struct
