1. Install Visual Studio 2019 Enterprise 16.11 configuring the installer with the `.vsconfig` file [here](./OData.E2E.vsconfig) (other versions and SKUs of Visual Studio 2019 should also work, but these instructions were tested with the version specified here). Continue through any prompts about components that are out-of-support. You can find more information about using `.vsconfig` files [here](https://docs.microsoft.com/en-us/visualstudio/install/import-export-installation-configurations?view=vs-2022)
2. Install [.NET Core SDK 1.1.14](https://dotnet.microsoft.com/download/dotnet/1.1)
3. Install [.NET Core SDK 2.1.818](https://dotnet.microsoft.com/download/dotnet/2.1)
4. Despite being documented as included in .NET Core SDK 2.1.818, install [.NET Core Runtime 2.1.30](https://dotnet.microsoft.com/download/dotnet/2.1)
5. Create a local directory for the repository, then run `git clone https://github.com/OData/odata.net.git` from the command prompt in that directory. NOTE: A build issue currently exists which generates extremely long paths that cause the build to fail. You should try to keep your root local directory as short as possible, for example `c:\github`, resulting in the clone being at `c:\github\odata.net`. Much longer root directories will cause build failures. 
6. From an adminitrator command prompt, enable windows long paths by installing [tools\EnableLongPaths.reg](../tools/EnableLongPaths.reg). You can do this by simply running `EnableLongPaths.reg` as a command. Continue through any prompts
7. From an adminitrator command prompt, open [odata.net/sln/OData.E2E.sln](OData.E2E.sln) using the previously installed Visual Studio
8. In Visual Studio, go to `Tools -> NuGet Package Manager -> Package Manager Console`
9. In the package manager console, run `Install-Package xunit.runner.visualstudio -Version 2.4.1`
10. Close Visual Studio, then re-open [odata.net/sln/OData.E2E.sln](OData.E2E.sln) from an administrator command prompt
11. In Visual Studio, right click `Solution 'OData.E2E' -> Build Solution`
12. Open the test explorer window from `Test -> Test Explorer`
13. Select all of the tests and `right click -> Run`
