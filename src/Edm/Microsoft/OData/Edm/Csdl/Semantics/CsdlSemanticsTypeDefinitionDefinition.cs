//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
