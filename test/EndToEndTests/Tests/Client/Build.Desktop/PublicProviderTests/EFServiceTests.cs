//---------------------------------------------------------------------
// <copyright file="EFServiceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.PublicProviderTests
{
    using System;
    using System.Linq;
    using Microsoft.Spatial;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.PublicProviderEFServiceReference.AstoriaDefaultServiceDBModel;
    using Microsoft.Test.OData.Services.TestServices.PublicProviderEFServiceReference.Microsoft.Test.OData.Services.Astoria;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EFServiceTests : EndToEndTestBase
    {
        public EFServiceTests()
            : base(ServiceDescriptors.PublicProviderEFService)
        {
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void ValidReadEFEntity()
        {
            var context = CreateWrappedContext<AstoriaDefaultServiceDBEntities>().Context;

            //Verify feed is working
            Assert.IsNotNull(context.EFCars.ToArray());
            //Verify filter is working
            Assert.IsNotNull(context.EFCars.Where(c => c.Description == Guid.NewGuid().ToString()).ToArray());
            //Verify paging is working
            Assert.AreEqual(10, context.EFCars.ToArray().Count());
            //Verify count is working
            Assert.IsTrue(context.EFCars.Count() >= 10);
            //Verify navigation link is working
            Assert.IsNotNull(context.EFPersonMetadatas.Expand("EFPerson").FirstOrDefault().EFPerson);
        }

        [TestMethod]
        public void ValidCUDEFEntity()
        {
            string desc = Guid.NewGuid().ToString();
            var context = CreateWrappedContext<AstoriaDefaultServiceDBEntities>().Context;

            //create
            var car = new EFCar {Description = desc};
            context.AddToEFCars(car);
            context.SaveChanges();
            Assert.IsTrue(car.VIN != 0);
            Assert.AreEqual(1, context.EFCars.Where(c => c.Description == desc).Count());

            //update
            string newdesc = Guid.NewGuid().ToString();
            car.Description = newdesc;
            context.UpdateObject(car);
            context.SaveChanges();
            Assert.AreEqual(1, context.EFCars.Where(c => c.Description == newdesc).Count());

            //delete
            context.DeleteObject(car);
            context.SaveChanges();
            Assert.AreEqual(0, context.EFCars.Where(c => c.Description == newdesc).Count());
        }

        [TestMethod]
        public void ValidServiceOperationEFEntity()
        {
            var context = CreateWrappedContext<AstoriaDefaultServiceDBEntities>().Context;

            int count = context.GetEFPersonCount();
            Assert.AreEqual(count, context.EFPersons.Count());

            var expectedPerson = context.EFPersons.FirstOrDefault();

            var person = context.GetEFPersonByExactName(expectedPerson.Name);
            Assert.AreEqual(expectedPerson.PersonId, person.PersonId);

            var persons = context.GetEFPersonsByName(expectedPerson.Name.Substring(0,3)).ToArray();
            Assert.IsTrue(persons.Any());
            Assert.IsTrue(persons.Any(p=>p.PersonId == expectedPerson.PersonId));
        }

        // Flaky test: OData.net GitHub #970
        [TestMethod]
        public void ValidMetadata()
        {
            var message = new HttpWebRequestMessage(new Uri(ServiceUri + "$metadata"));
            message.SetHeader("Accept", MimeTypes.ApplicationXml);

            using (var messageReader = new ODataMessageReader(message.GetResponse()))
            {
                var model = messageReader.ReadMetadataDocument();
                var container = model.EntityContainer;
                var entities = container.Elements.Where(e => e is IEdmEntitySet).ToArray();
                // Verify all the entities are exposed
                Assert.AreEqual(21, entities.Count(e => e.Name.StartsWith("EF")));
                Assert.IsTrue(entities.All(e => e.Name.StartsWith("EF")));

                // Verify all the service operation are exposed
                var functions = container.Elements.Where(e => e is IEdmOperationImport).ToArray();
                Assert.AreEqual(3, functions.Count());
            }
        }

        // Flaky test: OData.net GitHub #970
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
                Assert.AreEqual(21, workspace.EntitySets.Count(e => e.Name.StartsWith("EF")));
                Assert.IsTrue(workspace.EntitySets.All(e => e.Name.StartsWith("EF")));
            }
        }
#endif
    }
}
