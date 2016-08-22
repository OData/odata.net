---
layout: post
title: "Batch Operations"
description: ""
category: "4. Client"
---
 
OData Client for .NET supports batch processing of requests to an OData service. This ensures that all operations in the batch are sent to the data service in a single HTTP request, enables the server to process the operations atomically, and reduces the number of round trips to the service.

OData Client for .NET doesn't support sending both query and change in one batch request.
 
# Batch Query #

To execute multiple queries in a single batch, you must create each query in the batch as a separate instance of the `DataServiceRequest<TElement>` class. The batched query requests are sent to the data service when the `ExecuteBatch` method is called. It contains the query request objects. 

This method accepts an array of `DataServiceRequest` as parameters. It returns a `DataServiceResponse` object, which is a collection of `QueryOperationResponse<T>` objects that represent responses to individual queries in the batch, each of which contains either a collection of objects returned by the query or error information. When any single query operation in the batch fails, error information is returned in the `QueryOperationResponse<T>` object for the operation that failed and the remaining operations are still executed. 

{% highlight csharp %}

    DefaultContainer dsc = new DefaultContainer(new Uri("http://services.odata.org/V4/(S(uvf1y321yx031rnxmcbqmlxw))/TripPinServiceRW/"));
    public void BatchQuery()
    {
        var peopleQuery = dsc.People;
        var airlinesQuery = dsc.Airlines;

        var batchResponse = dsc.ExecuteBatch(peopleQuery, airlinesQuery);
        foreach(var r in batchResponse)
        {
            var people = r as QueryOperationResponse<Person>;
            if (people != null)
            {
                foreach (Person p in people)
                {
                    Console.WriteLine(p.UserName);
                }
            }

            var airlines = r as QueryOperationResponse<Airline>;
            if (airlines != null)
            {
                foreach (var airline in airlines)
                {
                    Console.WriteLine(airline.Name);
                }
            }
        }
    }

{% endhighlight %}

`ExecuteBatch` will send a "POST" request to `http://services.odata.org/V4/(S(uvf1y321yx031rnxmcbqmlxw))/TripPinServiceRW/$batch`. Each internal request contains its own http method "GET".

The payload of the request is as following:

	--batch_d3bcb804-ee77-4921-9a45-761f98d32029
	Content-Type: application/http
	Content-Transfer-Encoding: binary
	
	GET http://services.odata.org/V4/(S(uvf1y321yx031rnxmcbqmlxw))/TripPinServiceRW/People HTTP/1.1
	OData-Version: 4.0
	OData-MaxVersion: 4.0
	Accept: application/json;odata.metadata=minimal
	Accept-Charset: UTF-8
	User-Agent: Microsoft ADO.NET Data Services
	
	--batch_d3bcb804-ee77-4921-9a45-761f98d32029
	Content-Type: application/http
	Content-Transfer-Encoding: binary
	
	GET http://services.odata.org/V4/(S(uvf1y321yx031rnxmcbqmlxw))/TripPinServiceRW/Airlines HTTP/1.1
	OData-Version: 4.0
	OData-MaxVersion: 4.0
	Accept: application/json;odata.metadata=minimal
	Accept-Charset: UTF-8
	User-Agent: Microsoft ADO.NET Data Services
	
	--batch_d3bcb804-ee77-4921-9a45-761f98d32029--

# Batch Modification #

In order to batch a set of changes to the server, `ODataServiceContext` provides `SaveChangesOptions.BatchWithSingleChangeset` and `SaveChangesOptions.BatchWithIndependentOperations` when `SaveChanges`.

`SaveChangesOptions.BatchWithSingleChangeset` will save changes in a single change set in a batch request.

`SaveChangesOptions.BatchWithIndependentOperations` will save each change independently in a batch request. 

You can refer to [odata v4.0 protocol 11.7](http://docs.oasis-open.org/odata/odata/v4.0/errata02/os/complete/part1-protocol/odata-v4.0-errata02-os-part1-protocol-complete.html#_Toc406398359) to get more details about batch request and whether requests should be contained in one change set or not.
 

{% highlight csharp %}

    DefaultContainer dsc = new DefaultContainer(new Uri("http://services.odata.org/V4/(S(uvf1y321yx031rnxmcbqmlxw))/TripPinServiceRW/"));
    public void BatchModify()
    {
        dsc.MergeOption = MergeOption.PreserveChanges;
        var me = dsc.Me.GetValue();
        var myTrip = dsc.Me.Trips.First();

        me.LastName = "Test";
        myTrip.Description = "Updated Trip";

        dsc.UpdateObject(me);
        dsc.UpdateObject(myTrip);

        dsc.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

        Console.WriteLine(me.LastName);
        Console.WriteLine(myTrip.Description);
    }
{% endhighlight %}

The payload for all requests in one change set is like following

This will send request with URL http://services.odata.org/V4/(S(uvf1y321yx031rnxmcbqmlxw))/TripPinServiceRW/$batch.

The request headers contain following two headers:

	Content-Type: multipart/mixed; boundary=batch_06d8a02a-854a-4a21-8e5c-f737bbd2dea8
	Accept: multipart/mixed

The request Payload is as following:

	--batch_06d8a02a-854a-4a21-8e5c-f737bbd2dea8
	Content-Type: multipart/mixed; boundary=changeset_b98a784d-af07-4723-9d5c-4722801f4c4d
	
	--changeset_b98a784d-af07-4723-9d5c-4722801f4c4d
	Content-Type: application/http
	Content-Transfer-Encoding: binary
	Content-ID: 3
	
	PATCH http://services.odata.org/V4/(S(uvf1y321yx031rnxmcbqmlxw))/TripPinServiceRW/Me HTTP/1.1
	OData-Version: 4.0
	OData-MaxVersion: 4.0
	Content-Type: application/json;odata.metadata=minimal
	If-Match: W/"08D24EFA2E435C91"
	Accept: application/json;odata.metadata=minimal
	Accept-Charset: UTF-8
	User-Agent: Microsoft ADO.NET Data Services
	
	{"@odata.type":"#Microsoft.OData.SampleService.Models.TripPin.Person","AddressInfo@odata.type":"#Collection(Microsoft.OData.SampleService.Models.TripPin.Location)","AddressInfo":[{"@odata.type":"#Microsoft.OData.SampleService.Models.TripPin.Location","Address":"P.O. Box 555","City":{"@odata.type":"#Microsoft.OData.SampleService.Models.TripPin.City","CountryRegion":"United States","Name":"Lander","Region":"WY"}}],"Concurrency":635657333837618321,"Emails@odata.type":"#Collection(String)","Emails":["April@example.com","April@contoso.com"],"FirstName":"April","Gender@odata.type":"#Microsoft.OData.SampleService.Models.TripPin.PersonGender","Gender":"Female","LastName":"Test","UserName":"aprilcline"}
	--changeset_b98a784d-af07-4723-9d5c-4722801f4c4d
	Content-Type: application/http
	Content-Transfer-Encoding: binary
	Content-ID: 4
	
	PATCH http://services.odata.org/V4/(S(uvf1y321yx031rnxmcbqmlxw))/TripPinServiceRW/Me/Trips(1001) HTTP/1.1
	OData-Version: 4.0
	OData-MaxVersion: 4.0
	Content-Type: application/json;odata.metadata=minimal
	Accept: application/json;odata.metadata=minimal
	Accept-Charset: UTF-8
	User-Agent: Microsoft ADO.NET Data Services
	
	{"@odata.type":"#Microsoft.OData.SampleService.Models.TripPin.Trip","Budget":3000,"Description":"Updated Trip","EndsAt":"2014-01-04T00:00:00Z","Name":"Trip in US","ShareId":"9d9b2fa0-efbf-490e-a5e3-bac8f7d47354","StartsAt":"2014-01-01T00:00:00Z","Tags@odata.type":"#Collection(String)","Tags":["Trip in New York","business","sightseeing"],"TripId":1001}
	--changeset_b98a784d-af07-4723-9d5c-4722801f4c4d--
	--batch_06d8a02a-854a-4a21-8e5c-f737bbd2dea8--

