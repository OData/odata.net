//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsTypeDefinitionDefinition.cs" company="Microsoft">
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
    /// Provides semantics for CsdlTypeDefinition.
    /// </summary>
    internal class CsdlSemanticsTypeDefinitionDefinition : CsdlSemanticsTypeDefinition, IEdmTypeDefinition
    {
        private readonly CsdlSemanticsSchema context;
        private readonly CsdlTypeDefinition typeDefinition;

        private readonly Cache<CsdlSemanticsTypeDefinitionDefinition, IEdmPrimitiveType> underlyingTypeCache = new Cache<CsdlSemanticsTypeDefinitionDefinition, IEdmPrimitiveType>();
        private static readonly Func<CsdlSemanticsTypeDefinitionDefinition, IEdmPrimitiveType> ComputeUnderlyingTypeFunc = (me) => me.ComputeUnderlyingType();

        public CsdlSemanticsTypeDefinitionDefinition(CsdlSemanticsSchema context, CsdlTypeDefinition typeDefinition)
            : base(typeDefinition)
        {
            this.context = context;
            this.typeDefinition = typeDefinition;
        }

        IEdmPrimitiveType IEdmTypeDefinition.UnderlyingType
        {
            get { return this.underlyingTypeCache.GetValue(this, ComputeUnderlyingTypeFunc, null); }
        }

        EdmSchemaElementKind IEdmSchemaElement.SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        public string Namespace
        {
            get { return this.context.Namespace; }
        }

        string IEdmNamedElement.Name
        {
            get { return this.typeDefinition.Name; }
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.TypeDefinition; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.typeDefinition; }
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.context);
        }

        private IEdmPrimitiveType ComputeUnderlyingType()
        {
            if (this.typeDefinition.UnderlyingTypeName != null)
            {
                var underlyingTypeKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(this.typeDefinition.UnderlyingTypeName);
                return underlyingTypeKind != EdmPrimitiveTypeKind.None ?
                    EdmCoreModel.Instance.GetPrimitiveType(underlyingTypeKind) :
                    new UnresolvedPrimitiveType(this.typeDefinition.UnderlyingTypeName, this.Location);
            }

            return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
        }
    }
}
