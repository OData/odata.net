//---------------------------------------------------------------------
// <copyright file="FieldNameAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace ResultsComparer.Core
{
    /// <summary>
    /// Attribute used to specify which field name a particular property
    /// should get its value from.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    class FieldNameAttribute : Attribute
    {
        /// <summary>
        /// The field name from the source file to bind the property to.
        /// </summary>
        public string Name { get; set; }

        public FieldNameAttribute(string name)
        {
            Name = name;
        }
    }
}
