//---------------------------------------------------------------------
// <copyright file="NamedStreamAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>Indicates that a class that is an entity type has a related named binary stream.</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class NamedStreamAttribute : Attribute
    {
        /// <summary>Creates a new instance of the <see cref="Microsoft.OData.Client.NamedStreamAttribute" /> class.</summary>
        /// <param name="name">The name of a binary stream that belongs to the attributed entity.</param>
        public NamedStreamAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>The name of a binary stream that belongs to the attributed entity.</summary>
        /// <returns>The name of the binary stream.</returns>
        public string Name
        {
            get;
            private set;
        }
    }
}
