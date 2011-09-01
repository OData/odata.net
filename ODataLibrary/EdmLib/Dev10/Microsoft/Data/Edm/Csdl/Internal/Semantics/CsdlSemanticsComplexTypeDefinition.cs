//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlComplexType.
    /// </summary>
    internal class CsdlSemanticsComplexTypeDefinition : CsdlSemanticsStructuredTypeDefinition, IEdmComplexType, IEdmCheckable
    {
        private readonly CsdlComplexType complex;

        private readonly Cache<CsdlSemanticsComplexTypeDefinition, IEdmComplexType> baseTypeCache = new Cache<CsdlSemanticsComplexTypeDefinition, IEdmComplexType>();
        private readonly static Func<CsdlSemanticsComplexTypeDefinition, IEdmComplexType> s_computeBaseType = (me) => me.ComputeBaseType();
        private readonly static Func<CsdlSemanticsComplexTypeDefinition, IEdmComplexType> s_onCycleBaseType = (me) => new CyclicComplexType(me.GetCyclicBaseTypeName(me.complex.BaseTypeName), me.Location);

        public CsdlSemanticsComplexTypeDefinition(CsdlSemanticsSchema context, CsdlComplexType complex)
            : base(context)
        {
            this.complex = complex;
        }

        public override IEdmStructuredType BaseType
        {
            get { return this.baseTypeCache.GetValue(this, s_computeBaseType, s_onCycleBaseType); }
        }
        
        protected override CsdlStructuredType MyStructured
        {
            get { return this.complex; }
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Complex; }
        }

        public override bool IsAbstract
        {
            get { return this.complex.IsAbstract; }
        }

        public string Name
        {
            get { return this.complex.Name; }
        }

        private IEdmComplexType ComputeBaseType()
        {
            if (this.complex.BaseTypeName != null)
            {
                IEdmComplexType baseType = this.context.FindType(this.complex.BaseTypeName) as IEdmComplexType;
                if (baseType != null)
                {
                    IEdmStructuredType junk = baseType.BaseType; // Evaluate the inductive step to detect cycles.
                }

                return baseType ?? new UnresolvedComplexType(this.complex.BaseTypeName, this.Location);
            }

            return null;
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if ((this.BaseType is CyclicComplexType) || (this.BaseType is UnresolvedComplexType))
                {
                    return this.BaseType.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
