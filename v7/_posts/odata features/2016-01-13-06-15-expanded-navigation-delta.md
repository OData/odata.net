---
layout: post
title: "Expanded Navigation Property Support in Delta Response"
description: ""
category: "6. OData Features"
---

From ODataLib 6.15.0, we introduced the support for reading and writing expanded navigation properties (either collection or single) in delta responses. This feature is not covered by the current OData spec yet but the official protocol support is already in progress. As far as the current design, expanded navigation properties can **ONLY** be written within any `$entity` part of a delta repsonse. Every time an expanded navigation property is written, the full expanded feed or entity should be written instead of just the delta changes because in this way it's easier to manage the association among entities consistently. Inside the expanded feed or entity, there are **ONLY** normal feeds or entities. Multiple expanded navigation properties in a single `$entity` part is supported. Containment is also supported.

Basically the new APIs introduced are highly consistent with the existing ones for reading and writing normal delta responses so there should not be much trouble implementing this feature in OData services. This section shows how to use the new APIs.

### Write expanded navigation property in delta response
There are only four new APIs introduced for writing.

{% highlight csharp %}
public abstract class Microsoft.OData.Core.ODataDeltaWriter
{
    ...
    public abstract void WriteStart (Microsoft.OData.Core.ODataFeed expandedFeed)
    public abstract void WriteStart (Microsoft.OData.Core.ODataNavigationLink navigationLink) 
    public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.Core.ODataFeed expandedFeed)
    public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.Core.ODataNavigationLink navigationLink)
    ...
}
{% endhighlight %}

The following sample shows how to write an expanded feed (collection of entities) in a delta response. Please note that regardless of whether or not the navigation links will be eventually written to the payload, `WriteStart(navigationLink)` **MUST** be called before actually calling `WriteStart(expandedFeed)` to write an expanded feed. So is for a single expanded entity.

{% highlight csharp %}
ODataDeltaWriter writer = messageWriter.CreateODataDeltaWriter(customersEntitySet, customerType);
writer.WriteStart(deltaFeed);               // delta feed
writer.WriteStart(customerEntry);           // delta entity
writer.WriteStart(ordersNavigationLink);    // navigation link
writer.WriteStart(ordersFeed);              // normal expanded feed
writer.WriteStart(orderEntry);              // normal entity
writer.WriteEnd();  // orderEntry
writer.WriteEnd(); // ordersFeed
writer.WriteEnd(); // ordersNavigationLink
writer.WriteEnd(); // customerEntry
writer.WriteEnd(); // deltaFeed
writer.Flush();

string payloadLooksLike =
    "{" +
        "\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
        "\"value\":" +
        "[" + // deltaFeed
            "{" + // customerEntry
                "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                "\"ContactName\":\"Susan Halvenstern\"," +
                "\"Orders\":" + // ordersNavigationLink
                "[" + // ordersFeed
                    "{" + // orderEntry
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
writer.WriteStart(deltaFeed);                           // delta feed
writer.WriteStart(customerEntry);                       // delta entry
writer.WriteStart(productBeingViewedNavigationLink);    // navigation link
writer.WriteStart(productEntry);                        // normal expanded entry
writer.WriteStart(detailsNavigationLink);               // nested navigation link
writer.WriteStart(detailsFeed);                         // nested expanded feed
writer.WriteStart(productDetailEntry);                  // normal entry
writer.WriteEnd(); // productDetailEntry
writer.WriteEnd(); // detailsFeed
writer.WriteEnd(); // detailsNavigationLink
writer.WriteEnd(); // productEntry
writer.WriteEnd(); // productBeingViewedNavigationLink
writer.WriteEnd(); // customerEntry
writer.WriteEnd(); // deltaFeed
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
Though there is only one `WriteStart(entry)`, `ODataJsonLightDeltaWriter` keeps track of an internal state machine thus can correctly differentiate between writing a delta entry and writing a normal entry. Actually during writing the expanded navigation properties, all calls to `WriteStart(entry)`, `WriteStart(feed)` and `WriteStart(navigationLink)` are delegated to an internal `ODataJsonLightWriter` which is responsible for writing normal payloads. And the control will return to `ODataJsonLightDeltaWriter` after the internal writer completes. The internal writer pretends to write a phony entry but will skip writing any structural property or instance annotation until it begins to write a navigation link (means we are going to write an expanded navigation property).

### Read expanded navigation property in delta response
In the reader part, new APIs include a new state enum and a sub state property. All the other remains the same.

{% highlight csharp %}
public enum Microsoft.OData.Core.ODataDeltaReaderState
{
    ...
    ExpandedNavigationProperty = 10
    ...
}

public abstract class Microsoft.OData.Core.ODataDeltaReader
{
    ...
    Microsoft.OData.Core.ODataReaderState SubState  { public abstract get; }
    ...
}
{% endhighlight %}

Note that the sub state is `ODataReaderState` which is used for normal payloads. The sub state is a complement to the main state in `ODataDeltaReader` to specify the detailed reader state within expanded navigation properties. But the sub state is **ONLY** available and meaningful when the main state is `ODataDeltaReaderState.ExpandedNavigationProperty`. Users can still use the `Item` property to retrieve the current item being read out from an expanded payload.

The following sample shows the scaffolding code to read the expanded feeds and entries in a delta payload.

{% highlight csharp %}
ODataDeltaReader reader = messageReader.CreateODataDeltaReader(customersEntitySet, customerType);
while (reader.Read())
{
    switch (reader.State)
    {
        case ODataDeltaReaderState.DeltaFeedStart:
            // Start delta feed
            ...
        case ODataDeltaReaderState.FeedEnd:
            // End delta feed
            ...
        case ODataDeltaReaderState.DeltaEntryStart:
            // Start $entity (may be followed by an ExpandedNavigationProperty)
            ...
        case ODataDeltaReaderState.DeltaEntryEnd:
            // End $entity
            ...
        case ODataDeltaReaderState.ExpandedNavigationProperty:
            switch (reader.SubState)
            {
                case ODataReaderState.FeedStart:
                    var feed = reader.Item as ODataFeed;
                    ...
                case ODataReaderState.EntryStart:
                    var entry = reader.Item as ODataEntry;
                    ...
                case ODataReaderState.NavigationLinkStart:
                    var navigationLink = reader.Item as ODataNavigationLink;
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
Just as the implementation of the writer, there is also an internal `ODataJsonLightReader` to read the expanded payloads. When the delta reader reads a navigation property (can be inferred from the model) in the `$entity` part of a delta response, it creates the internal reader for reading either top-level feed or top-level entity. For reading delta payload, there are some hacks inside `ODataJsonLightReader` to skip parsing the context URLs thus each `ODataEntry` being read out has no metadata builder with it. Actually the expanded payloads are treated in the same way as the nested payloads when reading parameters. This is currently a limitation which means the service **CANNOT** get the metadata links from a normal entity in a delta response. The internal reader also skips the next links after an expanded feed because the `$entity` part of a delta payload is non-pageable. When the internal reader is consuming the expanded payload, the delta reader remains at the `ExpandedNavigationProperty` state until it detects the state of the internal reader to be `Completed`. However we still leave the `Start` and `Completed` states catchable to users so that they can do something before and after reading an expanded navigation property.
