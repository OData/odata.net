//---------------------------------------------------------------------
// <copyright file="ReflectionServiceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.PublicProviderTests
{
    using System;
    using Microsoft.OData.Client;
    using System.IO;
    using System.Linq;
    using Microsoft.Spatial;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.PublicProviderReflectionServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using HttpWebRequestMessage = Microsoft.Test.OData.Tests.Client.Common.HttpWebRequestMessage;

    [TestClass]
    public class ReflectionServiceTests : EndToEndTestBase
    {
        public ReflectionServiceTests()
            : base(ServiceDescriptors.PublicProviderReflectionService)
        {
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void ValidReadReflectionEntity()
        {
            var context = CreateWrappedContext<DefaultContainer>().Context;

            //Verify feed is working
            Assert.IsNotNull(context.Car.ToArray());
            //Verify filter is working
            Assert.IsNotNull(context.Car.Where(c => c.Description == Guid.NewGuid().ToString()).ToArray());
            //Verify paging and count is working
            Assert.IsTrue(context.Car.Count() == 10);
            //Verify navigation link is working
            Assert.IsNotNull(context.PersonMetadata.Expand("Person").FirstOrDefault().Person);
        }

        [TestMethod]
        public void ValidCUDReflectionEntity()
        {
            string desc = Guid.NewGuid().ToString();
            var context = CreateWrappedContext<DefaultContainer>().Context;
            var binaryTestData = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };

            //create
            var car = new Car { Description = desc, VIN = 1};
            context.AddToCar(car);
            context.SetSaveStream(car, new MemoryStream(new byte[] { 66, 67 }), true, "text/plain", "slug");
            var memoryStream = new MemoryStream(binaryTestData);
            context.SetSaveStream(car, "Video", memoryStream, true, new DataServiceRequestArgs { ContentType = "application/binary" });
            var memoryStream2 = new MemoryStream(binaryTestData);
            context.SetSaveStream(car, "Photo", memoryStream2, true, new DataServiceRequestArgs { ContentType = "application/binary" });
            context.SaveChanges();
            Assert.IsTrue(car.VIN != 0);
            Assert.AreEqual(1, context.Car.Where(c => c.Description == desc).Count());

            //update
            string newdesc = Guid.NewGuid().ToString();
            car.Description = newdesc;
            context.UpdateObject(car);
            context.SaveChanges();
            Assert.AreEqual(1, context.Car.Where(c => c.Description == newdesc).Count());

            //delete
            context.DeleteObject(car);
            context.SaveChanges();
            Assert.AreEqual(0, context.Car.Where(c => c.Description == newdesc).Count());
        }

        [TestMethod]
        public void ValidServiceOperationReflectionEntity()
        {
            var context = CreateWrappedContext<DefaultContainer>().Context;

            int count = context.GetPersonCount();
            Assert.AreEqual(count, context.Person.Count());

            var expectedPerson = context.Person.FirstOrDefault();

            var person = context.GetPersonByExactName(expectedPerson.Name);
            Assert.AreEqual(expectedPerson.PersonId, person.PersonId);

            var persons = context.GetPersonsByName(expectedPerson.Name.Substring(0, 3)).ToArray();
            Assert.IsTrue(persons.Any());
            Assert.IsTrue(persons.Any(p => p.PersonId == expectedPerson.PersonId));
        }
#endif

        [TestMethod]
        public void ValidMetadata()
        {
            var message = new HttpWebRequestMessage(new Uri(ServiceUri + "$metadata"));
            message.SetHeader("Accept", MimeTypes.ApplicationXml);

            using (var messageReader = new ODataMessageReader(message.GetResponse()))
            {
                var model = messageReader.ReadMetadataDocument();
                var container = model.EntityContainer;

                // Verify all the entities are exposed
                var entities = container.Elements.Where(e => e is IEdmEntitySet).ToArray();
                Assert.AreEqual(24, entities.Count());

                // Verify all the service operations are exposed
                var functions = container.Elements.Where(e => e is IEdmOperationImport).ToArray();
                Assert.AreEqual(3, functions.Count());
            }
        }

        [TestMethod]
        public void ValidServiceDocument()
        {
            var metadataMessage = new HttpWebRequestMessage(new Uri(ServiceUri + "$metadata"));
            metadataMessage.SetHeader("Accept", MimeTypes.ApplicationXml);

            var metadataMessageReader = new ODataMessageReader(metadataMessage.GetResponse());
            var model = metadataMessageReader.ReadMetadataDocument();

            var message = new HttpWebRequestMessage(ServiceUri);
            message.SetHeader("Accept", MimeTypes.ApplicationJson);
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceUri };

            using (var messageReader = new ODataMessageReader(message.GetResponse(), readerSettings, model))
            {
                var workspace = messageReader.ReadServiceDocument();
                Assert.AreEqual(24, workspace.EntitySets.Count());
            }
        }
    }
}
