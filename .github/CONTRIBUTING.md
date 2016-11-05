# How to contribute?
There are many ways for you to contribute to OData .NET. The easiest way is to participate in discussion of features and issues. You can also contribute by sending pull requests of features or bug fixes to us. Contribution to the [documentations](http://odata.github.io/odata.net/) is also highly welcomed.

## 1. Discussion
You can participate into discussions and ask questions about OData .NET at our [Github issues](https://github.com/OData/odata.net/issues).

## 2. Bug reports
When reporting a bug at the issue tracker, fill the template of issue. The issue related to other libraries should not be reported in OData .Net library issue tracker, but be reported to other libraries' issue tracker.

## 3. Pull request for code and document contribution
**Pull request is the only way we accept code and document contribution.** Pull request of document, features and bug fixes are both welcomed. Refer to this [link](https://help.github.com/articles/using-pull-requests/) to learn details about pull request. Before you send a pull request to us, you need to make sure you've followed the steps listed below.

### Pick an issue to work on
You should either create or pick an issue on the [issue tracker](https://github.com/OData/odata.net/issues) before you work on the pull request. After the OData .NET team has reviewed this issue and change its label to "accepting pull request", you can work on the code change.

### Prepare Tools
[Atom](https://atom.io/) with package [atom-beautify](https://atom.io/packages/atom-beautify) and [markdown-toc](https://atom.io/packages/markdown-toc) is recommended to edit the document. [MarkdownPad](http://www.markdownpad.com/) can also be used to edit the document.<br />Visual Studio 2013 or 2015 is recommended for code contribution.

### Steps to create a pull request
These are the recommended steps to create a pull request:<br />

1. Create a forked repository of [https://github.com/OData/odata.net.git](https://github.com/OData/odata.net.git)
2. Clone the forked repository into your local environment
3. Add a git remote to upstream for local repository with command _git remote add upstream [https://github.com/OData/odata.net.git](https://github.com/OData/odata.net.git)_
4. Make code changes and add test cases, refer Test specification section for more details about test
5. Test the changed codes with one-click build and test script
6. Commit changed code to local repository with clear message
7. Rebase the code to upstream via command _git pull --rebase upstream master_ and resolve conflicts if there is any then continue rebase via command _git pull --rebase continue_
8. Push local commit to the forked repository
9. Create pull request from forked repository Web console via comparing with upstream.
10. Complete a Contributor License Agreement (CLA), refer below section for more details.
11. Pull request will be reviewed by Microsoft OData team
12. Address comments and revise code if necessary
13. Commit the changes to local repository or amend existing commit via command _git commit --amend_
14. Rebase the code with upstream again via command _git pull --rebase upstream master_ and resolve conflicts if there is any then continue rebase via command _git pull --rebase continue_
15. Test the changed codes with one-click build and test script again
16. Push changes to the forked repository and use _--force_ option if existing commit is amended
17. Microsoft OData team will merge the pull request into upstream

### Test specification
Now we have introduced the modern xUnit test projects for OData.NET (in `Microsoft.OData.Lite.sln`). **We strongly recommend you to write all new tests with xUnit. And it would be very kind of you to help migrate some existing MSTest cases when you modify them.** Those xUnit projects all adopt the same new intuitive structure that helps contributors to find the corresponding tests for the given product code very easily. Here are some rules to follow when you are organizing the test code:
- **Project name correspondence** (`X -> X.Tests`). For instance, all the test code of the `Microsoft.OData.Edm` project should be placed in the `Microsoft.OData.Edm.Tests` project. Path and file name correspondence. (`X/Y/Z/A.cs -> X.Tests/Y/Z/ATests.cs`). For example, the test code of the `CsdlSemanticsEntityContainer` class (in the `Microsoft.OData.Edm/Csdl/Semantics/CsdlSemanticsEntityContainer.cs` file) should be placed in the `Microsoft.OData.Edm.Tests/Csdl/Semantics/CsdlSemanticsEntityContainerTests.cs` file.
- **Namespace correspondence** (`X.Tests/Y/Z -> X.Tests.Y.Z`). The namespace of the file should strictly follow the path. For example, the namespace of the `CsdlSemanticsEntityContainerTests.cs` file should be `Microsoft.OData.Edm.Tests.Csdl.Semantics`.
- **Utility classes**. The file for a utility class can be placed at the same level of its user or a shared level that is visible to all its users. But the file name must **NOT** be ended with `Tests` to avoid any confusion. For example, `Microsoft.OData.Core.Tests/UriParser/TokenAssertions.cs` is a utility class used by several test classes under the folder `Microsoft.OData.Core.Tests/UriParser`.
- **Integration and scenario tests**. Those tests usually involve multiple modules and have some specific scenarios. They should be placed separately in `X.Tests/IntegrationTests` and `X.Tests/ScenarioTests`. There is no hard requirement of the folder structure for those tests. But they should be organized logically and systematically as possible. Please see `Microsoft.OData.Core.Tests/ScenarioTests` for reference.

### Complete a Contribution License Agreement (CLA)
You will need to complete a Contributor License Agreement (CLA). Briefly, this agreement testifies that you are granting us permission to use the submitted change according to the terms of the project's license, and that the work being submitted is under appropriate copyright.

Please submit a Contributor License Agreement (CLA) before submitting a pull request. Download the agreement ([Microsoft Contribution License Agreement.pdf](https://github.com/odata/odatacpp/wiki/files/Microsoft Contribution License Agreement.pdf)), sign, scan, and email it back to [cla@microsoft.com](mailto:cla@microsoft.com). Be sure to include your Github user name along with the agreement. Only after we have received the signed CLA, we'll review the pull request that you send. You only need to do this once for contributing to any Microsoft open source projects.
