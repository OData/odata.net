//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsComplexTypeDefinition.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "baseType2",
            Justification = "Value assignment is required by compiler.")]
        private IEdmComplexType ComputeBaseType()
        {
            if (this.complex.BaseTypeName != null)
            {
                IEdmComplexType baseType = this.Context.FindType(this.complex.BaseTypeName) as IEdmComplexType;
                if (baseType != null)
                {
                    // Evaluate the inductive step to detect cycles.
                    // Overriding BaseType getter from concrete type implementing IEdmComplexType will be invoked to
                    // detect cycles. The object assignment is required by compiler only.
                    IEdmStructuredType baseType2 = baseType.BaseType;
                }

                return baseType ?? new UnresolvedComplexType(this.Context.UnresolvedName(this.complex.BaseTypeName), this.Location);
            }

            return null;
        }
    }
}
