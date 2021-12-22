1. Install Visual Studio 2019 Enterprise 16.11 configuring the installer with the `.vsconfig` file [here](./odata.vsconfig) (other versions and SKUs of Visual Studio 2019 should also work, but these instructions were tested with the version specified here). Continue through any prompts about components that are out-of-support. You can find more information about using `.vsconfig` files [here](https://docs.microsoft.com/en-us/visualstudio/install/import-export-installation-configurations?view=vs-2022)
2. Install [.NET Core SDK 1.1.14](https://dotnet.microsoft.com/download/dotnet/1.1)
3. Install [.NET Core SDK 2.1.818](https://dotnet.microsoft.com/download/dotnet/2.1)
4. Despite being documented as included in .NET Core SDK 2.1.818, install [.NET Core Runtime 2.1.30](https://dotnet.microsoft.com/download/dotnet/2.1)
5. Create a local directory for the repository, then run `git clone https://github.com/OData/odata.net.git` from the command prompt in that directory
6. From an adminitrator command prompt, open [odata.net/sln/odata.sln](OData.sln) using the previously installed Visual Studio
7. In Visual Studio, go to `Tools -> NuGet Package Manager -> Package Manager Console`
8. In the package manager console, run `Install-Package xunit.runner.visualstudio -Version 2.4.1`
9. Undo any changes that might now exist in your git working tree
10. Close Visual Studio, then re-open [odata.net/sln/odata.sln](OData.sln) from an administrator command prompt
11. In Visual Studio, right click `Solution 'OData' -> Build Solution`
12. Open the test explorer window from `Test -> Test Explorer`
13. Select all of the tests and `right click -> Run`; NOTE: The `Microsoft.OData.PublicApi.Tests` tests will not run unless individually selected. They will also fail if run. This issue is currently being tracked [here](https://github.com/OData/odata.net/issues/2284)
