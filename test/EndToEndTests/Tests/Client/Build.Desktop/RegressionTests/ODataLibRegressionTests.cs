//---------------------------------------------------------------------
// <copyright file="ODataLibRegressionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.RegressionTests
{
    using System.IO;
    using Microsoft.Spatial;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;

    public class ODataLibRegressionTests
    {
        [Fact]
        public void WriterShouldNotIncludeTypeNameForCollectionOfDerivedType()
        {
            // JSON Light: writer doesn't include type name for collection of derived type
            // If I have a collection property declared in metadata as Collection(Edm.Geography), 
            // and at serialization type, it's clearly a Collection(Edm.GeographyPoint), 
            // we won't write the type name for that property by default (i.e., minimal metadata mode).

            var model = new EdmModel();
            var entityType = new EdmEntityType("Var1", "Type");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddProperty(new EdmStructuralProperty(entityType, "Geographies", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false)))));
            model.AddElement(entityType);

            var writerSettings = new ODataMessageWriterSettings();
            writerSettings.SetContentType(ODataFormat.Json);
            writerSettings.EnableMessageStreamDisposal = false;

            var message = new InMemoryMessage { Stream = new MemoryStream() };
            using (ODataMessageWriter odataMessageWriter = new ODataMessageWriter((IODataRequestMessage)message, writerSettings, model))
            {
                ODataWriter odataWriter = odataMessageWriter.CreateODataResourceWriter();
                odataWriter.WriteStart(
                    new ODataResource
                    {
                        TypeName = "Var1.Type",
                        Properties = new[]
                        {
                            new ODataProperty()
                            {
                                Name = "Id",
                                Value = 1
                            },
                            new ODataProperty()
                            {
                                Name = "Geographies",
                                Value = new ODataCollectionValue
                                {
                                    Items = new[]
                                    {
                                        GeographyPoint.Create(0,0),
                                        GeographyPoint.Create(1,1),
                                        GeographyPoint.Create(2,2)
                                    }
                                }
                            },
                        }
                    });
                odataWriter.WriteEnd();
                odataWriter.Flush();
            }

            message.Stream.Position = 0;
            var output = new StreamReader(message.Stream).ReadToEnd();
            Assert.False(output.Contains("Collection(Edm.GeographyPoint)"), @"output.Contains(""Collection(Edm.GeographyPoint)"" == false");
        }
    }
}

