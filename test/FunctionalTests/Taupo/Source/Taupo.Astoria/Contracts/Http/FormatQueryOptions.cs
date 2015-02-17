//---------------------------------------------------------------------
// <copyright file="FormatQueryOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    /// <summary>
    /// A collection of constants for $format query options
    /// </summary>
    public static class FormatQueryOptions
    {
        /// <summary>
        /// The 'atom' query option
        /// </summary>
        public const string Atom = "atom";

        /// <summary>
        /// The 'json' query option
        /// </summary>
        public const string Json = "json";

        /// <summary>
        /// The 'verbosejson' query option
        /// </summary>
        public const string VerboseJson = "verbosejson";

        /// <summary>
        /// The 'xml' query option
        /// </summary>
        public const string Xml = "xml";
    }
}
