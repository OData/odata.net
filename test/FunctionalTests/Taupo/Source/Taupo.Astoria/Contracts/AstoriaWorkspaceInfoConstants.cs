//---------------------------------------------------------------------
// <copyright file="AstoriaWorkspaceInfoConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    /// <summary>
    /// A set of constants used to provide extra information on the workspace info during service building
    /// </summary>
    public static class AstoriaWorkspaceInfoConstants
    {
        /// <summary>
        /// The prefix for the CSDL document(s). Will be followed by an index.
        /// </summary>
        public const string CsdlKeyPrefix = "Csdl_";

        /// <summary>
        /// The key storing the resource strings for the Astoria server dll
        /// </summary>
        public const string SystemDataServicesResourcesKey = "Resources." + DataFxAssemblyRef.Name.DataServices;

        /// <summary>
        /// The separator to use in between resource string key-value pairs.
        /// Should be semi-unique but still human-readable.
        /// </summary>
        public const string ResourceLineSeparator = "\r\n\t  \t\r\n";

        /// <summary>
        /// The separator to use between resource string identifiers and values.
        /// Should be semi-unique but still human-readable.
        /// </summary>
        public const string ResourceKeyValueSeparator = "\t=\t";

        /// <summary>
        /// The key storing the types registered with EnableTypeAccess
        /// </summary>
        public const string RegisteredTypes = "RegisteredTypes";

        /// <summary>
        /// The separator to use between the types registered with EnableTypeAccess
        /// </summary>
        public const char RegisteredTypesSeparator = ',';
    }
}