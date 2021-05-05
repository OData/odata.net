//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsInclude.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsInclude : CsdlSemanticsElement, IEdmInclude
    {
        protected readonly CsdlSemanticsModel model;
        protected CsdlInclude include;

        public CsdlSemanticsInclude(CsdlSemanticsModel model, CsdlInclude include)
            : base(include)
        {
            this.model = model;
            this.include = include;
        }

        public string Alias => include.Alias;

        /// <summary>
        /// Gets the namespace to include.
        /// </summary>
        public string Namespace => include.Namespace;

        public override CsdlSemanticsModel Model => model;

        public override CsdlElement Element => include;
    }
}
