### Consuming .NET features in down-level ODL versions

There are often features in the latest versions of .NET that we want to consume in ODL. However, all versions of ODL are not on the same version of .NET, so not all .NET features may be accessible to each ODL version that needs support. In such cases, we should copy the entire file containing the feature and include it in the ODL repository. This file will be included in the appropriate `.csproj`. It can be excluded from the latest version (to not conflict with the feature shipped with .NET) by conditionally compiling it:

```
<ItemGroup> <!--TODO make additive so that we don't have increased maintenance over time-->
  <Compile Condition="'$(TargetFramework)' == '{framework_version_that_has_feature}'" Remove="Class1.cs" />
</ItemGroup>
```

### Follow-through work

Once we are depending on the source instead of the framework, we are now responsible for any security fixes that are introduced to the feature. We should create tracking work items to regularly check if there are updates to the feature, and to pull those updates into our repo if necessary. 

### Limitations

1. This tactic only works if the .NET feature does not depend on a CLR feature. In this case, some other mechanism will need to be used.
2. A .NET feature can depend upon other, new .NET features. We will only take a file if it doesn't require bringing in too many other files. 

### Open questions

1. How often should we check for updates?
2. How should we record the URL that the file was taken from?
3. What limit do we want to place on nested "new feature" dependencies?
