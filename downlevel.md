### Consuming .NET features in down-level ODL versions

There are often features in the latest versions of .NET that we want to consume in ODL. However, all versions of ODL are not on the same version of .NET, so not all .NET features may be accessible to each ODL version that needs support. In such cases, we should copy the entire file containing the feature and include it in the ODL repository. The file should be put at `./opensource/{directorystructure}/{filename}.{extension}` with an additional file at `./opensource/{directorystructure}/{filename}.md` which contains the URL that the file was copied from. The [Microsoft IP Separation guidelines](https://docs.opensource.microsoft.com/policies/ip-separation/) also calls out additional requirements that need to be adhered to on a case-by-case basis; in the event that a header is not included in the original source file and the guidelines require one, the header should be included in the `.md` file. 

This file will be included in the appropriate `.csproj`. It can be excluded from the latest version (to not conflict with the feature shipped with .NET) by conditionally compiling it:

```
<ItemGroup> <!--TODO make additive so that we don't have increased maintenance over time; also have a work item in the "follow-through" to remove these lines and the no longer relevant source file-->
  <Compile Condition="'$(TargetFramework)' == '{framework_version_that_has_feature}'" Remove="Class1.cs" />
</ItemGroup>
```

### Follow-through work

Once we are depending on the source instead of the framework, we are now responsible for any security fixes that are introduced to the feature. We should create tracking work items to check if there are updates to the feature every 3 months, and pull those updates into our repo if necessary. If the URL of the source file has changed, update the `.md` file with the new URL. 

### Limitations

1. This tactic only works if the .NET feature does not depend on a CLR feature. In this case, some other mechanism will need to be used.
2. A .NET feature can depend upon other, new .NET features. If a feature is contained within a single file, we should always take that file. If the feature requires bringing in more than one file, it will be update to the contributor and reviewers of the ODL code change to make the judgment call on how to proceed. 
3. Preprocessor directives can cause issues when bringing in a source file. If a source file contains these directives, then that means that a framework version is compiled for a specific SKU. If ODL does not ship a package that targets *all* of those SKUs, the the source file cannot be taken. Note that, currently, .NET does not use preprocessor directives in the C# libraries; you can verify this by [searching](https://github.com/dotnet/runtime/search?q=filename%3A.cs+ifdef&type=code) the github repo.
4. The Microsoft [open source guidelines](https://docs.opensource.microsoft.com/) should be followed *in addition* to this document, specifically the "Policies & guidance" section

### Future

1. We might be able to use git modules to pull source files, take updates to the module whenever a file is modified.
2. Github does not explicitly have a way to subscribe to changes made in a file. We could add something like this to our build process to regularly check for changes in an automated way. 
3. We should be on the lookout for when a conflict might arise between existing ODL code/resources and the .NET files. This is a living document and may need to be modified as issues arise. 
