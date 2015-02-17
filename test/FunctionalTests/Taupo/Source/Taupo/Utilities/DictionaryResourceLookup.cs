//---------------------------------------------------------------------
// <copyright file="DictionaryResourceLookup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Resource lookup implementation that simply indexes into a dictionary provided at construction time
    /// </summary>
    public class DictionaryResourceLookup : IResourceLookup
    {
        private readonly IDictionary<string, string> dictionary;

        /// <summary>
        /// Initializes a new instance of the DictionaryResourceLookup class.
        /// </summary>
        /// <param name="dictionary">The dictionary containing the resource strings.</param>
        public DictionaryResourceLookup(IDictionary<string, string> dictionary)
        {
            ExceptionUtilities.CheckArgumentNotNull(dictionary, "dictionary");
            this.dictionary = dictionary;
        }

        /// <summary>
        /// Finds a specific string resource
        /// </summary>
        /// <param name="resourceKey">Key of the resource to be located</param>
        /// <returns>The localized resource value</returns>
        public string LookupString(string resourceKey)
        {
            string value;
            ExceptionUtilities.Assert(this.dictionary.TryGetValue(resourceKey, out value), "Could not find resource key '{0}' in dictionary", resourceKey);
            return value;
        }
    }
}
