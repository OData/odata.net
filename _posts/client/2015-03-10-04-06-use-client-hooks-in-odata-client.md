---
layout: post
title: "Client Hooks in OData Client"
description: "How to use client hooks to compose higher level functionality"
category: "4. Client"
---

OData Client provides several ways to allow developers to hook into the client request and response. It gives developers the opportunity to inspect, adjust or replace some request or response.

This doc will give you several real world examples to explain all these kinds of methods in OData Client.


# Event Handler #

`DataServiceContext` provided three events to let developers to hook up to.
 
## BuildingRequest ##

`public event EventHandler<BuildingRequestEventArgs> BuildingRequest;`

This event is fired before a request message object is built, giving the handler the opportunity to inspect, adjust and/or replace some request information before the message is built. This event is always used to modify the outgoing Url of the request, alter request headers or change the http method.



    DefaultContainer dataServiceContext = new DefaultContainer(new Uri("http://services.odata.org/V4/TripPinServiceRW/"));
    dataServiceContext.BuildingRequest += (sender, eventArgs)=>
    {
        eventArgs.RequestUri = new Uri("http://services.odata.org/V4/(S(ghojd5jj5d33cwotkyfwn431))/TripPinServiceRW/People");
    };
    dataServiceContext.People.Execute();

Developers can also change the HttpMethod of the request. 

    dataServiceContext.BuildingRequest += (sender, eventArgs) =>
    {
        eventArgs.Method = "PUT";
    }; 

## ReceivingResponse ##

`public event EventHandler<ReceivingResponseEventArgs> ReceivingResponse;`

This event is fired when a response is received by the client. It is fired for both top level responses and each operation or query within a batch response.

For a non-batch response:

	DefaultContainer dataServiceContext = new DefaultContainer(new Uri("http://services.odata.org/V4/TripPinServiceRW/"));
    dataServiceContext.ReceivingResponse += (sender, eventArgs) =>
    {
        Console.WriteLine(eventArgs.ResponseMessage.GetHeader("OData-Version"));
    };
    dataServiceContext.People.First();

For a batch request for query, the `ReceivingResponse` will firstly be fired when the client receives the top level response. Then, the event will be fired when the client enumerates the inner `QueryOperationResponse`. `ReceivingResponse` is fired as many times as the responses are enumerated. So about the following code, before the client executes `foreach`, the code will only print the `Content-Type` for the top-level request. The last several lines of following code enumerates each of the `QueryOperationResponse`. `RecivingResponse` will be fired accordingly. 

	DefaultContainer dataServiceContext = new DefaultContainer(new Uri("http://services.odata.org/V4/(S(irl1k2jt4e4bscxuk30bpgji))/TripPinServiceRW/"));
    dataServiceContext.ReceivingResponse += (sender, eventArgs) =>
    {
        Console.WriteLine(eventArgs.ResponseMessage.GetHeader("Content-Type"));
    };
    var responses = dataServiceContext.ExecuteBatch(dataServiceContext.People, dataServiceContext.Airlines);

	// Enumerate the response will fire the ReceivingResponse for each of the inner query  
    foreach (QueryOperationResponse response in responses)
    {
        
    }

But for a batch request for changes. `ReceivingResponse` will be fired for both top level response and inner response even the client doesn't enumerate the response. So the following code will print 

    200
    204
    204
<p/>

200 is the response status code of the top level message. the other two 204 status codes are of the inner responses. 

	DefaultContainer dataServiceContext = new DefaultContainer(new Uri("http://services.odata.org/V4/(S(irl1k2jt4e4bscxuk30bpgji))/TripPinServiceRW/"));
    var p1 = dataServiceContext.People.First();
    var p2 = dataServiceContext.People.Skip(1).First();
    dataServiceContext.ReceivingResponse += (sender, eventArgs) =>
    {
        Console.WriteLine(eventArgs.ResponseMessage.StatusCode);
    };
    p1.FirstName = "aa";
    p2.FirstName = "bb";
    dataServiceContext.UpdateObject(p1);
    dataServiceContext.UpdateObject(p2);
    dataServiceContext.SaveChanges(Microsoft.OData.Client.SaveChangesOptions.BatchWithSingleChangeset);

## SendingRequest2 ##

`public event EventHandler<SendingRequest2EventArgs> SendingRequest2;`

This event is fired before a request is sent to the server, giving the handler the opportunity to inspect, adjust and/or replace the WebRequest object used to perform the request.

The most common use of this event is to set the headers of the request. You can set the header for response payload format, or the authentication information like token or cert name. You also can use this event to set preferences, If-Match headers.

The code below will add the `odata.include-annotations` preference in request header to enable getting instance annotations.
    
    DefaultContainer dataServiceContext = new DefaultContainer(new Uri("http://services.odata.org/V4/(S(ghojd5jj5d33cwotkyfwn431))/TripPinServiceRW/"));

    dataServiceContext.SendingRequest2 += (sender, eventArgs) =>
    {
        eventArgs.RequestMessage.SetHeader("Prefer", "odata.include-annotations=\"*\"");
    };

    dataServiceContext.People.Execute();

You can also use this event to check other information in the request message.


# DataServiceClientConfigurations # 

`DataServiceContext` defines a `Configurations` property of `DataServiceClientConfigurations` which contains a `RequestPipeline` and `ResponsePipeline`. These two pipelines provide several hooks to developers to hook into the client request or response. 

## OnMessageCreating ##

`OnMessageCreating` is a property of the `RequestPipeline`.
 
`public Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> OnMessageCreating`

Developers can use this function to customize the request message.

### Customize request message ###
Following code provides a sample which overrides the `GetResponse()` method in user-defined request message which fakes a response message. We define a client request message which inherits `HttpWebRequestMessage`. `HttpWebRequestMessage` is a sub class of `DataServiceClientRequestMessage`

    public class CustomizedRequestMessage : HttpWebRequestMessage
    {
        public string Response { get; set; }
        public Dictionary<string, string> CutomizedHeaders { get; set; }

        public CustomizedRequestMessage(DataServiceClientRequestMessageArgs args)
            : base(args)
        {
        }

        public CustomizedRequestMessage(DataServiceClientRequestMessageArgs args, string response, Dictionary<string, string> headers)
            : base(args)
        {
            this.Response = response;
            this.CutomizedHeaders = headers;
        }

        public override IODataResponseMessage GetResponse()
        {
            return new HttpWebResponseMessage(
                this.CutomizedHeaders,
                200,
                () =>
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(this.Response);
                    return new MemoryStream(byteArray);
                });
        }
    }

### Set `OnMessageCreating` ###

Then, Developers can replace the default client message with `CustomizedClientRequestMessage` by using following code. Then if the client sends a request after this setting, it will automatically return the fake response message.

    DefaultContainer dataServiceContext = new DefaultContainer(new Uri("http://services.odata.org/V4/(S(irl1k2jt4e4bscxuk30bpgji))/TripPinServiceRW/"));

    string response = "..." //set the response
    dataServiceContext.Configurations.RequestPipeline.OnMessageCreating = 
    (args) =>
    {
        return new CustomizedRequestMessage(
            args,
            response,
            new Dictionary<string, string>()
            {
                {"Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8"},
                {"Preference-Applied", "odata.include-annotations=\"*\""}
            });
    };
    dataServiceContext.PeoplePlus.ByKey("Jason").GetValue();

## More client hooks in RequestPipeline && ResponsePipeline ##

These two configurations provide more other client hooks in request pipeline and response pipeline.

Please refer to this [link](http://blogs.msdn.com/b/odatateam/archive/2013/07/26/using-the-new-client-hooks-in-wcf-data-services-client.aspx) for details.