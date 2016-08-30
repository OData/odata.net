---
layout: post
title: "2.2 Read and write models"
description: "Read and write entity data models using EdmLib APIs"
category: "2. EdmLib"
---

Models built with EdmLib APIs are in **object representation**, while CSDL documents are in **XML representation**. The conversion from models to CSDL is accomplished by the `CsdlWriter` APIs which are mostly used by OData services to **expose metadata documents** (CSDL). In contrast, the conversion from CSDL to models is done by the `CsdlReader` APIs which are usually used by OData clients to **read metadata documents** from services.

This section shows how to read and write entity data models using EdmLib APIs. We will use and extend the sample from the previous section.

### Using the CsdlWriter APIs
We have already used one of the `CsdlWriter` APIs to write the model to a CSDL document in the previous section.

{% highlight csharp %}
namespace EdmLibSample
{
    class Program
    {
        ...
        private static void WriteModelToCsdl(IEdmModel model, string fileName)
        {
            using (var writer = XmlWriter.Create(fileName))
            {
                IEnumerable<EdmError> errors;
                CsdlWriter.TryWriteCsdl(model, writer, CsdlTarget.OData, out errors);
            }
        }
    }
}
{% endhighlight %}

The `CsdlWriter.TryWriteCsdl()` method is prototyped as:

{% highlight csharp %}
namespace Microsoft.OData.Edm.Csdl
{
    public class CsdlWriter
    {
        ...
        public static bool TryWriteCsdl(
            IEdmModel model,
            XmlWriter writer,
            CsdlTarget target,
            out IEnumerable<EdmError> errors);
        ...
    }
}
{% endhighlight %}

The **second parameter** `writer` requires an `XmlWriter` which can be created through the overloaded `XmlWriter.Create()` methods. Remember to either apply a `using` statement on an `XmlWriter` instance or explicitly call `XmlWriter.Flush()` (or `XmlWriter.Close()`) to **flush the buffer to its underlying stream**. The **third parameter** `target` specifies the target implementation of the CSDL being generated, which can be either `CsdlTarget.EntityFramework` or `CsdlTarget.OData`. The **4th parameter** `errors` is used to pass out the errors encountered in writing the model. If the method **returns** `true` (indicate success), the `errors` should be an empty `Enumerable`; otherwise it contains all the errors encountered.

### Using the CsdlReader APIs
The simplest `CsdlReader` API is prototyped as:

{% highlight csharp %}
namespace Microsoft.OData.Edm.Csdl
{
    public class CsdlReader
    {
        public static bool TryParse(XmlReader reader, out IEdmModel model, out IEnumerable<EdmError> errors);
    }
}
{% endhighlight %}

The **first parameter** `reader` takes an `XmlReader` that reads a CSDL document. The **second parameter** `model` passes out the parsed model. The **third parameter** `errors` passes out the errors encountered in parsing the CSDL document. If the **return value** of this method is `true` (indicate success), the `errors` should be an empty `Enumerable`; otherwise it will contain all the errors encountered.

### Roundtrip the model
In the **Program.cs** file, insert the following code to the `Program` class:

{% highlight csharp %}
namespace EdmLibSample
{
    class Program
    {
        public static void Main(string[] args)
        {
            ...
            WriteModelToCsdl(model, "csdl.xml");
#region     !!!INSERT THE CODE BELOW!!!
            var model1 = ReadModel("csdl.xml");
            WriteModelToCsdl(model1, "csdl1.xml");
#endregion
        }
        ...
        private static IEdmModel ReadModel(string fileName)
        {
            using (var reader = XmlReader.Create(fileName))
            {
                IEdmModel model;
                IEnumerable<EdmError> errors;
                if (CsdlReader.TryParse(reader, out model, out errors))
                {
                    return model;
                }
                return null;
            }
        }
    }
}
{% endhighlight %}

This code first reads the model from the CSDL document **csdl.xml**, and then writes the model to another CSDL document **csdl1.xml**.

### Run the sample
Build and run the sample. Then open the file **csdl.xml** and the file **csdl1.xml** under the **output directory**. The content of **csdl1.xml** should look like the following:

![]({{site.baseurl}}/assets/2015-04-17-csdl1.png)

You can see that the contents of **csdl.xml** and **csdl1.xml** are exactly the same except for **the order of the elements**. This is because EdmLib will reorder the elements when parsing a CSDL document.

### References
[[Tutorial & Sample] Refering when Constructing EDM Model](http://blogs.msdn.com/b/odatateam/archive/2014/06/30/refer-other-models-when-constructing-edm-model.aspx).
