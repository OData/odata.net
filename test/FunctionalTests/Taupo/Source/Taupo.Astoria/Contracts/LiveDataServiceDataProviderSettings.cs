//---------------------------------------------------------------------
// <copyright file="LiveDataServiceDataProviderSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Runtime.Serialization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Specifies deployment-level settings specific to ExistingDataServiceDeployer
    /// </summary>
    [ImplementationName(typeof(DataProviderSettings), "Live")]
    public class LiveDataServiceDataProviderSettings : ExistingDataServiceDataProviderSettings
    {
        /// <summary>
        /// Initializes a new instance of the LiveDataServiceDataProviderSettings class.
        /// </summary>
        public LiveDataServiceDataProviderSettings()
            : base("Live")
        {
        }

        /// <summary>
        /// Gets or sets the Live Application ID
        /// </summary>
        [InjectTestParameter("LiveApplicationId", HelpText = "The Application ID with which the test runner is registered with Windows Live")]
        public string LiveApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the Live Application ID
        /// </summary>
        [InjectTestParameter("LiveApplicationSecret", HelpText = "The Application Secret with which the test runner is registered with Windows Live")]
        public string LiveApplicationSecret { get; set; }

        /// <summary>
        /// Gets or sets the Live Application ID
        /// </summary>
        [InjectTestParameter("LiveUserName", HelpText = "The User name of the live account to login to the Live Services")]
        public string LiveUserName { get; set; }

        /// <summary>
        /// Gets or sets the Live Application ID
        /// </summary>
        [InjectTestParameter("LivePassword", HelpText = "The password of the live account to login to the Live Services")]
        public string LivePassword { get; set; }

        /// <summary>
        /// Gets or sets the Live Application ID
        /// </summary>
        [InjectTestParameter("LiveServiceRoot", DefaultValueDescription = "http://apis.live-tst.net/V4.0/", HelpText = "The service root of Live Services")]
        public string LiveServiceRoot { get; set; }

        /// <summary>
        /// Gets or sets the Live Application ID
        /// </summary>
        [InjectTestParameter("LiveEnvironment", DefaultValueDescription = "int", HelpText = "Possible values are :int,func,prod")]
        public string LiveEnvironment { get; set; }
    }
}
