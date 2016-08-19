---
layout: post
title: "Asynchronous operations"
description: ""
category: "4. Client"
---
 
*All samples in this doc are based on the Trippin Service. You can follow "[How to use OData Client Code Generator to generate client-side proxy class](http://blogs.msdn.com/b/odatateam/archive/2014/03/11/how-to-use-odata-client-code-generator-to-generate-client-side-proxy-class.aspx)" to generate the client proxy file.*

OData Client for .NET provides a serial of Begin/End methods to support asynchronous operations, such as executing queries and saving changes. Each Begin method takes a state parameter that can pass a state object to the callback. This state object is retrieved from the `IAsyncResult` that is supplied with the callback and is used to call the corresponding End method to complete the asynchronous operation. 

OData Client for .NET (from 6.4.0) also provides another set of asynchronous APIs in .NET 4.0 format, like `ExecuteAsync`;
 
#Asynchronous Query #

## Query an Entity Set ##

`DataServiceQuery<TElement>` provides `BeginExecute` and `EndExecute` methods to support query a collection of entities

    DefaultContainer dsc = new DefaultContainer(new Uri("http://services.odata.org/V4/(S(uvf1y321yx031rnxmcbqmlxw))/TripPinServiceRW/"));

    public void AsyncQueryAnEntitySet()
    {
        var people = dsc.People;
        people.BeginExecute(ReadingPeople, people);

        //Waiting for the Begin/End finished.
        System.Threading.Thread.Sleep(5000);
    }

    public void ReadingPeople(IAsyncResult ar)
    {
        var peopleQuery = ar.AsyncState as DataServiceQuery<Person>;
        if (peopleQuery != null)
        {
            var people = peopleQuery.EndExecute(ar);
            if (people != null)
            {
                foreach (var p in people)
                {
                    Console.WriteLine(p.UserName);
                }
            }
        }
    }

The `EndExecute` API returns an `IEnumerable<Person>`.

You also can use `DataServiceQuery<TElement>.ExecuteAsync` to query an entity set.

    public async Task AsyncAPIGetEntitySet()
    {
        var response = await dsc.People.ExecuteAsync();
        foreach (var p in (response as QueryOperationResponse<Person>))
        {
            Console.WriteLine(p.UserName);
        }
    }
 
## Query an Entity Set with Paging ##

`DataServiceContext` provides `BeginExecute` method which could take a `DataServiceQueryContinuation<TElement>` parameter to get the next page of data in a paged query result.  

    public void AsyncQueryAnEntitySetWithPaging()
    {
        var people = dsc.People;
        people.BeginExecute(ReadingPeople, people);

        //Waiting for the Begin/End finished.
        System.Threading.Thread.Sleep(10000);
    }

    public void ReadingPeople(IAsyncResult ar)
    {
        var peopleQuery = ar.AsyncState as DataServiceQuery<Person>;
        if (peopleQuery != null)
        {
            var response = peopleQuery.EndExecute(ar) as QueryOperationResponse<Person>;
            if (response != null)
            {
                foreach (var p in response)
                {
                    Console.WriteLine(p.UserName);
                }
            }

            var continuation = response.GetContinuation();
            if (continuation != null)
            {
                dsc.BeginExecute(continuation, ReadingContinuation, dsc);
            }                
        }
    }

    public void ReadingContinuation(IAsyncResult ar)
    {
        var dsc = ar.AsyncState as DataServiceContext;
        if (dsc != null)
        {
            var response = dsc.EndExecute<Person>(ar) as QueryOperationResponse<Person>;
            if (response != null)
            {
                foreach (var p in response)
                {
                    Console.WriteLine(p.UserName);
                }
            }

            var continuation = response.GetContinuation();
            if (continuation != null)
            {
                dsc.BeginExecute(continuation, ReadingContinuation, dsc);
            }
        }
    }

You also can use `DataServiceContext.ExecuteAsync` to get the next page of an entity set.

    public async Task AsyncAPIPaging()
    {
        var response = (await dsc.People.ExecuteAsync()) as QueryOperationResponse<Person>;
        foreach (var p in response)
        {
            Console.WriteLine(p.UserName);
        }

        var continuation = response.GetContinuation();
        while (continuation != null)
        {
            response = (await dsc.ExecuteAsync(continuation)) as QueryOperationResponse<Person>;
            foreach (var p in response)
            {
                Console.WriteLine(p.UserName);
            }
            continuation = response.GetContinuation();
        }
    }

## Query a Single Entity ##

`DataServiceContext` provides `BeginGetValue` and `EndGetValue` methods to support querying a single entity

    public void AsyncQueryAnEntitySet()
    {
        var person = dsc.People.ByKey("russellwhyte");
        person.BeginGetValue(ReadingPerson, person);

        //Waiting for the Begin/End finished.
        System.Threading.Thread.Sleep(5000);
    }

    public void ReadingPerson(IAsyncResult ar)
    {
        var personQuery = ar.AsyncState as DataServiceQuerySingle<Person>;
        if (personQuery != null)
        {
            var person = personQuery.EndGetValue(ar);
            Console.WriteLine(person.UserName);
        }
    }

Or, you can use `DataServiceContext.GetValueAsync` to support such query.

    public async Task AsyncAPIGetSingleEntity()
    {
        var russell = await dsc.People.ByKey("russellwhyte").GetValueAsync();
        Console.WriteLine(russell.UserName);
    }

## Query Navigation property ##

`Expand` method of `DataServiceQuery<TElement>` provides a way to query related entities. But if you want to query the navigation property separately, `DataServiceContext` provides LoadProperty method to support it. `BeginLoadProperty` and `EndLoadProperty` methods are the related asynchronous APIs.

    public void AsyncQueryNavigationProperty()
    {
        var me = dsc.Me.GetValue();
        dsc.BeginLoadProperty(me, "Trips", ReadingTrips, dsc);

        //Waiting for the Begin/End finished.
        System.Threading.Thread.Sleep(10000);
    }

    public void ReadingTrips(IAsyncResult ar)
    {
        var dsc = ar.AsyncState as DataServiceContext;
        if (dsc != null)
        {
            var response = dsc.EndLoadProperty(ar);

            if (response != null)
            {
                foreach (Trip t in response)
                {
                    Console.WriteLine(t.Name);
                }
            }
        }
    }

You can also use `DataServiceContext.LoadPropertyAsync` to query the related properties.

    public async Task AsyncAPIGetNavigation()
    {
        var me = await dsc.Me.GetValueAsync();
        
        await dsc.LoadPropertyAsync(me, "Trips");
        foreach(var t in me.Trips)
        {
            Console.WriteLine(t.Name);
        }
    }

## Query a Batch ##

`DataServiceContext` provides `BeginExecuteBatch` to put several query in a batch. The queries are specified as `DataServiceRequest<TElement>` instances. The `EndExecuteBatch` returns a `DataServiceResponse` that represents the response of the batch request as a whole. Individual query responses are represented as `DataServiceResponse` objects that can be accessed by enumerating the `DataServiceResponse` instance.

    public void AsyncQueryBatch()
    {
        var requests = new DataServiceRequest[]
        {
            dsc.People,
            dsc.Airlines
        };
        dsc.BeginExecuteBatch(ReadingBatch, dsc, requests);

        System.Threading.Thread.Sleep(5000);
    }
    
    public void ReadingBatch(IAsyncResult ar)
    {
        var dsc = ar.AsyncState as DataServiceContext;

        var response = dsc.EndExecuteBatch(ar);
        foreach (var r in response)
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

Or, you can use `ExecuteBatchAsync` to do the same thing.

	public async Task AsyncAPIExecuteBatch()
    {
        var requests = new DataServiceRequest[]
        {
            dsc.People,
            dsc.Airlines
        };
        var response = await dsc.ExecuteBatchAsync(requests);

        foreach (var r in response)
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

# Create/Update/Delete an Entity or a relationship #

`DataServiceContext` provides `BeginSaveChanges` and `EndSavechanges` methods to asynchronously submits the pending changes to the data service. Changes are added to the `DataServiceContext` by calling AddObject, UpdateObject, DeleteObject, AddLink, DeleteLink SetLink, SetSaveStream, etc.  

You can use the `SaveChangesOption` to control whether you need to send a batch request. 

##Create an entity##

    DefaultContainer dsc = new DefaultContainer(new Uri("http://services.odata.org/V4/(S(uvf1y321yx031rnxmcbqmlxw))/TripPinServiceRW/"));

    public void AsyncCreatePerson()
    {
        var person = new Person()
        {
            FirstName = "Tom",
            LastName = "White",
            UserName = "TomWhite",
        };

        dsc.AddToPeople(person);
        dsc.BeginSaveChanges(CreatingPerson, dsc);

        //Waiting for the Begin/End finished.
        System.Threading.Thread.Sleep(10000);
    }

    public void CreatingPerson(IAsyncResult ar)
    {
        var dsc = ar.AsyncState as DataServiceContext;
        if (dsc != null)
        {
            var response = dsc.EndSaveChanges(ar);
        }
    }

## Update an entity ##

    public void AsyncUpdatePerson()
    {
		// Get the single entity first.
        var personQuery = dsc.People.ByKey("TomWhite");

        var ar = personQuery.BeginGetValue(null, null);
        ar.AsyncWaitHandle.WaitOne();
        var person = personQuery.EndGetValue(ar);

        person.LastName = "Bourne";
        dsc.UpdateObject(person);
        dsc.BeginSaveChanges(ChangingPerson, dsc);

        //Waiting for the Begin/End finished.
        System.Threading.Thread.Sleep(10000);
    }

## Delete an entity ##

The code is almost the same with that in Update an entity part, you only need to change the update part to `dsc.DeleteObject(person);`

## Modify Link ##

`DataServiceContext.BeginSaveChanges` can submit the pending changes of relationships modification to service. The code is almost same as before.

## Use Async APIs to modify a entity

`DataServiceContext` provides `SaveChangesAsync` to support all the modification operation. 

	public async Task AsyncModifyEntity()
    {
        var person = new Person()
        {
            FirstName = "Alica",
            LastName = "White",
            UserName = "TomWhite",
        };

        dsc.AddToPeople(person);
        await dsc.SaveChangesAsync();

        person.FirstName = "Tom";
        dsc.UpdateObject(person);
        await dsc.SaveChangesAsync();

        dsc.DeleteObject(person);
        await dsc.SaveChangesAsync();
    }

#Read Stream#

`DataServiceContext` provides `BeginGetReadStream` and `EndGetReadStream` to support asynchronously requesting the binary data stream that belongs to the requested entity.

    public void ReadingStream(IAsyncResult ar)
    {
        var dsc = ar.AsyncState as DataServiceContext;
        
        var receiveStream = dsc.EndGetReadStream(ar).Stream;
        var sr = new StreamReader(receiveStream).ReadToEnd();

        Console.WriteLine(sr.Length);
    }

    public void AsyncAPIGetReadStream()
    {
        var task = dsc.Photos.ByKey(1).GetValueAsync();
        task.Wait();

        dsc.GetReadStreamAsync(task.Result, new DataServiceRequestArgs());
    }

You also can use `GetReadStreamAsync` to get the binary data.

	public async Task AsyncAPIGetReadStream()
    {
        var task = dsc.Photos.ByKey(1).GetValueAsync();
        task.Wait();

        var stream = (await dsc.GetReadStreamAsync(task.Result, new DataServiceRequestArgs())).Stream;
        var sr = new StreamReader(stream).ReadToEnd();

        Console.WriteLine(sr.Length);
    }