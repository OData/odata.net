.NET support rules:
1. ODL does not support .NET versions that are out-of-support
2. .NET support has been tested prior to publishing a version with that .NET support
3. removing support for a .NET version is a breaking change (and therefore requires a major version increment)
4. adding support for a .NET version is not a breaking change
5. ODL major versions are support for at least 1 year after release (TODO this claim was made in the last meeting, but i can't find in the existing doc)

open items:
1. How do we communicate the lifecycle moves for each ODL major version? (from John)
	a. My proposal is to let the nuget release notes do this for us.
2. How do we want to handle testing of .NET release candidates? (from many folks on the team)
	a. My proposal is to test .NET support after the official .NET release, giving a 3 month runway.
	b. Mike had feedback to use ODL beta releases, and had this comment: https://teams.microsoft.com/l/message/19:meeting_NWViNmQzN2ItMmQ0Mi00OGFmLWIxMzktZGVhY2EwMWMyMzBm@thread.v2/1720547775436?context=%7B%22contextType%22%3A%22chat%22%7D
	> There should never be a pre-release version of .NET that does not have OData support (either release or, at minimum, beta). Customers should never be prohibited from moving to any supported or pre-release version of .NET because of lack of OData support.
3. Mike had this comment in the previous meeting: https://teams.microsoft.com/l/message/19:meeting_NWViNmQzN2ItMmQ0Mi00OGFmLWIxMzktZGVhY2EwMWMyMzBm@thread.v2/1720547233614?context=%7B%22contextType%22%3A%22chat%22%7D
> Even if the versioning system does not require that we limit ourselves to only doing breaking change releases during LTS releases, in practice I think we should try to align breaking change releases with the LTS releases.
