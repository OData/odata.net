---
layout: post
title: "2.7 Define annotations"
description: "Define annotations using EdmLib APIs"
category: "2. EdmLib"
---

EdmLib supports adding annotations on various model elements, including entity sets, entity types, properties and so on. Annotations can be put under **the `Annotations` element** in the schema as well as **the targetted model elements** (**inline** annotations). Users can specify the **serialization location** using EdmLib API.

This section shows how to define annotations using EdmLib APIs. We will continue to use and extend the sample from the previous sections.

### Add an Annotation to the Entity Set *Customers*
In the **SampleModelBuilder.cs** file, add the following `using` clause:

{% highlight csharp %}
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
{% endhighlight %}

Then add the following code into the `SampleModelBuilder` class:

{% highlight csharp %}
namespace EdmLibSample
{
    public class SampleModelBuilder
    {
        ...
        public SampleModelBuilder BuildAnnotations()
        {
            var term1 = new EdmTerm("Sample.NS", "MaxCount", EdmCoreModel.Instance.GetInt32(true));
            var annotation1 = new EdmVocabularyAnnotation(_customerSet, term1, new EdmIntegerConstant(10000000L));
            _model.AddVocabularyAnnotation(annotation1);
            return this;
        }
        ...
    }
}
{% endhighlight %}

And in the **Program.cs** file, insert the following code into the `Main` method:

{% highlight csharp %}
namespace EdmLibSample
{
    class Program
    {
        public static void Main(string[] args)
        {
            var builder = new SampleModelBuilder();
            var model = builder
                ...
                .BuildMostValuableFunctionImport()
#region         !!!INSERT THE CODE BELOW!!!
                .BuildAnnotations()
#endregion
                .GetModel();
            WriteModelToCsdl(model, "csdl.xml");
        }
    }
}
{% endhighlight %}

This code adds an `Edm.Int32` annotation `Sample.NS.MaxCount` targetting the entity set `Customers` to the `Annotations` element.

### Add an Inline Annotation to the Entity Type *Customer*
In the **SampleModelBuilder.cs** file, insert the following code into the `SampleModelBuilder.BuildAnnotations()` method:

{% highlight csharp %}
namespace EdmLibSample
{
    public class SampleModelBuilder
    {
        ...
        public SampleModelBuilder BuildAnnotations()
        {
            ...
            _model.AddVocabularyAnnotation(annotation1);
#region     !!!INSERT THE CODE BELOW!!!
            var term2 = new EdmTerm("Sample.NS", "KeyName", EdmCoreModel.Instance.GetString(true));
            var annotation2 = new EdmVocabularyAnnotation(_customerType, term2, new EdmStringConstant("Id"));
            annotation2.SetSerializationLocation(_model, EdmVocabularyAnnotationSerializationLocation.Inline);
            _model.AddVocabularyAnnotation(annotation2);
#endregion
            return this;
        }
        ...
    }
}
{% endhighlight %}

This code adds an **inline** `Edm.String` annotation `Sample.NS.KeyName` targetting the entity type `Customer`.

### Add an Inline Annotation to the Property *Customer.Name*
In the **SampleModelBuilder.cs** file, insert the following code into the `SampleModelBuilder.BuildAnnotations()` method:

{% highlight csharp %}
namespace EdmLibSample
{
    public class SampleModelBuilder
    {
        ...
        public SampleModelBuilder BuildAnnotations()
        {
            ...
            _model.AddVocabularyAnnotation(annotation2);
#region     !!!INSERT THE CODE BELOW!!!
            var term3 = new EdmTerm("Sample.NS", "Width", EdmCoreModel.Instance.GetInt32(true));
            var annotation3 = new EdmVocabularyAnnotation(_customerType.FindProperty("Name"), term3, new EdmIntegerConstant(10L));
            annotation3.SetSerializationLocation(_model, EdmVocabularyAnnotationSerializationLocation.Inline);
            _model.AddVocabularyAnnotation(annotation3);
#endregion
            return this;
        }
        ...
    }
}
{% endhighlight %}

This code adds an **inline** `Edm.Int32` annotation `Sample.NS.Width` targetting the property `Customer.Name`.

### Run the Sample
Build and run the sample. Then open the **csdl.xml** file under the **output directory**. The content of **csdl.xml** should look like the following:

![]({{site.baseurl}}/assets/2015-04-20-csdl1.png)