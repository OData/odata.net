---
layout: post
title: "2.3 Define entity relations"
description: "Define entity relations using EdmLib APIs"
category: "2. EdmLib"
---

Entity relations are defined by **navigation properties** in entity data models. Adding a navigation property to an entity type using EdmLib APIs is as simple as adding a structural property shown in the previous sections. EdmLib APIs support adding navigation properties targetting some entity set in the entity container as well as **contained** entity set belonging to some specific navigation property.

This section shows how to define navigation properties using EdmLib APIs. We will continue to use and extend the sample from the previous sections.

### Add a Navigation Property *Friends*
In the **SampleModelBuilder.cs** file, insert the following code into the `SampleModelBuilder` class:

{% highlight csharp %}
namespace EdmLibSample
{
    public class SampleModelBuilder
    {
        ...
        private EdmNavigationProperty _friendsProperty;
        ...
        public SampleModelBuilder BuildCustomerType()
        {
            ...
            _customerType.AddStructuralProperty("Address", new EdmComplexTypeReference(_addressType, isNullable: false));
#region     !!!INSERT THE CODE BELOW!!!
            _friendsProperty = _customerType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    ContainsTarget = false,
                    Name = "Friends",
                    Target = _customerType,
                    TargetMultiplicity = EdmMultiplicity.Many
                });
#endregion
            _model.AddElement(_customerType);
            return this;
        }
        ...
        public SampleModelBuilder BuildCustomerSet()
        {
            _customerSet = _defaultContainer.AddEntitySet("Customers", _customerType);
#region     !!!INSERT THE CODE BELOW!!!
            _customerSet.AddNavigationTarget(_friendsProperty, _customerSet);
#endregion
            return this;
        }
        ...
    }
}
{% endhighlight %}

This code:

 - Adds a navigation property `Friends` to the entity type `Customer`;
 - Sets the `ContainsTarget` property to `false` since this property has **no contained entities** and targets one or more `Customer` entites in the entity set `Customers`;
 - Sets the `TargetMultiplicity` property to `EdmMultiplicity.Many` indicating that one customer can have **many** orders. Other possible values are `ZeroOrOne` and `One`;
 <br />
 
### Add an Entity Type *Order* and an Entity Set *Orders*
Just as how we added the entity set `Customers`, we first add an entity type `Order` and then the entity set `Orders`.

In the **SampleModelBuilder.cs** file, insert the following code into the `SampleModelBuilder` class:

{% highlight csharp %}
namespace EdmLibSample
{
    public class SampleModelBuilder
    {
        ...
        private EdmEntityType _orderType;
        ...
        private EdmEntitySet _orderSet;
        ...
        public SampleModelBuilder BuildOrderType()
        {
            _orderType = new EdmEntityType("Sample.NS", "Order");
            _orderType.AddKeys(_orderType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            _orderType.AddStructuralProperty("Price", EdmPrimitiveTypeKind.Decimal);
            _model.AddElement(_orderType);
            return this;
        }
        ...
        public SampleModelBuilder BuildOrderSet()
        {
            _orderSet = _defaultContainer.AddEntitySet("Orders", _orderType);
            return this;
        }
        ...
    }
}
{% endhighlight %}

In the **Program.cs** file, insert the following code into the `Main` method:

{% highlight csharp %}
namespace EdmLibSample
{
    class Program
    {
        public static void Main(string[] args)
        {
            var builder = new SampleModelBuilder();
            var model = builder
                .BuildAddressType()
                .BuildCategoryType()
#region         !!!INSERT THE CODE BELOW!!!
                .BuildOrderType()
#endregion
                .BuildCustomerType()
                .BuildDefaultContainer()
#region         !!!INSERT THE CODE BELOW!!!
                .BuildOrderSet()
#endregion
                .BuildCustomerSet()
                .GetModel();
            WriteModelToCsdl(model, "csdl.xml");
        }
    }
}
{% endhighlight %}

### Add Navigation Properties *Purchases* and *Intentions*
In the **SampleModelBuilder.cs** file, insert the following code into the `SampleModelBuilder` class:

{% highlight csharp %}
namespace EdmLibSample
{
    public class SampleModelBuilder
    {
        ...
        private EdmNavigationProperty _purchasesProperty;
        private EdmNavigationProperty _intentionsProperty;
        ...
        public SampleModelBuilder BuildCustomerType()
        {
            ...
#region     !!!INSERT THE CODE BELOW!!!
            _purchasesProperty = _customerType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    ContainsTarget = false,
                    Name = "Purchases",
                    Target = _orderType,
                    TargetMultiplicity = EdmMultiplicity.Many
                });
            _intentionsProperty = _customerType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    ContainsTarget = true,
                    Name = "Intentions",
                    Target = _orderType,
                    TargetMultiplicity = EdmMultiplicity.Many
                });
#endregion
            _model.AddElement(_customerType);
            return this;
        }
        ...
        public SampleModelBuilder BuildCustomerSet()
        {
            _customerSet = _defaultContainer.AddEntitySet("Customers", _customerType);
            _customerSet.AddNavigationTarget(_friendsProperty, _customerSet);
#region     !!!INSERT THE CODE BELOW!!!
            _customerSet.AddNavigationTarget(_purchasesProperty, _orderSet);
#endregion
            return this;
        }
    }
}
{% endhighlight %}

This code:

 - Adds a `Purchases` property targetting one or more **settled orders** in the entity set `Orders`;
 - Adds a `Intentions` property targetting a **contained entity set** of **unsettled orders** that should not be listed in the entity set `Orders`.
 <br />

### Run the Sample
Build and run the sample. Then open the **csdl.xml** file under the **output directory**. The content of **csdl.xml** should look like the following:

![]({{site.baseurl}}/assets/2015-04-18-csdl.png)

### References
[[Tutorial & Sample] Containment is Coming with OData V4](http://blogs.msdn.com/b/odatateam/archive/2014/03/13/containment-is-coming-with-odata-v4.aspx).