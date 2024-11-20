## pattern overview

We should stick to the naming conventions laid out in the [architecture](./architecture.md). So, we should have "parser"s to create CSTs from strings, "converter"s to move between CSTs and ASTs, "translator"s to move between different types of ASTs, "transcriber"s to create strings from CSTs, "serializer"s to go from user defined types to ASTs, and "deserializer"s to go from ASTs to user defined types. 

The CSTs will be a discriminated union that corresponds directly to the [OData ABNF](https://docs.oasis-open.org/odata/odata/v4.01/cs01/abnf/odata-abnf-construction-rules.txt).

The ASTs will be a discriminated union that corresponds to the same ABNF but with the string literals removed, as well as "overhead" from the CST like aliases. 

The transcribers will be implemented as visitors on the CST nodes to convert them to strings using an intermediate `StringBuilder`.

The converters will be implemented as visitors on the AST or CST nodes to produce instances of the other.

TODO parsers will be...

TODO translators will be...

Please see the [appendix](#appendix) for other modeling options that were explored.

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

## let's now attempt take some time to show mechanically how to implement a node in the AST and all of its associated utilities

Let's try with `keyPredicate` using the [ABNF](https://docs.oasis-open.org/odata/odata/v4.01/cs01/abnf/odata-abnf-construction-rules.txt).

## appendix

### parsing

Parser combinators were tried using the Sprache nuget package. Though combinators are very flexible, the code is trivially readable, and development is exceedingly fast, combinators result in potentially incorrect parsing on grammars that are not context-free; the OData ABNF is not a context-free grammar, so we cannot rely on combinators. You can see an attempt at this implementation [here](https://github.com/OData/odata.net/blob/corranrogue9/framework/interfacesanddatatypes/odata/Root/OdataResourcePath/CombinatorParsers/OdataRelativeUriParser.cs).
