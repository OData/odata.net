//---------------------------------------------------------------------
// <copyright file="CsdlWriterSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Configuration settings for CSDL writers.
    /// </summary>
    public class CsdlWriterSettings
    {
        internal static CsdlWriterSettings Default = new CsdlWriterSettings
        {
            Indent = true
        };

        /// <summary>
        /// Gets/sets a value indicating whether the writer write large integers as strings.
        /// </summary>
        public bool IsIeee754Compatible { get; set; }

        /// <summary>
        /// Gets/sets a value indicating whether the output is indented.
        /// </summary>
        public bool Indent { get; set; }

        /// <summary>
        /// Gets/sets a property name switch function.
        /// </summary>
        public Func<string, string> PropertyNameFunc { get; set; }
    }
}