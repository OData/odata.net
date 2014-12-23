//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Helper class to verify that all items of a collection are of the same kind and type.
    /// </summary>
    /// <remarks>This class is only used if no expected item type is specified for the collection; 
    /// otherwise all items are already validated against the expected item type.</remarks>
    internal sealed class FeedWithoutExpectedTypeValidator
    {
        /// <summary>
        /// The base type for all entries in the feed.
        /// </summary>
        private IEdmEntityType itemType;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal FeedWithoutExpectedTypeValidator()
        {
        }

        /// <summary>
        /// Validates the type of an entry in a top-level feed.
        /// </summary>
        /// <param name="entityType">The type of the entry.</param>
        internal void ValidateEntry(IEdmEntityType entityType)
        {
            Debug.Assert(entityType != null, "entityType != null");

            // If we don't have a type, store the type of the first item.
            if (this.itemType == null)
            {
                this.itemType = entityType;
            }

            // Validate the expected and actual types.
            if (this.itemType.IsEquivalentTo(entityType))
            {
                return;
            }

            // If the types are not equivalent, make sure they have a common base type.
            IEdmType commonBaseType = EdmLibraryExtensions.GetCommonBaseType(this.itemType, entityType);
            if (commonBaseType == null)
            {
                throw new ODataException(Strings.FeedWithoutExpectedTypeValidator_IncompatibleTypes(entityType.ODataFullName(), this.itemType.ODataFullName()));
            }

            this.itemType = (IEdmEntityType)commonBaseType;
        }
    }
}
