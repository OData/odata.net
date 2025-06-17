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
// Create EDM model (same as before)
var model = new EdmModel();
// ... model setup code same as above ...

// Setup the writer providers
var propertyValueWriterProvider = new EdmPropertyValueJsonWriterProvider();

// Register a customer property writer that only writes $select'ed properties
propertyValueWriterProvider.Add<Customer>((customer, property, state, context) =>
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
});

// Register order writer 
propertyValueWriterProvider.Add<Order>((order, property, state, context) =>
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
});

// Create writer context
var writerContext = new ODataJsonWriterContext
{
    Model = model,
    ODataUri = odataUri,
    MetadataLevel = ODataMetadataLevel.Minimal,
    PayloadKind = ODataPayloadKind.ResourceSet,
    ODataVersion = ODataVersion.V4,
    JsonWriter = jsonWriter,
    ValueWriterProvider = new ResourceJsonWriterProvider(),
    MetadataWriterProvider = new JsonMetadataWriterProvider(new JsonMetadataValueProvider()),
    PropertyValueWriterProvider = propertyValueWriterProvider,
    ResourcePropertyWriterProvider = new EdmPropertyJsonWriterProvider()
};

// Setup writer stack 
var writerStack = new ODataJsonWriterStack();
writerStack.Push(new ODataJsonWriterStackFrame
{
    EdmType = new EdmCollectionType(
        new EdmEntityTypeReference(
            model.FindType("NS.Customer") as IEdmEntityType, 
            isNullable: false)),
    SelectExpandClause = odataUri.SelectAndExpand
});

// Write the payload
var odataWriter = new ODataResourceSetEnumerableJsonWriter<Customer>();
await odataWriter.WriteAsync(customers, writerStack, writerContext);
```

Notable improvements over the existing writer:

- No intermediate ODataResource or ODataProperty allocations required
- Properties are written directly from source objects without boxing
- Property writers only handle properties they know about, making $select efficient and reducing the need for validating that structural properties exist in the model
- Generic implementations avoid reflection and boxing of primitive values
- Reusable providers can be registered once and reused across requests
- Select/expand handling is built into the resource writer based on `SelectExpandClause`, but can be customized.

With additional abstractions, we can simplify the above code to something like

```csharp
var options = ODataSerializerOptions.CreateDefault();

class CustomerWriter : ODataResourceJsonWriter<Customer>
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


await ODataSerializer.WriteAsync(customers, model, ODataPayloadKing.ResourceSet, ODataVersion.V4, options);
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