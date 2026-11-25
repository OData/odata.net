# ODL 9 Release Planning

## Project Goal
The objective of ODL 9 is to enhance the OData .NET Libraries by addressing significant issues and implementing key features identified as major release opportunities.

This release aims to improve performance, security, and usability, ensuring the libraries meet the evolving needs of developers and organizations. The General Availability (GA) release is scheduled for November 2025, with multiple preview, beta, and release candidate versions planned to gather feedback and ensure stability.

## Scope
The scope of ODL 9 includes evaluating and potentially incorporating the issues in the table below. Each item will be assessed based on its impact, feasibility, and alignment with the project's goals.

## .NET Framework Support
ODL 9 will align with modern .NET platform support to ensure compatibility and long-term maintainability. The release will target **.NET 10 (LTS) Preview** during development. The framework will be updated to target **.NET 10** after its officially release.
  - .NET 10 Preview was released on Feb 25 2025
  - .NET 10 General Availability (GA) is expected on Nov 11, 2025

## Proposed Items for Inclusion
| Item | Details | Library | Status | Remark |
| ---- | ----------- | ------- | ------ | ------ |
| **[2908](https://github.com/OData/odata.net/issues/2908): Remove `OdataMessageInfo` from the DI container** | It is pointless to have this injectable as a service since we overwrite all the properties of the resolved instance. | Core | In scope - breaking change | |
| **[2907](https://github.com/OData/odata.net/issues/2907): Remove `IContainerProvider` and pass the `IServiceProvider` directly to the message reader or writer** |  |  Core | In scope - breaking change | Consult with Robert McLaws to gather his input on how best to redesign dependency injection in the writers and readers |
| **[2208](https://github.com/OData/odata.net/issues/2208): Remove `DataServiceContext` `KeyComparisonGeneratesFilterQuery` flag in the next breaking change release** | In ODL 8, the default value was set to true and the flag was marked as obsolete |  Client | In scope - breaking change | In next 8.x minor release, update obsolete message for customers to use `ByKey` to generate query by key expression |
| **[3066](https://github.com/OData/odata.net/issues/3066): Default `EnableCaseInsensitive` to true when initializing a new instance of `ODataUriResolver`** | The call to `AddRouteComponents` in ASP.NET Core OData configures an `ODataUriResolver` with `EnableCaseInsensitive` set to true but when `AddDefaultODataServices` is called, it configures the same property with the value false |  Core | In scope - breaking change | Default behaviour for creating an `ODataUriResolver` should be `EnableCaseInsensitive = true`. Customers can change behaviour by injecting an `ODataUriResolver` with `EnableCaseInsensitive` set to false. Expand this to align the defaults for ASP.NET Core OData with the defaults for ODL. Also align with the OData standard where possible. Default constructor for should initialize the properties to the right defaults |
| **[2801](https://github.com/OData/odata.net/issues/2801): Proposal to create model.Find methods that accept `ReadOnlySpan<&lt;>char>`** | To reduce allocations resulting from splitting path into segments |  Core | In scope - breaking change | |
| ~~**[2816](https://github.com/OData/odata.net/issues/2816): Use `ValueTask<T>` instead of `Task<T>` for async I/O APIs**~~ |  |  Core | In Scope? | Can it be broken down into smaller manageable sizes? |
| **[2657](https://github.com/OData/odata.net/issues/2657): Resolve untyped numeric value as `Edm.Double` (not `Edm.Decimal` or `Edm.Int32`)** | Currently, untyped/dynamic numeric property is resolved into `Edm.Decimal` if the property is single-valued or `Edm.Int32` if the property is collection-valued. Per the protocol, if there’s no type information, we have to determine the type heuristically – resolve it as an `Edm.Double`. **NOTE**: Change should be behind a feature flag that customers can use to retain the old behaviour. UPDATE: Consider controlling behaviour via injectable service. |  Core | In scope | |
| **[2881](https://github.com/OData/odata.net/issues/2881): Implement cleaner approach to use of character array pool in `JsonWriter`** | Redesign logic around use of character array pool to eliminate coupling of char array pool with the `ODataJsonWriter` |  Core | In scope - breaking change | |
| **[2430](https://github.com/OData/odata.net/issues/2430): `ODataJsonWriter` should use `Encoding.CreateTranscodingStream` in .NET 6+** | Swap `TranscodingStream` with built-in `Encoding.CreateTranscodingStream` used to convert UTF-8 to other encodings |  Core | Tech debt - non-breaking change | |
| **[2894](https://github.com/OData/odata.net/issues/2894): EdmLib successfully parses type names with unbalanced parens** | Bug in Edm lib causes the parser to parse type names with unbalance parens, e.g., `“Collection(NS.MyType”` |  Edm | In scope | |
| **[2420](https://github.com/OData/odata.net/issues/2420): Use `Utf8JsonWriter.WriteRawValue` to implement `ODataUtf8JsonWriter.WriteRawValue` in .NET 6+** | Current implementation of `ODataUtf8JsonWriter.WriteRawValue` manually writes to the stream. Refactor to use `Utf8JsonWriter.WriteRawValue` available in .NET 6+ |  Core | Tech debt - non-breaking change | |
| **[3085](https://github.com/OData/odata.net/issues/3085): Change type of `ReturnType` of `IEdmOperation` to use `IEdmOperationReturn`** | Use `IEdmOperationReturn` introduced in 7.x as the type for `IEdmOperation.ReturnType` |  Edm | In scope - breaking change  | |
| **[2911](https://github.com/OData/odata.net/issues/2911): `CustomUriFunctions` should not be static or irreversible** |  |  Core |  | Related to [3158](https://github.com/OData/odata.net/issues/3158), [2712](https://github.com/OData/odata.net/issues/2712)? |
| **[3158](https://github.com/OData/odata.net/issues/3158): `CustomUriLiteralParsers` and `CustomUriLiteralPrefixes` should not be static or irreversible** |  |  Core |  | Related to [2911](https://github.com/OData/odata.net/issues/2911), [2712](https://github.com/OData/odata.net/issues/2712)? |
| **[3064](https://github.com/OData/odata.net/issues/3064): POST payload generated by OData client contains `@odata.type` property annotations for both declared and dynamic enum properties** | Fix bug where OData client generates `@odata.type` property annotations for declared enum properties which are not required and cause the payload to be verbose |  Client | In scope - breaking change | |
| **[3082](https://github.com/OData/odata.net/issues/3082): Implement full support for `DateOnly` and `TimeOnly` in OData Client and deprecate `Date` and `TimeOfDay`** | Support for `DateOnly` and `TimeOnly` is already implemented in OData Core. For OData Client to achieve parity, **OData Connected Service** should be refactored to emit `DateOnly` and `TimeOnly` properties (controlled by a setting to toggle legacy behaviour?) and OData Client adapted to work with the new types |  Client | In scope | Marking `Date` and `TimeOfDay` as obsolete can be done in ODL 8 immediately. Updating OCS to emit `DateOnly` and `TimeOnly` is not a breaking change but might require a flag to allow user to decide which set of types to emit. OCS work should be done prior to ODL 9 release. Dropping `Date` and `TimeOfDay` is a breaking change. After the legacy types are dropped, OCS should be updated to only emit `DateOnly` and `TimeOnly` types. |
| **[3020](https://github.com/OData/odata.net/issues/3020): OData Client supports nullable generic types for enum as key** | Fix bug where OData client supports nullable enum property as a key |  Client | In scope - breaking change | |
| **[2479](https://github.com/OData/odata.net/issues/2479): Revisit design of OData URI resolvers into something more composable** | We have resolvers for alternate key, enum as string, etc. However, they are not composable – if you want to support multiple conventions, you have to write URI resolvers for each combination |  Core | In scope | |
| **[2395](https://github.com/OData/odata.net/issues/2395): Swapping Microsoft.Spatial with .NET Topology Suite** | Support for spatial types was introduced in EF Core 2.2 via the **.NET Topology Suite** library. EF Core maps the **.NET Topology Suite** spatial types onto SQL database spatial types. **Microsoft.Spatial** types on the other hand have no direct mapping to SQL database spatial types. To improve end-to-end user experience when working with spatial types, it makes sense to replace **Microsoft.Spatial** library with the **.NET Topology Suite** |  Core | In scope - breaking change, potentially | Feature could be implemented behind a feature flag |
| ~~**[2696](https://github.com/OData/odata.net/issues/2696): Rationalize `ODataUri` and `ODataUriSlim` into a single type**~~ |  |  Core |  | |
| **[2714](https://github.com/OData/odata.net/issues/2714): `ODataReader` fails reading count for a non-expanded navigation property** | Support reading/writing control information (i.e., count) and annotations on non-expanded collections |  Core |  | |
| **[3147](https://github.com/OData/odata.net/issues/3147): Improve APIs for `IEdmModel` look ups and traversal** |  |  Edm | In scope | Related to [2382](https://github.com/OData/odata.net/issues/2382), [2480](https://github.com/OData/odata.net/issues/2480), [2095](https://github.com/OData/odata.net/issues/2095)? |
| ~~**[2508](https://github.com/OData/odata.net/issues/2508): `ODataResource` allocates unnecessary lists of actions and functions**~~ |  |  Core | In scope - non-breaking change | Not a breaking change |
| **[3104](https://github.com/OData/odata.net/issues/3104): Allow `ReadAsStreamFunc` delegate to access property annotations** | To make it possible to check for particular property annotation and read the property as a stream |  Core | In scope - breaking change | Internal? `ReadAsStreamFunc` property currently accepts a predicate `Func` which will be called for each property, and should return `true` for properties which should be read as a stream instead of inline. This doesn't work well when the property is dynamic. The ask if for `ReadAsStreamFunc` to have access to property annotations such that one can use that to determine when to read a property as a stream. For example, read all property with `is.Large` annotation as stream |
| **[3100](https://github.com/OData/odata.net/issues/3100): Allow `ODataUtf8JsonWriter` buffer size to be configurable by the user** | To enable buffer size to be configurable so a customer can control how frequent stream I/O is invoked |  Core | In scope - breaking change | Internal? |
| **[2882](https://github.com/OData/odata.net/issues/2882): Implement a cleaner approach to support for buffering in `JsonWriter`** | To eliminate a hack in `ODataJsonLightOutputContext` where we check if the writer is an `ODataUtfJsonWriter` before using a buffering stream |  Core | In scope - breaking change | |
| **[2727](https://github.com/OData/odata.net/issues/2727): `DataServiceQuerySingle<T>.GetValue` throws `InvalidOperationException`** | Because `GetValue/GetValueAsync` internally calls `Single()` instead or `SingleOrDefault()`, an `InvalidOperationException` exception is thrown if an entity is not found. |  Client |  | Consider changing the behavior? Would there be any ramifications? To consider: If the action with a nullable return type successfully returns `null`, we should return `null`. However, if the action with a nullable return type fails, we should throw. Otherwise callers would not be able to differentiate between successful calls that return `null` and failed calls  |
| **[2717](https://github.com/OData/odata.net/issues/2717): Action and function with the same name in the CSDL cause problems in MS Graph** |  |  Core |  | Should this be implemented and controlled by a setting? |
| **[2662](https://github.com/OData/odata.net/issues/2662): ODL can not read the top-level untyped collection** |  |  Core | In scope | Resolve numeric values in untyped collection as double ([2657](https://github.com/OData/odata.net/issues/2657))? |
| **[2562](https://github.com/OData/odata.net/issues/2562): Fix logic for `FindNavigationTarget` in major release** | Fix for how binding path works for contained navigation properties was included in 7.4.2. For backward compatibility, the legacy behavior was retained. This needs clean up in a major release. See [`EdmContainedEntitySet.FindNavigationTarget`](https://github.com/OData/odata.net/blob/aba9a63ec5ca1cf8180ab83df8fe953f018e4785/src/Microsoft.OData.Edm/Schema/EdmContainedEntitySet.cs#L147) and [`ExtensionMethods.TryGetRelativeEntitySetPath`](https://github.com/OData/odata.net/blob/aba9a63ec5ca1cf8180ab83df8fe953f018e4785/src/Microsoft.OData.Edm/ExtensionMethods/ExtensionMethods.cs#L3277). Relevant pull request [1109](https://github.com/OData/odata.net/pull/1109), relevant test [`CollectionOfExpandedEntities_Version741AndBefore`](https://github.com/OData/odata.net/blob/aba9a63ec5ca1cf8180ab83df8fe953f018e4785/test/UnitTests/Microsoft.OData.Core.Tests/ScenarioTests/Roundtrip/ContextUrlWriterReaderTests.cs#L787). |  Core | In scope? | V7.4.1 expected the path to be prefixed with the path to the contained navigation source. For backward compatibility, if the binding path received starts with the path to this contained resource, we trim it off and then treat the remainder as the path to the target. This logic should be removed in the next breaking change as it could be ambiguous in the case that the prefix of the path to the contained source matches a valid path to the target of the contained source. Might require significant redesign. Related to [1114](https://github.com/OData/odata.net/issues/1114). |
| **[2478](https://github.com/OData/odata.net/issues/2478): Remove `JSONP` support** | `JSONP` feature was marked as obsolete in ODL 8. Drop it altogether in ODL 9 |  Core | In scope - breaking change | |
| ~~**[2530](https://github.com/OData/odata.net/issues/2530): Schema annotations not included in vocabulary annotations for model**~~ | Edm model does not load in-line schema annotations and errors when out-of-line annotations are included |  Edm | Not a breaking change? | |
| ~~**[2064](https://github.com/OData/odata.net/issues/2064): Explore improving perf around `ICollection<T>.Contains()`**~~ | See [`ClientPropertyAnnotation`](https://github.com/OData/odata.net/blob/aba9a63ec5ca1cf8180ab83df8fe953f018e4785/src/Microsoft.OData.Client/Metadata/ClientPropertyAnnotation.cs#L172) class |  Core |  | |
| **[2105](https://github.com/OData/odata.net/issues/2105): AddLink/SetLink to a navigation property in a complex type** | Support for navigation properties on complex types was added in [1743](https://github.com/OData/odata.net/pull/1743). However, there is currently no way to apply it to a complex type in OData client |  Client | In scope? | |
| **[1218](https://github.com/OData/odata.net/issues/1218): Remove explicit `HttpVersion` check in `ODataMultipartMixedBatchReader`** | See [`HttpVersionInBatching`](https://github.com/OData/odata.net/blob/aba9a63ec5ca1cf8180ab83df8fe953f018e4785/src/Microsoft.OData.Core/ODataConstants.cs#L73) constant |  Core | In scope | |
| **[3181](https://github.com/OData/odata.net/issues/3181): Remove obsolete `ODataMessageReaderSettings.ReadUntypedAsString property`** |  |  Core | In scope - breaking change | |
| **[3182](https://github.com/OData/odata.net/issues/3182): Remove obsolete `DataServiceContext.Timeout` property** |  |  Client | In scope | |
| **[3183](https://github.com/OData/odata.net/issues/3183): Resolve the `SYSLIB0051` obsoletion warning reported on `DataServiceClientException`** |  |  Client | In scope | |
| Redesign OData serializer to use System.Text.Json JsonSerializer |  |  Core | In scope | |
| **[2067](https://github.com/OData/odata.net/issues/2067): Redesign OData Uri Parser** |  |  Core | In scope? | |
| **[3229](https://github.com/OData/odata.net/issues/3229): Support navigation property bindings ending in a cast segment** | For example, all directory objects that are users can be bound to /DirectoryObjects/Namespace.User, and all directory objects that are groups can be bound to /DirectoryObjects/Namespace.Group |  Core | In scope - breaking change, potentially - due to redesign that might be necessary | Internal |
**[2076](https://github.com/OData/odata.net/issues/2076): Add a validation rule for checking and validating navigation paths and property paths in annotations** | OData validation rules do not currently check if annotations containing navigation properties or property paths are valid or reachable |  Core | In scope - breaking change  | |

[//]: # "**[](https://github.com/OData/odata.net/issues/): ** |  |  Core |  | |"

## Milestones
### 1. Assessment Phase (February - March 2025)
  - Review each proposed item to determine its relevance and priority for the ODL 9 release.
  - Conduct feasibility analysis to assess the complexity and resources required for implementation.

### 2. Development Phase (April - September 2025)
  - Implement selected features and address prioritized issues.
  - Perform internal testing to ensure stability and performance improvements.

### 3. Preview Releases (June 2025)
  - Release a preview version to gather feedback from the community.
  - Address any critical issues identified during the preview phase.

### 4. Beta Releases (August 2025)
 - Release a beta version incorporating feedback from the preview phase.
  - Continue to monitor and address any reported issues.

### 5. Release Candidate (October 2025)
  - Finalize features and perform thorough testing.
  - Prepare documentation and resources for the GA release.

### 6. General Availability (November 2025)
  - Officially release ODL 9 to the public.
  - Provide ongoing support and address any post-release issues.

## Next Steps
1. **Team Discussion**: Convene to discuss the proposed items and determine which should be included in the ODL 9 scope.
2. **Community Engagement**: Engage with the developer community to gather feedback on proposed changes and identify additional areas for improvement.
3. **Resource Allocation**: Assign team members to specific tasks based on expertise and project priorities.

By carefully selecting and implementing these enhancements, ODL 9 aims to provide a robust and efficient framework that meets the needs of its users and supports the continued growth of the OData ecosystem.
