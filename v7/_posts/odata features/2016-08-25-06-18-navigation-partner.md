---
layout: post
title: "Navigation property partner"
description: ""
category: "5. OData Features"
---

The library supports setting and retrieving [partner](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part3-csdl/odata-v4.0-errata03-os-part3-csdl-complete.html#_Toc453752541) information of navigation properties.

The following APIs can be used to set partner information:

{% highlight csharp %}
public class EdmEntityType
{
    ...
    public void SetNavigationPropertyPartner(
        EdmNavigationProperty navigationProperty,
        IEdmPathExpression navigationPropertyPath,
        EdmNavigationProperty partnerNavigationProperty,
        IEdmPathExpression partnerNavigationPropertyPath);
    public EdmNavigationProperty AddBidirectionalNavigation(
        EdmNavigationPropertyInfo propertyInfo,
        EdmNavigationPropertyInfo partnerInfo);
    ...
}
{% endhighlight %}

The former is general-purpose while the latter is a convenient shortcut for certain scenarios. Let's look at the first method. To use this method, you must first have a top-level navigation property `navigationProperty` already defined on the entity type, and another possibly nested navigation property `partnerNavigationProperty` already defined on the target entity type. This method then sets the partner information of the two navigation properties. Since `navigationProperty` is a top-level navigation property defined on the entity type on which the method is invoked, `navigationPropertyPath` is simply the name of the navigation property. `partnerNavigationProperty`, on the other hand, could be defined on either an entity or complex type. `partnerNavigationPropertyPath` specifies the path to the partner navigation property on the target entity type. For example, if the partner navigation property is called `InnerNav` which is defined on a complex-typed property `ComplexProp` which is a top-level structural property defined on the target entity type, then `partnerNavigationPropertyPath` should be passed something like `new EdmPathExpression("ComplexProp/InnerNav")`. According to the spec, the partner information must not be set for navigation properties defined on a complex type. So, in this case, partner information will only be set on `navigationProperty`, but not `partnerNavigationProperty`. Let's give another example, say the partner navigation property is called `Nav` which is a top-level navigation property defined on the target entity type. Then `partnerNavigationPropertyPath` should be passed something like `new EdmPathExpression("Nav")`. In this case, the partner information will be set for both `navigationProperty` and `partnerNavigationProperty`.

The second method is a shortcut for the case when both navigation properties are top-level properties defined on the entity types. It doesn't work for the case when one navigation property is defined on a complex type, and thus a nested property on the entity type. It doesn't require you to define the navigation properties first; it will do this on your behalf. In effect, it will first add the two navigation properties on the entity types, and then set their partner information.

The following APIs are provided to retrieve the partner information of a navigation property:

{% highlight csharp %}
public interface IEdmNavigationProperty
{
    ...
    IEdmNavigationProperty Partner { get; }
    ...
}

public static class ExtensionMethods
{
    ...
    public static IEdmPathExpression GetPartnerPath(
        this IEdmNavigationProperty navigationProperty);
    ...
}
{% endhighlight %}

The first is used to retrieve the partner navigation property itself, while the second is used to retrieve the path to the partner navigation property within the target entity type.
