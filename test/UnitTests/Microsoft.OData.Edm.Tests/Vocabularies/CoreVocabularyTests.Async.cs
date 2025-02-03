//---------------------------------------------------------------------
// <copyright file="CoreVocabularyTests.Async.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Vocabularies
{
    public partial class CoreVocabularyTests
    {
        [Fact]
        public async Task TestBaseCoreVocabularyModel_Async()
        {
            var s = coreVocModel.FindDeclaredTerm("Org.OData.Core.V1.OptimisticConcurrency");
            Assert.NotNull(s);
            Assert.Equal("Org.OData.Core.V1", s.Namespace);
            Assert.Equal("OptimisticConcurrency", s.Name);

            var type = s.Type;
            Assert.Equal("Collection(Edm.PropertyPath)", type.FullName());
            Assert.Equal(EdmTypeKind.Collection, type.Definition.TypeKind);

            var descriptionTerm = coreVocModel.FindTerm("Org.OData.Core.V1.Description");
            Assert.NotNull(descriptionTerm);
            var descriptionType = descriptionTerm.Type.Definition as IEdmPrimitiveType;
            Assert.NotNull(descriptionType);
            Assert.Equal(EdmPrimitiveTypeKind.String, descriptionType.PrimitiveKind);

            var longDescriptionTerm = coreVocModel.FindTerm("Org.OData.Core.V1.LongDescription");
            Assert.NotNull(longDescriptionTerm);
            var longDescriptionType = longDescriptionTerm.Type.Definition as IEdmPrimitiveType;
            Assert.NotNull(longDescriptionType);
            Assert.Equal(EdmPrimitiveTypeKind.String, longDescriptionType.PrimitiveKind);

            var isLanguageDependentTerm = coreVocModel.FindTerm("Org.OData.Core.V1.IsLanguageDependent");
            Assert.NotNull(isLanguageDependentTerm);
            var isLanguageDependentType = isLanguageDependentTerm.Type.Definition as IEdmTypeDefinition;
            Assert.NotNull(isLanguageDependentType);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, isLanguageDependentType.UnderlyingType.PrimitiveKind);

            var requiresExplicitBindingTerm = coreVocModel.FindTerm("Org.OData.Core.V1.RequiresExplicitBinding");
            Assert.NotNull(requiresExplicitBindingTerm);
            var requiresExplicitBindingType = requiresExplicitBindingTerm.Type.Definition as IEdmTypeDefinition;
            Assert.NotNull(requiresExplicitBindingType);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, requiresExplicitBindingType.UnderlyingType.PrimitiveKind);

            var explicitOperationBindingsTerm = coreVocModel.FindTerm("Org.OData.Core.V1.ExplicitOperationBindings");
            Assert.NotNull(explicitOperationBindingsTerm);
            var explicitOperationBindingsType = explicitOperationBindingsTerm.Type.Definition;
            Assert.NotNull(explicitOperationBindingsType);
            Assert.Equal("Collection(Org.OData.Core.V1.QualifiedBoundOperationName)", explicitOperationBindingsType.FullTypeName());
            Assert.Equal(EdmTypeKind.Collection, explicitOperationBindingsType.TypeKind);

            var qualifiedBoundOperationNameType = coreVocModel.FindType("Org.OData.Core.V1.QualifiedBoundOperationName");
            Assert.NotNull(qualifiedBoundOperationNameType);
            Assert.Equal(qualifiedBoundOperationNameType, explicitOperationBindingsType.AsElementType());

            var sw = new StringWriter();
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                Encoding = System.Text.Encoding.UTF8,
                Async = true
            };

            XmlWriter xw = XmlWriter.Create(sw, settings);
            var (_, errors) = await coreVocModel.TryWriteSchemaAsync(xw).ConfigureAwait(false);
            await xw.FlushAsync().ConfigureAwait(false);
            xw.Close();
            string output = sw.ToString();
            Assert.False(errors.Any(), "No Errors");
            Assert.Equal(expectedText, output);
        }
    }
}
