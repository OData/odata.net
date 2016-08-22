---
layout: post
title: "2.2 Read and write models"
description: "Read and write entity data models using EdmLib APIs"
category: "2. EdmLib"
---

Models built with EdmLib APIs are in **object representation** while CSDL documents are in **XML representation**. The conversion from models to CSDL is accomplished by the `CsdlWriter` APIs which are mostly used by OData services to **expose metadata documents** (CSDL). In contrast, the conversion from CSDL to models is done by the `CsdlReader` APIs which are usually used by OData clients to **read metadata documents** from services.

This section shows how to read and write entity data models using EdmLib APIs. We will continue to use and extend the sample from the previous section.

### Using the CsdlWriter APIs
We have already used one of the APIs to write the model to a CSDL document in the last section.

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
                model.TryWriteCsdl(writer, out errors);
            }
        }
    }
}
{% endhighlight %}

The `CsdlWriter.TryWriteCsdl()` method is defined as an extension method to `IEdmModel`:

{% highlight csharp %}
namespace Microsoft.OData.Edm.Csdl
{
    public static class CsdlWriter
    {
        ...
        public static bool TryWriteCsdl(this IEdmModel model, XmlWriter writer, out IEnumerable<EdmError> errors);
        ...
    }
}
{% endhighlight %}

The **second parameter** `writer` requires an `XmlWriter` which can be created through the overloaded `XmlWriter.Create` methods. Remember to either apply a `using` clause to an `XmlWriter` instance or explicitly call `XmlWriter.Flush()` (or `XmlWriter.Close()`) to **flush the buffer to its underlying stream**. The **third parameter** `errors` is used to pass out the errors found when writing the model. If the method **returns** `true` (indicating write success), the `errors` should be an empty `Enumerable`; otherwise it contains all the model errors.

The other version of the `CsdlWriter.TryWriteCsdl()` method is:

{% highlight csharp %}
namespace Microsoft.OData.Edm.Csdl
{
    public static class CsdlWriter
    {
        ...
        public static bool TryWriteCsdl(this IEdmModel model, Func<string, XmlWriter> writerProvider, out IEnumerable<EdmError> errors);
        ...
    }
}
{% endhighlight %}

This overload is called when the model to write contains **referenced models**. The referenced models need to be written into separate files. So the **second parameter** `writerProvider` takes a callback to create a different `XmlWriter` for each referenced model where the `string` parameter is the schema namespace of that model. A simple `writerProvider` would be:

{% highlight csharp %}
public XmlWriter CreateXmlWriter(string namespace)
{
    return XmlWriter.Create(string.Format("{0}.xml", namespace));
}
{% endhighlight %}

### Using the CsdlReader APIs
The `CsdlReader` APIs are defined as follows:

{% highlight csharp %}
namespace Microsoft.OData.Edm.Csdl
{
    public static class CsdlReader
    {
        public static bool TryParse(IEnumerable<XmlReader> readers, out IEdmModel model, out IEnumerable<EdmError> errors);
        public static bool TryParse(IEnumerable<XmlReader> readers, IEdmModel reference, out IEdmModel model, out IEnumerable<EdmError> errors);
        public static bool TryParse(IEnumerable<XmlReader> readers, IEnumerable<IEdmModel> references, out IEdmModel model, out IEnumerable<EdmError> errors);
    }
}
{% endhighlight %}

The **first overload** is mostly used. The **second and third overloads** are similar to the first one except that they also accept one or more referenced models.

The **first parameter** `readers` takes a set of `XmlReader` each of which reads a CSDL document. The **second paramter** `model` passes out the parsed model. The **third parameter** `errors` passes out the errors when parsing the CSDL document. If the **return value** of this method is `true` (indicating parse success), the `errors` should be an empty otherwise it will contain all the model errors.

### Roundtrip the Model
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
                if (CsdlReader.TryParse(new[] { reader }, out model, out errors))
                {
                    return model;
                }
                return null;
            }
        }
    }
}
{% endhighlight %}

This code first reads the model from the CSDL document **csdl.xml** and then writes the model to another CSDL document **csdl1.xml**.

### Run the Sample
Build and run the sample. Then open both the **csdl.xml** file and the **csdl1.xml** file under the **output directory**. The content of **csdl1.xml** should look like the following:

![]({{site.baseurl}}/assets/2015-04-17-csdl1.png)

You can see that the contents of **csdl.xml** and **csdl1.xml** are exactly the same except for **the order of the elements**. This is because EdmLib will reorder the elements when parsing a CSDL document.

### References
[[Tutorial & Sample] Refering when Constructing EDM Model](http://blogs.msdn.com/b/odatateam/archive/2014/06/30/refer-other-models-when-constructing-edm-model.aspx).