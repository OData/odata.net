---
layout: post
title: "OData Connected Service"
description: "How to use OData Connected Service to generate client proxy file"
category: "8. Tooling"
---


The OData Connected Service tool lets app developers connect their applications to OData Services and generate the client proxy files for the services.

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