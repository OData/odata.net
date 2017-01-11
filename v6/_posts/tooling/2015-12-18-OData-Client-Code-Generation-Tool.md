---
layout: post
title: "OData Client Code Generation Tool"
description: "How to generate client proxy file for an OData service"
category: "8. Tooling"
---

OData provide two tools to generate client proxy file for an OData Service.

- [OData Client Code Generator](https://visualstudiogallery.msdn.microsoft.com/9b786c0e-79d1-4a50-89a5-125e57475937?SRC=Featured) support generating client proxy file for OData V4 Service. It supports following Visual Studio:

	- Visual Studio 2010 (The last version of this tool for VS 2010 is 2.3.0, You can download it from [ODataItemTemplate.2.3.0.vsix](https://github.com/OData/lab/blob/Tools/Tools/ODataT4ItemTemplate.2.3.0.vsix).)
	- Visual Studio 2012
	- Visual Studio 2013
	- Visual Studio 2015

	For full documentation, please refere to "[How to use odata client generator to generate client proxy file](http://blogs.msdn.com/b/odatateam/archive/2014/03/12/how-to-use-odata-client-code-generator-to-generate-client-side-proxy-class.aspx)".

- [OData Connected Service](https://visualstudiogallery.msdn.microsoft.com/b343d0eb-6493-44c2-b558-13a0408d013f) lets app developers connect their applications to OData Services (both V3 & V4) and generate the client proxy files for the services. It supports following Visual Studio:

	- Visual Studio 2015
	
The following part will mainly focus on how to use the OData Connected Service to generate client proxy file.  
	
	
# Install OData Connected Service Extension #

You can install this extension by [this link](https://visualstudiogallery.msdn.microsoft.com/b343d0eb-6493-44c2-b558-13a0408d013f/file/163980/4/Microsoft.OData.ConnectedService.vsix) from vs gallery. Or, you can install it in Visual Studio 2015.

In Visual Studio, Click **Tools > Extensions and Updates**.

Expand **Online > Visual Studio Gallery > Tools > Connected Service**, and select the **OData Connected Service** extension.

Click **Download**.

 ![]({{site.baseurl}}/assets/tooling/odata-connected-service-install-extension.png)

Then it will pop up a **VSIX Installer** window, Click **Install**.

Click **Close** once the installation finishes.

You need to restart the visual studio in order for the installation to take effect.

# Generate Client Proxy#

## Create a new project ##

Create your project. Here, we take "Console Application" project as an example.

Start Visual Studio and from the **File** menu, select **New** and then **Project**.

In the **Templates** pane, select **Installed > Templates**, expand the **Visual C# > Windows > Classic Desktop** and select **Console Application**. Name the Project "TrippinApp" and click **OK**.

## Generate client proxy for an OData service ##

In the **Solution Explorer** pane, right click the "TrippinApp" project and select **Add** and then **Connected Service**.

In the **Add Connected Service** dialog, select **OData** and then click **Configure**.

 ![]({{site.baseurl}}/assets/tooling/odata-connected-service-new.PNG)

In the **Configure endpoint** dialog, input the service name and the OData service endpoint, then click **Next** button.

![]({{site.baseurl}}/assets/tooling/odata-connected-service-config-endpoint.PNG)
 
In the **Settings** dialog, enter the file name(without extension) of the proxy file and click **Finish**.

![]({{site.baseurl}}/assets/tooling/odata-connected-service-config-file-name.PNG)

In the **Settings** dialog, You also can configure some other settings by click **AdvancedSettings** link. Then you can set the related code generation settings.

![]({{site.baseurl}}/assets/tooling/odata-connected-service-advanced-settings.PNG)

Once you finished all those settings, click **Finish**. This tool will begin to install the related NuGet packages and generate the client proxy file into your project.

![]({{site.baseurl}}/assets/tooling/odata-connected-service-generate.PNG)

## Consume the OData service ##

Now, the developer can write client code to consume the OData Service.

    using System;
	using Microsoft.OData.SampleService.Models.TripPin;
	
	namespace TrippinApp
	{
	    class Program
	    {
	        static void Main(string[] args)
	        {
	            DefaultContainer dsc = new DefaultContainer(
	                new Uri("http://services.odata.org/V4/(S(fgov00tcpdbmkztpexfg24id))/TrippinServiceRW/"));
	            var me = dsc.Me.GetValue();
	            Console.WriteLine(me.UserName);
	        }
	    }
	}


![]({{site.baseurl}}/assets/tooling/odata-connected-service-consume.PNG)

# Summary #

Now you have the OData Connected Service at your disposal to generate your client proxy for any OData service. To leave us feedback, please open github issues at [OData Lab GitHub](https://github.com/OData/lab/issues).