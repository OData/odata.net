//---------------------------------------------------------------------
// <copyright file="PhpQueryOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Php
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The Query options provided by the PHP client library
    /// </summary>
    public class PhpQueryOptions
    {
        /// <summary>
        /// Gets or sets Name of the Entity Container for the Data Service being tested
        /// </summary>
        public string EntityContainer { get; set; }

        /// <summary>
        /// Gets or sets Name of the Entity Set to be queryied or updated 
        /// </summary>
        public string EntitySet { get; set; }

        /// <summary>
        /// Gets or sets Filter expression for the Filter option
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets OrderBy expression for the OrderBy option
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Gets or sets Skip count for the Skip option
        /// </summary>
        public string Skip { get; set; }

        /// <summary>
        /// Gets or sets Top count for the Top option
        /// </summary>
        public string Top { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to call the includeCount option
        /// </summary>
        public bool InlineCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to call the Count option
        /// </summary>
        public bool Count { get; set; }

        /// <summary>
        /// Gets or sets The Select expression for the Select option
        /// </summary>
        public string Select { get; set; }

        /// <summary>
        /// Gets or sets The Expand expression for the Expand option
        /// </summary>
        public string Expand { get; set; }

        /// <summary>
        /// Gets or sets the primary key to query if there is one
        /// </summary>
        public string PrimaryKey { get; set; } 
    }
}
