# OData Writer re-design for performance

The issue https://github.com/OData/odata.net/issues/3236 summarizes the performance issues of the current OData Writer design
that the proposed designed aims to address.

This is a follow-up to the design and discussion presented [in this doc](https://github.com/OData/odata.net/pull/3260). This document
presents a more concrete architecture and sample implementation.

## Minimal example

Assuming we want to write the following list of `Customer` objects in response
to the request url `GET https://service/Customers?$select=Id,Name&$expand=Orders($select=Id,Amount,Status)`

```c#
// Sample data
var customers = new List<Customer>
{
    new Customer 
    {
        Id = 1,
        Name = "John Doe",
        Email = "john@example.com",
        RegisterDate = new DateTime(2024, 1, 15),
        CreditLimit = 5000.00m,
        IsPreferred = true,
        PhoneNumbers = new[] { "+1-555-123-4567", "+1-555-987-6543" },
        BillingAddress = new Address 
        { 
            Street = "123 Main St",
            City = "Seattle",
            Country = "USA",
            PostalCode = "98101"
        },
        Orders = new List<Order>
        {
            new Order { Id = 101, OrderDate = new DateTime(2025, 6, 15), Amount = 150.00m, Status = "Shipped" },
            new Order { Id = 102, OrderDate = new DateTime(2025, 6, 16), Amount = 75.50m, Status = "Processing" }
        }
    },
    new Customer
    {
        Id = 2,
        Name = "Jane Smith",
        Email = "jane@example.com",
        RegisterDate = new DateTime(2024, 3, 20),
        CreditLimit = 7500.00m,
        IsPreferred = false,
        PhoneNumbers = new[] { "+1-555-789-0123" },
        BillingAddress = new Address 
        { 
            Street = "456 Oak Ave",
            City = "Portland",
            Country = "USA",
            PostalCode = "97201"
        },
        Orders = new List<Order> 
        {
            new Order { Id = 103, OrderDate = new DateTime(2025, 6, 17), Amount = 240.00m, Status = "Delivered" }
        }
    }
};
```

### Writing the entity set payload using the existing OData writer

```csharp

var settings = new ODataMessageWriterSettings
{
    ODataUri = odataUri }
};

var message = new ODataResponseMessage { Stream = outputStream };
var writer = new ODataMessageWriter(message, settings, model);

// Start writing a feed
var writer = await writer.CreateODataResourceSetWriterAsync(
    new ODataResourceSetSerializationInfo
    {
        NavigationSourceEntityTypeName = customerType.FullName(),
        NavigationSourceName = "Customers",
        SelectExpandClause = selectExpandClause
    });

await writer.WriteStartAsync(new ODataResourceSet
{
    Count = customers.Count
});

// Write each customer
foreach (var customer in customers)
{
    // Only include properties specified in $select
    var resource = new ODataResource
    {
        Properties = new[]
        {
            new ODataProperty { Name = "Id", Value = customer.Id },
            new ODataProperty { Name = "Name", Value = customer.Name }
        }
    };

    // Write nested orders
    var ordersResource = new ODataNestedResourceInfo
    {
        Name = "Orders",
        IsCollection = true
    };

    await writer.WriteStartAsync(resource);
    await writer.WriteStartNestedResourceInfoAsync(ordersResource);

    foreach (var order in customer.Orders)
    {
        var orderResource = new ODataResource
        {
            Properties = new[]
            {
                new ODataProperty { Name = "Id", Value = order.Id },
                new ODataProperty { Name = "Amount", Value = order.Amount },
                new ODataProperty { Name = "Status", Value = order.Status }
            }
        };
        
        await writer.WriteNestedResourceAsync(orderResource);
    }

    await writer.WriteEndNestedResourceInfoAsync();
    await writer.WriteEndAsync();
}

await writer.WriteEndAsync();
```

This produces output like:

```json
{
  "@odata.context": "https://service/$metadata#Customers(Id,Name,Orders(Id,Amount,status))",
  "value": [
    {
      "Id": 1,
      "Name": "John Doe",
      "Orders": [
        {
          "Id": 101,
          "Amount": 150.00,
          "Status": "Shipped"
        },
        {
          "Id": 102,
          "Amount": 75.50,
          "Status": "Processing"
        }
      ]
    },
    {
      "Id": 2,
      "Name": "Jane Smith", 
      "Orders": [
        {
          "Id": 103,
          "Amount": 240.00,
          "Status": "Delivered"
        }
      ]
    }
  ]
}
```

### Writing a the same entity set using the proposed writer

This is just a demonstration and subject to change the as the design evolves:

```csharp
var options = ODataSerializerOptions.CreateDefault();

// Note: default writers for plain CLR types may be generated dynamically
// either through runtime code generation or source generators at compile time
class CustomerJsonWriter : ODataResourceJsonWriter<Customer>
{
    public ValueTask WritePropertyValue(Customer customer, IEdmProperty property, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        if (property.Name == "Id")
        {
            return context.WriteValueAsync(customer.Id, state);
        }
        else if (property.Name == "Name")
        {
            return context.WriteValueAsync(customer.Name, state);
        }
        else if (property.Name == "Email")
        {
            return context.WriteValueAsync(customer.Email, state);
        }
        else if (property.Name == "Orders")
        {
            return context.WriteValueAsync(customer.Orders, state);
        }
    
        // etc.
    
        return ValueTask.CompletedTask;
    }
}

class OrderJsonWriter : ODataResourceJsonWriter<Order>
{
    public ValueTask WritePropertyValue(Order order, IEdmProperty property, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        if (property.Name == "Id")
        {
            return context.WriteValueAsync(order.Id, state);
        }
        else if (property.Name == "OrderDate")
        {
            return context.WriteValueAsync(order.OrderDate, state);
        }
        else if (property.Name == "Amount")
        {
            return context.WriteValueAsync(order.Amount, state);
        }
        else if (property.Name == "Status")
        {
            return context.WriteValueAsync(order.Status, state);
        }
    
        return ValueTask.CompletedTask;
    }
}

options.AddValueWriter(new CustomerJsonWriter());
options.AddValueWriter(new OrderJsonWriter());


await ODataSerializer.WriteAsync(customers, odataUri, model, options);
```

Notable improvements over the existing writer:

- No intermediate ODataResource or ODataProperty allocations required
- Properties are written directly from source objects without boxing
- Instead of pushing properties to the writer, the writer requests the property it wants to write based on the `IEdmModel` and `$select`, reducing the need for validation.
- Generic implementations avoid reflection and boxing of primitive values
- Reusable providers can be registered once and reused across requests
- Select/expand handling is built into the resource writer based on `SelectExpandClause`, but can be customized.

## Preliminary benchmarks

These are preliminary benchmark results to demonstrate that this design can improve performance. I'll run benchmark
more scenarios over time. Bear in mind that there's still room for improvement as the design and implementation is improved.

```txt
// * Summary *

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.3775/24H2/2024Update/HudsonValley)
Intel Xeon W-2123 CPU 3.60GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.100-preview.1.25120.13
  [Host]     : .NET 10.0.0 (10.0.25.8005), X64 RyuJIT AVX-512F+CD+BW+DQ+VL
  DefaultJob : .NET 10.0.0 (10.0.25.8005), X64 RyuJIT AVX-512F+CD+BW+DQ+VL
```


| Method             | WriterName                                           | Mean     | Error    | StdDev    | Median   | Gen0      | Gen1    | Gen2    | Allocated |
|------------------- |----------------------------------------------------- |---------:|---------:|----------:|---------:|----------:|--------:|--------:|----------:|
| WriteToMemoryAsync | JsonSerializer                                       | 11.46 ms | 0.297 ms |  0.855 ms | 11.19 ms |   46.8750 | 31.2500 | 31.2500 |    7.2 MB |
| WriteToMemoryAsync | **NewODataSerializer**                               | 22.75 ms | 0.452 ms |  1.135 ms | 22.66 ms |  125.0000 | 31.2500 | 31.2500 |  10.91 MB |
| WriteToMemoryAsync | ODataMessageWriter                                   | 47.74 ms | 1.914 ms |  5.271 ms | 47.10 ms | 7000.0000 |       - |       - |  35.71 MB |
| WriteToMemoryAsync | ODataMessageWriter-Async                             | 62.43 ms | 1.724 ms |  4.833 ms | 61.06 ms | 7000.0000 |       - |       - |  35.71 MB |
| WriteToMemoryAsync | ODataMessageWriter-NoValidation                      | 38.10 ms | 0.933 ms |  2.602 ms | 37.37 ms | 7500.0000 |       - |       - |  35.71 MB |
| WriteToMemoryAsync | ODataMessageWriter-NoValidation-Async                | 55.81 ms | 1.884 ms |  5.467 ms | 53.91 ms | 7000.0000 |       - |       - |  35.71 MB |

The results are based on the benchmarks in
[`test/PerformanceTests/SerializationComparisonTests/JsonWriterBenchmarks`](../../test/PerformanceTests/SerializationComparisonsTests/JsonWriterBenchmarks)
excuted using the following command

```sh
dotnet run -c Release --framework=net10.0 -- --filter *Memory*
```

The benchmarks serialize a collection of entities to a `MemoryStream`:

```json
{
  "@odata.context": "https://services.odata.org/V4/OData/OData.svc/$metadata#Customers",
  "value": [
    {
      "Id": 1,
      "Name": "Cust1 𐀅 ä",
      "Emails": [
        "emailA@mailer.com1",
        "emailB@mailer.com1"
      ],
      "Bio": "This is a bio 1",
      "Content": "AQIDBAE=",
      "HomeAddress": {
        "City": "City1 𐀅 ä",
        "Street": "Street1\n\"escape this\"",
        "Misc": "This is a test1"
      },
      "Addresses": [
        {
          "City": "CityA1",
          "Street": "StreetA1",
          "Misc": "This is a test A1"
        },
        {
          "City": "CityB1",
          "Street": "StreetB1",
          "Misc": "This is a test B1"
        }
      ]
    }
  ]
}
```

The following tests are based on running the [`bombardier`](https://github.com/codesenberg/bombardier) load testing tool against the sample
server in [`test/PerformanceTests/SerializationComparisonTests/TestServer`](../../test/PerformanceTests/SerializationComparisonsTests/TestServer/) to compare the throughput and latency of different writers when running concurrent requests
on localhost.

`JsonSerializer`:

```sh
bombardier -l -d 10s https://localhost:7120/customers/JsonSerializer
```

```sh
Statistics        Avg      Stdev        Max
  Reqs/sec      5027.29    2090.85   14012.25
  Latency       25.09ms    12.55ms   396.30ms
  Latency Distribution
     50%    22.01ms
     75%    27.90ms
     90%    37.62ms
     95%    46.48ms
     99%    72.33ms
  HTTP codes:
    1xx - 0, 2xx - 49863, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:   202.11MB/s
```

**`NewODataSerializer`**:

```sh
bombardier -l -d 10s https://localhost:7120/customers/NewODataSerializer
```

```sh
Statistics        Avg      Stdev        Max
  Reqs/sec      1925.23    1678.85   11420.68
  Latency       66.20ms    32.69ms   601.98ms
  Latency Distribution
     50%    57.93ms
     75%    81.98ms
     90%    98.72ms
     95%   122.86ms
     99%   266.88ms
  HTTP codes:
    1xx - 0, 2xx - 18911, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:    76.48MB/s
```

**`ODataMessageWriter-Async`**

```sh
bombardier -l -d 10s https://localhost:7120/customers/ODataMessageWriter-Async
```

```sh
Statistics        Avg      Stdev        Max
  Reqs/sec      1183.38    1402.75   18027.04
  Latency      119.10ms    85.04ms      1.14s
  Latency Distribution
     50%    85.57ms
     75%   103.60ms
     90%   207.79ms
     95%   386.40ms
     99%   753.66ms
  HTTP codes:
    1xx - 0, 2xx - 10538, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:    32.74MB/s
```

## Principles

### Performance as a feature

Performance is a key concern of this proposal and should guide design decisions.
Though we should strike a careful balance.
When deciding between convenience and performance, we may prioritize performance. Prefer
to build a convenience layer on top of a performance layer such that users may
have an option to sacrifice the convenience if they need more performance. This
also means that in some cases we may sacrifice some common coding practices in favour
of performance. For example, we may need to maintain multiple implementations of similar
functionality in order to tune performance for different scenarios.

In general, we should not shy away (safe) performance-oriented features in .NET
(e.g. spans, structs, SIMD, array pools, stackallocs, code gen, etc.)

### Flexibility

We strive to offer at least a similar level of flexibility as the current OData writer,
to offer access to similar features even though they may be arrived at differently.
For example, we should support:

- Different input types (CLR objects, EdmObject, Dictionaries, JObject)
- Different types of payloads: resource set, values, aggregation results, service document, raw values, metadata document, etc.
- Dynamic properties, delta payloads, stream properties, polymorphic properties, $select/$expand
- Standard OData annotations, custom annotations, etc.
- Streaming support (start serialization before all data is available)

We also aim to expose different layers of abstractions to support different use cases. We aim to provide
built-in implementations for common use cases while still providing different layers where users can customize
or replace components to suit their unique scenarios or achieve better performance.

### Don't force users to allocate intermediate data unnecessarily

A lot of the cost of the existing writer stems from computing intermediate data (e.g. `ODataResource`, Uris, etc.).
We want to avoid a design that requires the user to allocate intermediate objects during serialization in
scenarios where it should be possible to accomplish the same action without allocation.

If we provide a convenient API that requires allocation, it should be possible for the user to use an alternate API if they
want to avoid the allocation without having to re-write significant portions of the writer.

The proposed design will provide convenient implementations that require allocation of components, but we'll try
to limit the allocations to components that are created either once per request or singletons that are re-used
throught the lifetime of the process (e.g providers, handlers, context objects, etc.). And even in this scenarios, we should strive to make it possible for determined
users to bypass these allocations with custom implementations.

As part of this principle, we also want to avoid boxing user inputs. Therefore we will rely on the extensive use of generics and code generation
instead of reflection.

Another example of this principle in practice is that we should either avoid assuming specific input types, and when do for convenience,
we should provide alternatives that do not. For example, in the current writer we create `Uri` objects to represent things like
next links, OData ids, etc. But we may be able to serialize links, ids, context urls, etc. without explicitly allocating `Uri` objects.

Another example, if we except some value to be written as a string, we may expose an API with as signature like `WriteValue(string value)`.
While this may be fine in most cases, it might force allocations or unnecessary computations for scenarios where the input source
is stored different. For example, if the user's input is in a UTF8 `ReadOnlySpan<byte>` we force the user to transcode that value
and allocate a `string`. If the value exists within a subset of larger `string`, we force the user to allocate a substring to get the value.
If the intent of this API is to write the value, we should consider exposing the writer to the user and allow the user to extract the value
in the most efficient way possible.

### Don't pay for what you don't use

Simply put, we don't want users to pay a (considerable) performance overhead for features they don't use or need.
If a feature is on by default, but not required, and it has some noticeable overhead, then it should be possible
to avoid that feature and subsequently remove the overhead.

If a non-default feature has some performance overhead, then users who have not opted in to the feature should
not experience the performance overhead related to the feature.

In the existing writer there are many cases where users pay for things that they do not use (e.g. some things
are only applicable in full-metadata scenarios but everyone pays the cost by default). In some cases
we are very defensive that we add expensive validations for scenarios that are not likely to occur in practice,
or that can be avoided but are hard to turn off.

In the proposed design, every cost we add should be justified and measured, and ideally there should be an alternative
that avoids that cost. The more like the cheaper alternative is to be adopted, the easier we should make it to adopt,
including providing built-in implementations for such scenarios.

Let's consider a possible implementation of some feature:

```csharp
void ExecuteSomeFeature()
{
    DoSomeWork();
    PerformExpensiveOperationThatIsOnlyValidInSomeScenarios();
    DoSomeMoreWork();
}
```

If the expensive operation is not always valid, we can add some checks to see if the operation is required:

```c#
void ExecuteSomeFeature()
{
    DoSomeWork();
    if (IsOperationRequired())
    {
        PerformExpensiveOperationThatIsOnlyValidInSomeScenarios();
    }
    
    DoSomeWork();
}
```

In this case `IsOperationRequired()` is assumed to be cheaper than the expensive operation.
For example escape some characters, we can first check if we have any character that needs to be escaped,
so we don't allocate any memory if there's nothing to be escaped.

This means we have reduced the memory cost for cases where there's no escaping needed. However,
now the cost of `IsOperationRequired()` is still paid by scenarios that don't require escaping,
and this cost is added to those scenarios that require escaping.

For users who know that their data is already pre-escaped, we could consider adding a setting

```csharp
void ExecuteSomeFeature(Options options)
{
    DoSomeWork();
    if (options.OperationEnabled)
    {
        if (IsOperationRequired())
        {
            PerformExpensiveOperationThatIsOnlyValidInSomeScenarios();
        }
    }
    
    DoSomeWork();
}
```

In this case, the checking the boolean flag is a much a cheaper option that computing `IsOperationRequired()`.
And since this setting is likely to be the same each time this method is called in the current session,
it could benefit from caching by the runtime or CPU branch predictor.

But we could go a step further and create separate implementations:

```csharp
void ExecuteSomeFeature_Impl1()
{
    DoSomeWork();
    DoSomeWork();
}

void ExecuteSomeFeature_Impl2()
{
    DoSomeWork();
    PerformExpensiveOperationThatIsOnlyValidInSomeScenarios();
    DoSomeWork();
}
```

In this case we can make stronger assumptions in each implementation and leverage
these assumptions for more effective performance tuning.
But we'll pay the cost of maintaining separate implementations and keeping
them in sync. So we'll have to evaluate where such measures are worthwhile.

And finally, we could leverage code generate, e.g. using source generators,
to generate different specialized implementations where necessary.

## State, Context and global configuration

This design proposes 3 layers of configuration with different scopes. 

- Global configuration refers to components that can be reused throughout multiple requests, like custom resource handlers or providers. These will usually be registered as singletons in the service.
- **Context** refers to configuration that reused across a single serialization request. A new context will be created for each request. But the context is not expected to be modified by the serializer during the request. The context may includes data like the `ODataUri` of the request, the payload kind of the request, etc. This is similar to the `OutputContext` in the existing writier implementation.
- **State** refers to data that is available throughout the request, but can be modified by the request. This may track things like the EDM type of the current object being written, the recursion depth, etc. This is similar to the `Scope` in the current writer.

In this architecture, we assume the type of the context and state are dependent on the format and specific writer implementation. Therefore, at the lowest levels,
they'll be represented using generic types `TContext` and `TState` respectively. They will be passed as arguments to most methods and exposed to APIs that
users customize or override to ensure that contextual data is properly propagated. We do not defined a generic type of the global configuration because
we expect that the context will also store references to the global dependencies.

In a previous iteration, I only had a type to represent the context `TContext` and the state was assumed to be exposed through the context. This is still an option
I'm considering. Removing one generic type from declarations may make the APIs simpler to work with and extend.

## Low-level `IODataWriter`

At the lowest level, we defined the `IODataWriter` interface as follows:

```csharp
interface IODataWriter<TValue, TState, TContext>
{
    ValueTask WriteValueAsync(TValue value, TState state, TContext context);
}
```

This is the most basic representation of an OData writer. It's generic so we can create specialized
writers for different types of values. This means that in practice we'll instantiate a different
writer per type. `TValue` may represent a primitive type, a custom CLR class or struct,
a `JsonElement`, a `Dictionary<K, V>`, a `Stream`, etc.

It's also format-agnostic. The same interface will be used for writing not only JSON payloads, but also
XML payloads (e.g. `/$metadata`) and raw values (`/$value` endpoints). For different types of formats,
we would defined different `TState` and `TContext` types.

## JSON context and state

In this iteration of the design, we'll focus on JSON payloads. The library will define built-in
concrete `TContext` and `TState` types to be used for JSON payloads:
`ODataJsonWriterContext` and `ODataJsonWriterStack`.

The `ODataJsonWriterContext` will store references to the `IEdmModel`, `ODataUri`,
global options, handlers and providers. It will also provides a reference to the
actual `Utf8JsonWriter` (and possibly also the output `Stream`?).

The `ODataJsonWriterStack` is a stack where a stack frame is pushed when we
enter a nested scope. The stack frames stores a reference to the current `IEdmType`,
`SelectExpandClause`, etc.

Therefore, built-in OData JSON writers will implement the following interface:

```csharp
interface IODataJsonWriter<TValue> : IODataWriter<TValue, ODataJsonWriterStack, ODataJsonWriterContext>
{
    ValueTask WriteValueAsync(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context);
}
```

### Adding custom data to state and context

The user may want to add custom information to either the state or context, such that it's available
to them in their custom handlers or hooks. Being able to add custom state may be useful in cases where
the user needs to access some external information in order to implement custom serialization logic.

If the custom data is not dynamic, then they can store that state in a custom class that implements their handlers.
But if it's dynamic such that it changes each time the handler is invoked, it may be better to include it in the state,
otherwise they user may have to create closures.

Since the built-in JSON writer will assume the use of specific context and writer stack, we can
consider making them extensible either through inheritance or constrained generics. I'll go with the latter
since it avoid type casts (it also makes it possible to define the types as structs).

```csharp
interface IODataJsonWriter<TValue, TState, TContext> : IODataWriter<TValue, TState, TContext>
    where TState: IODataJsonWriterStack, TContext: IODataJsonWriterContext
{
    ValueTask WriteValueAsync(TValue value, TState state, TContext context);
}
```

Since this leads to more convoluted type declaration, I'll use the concrete context and state
types moving forward to make the design easier to read.

## Built-in OData conventions with customization through handlers, providers and overrides

We don't expect customers to implement the `IODataWriter` interface directly so that would
require them to implement the OData specifications and conventions on their own, which
defeats the purpose of the library.

The library will provide default base implementations that take care of OData conventions
and specifications so that users don't have to deal with such details. For example,
these default implementations will know when how to write context urls, when to write
annotations, how to select which properties to write based on the model and SelectExpand, etc.

Such base implementations should be foundational and flexible enough that users don't need
to create custom implementations for their unique scenarios, they should provide extensibility
points that allow the user to customize behaviour or specialize them for their data types.

We'll generally expose these extensibility points through handlers and inheritance:

A handler is an interface that exposes a specific, granular piece of functionality. For example,
we may have a next link handler that knows how to write next links, a count handler that knows
how to write the `@odata.count` property, a property writer handler that knows how to write
properties of a given resource type, etc.

The user may customize such functionality by injecting a custom implementation of a given handler.
The handlers will be stored in a provider that is made available to the writer's context. A
provider is a like a light-weight dependency injection mechanism with a narrow scope (e.g. next handler provider
only provides next handlers). I opted against an `IServiceProvider` to avoid dependencies but also to make
it clear to the caller and consumer what kinds of services can be requested. An `IServiceProvider` is too generic.

With this in mind, we can conceptualize a base resource writer that handles structured types (entities and complex types)
as follows:

```c#
class ODataResourceJsonWriter<T> : IODataJsonWriter<T, ODataJsonWriterStack, ODataJsonWriterContext>
{
    public virtual async ValueTask WriteAsync(T value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var jsonWriter = context.JsonWriter;
        jsonWriter.WriteStartObject();

        if (context.MetadataLevel >= ODataMetadataLevel.Minimal)
        {
            // metadata writer is handler that combines
            // multiple metadata handlers. Can be replaced with custom handlers.
            var metadataWriter = context.GetMetadataWriter<T>(state);
            if (context.IsTopLevel())
            {
                await metadataWriter.WriterContextUrl(value, state, context);
            }
            
            await metadataWriter.WriteEtagPropertyAsync(value, state, context);
        }

        await WriteProperties(value, state, context);

        jsonWriter.WriteEndObject();
    }

    protected virtual async ValueTask WriteProperties(T value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var edmType = state.Current.EdmType as IEdmStructuredType;
        var selectExpand = state.Current.SelectExpandClause;

        var propertyWriter = context.GetPropertyWriter<T>(state);

        if (selectExpand == null || selectExpand.AllSelected == true)
        {
            foreach (var property in edmType.StructuralProperties())
            {
                await WriteProperty(propertyWriter, value, property, null, state, context);
            }
        }
        else
        {
            foreach (var item in selectExpand.SelectedItems)
            {
                if (item is PathSelectItem pathSelectItem)
                {
                    var propertySegment = pathSelectItem.SelectedPath.LastSegment as PropertySegment;
                    var property = propertySegment.Property;
                    await WriteProperty(propertyWriter, value, property, pathSelectItem.SelectAndExpand, state, context);
                }
            }

        }

        if (selectExpand != null)
        {
            foreach (var item in selectExpand.SelectedItems)
            {
                if (item is ExpandedNavigationSelectItem expandedItem)
                {
                    var propertySegment = expandedItem.PathToNavigationProperty.LastSegment as NavigationPropertySegment;
                    var property = propertySegment.NavigationProperty;
                    await WriteProperty(propertyWriter, value, property, expandedItem.SelectAndExpand, state, context);
                }
            }
        }

        // TODO: handle dynamic properties
    }

    protected virtual async ValueTask WriteProperty(
        IResourcePropertyWriter<T, IEdmProperty, ODataJsonWriterStack, ODataJsonWriterContext> propertyWriter,
        T resource,
        IEdmProperty property,
        SelectExpandClause selectExpand,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        if (property.Type.IsStructured() || property.Type.IsStructuredCollection())
        {             
            var nestedState = new ODataJsonWriterStackFrame
            {
                SelectExpandClause = selectExpand,
                EdmType = property.Type.Definition
            };

            state.Push(nestedState);
            await propertyWriter.WriteProperty(resource, property, state, context);
            state.Pop();
        }
        else
        {
            await propertyWriter.WriteProperty(resource, property, state, context);
        }
    }
}
```

And we could customize them as follows:

```c#
options.AddCountHandler<IEnumerable<Order>>((orders, state, context) => orders.Count());
```

The handler approach allows users to customize individual capabilities without having to worry
about overall writer. It also makes it possible for different implementations to re-use the same
providers and handlers. It provides flexibility in how handlers can be implemented.

However this approach has some problems:

- If we want to customize multiple things, we have to create and register multiple handlers for the same resource type. This can lead to messy code.
- Each time we want to invoke a handler, we have to fetch it from the relevant provider. The provider will most likely have an internal cache backed by a `ConcurrentDictionary`. This means that we have to perform cache lookups for each invocation, and do this for multiple handlers. This cost adds up and we should minimize/avoid it.

An alternative to handlers is to create a base class we expose granular virtual methods that handle specific concerns. The user can create a child class
and override the methods they want to customize. This is similar to how we handle binders in AspNetCoreOData.

## Metadata and standard annotations

## Custom annotations

## Dynamic properties

## Streaming

## Inheritance and polymorphic payloads

## Resource sets

## Primitive types

## Primitive collections

## `IEdmModel` and `ODataUri` coupling and concerns

The default context and base writer implementations in the sample implementation are coupled to `IEdmModel` and `ODataUri`. This tight coupling may be cause of concern
in that if a customer does not have these types available or does not want to use them, they would not be able to use the base abstractions that implement
OData spec and conventions, they would have to implement these conventions on their own (e.g. handling `$select`/`$expand`, annotations, etc.).

On the other hand, in order to have built-in implementations of OData standard and conventions, we do make a few assumptions and have some standard ways
of representing types, properties, navigation links, which properties should be included in the response, etc. If we do not use the existing `IEdmModel`
and `ODataUri` types, we may end up creating alternative interfaces that play a similar role. This might lead to redundant effort that still end up
requiring users to handle OData-specific types and constructs in order to leverage our built-in base implementations. But this path is still worth exploring
to evaluate how far it can go before it becomes impractical.

Another concern with the coupling the default context with `IEdmModel` et al is the cost of using the Edm library on performance.
Working on the sample implementation, I was able to reduce the runtime and allocations within the new serializer by ~20% simply by avoiding looping over `edmType.Properties()` and `edmType.StructuredPropertypes()` in favor of computing a concrete ToList() and caching the results. Of course it would be better if we didn't have to pay the cost of this cache, but that would require changing the implementation `edmType.Properties()`, potentially with breaking changes. And in the past [we have also demonstrated](https://github.com/OData/odata.net/blob/eb47505f4ba406d37e0d347fc370cf1a2e5bcbe6/docs/uri-parser-rewriter-proposal.md) that `ODataUri` parser's performance could be considerably
improved with a different architecture would require breaking changes.

Therefore, if foundational built-in base writers do assume `IEdmModel` and `ODataUri`, I'd propose committing to also making these components more efficient even if some breaking or design changes might be required down the line, otherwise they'd become a bottleneck to the performance improvement effort. I will explore implementing the base writers such that these interfaces could be swapped without having to reimplement OData conventions manually. But as a start, I will proceed with these coupled implementations (unless there's strong opposition or better concrete alternatives) then create another layer below this in the future that can be is less loosely coupled, without breaking changes.

## Type mapping

## Serializing different types of sources

## Serializing other formats

## How to handle async

Since we have to account for async I/O, the `Write*` methods return `ValueTask`. But we expect most of the writes to be buffered. `Utf8JsonWriter` Write* methods are all synchronous and buffered and only performs I/O when you explicitly call `FlushAsync()`. So in most cases we'll be calling async methods that do only synchronous writes.
And in many cases we're writing small values that we don't need to flush. The fact that we return `ValueTask` instead of `Task` reduces the cost of async
as it doesn't allocate to the heap.

It's also possible that handlers perform asynchronous work besides writing to the output, e.g. some handlers may need to perform async work to read the input.

However, there could be scenarios where no async work is performed, but the caller can't assume that an async method does not return async work. So have to await
all async methods, even in cases where their implementation never performs async work.

If we always await methods even when the underlying implementation is synchronous, does this become a case of paying a cost you don't benefit from?

- What is the cost of the running the state machine (if the implementation is synchronous, the cost should be negligible)
- What about `ref structs` like `Span<T>`? Using `async`/`await` means we cannot use them in async method signatures.

Options:

- Keep everything async
- Support different implementations
- How does `JsonSerializer.SerializeAsync()` implement its re-entrant writing to flush occasionally?

## Roll-out

- Add preview to ODL 9 with support for
- The new writer will be an optional alternative, will not replace the existing one.
- How do we name it?
- Long term plan?