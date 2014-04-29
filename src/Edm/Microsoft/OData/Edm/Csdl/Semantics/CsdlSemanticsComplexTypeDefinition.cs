//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlComplexType.
    /// </summary>
    internal class CsdlSemanticsComplexTypeDefinition : CsdlSemanticsStructuredTypeDefinition, IEdmComplexType
    {
        private readonly CsdlComplexType complex;

        private readonly Cache<CsdlSemanticsComplexTypeDefinition, IEdmComplexType> baseTypeCache = new Cache<CsdlSemanticsComplexTypeDefinition, IEdmComplexType>();
        private static readonly Func<CsdlSemanticsComplexTypeDefinition, IEdmComplexType> ComputeBaseTypeFunc = (me) => me.ComputeBaseType();
        private static readonly Func<CsdlSemanticsComplexTypeDefinition, IEdmComplexType> OnCycleBaseTypeFunc = (me) => new CyclicComplexType(me.GetCyclicBaseTypeName(me.complex.BaseTypeName), me.Location);

        public CsdlSemanticsComplexTypeDefinition(CsdlSemanticsSchema context, CsdlComplexType complex)
            : base(context, complex)
        {
            this.complex = complex;
        }

        public override IEdmStructuredType BaseType
        {
            get { return this.baseTypeCache.GetValue(this, ComputeBaseTypeFunc, OnCycleBaseTypeFunc); }
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Complex; }
        }

        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Type; }
        }

        public override bool IsAbstract
        {
            get { return this.complex.IsAbstract; }
        }

        public override bool IsOpen
        {
            get { return this.complex.IsOpen; }
        }

        public string Name
        {
            get { return this.complex.Name; }
        }

        protected override CsdlStructuredType MyStructured
        {
            get { return this.complex; }
        }

        private IEdmComplexType ComputeBaseType()
        {
            if (this.complex.BaseTypeName != null)
            {
                IEdmComplexType baseType = this.Context.FindType(this.complex.BaseTypeName) as IEdmComplexType;
                if (baseType != null)
                {
                    IEdmStructuredType junk = baseType.BaseType; // Evaluate the inductive step to detect cycles.
                }

                return baseType ?? new UnresolvedComplexType(this.Context.UnresolvedName(this.complex.BaseTypeName), this.Location);
            }

            return null;
        }
    }
}
