//---------------------------------------------------------------------
// <copyright file="IODataJsonOperationsDeserializerContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Interface representing a context necessary for reading JSON operations values.
    /// </summary>
    internal interface IODataJsonOperationsDeserializerContext
    {
        /// <summary>
        /// The JSON reader to read the operations value from.
        /// </summary>
        IJsonReader JsonReader { get; }

        /// <summary>
        /// Given a URI from the payload, this method will try to make it absolute, or fail otherwise.
        /// </summary>
        /// <param name="uriFromPayload">The URI string from the payload to process.</param>
        /// <returns>An absolute URI to report.</returns>
        Uri ProcessUriFromPayload(string uriFromPayload);

        /// <summary>
        /// Adds the specified action to the current resource.
        /// </summary>
        /// <param name="action">The action which is fully populated with the data from the payload.</param>
        void AddActionToResource(ODataAction action);

        /// <summary>
        /// Adds the specified function to the current resource.
        /// </summary>
        /// <param name="function">The function which is fully populated with the data from the payload.</param>
        void AddFunctionToResource(ODataFunction function);
    }
}
