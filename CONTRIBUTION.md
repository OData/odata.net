#How to contribute?
There are many ways for you to contribute to OData.NET. The easiest way is to participate in discussion of features and issues. You can also contribute by sending pull requests of features or bug fixes to us. Contribution to the documentations at our [Wiki](https://github.com/OData/odata.net/wiki) is also highly welcomed. 
##Discussion
You can participate into discussions and ask questions about OData.NET at our [Github issues](https://github.com/OData/odata.net/issues). 
###Bug reports
When reporting a bug at the issue tracker, please use the following template:
```
### Description
*Does the bug result in any actual functional issue, if so, what?*  

### Minimal repro steps
*What is the smallest, simplest set of steps to reproduce the issue. If needed, provide a project that demonstrates the issue.*  

### Expected result
*What would you expect to happen if there wasn't a bug*  

### Actual result
*What is actually happening*  

### Further technical details
*Optional, details of the root cause if known*  
```

##Pull requests
Pull request of features and bug fixes are both welcomed. Before you send a pull request to us, there are a few steps you need to make sure you've followed. 
###Complete a Contribution License Agreement (CLA)
You will need to complete a Contributor License Agreement (CLA). Briefly, this agreement testifies that you are granting us permission to use the submitted change according to the terms of the project's license, and that the work being submitted is under appropriate copyright.

Please submit a Contributor License Agreement (CLA) before submitting a pull request. Download the agreement ([Microsoft Contribution License Agreement.pdf](https://github.com/odata/odatacpp/wiki/files/Microsoft Contribution License Agreement.pdf)), sign, scan, and email it back to [cla@microsoft.com](mailto:cla@microsoft.com). Be sure to include your Github user name along with the agreement. Only after we have received the signed CLA, we'll review the pull request that you send. You only need to do this once for contributing to any Microsoft open source projects. 

###Create a new issue on the issue tracker and link the pull request to it
You should have an issue created on the [issue tracker](https://github.com/OData/odata.net/issues) before you work on the pull request. After the OData.NET team has reviewed this issue and change its label to "accepting pull request", you can issue a pull request to us in which the link to the related issue is included.
###Requirement of pull requests
Your pull request should:

 - Include a description of what your change intends to do
 - Have clear commit messages
 - Include a link to the issue created at the issue tracker or its issue number
 - Include adequate tests

###Test specification
Now we have introduced the modern xUnit test projects for OData.NET (in `Microsoft.OData.Lite.sln`). **We strongly recommend you to write all new tests with xUnit. And it would be very kind of you to help migrate some existing MSTest cases when you modify them.** Those xUnit projects all adopt the same new intuitive structure that helps contributors to find the corresponding tests for the given product code very easily. Here are some rules to follow when you are organizing the test code:

 - **Project name correspondence** (`X -> X.Tests`). For instance, all the test code of the `Microsoft.OData.Edm` project should be placed in the `Microsoft.OData.Edm.Tests` project.
 - Path and file name correspondence. (`X/Y/Z/A.cs -> X.Tests/Y/Z/ATests.cs`). For example, the test code of the `CsdlSemanticsEntityContainer` class (in the `Microsoft.OData.Edm/Csdl/Semantics/CsdlSemanticsEntityContainer.cs` file) should be placed in the `Microsoft.OData.Edm.Tests/Csdl/Semantics/CsdlSemanticsEntityContainerTests.cs` file.
 - **Namespace correspondence** (`X.Tests/Y/Z -> X.Tests.Y.Z`). The namespace of the file should strictly follow the path. For example, the namespace of the `CsdlSemanticsEntityContainerTests.cs` file should be `Microsoft.OData.Edm.Tests.Csdl.Semantics`.
 - **Utility classes**. The file for a utility class can be placed at the same level of its user or a shared level that is visible to all its users. But the file name must **NOT** be ended with `Tests` to avoid any confusion. For example, `Microsoft.OData.Core.Tests/UriParser/TokenAssertions.cs` is a utility class used by several test classes under the folder `Microsoft.OData.Core.Tests/UriParser`.
 - **Integration and scenario tests**. Those tests usually involve multiple modules and have some specific scenarios. They should be placed separately in `X.Tests/IntegrationTests` and `X.Tests/ScenarioTests`. There is no hard requirement of the folder structure for those tests. But they should be organized logically and systematically as possible. Please see `Microsoft.OData.Core.Tests/ScenarioTests` for reference.