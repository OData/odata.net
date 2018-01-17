//---------------------------------------------------------------------
// <copyright file="ODataServiceDocument.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Class representing the a service document.
    /// </summary>
    public sealed class ODataServiceDocument : ODataAnnotatable
    {
        /// <summary>Gets or sets the set of entity sets in the service document.</summary>
        /// <returns>The set of entity sets in the service document.</returns>
        public IEnumerable<ODataEntitySetInfo> EntitySets
        {
            get;
            set;
        }

        /// <summary>Gets or sets the set of singletons in the service document.</summary>
        /// <returns>The set of singletons in the service document.</returns>
        public IEnumerable<ODataSingletonInfo> Singletons
        {
            get;
            set;
        }

        /// <summary>Gets or sets the set of function imports in the service document.</summary>
        /// <returns>The set of function imports in the service document.</returns>
        public IEnumerable<ODataFunctionImportInfo> FunctionImports
        {
            get;
            set;
        }
    }
}
