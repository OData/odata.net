1. Increment the version number referenced during build in [`Versioning.props`](tools/CustomMSBuild/Versioning.props) by using [semantic versioning](https://semver.org/). Also copy the contents of any `PublicAPI.Unshipped.txt` file that has changed since the last release into its corresponding `PublicAPI.Shipped.txt` file. Both of these steps can usually be accomplished by running the `tools\IncrementVersion.cmd` script. Create a pull request with these changes and merge it.
2. Kick off a new [nightly build](https://identitydivision.visualstudio.com/OData/_build?definitionId=1104) to generate the nuget packages that will be published for this release:
3. Generate release notes in the [change log](https://github.com/MicrosoftDocs/OData-docs/blob/main/Odata-docs/changelog/odatalib-7x.md) to be published on release of the new version. This is done by referencing the PRs that have been merged into `master` since the last version increment. In github, each commit should have a link to the PR that was merged to generate that commit, so you can use github history to generate the changelog. Create a pull request with these changes in the docs repo and merge it. 
4. Download the build artifacts from the nightly build to your local machine
5. Mark the nightly build to be retained indefinitely
6. Download NuGet Package Explorer from the windows store and use it to verify the URLs of each package for license, release notes, and package information
7. Log into to your nuget.org account and link it to your AAD account. Ask someone from the team to add you as an administrator. You will need to accept this request once it is sent to you
8. Create a new API key for this release and limit the lifetime so that it won't be re-used
9. Verify the signatures of the packages downloaded from the build artifacts by running the following command: `nuget verify -Signature Microsoft.Spatial.7.4.4.nupkg`. Do the same for Microsoft.OData.Edm, Microsoft.OData.Core, and Microsoft.OData.Client
10. Upload all 4 nupkg files using the nuget.org UI in the following order:
    1. Microsoft.Spatial
    2. Microsoft.OData.Edm
    3. Microsoft.OData.Core
    4. Microsoft.OData.Client
11. Upload all 4 snupkg files in the same order
12. Create a new tag corresponding to the new version number by running:
    ```
    git tag -a 6.16.0 -m "6.16.0 RTM"
    git push origin 6.16.0
    ```
    and repacing `6.16.0` with the new version number
13. Create a [new release](https://github.com/OData/odata.net/releases) in github with the name "ODL 7.9.4" replacing "7.9.4" with the new version number. Also choose the newly created tag. Add links to the new nuget packages in the description. Note that the assets files will be generated automatically by github when the release is created
14. Cherry-pick the change log commit in the docs repo from the `main` branch to the `live` branch
