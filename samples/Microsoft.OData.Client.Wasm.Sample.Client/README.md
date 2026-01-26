<!-- markdownlint-disable MD002 MD041 -->

# Microsoft.OData.Client.Wasm.Sample.Client

This is the Blazor WebAssembly client application for testing Microsoft.OData.Client async APIs in a WebAssembly environment.

## Important: How to Run This Application

### Recommended Approach (Run Server Only)

**You do NOT need to run this Client project directly.** Instead, simply run the **Server project**, which will automatically serve both the OData API and this Blazor WASM client:

```
cd samples/Microsoft.OData.Client.Wasm.Sample.Server
dotnet run
```

Then navigate to `http://localhost:5023/` in your browser.

### Why Run the Server Project?

The Server project is configured to:
- [x] Host the OData API at `/odata`
- [x] Automatically build and serve this Blazor WASM client at the root URL `/`
- [x] Handle all routing and static file serving for the client
- [x] Provide a single unified development experience

### Alternative: Running Both Projects Separately (If Preferred)

If you need to run the projects separately (e.g., for debugging purposes), you can:

1. **Run the Server project first:**
```
   cd samples/Microsoft.OData.Client.Wasm.Sample.Server
   dotnet run
```

2. **Then run this Client project:**
```
   cd samples/Microsoft.OData.Client.Wasm.Sample.Client
   dotnet run
```

3. **Update the service URI** in `wwwroot/appsettings.json` if the server runs on a different port:
```json
   { "ODataWasmServiceUri": "http://localhost:5023/odata/" }
```

However, this approach requires:
- Managing two separate processes
- Ensuring CORS is properly configured
- Manually coordinating port numbers

**Again, it's much simpler to just run the Server project.**

## What This Sample Tests

This Blazor WebAssembly application provides an interactive UI for testing **all async methods** in the Microsoft.OData.Client library. Each button in the UI corresponds to a specific async API operation that sends requests to the OData service.

### Tested Async APIs

The sample covers the following categories of async operations:

#### Entity Operations
- `DataServiceContext.SaveChangesAsync()`
- `DataServiceContext.SaveChangesAsync(SaveChangesOptions)`
- `DataServiceContext.BulkUpdateAsync()`
- `DataServiceContext.DeepInsertAsync()`

#### Query Operations
- `DataServiceQuerySingle<T>.GetValueAsync()`
- `DataServiceContext.ExecuteAsync<T>()`
- `DataServiceContext.ExecuteBatchAsync()`

#### Function & Query Operations
- `DataServiceFunctionQuery<T>.ExecuteAsync()`
- `DataServiceQuery.ExecuteAsync()`
- `DataServiceQuery<T>.GetAllPagesAsync()`
- `DataServiceQuery<T>.EnumerateAllPagesAsync()`

#### Navigation & Loading
- `DataServiceContext.LoadPropertyAsync(object, string)`
- `DataServiceContext.LoadPropertyAsync(object, string, Uri)`
- `DataServiceContext.LoadPropertyAsync(object, string, DataServiceQueryContinuation)`
- `LoadPropertyAllPagesAsync()` (implemented inline using the public `LoadPropertyAsync` continuation-paging loop)

#### Stream Operations
- `DataServiceContext.GetReadStreamAsync(object, DataServiceRequestArgs)`
- `DataServiceContext.GetReadStreamAsync(object, string, DataServiceRequestArgs)`

#### Action Operations
- `DataServiceActionQuerySingle<T>.GetValueAsync()`

## Project Structure

```
Microsoft.OData.Client.Wasm.Sample.Client/
├── Pages/
│   └── Home.razor           # Main test UI with all async operation tests
├── Layout/
│   ├── MainLayout.razor     # Application layout
│   └── NavMenu.razor        # Navigation menu
├── Connected Services/      # Generated OData client proxies
├── wwwroot/                 # Static assets
└── README.md                # This file
```

## Development Notes

- The client uses the **generated OData client proxy** from the Server's metadata endpoint
- All operations use `async/await` to demonstrate proper WebAssembly async patterns
- The service URI is read from `wwwroot/appsettings.json` (`ODataWasmServiceUri`) — update that file if the server runs on a different port. The URI must include a trailing slash to ensure proper relative URI construction.
- Random IDs are generated for entity creation operations to avoid conflicts

## Related Projects

- **Server Project**: `Microsoft.OData.Client.Wasm.Sample.Server` - The OData API backend and WASM host
- **OData.NET**: The core OData libraries being tested

## License

Copyright (c) .NET Foundation and Contributors. All rights reserved.
See License.txt in the project root for license information.
