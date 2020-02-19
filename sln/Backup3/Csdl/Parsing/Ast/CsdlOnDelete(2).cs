//---------------------------------------------------------------------
// <copyright file="CsdlOnDelete.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL "on delete" action.
    /// </summary>
    internal class CsdlOnDelete : CsdlElement
    {
        private readonly EdmOnDeleteAction action;

        public CsdlOnDelete(EdmOnDeleteAction action, CsdlLocation location)
            : base(location)
        {
            this.action = action;
        }

        public EdmOnDeleteAction Action
        {
            get { return this.action; }
        }
    }
}
