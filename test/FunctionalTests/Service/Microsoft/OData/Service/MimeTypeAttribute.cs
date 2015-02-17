//---------------------------------------------------------------------
// <copyright file="MimeTypeAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Use this attribute on a DataService service operation method
    /// or a data object property to indicate than the type returned is 
    /// of a specific MIME type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class MimeTypeAttribute : Attribute
    {
        /// <summary>Name of the attributed method or property.</summary>
        private readonly string memberName;

        /// <summary>MIME type for the attributed method or property.</summary>
        private readonly string mimeType;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Service.MimeTypeAttribute" /> class. </summary>
        /// <param name="memberName">The name of the attribute.</param>
        /// <param name="mimeType">The MIME type of the attribute.</param>
        public MimeTypeAttribute(string memberName, string mimeType)
        {
            this.memberName = memberName;
            this.mimeType = mimeType;
        }

        /// <summary>Gets the name of the attribute.</summary>
        /// <returns>A string value that contains the name of the attribute. </returns>
        public string MemberName
        {
            get { return this.memberName; }
        }

        /// <summary>Gets the MIME type of a request.</summary>
        /// <returns>A string that contains the MIME type.</returns>
        public string MimeType
        {
            get { return this.mimeType; }
        }
    }
}
