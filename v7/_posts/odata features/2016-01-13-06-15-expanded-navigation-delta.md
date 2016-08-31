---
layout: post
title: "Expanded Navigation Property Support in Delta Response"
description: ""
category: "5. OData Features"
---

From ODataLib 6.15.0, we introduced the support for reading and writing expanded navigation properties (either collection or single) in delta responses. This feature is not covered by the current OData spec yet but the official protocol support is already in progress. As far as the current design, expanded navigation properties can **ONLY** be written within any `$entity` part of a delta response. Every time an expanded navigation property is written, the full expanded resource set or resource should be written instead of just the delta changes because in this way it's easier to manage the association among resources consistently. Inside the expanded resource set or resource, there are **ONLY** normal resource sets or resources. Multiple expanded navigation properties in a single `$entity` part is supported. Containment is also supported.

Basically the new APIs introduced are highly consistent with the existing ones for reading and writing normal delta responses so there should not be much trouble implementing this feature in OData services. This section shows how to use the new APIs.

### Write expanded navigation property in delta response
There are only four new APIs introduced for writing.

{% highlight csharp %}
public abstract class Microsoft.OData.ODataDeltaWriter
{
    ...
    public abstract void WriteStart (Microsoft.OData.ODataResourceSet expandedResourceSet)
    public abstract void WriteStart (Microsoft.OData.ODataNestedResourceInfo nestedResourceInfo) 
    public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.ODataResourceSet expandedResourceSet)
    public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.ODataNestedResourceInfo nestedResourceInfo)
    ...
}
{% endhighlight %}

The following sample shows how to write an expanded resource set (collection of resources) in a delta response. Please note that regardless of whether or not the nested resource info will be eventually written to the payload, `WriteStart(nestedResourceInfo)` **MUST** be called before actually calling `WriteStart(expandedResourceSet)` to write an expanded resource set. So is for a single expanded resource.

{% highlight csharp %}
ODataDeltaWriter writer = messageWriter.CreateODataDeltaWriter(customersEntitySet, customerType);
writer.WriteStart(deltaResourceSet);               // delta resource set
writer.WriteStart(customerResource);           // delta resource
writer.WriteStart(ordersNestedResourceInfo);    // nested resource info
writer.WriteStart(ordersResourceSet);              // normal expanded resource set
writer.WriteStart(orderResource);              // normal resource
writer.WriteEnd();  // orderResource
writer.WriteEnd(); // ordersResourceSet
writer.WriteEnd(); // ordersNestedResourceInfo
writer.WriteEnd(); // customerResource
writer.WriteEnd(); // deltaResourceSet
writer.Flush();

string payloadLooksLike =
    "{" +
        "\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
        "\"value\":" +
        "[" + // deltaResourceSet
            "{" + // customerResource
                "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                "\"ContactName\":\"Susan Halvenstern\"," +
                "\"Orders\":" + // ordersNestedResourceInfo
                "[" + // ordersResourceSet
                    "{" + // orderResource
                        "\"@odata.id\":\"http://host/service/Orders(10643)\"," +
                        "\"Id\":10643," +
                        "\"ShippingAddress\":" +
                        "{" +
                            "\"Street\":\"23 Tsawassen Blvd.\"," +
                            "\"City\":\"Tsawassen\"," +
                            "\"Region\":\"BC\"," +
                            "\"PostalCode\":\"T2F 8M4\"" +
                        "}" +
                    "}" +
                "]" +
            "}" +
        "]"+
    "}";
{% endhighlight %}

The next sample shows how to write a single expanded entity in a delta response.

{% highlight csharp %}
ODataDeltaWriter writer = messageWriter.CreateODataDeltaWriter(customersEntitySet, customerType);
writer.WriteStart(deltaResourceSet);                           // delta resource set
writer.WriteStart(customerResource);                       // delta resource
writer.WriteStart(productBeingViewedNestedResourceInfo);    // nested resource info
writer.WriteStart(productResource);                        // normal expanded resource
writer.WriteStart(detailsNestedResourceInfok);               // nested resource info
writer.WriteStart(detailsResourceSet);                         // nested expanded resource set
writer.WriteStart(productDetailResource);                  // normal resource
writer.WriteEnd(); // productDetailResource
writer.WriteEnd(); // detailsResourceSet
writer.WriteEnd(); // detailsNestedResourceInfo
writer.WriteEnd(); // productResource
writer.WriteEnd(); // productBeingViewedNestedResourceInfo
writer.WriteEnd(); // customerResource
writer.WriteEnd(); // deltaResourceSet
writer.Flush();

string payloadLooksLike =
    "{" +
        "\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
        "\"value\":" +
        "[" +
            "{" +
                "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                "\"ContactName\":\"Susan Halvenstern\"," +
                "\"ProductBeingViewed\":" +
                "{" +
                    "\"@odata.id\":\"http://host/service/Product(1)\"," +
                    "\"Id\":1," +
                    "\"Name\":\"Car\"," +
                    "\"Details\":" +
                    "[" +
                        "{" +
                            "\"@odata.type\":\"#MyNS.ProductDetail\"," +
                            "\"Id\":1," +
                            "\"Detail\":\"made in china\"" +
                        "}" +
                    "]" +
                "}" +
            "}" +
        "]" +
    "}";
{% endhighlight %}

### Some internals behind the writer
Though there is only one `WriteStart(resource)`, `ODataJsonLightDeltaWriter` keeps track of an internal state machine thus can correctly differentiate between writing a delta resource and writing a normal resource. Actually during writing the expanded navigation properties, all calls to `WriteStart(resource)`, `WriteStart(resourceSet)` and `WriteStart(nestedResourceInfo)` are delegated to an internal `ODataJsonLightWriter` which is responsible for writing normal payloads. And the control will return to `ODataJsonLightDeltaWriter` after the internal writer completes. The internal writer pretends to write a phony resource but will skip writing any structural property or instance annotation until it begins to write a nested resource info (means we are going to write an expanded navigation property).

### Read expanded navigation property in delta response
In the reader part, new APIs include a new state enum and a sub state property. All the other remains the same.

{% highlight csharp %}
public enum Microsoft.OData.ODataDeltaReaderState
{
    ...
    NestedResource = 10
    ...
}

public abstract class Microsoft.OData.ODataDeltaReader
{
    ...
    Microsoft.OData.Core.ODataReaderState SubState  { public abstract get; }
    ...
}
{% endhighlight %}

Note that the sub state is `ODataReaderState` which is used for normal payloads. The sub state is a complement to the main state in `ODataDeltaReader` to specify the detailed reader state within expanded navigation properties. But the sub state is **ONLY** available and meaningful when the main state is `ODataDeltaReaderState.NestedResource`. Users can still use the `Item` property to retrieve the current item being read out from an expanded payload.

The following sample shows the scaffolding code to read the expanded resource set and resource in a delta payload.

{% highlight csharp %}
ODataDeltaReader reader = messageReader.CreateODataDeltaReader(customersEntitySet, customerType);
while (reader.Read())
{
    switch (reader.State)
    {
        case ODataDeltaReaderState.DeltaResourceSetStart:
            // Start delta resource set
            ...
        case ODataDeltaReaderState.DeltaResourceSetEnd:
            // End delta resource set
            ...
        case ODataDeltaReaderState.DeltaResourceStart:
            // Start $entity (may be followed by an NestedResource)
            ...
        case ODataDeltaReaderState.DeltaResourceEnd:
            // End $entity
            ...
        case ODataDeltaReaderState.NestedResource:
            switch (reader.SubState)
            {
                case ODataReaderState.ResourceSetStart:
                    var resourceSet = reader.Item as ODataResourceSet;
                    ...
                case ODataReaderState.ResourceStart:
                    var resource = reader.Item as ODataResource;
                    ...
                case ODataReaderState.NestedResourceInfoStart:
                    var nestedResourceInfo = reader.Item as ODataNestedResourceInfo;
                    ...
                case ODataReaderState.Start:
                    // Start the expanded payload
                    ...
                case ODataReaderState.Completed:
                    // Finish the expanded payload
                    ...
                ...
            }
            break;
        ...
    }
}
{% endhighlight %}

### Some internals behind the reader
Just as the implementation of the writer, there is also an internal `ODataJsonLightReader` to read the expanded payloads. When the delta reader reads a navigation property (can be inferred from the model) in the `$entity` part of a delta response, it creates the internal reader for reading either top-level feed or top-level entity. For reading delta payload, there are some hacks inside `ODataJsonLightReader` to skip parsing the context URLs thus each `ODataResource` being read out has no metadata builder with it. Actually the expanded payloads are treated in the same way as the nested payloads when reading parameters. This is currently a limitation which means the service **CANNOT** get the metadata links from a normal entity in a delta response. The internal reader also skips the next links after an expanded resource set because the `$entity` part of a delta payload is non-pageable. When the internal reader is consuming the expanded payload, the delta reader remains at the `NestedResource` state until it detects the state of the internal reader to be `Completed`. However we still leave the `Start` and `Completed` states catchable to users so that they can do something before and after reading an expanded navigation property.
