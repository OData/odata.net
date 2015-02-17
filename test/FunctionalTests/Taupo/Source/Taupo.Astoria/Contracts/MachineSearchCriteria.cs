//---------------------------------------------------------------------
// <copyright file="MachineSearchCriteria.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Class has parameters that are used to find machines
    /// within a pool that meet the particular criteria below
    /// based on Arch/Locale/NDPVersion
    /// </summary>
    public class MachineSearchCriteria
    {
        /// <summary>
        /// Initializes a new instance of the MachineSearchCriteria class with the ServerLanguage default to English
        /// </summary>
        public MachineSearchCriteria()
        {
            this.MachinesToAvoid = new List<string>();
        }

        /// <summary>
        /// Gets the MachinesToAvoid
        /// </summary>
        public IList<string> MachinesToAvoid { get; private set; }

        /// <summary>
        /// Gets or sets the ArchType
        /// </summary>
        public ArchType? ArchType { get; set; }

        /// <summary>
        /// Gets or sets the SkuType
        /// </summary>
        public LabSkuType? SkuType { get; set; }

        /// <summary>
        /// Gets or sets the InternetInformationServerMajorVersion
        /// </summary>
        public InternetInformationServerMajorVersion? InternetInformationServerMajorVersion { get; set; }
        
        /// <summary>
        /// Gets or sets the LabServerLanguage
        /// </summary>
        public LabServerLanguage? ServerLanguage { get; set; }

        /// <summary>
        /// Gets or sets the FrameworkVersion of NDP installed
        /// </summary>
        public string FrameworkVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to look for a machine that is ready or not
        /// </summary>
        public bool? Ready { get; set; }
    }
}