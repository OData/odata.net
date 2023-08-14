# Onboarding

## Local Environment Setup

1. Install Microsoft Visual Studio Enterprise 2022 Version 17.6.6 using [the vsconfig in the repo](./install.vsconfig)
2. Create and trust a local certificate for use with `https` by running the following commands:
```sh
dotnet dev-certs https
dotnet dev-certs https --trust
```

## Running Locally

To start the service, run the following command from the repository root:
```sh
dotnet run --project Portal\Portal.csproj
```

Then, you can navigate to `https://localhost` in your web browser to view the dashboard.

You can also start the service in visual studio by starting a new instance of the `Portal` project (using by hitting `f5`)
