---
layout: post
title: "Navigation property under complex type"
description: "Navigation property under complex type"
category: "5. OData Features"
---

Since OData V7.0, it supports to add navigation property under complex type. Basically navigation under complex are same with navigation under entity for usage, the only differences are: **1.** Navigation under complex can have multiple bindings with different path. **2.** Complex type does not have id, so the navigation link and association link of navigation under complex need contain the entity id which the complex belongs to. 

The page will include the usage of navigation under complex in EDM, Uri parser, and serializer and deserializer.

## 1. EDM ##

### Define navigation property to complex type ###
{% highlight csharp %}
EdmModel model = new EdmModel();

EdmEntityType city = new EdmEntityType("Sample", "City");
EdmStructuralProperty cityId = city.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
city.AddKeys(cityId);

EdmComplexType complex = new EdmComplexType("Sample", "Address");
complex.AddStructuralProperty("Road", EdmCoreModel.Instance.GetString(false));
EdmNavigationProperty navUnderComplex = complex.AddUnidirectionalNavigation(
    new EdmNavigationPropertyInfo()
    {
        Name = "City",
        Target = city,
        TargetMultiplicity = EdmMultiplicity.One,
    });

model.AddElement(city);
model.AddElement(complex);
{% endhighlight %}

Please note that only unidirectional navigation is supported, since navigation property must be entity type, so bidirectional navigation property does not make sense to navigation under complex type.

### Add navigation property binding for navigation property under complex ###
When entity type has complex as property, its corresponding entity set can bind the navigation property under complex to a specified entity set. Since multiple properties can be with same complex type, the navigation property under complex can have multiple bindings with different path.

A valid binding path for navigation under complex is: `[ qualifiedEntityTypeName "/" ] *( ( complexProperty / complexColProperty ) "/" [ qualifiedComplexTypeName "/" ] ) navigationProperty`

For example:
{% highlight csharp %}
EdmEntityType person = new EdmEntityType("Sample", "Person");
EdmStructuralProperty entityId = person.AddStructuralProperty("UserName", EdmCoreModel.Instance.GetString(false));
person.AddKeys(entityId);

person.AddStructuralProperty("Address", new EdmComplexTypeReference(complex, false));
person.AddStructuralProperty("Addresses", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(complex, false))));

model.AddElement(person);

var entityContainer = new EdmEntityContainer("Sample", "Container");
model.AddElement(entityContainer);
EdmEntitySet people = new EdmEntitySet(entityContainer, "People", person);
EdmEntitySet cities1 = new EdmEntitySet(entityContainer, "Cities1", city);
EdmEntitySet cities2 = new EdmEntitySet(entityContainer, "Cities2", city);
people.AddNavigationTarget(navUnderComplex, cities1, new EdmPathExpression("Address/City"));
people.AddNavigationTarget(navUnderComplex, cities2, new EdmPathExpression("Addresses/City"));

entityContainer.AddElement(people);
entityContainer.AddElement(cities1);
entityContainer.AddElement(cities2);
{% endhighlight %}

The navigation property `navUnderComplex` is binded to cities1 and cities2 with path `"Address/City"` and `"Addresses/City"` respectively.

Then the csdl of the model would be like:

    <Schema xmlns="http://docs.oasis-open.org/odata/ns/edm" Namespace="Sample">
        <EntityType Name="City">
            <Key>
                <PropertyRef Name="Name"/>
            </Key>
            <Property Name="Name" Nullable="false" Type="Edm.String"/>
        </EntityType>
        <ComplexType Name="Address">
            <Property Name="Road" Type="Edm.String" Nullable="false" />
            <NavigationProperty Name="City" Nullable="false" Type="Sample.City"/>
        </ComplexType>
        <EntityType Name="Person">
            <Key>
                <PropertyRef Name="UserName"/>
            </Key>
            <Property Name="UserName" Nullable="false" Type="Edm.String"/>
            <Property Name="Address" Nullable="false" Type="Sample.Address"/>
            <Property Name="Addresses" Nullable="false" Type="Collection(Sample.Address)"/>
        </EntityType>
        <EntityContainer Name="Container">
            <EntitySet Name="People" EntityType="Sample.Person">
                <NavigationPropertyBinding Target="Cities1" Path="Address/City"/>
                <NavigationPropertyBinding Target="Cities2" Path="Addresses/City"/>
            </EntitySet>
            <EntitySet Name="Cities1" EntityType="Sample.City"/>
            <EntitySet Name="Cities2" EntityType="Sample.City"/>
        </EntityContainer>
    </Schema>

The binding path may need include type cast. For example, if there is a navigation property `City2` defined in a complex type `UsAddress` which is derived from `Address`. If add a binding to `City2`, it should be like this:
`people.AddNavigationTarget(navUnderDerivedComplex, cities1, new EdmPathExpression("Address/Sample.UsAddress/City2"));`
Here we do not include type cast sample to keep the scenario simple.

### Find navigation target for navigation property under complex ###
Since a navigation property can be binded to different paths, the exact binding path must be specified for finding the navigation target. 
For example: 
{% highlight csharp %}
IEdmNavigationSource navigationTarget = people.FindNavigationTarget(navUnderComplex, new EdmPathExpression("Address/City"));
{% endhighlight %}
`Cities1` will be returned for this case.


## 2. Uri ##

### Query ###

Here lists some sample valid query Uris to access navigation property under complex:

#### Path ####

`http://host/People('abc')/Address/City`

Accessing navigation property under collection of complex is not valid, since item in complex collection does not have a canonical Url. That is to say, `http://host/People('abc')/Addresses/City` is not valid, `City` under `Addresses` can only be accessed through `$expand`.

#### Query option ####

Different with path, navigation under collection of complex can be accessed directly in expressions of $select and $expand, which means `Addresses/City` is supported. Refer [ABNF](http://docs.oasis-open.org/odata/odata/v4.0/errata02/os/complete/abnf/odata-abnf-construction-rules.txt) for more details.

	$select:  
	http://host/People('abc')/Address?$select=City
	http://host/People?$select=Address/City
	http://host/People?$select=Addresses/City
	
	$expand: 
	http://host/People('abc')/Address?$expand=City
	http://host/People?$expand=Address/City
	http://host/People?$expand=Addresses/City
	
	$filter:
	http://host/People?$filter=Address/City/Name eq 'Shanghai'
	http://host/People('abc')/Addresses?$filter=City/Name eq 'Shanghai'
	http://host/People?$filter=Addresses/any(a:a/City/Name eq 'Shanghai')
	
	$orderby:
	http://host/People?$order=Address/City/Name

### Uri parser ###
There is nothing special if using `ODataUriParser`. For `ODataQueryOptionParser`, if we need resolve the navigation property under complex to its navigation target in the query option, navigation source that the complex belongs to and the binding path are both needed. If the navigation source or part of binding path is in the path, we need it passed to the constructor of `ODataQueryOptionParser`. So there are 2 overloaded constructor added to accept `ODataPath` as parameter. 

{% highlight csharp %}
public ODataQueryOptionParser(IEdmModel model, ODataPath odataPath, IDictionary<string, string> queryOptions)
public ODataQueryOptionParser(IEdmModel model, ODataPath odataPath, IDictionary<string, string> queryOptions, IServiceProvider container)
{% endhighlight %}
Note: Parameter IServiceProvider is related to [Dependency Injection](http://odata.github.io/odata.net/v7/#01-04-di-support).

Actually we do not recommend to use `ODataQueryOptionParser` in this case, `ODataUriParser` would be more convenient. Here we still give an example just in case:

{% highlight csharp %}
// http://host/People('abc')/Address?$expand=City
ODataUriParser uriParser = new ODataUriParser(Model, ServiceRoot, new Uri("http://host/People('abc')/Address"));
ODataPath odataPath = uriParser.ParsePath();
ODataQueryOptionParser optionParser = new ODataQueryOptionParser(Model, odataPath, new Dictionary<string, string> { { "$expand", "City" } });
SelectExpandClause clause = optionParser.ParseSelectAndExpand();

// This can achieve same result.
uriParser = new ODataUriParser(Model, ServiceRoot, new Uri("http://host/People('abc')/Address?$expand=City"));
clause = uriParser.ParseSelectAndExpand();
{% endhighlight %}

## 3. Serializer (Writer) ##
Basically, the writing process is same with writing navigation under entity.
Let's say we are writing an response of query `http://host/People('abc')?$expand=Address/City`.

Sample code:

{% highlight csharp %}
var uriParser = new ODataUriParser(Model, ServiceRoot, new Uri("http://host/People('abc')?$expand=Address/City"));
var odataUri = uriParser.ParseUri();
settings.ODataUri = odataUri;// Specify the odataUri to ODataMessageWriterSettings, which will be reflected in the context url.

ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "UserName", Value = "abc" } } };
ODataNestedResourceInfo nestedComplexInfo = new ODataNestedResourceInfo() { Name = "Address" };
ODataResource nestedComplex = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Road", Value = "def" } } };
ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "City", IsCollection = false };
ODataResource nestednav = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Name", Value = "Shanghai" } } };

// Ignore code to CreateODataResourceWriter.

writer.WriteStart(res);
writer.WriteStart(nestedComplexInfo);
writer.WriteStart(nestedComplex);
writer.WriteStart(nestedResInfo);
writer.WriteStart(nestednav);
writer.WriteEnd();   // End of City
writer.WriteEnd();   // End of City nested info
writer.WriteEnd();// End of complex
writer.WriteEnd();// End of complex info
writer.WriteEnd();// End of entity
{% endhighlight %}

Payload:

    {
    "@odata.context": "http://host/$metadata#People/$entity",
    "UserName":"abc",
    "Address":
    {
       "Road":"def",
       "City":
       {
           "Name":"Shanghai"
       }
    }
    }

## 4. Deserializer (Reader) ##
Reading process is same with reading an navigation property under entity. For navigation property `City` under `Address`, it will be read as an `ODataNestedResourceInfo` which has navigation url `http://host/People('abc')/Complex/City` and an `ODataResource` which has Id `http://host/Cities1('Shanghai')`. 
