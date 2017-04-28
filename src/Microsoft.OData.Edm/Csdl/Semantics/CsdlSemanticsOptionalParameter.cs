//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsOptionalParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlSemanticsOptionalParameter.
    /// </summary>
    internal class CsdlSemanticsOptionalParameter : CsdlSemanticsOperationParameter, IEdmOptionalParameter
    {
        public CsdlSemanticsOptionalParameter(CsdlSemanticsOperation declaringOperation, CsdlOperationParameter parameter, string defaultValue)
            : base(declaringOperation, parameter)
        {
            this.DefaultValueString = defaultValue;
        }

        public string DefaultValueString { get; private set; }
    }
}
