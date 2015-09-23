---
layout: post
title: "Abstract entity type support in .NET client"
description: ""
category: "6. OData Features"
---

OData Client for .NET supports abstract entity type without key from ODataLib 6.11.0.

# Create model with abstract entity type

{% highlight csharp %}
var abstractType = new EdmEntityType("DefaultNS", "AbstractEntity", null, true, false);
model.AddElement(abstractType);

var orderType = new EdmEntityType("DefaultNS", "Order", abstractType);
var orderIdProperty = new EdmStructuralProperty(orderType, "OrderID", EdmCoreModel.Instance.GetInt32(false));
orderType.AddProperty(orderIdProperty);
orderType.AddKeys(orderIdProperty);
model.AddElement(orderType);
{% endhighlight %}

# Output model

    <EntityType Name=""AbstractEntity"" Abstract=""true"" />
    <EntityType Name=""Order"" BaseType=""DefaultNS.AbstractEntity"">
      <Key>
        <PropertyRef Name=""OrderID"" />
      </Key>
    </EntityType>
      
# Client generated proxy file

{% highlight csharp %}
[global::Microsoft.OData.Client.EntityType()]
public abstract partial class AbstractEntity : global::Microsoft.OData.Client.BaseEntityType, global::System.ComponentModel.INotifyPropertyChanged
{
 ...
}

[global::Microsoft.OData.Client.Key("OrderID")]
[global::Microsoft.OData.Client.EntitySet("Orders")]
public partial class Order : AbstractEntity
{
 ...
}
{% endhighlight %}
