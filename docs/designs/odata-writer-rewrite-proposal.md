# OData Writer re-design for better performance

The issue https://github.com/OData/odata.net/issues/3236 summarizes the performance issues of the current OData Writer design.
We aim to create a foundation for a new writer this will address these performance issues and scale better with larger inputs.

This document presents a very high-level proposal with aim of figuring the correct layering of abstraction levels and extensibility
mechanisms to expose to callers.

More concretely, here are the questions I hope to get answers to:

- What layers of abstractions should the new writer have to ensure we offer sufficient flexibility for use cases the OData writer must support, while providing efficient default implementations for common scenarios?
- How tightly coupled should we be with `JsonSerializer` and `Utf8JsonWriter`?
- Should we account for other formats other than JSON?
- Should we require an `IEdmModel` to be provided for serialization?

Here are the design principles that I want to adhere to:

- Avoid forcing the caller to create intermediate data unnecessarily.
  - A lot of the cost of the existing writer comes from intermediate data and repeated/redundant computations.
  - The existing writer requires the caller to create intermediate data via `ODataResource`, `ODataValue`, etc.
- If the caller passes structs as input, they should not be forced to box them.
  - Or existing abstractions often accept or return `object`, which force value types to be boxed.
- Avoid paying the cost for things we don't use.
  - Existing writer allocates things that are not used (e.g. `ODataResource.Actions`).
  - Existing writer performs validation eagerly (e.g. `ODataResourceBase.VerifyProperties()`) even when not necessary.
- New writer should expose similar levels of abstractions and extensibility to existing writer
- New writer should provide support for all the scenarios that the existing writer supports
  - Different input types (CLR objects, EdmObject, Dictionaries, JObject)
  - Different types of payloads: resource set, values, aggregation results, service document, raw values, metadata document, etc.
  - Dynamic properties, delta payloads, stream properties, polymorphic properties, $select/$expand
  - Standard OData annotations, custom annotations, etc.
  - Streaming support (start serialization before all data is available)

The sample code in this doc or for demonstration purposes and are subject to change as the design is fleshed out more.

## Top-level writer

This is the equivalent to the existing `ODataMessageWriter` layer, which also exposes the abstract `ODataWriter` and `ODataOutputContext` classes.
The `ODataMessageWriter` exposes methods for writing top-level payloads like metadata, service document, property values, etc. or methods
for creating writers for writing more complex, nested payloads like resources and resouce sets.

However, `ODataMessageWriter` doesn't implement any logic, all the logic is delegated to the abstract `ODataOutputContext` and `ODataWriter` classes.
The `ODataOutputContext` does most of the heavy lifting and is the one that determines the output format.

This means that if you wanted to create your own customer `ODataMessageWriter` from scratch (e.g. to output a different format other than JSON), you would
need to provide custom `ODataOutputContext` and `ODataWriter` implementations. This is a non-trivial undertaking and I doubt there are any implementations
in production other than those provided by the library.

If you do create custom implementations of `ODataWriter` and `ODataOutputContext`, it is up to you to ensure that you adhere to OData's protocol, for example
it's up to you to ensure the format of the response JSON matches the protocol, it is up to you to ensure you place

The library's default implementations of `ODataWriter` and `ODataOutputContext` (e.g. `ODataJsonOutputContext`) are internal and not exposed to the caller,
therefore they cannot be directly customized or extended. Instead OData provides extensions and customizability through the following mechanisms:

- You can configure various settings and options in `ODataMessageWriterSettings`
- You can replace the low-level `IJsonWriter` by implementing the interface
- You can control what gets written through the properties and annotatons that you add to `ODataResource` and similar types

Here's a rough proposal for the new writer:

```csharp
interface IODataMessageWriter<TContext, TState>
{
    IODataWriter<TContext, TState, TValue> GetWriter<TValue>(ODataPayloadKind payloadKind);
    // Alternatively we could have different `GetWriter` methods for different
}


// Writer for a specific payload kind
interface IODataWriter<TContext, TState, TValue>
{
    ValueTask WriterAsync(TValue value, TState state, TContext context);
}
```

where:

- `TContext`: represents an implementation-specific context type that encapsulates session-specific settings and format information (e.g. JSON options, ODataUri, Output stream, custom services)
- `TState`: implementation-specific state type that represents the current state of the serialization. Unlike the context, this is expected to change per object or level (e.g. current type being serialized, nesting depth, etc.). I'm not yet sure if it makes sense to have this
- `TValue`: the value being written. This is generic so we can write any value and make it possible to create strongly-typed writers without boxing, or without heavy use of reflection (e.g. source generators)

This is a fairly high-level generic that I'm still debating whether it's too generic to be useful. We do not expect customers to create custom implementation for this interface,
but it gives us some leeway to implement a writer for a different a output format if necessary. In practice, we would create a factory that returns a concrete implementation
based on some user configuration or builder.

We would have a single default `IODataMessageWriter` implementation for the default formats we support (xml $metadata, JSON paylods, raw values), and some default `IODataWriter` implementations
for different payload types (for resources, resource sets, etc.). The default `IODataWriter` implementation would support extensibility via custom provider interfaces to provide support
for different data types (e.g. CLR POCO classes, custom input sources, etc.)

Alternatively, we could have a single writer interface and create factory class that can create a factory or builder that can return a concrete instance based on configuration:

```csharp
interface IODataWriter<TContext, TState, TValue>
{
    ValueTask WriteAsync(TValue value, TState state, TContext context);
}

var odataWriter = ODataWriterBuilder.FromPayloadKind(payloadKind)
    .WithJsonOptions(jsonSerializationOptions)
    .WithODataUri(odataUri)
    .ConfigureResourceWriters(provider => provider.Add<Customer, CustomerResourceWriter>())
    .ForCollectionOf<Customer>()
    .Build();

odataWriter.GetWriter
```

## Custom extensible implementation

This layer is equivalent to the `ODataResource` and similar types. Since this design moves away from intermediate data, we'll replace these with interfaces that implement the logic of writing a specific type.

```c#
interface ODataJsonWriterContext
{
    public ODataUri ODataUri { get; }
    public IEdmModel Model { get; }
    public Ut8JsonWriter JsonWriter { get; }
    public JsonSerializerOptions { get; }
    public ODataPayloadKind PayloadKind { get; }
    public ODataMetadataLevel MetadataLevel {get; }
    public IODataValueWriterProvider { get; }
    // etc.
}

interface ODataJsonWriterState
{
    public IEdmType EdmType { get; }
    // this property would only be relevant when writing resource or resource set
    // in this model, 
    public SelectExpandClause { get; }
}

class ODataResourceSetWriter<IEnumerable<T>> : IODataWriter<ODataWriterContext, ODataWriterState, IEnumerable<T>>
{
    public async ValueTask WriteAsync(IEnumerable<T> value, ODataWriterState state, ODataWriterContext context)
    {
        if (state.IsTopLevel())
        {
            context.JsonWriter.WriteStartObject();
            var annotationsWriter = context.AnnotationsWriterProvider.GetAnnotationsWriter();
            annotationsWriter.WriteContextProperty(value, state, context);
            annotationsWriter.WriteDefaultAnnotations(value, state, context);
            annotationsWriter.WriteCustomAnnotations(value, state, context);

            context.JsonWriter.WriteEndObject();
        }
        else
        {
            context.JsonWriter.WriteStartArray();
            
            context.Jsonwriter.WriteEndArray();
        }
    }
}

class ODataClrResourceWriter<T> : IODataWriter<ODataWriterContext, ODataWriterState, T>
{
    public async ValueTask WriteAsync(IEnumerable<T> value, ODataWriterState state, ODataWriterContext context)
    {
        var writer = context.JsonWriter;
        writer.WriteStartObject();
    
        
        // These provides can be overriden by the customer, allowing them to provide support
        // for different input types without having to write custom IODataWriter implementations
        
        
        var annotationsWriter = context.AnnotationsWriterProvider.GetAnnotationsWriter<T>(value, state, context);
        // Selects properties based on SelectExpandClause
        // TODO: should we have separate methods for structural, navigation and dynamic properties?
        // I'm also skeptical about splitting propertySelector and property writer since it makes the
        // implementer return the collection of properties, which could be considered intermediate data.
        var propertySelector = context.ProperySelectorProvider.GetPropertySelector<T>(value, state, context);
        var propertyWriter = context.PropertyWriterProvider.GetPropertyWriter<T>(value, state, context);

        if (state.IsTopLevel())
        {
            annotationsWriter.WriteContextProperty(value, state, context);
        }
        
        annotationsWriter.WriteDefaultAnnotations(value, state, context);
        annotationsWriter.WriteCustomAnnotations(value, state, context);

        // should property be name or IEdmProperty?
        foreach (var property in propertySelector)
        {
            propertyWriter.Write(value, property, state, context);
        }
        
        if (state.EdmType.IsOpen)
        {
            foreach (var dynamicProperty in context.DynamicPropertySelectorProvider.GetPropertySelector<T>(value, state, context))
            {
                propertyWriter.WriteDynamicProperty(value, dynamicProperty, state, context);
            }
        }

        writer.WriteEndObject();
    }
}
```

In this model we create a number of default generic `IODataWriter` implementations for different payload kinds. These replace
our current `ODataResource` types as well as our existing internal writer and serializer implementations. This is where
we implement the OData serialization protocol (structure, annotations, conventions, etc.).

The user can leverage our `IODataWriter` and customize the serialization through custom implementations of
property writer, annotations writer etc.

The library can provide default property writer, annotations writer, etc. for common scenarios like `POCO` classes.
The types could even be generated at compile-time using source generators. Since we'll have one of each per type,
it should be much more efficient and scalable that creating `ODataValue`s per value instance.

Services who don't use CLR POCO classes as input can create custom property writers for their types.
For example, here's a custom property that uses a `JObject` as input.

```csharp
class JObjectPropertyWriter : IPropertyWriter<JObject, ODataWriterContext, ODataWriterState>
{
    public WriteProperty(JObject value, string property, ODataWriterState state, ODataWriterContext context)
    {
        var writer = context.JsonWriter;
        writer.WritePropertyName(property);
        // Should check the type of value, whether primitive, complex, etc.
        JsonSerializer.Serialize(writer, value[property]);
    }
}
```

## `JsonSerializer` integration

Here’s a refined and polished version of your proposal. I’ve improved the grammar, clarity, and flow while keeping all the key points intact:

---

**Should we provide an OData writer as a `JsonConverter`?**

There is some demand for this, and it does offer benefits:

* It’s easy to set up and works well with minimal API support—no need for a special formatter.
* We would need to create a new `JsonSerializerOptions` extension that includes OData-specific converters and settings. This may also require creating the options instance per top-level call to `JsonSerializer.Serialize`.
* In theory, customers could customize serialization using a custom `JsonConverter` instead of OData-specific constructs. Standard `JsonSerializer` features like `JsonPropertyNameAttribute`, `JsonIgnoreAttribute`, etc., would “just work.”

### Concerns:

* Some OData payloads—particularly `$value` responses—aren’t always valid JSON (e.g., unquoted strings), so they cannot be written using `Utf8JsonWriter`. Since `JsonSerializer` doesn’t expose the output stream, we’d need to:

  * Detect the payload type and use a different writer that supports `$value` (e.g., a custom `ODataValueWriter`), or
  * Design the new OData writer to write directly to the stream.

  Note: `$metadata` currently still defaults to XML.

* If customers implement their own `JsonConverter` to customize serialization for specific types, they would also need to manually handle support for `$select`, `$expand`, annotations, dynamic properties, etc. This includes knowing which annotations to write and where. If they already have generic `JsonConverters`, they likely won't work out-of-the-box. They would need to write OData-specific `JsonConverters`.

  Current customizations are handled through:

  * `ODataModelBuilder` (mapping between CLR types and `IEdmModel`, including property names)
  * `ODataMessageWriterSettings`
  * Custom `ODataSerializer` in ASP.NET Core OData
  * Custom OData resource construction
  * Custom `IJsonWriter` usage (though this is rarely done)

In minimal APIs, we create custom `JsonConverters` for `AspNetCoreOData` query result wrappers like `SelectExpandWrapper<T>` and use the default `JsonSerializer`. Select-expanded properties are exposed via `ToDictionary()`.

---

### Do customers often need to customize how data is serialized?

Typically, customers customize *what* data is written—not how it’s serialized. Mapping from `IEdmModel` to CLR types can be separated from the serialization process. The writer can serialize directly from the `IEdmModel`, which is common in scenarios not using CLR input or `AspNetCoreOData`.

---

### An Alternative: OData Writer Separate from `JsonSerializer`

Another option is to treat the OData writer as independent from `JsonSerializer`. In this model:

* `JsonSerializer` is just a tool that the OData writer can invoke when needed.
* The OData writer decides whether to use `JsonSerializer`, `Utf8JsonWriter`, or something else.
* The default OData writer could still use `Utf8JsonWriter`, unless we need a more abstract output layer for other formats.

---

### Should the payload writer use custom `JsonConverter`s?

This could be disabled by default and only enabled if there’s sufficient demand. We’d likely need to expose hooks for users to control inclusion of annotations and other OData-specific features.

We could also create a `JsonConverter` that internally calls the OData payload writer. But the key is to design a **custom OData payload writer first**, then explore how and where `JsonSerializer` or `JsonConverters` can be leveraged.

---

### Benefits

* OData writer interfaces are format-agnostic, enabling creation of custom writer implementations in the future.
* We retain control over how tightly we couple with `JsonSerializer`. (We already transitioned from `JsonWriter` to `Utf8JsonWriter` without breaking changes, thanks to `IJsonWriter` abstraction.)
* We can freely write any payload type via a unified top-level writer interface.
* Easier to standardize and control what annotations are written and when.
* We can customize primitive converters for compatibility with OData (e.g., handling of `Infinity`, date formats, etc.).

---

### Concerns

* We may end up reimplementing a less efficient version of the `JsonConverter` infrastructure (e.g., for custom type writers).
* Users would need to learn and implement custom OData writer interfaces and concepts, instead of reusing familiar `JsonConverter`s. However, this is not significantly different from the current approach, where users already work with `ODataSerializer` and custom OData resources.


## Support for streamining

The current OData writer supports streaming since each nested ODataResource is written separately from the its parent or its children. The `WriteStart` and `WriteEnd` methods of the writer
are used to mark the beginning and end of a scope.

We could consider a similar approach and expose `WriteStart` and `WriteEnd` methods in the new writer. Alternatively, we could support streaming though `IAsyncEnumerable`. We would create
`IODataWriter`s that support `IAsyncEnumerable` and the caller would use this this to stream values into the writer.

## Should we always require an `IEdmModel` to be provided?
