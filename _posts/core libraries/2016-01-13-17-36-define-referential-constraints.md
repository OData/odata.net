---
layout: post
title: "2.11 Define referential constraints"
description: "Define referential constraints using EdmLib APIs"
category: "2. EdmLib"
---

Referential constraints ensure that entities being referenced always exist. In OData, having one or more referential constraints defined for a navigation property also enables users to address related entities using shortened key predicates (see [[OData-URL]](http://docs.oasis-open.org/odata/odata/v4.0/errata02/os/complete/part2-url-conventions/odata-v4.0-errata02-os-part2-url-conventions-complete.html#_Toc406398079)). A referential constraint in OData consists of one **principal property** (the ID property to reference another entity) and one **dependent property** (the ID property of the entity being referenced). This section shows how to define referential constraints for a navigation property.

### Sample
Create an entity type `Test.Customer` with a key property `id` of `Edm.String`.

{% highlight csharp %}
var model = new EdmModel();

var customer = new EdmEntityType("Test", "Customer", null, false, true);
var customerId = customer.AddStructuralProperty("id", EdmPrimitiveTypeKind.String, false);
customer.AddKeys(customerId);
model.AddElement(customer);
{% endhighlight %}

Create an entity type `Test.Order` with a composite key consisting of two key properties `customerId` and `orderId` both of `Edm.String`.

{% highlight csharp %}
var order = new EdmEntityType("Test", "Order", null, false, true);
var orderCustomerId = order.AddStructuralProperty("customerId", EdmPrimitiveTypeKind.String, true);
var orderOrderId = order.AddStructuralProperty("orderId", EdmPrimitiveTypeKind.String, true);
order.AddKeys(orderCustomerId, orderOrderId);
model.AddElement(order);
{% endhighlight %}

`Order.customerId` is the principal property while `Customer.id` is the dependent property. Create a navigation property `orders` with such a referential constraint.

{% highlight csharp %}
var customerOrders = customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
{
    ContainsTarget = true,
    Name = "orders",
    Target = order,
    TargetMultiplicity = EdmMultiplicity.Many,
    DependentProperties = new[] { customerId },
    PrincipalProperties = new[] { orderCustomerId }
});
{% endhighlight %}

Create an entity type `Test.Detail` with a composite key consisting of three key properties `customerId` of `Edm.String`, `orderId` of `Edm.String` and `id` of `Edm.Int32`.

{% highlight csharp %}
var detail = new EdmEntityType("Test", "Detail");
var detailCustomerId = detail.AddStructuralProperty("customerId", EdmPrimitiveTypeKind.String);
var detailOrderId = detail.AddStructuralProperty("orderId", EdmPrimitiveTypeKind.String);
detail.AddKeys(detailCustomerId, detailOrderId, detail.AddStructuralProperty("id", EdmPrimitiveTypeKind.Int32, false));
model.AddElement(detail);
{% endhighlight %}

Create an entity type `Test.DetailedOrder` which is a derived type of `Test.Order`. We will use this type to illustrate type casting in the middle of multiple navigation properties.

{% highlight csharp %}
var detailedOrder = new EdmEntityType("Test", "DetailedOrder", order);
{% endhighlight %}

Come back to the type `Test.Detail`. There are two referential constraints here:

 - `Detail.orderId` is the principal property while `DetailedOrder.orderId` is the dependent property.
 - `Detail.customerId` is the principal property while `DetailedOrder.customerId` is the dependent property.

Create a navigation property `details` with these two constraints.

{% highlight csharp %}
var detailedOrderDetails = detailedOrder.AddUnidirectionalNavigation(
    new EdmNavigationPropertyInfo
    {
        ContainsTarget = true,
        Target = detail,
        TargetMultiplicity = EdmMultiplicity.Many,
        Name = "details",
        DependentProperties = new[] { orderOrderId, orderCustomerId },
        PrincipalProperties = new[] { detailOrderId, detailCustomerId }
    });
model.AddElement(detailedOrder);
{% endhighlight %}

Please note that you should **NOT** specify `Customer.id` as the dependent property because the association (represented by the navigation property `details`) is from `DetailedOrder` to `Detail` rather than from `Customer` to `Detail`. And those properties must be specified **in the same order**.

Then you can query the `details` either by a full key predicate

```
http://host/customers('customerId')/orders(customerId='customerId',orderId='orderId')/Test.DetailedOrder/details(customerId='customerId',orderId='orderId',id=1)
```

or a shortened key predicate.

```
http://host/customers('customerId')/orders('orderId')/Test.DetailedOrder/details(1)
```

Key-as-segment convention is also supported.

```
http://host/customers/customerId/orders/orderId/Test.DetailedOrder/details/1
```