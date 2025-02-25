# Introduction
When adopting new features from recent .NET releases in OData.net, we may encounter pre-release APIs that could be useful to integrate. These pre-release versions often interface with public APIs but may not yet offer backporting options for older .NET versions. However, leveraging them through preprocessor directives is a common practice in OData.net and other .NET projects.


## Example : Adding new API present only in future .NET versions

```cs
// Default API for lowest version that we compile for e.g. in main NET8.0
IEdmEntitySet FindEntitySet(string setName);

// New API that targets a new version of .Net framework
#if NET9_0_OR_GREATER
IEdmEntitySet FindEntitySet(ReadOnlyMemory<char> setName);
#endif
```
However we also will need to start providing support for newer dotnet versions with the new release plan and in order to support these we could also adopt a few new practices for consistency.

Over time, these APIs will be fully implemented using stable features in .NET 9. However, there are cases where exposing a pre-release API can provide immediate benefits, such as improving performance or enabling early adoption of upcoming .NET features.

## Motivation for Using Pre-Release APIs
Pre-release APIs may:

* Improve performance: Offer optimizations such as reduced memory allocations.
* Enhance future compatibility: Provide a migration path for upcoming .NET releases.
* Enable experimentation: Allow users to try out new features before they become stable.

These APIs will eventually have full implementations that leverage stable .NET 9 features. However, there are cases where exposing a new API earlier can be beneficial—both for improving our projects and for preparing for upcoming .NET framework versions that are still finalizing preview features.  

Preview APIs may:  
- Become fully supported in future stable releases.  
- Undergo modifications or redesigns before finalization.  
- Help maintain compatibility and code parity with existing stable APIs while offering immediate benefits.  

### **Example: Optimizing URI Parsing for Zero Allocations**  
Consider a scenario where we want to introduce a new API to achieve **zero allocations** while parsing a URI.  

#### **Current Implementation**  
```csharp
public abstract class ODataPathSegment
{
    /// <summary>Returns the identifier for this segment, i.e., the string part without the keys.</summary>
    public string Identifier { get; set; }
    // ... redacted
}

// This references an interned string, so memory usage is optimized.
this.Identifier = navigationProperty.Name;

// However, we perform lookups before creating PathSegments using FindProperty, FindEntitySet, and FindSingleton in ODataUriResolver.
// Currently, resolving each segment requires a Find operation or string manipulation.

// We can achieve the same functionality more efficiently by using .NET 9 Find methods,
// which perform zero allocations and avoid performance penalties.
```

### **Adopting Preview APIs and Migration Strategy**  
To adopt these new APIs efficiently, we may need to:  
- Replace certain existing APIs and fields with non-allocating alternatives while ensuring compatibility with stable APIs.  
- Target upcoming APIs that are expected to be available by the final release date.  
- Introduce **gated preview APIs** that users can opt into, allowing early testing before official support in .NET 9 and .NET 10.  

By carefully integrating these preview features, we can enhance performance while maintaining long-term support for stable .NET versions.  

### Migration Strategy

## **Migration Strategy**  
### **Examples**  

### **Case 1: Incremental Adoption (Self-Use)**  
In some scenarios, users may want to introduce changes gradually, allowing services or other projects to start using them before the official release.  
To facilitate this, **nightly or preview builds** can be gated using the [`[Experimental]` attribute](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-12.0/experimental-attribute) introduced in .NET 8.  

#### **Proposal**  
- Introduce new features in a **non-breaking** way to maintain compatibility with existing APIs.  
- Ensure that any experimental API does not disrupt current functionality while allowing early adopters to test and provide feedback.  

### **Case 2: Stable APIs and Breaking Changes**  
If an API is **public**, changing its signature is a breaking change. However, we may still want to enable users on newer frameworks to benefit from improvements while maintaining support for older implementations.  

#### **Decision Points**  
- Should we provide an **alternative API** alongside the existing one?  
- Can we use **preprocessor directives** or conditional compilation to introduce changes only in newer framework versions?  
- Should we introduce an **opt-in mechanism**, such as preview attributes, to control adoption?  

By carefully planning the migration strategy, we can balance innovation with stability, ensuring a smooth transition for users across different .NET versions.  

### **Migration**
#### **Case: Stable API's are used**
** API is public and changing signature is breaking** but we also want to allow users using the old API in new frameworks to also benefit from the new API that we are adding which considerations should we make 

Do we choose 

**A**
```cs
    public static IEdmTerm FindTerm(this IEdmModel model, string qualifiedName)
    {
        EdmUtil.CheckArgumentNull(model, "model");
        EdmUtil.CheckArgumentNull(qualifiedName, "qualifiedName");

        string fullyQualifiedName = model.ReplaceAlias(qualifiedName);
#if NET9_0_OR_GREATER
        return FindAcrossModels(
            model,
            fullyQualifiedName.AsSpan(),
            findTerm,
            (first, second) => RegistrationHelper.CreateAmbiguousTermBinding(first, second));
#else
        return FindAcrossModels( // call underlying method
            model,
            fullyQualifiedName,
            findTerm,
            (first, second) => RegistrationHelper.CreateAmbiguousTermBinding(first, second));
#endif
    }

#if NET9_0_OR_GREATER 
    // introduce new api
    public static IEdmTerm FindTerm(this IEdmModel model, string qualifiedName)
        => FindAcrossModels(
            model,
            fullyQualifiedName,
            findTerm,
            (first, second) => RegistrationHelper.CreateAmbiguousTermBinding(first, second));
#endif
```
_OR_

**B**
```cs
        public static IEdmTerm FindTerm(this IEdmModel model, string qualifiedName)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(qualifiedName, "qualifiedName");

            string fullyQualifiedName = model.ReplaceAlias(qualifiedName);
#if NET9_0_OR_GREATER 
            return FindTerm( /* call new API */
                model,
                fullyQualifiedName.AsSpan(),
            );
#else
            return FindAcrossModels(
                model,
                fullyQualifiedName,
                findTerm,
                (first, second) => RegistrationHelper.CreateAmbiguousTermBinding(first, second));
#endif
        }

#if NET9_0_OR_GREATER 
    // introduce new api which is stable
    public static IEdmTerm FindTerm(this IEdmModel model, string qualifiedName)
        => FindAcrossModels(
            model,
            fullyQualifiedName,
            findTerm,
            (first, second) => RegistrationHelper.CreateAmbiguousTermBinding(first, second));
#endif

```
or 

**C**
```cs
        public static IEdmTerm FindTerm(this IEdmModel model, string qualifiedName)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(qualifiedName, "qualifiedName");

            string fullyQualifiedName = model.ReplaceAlias(qualifiedName);

            return FindAcrossModels(
                model,
                fullyQualifiedName,
                findTerm,
                (first, second) => RegistrationHelper.CreateAmbiguousTermBinding(first, second));
        }

#if NET9_0_OR_GREATER 
    // introduce new api which is stable and user gets to choose what behaviour they want to opt into.
    public static IEdmTerm FindTerm(this IEdmModel model, string qualifiedName)
        => FindAcrossModels(
            model,
            fullyQualifiedName,
            findTerm,
            (first, second) => RegistrationHelper.CreateAmbiguousTermBinding(first, second));
    // Our libraries that consume the library will also opt in on whatever behaviour they need.
#endif

```


### ** Private and internal APIS **
These are a bit different that publicly expose API's and because of this we can opt to fully switch over or have directives that change the signatures and types.
For these we can afford to change the signatures or introduce a new parallel api during the transition period.  

**A**

```cs
// This is a private api and we can change the signature and body between the supported versions of the framework.
// Note: This is possible for very simple methods but for more complex methods we may need to introduce a new api as shown in B.
#if NET9_0_OR_GREATER 
        private static IEdmTerm FindTerm(this IEdmModel model, ReadOnlySpan<char> qualifiedName) // modifies the signature between the two and modifies the body when required.
#else
        private static IEdmTerm FindTerm(this IEdmModel model, string qualifiedName)
#endif
        {    
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(qualifiedName, "qualifiedName");
#if NET9_0_OR_GREATER 
            ReadOnlySpan<char> fullyQualifiedName = model.ReplaceAlias(qualifiedName); // modifies body
#else
            string fullyQualifiedName = model.ReplaceAlias(qualifiedName);
#endif
            return FindAcrossModels(
                model,
                fullyQualifiedName,
                findTerm,
                (first, second) => RegistrationHelper.CreateAmbiguousTermBinding(first, second));

        }
```

**B**
```cs
// This is a private api and we can change the signature and body between the supported versions of the framework.
// However there is a bit of duplication in the bodies of the methods and in some cases we may need to also support more methods for the same functionallity to work.
#if NET9_0_OR_GREATER 
        private static IEdmTerm FindTerm(this IEdmModel model, ReadOnlySpan<char> qualifiedName) { /*Body*/} // modifies the signature between the two
#else
        private static IEdmTerm FindTerm(this IEdmModel model, string qualifiedName) { /*Body*/}
#endif
```

### **Targeting an underlying api that is still in preview or releasing api in preview mode**
When our codebase depends on a preview feature, we need to carefully manage its usage and communicate its status to users.
Another case would be when users want to have a pre-release version with the API specification

**Scenario**
We may encounter cases where:

* A preview API is available, and we want to use it in our project.
* We want to introduce a new API that relies on a preview feature.

**Considerations**
* Should we mark all APIs that depend on a preview feature with [Experimental](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-12.0/experimental-attribute), or only those that are uncertain and may not be supported in the future?
* If a preview feature is critical to our implementation, should we provide an alternative fallback in case it is removed? e.g. In the case of Net9 AlternateLookup if _ReadOnlySpan_ was not supported we could use the string or _ReadOnlyMemory_ api's to keep the feature before releasing the library.
* Should these preview features be in a partial class that can be removed or migrated to the main class when we promote the API to GA?
e.g.

```cs
public abstract partial class ODataPathSegment{} //  stable api's inside ODataPathSegment.cs

public abstract partial class ODataPathSegment
{
    // api that may not be fully ready before the next stable release or is using a preview sdk.
} //  inside ODataPathSegment.Future|Proposed|FeatureName.cs

```

**Note**: This approach enables early adoption of platform features before their official release, allowing us to prepare AspNetCoreOData for upcoming .NET versions while maintaining compatibility with current stable releases. This is particularly valuable for AspNetCoreOData as it can leverage new platform capabilities to improve performance and functionality as soon as they become available in preview builds.


## Related

- [Proposal: ExperimentalAttribute](https://github.com/dotnet/runtime/issues/31542)
- [Experimental](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-12.0/experimental-attribute)
