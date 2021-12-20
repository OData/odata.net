//---------------------------------------------------------------------
// <copyright file="BdnComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace ResultsComparer.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    class FieldNameAttribute : Attribute
    {
        public string Name { get; set; }

        public FieldNameAttribute(string name)
        {
            Name = name;
        }
    }
}
