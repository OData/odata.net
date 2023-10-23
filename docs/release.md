# Release Procedure

## Prerequisites
* Administrator account on NuGet
    1. Log into your nuget.org account and link it to your Entra ID account by signing in with your Microsoft work account.
    2. Ask someone from the team to add you as an administrator on NuGet. You will need to accept this request once it is sent to you via email.
* NuGet Package Explorer
    1. Download [NuGet Package Explorer](https://apps.microsoft.com/store/detail/nuget-package-explorer/9WZDNCRDMDM3) from the Windows store.
* NuGet Command Line Interface
    1. Install according to [these instructions](https://learn.microsoft.com/en-us/nuget/reference/nuget-exe-cli-reference).

## Directions

### Initiate the Release
1. Increment the version number and update the Public API files; both of these steps can be accomplished by running the `tools\IncrementVersion.cmd` script. Then create a pull request with these changes using the `tools\Release\CreateAndPushBranch.cmd` script and merge the pull request.
    * Increment the version number referenced during build in [`Versioning.props`](tools/CustomMSBuild/Versioning.props) by using [semantic versioning](https://semver.org/).
    * Update the Public API files by copying the contents of any `PublicAPI.Unshipped.txt` file that has changed since the last release into its corresponding `PublicAPI.Shipped.txt` file.
2. Kick off a new [nightly build](https://identitydivision.visualstudio.com/OData/_build?definitionId=1104) by running the pipeline to generate the NuGet packages that will be published for this release.
3. Generate release notes in the [change log](https://github.com/MicrosoftDocs/OData-docs/blob/main/Odata-docs/changelog/odatalib-7x.md) to be published on release of the new version. Then create a pull request with these changes in the [Docs](https://github.com/MicrosoftDocs/OData-docs/) repo and merge it.
    * Generate new release notes by referencing the PRs that have been merged into `master` since the last version increment. In GitHub, each commit should have a link to the PR that was merged to generate that commit, so you can use GitHub history to generate the changelog.

### Create the NuGet Packages
4. Download the build artifacts from the nightly build to your local machine.
    * Access the artifacts by navigating to the "published" items in the "Scheduled" section of the summary page for the new build.
5. Mark the nightly build to be retained indefinitely.
    * Find this setting by clicking the button with three dots next to the title of the build.
6. Use NuGet Package Explorer to verify the URLs of each package in the Nuget-Release folder for license terms, package information, and release notes.
    * Expected URLs:
        * License Terms: http://go.microsoft.com/fwlink/?linkid=833178 (forwards to https://raw.githubusercontent.com/OData/odata.net/master/LICENSE.txt)
        * Package Information: http://odata.github.io/ (forwards to https://learn.microsoft.com/en-us/odata/)
        * Release Notes: http://docs.microsoft.com/en-us/odata/changelog
    * Note: Symbols packages will not have license terms and Nightly packages will not have release notes.
7. Verify the signatures of the packages downloaded from the build artifacts by running the following command: `nuget verify -Signature Microsoft.Spatial.7.9.4.nupkg`, replacing "7.9.4" with the new version number. Do the same for Microsoft.OData.Edm, Microsoft.OData.Core, and Microsoft.OData.Client.

### Upload the NuGet Packages
8. Create a new API key on nuget.org for this release and limit the lifetime so that it won't be re-used.
    * Limit the lifetime by setting the key to expire in one day (for example).
    * You can use the default setting for scopes when creating the API key.
9. Upload all 4 nupkg files using the nuget.org UI in the following order:
    1. Microsoft.Spatial
    2. Microsoft.OData.Edm
    3. Microsoft.OData.Core
    4. Microsoft.OData.Client
10. Upload all 4 snupkg files in the same order.

### Document the Release
11. Create a new tag on the `master` branch in the [ODL repo](https://github.com/OData/odata.net) corresponding to the new version number by running:
    ```
    git tag -a 7.9.4 -m "7.9.4 RTM"
    git push origin 7.9.4
    ```
    and repacing `7.9.4` with the new version number
12. Create a [new release](https://github.com/OData/odata.net/releases) in GitHub with the name "ODL 7.9.4", replacing "7.9.4" with the new version number. Also choose the newly created tag. Add links to the new NuGet packages in the description. (Note that the assets files will be generated automatically by GitHub when the release is created.)
13. Cherry-pick the change log commit in the [Docs](https://github.com/MicrosoftDocs/OData-docs/) repo from the `main` branch to the `live` branch.
