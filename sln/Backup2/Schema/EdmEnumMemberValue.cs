//---------------------------------------------------------------------
// <copyright file="EdmEnumMemberValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// The edm enum member value.
    /// </summary>
    public class EdmEnumMemberValue : IEdmEnumMemberValue
    {
        private readonly long value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEnumMemberValue"/> class.
        /// </summary>
        /// <param name="value">The value of enum member</param>
        public EdmEnumMemberValue(long value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value of enum member
        /// </summary>
        public long Value
        {
            get
            {
                return this.value;
            }
        }
    }
}
