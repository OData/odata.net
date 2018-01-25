//---------------------------------------------------------------------
// <copyright file="ReaderTestConfigurationProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
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
    /// Provider of collections of reader test configurations.
    /// </summary>
    public class ReaderTestConfigurationProvider
    {
        /// <summary>The test configurations cache used for checkin runs.</summary>
        private static CachedConfigurations checkinConfigurations;

        /// <summary>The test configurations cache used for full runs.</summary>
        private static CachedConfigurations allConfigurations;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReaderTestConfigurationProvider()
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
        /// List of all interesting configurations.
        /// </summary>
        public virtual IEnumerable<ReaderTestConfiguration> AllFormatConfigurations
        {
            get
            {
                CachedConfigurations configurations = this.InitializeConfigurations();
                return configurations.AllFormatsConfigurations;
            }
        }

        /// <summary>
        /// List of all interesting configurations with formats for payloads which are supported in all formats.
        /// </summary>
        public virtual IEnumerable<ReaderTestConfiguration> ExplicitFormatConfigurations
        {
            get
            {
                CachedConfigurations configurations = this.InitializeConfigurations();
                return configurations.ExplicitFormatConfigurations;
            }
        }

        /// <summary>
        /// List of all interesting configuration for JSON format payloads.
        /// </summary>
        public virtual IEnumerable<ReaderTestConfiguration> JsonLightFormatConfigurations
        {
            get
            {
                CachedConfigurations configurations = this.InitializeConfigurations();
                return configurations.JsonLightConfigurations;
            }
        }

        /// <summary>
        /// List of all interesting configuration for ATOM format payloads.
        /// </summary>
        //public virtual IEnumerable<ReaderTestConfiguration> AtomFormatConfigurations
        //{
        //    get
        //    {
        //        CachedConfigurations configurations = this.InitializeConfigurations();
        //        return configurations.AtomConfigurations;
        //    }
        //}

        /// <summary>
        /// List of all interesting configurations with format for payloads which doesn't depend on format = raw content.
        /// </summary>
        public virtual IEnumerable<ReaderTestConfiguration> DefaultFormatConfigurations
        {
            get
            {
                CachedConfigurations configurations = this.InitializeConfigurations();
                return configurations.DefaultFormatConfigurations;
            }
        }

        /// <summary>
        /// Initializes the test configurations.
        /// </summary>
        private CachedConfigurations InitializeConfigurations()
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
        private static CachedConfigurations CreateConfigurations(TestRunKind runKind)
        {
            ICombinatorialEngineProvider combinatorialEngine = new FullCombinatorialEngineProvider();

            return new CachedConfigurations
            {
                // AtomConfigurations = CreateConfigurations(runKind, combinatorialEngine, ODataFormat.Atom),
                JsonLightConfigurations = CreateConfigurations(runKind, combinatorialEngine, ODataFormat.Json),
                DefaultFormatConfigurations = CreateConfigurations(runKind, combinatorialEngine, /*format*/ null),
            };
        }

        /// <summary>
        /// Creates the configurations for the specified format using the default settings.
        /// </summary>
        /// <param name="runKind">The kind of test run for which to create the configurations.</param>
        /// <param name="combinatorialEngine">The combinatorial engine.</param>
        /// <param name="format">The <see cref="ODataFormat"/> to create test configurations for.</param>
        /// <returns>The list of generated test configurations.</returns>
        private static List<ReaderTestConfiguration> CreateConfigurations(TestRunKind runKind, ICombinatorialEngineProvider combinatorialEngine, ODataFormat format)
        {
            List<ReaderTestConfiguration> configurations = new List<ReaderTestConfiguration>();

            if (runKind == TestRunKind.All)
            {
                combinatorialEngine.RunCombinations(
                    ODataVersionUtils.AllSupportedVersions,
                    new bool[] { true, false },  // enableMessageStreamDisposal
                    new bool[] { true, false },  // isRequest
                    new bool[] { true, false },  // synchronous
                    new bool[] { true, false },  // skipStateValidationBeforeRead
                    (version, enableMessageStreamDisposal, isRequest, synchronous, skipStateValidationBeforeRead) =>
                    {

#if SILVERLIGHT || WINDOWS_PHONE
                        // If we are running in Silverlight or windows phone, we don't want to generate asynchronous variations
                        // hence we will skip generation of asynchronous test combinations
                        if (!synchronous)
                        {
                            return;
                        }
#endif
                        var settings = new ODataMessageReaderSettings
                        {
                            EnableMessageStreamDisposal = enableMessageStreamDisposal
                        };
                        configurations.Add(new ReaderTestConfiguration(
                            format,
                            settings,
                            isRequest,
                            synchronous,
                            skipStateValidationBeforeRead: skipStateValidationBeforeRead));
                    });
            }
            else
            {
                var limitedCombinations = new[]
                {
                    new {
                        EnableMessageStreamDisposal = true,
                        Synchronous = true,
                    },
                    new {
                        EnableMessageStreamDisposal = false,
                        Synchronous = false,
                    },
                };

                combinatorialEngine.RunCombinations(
                    limitedCombinations,
                    new bool[] { true, false }, // isRequest
                    new bool[] { true, false },  // skipStateValidationBeforeRead
                    (limitedCombination, isRequest, skipStateValidationBeforeRead) =>
                    {

#if SILVERLIGHT || WINDOWS_PHONE
                        // If we are running in Silverlight or windows phone, we don't want to generate asynchronous variations
                        // hence we will skip generation of asynchronous test combinations
                        if (!limitedCombination.Synchronous)
                        {
                            return;
                        }
#endif
                        var settings = new ODataMessageReaderSettings
                        {
                            EnableMessageStreamDisposal = limitedCombination.EnableMessageStreamDisposal
                        };
                        configurations.Add(new ReaderTestConfiguration(
                            format,
                            settings,
                            isRequest,
                            limitedCombination.Synchronous,
                            skipStateValidationBeforeRead: skipStateValidationBeforeRead));
                    });
            }

            return configurations;
        }

        /// <summary>
        /// A set of cached test configurations for all the formats.
        /// </summary>
        private sealed class CachedConfigurations
        {
            /// <summary>
            /// List of all ATOM configurations with default settings.
            /// </summary>
            // public List<ReaderTestConfiguration> AtomConfigurations { get; set; }

            /// <summary>
            /// List of all JSON Lite configurations with default settings.
            /// </summary>
            public List<ReaderTestConfiguration> JsonLightConfigurations { get; set; }

            /// <summary>
            /// List of all default configurations with default settings.
            /// </summary>
            public List<ReaderTestConfiguration> DefaultFormatConfigurations { get; set; }

            /// <summary>
            /// List of all interesting configurations with formats for payloads which are supported in all formats.
            /// </summary>
            public IEnumerable<ReaderTestConfiguration> ExplicitFormatConfigurations
            {
                get
                {
                    return this.JsonLightConfigurations;
                }
            }

            /// <summary>
            /// List of all the configurations.
            /// </summary>
            public IEnumerable<ReaderTestConfiguration> AllFormatsConfigurations
            {
                get
                {
                    return this.JsonLightConfigurations.Concat(this.DefaultFormatConfigurations);
                }
            }
        }
    }
}
