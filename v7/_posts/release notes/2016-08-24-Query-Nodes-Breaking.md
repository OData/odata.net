---
layout: post
title: "Breaking changes about Query Nodes"
description: ""
category: "4. Release Notes"
---

The expression of `$filter` and `$orderby` will be parsed to multiple query nodes. Each node has particular representation, for example, a navigation property access will be interpreted as `SingleNavigationNode` and collection of navigation property access will be interpreted as `CollectionNavigationNode`.

Since we have merged complex type and entity type in OData Core lib, complex have more similarity with entity other than primitive property. Also in order to support navigation property under complex, the query nodes' type and hierarchy are changed and adjusted to make it more reasonable.

## Nodes Change ##

### Nodes Added ###
	SingleComplexNode
	CollectionComplexNode


### Nodes Renamed ###

| Old                                 | New                                   |
|-------------------------------------|---------------------------------------|
| NonentityRangeVariable              | NonResourceRangeVariable              |
| EntityRangeVariable                 | ResourceRangeVariable                 |
| NonentityRangeVariableReferenceNode | NonResourceRangeVariableReferenceNode |
| EntityRangeVariableReferenceNode    | ResourceRangeVariableReferenceNode    |
| EntityCollectionCastNode            | CollectionResourceCastNode            |
| EntityCollectionFunctionCallNode    | CollectionResourceFunctionCallNode    |
| SingleEntityCastNode                | SingleResourceCastNode                |
| SingleEntityFunctionCallNode        | SingleResourceFunctionCallNode        |



### Nodes Removed ###
	CollectionPropertyCast
	SingleValueCast
	


## API Change ##

1. Add SingleResourceNode as the  base class of SingleEntityNode and SingleComplexNode

2. SingleNavigationNode and CollectionNavigationNode accepts SingleResourceNode as parent node and also accepts bindingpath in the constructor.The parameter order is also adjusted.

    Take SingleNavigationNode for example:
{% highlight csharp %}
public SingleNavigationNode(IEdmNavigationProperty navigationProperty, SingleEntityNode source)
{% endhighlight %}
Changed to: 
{% highlight csharp %}
public SingleNavigationNode(SingleResourceNode source, IEdmNavigationProperty navigationProperty, IEdmPathExpression bindingPath)
{% endhighlight %}


## Behavior Change for complex type nodes ##  
Complex property used to share nodes with primitive property, now it shares most nodes with entity.
Here lists the nodes that complex used before and now.

| Before                          | Now                                |
|---------------------------------|------------------------------------|
| NonentityRangeVariable          | ResourceRangeVariable              |
| NonentityRangeVariableReference | ResourceRangeVariableReference     |
| SingleValuePropertyAccessNode   | SingleComplexNode                  |
| SingleValueCastNode             | SingleResourceCastNode             |
| SingleValueFunctionCallNode     | SingleResourceFunctionCallNode     |
| CollectionPropertyAccessNode    | CollectionComplexNode              |
| CollectionPropertyCastNode      | CollectionResourceCastNode         |
| CollectionFunctionCallNode      | CollectionResourceFunctionCallNode |
