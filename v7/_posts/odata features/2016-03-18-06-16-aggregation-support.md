---
layout: post
title: "Basic Uri parser support for aggregations"
description: ""
category: "5. OData Features"
---

From ODataLib 6.15.0, we introduced the `basic` Uri parser support for aggregations, this is first step for us to support aggregations,  Issues and PR to make this support better is very welcome, details about aggregation in spec can be found [here](http://docs.oasis-open.org/odata/odata-data-aggregation-ext/v4.0/odata-data-aggregation-ext-v4.0.html).

### Aggregate
The aggregate transformation takes a comma-separated list of one or more aggregate expressions as parameters and returns a result set with a single instance, representing the aggregated value for all instances in the input set.

#### Examples

`GET ~/Sales?$apply=aggregate(Amount with sum as Total)`

`GET ~/Sales?$apply=aggregate(Amount with min as MinAmount)`

#### Open Issue
[#463](https://github.com/OData/odata.net/issues/463)

### Groupby
The groupby transformation takes one or two parameters and `Splits` the initial set into subsets where all instances in a subset have the same values for the grouping properties specified in the first parameter, `Applies` set transformations to each subset according to the second parameter, resulting in a new set of potentially different structure and cardinality, `Ensures` that the instances in the result set contain all grouping properties with the correct values for the group, `Concatenates` the intermediate result sets into one result set.

#### Examples

`GET ~/Sales?$apply=groupby((Category/CategoryName))`

`GET ~/Sales?$apply=groupby((ProductName), aggregate(SupplierID with sum as SupplierID))`

### Apply with other QueryOptions
Apply queryoption will get parse first and we can add filter, orderby, top, skip with apply.

#### Examples

`$apply=groupby((Address/City))&$filter=Address/City eq 'redmond'`

`$apply=groupby((Name))&$top=1`

`$apply=groupby((Address/City))&$orderby=Address/City`

### Test
All the support scenarios can be found in [WebAPI case](https://github.com/OData/WebApi/blob/master/OData/test/UnitTest/System.Web.OData.Test/OData/Query/ApplyQueryOptionTest.cs), [ODL case](https://github.com/OData/odata.net/blob/master/test/FunctionalTests/Microsoft.OData.Core.Tests/UriParser/Extensions/Binders/ApplyBinderTests.cs).
