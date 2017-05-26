//---------------------------------------------------------------------
// <copyright file="IODataObjectModelValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Contracts
{
    #region Namespaces
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Contract for OData object model validators.
    /// </summary>
    [ImplementationSelector("IODataObjectModelValidator", DefaultImplementation = "ReaderDefaultODataObjectModelValidator", HelpText = "OData OM validator")]
    public interface IODataObjectModelValidator
    {
        /// <summary>
        /// Validates the OData object model.
        /// </summary>
        /// <param name="objectModelRoot">The root of the object model.</param>
        /// <remarks>The OData object model can be rooted with:
        /// ODataResourceSet
        /// ODataResource
        /// ODataProperty
        /// 
        /// ODataResourceSet may have annotation which stores the entries in it.
        /// ODataNestedResourceInfo may have annotation which stores the expanded items in it.</remarks>
        void ValidateODataObjectModel(object objectModelRoot);
    }
}
