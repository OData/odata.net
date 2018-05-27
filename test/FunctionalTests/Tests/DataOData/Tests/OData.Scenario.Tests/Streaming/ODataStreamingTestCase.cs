//---------------------------------------------------------------------
// <copyright file="ODataStreamingTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.Streaming
{
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;

    public class ODataStreamingTestCase : ODataReaderTestCase
    {
        public static ComplexInstance GetComplexInstanceWithManyPrimitiveProperties(EdmModel model)
        {
            var primitiveValues = TestValues.CreatePrimitiveValuesWithMetadata(true).Where(
                (v) =>
                {
                    var edmPrimitiveType =
                        v.Annotations.OfType<EntityModelTypeAnnotation>().Single().EdmModelType as
                        IEdmPrimitiveTypeReference;

                    var edmPrimitiveKind = edmPrimitiveType.PrimitiveKind();

                    if (edmPrimitiveKind != EdmPrimitiveTypeKind.None)
                        return edmPrimitiveKind != EdmPrimitiveTypeKind.DateTimeOffset;

                    return false;
                }).ToArray();

            string typeName = "ComplexTypeWithManyPrimitiveProperties";
            var complexType = model.ComplexType(typeName);
            for (int i = 0; i < primitiveValues.Count(); ++i)
            {
                complexType.Property("property" + i, primitiveValues[i].GetAnnotation<EntityModelTypeAnnotation>().EdmModelType);
            }

            var complexValue = PayloadBuilder.ComplexValue("TestModel." + typeName).WithTypeAnnotation(complexType);
            for (int j = 0; j < primitiveValues.Count(); ++j)
            {
                complexValue.PrimitiveProperty("property" + j, primitiveValues[j].ClrValue);
            }

            return complexValue;
        }

        protected override void ConfigureDependencies(Taupo.Contracts.DependencyInjectionContainer container)
        {
            base.ConfigureDependencies(container);
            container.Register<IProtocolFormatNormalizerSelector, DefaultProtocolFormatNormalizerSelector>();
            container.Register<IPayloadElementToJsonConverter, AnnotatedPayloadElementToJsonConverter>();
            container.Register<IPayloadElementToJsonLightConverter, AnnotatedPayloadElementToJsonLightConverter>();
            container.Register<IEntityModelSchemaComparer, ODataEntityModelSchemaComparer>();
        }
    }
}
