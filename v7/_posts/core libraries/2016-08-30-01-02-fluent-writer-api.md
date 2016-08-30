---
layout: post
title: "1.2 Fluent functional-style writer API"
description: ""
category: "1. Core"
---

In the previous section, paired `WriteStart()`/`WriteEnd()` calls have been made to write payloads. In this version, a new set of fluent functional-style API has been introduced as an improvement over the previous API which is rather primitive, requiring paired `WriteStart()`/`WriteEnd()` calls.

The new API replaces paired `WriteStart()`/`WriteEnd()` calls with a single `Write()` call. `Write()` comes in two flavors. The first flavor takes a single argument which is the thing you want to write. For example, `writer.Write(entry);` is equivalent to

{% highlight csharp %}
writer.WriteStart(entry);
writer.WriteEnd();
{% endhighlight %}

The second flavor takes two arguments. The first argument is same as before. The second argument is an [`Action`](https://msdn.microsoft.com/en-us/library/system.action(v=vs.110).aspx) delegate which is to be invoked in-between writing the first argument. For instance,

{% highlight csharp %}
writer.Write(outer, () => writer
    .Write(inner1)
    .Write(inner2));
{% endhighlight %}

is equivalent to

{% highlight csharp %}
writer.WriteStart(outer);
    writer.WriteStart(inner1);
    writer.WriteEnd();
    writer.WriteStart(inner2);
    writer.WriteEnd();
writer.WriteEnd();
{% endhighlight %}

In general, this new API should be preferred to the previous one.
