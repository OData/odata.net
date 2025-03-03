# OData URI Parser Rewrite for high performance and reliability

This is a proposal to rewrite the OData URI parser stack from the ground up based on a different architecture. The primary motivation for this proposal is performance,
but it also aims to make improvements to usability and developer experience.

## Motivations

### Performance

This proposal is part of larger effort to turn OData into a high-performance library. The URI parser is a key component of the OData experience as parsing the request URL is the first
step to handling and OData request. We have made efforts aimed at improving performance, the most recent being the [work by `xuhg` to use `ReadOnlySpan<char>` to cut down
string allocations](https://github.com/OData/odata.net/pull/3077). While we have seen improvements in such efforts, we're limited by the current architecture and public-facing APIs.
If we entertain a breaking change, we can change the architecture and redesign the parser for better performance and low resource use.

The aim of this effort to reach at least 5x improvement in performance and reduction in allocations on the core URI parser on benchmarks that demonstrate end-to-end semantic parsing of common URL patterns.

### Usability

The OData URI has two main parts, the path segments and query options. There different parsers for the path and the query options.
In general, each component has two parsing phases: syntactic parse and semantic parse. The syntactic parse is supposed to return a tree or sequence of nodes that represent the structure without binding model information. The semantic pass adds type information and validation from the model. However, the separation between syntax and semantic parse isn't cleanly implemented in all cases:

- The `SelectExpandSyntactiParser` which performs syntactic parsing of the select and expand query options is internal, meaning that end users cannot access it. It also has a dependency on `IEdmStructuredType`, meaning the model is required to use it. Customers have expressed interest in parsing URLs without the model.
- The path parser with parses the path segments simply splits the string on `/` slash symbols. As a result it doesn't properly handle cases where the keys in the path may contain slashes. [A comment in the code](https://github.com/OData/odata.net/blob/main/src/Microsoft.OData.Core/UriParser/Parsers/UriPathParser.cs#L55) suggests that this is a known bug.
- The `UriQueryOptionsParser` with does a lexical parse of some query options like `$filter` reads sequences delimited by `(` and `)` as single string. This means that items of literal collections cannot be individually consumed in the syntactic phase. These are properly parsed later in the semantic binder (for example, `FilterBinder`) where a model is required. This binding is also not ideal. For example, when parsing an `in` or `has` expression, the collection [literal is parsed by replacing the delimiting `(` and `)` with `[` and `]` and then calling the JsonReader to parse the array](https://github.com/OData/odata.net/blob/main/src/Microsoft.OData.Core/UriParser/Binders/InBinder.cs#L119). This "hack" is the cause of several bugs and incompalities related to the `in` operator, such as:
  - [](https://github.com/OData/odata.net/pull/3190)
  - https://github.com/OData/odata.net/issues/2875
  - https://github.com/OData/odata.net/issues/3098
  - https://github.com/OData/odata.net/pull/2711
  - [Source comment acknowleging flaws of this approach](https://github.com/OData/odata.net/blob/main/src/Microsoft.OData.Core/Uri/ODataUriConversionUtils.cs#L677).

The rewrite provides an opportunity to parse each expression properly from the lexical tokens up to the semantic nodes.

## High-level architectural changes

The key changes proposed:

- Avoid premature string allocations. Reference segments in the source string using `ReadOnlySpan<char>` until the user explicitly asks for a `string`.
- Store parsed nodes in array of structs instead of graph of objects.
- Frugal use of space, for example pack data in bit flags where applicable instead of separate bool fields. This helps keep the structs small and less expensive to move around.

Using an array of node structs is a common approach for memory-efficient parsers like `System.Text.Json`.

### Impact on customer-facing APIs and usability

A key side-effect of this approach is that user-facing nodes will not be class instances, but structs. This means no inhertiance. The developer will not rely on `is` operator or visitors based on polymorphism to process nodes.
They will rely on enums to distinguish between different node kinds and use methods such as `TryGetString()` or `TryGetInt()` or `EnumerateChildren()` to extract content from the nodes. This is similar to the `JsonDocument` and `JsonElement` APIs in `System.Text.Json`. We may consider adding a higher-level layer of class-based APIs if they provide significant value.

## Proof-concept and experiments

[This repository](https://github.com/habbes/experiments/tree/master/ODataSlimUrlParserConcept) contains the source code, benchmarks and experiments used in the proof-of-concept to demonstrate the feasibility of this proposal.

## Components

This section describes the proposal in more detail for each of the key components of the parser. The focus is on the query options parser, and the `$filter` query option in particular because of its complexity relative to other query options like `$select` and `$expand`. We'll also mention the path parser for completeness.

### Path parser

The current path parser is implemented in two phases:

1. [The `UriPathParser`](https://github.com/OData/odata.net/blob/main/src/Microsoft.OData.Core/UriParser/Parsers/UriPathParser.cs) splits the path into an `ICollection<string>` of segments using `/` as the separator. It doesn't check for presence of `/` charaters inside of string literals in key segments.
2. The internal [ODataPathParser](https://github.com/OData/odata.net/blob/main/src/Microsoft.OData.Core/UriParser/Parsers/ODataPathParser.cs) goes through each segment and binds to it a schema element and type. This steps returns an `IList<ODataPathSegment>` where `ODataPathSegment` in abstract class. Each type of segment is a subclass of `ODataPathSegment`, for example: `EntitySetSegment`, `EntityIdSegment`, etc.

Here are the proposed changes:

1. The lexer should not split the string. It should be an iterator that successively get the next each time `Read()` is called. Each token as span into the input string. There should be no heap allocations in this phase. We can also properly handle slash characters inside string literals.
2. The semantic parser progressively advances the tokenizer creating a semantic node for each segment. The semantic node would have roughly the following shape:

```csharp
internal readonly struct PathSegmentNode
{
    public PathSegmentKind Kind { get; }
    // index of the last child, for example if this is a function with args or a compound key.
    // So paths don't have nested children, we can easily compute the child based on the current index
    // and index of the last child.
    int LastChild { get; } 
    public ValueRange Range { get }
    public IEdmElement EdmElement { get;} // Should we make edm-element lookup lazy?
}
```

These nodes don't have a direct reference to the string, the range just contains the start and end, but don't point to the actual string. This makes the nodes flexible to use with either `ReadOnlySpan<char>` or `ReadOnlyMemory<char>`.
The `PathSegmentNode` is not exposed to the user, it needs to be linked to the parent list to be usable, it doesn't have enough context to be used as a standalone node. The is also relatively large and we don't want to incur the cost of direct copies.

The parsed path would look like:

```csharp
public sealed class PathSegmentList : IReadOnlyList<PathSegment>
{
    private PathSegmentNode[] _nodes;
    
    public PathSegment this[int index] => return new PathSegment(this, index);
}
```

The `PathSegment` is the public-facing type representing a path segment. It references the index of the `PathSegmentNode` in the list and exposes
method to extract data from the node based on its type.

