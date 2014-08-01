//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlTypeDefinition.
    /// </summary>
    internal class CsdlSemanticsTypeDefinitionDefinition : CsdlSemanticsTypeDefinition, IEdmTypeDefinitionWithFacets
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

        int? IEdmTypeDefinitionWithFacets.MaxLength
        {
            get { return this.typeDefinition.MaxLength; }
        }

        bool? IEdmTypeDefinitionWithFacets.IsUnicode
        {
            get { return this.typeDefinition.IsUnicode; }
        }

        int? IEdmTypeDefinitionWithFacets.Precision
        {
            get { return this.typeDefinition.Precision; }
        }

        int? IEdmTypeDefinitionWithFacets.Scale
        {
            get { return this.typeDefinition.Scale; }
        }

        int? IEdmTypeDefinitionWithFacets.Srid
        {
            get { return this.typeDefinition.Srid; }
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
