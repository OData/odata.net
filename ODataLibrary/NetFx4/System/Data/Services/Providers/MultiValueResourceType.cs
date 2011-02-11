//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Providers
{
    #region Namespaces.
    using System;
    using System.Collections.Generic;
    using System.Data.OData;
    using System.Diagnostics;
    using System.Globalization;
    #endregion Namespaces.

    /// <summary>
    /// Use this class to represent a DataService type representing a MultiValue property of primitive or complex types.
    /// </summary>
    [DebuggerDisplay("{Name}: {InstanceType}, {ResourceTypeKind}")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "MultiValue is a Name")]
#if INTERNAL_DROP
    internal class MultiValueResourceType : ResourceType
#else
    public class MultiValueResourceType : ResourceType
#endif
    {
        /// <summary>
        /// Resource type of a single item in the MultiValue property.
        /// </summary>
        private ResourceType itemType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemType">Resource type of a single item in the MultiValue property.</param>
        internal MultiValueResourceType(ResourceType itemType)
            : base(GetInstanceType(itemType), ResourceTypeKind.MultiValue, string.Empty, GetName(itemType))
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(itemType != null, "itemType != null");

            if (itemType.ResourceTypeKind != ResourceTypeKind.Primitive && itemType.ResourceTypeKind != ResourceTypeKind.ComplexType)
            {
                throw new ArgumentException(Strings.ResourceType_MultiValueItemCanBeOnlyPrimitiveOrComplex);
            }

            this.itemType = itemType;
        }

        /// <summary>
        /// Resource type of a single item in the multiValue.
        /// </summary>
        public ResourceType ItemType
        {
            get
            {
                return this.itemType;
            }
        }

        /// <summary>
        /// Returns instance type for a MultiValue property of specified <paramref name="itemType"/>.
        /// </summary>
        /// <param name="itemType">Resource type of a single item in the MultiValue property.</param>
        /// <returns>Instance type of the MultiValue property of <paramref name="itemType"/>.</returns>
        private static Type GetInstanceType(ResourceType itemType)
        {
            Debug.Assert(itemType != null, "itemType != null");
            return typeof(IEnumerable<>).MakeGenericType(itemType.InstanceType);
        }

        /// <summary>
        /// Returns EDM name of the type for a MultiValue property of specified <paramref name="itemType"/>.
        /// </summary>
        /// <param name="itemType">Resource type of a single item in the MultiValue property.</param>
        /// <returns>EDM name of the type of the MultiValue property of <paramref name="itemType"/>.</returns>
        private static string GetName(ResourceType itemType)
        {
            Debug.Assert(itemType != null, "itemType != null");
            return String.Format(CultureInfo.InvariantCulture, EdmConstants.MultiValueTypeFormat, itemType.FullName);
        }
    }
}
