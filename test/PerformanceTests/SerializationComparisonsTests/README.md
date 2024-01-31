# Serialization Comparison Performance Tests

This solution contains tests that compare different configurations for `ODataMessageWriter` and `IJsonWriter` implementations. It also includes `JsonSerializer` and `Utf8JsonWriter` to provide a lower-bound or baseline.

All the writers are evaluated based on writing a simple collection response to an output stream. The collection contains synthetic customer objects that include primitive and complex properties. The complex properties are used to evaluate nested resources. The string values contain some non-ASCII characters to force string escaping.

The solution can be used to run two types of tests:
- Basic benchmarks based on [BenchmarkDotNet](https://benchmarkdotnet.org/) where each of the registered writer writes the same payload to a file and in-memory stream.
- Load tests that simulate client-server requests, where you select the writer that should handle the responses.

The easiest way to run these tests is to use the crank configurations in the root directory of this repository (`cd ../../..`), i.e. `benchmarks.yml` and `loadtests.yml`.

The different writers are defined in the `Lib` project: [Lib/DefaultWriterCollection.cs](./Lib/DefaultWriterCollection.cs).

## Basic benchmarks

These benchmarks are defined in the [`JsonWriterBenchmarks`](./JsonWriterBenchmarks/) project.

To run the benchmarks, go to the root of the repository (`cd ../../..`) and run the following command:

```
crank --config benchmarks.yml --profile lab-windows --scenario SerializationComparisons
```

This will run the benchmarks on the lab machines. To run them locally, use `--profile local` instead.

Here are sample results:

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.20348
Intel Xeon E-2336 CPU 2.90GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=        5.0.404 [C:\Program Files\dotnet\sdk]
  [Host] : .NET 6.0.7 (6.0.722.32202), X64 RyuJIT

Toolchain=InProcessEmitToolchain  InvocationCount=1  UnrollFactor=1

|           Method |                                           WriterName |       Mean |     Error |    StdDev |     Median |      Gen 0 |     Gen 1 |  Allocated |
----------------- |----------------------------------------------------- |-----------:|----------:|----------:|-----------:|-----------:|----------:|-----------:|
 WriteToFileAsync |                                       JsonSerializer |  31.501 ms | 0.6233 ms | 1.0916 ms |  30.733 ms |          - |         - |      12 KB |
 WriteToFileAsync |                                   ODataMessageWriter | 281.088 ms | 0.5729 ms | 0.5078 ms | 280.970 ms | 41000.0000 |         - | 254,692 KB |
 WriteToFileAsync |                             ODataMessageWriter-Async | 907.453 ms | 4.1577 ms | 3.4718 ms | 906.944 ms | 66000.0000 | 1000.0000 | 406,552 KB |
 WriteToFileAsync |                      ODataMessageWriter-NoValidation | 245.621 ms | 0.5507 ms | 0.4881 ms | 245.605 ms | 37000.0000 |         - | 231,252 KB |
 WriteToFileAsync |                ODataMessageWriter-NoValidation-Async | 860.340 ms | 2.2663 ms | 2.1199 ms | 860.096 ms | 62000.0000 | 1000.0000 | 382,899 KB |
 WriteToFileAsync |                    ODataMessageWriter-Utf8JsonWriter | 268.592 ms | 0.5407 ms | 0.5058 ms | 268.590 ms | 40000.0000 | 1000.0000 | 248,268 KB |
 WriteToFileAsync |              ODataMessageWriter-Utf8JsonWriter-Async | 480.797 ms | 0.7170 ms | 0.6707 ms | 480.779 ms | 44000.0000 | 1000.0000 | 273,436 KB |
 WriteToFileAsync |       ODataMessageWriter-Utf8JsonWriter-NoValidation | 232.998 ms | 0.2320 ms | 0.2057 ms | 232.957 ms | 36000.0000 | 1000.0000 | 224,828 KB |
 WriteToFileAsync | ODataMessageWriter-Utf8JsonWriter-NoValidation-Async | 434.364 ms | 0.4606 ms | 0.4309 ms | 434.374 ms | 40000.0000 | 1000.0000 | 249,996 KB |

By default, there's a filter applied that only runs the "ToFile" tests. If you want to run the in-memory tests, or apply a different filter, you can pass the `filter` variable manually to the `crank` command:

```
crank --config benchmarks.yml --profile lab-windows --scenario SerializationComparisons --variable filter=*Memory*
```

There's also an option to include "raw values" in the test, i.e. using `IJsonWriter.WriteRawValue()` (occurs if there are `ODataUntyped` values in the resources). This was option was added because `ODataUtf8JsonWriter.WriteRawvalue()`'s implementation requires special handling which may impact performance.

```
crank --config benchmarks.yml --profile lab-windows --scenario SerializationComparisons --variable filter=*RawValues*
```

## Load tests

The load tests are based on a simple AspNetCore service defined in the [TestServer](./TestServer/) project. It defines an endpoint that returns a collection of `Customer` objects. It allows you to specify the writer to use to serialize the request's response as well as the number of entries to return.

The client is based on [Bombardier](https://github.com/codesenberg/bombardier), a simple load-testing tool that sends a number of concurrent requests to a server, over a configured period time, then displays stats like latency, requests/s, total requests, etc.

Here's an example:

```
crank --config .\loadtests.yml --scenario SerializationComparisons --profile lab-windows  --application.options.counterProviders System.Runtime --variable writer=ODataMessageWriter
```

Here's a sample response:

| application                             |               |
| --------------------------------------- | ------------- |
| CPU Usage (%)                           | 100           |
| Cores usage (%)                         | 1,202         |
| Working Set (MB)                        | 203           |
| Private Memory (MB)                     | 360           |
| Build Time (ms)                         | 5,691         |
| Start Time (ms)                         | 259           |
| Published Size (KB)                     | 101,574       |
| .NET Core SDK Version                   | 6.0.301       |
| ASP.NET Core Version                    | 6.0.6+68bb6fb |
| .NET Runtime Version                    | 6.0.6+7cca709 |
| Max CPU Usage (%)                       | 102           |
| Max Working Set (MB)                    | 212           |
| Max GC Heap Size (MB)                   | 104           |
| Size of committed memory by the GC (MB) | 150           |
| Max Number of Gen 0 GCs / sec           | 56.00         |
| Max Number of Gen 1 GCs / sec           | 15.00         |
| Max Number of Gen 2 GCs / sec           | 1.00          |
| Max Time in GC (%)                      | 8.00          |
| Max Gen 0 Size (B)                      | 8,139,272     |
| Max Gen 1 Size (B)                      | 4,320,976     |
| Max Gen 2 Size (B)                      | 16,491,440    |
| Max LOH Size (B)                        | 4,942,352     |
| Max POH Size (B)                        | 1,322,200     |
| Max Allocation Rate (B/sec)             | 5,103,750,072 |
| Max GC Heap Fragmentation               | 37            |
| # of Assemblies Loaded                  | 120           |
| Max Exceptions (#/s)                    | 0             |
| Max Lock Contention (#/s)               | 34            |
| Max ThreadPool Threads Count            | 32            |
| Max ThreadPool Queue Length             | 58            |
| Max ThreadPool Items (#/s)              | 19,253        |
| Max Active Timers                       | 0             |
| IL Jitted (B)                           | 380,282       |
| Methods Jitted                          | 5,286         |


| load                   |         |
| ---------------------- | ------- |
| CPU Usage (%)          | 14      |
| Cores usage (%)        | 166     |
| Working Set (MB)       | 42      |
| Private Memory (MB)    | 110     |
| Start Time (ms)        | 93      |
| First Request (ms)     | 185     |
| Requests               | 240,750 |
| Bad responses          | 0       |
| Latency 50th (ms)      | 15.92   |
| Latency 75th (ms)      | 16.17   |
| Latency 90th (ms)      | 16.50   |
| Latency 95th (ms)      | 16.82   |
| Latency 99th (ms)      | 17.85   |
| Mean latency (ms)      | 15.95   |
| Max latency (ms)       | 183.83  |
| Requests/sec           | 8,028   |
| Requests/sec (max)     | 17,554  |
| Read throughput (MB/s) | 110.61  |

The first table, `application`, displays metrics collected on the server. You may have noticed that we passed an `--application.options.counterProviders System.Runtime` option to the command. This instructs crank to collect event counters using the [System.Runtime](https://docs.microsoft.com/en-us/dotnet/core/diagnostics/available-counters#systemruntime-counters) provider. Doing this adds additional metrics to the output (e.g. thread information, GC and heap information, etc.).

The second table, `load`, displays metrics collected on the client. This includes the requests data collected by `Bombardier`.

In the example above we were using the `ODataMessageWriter` writer to handle responses. You can specify a different using the `--variable writer=x` variable. For example, to use `ODataMessageWriter-Utf8JsonWriter` we would use the following command:

```
crank --config .\loadtests.yml --scenario SerializationComparisons --profile lab-windows  --application.options.counterProviders System.Runtime --variable writer=ODataMessageWriter-Utf8JsonWriter
```

Results:

| application                             |               |
| --------------------------------------- | ------------- |
| CPU Usage (%)                           | 100           |
| Cores usage (%)                         | 1,205         |
| Working Set (MB)                        | 186           |
| Private Memory (MB)                     | 343           |
| Build Time (ms)                         | 5,437         |
| Start Time (ms)                         | 263           |
| Published Size (KB)                     | 101,574       |
| .NET Core SDK Version                   | 6.0.301       |
| ASP.NET Core Version                    | 6.0.6+68bb6fb |
| .NET Runtime Version                    | 6.0.6+7cca709 |
| Max CPU Usage (%)                       | 102           |
| Max Working Set (MB)                    | 194           |
| Max GC Heap Size (MB)                   | 102           |
| Size of committed memory by the GC (MB) | 123           |
| Max Number of Gen 0 GCs / sec           | 64.00         |
| Max Number of Gen 1 GCs / sec           | 18.00         |
| Max Number of Gen 2 GCs / sec           | 2.00          |
| Max Time in GC (%)                      | 24.00         |
| Max Gen 0 Size (B)                      | 16,073,800    |
| Max Gen 1 Size (B)                      | 4,861,864     |
| Max Gen 2 Size (B)                      | 17,368,952    |
| Max LOH Size (B)                        | 4,418,008     |
| Max POH Size (B)                        | 1,754,688     |
| Max Allocation Rate (B/sec)             | 5,747,380,304 |
| Max GC Heap Fragmentation               | 54            |
| # of Assemblies Loaded                  | 120           |
| Max Exceptions (#/s)                    | 0             |
| Max Lock Contention (#/s)               | 36            |
| Max ThreadPool Threads Count            | 36            |
| Max ThreadPool Queue Length             | 77            |
| Max ThreadPool Items (#/s)              | 19,879        |
| Max Active Timers                       | 0             |
| IL Jitted (B)                           | 382,853       |
| Methods Jitted                          | 5,311         |


| load                   |         |
| ---------------------- | ------- |
| CPU Usage (%)          | 13      |
| Cores usage (%)        | 155     |
| Working Set (MB)       | 41      |
| Private Memory (MB)    | 110     |
| Start Time (ms)        | 91      |
| First Request (ms)     | 184     |
| Requests               | 252,387 |
| Bad responses          | 0       |
| Latency 50th (ms)      | 14.83   |
| Latency 75th (ms)      | 16.16   |
| Latency 90th (ms)      | 18.15   |
| Latency 95th (ms)      | 19.70   |
| Latency 99th (ms)      | 23.36   |
| Mean latency (ms)      | 15.21   |
| Max latency (ms)       | 161.76  |
| Requests/sec           | 8,412   |
| Requests/sec (max)     | 12,376  |
| Read throughput (MB/s) | 118.45  |

It's always important to look at the `Bad responses` metric before making any conclusions from the other stats.
If Bad responses is 0, it means all requests were successful (i.e. 2xx response codes). If it's non-zero, it means there were failed requests. Common causes of failed requests include passing an unknown writer to the `--variable writer=...` variable and flooding the server with more requests than it can handle. To mitigate against the latter, you can try to limit the load on the server by limiting the number of concurrent connections and/or the max request rate (number of requests per second). Different servers can handle different capacities, so may need to play around with different values until you find the highest load that results in 0 bad responses.

The following command limits the number of concurrent connections to 128 and the request rate to 200 requets/s.

```
crank --config .\loadtests.yml --scenario SerializationComparisons --profile lab-windows  --application.options.counterProviders System.Runtime --variable writer=ODataMessageWriter-Utf8JsonWriter --variable connections=128 --variable rate=200
```

The following command includes raw values (`ODataUntypeValue` and `IJsonWriter.WriteRawValue`) in the payload:

```
crank --config .\loadtests.yml --scenario SerializationComparisons --profile lab-windows  --application.options.counterProviders System.Runtime --variable includeRawValues=true --variable writer=ODataMessageWriter
```

Another important metric to look at when trying to make sense of results is the **CPU Usage**. If it's 100% it means all the CPUs were busy throughout. If the value is low it means the CPUs were not doing a lot of work. This can be an indication that the bottleneck in the application is I/O. **Cores Usage** is a similar metric, but is not normalized. If you have 12 CPU cores, the total Cores Usage will be 1200%, total CPU Usage will still be 100%.

In the results above, the CPU usage is high because the latency of the network requests is low. This is because in the selected profile (`lab-windows`), both client and server machines are in the same local network with fast links. So the server spent most of its time building the responses rather than waiting for network transfers to complete.

### Load testing across the internet

In a real-world scenario, the client and server may be separated by a much slower internet connection. To simulate this scenario, we can run the server on a remote machine, for example on Azure. This would require setting up a crank-agent on a machine on Azure. The `loadtests.yml` config defines a `remote-windows` profile which runs the client locally and the server on a machine that you specify. You have manually edit the `loadtests.yml` with the remote server's IP address.

For the purposes of this example, I created a Windows VM with 4 cores on Azure, installed .NET on the machine then installed and ran the `crank-agent` service. I configured the firewall to allow the server to accept remote HTTP requests via ports 5010 (where crank-agent is running) and 5000 (where the test service will run). You should take precautions to secure the VM and prevent unauthorized access since crank-agent allows code to be uploaded and executed on the server.

Here's an example running the following command on the `remote-windows` profile:

```
crank --config .\loadtests.yml --scenario SerializationComparisons --profile remote-windows  --application.options.counterProviders System.Runtime --variable writer=ODataMessageWriter-Utf8JsonWriter
```

| application                             |               |
| --------------------------------------- | ------------- |
| CPU Usage (%)                           | 38            |
| Cores usage (%)                         | 151           |
| Working Set (MB)                        | 160           |
| Private Memory (MB)                     | 306           |
| Build Time (ms)                         | 28,064        |
| Start Time (ms)                         | 4,267         |
| Published Size (KB)                     | 101,575       |
| .NET Core SDK Version                   | 6.0.301       |
| ASP.NET Core Version                    | 6.0.6+68bb6fb |
| .NET Runtime Version                    | 6.0.6+7cca709 |
| Max CPU Usage (%)                       | 43            |
| Max Working Set (MB)                    | 167           |
| Max GC Heap Size (MB)                   | 93            |
| Size of committed memory by the GC (MB) | 108           |
| Max Number of Gen 0 GCs / sec           | 5.00          |
| Max Number of Gen 1 GCs / sec           | 2.00          |
| Max Number of Gen 2 GCs / sec           | 1.00          |
| Max Time in GC (%)                      | 3.00          |
| Max Gen 0 Size (B)                      | 96            |
| Max Gen 1 Size (B)                      | 9,171,536     |
| Max Gen 2 Size (B)                      | 4,992,784     |
| Max LOH Size (B)                        | 737,592       |
| Max POH Size (B)                        | 1,523,072     |
| Max Allocation Rate (B/sec)             | 382,207,480   |
| Max GC Heap Fragmentation               | 22            |
| # of Assemblies Loaded                  | 120           |
| Max Exceptions (#/s)                    | 384           |
| Max Lock Contention (#/s)               | 22            |
| Max ThreadPool Threads Count            | 17            |
| Max ThreadPool Queue Length             | 0             |
| Max ThreadPool Items (#/s)              | 2,374         |
| Max Active Timers                       | 0             |
| IL Jitted (B)                           | 395,685       |
| Methods Jitted                          | 5,458         |


| load                   |          |
| ---------------------- | -------- |
| CPU Usage (%)          | 2        |
| Cores usage (%)        | 18       |
| Working Set (MB)       | 28       |
| Private Memory (MB)    | 26       |
| Start Time (ms)        | 104      |
| First Request (ms)     | 1,058    |
| Requests               | 16,780   |
| Bad responses          | 0        |
| Latency 50th (ms)      | 216.05   |
| Latency 75th (ms)      | 217.24   |
| Latency 90th (ms)      | 221.05   |
| Latency 95th (ms)      | 228.61   |
| Latency 99th (ms)      | 715.48   |
| Mean latency (ms)      | 229.66   |
| Max latency (ms)       | 1,542.93 |
| Requests/sec           | 555      |
| Requests/sec (max)     | 2,302    |
| Read throughput (MB/s) | 7.70     |

As you can see, the average latency here about 15x more than on the `lab-windows` profile. The CPU Usage on the server is also much lower (34% vs 100%). This indicates that the CPU was idle most of the time.

Since the CPU was idle, maybe it has capacity to serve more requests while waiting for network transfers to complete. We can test this by increasing the number of concurrent connections to 512 (the default is 128).

```
crank --config .\loadtests.yml --scenario SerializationComparisons --profile remote-windows  --application.options.counterProviders System.Runtime --variable writer=ODataMessageWriter-Utf8JsonWriter --variable connections=512
```

| application                             |               |
| --------------------------------------- | ------------- |
| CPU Usage (%)                           | 98            |
| Cores usage (%)                         | 391           |
| Working Set (MB)                        | 173           |
| Private Memory (MB)                     | 316           |
| Build Time (ms)                         | 28,287        |
| Start Time (ms)                         | 4,173         |
| Published Size (KB)                     | 101,575       |
| .NET Core SDK Version                   | 6.0.301       |
| ASP.NET Core Version                    | 6.0.6+68bb6fb |
| .NET Runtime Version                    | 6.0.6+7cca709 |
| Max CPU Usage (%)                       | 99            |
| Max Working Set (MB)                    | 180           |
| Max GC Heap Size (MB)                   | 96            |
| Size of committed memory by the GC (MB) | 119           |
| Max Number of Gen 0 GCs / sec           | 15.00         |
| Max Number of Gen 1 GCs / sec           | 4.00          |
| Max Number of Gen 2 GCs / sec           | 1.00          |
| Max Time in GC (%)                      | 6.00          |
| Max Gen 0 Size (B)                      | 18,174,056    |
| Max Gen 1 Size (B)                      | 3,217,984     |
| Max Gen 2 Size (B)                      | 15,647,624    |
| Max LOH Size (B)                        | 1,130,808     |
| Max POH Size (B)                        | 6,343,472     |
| Max Allocation Rate (B/sec)             | 1,308,268,920 |
| Max GC Heap Fragmentation               | 60            |
| # of Assemblies Loaded                  | 120           |
| Max Exceptions (#/s)                    | 1,533         |
| Max Lock Contention (#/s)               | 62            |
| Max ThreadPool Threads Count            | 15            |
| Max ThreadPool Queue Length             | 97            |
| Max ThreadPool Items (#/s)              | 4,836         |
| Max Active Timers                       | 0             |
| IL Jitted (B)                           | 395,907       |
| Methods Jitted                          | 5,466         |


| load                   |           |
| ---------------------- | --------- |
| CPU Usage (%)          | 10        |
| Cores usage (%)        | 78        |
| Working Set (MB)       | 36        |
| Private Memory (MB)    | 47        |
| Start Time (ms)        | 102       |
| First Request (ms)     | 1,051     |
| Requests               | 32,897    |
| Bad responses          | 0         |
| Latency 50th (ms)      | 240.17    |
| Latency 75th (ms)      | 435.79    |
| Latency 90th (ms)      | 944.92    |
| Latency 95th (ms)      | 1,299.65  |
| Latency 99th (ms)      | 4,036.39  |
| Mean latency (ms)      | 462.74    |
| Max latency (ms)       | 12,617.31 |
| Requests/sec           | 1,223     |
| Requests/sec (max)     | 9,497     |
| Read throughput (MB/s) | 14.66     |