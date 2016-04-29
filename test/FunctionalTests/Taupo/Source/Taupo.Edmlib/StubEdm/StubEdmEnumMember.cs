//---------------------------------------------------------------------
// <copyright file="StubEdmEnumMember.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Edmlib.StubEdm
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>
    /// Stub implementation of EdmEnumMember
    /// </summary>
    public class StubEdmEnumMember : StubEdmElement, IEdmEnumMember
    {
        /// <summary>
        /// Initializes a new instance of the StubEdmEnumMember class.
        /// </summary>
        /// <param name="name">the name of the enum member</param>
        public StubEdmEnumMember(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public IEdmEnumMemberValue Value { get; set; }

        /// <summary>
        /// Gets or sets the declaring type
        /// </summary>
        public IEdmEnumType DeclaringType { get; set; }
    }
}
