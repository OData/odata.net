<!-- markdownlint-disable MD002 MD041 -->

# Microsoft.OData.Client.Wasm.Sample.Server

This is the server project for the OData Client WebAssembly sample application. It provides both the OData API backend and hosts the Blazor WebAssembly client application.

## Running the Application

### Quick Start (Recommended)

Simply run this Server project, and both the OData API and the Blazor WASM client will be available:

```
cd samples/Microsoft.OData.Client.Wasm.Sample.Server
dotnet run
```

Once running, navigate to:
- **Blazor WASM Client**: `http://localhost:5023/` (or the port shown in your terminal)
- **OData Service**: `http://localhost:5023/odata`
- **OData Metadata**: `http://localhost:5023/odata/$metadata`

### What This Sample Demonstrates

This sample application provides an interactive UI for testing **Microsoft.OData.Client async APIs** in a WebAssembly environment. The client application includes test buttons for the following async operations:

#### Entity Operations
- `SaveChangesAsync` - Add entities
- `SaveChangesAsync` with batch options
- `BulkUpdateAsync` - Update entities
- `DeepInsertAsync` - Insert entity graphs

#### Query Operations
- `GetValueAsync` - Retrieve single entities
- `ExecuteAsync` - Execute filtered queries
- `ExecuteBatchAsync` - Execute batch requests

#### Function & Query Operations
- `ExecuteAsync` - Call OData functions
- `GetAllPagesAsync` - Retrieve all pages of paged results

#### Navigation & Loading
- `LoadPropertyAsync` - Load navigation properties
- `LoadPropertyAsync` with continuation tokens
- `LoadPropertyAllPagesAsync` - Load all pages of related entities

#### Stream Operations
- `GetReadStreamAsync` - Read media entity streams
- `GetReadStreamAsync` - Read named streams

#### Action Operations
- `GetValueAsync` - Execute OData actions returning scalars

### Project Structure

```
Microsoft.OData.Client.Wasm.Sample.Server/
├── Controllers/          # OData controllers for Customers, Orders, Media
├── Data/                 # In-memory data source
├── Models/               # Entity models (Customer, Order, MediaAsset)
├── Program.cs            # Application configuration and startup
└── README.md             # This file
```

### Development Notes

- The server uses an **in-memory data source** that can be reset using the "Reset Data Source" button in the client UI
- The OData service is configured with **OData v4.01** protocol
- **CORS** is enabled for all origins in development mode
- The server automatically serves the Blazor WASM static files from the Client project

### Advanced: Running Separately

While not recommended, you can run the Server and Client projects separately:

1. Run the Server project (this project)
2. Run the Client project separately
3. Update the Client's `serviceUri` to point to the Server's URL

However, running just the Server project is simpler and provides the same functionality.

## Related Projects

- **Client Project**: `Microsoft.OData.Client.Wasm.Sample.Client` - The Blazor WebAssembly frontend
- **OData.NET**: The core OData libraries being tested

## License

Copyright (c) .NET Foundation and Contributors. All rights reserved.
See License.txt in the project root for license information.
