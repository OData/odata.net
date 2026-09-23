# Enable parsing JSON object and JSON array literals

## Related issues:

https://github.com/OData/odata.net/issues/3500
https://github.com/OData/odata.net/issues/2712
https://github.com/OData/odata.net/issues/2283
https://github.com/OData/odata.net/issues/1164

## Scenarios


OData has the following scenarios related to JSON object and JSON array literals:

1) 'in' operator

- ...$filter=Id in (1,2,3)
- ...$filter=Id in [1,2,3]
- ...?$filter=Name in ('Milk', 'Cheese')

2) function parameter

   a) In-Line
- ~/GetNearest(Point={"Lat":10,"Lon":20})
- ~/CalculateTotal(Items=[{"ID":1,"Qty":2},{"ID":2,"Qty":5}])
 
    b) With Alias
* ~/GetNearest(Point=@p)?@p={"Lat":10,"Lon":20}
* ~/CalculateTotal(Items=@p)?@p=[{"ID":1,"Qty":2},{"ID":2,"Qty":5}]

These literals are JSON object or JSON array string.

## Problems

The OData.net implementation so far parses the above literals as raw string literals. That's, there's no structure of data but raw string value. We are facing couple issues based on this implementation:
1) Cannot identify the syntax errors as early as possible
2) Cannot handle the nested JSON object or JSON array
3) Cannot use the raw string in collection scenarios since raw string is sinlge value node. for example, hassubset.
4) Cannot verify the content even with the metadata, for example, if using the wrong data for a property in JSON object.
5) ASP.NET Core OData or other consumer using ODL should have special logic to parse the raw string to understand the data. 

## Proposal

In tokenization process, add two new token classes to hold the JSON object and JSON array literal.

1) <strong>CollectionLiteralToken</strong>
   
   It's a collection of QueryToken, use it to hold the JSON array literal. Each item (separate using comma) is parsed as QueryToken and saved as an item into the CollectionLiteralToken.


2) <strong>ResourceLiteralToken</strong>
   
   It's a collection of Key/Value pairs, use it to hold the JSON objet literal.
   Each key/value pair (separte using comma) is parsed as QueryToken pairs and saved as a property into ResourceLiteralToken.

   The key and the value are QueryToken. But, for simplicity, we can use string as the key since it's the property name. 


In metadata binding process of current ODL implementation, there're two classes existed:

1) <strong>ConstantNode</strong>

   It's a node to hold the primitive literal value or enum literal value. But, in the existing implementation, it's temporarly used to hold the raw string of JSON object and JSON array. We should fix it.

2) <strong>CollectionConstantNode</strong>
   
   It's a node to hold the collection of contant. We can update it to hold the JSON array literals.

We need to add a new AST node class to hold the JSON object literal. 

3) <strong>ResourceConstantNode</strong>

   It's a new added class to hold the JSON object literal. It contains a collection of Key/value paris, The key is string (or ContantNode with string value), the value is either a ConstantNode, CollectionConstantNode, or ResourceConstantNode.


## Implmentation details

1) Update 'ExpressionLexer' class to support the double-quoted string tokenize, since the double quotes string is valid in JSON.
2) Update 'ExpressionLexer' class to support '(..)', '[...]', '{...}' tokenization.
3) Update 'UriQueryExpressionParser' to parse the JSON object literal to MapToken, JSON arrary to CollectionToken.
   
   Here's two exceptions:
   
   a) Support parentheised collection for 'in' operator.
   
   b) Support route templated, route template is a raw string like: '~/CalculateTotal(id={myid:int})`


6) Update 'MetadataBinder' to bind the CollectionToken to 'CollectionConstantNode', MapToken to 'ResourceConstantNode'


    Be noted, it's recursively that the item in collection could be any literal [primive, enum, complex (JSON object), collection of them ].
    and the property value of JSON object could be any literal, same as item in the collection.

7) Update the other related codes
8) Update the existing test cases
9) Add new test cases to cover the changes.

For JSON format of OData, check '5. JSON format for function parameters' in [OData ABNF](https://docs.oasis-open.org/odata/odata/v4.01/cs01/abnf/odata-abnf-construction-rules.txt)

## Metadata binding details

### Binding ResourceLiteralToken

A resource literal could have the type name metadata within it, such as:

`{'@odata.type':'#Fully.Qualified.Namespace.Address','Street':'NE 24th St.','City':'Redmond'}`

Such literal can be used for a parameter value of function as inline literal as:

`~/myfunction(location={'@odata.type':'#Fully.Qualified.Namespace.Address','Street':'NE 24th St.','City':'Redmond'}) `  (be noted, single quoted string used here, but for valid JSON, it should be double quoted).

In this case, the parameter 'location' has its own metadata type. So the binding process for 'ResourceLiteralToken' is designed as:

| TypeNameFromLiteral | ExpectedTypeFromMetadata | Action 3 |
|:-------------:|:-------------:| -------------:|
| No | No| Each property binding using its literal |
| No | Yes |Use the expected type to binding its property value |
| Yes | No| Resolve type name and use it to binding its property value |
| Yes | Yes | Bind using TypeName from literal first, Make sure the types are compatible, Type from literal should be sub type of the expected, if it's, create a convert |

## Challenges 

1) JSON object could contain metadata control information. For example, it could contains the type as below:
   {'@odata.type':'#Fully.Qualified.Namespace.Address','Street':'NE 24th St.','City':'Redmond'}

2) Single-quoted and double-quoted JSON object. The above JSON object is using single quoted string. For back-compatible, we should support both??

3) We need to convert the node if real type of data is not same as the expected type.
