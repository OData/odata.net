//---------------------------------------------------------------------
// <copyright file="AstoriaMachineSearchCriteria.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;

    /// <summary>
    /// Astoria specific machine criteria used to find a machine
    /// </summary>
    public class AstoriaMachineSearchCriteria
    {
        /// <summary>
        /// Initializes a new instance of the AstoriaMachineSearchCriteria class with Ready default to true
        /// </summary>
        public AstoriaMachineSearchCriteria()
        {
        }

        /// <summary>
        /// Gets or sets the major a version of the DataServices installed on a machine
        /// </summary>
        public MajorAstoriaReleaseVersion? MajorReleaseVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the setuo is running or not on the specified machine
        /// </summary>
        public bool? SetupRunning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the machine is configured to be able
        /// to drop an DataService server bits on the box to run or 
        /// if its for a specified build
        /// </summary>
        public bool? SetupType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to look for a machine for versioning related webserver
        /// </summary>
        public bool? Versioning { get; set; }
    }
}
