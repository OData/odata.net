---
layout: post
title: "Support untyped json in ODataLib and Client"
description: ""
category: "5. OData Features"
---

Starting from ODataV3 5.7.0, undeclared property is better supported by ODataLib and OData Client. ODataMessageReader is extended to be capabale of reading arbitrary JSON as raw string from the payload.

#### In ODataLib

The MessageReaderSettings used to accept ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty/.None as settings for reading undeclared value properties in payload, now it can accept a new setting value called ODataUndeclaredPropertyBehaviorKinds.SupportUndeclaredValueProperty. They correspond to the below behaviors:

* ODataUndeclaredPropertyBehaviorKinds.None : throws exception on undeclared property.
* ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty : skip undeclared property in payload.
* ODataUndeclaredPropertyBehaviorKinds.SupportUndeclaredValueProperty : read undeclared property as either an OData valid value instance or ODataUntypedValue instance.

What the undeclared property means are:

1. In open entity: a property whose name isn't defined in the model and whose value's type can't be inferred (or they will be valid dynamic property in open entity if the value's type can be determined).

2. In normal entity or complex: a property whose name isn't defined in the model.

The below messageWriterSettings will enable reading undeclared / untyped property. It reads an undeclared and untype JSON as ODataUntypedValue whose .RawJson has the raw JSON string.
{% highlight csharp %}

ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings
	    {
	        Version = ODataVersion.V3,
	        BaseUri = new Uri("http://example.com/"),
	        UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.SupportUndeclaredValueProperty
	    };
const string payload = @"{""odata.metadata"":""http://www.example.com/#Server.NS.container1.serverEntitySet/@Element"",""Id"":123,"
          ""undeclaredComplex1"":{""property1"":""aa"",""property2"":""bb""}}";
ODataEntry entry = ...; // read with messageWriterSettings
ODataUntypedValue undeclaredComplex1Value= entry.Properties.Single(s => string.Equals(s.Name, "undeclaredComplex1"))
        .Value as ODataUntypedValue; // here undeclaredComplex1Value.RawJson is string "{\"property1\":\"aa\",\"property2\":\"bb\"}"
	
{% endhighlight %}

#### In OData Client

DataServiceContext now has a new setting called UndeclaredPropertyBehavior.

* UndeclaredPropertyBehavior.None: it means respecting DataServiceContext's old IgnoreMissingProperty boolean, for backward compatibility.
* UndeclaredPropertyBehavior.Ignore: it overwrites DataServiceContext's old IgnoreMissingProperty boolean, means always skipping undeclared property.
* UndeclaredPropertyBehavior.Support: it overwrites DataServiceContext's old IgnoreMissingProperty boolean, means always reading undeclared property as either an OData valid value instance or ODataUntyped instance.

The below code demostrates UndeclaredPropertyBehavior.Support:

{% highlight csharp %}

var context = new DefaultContainer(new Uri("http://services.odata.org/v4/(S(lqbvtwide0ngdev54adgc0lu))/TripPinServiceRW/"));
context.Format.UseJson();
context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
context.Configurations.ResponsePipeline.OnEntryEnded += (ReadingEntryArgs e) =>
    {
        // undeclared property can be accessed here (with the above UndeclaredPropertyBehavior.Support) :
        ODataProperty property = e.Entry.Properties.Single(s => string.Equals(s.Name, "..."));

    };
var peopleQuery = from p in cxt.People select p;
var peopleList = peopleQuery.ToList();

{% endhighlight %}
