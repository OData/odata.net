//---------------------------------------------------------------------
// <copyright file="PhpConstants.cs" company="Microsoft">
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
    /// Constants for PHP Code Generation
    /// </summary>
    public static class PhpConstants
    {
        /// <summary>
        /// Opening tag for PHP code
        /// </summary>
        public const string OpeningPhpTag = "<?php";

        /// <summary>
        /// Closing tag for PHP code
        /// </summary>
        public const string ClosingPhpTag = "?>";

        /// <summary>
        /// command to include a PHP file in the PHP code
        /// </summary>
        public const string IncludeDataServiceClassFile = "require_once ";

        /// <summary>
        /// Object for the DataServiceContainer Class provided by the PHP client library
        /// </summary>
        public const string DataServiceContainerObject = "$proxy";

        /// <summary>
        /// Object for the Data Service Query Class provided by the PHP client library
        /// </summary>
        public const string DataServiceQueryObject = "$query";

        /// <summary>
        /// Object for the Data Service Response Class provided by the PHP client library
        /// </summary>
        public const string DataServiceResponseObject = "$response";

        /// <summary>
        /// The PHP Client API for Executing a Data Service Query
        /// </summary>
        public const string Execute = "Execute";

        /// <summary>
        /// The PHP Client API for adding the filter option to a query
        /// </summary>
        public const string FilterOption = "Filter";

        /// <summary>
        /// The PHP Client API for adding the select option to a query
        /// </summary>
        public const string SelectOption = "Select";

        /// <summary>
        /// The PHP Client API for adding the expand option to a query
        /// </summary>
        public const string ExpandOption = "Expand";

        /// <summary>
        /// The PHP Client API for adding the OrderBy option to a query
        /// </summary>
        public const string OrderByOption = "OrderBy";

        /// <summary>
        /// The PHP Client API for adding the Skip option to a query
        /// </summary>
        public const string SkipOption = "Skip";

        /// <summary>
        /// The PHP Client API for adding the Top option to a query
        /// </summary>
        public const string TopOption = "Top";

        /// <summary>
        /// The PHP Client API for adding the Include Total count option to query
        /// </summary>
        public const string InlineCountOption = "IncludeCount";

        /// <summary>
        /// The PHP Client API to instruct a query to return only the count of the enitities
        /// </summary>
        public const string CountOption = "Count";
    }
}
