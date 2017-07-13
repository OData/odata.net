---
layout: post
title: "Optional Parameters"
description: ""
category: "5. OData Features"
---

Starting with ODataLib 7.3, parameters can be marked as optional. Optional parameters may be omitted when invoking a custom function. 

Function resolution will first attempt to find an overload that exactly matches the specified function. If there is no exact match, it will select the function overload that matches all specified parameters and for which all required parameters are specified, and fail if there is more than one such function. 

Optional parameters are annotated in CSDL using the new `Org.OData.Core.V1.OptionalParameter` annotation term:

    "<Parameter Name = "optionalParam" Type="Edm.String">"
       "<Annotation Term="Org.OData.Core.V1.OptionalParameter"/>"
    "</Parameter>"

An optional parameter may optionally specify a default value. The default value is purely informational, and conveys to the consumer the value that will be used if the parameter is not supplied:

     "<Parameter Name = "optionalParamWithDefault" Type="Edm.String">"
        "<Annotation Term="Org.OData.Core.V1.OptionalParameter">"
          "<Record>"
            "<PropertyValue Property="DefaultValue" String="Smith"/>"
           "</Record>"
         "</Annotation>"
      "</Parameter>"

Any optional parameters must be come after all non-optional parameters for a function or action. 

When reading CSDL, optional values are returned as parameters that support `IEdmOptionalParameter`:

{% highlight csharp %}    

       foreach (IEdmOperationParameter parameter in function)
       {
         var optional = parameter as IEdmOptionalParameter;
         Console.Write("Parameter '{0}' is {1} ", parameter.Name, optional == null ? "required" : "optional");

         if (optional != null && !String.IsNullOrEmpty(optional.DefaultValueString))
         {
           Console.Write(" and has a default value of " + optional.DefaultValueString);
         }

         Console.WriteLine();
       }

{% endhighlight %}

When building a model, optional parameters may be specified using the new `EdmOptionalParameter` class:

{% highlight csharp %}    

       var stringTypeReference = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);
       var function = new EdmFunction("test", "TestFunction", stringTypeReference);
       var requiredParam = new EdmOperationParameter(function, "requiredParam", stringTypeReference);
       var optionalParam = new EdmOptionalParameter(function, "optionalParam", stringTypeReference, null);
       var optionalParamWithDefault = new EdmOptionalParameter(function, "optionalParamWithDefault", stringTypeReference, "Smith");
       function.AddParameter(requiredParam);
       function.AddParameter(optionalParam);
       function.AddParameter(optionalParamWithDefault);

{% endhighlight %}
