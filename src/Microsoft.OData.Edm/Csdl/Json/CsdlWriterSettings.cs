//---------------------------------------------------------------------
// <copyright file="CsdlWriterSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Csdl.Json
{
    public class CsdlWriterSettings
    {
        public static CsdlWriterSettings Default = new CsdlWriterSettings();

        public bool IsIeee754Compatible { get; set; }

        public bool Indent { get; set; } = true;

        public string IndentPattern { get; set; }

        public Func<string, string> PropertyNameFunc { get; set; }
    }
}