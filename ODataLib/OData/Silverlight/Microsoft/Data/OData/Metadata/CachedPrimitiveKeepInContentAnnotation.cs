//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Annotation which stores a hashset of property names of a complex type that returned KeepInContent == true 
    /// when written the first time. See the comments on ODataWriterBehavior.UseV1ProviderBehavior for more details.
    /// </summary>
    internal sealed class CachedPrimitiveKeepInContentAnnotation
    {
        /// <summary>
        /// A hash set with the property names of properties that are kept in the content.
        /// </summary>
        private readonly HashSet<string> keptInContentPropertyNames;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="keptInContentPropertyNames">Enumeration of property names that are kept in content.</param>
        internal CachedPrimitiveKeepInContentAnnotation(IEnumerable<string> keptInContentPropertyNames)
        {
            DebugUtils.CheckNoExternalCallers();

            this.keptInContentPropertyNames = keptInContentPropertyNames == null 
                ? null 
                : new HashSet<string>(keptInContentPropertyNames, StringComparer.Ordinal);
        }

        /// <summary>
        /// Determines if a property is in a list of properties that are kept in the content.
        /// </summary>
        /// <param name="propertyName">The name of the property to lookup.</param>
        /// <returns>true if the property is kept in the content; false otherwise.</returns>
        internal bool IsKeptInContent(string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            return this.keptInContentPropertyNames == null 
                ? false 
                : this.keptInContentPropertyNames.Contains(propertyName);
        }
    }
}
