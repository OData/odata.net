//---------------------------------------------------------------------
// <copyright file="SandboxObjectFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// The default <see cref="ISandboxObjectFactory"/>, which is cognizant of
    /// changes in the desired trust level, so tests can be run in
    /// partial trust.
    /// </summary>
    [ImplementationName(typeof(ISandboxObjectFactory), "Default", HelpText = "Handles creating instances of classes in a sandbox.")]
    public class SandboxObjectFactory : ISandboxObjectFactory
    {
        private static List<string> fullyTrustedAssemblies = new List<string>
        {
            "Microsoft.Test.Taupo",
            "Microsoft.Test.Taupo.Astoria",
            "Microsoft.Test.Taupo.Astoria.Design",
            "Microsoft.Test.Taupo.Astoria.Providers",
            "Microsoft.Test.Taupo.CodeFirst",
            "Microsoft.Test.Taupo.EntityDesign",
            "Microsoft.Test.Taupo.EntityFramework",
            "Microsoft.Test.Taupo.MSTest",
            "Microsoft.Test.Taupo.Providers",
            "Microsoft.Test.Taupo.Web",
            "Microsoft.Test.Taupo.WebServices",
        };

        private static AppDomain sandbox;

        /// <summary>
        /// Initializes a new instance of the <see cref="SandboxObjectFactory" /> class.
        /// </summary>
        public SandboxObjectFactory()
        {
            this.TrustLevel = TrustLevel.Medium;
        }

        /// <summary>
        /// Gets or sets the trust level in which to create
        /// the <see cref="TestItem"/>.
        /// </summary>
        [InjectTestParameter("TrustLevel", DefaultValueDescription = "Medium", HelpText = "Specifies the trust level at which to run tests.")]
        public TrustLevel TrustLevel { get; set; }

        /// <summary>
        /// Creates an object of the specified <paramref name="type"/>, passing
        /// the specified <paramref name="args"/> to its constructor.
        /// </summary>
        /// <param name="type">The type of <see cref="TestItem"/> to create.</param>
        /// <param name="args">The arguments to pass to the constructor.</param>
        /// <returns>An instance of <paramref name="type"/>.</returns>
        public object CreateInstance(Type type, params object[] args)
        {
            if (this.TrustLevel == TrustLevel.Full)
            {
                return Activator.CreateInstance(type, args);
            }

            ExceptionUtilities.Assert(this.TrustLevel == TrustLevel.Medium, "This component only accounts for full trust and medium trust.");

            if (sandbox == null)
            {
                CreateMediumTrustAppDomain();
            }

            var testItem = sandbox.CreateInstanceAndUnwrap(
                type.GetAssembly().FullName,
                type.FullName,
                false,
                BindingFlags.Default, // TODO: make this work on Win8?
                null,
                args,
                null,
                null);

            return testItem;
        }

        private static void CreateMediumTrustAppDomain()
        {
            var securityConfig = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "CONFIG", "web_mediumtrust.config");
            var permissionXml = File.ReadAllText(securityConfig).Replace("$AppDir$", IOHelpers.BaseDirectory);

            // ASP.NET's configuration files still use the full policy levels rather than just permission sets,
            // so we can either write a lot of code to parse them ourselves, or we can use a deprecated API to
            // load them. This API used to throw an Exception in the .NET 4 development cycle, but ASP.NET themselves
            // use it to determine the grant set of the AppDomain, so this was changed to work before .NET 4 RTM.
#pragma warning disable 0618
            var grantSet = SecurityManager.LoadPolicyLevelFromString(permissionXml, PolicyLevelType.AppDomain).GetNamedPermissionSet("ASP.Net");
#pragma warning restore 0618

            var fileIOPermission = (FileIOPermission)grantSet.GetPermission(typeof(FileIOPermission));
            fileIOPermission.AddPathList(FileIOPermissionAccess.AllAccess, IOHelpers.BaseTempDirectory);

            var info = new AppDomainSetup
            {
                ApplicationBase = IOHelpers.BaseDirectory,
                PartialTrustVisibleAssemblies = new string[]
                {
                    "System.ComponentModel.DataAnnotations, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9",
                    "System.Web, PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293",
                }
            };

            var fullTrustStrongNames = new List<StrongName>();
            var taupoStrongName = typeof(SandboxObjectFactory).GetAssembly().Evidence.GetHostEvidence<StrongName>();

            foreach (var assemblyName in fullyTrustedAssemblies)
            {
                fullTrustStrongNames.Add(new StrongName(taupoStrongName.PublicKey, assemblyName, taupoStrongName.Version));
            }

            sandbox = AppDomain.CreateDomain(
                "Medium Trust Sandbox",
                null,
                info,
                grantSet,
                fullTrustStrongNames.ToArray());
        }
    }
}
