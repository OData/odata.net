//---------------------------------------------------------------------
// <copyright file="WriterTestConfigurationProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.JsonLight;
    #endregion Namespaces

    /// <summary>
    /// Provider for creating lists of configurations for writer tests.
    /// </summary>
    public class WriterTestConfigurationProvider
    {
        /// <summary>The test configurations cache used for checkin runs.</summary>
        private static RunKindTestConfigurations checkinConfigurations;

        /// <summary>The test configurations cache used for full runs.</summary>
        private static RunKindTestConfigurations allConfigurations;

        /// <summary>
        /// Constructor.
        /// </summary>
        public WriterTestConfigurationProvider()
        {
            // The default is to run checkin tests.
            this.RunKind = TestRunKind.CheckinSuite;
        }

        /// <summary>
        /// The test run kind to use for this test run.
        /// </summary>
        [InjectTestParameter("TestRunKind", HelpText = "The test run kind to use for running the tests.")]
        public TestRunKind RunKind { get; set; }

        /// <summary>
        /// List of all interesting writer configurations for all formats.
        /// </summary>
        public virtual IEnumerable<WriterTestConfiguration> AllFormatsConfigurationsWithIndent
        {
            get
            {
                RunKindTestConfigurations configurations = this.InitializeConfigurations();
                return configurations.ConfigurationsWithIndent.AllFormatsConfigurations;
            }
        }

        /// <summary>
        /// List of all interesting writer configuration for all formats with default settings only.
        /// </summary>
        public virtual IEnumerable<WriterTestConfiguration> AllFormatsConfigurations
        {
            get
            {
                RunKindTestConfigurations configurations = this.InitializeConfigurations();
                return configurations.DefaultConfigurations.AllFormatsConfigurations;
            }
        }

        /// <summary>
        /// List of all interesting configurations with formats for payloads which are supported in all formats.
        /// </summary>
        public virtual IEnumerable<WriterTestConfiguration> ExplicitFormatConfigurationsWithIndent
        {
            get
            {
                RunKindTestConfigurations configurations = this.InitializeConfigurations();
                return configurations.ConfigurationsWithIndent.ExplicitFormatConfigurations;
            }
        }

        /// <summary>
        /// List of all interesting configurations with default settings with formats for payloads which are supported in all formats.
        /// </summary>
        public virtual IEnumerable<WriterTestConfiguration> ExplicitFormatConfigurations
        {
            get
            {
                RunKindTestConfigurations configurations = this.InitializeConfigurations();
                return configurations.DefaultConfigurations.ExplicitFormatConfigurations;
            }
        }

        /// <summary>
        /// List of all interesting configurations with formats for ATOM.
        /// </summary>
        public virtual IEnumerable<WriterTestConfiguration> AtomFormatConfigurationsWithIndent
        {
            get
            {
                RunKindTestConfigurations configurations = this.InitializeConfigurations();
                return configurations.ConfigurationsWithIndent.AtomConfigurations;
            }
        }

        /// <summary>
        /// List of all interesting configurations with default settings for ATOM.
        /// </summary>
        public virtual IEnumerable<WriterTestConfiguration> AtomFormatConfigurations
        {
            get
            {
                RunKindTestConfigurations configurations = this.InitializeConfigurations();
                return configurations.DefaultConfigurations.AtomConfigurations;
            }
        }

        /// <summary>
        /// List of all interesting configurations with formats for JSON Lite.
        /// </summary>
        public virtual IEnumerable<WriterTestConfiguration> JsonLightFormatConfigurationsWithIndent
        {
            get
            {
                RunKindTestConfigurations configurations = this.InitializeConfigurations();
                return configurations.ConfigurationsWithIndent.JsonLightConfigurations;
            }
        }

        /// <summary>
        /// List of all interesting configurations with default settings for JSON Lite.
        /// </summary>
        public virtual IEnumerable<WriterTestConfiguration> JsonLightFormatConfigurations
        {
            get
            {
                RunKindTestConfigurations configurations = this.InitializeConfigurations();
                return configurations.DefaultConfigurations.JsonLightConfigurations;
            }
        }

        /// <summary>
        /// List of all interesting configurations with format for writing payload which doesn't depend on format = raw content.
        /// </summary>
        public virtual IEnumerable<WriterTestConfiguration> DefaultFormatConfigurationsWithIndent
        {
            get
            {
                RunKindTestConfigurations configurations = this.InitializeConfigurations();
                return configurations.ConfigurationsWithIndent.DefaultFormatConfigurations;
            }
        }

        /// <summary>
        /// List of all interesting configurations with default settings with format for writing payload which doesn't depend on format = raw content.
        /// </summary>
        public virtual IEnumerable<WriterTestConfiguration> DefaultFormatConfigurations
        {
            get
            {
                RunKindTestConfigurations configurations = this.InitializeConfigurations();
                return configurations.DefaultConfigurations.DefaultFormatConfigurations;
            }
        }

        /// <summary>
        /// Initializes the list of configurations.
        /// </summary>
        private RunKindTestConfigurations InitializeConfigurations()
        {
            // If the tests were running on multiple threads there might be a race condition here.
            // But since the computation should always return the same result, it doesn't matter that the last one will win.
            switch (this.RunKind)
            {
                case TestRunKind.CheckinSuite:
                    if (checkinConfigurations == null)
                    {
                        checkinConfigurations = CreateConfigurations(this.RunKind);
                    }

                    return checkinConfigurations;
                case TestRunKind.All:
                    if (allConfigurations == null)
                    {
                        allConfigurations = CreateConfigurations(this.RunKind);
                    }

                    return allConfigurations;
                default:
                    throw new NotSupportedException("Unsupported run kind '" + this.RunKind.ToString() + "' found.");
            }
        }

        /// <summary>
        /// Creates the configurations for a particular test run kind.
        /// </summary>
        /// <param name="runKind">The run kind to create the configurations for.</param>
        /// <returns>The created test configurations.</returns>
        private static RunKindTestConfigurations CreateConfigurations(TestRunKind runKind)
        {
            CachedConfigurations defaultConfigurations = CreateDefaultConfigurations(runKind);
            CachedConfigurations configurationsWithIndent = CreateConfigurationsWithIndent(runKind, defaultConfigurations);

            return new RunKindTestConfigurations
            {
                DefaultConfigurations = defaultConfigurations,
                ConfigurationsWithIndent = configurationsWithIndent,
            };
        }

        /// <summary>
        /// Creates the default test configurations for the specified run kind.
        /// </summary>
        /// <param name="runKind">The run kind to create the default configurations for.</param>
        /// <returns>The created test configurations.</returns>
        private static CachedConfigurations CreateDefaultConfigurations(TestRunKind runKind)
        {
            ICombinatorialEngineProvider combinatorialEngine = new FullCombinatorialEngineProvider();

            return new CachedConfigurations
            {
                //AtomConfigurations = CreateConfigurationsWithDefaultSettings(runKind, combinatorialEngine, ODataFormat.Atom),
                JsonLightConfigurations = CreateConfigurationsWithDefaultSettings(runKind, combinatorialEngine, ODataFormat.Json),
                DefaultFormatConfigurations = CreateConfigurationsWithDefaultSettings(runKind, combinatorialEngine, /*format*/ null),
            };
        }

        /// <summary>
        /// Creates test configurations with and without indenting based on a set of default configurations.
        /// </summary>
        /// <param name="runKind">The run kind to create the configurations for.</param>
        /// <param name="defaultConfigurations">The set of default configurations.</param>
        /// <returns>The created test configurations.</returns>
        private static CachedConfigurations CreateConfigurationsWithIndent(TestRunKind runKind, CachedConfigurations defaultConfigurations)
        {
            ICombinatorialEngineProvider combinatorialEngine = new FullCombinatorialEngineProvider();

            return new CachedConfigurations
            {
                // AtomConfigurations = CreateConfigurationsWithIndent(runKind, combinatorialEngine, ODataFormat.Atom, defaultConfigurations.AtomConfigurations),
                JsonLightConfigurations = CreateConfigurationsWithIndent(runKind, combinatorialEngine, ODataFormat.Json, defaultConfigurations.JsonLightConfigurations),
                DefaultFormatConfigurations = CreateConfigurationsWithIndent(runKind, combinatorialEngine, /*format*/ null, defaultConfigurations.DefaultFormatConfigurations),
            };
        }

        /// <summary>
        /// Creates the configurations for the specified format using the default settings.
        /// </summary>
        /// <param name="runKind">The kind of test run for which to create the configurations.</param>
        /// <param name="combinatorialEngine">The combinatorial engine.</param>
        /// <param name="format">The <see cref="ODataFormat"/> to create test configurations for.</param>
        /// <returns>The list of generated test configurations.</returns>
        private static List<WriterTestConfiguration> CreateConfigurationsWithDefaultSettings(TestRunKind runKind, ICombinatorialEngineProvider combinatorialEngine, ODataFormat format)
        {
            List<WriterTestConfiguration> configurations = new List<WriterTestConfiguration>();

            if (runKind == TestRunKind.All)
            {
                combinatorialEngine.RunCombinations(
                    ODataVersionUtils.AllSupportedVersions,
                    new bool[] { true, false },  // disableMessageStreamDisposal
                    new bool[] { true, false },  // isRequest
                    new bool[] { true, false },  // synchronous
                    (version, enableMessageStreamDisposal, isRequest, synchronous) =>
                    {
                        configurations.Add(new WriterTestConfiguration(
                            format,
                            GetDefaultMessageWriterSettings(format, null, enableMessageStreamDisposal, version),
                            isRequest,
                            synchronous));
                    });
            }
            else
            {
                IEnumerable<LimitedCombinationSpecification> limitedCombinations;
                limitedCombinations = new[]
                {
                    new LimitedCombinationSpecification {
                        EnableMessageStreamDisposal = true,
                        Synchronous = true,
                    },
                    new LimitedCombinationSpecification {
                        EnableMessageStreamDisposal = false,
                        Synchronous = false,
                    },
                };

                if (format == ODataFormat.Json)
                {
                    limitedCombinations
                        .ConcatSingle(new LimitedCombinationSpecification
                        {
                            EnableMessageStreamDisposal = false,
                            Synchronous = true
                        });
                }

                combinatorialEngine.RunCombinations(
                    limitedCombinations,
                    new bool[] { true, false },  // isRequest
                    (limitedCombination, isRequest) =>
                    {
                        configurations.Add(new WriterTestConfiguration(
                            format,
                            GetDefaultMessageWriterSettings(
                                format,
                                null,
                                limitedCombination.EnableMessageStreamDisposal),
                            isRequest,
                            limitedCombination.Synchronous));
                    });
            }

            return configurations;
        }

        /// <summary>
        /// Creates the configurations for the specified format with indenting.
        /// </summary>
        /// <param name="combinatorialEngine">The combinatorial engine.</param>
        /// <param name="format">The <see cref="ODataFormat"/> to create test configurations for.</param>
        /// <param name="configurationsWithDefaultSettings">The list of test configurations with default settings (no indenting).</param>
        /// <returns>The list of generated test configurations.</returns>
        private static List<WriterTestConfiguration> CreateConfigurationsWithIndent(TestRunKind runKind, ICombinatorialEngineProvider combinatorialEngine, ODataFormat format, IEnumerable<WriterTestConfiguration> configurationsWithDefaultSettings)
        {
            List<WriterTestConfiguration> configurations = new List<WriterTestConfiguration>();

            if (runKind == TestRunKind.All)
            {
                combinatorialEngine.RunCombinations(
                    configurationsWithDefaultSettings,
                    (config) =>
                    {
                        ODataMessageWriterSettings settings = config.MessageWriterSettings.Clone();
                        configurations.Add(new WriterTestConfiguration(config.Format,
                            settings,
                            config.IsRequest,
                            config.Synchronous));
                    });
            }
            else
            {
                // We need only some to use indentation but we want for each of ATOM and JSON to to have some which use it for sure
                int[] primes = new int[] { 1, 2, 3, 5, 7, 11, 13, 17, 19 };
                foreach (var config in configurationsWithDefaultSettings)
                {
                    ODataMessageWriterSettings settings = config.MessageWriterSettings.Clone();
                    configurations.Add(new WriterTestConfiguration(config.Format,
                        settings,
                        config.IsRequest,
                        config.Synchronous));
                }
            }

            return configurations;
        }

        /// <summary>
        /// Creates the default settings for a given <see cref="ODataFormat"/>.
        /// </summary>
        /// <param name="format">The format for the message writer settings to create.</param>
        /// <param name="version">Version of the OData protocol.</param>
        /// <param name="baseUri">Base uri for all relative Uris.</param>
        /// <param name="enableMessageStreamDisposal">When set to true, the writer will dispose the message stream. When set to false, the message stream will not be disposed.</param>
        /// <returns>The default message writer settings for the specified <paramref name="format"/>.</returns>
        private static ODataMessageWriterSettings GetDefaultMessageWriterSettings(
            ODataFormat format,
            Uri baseUri,
            bool enableMessageStreamDisposal,
            ODataVersion version = ODataVersion.V4)
        {
            var settings = new ODataMessageWriterSettings()
            {
                Version = version,
                BaseUri = baseUri,
                EnableMessageStreamDisposal = enableMessageStreamDisposal,
            };
            settings.SetServiceDocumentUri(new Uri("http://odata.org/test"));
            settings.SetContentType(format);
            return settings;
        }

        /// <summary>
        /// A set of cached test configurations for all the formats.
        /// </summary>
        private sealed class CachedConfigurations
        {
            /// <summary>
            /// List of all ATOM configurations with default settings.
            /// </summary>
            public List<WriterTestConfiguration> AtomConfigurations { get; set; }

            /// <summary>
            /// List of all JSON Lite configurations with default settings.
            /// </summary>
            public List<WriterTestConfiguration> JsonLightConfigurations { get; set; }

            /// <summary>
            /// List of all default configurations with default settings.
            /// </summary>
            public List<WriterTestConfiguration> DefaultFormatConfigurations { get; set; }

            /// <summary>
            /// List of all interesting configurations with formats for payloads which are supported in all formats.
            /// </summary>
            public IEnumerable<WriterTestConfiguration> ExplicitFormatConfigurations
            {
                get
                {
                    return this.JsonLightConfigurations;
                }
            }

            /// <summary>
            /// List of all the configurations.
            /// </summary>
            public IEnumerable<WriterTestConfiguration> AllFormatsConfigurations
            {
                get
                {
                    return this.AtomConfigurations
                        .Concat(this.JsonLightConfigurations)
                        .Concat(this.DefaultFormatConfigurations);
                }
            }
        }

        /// <summary>
        /// The cached test configurations for a particular run kind.
        /// </summary>
        private sealed class RunKindTestConfigurations
        {
            /// <summary>
            /// The default test configurations.
            /// </summary>
            public CachedConfigurations DefaultConfigurations { get; set; }

            /// <summary>
            /// The test configurations that also include indented configurations.
            /// </summary>
            public CachedConfigurations ConfigurationsWithIndent { get; set; }
        }

        /// <summary>
        /// Helper class to specify set of handcrafter combinations for limited combinations.
        /// </summary>
        private sealed class LimitedCombinationSpecification
        {
            /// <summary>
            /// The Version setting value.
            /// </summary>
            public ODataVersion Version { get; set; }

            /// <summary>
            /// The EnableMessageStreamDisposal setting value.
            /// </summary>
            public bool EnableMessageStreamDisposal { get; set; }

            /// <summary>
            /// The Synchronous setting value.
            /// </summary>
            public bool Synchronous { get; set; }
        }
    }
}
