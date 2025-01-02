//---------------------------------------------------------------------
// <copyright file="MeasuresVocabularyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.Measures.V1;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Vocabularies
{
    public class MeasuresVocabularyModelTests
    {
        private readonly IEdmModel model = MeasuresVocabularyModel.Instance;

        [Theory]
        [InlineData("ISOCurrency", EdmTypeKind.Primitive, "Edm.String")]
        [InlineData("Scale", EdmTypeKind.Primitive, "Edm.Byte")]
        [InlineData("Unit", EdmTypeKind.Primitive, "Edm.String")]
        [InlineData("DurationGranularity", EdmTypeKind.TypeDefinition, "Org.OData.Measures.V1.DurationGranularityType")]
        public void MeasuresVocabularyTerms(string termName, EdmTypeKind typeKind, string typeName)
        {
            var term = this.model.FindDeclaredTerm("Org.OData.Measures.V1." + termName);
            Assert.NotNull(term);

            Assert.NotNull(term.Type);
            Assert.Equal(typeKind, term.Type.Definition.TypeKind);
            var x = term.Type.Definition.FullTypeName();
            Assert.Equal(typeName, term.Type.Definition.FullTypeName());
        }

        [Theory]
        [InlineData("DurationGranularityType", EdmTypeKind.TypeDefinition, "Org.OData.Measures.V1.DurationGranularityType")]
        public void MeasuresVocabularyTypes(string termName, EdmTypeKind typeKind, string typeName)
        {
            var type = this.model.FindDeclaredType("Org.OData.Measures.V1." + termName);
            Assert.NotNull(type);

            Assert.Equal(typeKind, type.TypeKind);
            Assert.Equal(typeName, type.FullTypeName());
        }
    }
}

