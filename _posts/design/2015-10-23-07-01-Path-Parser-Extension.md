---
layout: post
title: "7.1 Parser Extension Design"
description: "Design doc for uriparser, keyassegment"
category: "7. Design"
---

This doc discuss about the design of extension of Path parser part of UriParser.

# 1 Path Parser Overview

## 1.1 Current path parsing logic

- Linear parsing, one segment is determined, then next one;
- Syntax and semantic passes are combined together, as we need type information from model to validate the identifiers (Web API );
- For certain raw segment, each kind of segment is tried one by one. And it stops processing when one matches;
- Exception thrown immediately when unknown segment encountered.

Related code: ODataPathParser.cs
{% highlight csharp %}
internal IList<ODataPathSegment> ParsePath(ICollection<string> segments)
{
    // ...
    try
    {
        while (this.TryGetNextSegmentText(out segmentText))
        {
            if (this.parsedSegments.Count == 0)
            {
                this.CreateFirstSegment(segmentText);
            }
            else
            {
                this.CreateNextSegment(segmentText);
            }
        }
    }
    catch (ODataUnrecognizedPathException ex)
    {
        ex.ParsedSegments = this.parsedSegments;
        ex.CurrentSegment = segmentText;
        ex.UnparsedSegments = this.segmentQueue.ToList();
        throw ex;
    }
    // ...
}

private void CreateNextSegment(string text)
{
    if (this.TryHandleAsKeySegment(text))
    {
        return;
    }

    if (this.TryCreateValueSegment(text))
    {
        return;
    }
        
    ...

    if (this.TryCreateTypeNameSegment(previous, identifier, parenthesisExpression))
    {
        return;
    }

    if (this.TryCreateSegmentForOperation(previous, identifier, parenthesisExpression))
    {
        return;
    }
}
{% endhighlight %}

## 1.2 KeyAsSegment Rule

Key as segment means user could choose to place a key in the path, instead of in brackets, which is OData convention.
Currently the ODataLib supports KeyAsSegment by providing an ODataUrlConventions setting on ODataUriparser. But the introducing of KeyAsSegment does bring conflicts, in order to solve those conflicts, ODataLib also introduced the ‘$’ escape sign. Here are some words taken from source code:
{% highlight text %}
If this segment is the special escape-marker segment, then remember that the next segment cannot be a key, 
even if we are in key-as-segments mode. Essentially, it is an escape into 'metadata-space', so to speak. 
DEVNOTE: We went back and forth several times on whether this should be allowed everywhere or only 
where a key could appear. We landed on allowing it absolutely everywhere for several reasons: 
  1) The WCF DS client naively adds the escape marker before all type segments, regardless of whether the  
     prior segment is a collection. 
  2) The WCF DS server already allowed the escape marker almost everywhere in 5.3 
  3) It's better to be either extremely loose or extremely strict than allow it in some cases and not in others.
Note that this is not publicly documented in OData V3 nor is it planned to be documented in OData V4, but it 
is a part of supporting the Key-As-Segment conventions that are used by many Azure services.
{% endhighlight %}

In this case for the following 2 Urls would have different parsing results:
1. 
```
/Me/Messages/ns.Message
{Singleton}/{Navigation}/{Key}
```
2.
```
/Me/Messages/$/ns.Message
{Singleton}/{Navigation}/{Type}
```

As the quote says, for now we still do not have document to describe the detailed behavior of ‘$’, so it would be nice if we can have some pre-defined parsing rules to resolve the confliction while not relying on ‘$’. Also it is supposed to be the default Url convention for OData simplified. Detailed design would be discussed in later part.

## 1.3 Design goal

- Do not throw exception until segment cannot be handled eventually, customer extensions may define new syntax rules.
- Modularize single segment parser, we easily integrate community contributed parser extensions. (We’d provide an extension framework, instead of hard-coded extensions.)

# 2 Design Detail

## 2.1 New parser context class

Add the following class:
{% highlight csharp %}
public class ODataUriParserContext
{
    public readonly ODataUriParserSettings Settings;
    public readonly IEdmModel Model;
    public readonly Uri ServiceRoot;
    public readonly ODataUrlConventions UrlConventions;
    public readonly Func<string, BatchReferenceSegment> BatchReferenceCallback;
    public readonly bool EnableUriTemplateParsing;

    public List<ODataPathSegment> ParsedSegments { get; set; }
    public Queue<string> RawSegments { get; set; }
}
{% endhighlight %}


## 2.2 Update Parser configuration

Modify the following class:
{% highlight csharp %}
public sealed class ODataUriParser
{
    public delegate void ODataPathSegmentHandler(ODataUriParserContext context);
    public ODataPathSegmentHandler OnSegmentParsing { get; set; }
    // ...
}
{% endhighlight %}

User could choose to modify ParsedSegments in ODataUriParserContext, in order to custom path parser behavior.
For implementation wise, we should first modularize current path parser, and change various TryCreateXXXSegment into some method working with the ODataUriParserContext. That means the Path parser now becomes a parsing flow runner with extensions for users to implement, while the default implementation is the old parsing behavior.

Please note the Uri string segment and parsed path segment do not necessarily have an one-to-one mapping.
One thing to note is that we do not enforce forward-only parsing flow for current Uri Parser, that means a later action could delete a previous parsed segment in the upcoming steps.
For example, when parsing the following Uri string:
```
~/People(1)/Orders/$ref
```
When parser reaches ‘Orders’, it would recognize it as a navigation property segment on people, and add the new navigation segment to the ParsedSegments list. Later when it meets the ‘$ref’ keyword, it knows that the Uri is to address a reference link instead of the navigation target directly. Unfortunately, we’ve got different segment kinds for the two (NavigationPropertySegment and NavigationPropertyLinkSegment). In this case, the parser would remove the current tailing  NavigationPropertySegment segment, and then add a new NavigationPropertyLinkSegment to the parsed list.
For this case the expected behavior is: when the parser reaches ‘Orders’, it do a pre-peek at the coming up segment, and decide whether to add a new NavigationPropertySegment or NavigationPropertyLinkSegment.
The current behavior doesn’t look like an idea model for path parsing, while a non-backtracking parser would be more intuitive both for the parsing flow and the extension providers. 
If we are going to change this, we may have a simplified parser context and parsing flow.

## 2.3 New Convention for KeyAsSegment

{% highlight csharp %}
public sealed class ODataUrlConventions{
    public static ODataUrlConventions Default { get; }
    public static ODataUrlConventions KeyAsSegment { get; }
    public static ODataUrlConventions ODataSimplified { get; }
}
{% endhighlight %}

When ODataSimplified convention is chosen, the following path segment parsing order is applied:

 collectionNavigation           | singleNavigation              
 ------------------------------ | ----------------------------  
 Fixed Segment ( $ref, $count)  | Fixed Segment ($ref, $value)  
 Type                           | Type                          
 Operation                      | Operation                      
 Key                            | Property                      
