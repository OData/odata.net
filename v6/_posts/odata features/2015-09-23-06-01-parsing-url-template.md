---
layout: post
title: "Parsing URI path template"
description: ""
category: "6. OData Features"
---

From ODataLib 6.11.0, it supports to parse Uri path template. A path template is any identifier string enclosed with curly brackets.
For example: 
{% highlight csharp %}
{dynamicProperty}
{% endhighlight %}

# Uri templates

There are three kind of Uri template:

1. Key template:  ~/Customers({key})
2. Function parameter template: ~/Customers/Default.MyFunction(name={name})
3. Path template: ~/Customers/{dynamicProperty}

Be caution:

1. please EnableUriTemplateParsing = true for UriParser.
2. Path template can't be the first segment.

# Example

{% highlight csharp %}
var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People({1})/{some}", UriKind.Relative))  
{  
  EnableUriTemplateParsing = true  
};

var paths = uriParser.ParsePath().ToList();

var keySegment = paths[1].As<KeySegment>();
var templateSegment = paths[2].As<PathTemplateSegment>();
templateSegment.LiteralText.Should().Be("{some}"); 

{% endhighlight %}

