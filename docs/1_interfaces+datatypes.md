## pattern overview

We should stick to the naming conventions laid out in the [architecture](./architecture.md). So, we should have "parser"s to create CSTs from strings, "converter"s to move between CSTs and ASTs, "translator"s to move between different types of ASTs, "transcriber"s to create strings from CSTs, "serializer"s to go from user defined types to ASTs, and "deserializer"s to go from ASTs to user defined types. 

The CSTs will be a discriminated union that corresponds directly to the [OData ABNF](https://docs.oasis-open.org/odata/odata/v4.01/cs01/abnf/odata-abnf-construction-rules.txt).

The ASTs will be a discriminated union that corresponds to the same ABNF but with the string literals removed, as well as "overhead" from the CST like aliases. 

The transcribers will be implemented as visitors on the CST nodes to convert them to strings using an intermediate `StringBuilder`.

The converters will be implemented as visitors on the AST or CST nodes to produce instances of the other.

TODO parsers will be...

The translators will be implemented as visitors on the AST nodes to produce a corresponding AST for the desired data store.

Please see the [appendix](#appendix) for other modeling options that were explored.

## odata resource path example

Let's use the odata resource path as an example of the above patterns.

### syntactic CST?

TODO do you want to have a syntactic CST that is a context-free grammar? then your parsers could be combinators; if you do this you will need a converter from syntactic to semantic CST
TODO regardless of the above, you need to have a syntactic CST **anyway** to have parity with ODL

### (semantic?) CST

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

## some takeaways

Using this discriminated union pattern, it is clear from the code where the "feature gaps" are: any nodes that only have a private constructor have not yet been implemented. We should implement a node in its completeness so that we can maintain this status. Doing this will ensure that the "feature gaps" are never tribal knowledge, but something that any developer can discover just by looking at the code.

These unions *also* allow us to easily scale across developers. Any number of developers can implement as many nodes as there are developers provided that no two developers are working on the same node. Also, by separating each phase of the handling process in this way, we are able to implement the nodes "piecewise", meaning: a developer can define the AST node, create a PR, and merge it; the developer can then define the CST node, create a PR, and merge it; they can then implement a converter, and so in. These can all be done as individual, discrete, atomic steps done (mostly) independently of each other. 

## appendix

### parsing

Parser combinators were tried using the Sprache nuget package. Though combinators are very flexible, the code is trivially readable, and development is exceedingly fast, combinators result in potentially incorrect parsing on grammars that are not context-free; the OData ABNF is not a context-free grammar, so we cannot rely on combinators. You can see an attempt at this implementation [here](https://github.com/OData/odata.net/blob/corranrogue9/framework/interfacesanddatatypes/odata/Root/OdataResourcePath/CombinatorParsers/OdataRelativeUriParser.cs).
