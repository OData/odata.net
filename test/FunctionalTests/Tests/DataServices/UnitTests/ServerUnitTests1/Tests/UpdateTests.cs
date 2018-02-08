//---------------------------------------------------------------------
// <copyright file="UpdateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Data.Test.Astoria.Util;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.OData.Edm;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using efpoco = AstoriaUnitTests.EFFK;
    using ocs = AstoriaUnitTests.ObjectContextStubs;
    using provider = Microsoft.OData.Service.Providers;
    using Microsoft.OData.Client;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/868
    /// <summary>This class contains inner classes that can run as LTM tests.</summary>
    [TestModule]
    public partial class UnitTestModule1 : AstoriaTestModule
    {
        /// <summary>This is a test class for update, insert and delete functionality.</summary>
        [DeploymentItem("Workspaces", "Workspaces")]
        [TestClass, TestCase]
        public class UpdateTests : AstoriaTestCase
        {
            #region Insert/Post Tests
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePostError_SpecifyNullPayload()
            {
                string jsonPayLoad = "{ }";

                VerifyInvalidRequestForVariousProviders(jsonPayLoad, "/Customers", UnitTestsUtil.JsonLightMimeType, "POST", 400);

                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://www.w3.org/2005/Atom\" />";

                VerifyInvalidRequestForVariousProviders(atomPayload, "/Customers", UnitTestsUtil.AtomFormat, "POST", 400);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdateDeepPostWithInvalidLinkTypeValue()
            {
                string customerFullName = typeof(Customer).FullName;
                string orderFullName = typeof(Order).FullName;

                #region atompayload and xpath
                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                        AtomUpdatePayloadBuilder.GetCategoryXml(customerFullName) +
                        "<content type=\"application/xml\"><adsm:properties>" +
                            "<ads:Name>Foo</ads:Name>" +
                            "<ads:ID>125</ads:ID>" +
                        "</adsm:properties></content>" +
                        "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' title='BestFriend'>" +
                             "<adsm:inline>" +
                                "<entry>" + AtomUpdatePayloadBuilder.GetCategoryXml(customerFullName) +
                                    "<content type=\"application/xml\"><adsm:properties>" +
                                        "<ads:Name>FooBestFriend</ads:Name>" +
                                        "<ads:ID>126</ads:ID>" +
                                    "</adsm:properties></content>" +
                                    "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' type='application/atom+xml;type=entry' title='Orders'>" + // <- the type attribute value is incorrect
                                        "<adsm:inline>" +
                                            "<feed>" +
                                                "<entry>" + AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Order).FullName) +
                                                    "<content type=\"application/xml\"><adsm:properties>" +
                                                        "<ads:ID>151</ads:ID>" +
                                                        "<ads:DollarAmount adsm:type='Edm.Double'>1500.00</ads:DollarAmount>" +
                                                    "</adsm:properties></content>" +
                                                "</entry>" +
                                                "<entry>" + AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Order).FullName) +
                                                    "<content type=\"application/xml\"><adsm:properties>" +
                                                        "<ads:ID>152</ads:ID>" +
                                                        "<ads:DollarAmount adsm:type='Edm.Double'>500.00</ads:DollarAmount>" +
                                                    "</adsm:properties></content>" +
                                                "</entry>" +
                                                "<entry>" + AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Order).FullName) +
                                                    "<content type=\"application/xml\"><adsm:properties>" +
                                                        "<ads:ID>153</ads:ID>" +
                                                        "<ads:DollarAmount adsm:type='Edm.Double'>0.00</ads:DollarAmount>" +
                                                    "</adsm:properties></content>" +
                                                "</entry>" +
                                            "</feed>" +
                                        "</adsm:inline>" +
                                    "</link>" +
                                    "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' type='application/atom+xml;type=feed' href='Orders(101)' />" +
                                "</entry>" +
                            "</adsm:inline>" +
                        "</link>" +
                    "</entry>";

                #endregion

                VerifyInvalidRequestForVariousProviders(
                    atomPayload,
                    "/Customers",
                    UnitTestsUtil.AtomFormat,
                    "POST",
                    400);
            }

            [Variation]
            public void UpdatePostInvalidSyntaxPayload()
            {
                // Test matrix:
                // - Payload format.
                // - Kind of error.
                object[] requestVerbs = new object[]
                {
                    RequestVerb.Post,
                };

                Workspace workspace = this.Workspaces.FilterByName("Aruba")[0];
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("SerializationFormatKind", new object[] { SerializationFormatKind.JSON }),
                    new Dimension("RequestVerb", requestVerbs));
                TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                {
                    SerializationFormatKind serializationFormatKind = (SerializationFormatKind)values["SerializationFormatKind"];
                    RequestVerb requestVerb = (RequestVerb)values["RequestVerb"];

                    if (!workspace.Settings.SupportsUpdate)
                    {
                        return;
                    }

                    // Try every kind of syntax error on a payload.
                    foreach (ResourceContainer container in workspace.ServiceContainer.ResourceContainers)
                    {
                        ExpNode query = Query.From(Exp.Variable(container)).Select();

                        // Create a canonical payload for this resource.
                        ResourceBodyTree updateTreeNode = CanonicalResourcePayload(workspace, container);

                        // For every node, try every applicable syntax error.
                        foreach (ResourceBodyTree changedTree in Node.MutateAllNodes(updateTreeNode, SyntaxErrorMutator))
                        {
                            AstoriaRequest request = workspace.CreateRequest(query, changedTree, requestVerb);
                            AstoriaResponse response = request.GetResponse();
                            response.Verify();
                        }
                    }
                });
            }

            private static IEnumerable<Node> SyntaxErrorMutator(Node node)
            {
                foreach (PayloadSyntaxErrorKind kind in PayloadSyntaxError.GetApplicableKindsForNode(node))
                {
                    if (kind != PayloadSyntaxErrorKind.None)
                    {
                        Node newNode = (Node)node.Clone();
                        newNode.Facets.Add(new PayloadSyntaxErrorFacet(new PayloadSyntaxError(kind)));
                        yield return newNode;
                    }
                }
            }

            private ResourceBodyTree CanonicalResourcePayload(Workspace workspace, ResourceContainer resourceContainer)
            {
                ResourceType type = resourceContainer.BaseType;

                // TODO: support PUT by adding key support.

                ResourceInstanceProperty[] keyProperties = new ResourceInstanceProperty[0];
                ResourceInstanceKey keyExpression = new ResourceInstanceKey(resourceContainer, type, keyProperties);

                List<ResourceInstanceProperty> properties = new List<ResourceInstanceProperty>();
                foreach (NodeProperty p in type.Properties)
                {
                    // TODO: support to implement binding.
                    if (p.Type is ResourceCollection)
                    {
                        continue;
                    }
                    if (p.Type is ComplexType)
                    {
                        continue;
                    }

                    properties.Add(new ResourceInstanceSimpleProperty(p.Name, p.GetSampleValue()));
                }

                KeyedResourceInstance instance = new KeyedResourceInstance(keyExpression, properties.ToArray());
                return instance;
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePostSimple()
            {
                #region Payload

                var payloadGenerator = new PayloadBuilder() { TypeName = typeof(Customer).FullName }
                    .AddProperty("ID", 125)
                    .AddProperty("Name", "Foo")
                    .AddProperty("Address", new PayloadBuilder()
                    {
                        TypeName = typeof(Address).FullName,
                        IsComplex = true
                    }
                        .AddProperty("StreetAddress", "Street Number, Street Address")
                        .AddProperty("City", "Redmond")
                        .AddProperty("State", "WA")
                        .AddProperty("PostalCode", "98052"));

                #endregion Payload

                string[] jsonLiteXPaths = new string[] {
                    String.Format("/{0}[ID=125 and Name='Foo' and Address/StreetAddress='Street Number, Street Address' and Address/City='Redmond' and Address/State='WA' and Address/PostalCode='98052']",
                                  JsonValidator.ObjectString)};

                string[] atomXPaths = new string[] {
                    String.Format("/atom:entry[atom:category/@term='#{0}' and atom:id='http://host/Customers(125)' and atom:content/adsm:properties[ads:Name='Foo' and ads:ID=125 and ads:Address[ads:StreetAddress='Street Number, Street Address' and ads:City='Redmond' and ads:PostalCode='98052']]]",
                                  typeof(Customer).FullName)};

                DoInsertsForVariousProviders("/Customers", UnitTestsUtil.AtomFormat, payloadGenerator, atomXPaths);
                DoInsertsForVariousProviders("/Customers", UnitTestsUtil.JsonLightMimeType, payloadGenerator, jsonLiteXPaths);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePostInsertAndBindReferenceResource()
            {
                #region Payload
                PayloadBuilder payloadGenerator = new PayloadBuilder() { TypeName = typeof(Customer).FullName }
                    .AddProperty("Name", "Bar")
                    .AddProperty("ID", 125)
                    .AddProperty("BestFriend", new PayloadBuilder()
                    {
                        TypeName = typeof(CustomerWithBirthday).FullName,
                        Uri = "/Customers(1)",
                    });

                #endregion // Payload

                #region Atom XPaths
                var atomXPaths1 = new KeyValuePair<string, string[]>(
                    "/Customers",
                    new string[] { "/atom:entry[atom:category/@term='#AstoriaUnitTests.Stubs.Customer' and atom:id='http://host/Customers(125)' and atom:content/adsm:properties[ads:Name='Bar' and ads:ID='125']]" });

                var atomXPaths2 = new KeyValuePair<string, string[]>(
                    "/Customers(125)/BestFriend",
                    new string[] { "/atom:entry[atom:category/@term='#AstoriaUnitTests.Stubs.CustomerWithBirthday' and atom:id='http://host/Customers(1)' and atom:content/adsm:properties/ads:ID='1']" });
                #endregion

                #region JsonLite XPaths
                var jsonLiteXPaths1 = new KeyValuePair<string, string[]>(
                    "/Customers",
                    new string[] { String.Format("/{0}[ID=125 and Name='Bar']", JsonValidator.ObjectString) });

                var jsonLiteXPaths2 = new KeyValuePair<string, string[]>(
                    "/Customers(125)/BestFriend",
                    new string[] { String.Format("/{0}[odata.type='#{1}' and ID=1]", JsonValidator.ObjectString, typeof(CustomerWithBirthday).FullName) });
                #endregion

                UnitTestsUtil.DoInsertsForVariousProviders("/Customers", UnitTestsUtil.AtomFormat, payloadGenerator, new KeyValuePair<string, string[]>[] { atomXPaths1, atomXPaths2 }, true/*verifyETag*/);
                UnitTestsUtil.DoInsertsForVariousProviders("/Customers", UnitTestsUtil.JsonLightMimeType, payloadGenerator, new KeyValuePair<string, string[]>[] { jsonLiteXPaths1, jsonLiteXPaths2 }, true/*verifyETag*/);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePostDeepInsert()
            {
                string customerFullName = typeof(Customer).FullName;
                string orderFullName = typeof(Order).FullName;

                #region Payload
                PayloadBuilder payloadGenerator = new PayloadBuilder() { TypeName = typeof(Customer).FullName }
                    .AddProperty("Name", "Foo")
                    .AddProperty("ID", 125)
                    .AddProperty("BestFriend", new PayloadBuilder()
                    {
                        TypeName = typeof(Customer).FullName,
                        Uri = "/Customers(1)",
                    }
                        .AddProperty("Name", "FooBestFriend")
                        .AddProperty("ID", 126)
                        .AddProperty("Orders", new PayloadBuilder[] {
                            new PayloadBuilder()
                            {
                                TypeName = typeof(Order).FullName,
                                Uri = "/Orders(101)",
                            }
                            .AddProperty("ID", 151)
                            .AddProperty("DollarAmount", 1500.00),

                            new PayloadBuilder()
                            {
                                TypeName = typeof(Order).FullName,
                            }
                            .AddProperty("ID", 152)
                            .AddProperty("DollarAmount", 500.00),

                            new PayloadBuilder()
                            {
                                TypeName = typeof(Order).FullName
                            }
                            .AddProperty("ID", 153)
                            .AddProperty("DollarAmount", 00.00),

                            new PayloadBuilder()
                            {
                                TypeName = typeof(Order).FullName,
                                Uri = "/Orders(101)",
                            }
                        }));

                #endregion // Payload

                #region Atom XPath
                var atomXPaths1 = new KeyValuePair<string, string[]>(
                    "/Customers",
                    new string[] { "/atom:entry[atom:category/@term='#" + customerFullName + "' and atom:id='http://host/Customers(125)' and atom:content/adsm:properties[ads:Name='Foo' and ads:ID='125']]" });

                var atomXPaths2 = new KeyValuePair<string, string[]>(
                    "/Customers(125)/BestFriend",
                    new string[] { "/atom:entry[atom:category/@term='#" + customerFullName + "' and atom:id='http://host/Customers(126)' and atom:content/adsm:properties[ads:Name='FooBestFriend' and ads:ID='126']]" });

                var atomXPaths3 = new KeyValuePair<string, string[]>(
                    "/Customers",
                    new string[] { "/atom:feed/atom:entry[atom:category/@term='#" + customerFullName + "' and atom:id='http://host/Customers(125)' and atom:content/adsm:properties[ads:Name='Foo' and ads:ID='125']]",
                                   "/atom:feed/atom:entry[atom:category/@term='#" + customerFullName + "' and atom:id='http://host/Customers(126)' and atom:content/adsm:properties[ads:Name='FooBestFriend' and ads:ID='126']]"});

                var atomXPaths4 = new KeyValuePair<string, string[]>(
                    "/Customers(125)/BestFriend/Orders",
                    new string[] {
                        "/atom:feed/atom:entry[atom:category/@term='#" + orderFullName + "' and atom:id='http://host/Orders(151)' and atom:content/adsm:properties[ads:DollarAmount='1500' and ads:ID='151']]",
                        "/atom:feed/atom:entry[atom:category/@term='#" + orderFullName + "' and atom:id='http://host/Orders(152)' and atom:content/adsm:properties[ads:DollarAmount='500' and ads:ID='152']]",
                        "/atom:feed/atom:entry[atom:category/@term='#" + orderFullName + "' and atom:id='http://host/Orders(153)' and atom:content/adsm:properties[ads:DollarAmount='0' and ads:ID='153']]",
                        "/atom:feed/atom:entry[atom:category/@term='#" + orderFullName + "' and atom:id='http://host/Orders(101)' and atom:content/adsm:properties[ads:ID='101']]"
                    });

                var atomXPaths5 = new KeyValuePair<string, string[]>(
                    "/Orders",
                    new string[] {
                        "/atom:feed/atom:entry[atom:category/@term='#" + orderFullName + "' and atom:id='http://host/Orders(151)' and atom:content/adsm:properties[ads:DollarAmount='1500' and ads:ID='151']]",
                        "/atom:feed/atom:entry[atom:category/@term='#" + orderFullName + "' and atom:id='http://host/Orders(152)' and atom:content/adsm:properties[ads:DollarAmount='500' and ads:ID='152']]",
                        "/atom:feed/atom:entry[atom:category/@term='#" + orderFullName + "' and atom:id='http://host/Orders(153)' and atom:content/adsm:properties[ads:DollarAmount='0' and ads:ID='153']]"
                    });

                #endregion

                #region JsonLite XPath
                var jsonLiteXPaths1 = new KeyValuePair<string, string[]>(
                    "/Customers",
                    new string[] { String.Format("/{0}[ID=125 and Name='Foo']", JsonValidator.ObjectString) });

                var jsonLiteXPaths2 = new KeyValuePair<string, string[]>(
                    "/Customers(125)/BestFriend",
                    new string[] { String.Format("{0}[ID=126 and Name='FooBestFriend']", JsonValidator.ObjectString) });

                var jsonLiteXPaths3 = new KeyValuePair<string, string[]>(
                    "/Customers",
                    new string[] { String.Format("//{0}/{1}[ID=125 and Name='Foo']", JsonValidator.ArrayString, JsonValidator.ObjectString) });

                var jsonLiteXPaths4 = new KeyValuePair<string, string[]>(
                     "/Customers(125)/BestFriend/Orders",
                     new string[] {
                               String.Format("//{0}/{1}[ID=151 and DollarAmount=1500]", JsonValidator.ArrayString, JsonValidator.ObjectString),
                               String.Format("//{0}/{1}[ID=152 and DollarAmount=500]", JsonValidator.ArrayString, JsonValidator.ObjectString),
                               String.Format("//{0}/{1}[ID=153 and DollarAmount=0]", JsonValidator.ArrayString, JsonValidator.ObjectString),
                               String.Format("//{0}/{1}[ID=101]", JsonValidator.ArrayString, JsonValidator.ObjectString) });

                var jsonLiteXPaths5 = new KeyValuePair<string, string[]>(
                    "/Orders",
                    new string[] {
                               String.Format("//{0}/{1}[ID=151 and DollarAmount=1500]", JsonValidator.ArrayString, JsonValidator.ObjectString),
                               String.Format("//{0}/{1}[ID=152 and DollarAmount=500]", JsonValidator.ArrayString, JsonValidator.ObjectString),
                               String.Format("//{0}/{1}[ID=153 and DollarAmount=0]", JsonValidator.ArrayString, JsonValidator.ObjectString)
                             });
                #endregion

                UnitTestsUtil.DoInsertsForVariousProviders("/Customers", UnitTestsUtil.AtomFormat, payloadGenerator, new KeyValuePair<string, string[]>[] { atomXPaths1, atomXPaths2, atomXPaths3, atomXPaths4, atomXPaths5 }, true/*verifyETag*/);
                UnitTestsUtil.DoInsertsForVariousProviders("/Customers", UnitTestsUtil.JsonLightMimeType, payloadGenerator, new KeyValuePair<string, string[]>[] { jsonLiteXPaths1, jsonLiteXPaths2, jsonLiteXPaths3, jsonLiteXPaths4, jsonLiteXPaths5 }, true/*verifyETag*/);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePostInsertResourceToCollection()
            {
                #region Payload
                var payloadBuilder = new PayloadBuilder() { TypeName = typeof(Order).FullName }
                    .AddProperty("ID", 151)
                    .AddProperty("DollarAmount", 1500.00);
                #endregion

                #region AtomPayload And XPaths
                var atomXPath1 = new KeyValuePair<string, string[]>(
                    "/Customers(1)/Orders",
                    new string[] { "/atom:entry[atom:category/@term='#" + typeof(Order).FullName + "' and atom:id='http://host/Orders(151)' and atom:content/adsm:properties[ads:ID='151' and ads:DollarAmount='1500']]" });

                var atomXPath2 = new KeyValuePair<string, string[]>(
                    "/Orders",
                    new string[] { "/atom:feed/atom:entry[atom:category/@term='#" + typeof(Order).FullName + "' and atom:id='http://host/Orders(151)' and atom:content/adsm:properties[ads:ID='151' and ads:DollarAmount='1500']]" });
                #endregion //AtomPayload And XPaths

                #region JsonXPaths
                var jsonLiteXPath1 = new KeyValuePair<string, string[]>(
                    "/Customers(1)/Orders",
                    new string[] { String.Format("/{0}[ID=151 and DollarAmount=1500]", JsonValidator.ObjectString) });

                var jsonLiteXPath2 = new KeyValuePair<string, string[]>(
                    "/Orders",
                    new string[] { String.Format("//{0}/{1}[ID=151 and DollarAmount=1500]",
                                    JsonValidator.ArrayString, JsonValidator.ObjectString) });
                #endregion //JsonXPaths

                UnitTestsUtil.DoInsertsForVariousProviders("/Customers(1)/Orders", UnitTestsUtil.AtomFormat, payloadBuilder, new KeyValuePair<string, string[]>[] { atomXPath1, atomXPath2 }, false/*verifyETag*/);
                UnitTestsUtil.DoInsertsForVariousProviders("/Customers(1)/Orders", UnitTestsUtil.JsonLightMimeType, payloadBuilder, new KeyValuePair<string, string[]>[] { jsonLiteXPath1, jsonLiteXPath2 }, false/*verifyETag*/);
            }

            [Ignore]
            // [TestCategory("Partition1"), TestMethod, Variation]
            // ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            public void UpdatePostInsertResourceToCollectionRepeated()
            {
                AdHocEntityType orderType = new AdHocEntityType()
                {
                    Name = "OrderType",
                    Properties = new List<AdHocProperty>()
                    {
                        new AdHocScalarProperty() { Name = "OrderID", Type = new AdHocPrimitiveType(typeof(string)) },
                    }
                };
                orderType.KeyProperties = new List<AdHocProperty>() { orderType.Properties[0] };

                AdHocEntityType customerType = new AdHocEntityType()
                {
                    Name = "CustomerType",
                    Properties = new List<AdHocProperty>()
                    {
                        new AdHocScalarProperty() { Name = "CustomerID", Type = new AdHocPrimitiveType(typeof(string)) },
                    }
                };
                customerType.KeyProperties = new List<AdHocProperty>() { customerType.Properties[0] };

                AdHocAssociationType pendingOrdersType = new AdHocAssociationType()
                {
                    Name = "PendingOrdersType",
                    Ends = new List<AdHocAssociationTypeEnd>()
                    {
                        new AdHocAssociationTypeEnd() { RoleName = "PendingCustomer", Multiplicity = "1", Type = customerType },
                        new AdHocAssociationTypeEnd() { RoleName = "PendingOrder", Multiplicity = "*", Type = orderType },
                    }
                };
                AdHocAssociationType deliveredOrdersType = new AdHocAssociationType()
                {
                    Name = "DeliveredOrdersType",
                    Ends = new List<AdHocAssociationTypeEnd>()
                    {
                        new AdHocAssociationTypeEnd() { RoleName = "DeliveredCustomer", Multiplicity = "1", Type = customerType },
                        new AdHocAssociationTypeEnd() { RoleName = "DeliveredOrder", Multiplicity = "*", Type = orderType },
                    }
                };

                pendingOrdersType.AddNavigationProperties();
                deliveredOrdersType.AddNavigationProperties();

                AdHocEntitySet customers = new AdHocEntitySet() { Name = "Customers", Type = customerType };
                AdHocEntitySet orders = new AdHocEntitySet() { Name = "Orders", Type = orderType };
                AdHocContainer container = new AdHocContainer()
                {
                    EntitySets = new List<AdHocEntitySet>() { customers, orders },
                    AssociationSets = new List<AdHocAssociationSet>()
                    {
                        new AdHocAssociationSet()
                        {
                            Name = "PendingOrders",  Type = pendingOrdersType,
                            Ends = new List<AdHocAssociationSetEnd>()
                            {
                                new AdHocAssociationSetEnd() { EntitySet = customers, EndType = pendingOrdersType.OneEnd },
                                new AdHocAssociationSetEnd() { EntitySet = orders, EndType = pendingOrdersType.ManyEnd },
                            }
                        },
                        new AdHocAssociationSet()
                        {
                            Name = "DeliveredOrders",  Type = deliveredOrdersType,
                            Ends = new List<AdHocAssociationSetEnd>()
                            {
                                new AdHocAssociationSetEnd() { EntitySet = customers, EndType = deliveredOrdersType.OneEnd },
                                new AdHocAssociationSetEnd() { EntitySet = orders, EndType = deliveredOrdersType.ManyEnd },
                            }
                        },
                    }
                };

                AdHocModel model = new AdHocModel(container) { ConceptualNs = XmlConstants.EdmOasisNamespace };
                model.CreateDatabase();

                try
                {
                    Assembly assembly = model.GenerateModelsAndAssembly("UpdateInsertResourceToCollectionRepeated", false /* isReflectionProviderBased */);
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = TestUtil.LoadDerivedTypeFromAssembly(assembly, typeof(System.Data.Objects.ObjectContext));
                        request.RequestUriString = "/Customers";
                        request.HttpMethod = "POST";
                        request.RequestContentType = SerializationFormatData.JsonLight.MimeTypes[0];

                        request.SetRequestStreamAsText("{ CustomerID : \"C1\" }");
                        request.SendRequest();

                        // Same customer reference in to-one associations.
                        request.RequestUriString = "/Orders";
                        request.SetRequestStreamAsText(
                            "{ OrderID : \"O1\", " +
                            " PendingCustomer : { odata.readlink : \"/Customers('C1')\" } , " +
                            " DeliveredCustomer : { odata.readlink : \"/Customers('C1')\" }  " +
                            "}");
                        request.SendRequest();

                        // Same order reference in to-many associations.
                        request.RequestUriString = "/Customers";
                        request.SetRequestStreamAsText(
                            "{ CustomerID : \"C2\", " +
                            " PendingOrder : [ { odata.readlink : \"/Orders('O1')\" } ], " +
                            " DeliveredOrder : [ { odata.readlink : \"/Orders('O1')\" } ]  " +
                            "}");
                        request.SendRequest();
                    }
                }

                finally
                {
                    model.DropDatabase();
                }
            }

            internal static void DoInsertsForVariousProviders(string uri, string responseFormat, PayloadBuilder payloadGenerator, string[] xPaths)
            {
                UnitTestsUtil.DoInsertsForVariousProviders(uri, responseFormat, payloadGenerator, new KeyValuePair<string, string[]>[] { new KeyValuePair<string, string[]>(uri, xPaths) }, true /*verifyETag*/);
            }

            internal static void DoInsertsForVariousProviders(string uri, string responseFormat, string payload, KeyValuePair<string, string[]>[] uriAndXPathsToVerify, bool verifyETag)
            {
                UnitTestsUtil.DoInsertsForVariousProviders(uri, responseFormat, payload, uriAndXPathsToVerify, verifyETag);
            }

            #endregion Insert/Post Tests

            #region Delete Tests
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdateTopLevelDelete()
            {
                PerformDeletesForVariousProvider("/Customers(1)",
                    new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("/Customers", 1) }, true /*verifyETagScenarios*/);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdateDeleteResourceFromCollection()
            {
                string uri = "/Customers(1)/Orders(1)";
                var verifyObjectCountForUris = new KeyValuePair<string, int>[]
                    {
                        new KeyValuePair<string, int>("/Orders", 1),
                        new KeyValuePair<string, int>("/Customers(1)/Orders", 1)
                    };

                PerformDeletesForVariousProvider(uri, verifyObjectCountForUris, false /*verifyETagScenarios*/);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdateDeleteResourceFromReference()
            {
                string uri = "/Customers(1)/BestFriend";
                var verifyObjectCountForUris = new KeyValuePair<string, int>[]
                    {
                        new KeyValuePair<string, int>("/Customers", 1),
                        new KeyValuePair<string, int>("/Customers(1)/BestFriend", 1)
                    };

                PerformDeletesForVariousProvider(uri, verifyObjectCountForUris, true /*verifyETagScenarios*/);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdateDeepDelete()
            {
                string uri = "/Customers(1)/BestFriend/Orders(0)";
                var verifyObjectCountForUris = new KeyValuePair<string, int>[]
                    {
                        new KeyValuePair<string, int>("/Orders", 1),
                        new KeyValuePair<string, int>("/Customers(1)/BestFriend/Orders", 1),
                        new KeyValuePair<string, int>("/Customers(0)/Orders", 1)
                    };

                PerformDeletesForVariousProvider(uri, verifyObjectCountForUris, false /*verifyETagScenarios*/);
            }

            [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdateDeleteSystem()
            {
                string[] urls = new string[] { "/", "/$metadata", "/$batch" };
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("url", urls));
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.HttpMethod = "DELETE";
                    TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                    {
                        request.RequestUriString = (string)values["url"];
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        TestUtil.AssertExceptionExpected(exception, true);
                        TestUtil.AssertExceptionStatusCode(
                            exception,
                            405,
                            "DELETE requests to system resources should return 405 (Method Not Allowed)");
                    });
                }
            }

            private void PerformDeletesForVariousProvider(string uri, KeyValuePair<string, int>[] verifyObjectCountForUris, bool verifyETagScenarios)
            {
                foreach (Type providerType in UnitTestsUtil.ProviderTypes)
                {
                    using (UnitTestsUtil.CreateChangeScope(providerType))
                    {
                        DoDeleteWithETag(providerType, uri, verifyObjectCountForUris, UnitTestsUtil.AtomFormat, verifyETagScenarios);
                    }
                }
            }

            private static void DoDeleteWithETag(Type contextType, string uri, KeyValuePair<string, int>[] verifyObjectCountForUris, string responseFormat, bool verifyETagScenarios)
            {
                string newUri = UnitTestsUtil.ConvertUri(contextType, uri);

                var newVerifyObjectCountForUris = new KeyValuePair<string, int>[verifyObjectCountForUris.Length];
                for (int i = 0; i < newVerifyObjectCountForUris.Length; i++)
                {
                    newVerifyObjectCountForUris[i] = new KeyValuePair<string, int>(UnitTestsUtil.ConvertUri(contextType, verifyObjectCountForUris[i].Key), verifyObjectCountForUris[i].Value);
                }

                if (verifyETagScenarios)
                {
                    TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(responseFormat, newUri, contextType, null, "GET");
                    string etag = request.ResponseETag;
                    request.Dispose();
                    Assert.IsTrue(!String.IsNullOrEmpty(etag), "!String.IsNullOrEmpty(etag)");

                    // If-None-Match not allowed in DELETE method
                    var ifNoneMatch = new KeyValuePair<string, string>("If-None-Match", etag);
                    UnitTestsUtil.VerifyInvalidRequest(null, newUri, contextType, responseFormat, "DELETE", (int)HttpStatusCode.BadRequest, new KeyValuePair<string, string>[] { ifNoneMatch });

                    // invalid etag value specified
                    var ifMatch = new KeyValuePair<string, string>("If-Match", "W/\"sdfsdffweljrwerjwekr\"");
                    UnitTestsUtil.VerifyInvalidRequest(null, newUri, contextType, responseFormat, "DELETE", (int)HttpStatusCode.PreconditionFailed, new KeyValuePair<string, string>[] { ifMatch });

                    // no etag specified while deleting a type with etag properties
                    UnitTestsUtil.VerifyInvalidRequest(null, newUri, contextType, responseFormat, "DELETE", (int)HttpStatusCode.BadRequest, (KeyValuePair<string, string>[])null);

                    // specifying right etag should succeed
                    ifMatch = new KeyValuePair<string, string>("If-Match", etag);
                    UpdateTests.PerformDeletes(contextType, newUri, newVerifyObjectCountForUris, new KeyValuePair<string, string>[] { ifMatch });
                }
                else
                {
                    // check and make sure the etag header is null
                    TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(responseFormat, newUri, contextType, null, "GET");
                    string etag = request.ResponseETag;
                    request.Dispose();
                    Assert.IsTrue(String.IsNullOrEmpty(etag), "String.IsNullOrEmpty(etag)");

                    // If-None-Match not allowed in DELETE method
                    var ifNoneMatch = new KeyValuePair<string, string>("If-None-Match", "W/\"dfjhdfkjsf\"");
                    UnitTestsUtil.VerifyInvalidRequest(null, newUri, contextType, responseFormat, "DELETE", (int)HttpStatusCode.BadRequest, new KeyValuePair<string, string>[] { ifNoneMatch });

                    // Invalid If-Match etag value specified
                    var ifMatch = new KeyValuePair<string, string>("If-Match", "W/\"dhdsfjdshsdjf\"");
                    UnitTestsUtil.VerifyInvalidRequest(null, newUri, contextType, responseFormat, "DELETE", (int)HttpStatusCode.BadRequest, new KeyValuePair<string, string>[] { ifMatch });

                    // actual delete should succeed
                    UpdateTests.PerformDeletes(contextType, newUri, newVerifyObjectCountForUris, null);
                }
            }

            private static void PerformDeletes(Type contextType, string uri, KeyValuePair<string, int>[] verifyObjectCountForUris, KeyValuePair<string, string>[] headerValues)
            {
                string responseFormat = UnitTestsUtil.JsonLightMimeType;
                WebServerLocation location = WebServerLocation.InProcess;

                int[] objectCountBeforeDelete = null;
                int[] objectCountAfterDelete = null;

                if (verifyObjectCountForUris != null)
                {
                    objectCountBeforeDelete = new int[verifyObjectCountForUris.Length];
                    objectCountAfterDelete = new int[verifyObjectCountForUris.Length];

                    for (int i = 0; i < verifyObjectCountForUris.Length; i++)
                    {
                        objectCountBeforeDelete[i] = GetObjectCount(contextType, verifyObjectCountForUris[i].Key, responseFormat, location);
                    }
                }

                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = contextType;
                    request.RequestUriString = uri;
                    request.Accept = responseFormat;
                    request.HttpMethod = "DELETE";
                    if (headerValues != null)
                    {
                        foreach (KeyValuePair<string, string> header in headerValues)
                        {
                            if (header.Key == "If-Match")
                            {
                                request.IfMatch = header.Value;
                            }
                            else if (header.Key == "If-None-Match")
                            {
                                request.IfNoneMatch = header.Value;
                            }
                            else
                            {
                                Assert.Fail(String.Format("Invalid '{0}' header name specified", header.Key));
                            }
                        }
                    }
                    request.SendRequest();
                }

                if (verifyObjectCountForUris != null)
                {
                    for (int i = 0; i < verifyObjectCountForUris.Length; i++)
                    {
                        objectCountAfterDelete[i] = GetObjectCount(contextType, verifyObjectCountForUris[i].Key, responseFormat, location);
                        if (verifyObjectCountForUris[i].Value == 404)
                        {
                            Assert.AreEqual<int>(verifyObjectCountForUris[i].Value, objectCountAfterDelete[i]);
                        }
                        else
                        {
                            Assert.AreEqual<int>(objectCountAfterDelete[i] + verifyObjectCountForUris[i].Value, objectCountBeforeDelete[i]);
                        }
                    }
                }

                if (uri.EndsWith("$value") || uri.EndsWith(")"))
                {
                    UnitTestsUtil.VerifyInvalidUri(uri, contextType);
                }
            }

            private static int GetObjectCount(Type contextType, string uri, string responseFormat, WebServerLocation location)
            {
                try
                {
                    Stream responseStream = UnitTestsUtil.GetResponseStream(
                        location, responseFormat,
                        uri, contextType);

                    XmlDocument document = UnitTestsUtil.VerifyXPaths(responseStream, responseFormat, new string[0]);

                    return document.SelectNodes(
                                 "//" + JsonValidator.ObjectString, new XmlNamespaceManager(document.NameTable)).Count;
                }
                catch (Exception e)
                {
                    // For open type provider, delete a resource via navigation property uri will result in 404
                    if (((DataServiceException)e.InnerException).StatusCode == 404)
                    {
                        return 404;
                    }

                    throw;
                }
            }

            #endregion Delete Tests

            #region Update/Put Tests
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePutError_SpecifyNullPayloadInUpdate()
            {
                string payLoad = "{" +
                                 "}";

                string atomPayload =
                    "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" " +
                        "xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://www.w3.org/2005/Atom\">" +
                    "</entry>";

                VerifyInvalidRequestForVariousProviders(atomPayload, "/Customers(1)", UnitTestsUtil.AtomFormat, "PUT", 400);
                VerifyInvalidRequestForVariousProviders(payLoad, "/Customers(1)", UnitTestsUtil.JsonLightMimeType, "PUT", 400);
            }

            [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePutError_SpecifyPayLoadWithMetadataInfoOnlyInUpdate()
            {
                string jsonLightPayload = "{ @odata.type:\"#AstoriaUnitTests.Stubs.CustomerWithBirthday\", odata.editlink:\"/Customers(1)\" }";

                VerifyInvalidRequestForVariousProviders(jsonLightPayload, "/Customers(1)", UnitTestsUtil.JsonLightMimeType, "PUT", 400);
            }

            [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePutError_CannotPutToCollectionPropertyInUpdate()
            {
                string payLoad = "[ " +
                                    "{" +
                                        "@odata.type:\"#AstoriaUnitTests.Stubs.Order\"" +
                                    "}," +
                                    "{" +
                                        "@odata.type:\"#AstoriaUnitTests.Stubs.Order\"" +
                                    "}" +
                                   "]";

                VerifyInvalidRequestForVariousProviders(payLoad, "/Customers(1)/BestFriend/Orders", UnitTestsUtil.JsonLightMimeType, "PUT", 405);
            }

            [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePutError_UpdateTopLevelResourceViaEntitySet()
            {
                DateTime birthday = new DateTime(1979, 02, 15);
                string jsonBirthdayString = JsonValidator.GetJsonDateTimeStringValue(birthday);
                string jsonPayload = "[" +
                                     "{" +
                                        "@odata.type:\"#AstoriaUnitTests.Stubs.CustomerWithBirthday\"," +
                                        "Name: \"Foo\"," +
                                        "Birthday: \"" + jsonBirthdayString + "\"," +
                                     "}" +
                                 "]";

                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<feed xml:base=\"/\" xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" " +
                            "xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\" " +
                            "xmlns=\"http://www.w3.org/2005/Atom\">" +
                        "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                            AtomUpdatePayloadBuilder.GetCategoryXml(typeof(CustomerWithBirthday).FullName) +
                            "<id>/Customers(1)</id>" +
                            "<content type=\"application/xml\"><adsm:properties>" +
                                "<ads:Name>Foo</ads:Name>" +
                                "<ads:Birthday>" + TypeData.XmlValueFromObject(birthday) + "</ads:Birthday>" +
                            "</adsm:properties></content>" +
                        "</entry>" +
                    "</feed>";

                VerifyInvalidRequestForVariousProviders(jsonPayload, "/Customers", UnitTestsUtil.JsonLightMimeType, "PUT", 405);
                VerifyInvalidRequestForVariousProviders(atomPayload, "/Customers", UnitTestsUtil.AtomFormat, "PUT", 405);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePutError_ErrorInEntityPayload()
            {
                string jsonPayload = "{" +
                                        "@odata.type:\"#AstoriaUnitTests.Stubs.CustomerWithBirthday\"," +
                                        "Name: \"Foo" +  // Missing " after Foo
                                     "}";

                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                        AtomUpdatePayloadBuilder.GetCategoryXml(typeof(CustomerWithBirthday).FullName) +
                            "<content type=\"application/xml\"><adsm:properties>" +
                                "<ads:Name>Foo" +  // Missing end element ads:Name
                            "</adsm:properties></content>" +
                    "</entry>";

                VerifyInvalidRequestForVariousProviders(jsonPayload, "/Customers(1)", UnitTestsUtil.JsonLightMimeType, "PUT", 400);
                VerifyInvalidRequestForVariousProviders(atomPayload, "/Customers(1)", UnitTestsUtil.AtomFormat, "PUT", 400);
            }

            [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePutMultipleTopLevelResourceInEntitySet()
            {
                DateTime birthday = new DateTime(1979, 02, 15);
                string jsonBirthdayValue = JsonValidator.GetJsonDateTimeStringValue(birthday);
                string jsonPayload = "[" +
                                     "{" +
                                        "@odata.type:\"#AstoriaUnitTests.Stubs.CustomerWithBirthday\"," +
                                        "Name: \"Foo\"," +
                                        "Birthday: \"" + jsonBirthdayValue + "\"" +
                                     "}," +
                                     "{" +
                                        "@odata.type:\"#AstoriaUnitTests.Stubs.Customer\"," +
                                        "Name: \"Foo1\"" +
                                     "}" +
                                 "]";

                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<feed xml:base=\"/\" xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://www.w3.org/2005/Atom\">" +
                        "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                            AtomUpdatePayloadBuilder.GetCategoryXml(typeof(CustomerWithBirthday).FullName) +
                            "<id>/Customers(1)</id>" +
                            "<content type=\"application/xml\"><adsm:properties>" +
                                "<ads:Name>Foo</ads:Name>" +
                                "<ads:Birthday>" + TypeData.XmlValueFromObject(birthday) + "</ads:Birthday>" +
                            "</adsm:properties></content>" +
                        "</entry>" +
                        "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                            AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Customer).FullName) +
                            "<id>/Customers(0)</id>" +
                            "<content type=\"application/xml\"><adsm:properties>" +
                                "<ads:Name>Foo1</ads:Name>" +
                            "</adsm:properties></content>" +
                        "</entry>" +
                    "</feed>";

                VerifyInvalidRequestForVariousProviders(jsonPayload, "/Customers", UnitTestsUtil.JsonLightMimeType, "PUT", 405);
                VerifyInvalidRequestForVariousProviders(atomPayload, "/Customers", UnitTestsUtil.AtomFormat, "PUT", 405);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePutTopLevelResourceDirectly()
            {
                PayloadBuilder payloadBuilder =
                    new PayloadBuilder()
                    {
                        TypeName = typeof(CustomerWithBirthday).FullName
                    }
                    .AddProperty("Name", "Foo");

                string[] atomXPath = new string[] {
                    "/atom:entry[atom:category/@term='#" + typeof(CustomerWithBirthday).FullName + "' and atom:id='http://host/Customers(1)' and atom:content/adsm:properties[ads:ID='1' and ads:Name='Foo']]" };

                string[] jsonLiteXPath = new string[] {
                    String.Format("/{0}[odata.type='#{1}' and ID=1 and Name='Foo']",
                                    JsonValidator.ObjectString,
                                    typeof(CustomerWithBirthday).FullName),
                             };

                DoUpdatesForVariousProviders("PATCH", "/Customers(1)", UnitTestsUtil.AtomFormat, payloadBuilder, atomXPath, true/*verifyETag*/);
                DoUpdatesForVariousProviders("PATCH", "/Customers(1)", UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteXPath, true/*verifyETag*/);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePutTopLevelResourceDirectlyWithoutMetadataInformation()
            {
                PayloadBuilder payloadBuilder =
                   new PayloadBuilder().AddProperty("DollarAmount", 1000.95);

                string[] atomXPath = new string[] {
                    "/atom:entry[atom:category/@term='#" + typeof(Order).FullName + "' and atom:id='http://host/Orders(1)' and atom:content/adsm:properties[ads:ID='1' and ads:DollarAmount='1000.95']]" };

                string[] jsonLiteXPath = new string[] {
                    String.Format("/{0}[ID=1 and DollarAmount=1000.95]", JsonValidator.ObjectString)
                             };

                DoUpdatesForVariousProviders("PATCH", "/Orders(1)", UnitTestsUtil.AtomFormat, payloadBuilder, atomXPath, false);
                DoUpdatesForVariousProviders("PATCH", "/Orders(1)", UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteXPath, false);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePutDeepReferencePropertyUpdate()
            {
                PayloadBuilder payloadBuilder =
                    new PayloadBuilder()
                    {
                        TypeName = typeof(CustomerWithBirthday).FullName
                    }
                   .AddProperty("BestFriend", new PayloadBuilder()
                   {
                       Uri = "/Customers(0)"
                   }.AddProperty("Name", "Foo"));

                VerifyInvalidRequestForVariousProviders1(payloadBuilder, "/Customers(1)", UnitTestsUtil.AtomFormat, "PUT", 400);
                VerifyInvalidRequestForVariousProviders1(payloadBuilder, "/Customers(1)", UnitTestsUtil.JsonLightMimeType, "PUT", 400);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePut_ChangeReferenceRelationshipInPayload()
            {
                string customerFullName = typeof(Customer).FullName;

                PayloadBuilder payloadBuilder =
                    new PayloadBuilder()
                    {
                        TypeName = typeof(Customer).FullName
                    }
                   .AddProperty("BestFriend", new PayloadBuilder()
                   {
                       TypeName = typeof(Customer).FullName,
                       Uri = "/Customers(1)/BestFriend"
                   });

                var atomUriAndXPath1 = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>("/Customers(2)", new string[] { "/atom:entry[atom:category/@term='#" + typeof(Customer).FullName + "' and atom:id='http://host/Customers(2)']" }),
                    new KeyValuePair<string, string[]>("/Customers(2)/BestFriend",
                     new string[] { "/atom:entry[atom:category/@term='#" + customerFullName + "' and atom:id='http://host/Customers(0)' and atom:content/adsm:properties[ads:ID='0']]" })};

                var jsonLiteUriAndXPath1 = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>("/Customers(2)/BestFriend",
                        new string[] { String.Format("/{0}[ID=0]",
                                    JsonValidator.ObjectString) })};

                DoUpdatesForVariousProviders("PATCH", "/Customers(2)", UnitTestsUtil.AtomFormat, payloadBuilder, atomUriAndXPath1, true, true /*verifyUpdateResponse*/);
                DoUpdatesForVariousProviders("PATCH", "/Customers(2)", UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteUriAndXPath1, true, true /*verifyUpdateResponse*/);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePut_ChangeReferenceSetRelationshipInPayload()
            {
                PayloadBuilder payloadBuilder = new PayloadBuilder() { TypeName = typeof(Customer).FullName }
                    .AddProperty("Orders", new PayloadBuilder[] {
                        new PayloadBuilder() { Uri = "/Orders(0)" },
                        new PayloadBuilder() { Uri = "/Orders(100)" }});

                var atomUriAndXPath1 = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>("/Customers(2)", new string[] { "/atom:entry[atom:category/@term='#" + typeof(Customer).FullName + "' and atom:id='http://host/Customers(2)']" }),
                    new KeyValuePair<string, string[]>(
                        "/Customers(2)/Orders",
                        new string[] { "/atom:feed/atom:entry[atom:category/@term='#" + typeof(Order).FullName + "' and atom:id='http://host/Orders(0)']",
                                       "/atom:feed/atom:entry[atom:category/@term='#" + typeof(Order).FullName + "' and atom:id='http://host/Orders(100)']"}) };

                var jsonLiteUriAndXPath1 = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>("/Customers(2)/Orders",
                        new string[] {
                            String.Format(
                                "//{0}/{1}[ID=0]",
                                JsonValidator.ArrayString,
                                JsonValidator.ObjectString),
                            String.Format(
                                "//{0}/{1}[ID=100]",
                                JsonValidator.ArrayString,
                                JsonValidator.ObjectString)
                        }) };

                DoUpdatesForVariousProviders("PATCH", "/Customers(2)", UnitTestsUtil.AtomFormat, payloadBuilder, atomUriAndXPath1, true, true /*verifyUpdateResponse*/);
                DoUpdatesForVariousProviders("PATCH", "/Customers(2)", UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteUriAndXPath1, true, true /*verifyUpdateResponse*/);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePutDeepResourceSetPropertyUpdate()
            {
                string jsonPayload = "{" +
                                        "@odata.type:\"#AstoriaUnitTests.Stubs.CustomerWithBirthday\"," +
                                        "BestFriend: { " +
                                            "@odata.type:\"#AstoriaUnitTests.Stubs.Customer\"," +
                                            "Orders: [ " +
                                                "{" +
                                                    "DollarAmount: 5555" +
                                                "}," +
                                                "{" +
                                                    "DollarAmount: 6767" +
                                                "}" +
                                            "]" +
                                        "}" +
                                     "}";

                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                        "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                            AtomUpdatePayloadBuilder.GetCategoryXml(typeof(CustomerWithBirthday).FullName) +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' title='BestFriend'>" +
                                "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                                    AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Customer).FullName) +
                                    "<id>/Customers(0)</id>" +
                                    "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' title='Orders'>" +
                                        "<feed>" +
                                            "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                                                AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Order).FullName) +
                                                "<id>/Orders(0)</id>" +
                                                "<content type=\"application/xml\"><adsm:properties>" +
                                                    "<ads:DollarAmount>5555</ads:DollarAmount>" +
                                                "</adsm:properties></content>" +
                                            "</entry>" +
                                            "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                                                AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Order).FullName) +
                                                "<id>/Orders(100)</id>" +
                                                "<content type=\"application/xml\"><adsm:properties>" +
                                                    "<ads:DollarAmount>6767</ads:DollarAmount>" +
                                                "</adsm:properties></content>" +
                                            "</entry>" +
                                        "</feed>" +
                                    "</link>" +
                                "</entry>" +
                            "</link>" +
                        "</entry>";

                VerifyInvalidRequestForVariousProviders(jsonPayload, "/Customers(1)", UnitTestsUtil.JsonLightMimeType, "PUT", 400);
                VerifyInvalidRequestForVariousProviders(atomPayload, "/Customers(1)", UnitTestsUtil.AtomFormat, "PUT", 400);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePutDeepResourceReferencePropertyToNull()
            {
                var payloadBuilder = new PayloadBuilder() { TypeName = typeof(CustomerWithBirthday).FullName }
                    .AddNavigationReferenceProperty("BestFriend", null);

                var atomUriAndXPath1 = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>("/Customers(1)", new string[] { "/atom:entry[atom:category/@term='#" + typeof(CustomerWithBirthday).FullName + "' and atom:id='http://host/Customers(1)']" }),
                    new KeyValuePair<string, string[]>("/Customers(1)?$expand=BestFriend",
                    new string[] {
                        "atom:entry[atom:category/@term='#" + typeof(CustomerWithBirthday).FullName + "' and atom:id='http://host/Customers(1)' and atom:link[@title='BestFriend' and adsm:inline[string()='']]]",
                    })};

                var jsonLiteUriAndXPath1 = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>("/Customers(1)?$expand=BestFriend",
                    new string[] { String.Format("/{0}[odata.type='#{1}' and ID=1 and BestFriend/@IsNull='true']",
                                    JsonValidator.ObjectString,
                                    typeof(CustomerWithBirthday).FullName),
                                })};

                DoUpdatesForVariousProvidersWithOpenMissing("PATCH", "/Customers(1)", UnitTestsUtil.AtomFormat, payloadBuilder, atomUriAndXPath1, true/*verifyETag*/);
                DoUpdatesForVariousProvidersWithOpenMissing("PATCH", "/Customers(1)", UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteUriAndXPath1, true/*verifyETag*/);
            }

            internal enum AddressableElementKind
            {
                Metadata,
                ServiceDocument,
                EntitySet,
                Entity,
                EntityPrimitiveProperty,
                EntityPrimitivePropertyValue,
                EntityComplexPropertyValue,
                EntityComplexProperty,
                EntityReference,
                EntityCollectionReference,
                EntityPrimitiveOpenProperty,
                EntityPrimitiveOpenPropertyValue,
                EntityComplexOpenPropertyValue,
                EntityComplexOpenProperty,
                EntityOpenReference,
                EntityOpenCollectionReference,
            }

            [DebuggerDisplay("{Kind}")]
            internal class AddressableElementData
            {
                private static AddressableElementData[] values;

                private AddressableElementData()
                {
                }

                internal AddressableElementKind Kind { get; private set; }

                internal bool Addressable
                {
                    get
                    {
                        return true;
                    }
                }

                internal static AddressableElementData ForKind(AddressableElementKind kind)
                {
                    return Values.Where((v) => v.Kind == kind).Single();
                }

                internal static AddressableElementData[] Values
                {
                    get
                    {
                        if (values == null)
                        {
                            values = new AddressableElementData[]
                            {
                                new AddressableElementData() { Kind = AddressableElementKind.Metadata },
                                new AddressableElementData() { Kind = AddressableElementKind.ServiceDocument },
                                new AddressableElementData() { Kind = AddressableElementKind.EntitySet },
                                new AddressableElementData() { Kind = AddressableElementKind.Entity },
                                new AddressableElementData() { Kind = AddressableElementKind.EntityPrimitiveProperty },
                                new AddressableElementData() { Kind = AddressableElementKind.EntityPrimitivePropertyValue },
                                new AddressableElementData() { Kind = AddressableElementKind.EntityComplexPropertyValue },
                                new AddressableElementData() { Kind = AddressableElementKind.EntityComplexProperty },
                                new AddressableElementData() { Kind = AddressableElementKind.EntityReference },
                                new AddressableElementData() { Kind = AddressableElementKind.EntityCollectionReference },
                                new AddressableElementData() { Kind = AddressableElementKind.EntityPrimitiveOpenProperty },
                                new AddressableElementData() { Kind = AddressableElementKind.EntityPrimitiveOpenPropertyValue },
                                new AddressableElementData() { Kind = AddressableElementKind.EntityComplexOpenPropertyValue },
                                new AddressableElementData() { Kind = AddressableElementKind.EntityComplexOpenProperty },
                                new AddressableElementData() { Kind = AddressableElementKind.EntityOpenReference },
                                new AddressableElementData() { Kind = AddressableElementKind.EntityOpenCollectionReference },
                            };
                        }
                        return values;
                    }
                }

                internal bool IsPrimitive
                {
                    get
                    {
                        return
                            this.Kind == AddressableElementKind.EntityComplexOpenPropertyValue ||
                            this.Kind == AddressableElementKind.EntityComplexPropertyValue ||
                            this.Kind == AddressableElementKind.EntityPrimitiveOpenPropertyValue ||
                            this.Kind == AddressableElementKind.EntityPrimitivePropertyValue;
                    }
                }

                internal bool SupportsKeys
                {
                    get
                    {
                        return
                            this.Kind == AddressableElementKind.Entity ||
                            this.Kind == AddressableElementKind.EntityOpenReference ||
                            this.Kind == AddressableElementKind.EntityReference;
                    }
                }

                internal bool SystemGenerated
                {
                    get
                    {
                        return
                            this.Kind == AddressableElementKind.Metadata ||
                            this.Kind == AddressableElementKind.ServiceDocument;
                    }
                }

                internal Stream BuildRequestBody(
                    string method,
                    SerializationFormatData format,
                    ServiceModelData model,
                    bool includeKeys,
                    bool includeId)
                {
                    if (method == "GET" || method == "DELETE")
                    {
                        return null;
                    }
                    else if (!this.Addressable || this.SystemGenerated)
                    {
                        return new MemoryStream(Encoding.UTF8.GetBytes("foo"));
                    }

                    string text = null;
                    switch (this.Kind)
                    {
                        case AddressableElementKind.Metadata:
                        case AddressableElementKind.ServiceDocument:
                        case AddressableElementKind.EntitySet:
                            break;
                        case AddressableElementKind.Entity:
                            string containerName = model.SampleContainer;
                            string containerTypeName = model.GetContainerRootTypeName(containerName);
                            string keyName = model.GetKeyProperties(containerTypeName).Single().Name;
                            string nonKeyName = model.GetNonKeyPrimitiveProperties(containerTypeName).First().Name;
                            if (format == SerializationFormatData.Atom)
                            {
                                // null.
                                //text = "<entry " +
                                //    "xmlns:ads='http://docs.oasis-open.org/odata/ns/data' " +
                                //    "xmlns:adsm='http://docs.oasis-open.org/odata/ns/metadata' " +
                                //    "xmlns='http://www.w3.org/2005/Atom' " +
                                //    "adsm:null='true' />";

                                // valid payload.
                                text = "<entry " +
                                    "xmlns:ads='http://docs.oasis-open.org/odata/ns/data' " +
                                    "xmlns:adsm='http://docs.oasis-open.org/odata/ns/metadata' " +
                                    "xmlns='http://www.w3.org/2005/Atom'>\r\n" +
                                    ((includeId) ? String.Format(" <id>{0}</id>", this.BuildRequestUri(model)) : " <id />") +
                                    "\r\n <author><name /></author>\r\n" +
                                    " <content type='application/xml'><adsm:properties>\r\n" +
                                    String.Format("  <ads:{0}>123</ads:{0}>", nonKeyName) +
                                    ((includeKeys) ? String.Format("  <ads:{0}>9999</ads:{0}>", keyName) : "") +
                                    "\r\n </adsm:properties></content>" +
                                    "</entry>";
                            }
                            else if (format == SerializationFormatData.JsonLight)
                            {
                            }
                            break;
                        case AddressableElementKind.EntityPrimitivePropertyValue:
                        case AddressableElementKind.EntityPrimitiveOpenPropertyValue:
                        case AddressableElementKind.EntityPrimitiveOpenProperty:
                            return new MemoryStream(Encoding.UTF8.GetBytes("1"));
                        case AddressableElementKind.EntityReference:
                            return null;
                        case AddressableElementKind.EntityCollectionReference:
                            return null;
                        case AddressableElementKind.EntityComplexOpenPropertyValue:
                            return null;
                        case AddressableElementKind.EntityComplexOpenProperty:
                            return null;
                        case AddressableElementKind.EntityOpenReference:
                            return null;
                        case AddressableElementKind.EntityOpenCollectionReference:
                            return null;
                    }

                    if (text == null)
                    {
                        return null;
                    }
                    else
                    {
                        return new MemoryStream(Encoding.UTF8.GetBytes(text));
                    }
                }

                internal string BuildRequestUri(ServiceModelData model)
                {
                    if (model == null)
                    {
                        throw new ArgumentNullException("model");
                    }

                    string sampleContainer = model.SampleContainer;
                    switch (this.Kind)
                    {
                        case AddressableElementKind.Metadata:
                            return "/$metadata";
                        case AddressableElementKind.ServiceDocument:
                            return "/";
                        case AddressableElementKind.EntitySet:
                            return "/" + sampleContainer;
                        case AddressableElementKind.Entity:
                            return "/" + sampleContainer + "(" + model.GetSampleKeyForUri(sampleContainer) + ")";
                        case AddressableElementKind.EntityPrimitiveProperty:
                            return "/" + sampleContainer + "(" + model.GetSampleKeyForUri(sampleContainer) + ")/" +
                                model.GetSampleNonKeyPrimitiveProperty(sampleContainer).Name;
                        case AddressableElementKind.EntityPrimitivePropertyValue:
                            return "/" + sampleContainer + "(" + model.GetSampleKeyForUri(sampleContainer) + ")/" +
                                model.GetSampleNonKeyPrimitiveProperty(sampleContainer).Name + "/$value";
                        case AddressableElementKind.EntityComplexPropertyValue:
                            return null;
                        case AddressableElementKind.EntityComplexProperty:
                            return null;
                        case AddressableElementKind.EntityReference:
                            return null;
                        case AddressableElementKind.EntityCollectionReference:
                            return null;
                        case AddressableElementKind.EntityPrimitiveOpenProperty:
                            return null;
                        case AddressableElementKind.EntityPrimitiveOpenPropertyValue:
                            return null;
                        case AddressableElementKind.EntityComplexOpenPropertyValue:
                            return null;
                        case AddressableElementKind.EntityComplexOpenProperty:
                            return null;
                        case AddressableElementKind.EntityOpenReference:
                            return null;
                        case AddressableElementKind.EntityOpenCollectionReference:
                            return null;
                    }

                    return null;
                }

                internal bool SupportsMethod(string method)
                {
                    switch (method)
                    {
                        case "GET":
                            return this.Addressable;
                        case "PUT":
                            return this.Addressable && !this.SystemGenerated &&
                                (this.Kind == AddressableElementKind.Entity ||
                                 this.Kind == AddressableElementKind.EntityOpenReference ||
                                 this.Kind == AddressableElementKind.EntityPrimitiveOpenPropertyValue ||
                                 this.Kind == AddressableElementKind.EntityPrimitivePropertyValue ||
                                 this.Kind == AddressableElementKind.EntityReference);
                        case "POST":
                            return
                                this.Kind == AddressableElementKind.EntityCollectionReference ||
                                this.Kind == AddressableElementKind.EntityOpenCollectionReference ||
                                this.Kind == AddressableElementKind.EntitySet;
                        case "DELETE":
                            return
                                this.Kind == AddressableElementKind.Entity ||
                                this.Kind == AddressableElementKind.EntityOpenReference ||
                                this.Kind == AddressableElementKind.EntityPrimitiveOpenPropertyValue ||
                                this.Kind == AddressableElementKind.EntityPrimitivePropertyValue ||
                                this.Kind == AddressableElementKind.EntityReference;
                    }

                    throw new Exception("Unknown method " + method);
                }

                public override string ToString()
                {
                    return this.Kind.ToString();
                }
            }

            [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdateTargetTests()
            {
                // Matrix of changes that can be made.
                TestUtil.RunCombinations(
                    new string[] { "GET", "PUT", "POST", "DELETE" },
                    SerializationFormatData.Values,
                    AddressableElementData.Values,
                    ServiceModelData.ValidValues,
                    (method, format, element, model) =>
                {
                    // This test case overlaps with other tests for regular resources; we do however
                    // focus on differents methods for the system resources.
                    if (!element.SystemGenerated)
                    {
                        return;
                    }

                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        bool reading = method == "GET";
                        bool hasBody = !reading && method != "DELETE";
                        request.DataServiceType = model.ServiceModelType;
                        request.HttpMethod = method;
                        request.RequestUriString = element.BuildRequestUri(model);
                        if (request.RequestUriString == null)
                        {
                            return;
                        }

                        if (hasBody)
                        {
                            request.RequestContentType = format.MimeTypes[0];
                            request.RequestStream = element.BuildRequestBody(method, format, model, true, true);
                            if (request.RequestStream == null)
                            {
                                return;
                            }

                            Trace.WriteLine("RequestStream:");
                            request.RequestStream = TestUtil.TraceStream(request.RequestStream);
                        }

                        Trace.WriteLine("Sending request to \"" + request.RequestUriString + "\".");
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        if (reading)
                        {
                            TestUtil.AssertExceptionExpected(exception, !element.Addressable);
                        }
                        else
                        {
                            TestUtil.AssertExceptionExpected(exception,
                                !element.Addressable,
                                !element.SupportsMethod(method),
                                hasBody && element.IsPrimitive != format.IsPrimitive);
                        }
                    }
                });
            }

            private class PutContentTypePayload
            {
                public string[] ContentTypes { get; set; }
                public string Payload { get; set; }
                public AddressableElementKind[] ValidKinds { get; set; }
                public string Description { get; set; }
                public Func<AddressableElementKind, string, bool> SkipWhen { get; set; }
                public Func<AddressableElementKind, string, int> ExpectedStatusCode { get; set; }

                public override string ToString()
                {
                    return this.Payload;
                }
            }

            private class PutContentTypeTarget
            {
                public string Url { get; set; }
                public AddressableElementKind Kind { get; set; }
                public string[] ValidContentTypes { get; set; }

                public override string ToString()
                {
                    return this.Url;
                }
            }

            public class PutContentTypeContext : CustomRowBasedOpenTypesContext
            {
                public override void SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    RowComplexType rowComplexType = targetResource as RowComplexType;
                    if (rowComplexType != null && IsOrderInstance(rowComplexType))
                    {
                        rowComplexType.Properties[propertyName] = propertyValue;
                        return;
                    }

                    base.SetValue(targetResource, propertyName, propertyValue);
                }
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePutContentTypeTest()
            {
                var targets = new[]
                {
                    // Declared property
                    new PutContentTypeTarget
                    {
                        Url = "/Orders(0)/OrderName",
                        Kind = AddressableElementKind.EntityPrimitiveProperty,
                        ValidContentTypes = new [] { UnitTestsUtil.MimeApplicationXml, UnitTestsUtil.MimeTextXml, UnitTestsUtil.JsonLightMimeType },
                    },
                    // Open property
                    new PutContentTypeTarget
                    {
                        Url = "/Orders(0)/OpenProperty",
                        Kind = AddressableElementKind.EntityPrimitiveOpenProperty,
                        ValidContentTypes = new [] { UnitTestsUtil.MimeApplicationXml, UnitTestsUtil.MimeTextXml, UnitTestsUtil.JsonLightMimeType },
                    },
                    // Declared property value
                    new PutContentTypeTarget
                    {
                        Url = "/Orders(0)/OrderName/$value",
                        Kind = AddressableElementKind.EntityPrimitivePropertyValue,
                        ValidContentTypes = new [] { UnitTestsUtil.MimeTextPlain },
                    },
                    // Open property
                    new PutContentTypeTarget
                    {
                        Url = "/Orders(0)/OpenProperty/$value",
                        Kind = AddressableElementKind.EntityPrimitiveOpenPropertyValue,
                        ValidContentTypes = new [] { UnitTestsUtil.MimeTextPlain },
                    },
                    // Entity
                    new PutContentTypeTarget
                    {
                        Url = "/Orders(0)",
                        Kind = AddressableElementKind.Entity,
                        ValidContentTypes = new [] { UnitTestsUtil.AtomFormat, "application/atom+xml;type=entry", UnitTestsUtil.MimeAny, UnitTestsUtil.JsonLightMimeType },
                    },
                    // Singleton $ref
                    new PutContentTypeTarget
                    {
                        Url = "/Orders(0)/Customer/$ref",
                        Kind = AddressableElementKind.EntityReference,
                        ValidContentTypes = new [] { UnitTestsUtil.MimeApplicationXml, UnitTestsUtil.MimeTextXml, UnitTestsUtil.JsonLightMimeType },
                    },
                };

                var payloads = new[]
                {
                    new PutContentTypePayload
                    {
                        Description = "JSON Single property OrderName",
                        ContentTypes = new [] { UnitTestsUtil.JsonLightMimeType },
                        Payload = "{ value: 'Foo' }",
                        ValidKinds = new [] {
                            AddressableElementKind.EntityPrimitiveOpenProperty,
                            AddressableElementKind.EntityPrimitiveProperty, 
                            // Note that in JSON there's no way to distiguish between the property and entry payloads - so this is a valid entry payload as well
                            AddressableElementKind.Entity },
                    },
                    new PutContentTypePayload
                    {
                        Description = "XML Single property OrderName",
                        ContentTypes = new [] { UnitTestsUtil.MimeApplicationXml, UnitTestsUtil.MimeTextXml },
                        Payload = "<ads:OrderName xmlns:ads='http://docs.oasis-open.org/odata/ns/data'>Foo</ads:OrderName>",
                        ValidKinds = new [] {
                            AddressableElementKind.EntityPrimitiveProperty, 
                            // Server ignores the property name for open properties - so this payload is valid for the open property as well.
                            AddressableElementKind.EntityPrimitiveOpenProperty }
                    },
                    new PutContentTypePayload
                    {
                        Description = "JSON Single property OpenProperty",
                        ContentTypes = new [] { UnitTestsUtil.JsonLightMimeType },
                        Payload = "{ value: 'Foo' }",
                        ValidKinds = new [] {
                            AddressableElementKind.EntityPrimitiveProperty,
                            AddressableElementKind.EntityPrimitiveOpenProperty, 
                            // Note that in JSON there's no way to distiguish between the property and entry payloads - so this is a valid entry payload as well
                            AddressableElementKind.Entity },
                    },
                    new PutContentTypePayload
                    {
                        Description = "Raw string value",
                        ContentTypes = new [] { UnitTestsUtil.MimeTextPlain },
                        Payload = "some value",
                        ValidKinds = new [] { AddressableElementKind.EntityPrimitivePropertyValue, AddressableElementKind.EntityPrimitiveOpenPropertyValue }
                    },
                    new PutContentTypePayload
                    {
                        Description = "ATOM Entity instance with type",
                        ContentTypes = new [] {
                            UnitTestsUtil.AtomFormat,
                            "application/atom+xml;type=entry",
                            "application/atom+xml;type=feed",
                            "application/atom+xml;type=test",
                            // Server treats */* as ATOM content type
                            UnitTestsUtil.MimeAny },
                        Payload = "<entry " +
                                    "xmlns:ads='http://docs.oasis-open.org/odata/ns/data' " +
                                    "xmlns:adsm='http://docs.oasis-open.org/odata/ns/metadata' " +
                                    "xmlns='http://www.w3.org/2005/Atom'>\r\n" +
                                    "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='#AstoriaUnitTests.Stubs.Order'/>" +
                                  "</entry>",
                        ValidKinds = new [] { AddressableElementKind.Entity },
                        ExpectedStatusCode = (kind, contentType) =>
                            {
                                // We need to accept wrong type parameter as long as the mime type is correct.
                                if (kind == AddressableElementKind.Entity && (contentType == "application/atom+xml;type=feed" || contentType == "application/atom+xml;type=test"))
                                {
                                    return 200;
                                }

                                return 0;
                            },
                    },
                    new PutContentTypePayload
                    {
                        Description = "JSON Entity instance with type",
                        ContentTypes = new [] { UnitTestsUtil.JsonLightMimeType },
                        Payload = "{ @odata.type: 'AstoriaUnitTests.Stubs.Order' }",
                        ValidKinds = new [] { AddressableElementKind.Entity },
                    },
                    new PutContentTypePayload
                    {
                        Description = "ATOM Entity instance without type",
                        ContentTypes = new [] {
                            UnitTestsUtil.AtomFormat,
                            "application/atom+xml;type=entry",
                            "application/atom+xml;type=feed",
                            "application/atom+xml;type=test",
                            // Server treats */* as ATOM content type
                            UnitTestsUtil.MimeAny },
                        Payload = "<entry " +
                                    "xmlns:ads='http://docs.oasis-open.org/odata/ns/data' " +
                                    "xmlns:adsm='http://docs.oasis-open.org/odata/ns/metadata' " +
                                    "xmlns='http://www.w3.org/2005/Atom'>\r\n" +
                                  "</entry>",
                        ValidKinds = new [] { AddressableElementKind.Entity },
                        ExpectedStatusCode = (kind, contentType) =>
                            {
                                // We need to accept wrong type parameter as long as the mime type is correct.
                                if (kind == AddressableElementKind.Entity && (contentType == "application/atom+xml;type=feed" || contentType == "application/atom+xml;type=test"))
                                {
                                    return 200;
                                }

                                return 0;
                            },
                    },
                    new PutContentTypePayload
                    {
                        Description = "JSON Entity instance without type",
                        ContentTypes = new [] { UnitTestsUtil.JsonLightMimeType },
                        Payload = "{ }",
                        ValidKinds = new [] { AddressableElementKind.Entity },
                    },
                    new PutContentTypePayload
                    {
                        Description = "ATOM Feed",
                        ContentTypes = new [] { UnitTestsUtil.AtomFormat, "application/atom+xml;type=feed", 
                            // Server treats */* as ATOM content type
                            UnitTestsUtil.MimeAny },
                        Payload = "<feed " +
                                    "xmlns:ads='http://docs.oasis-open.org/odata/ns/data' " +
                                    "xmlns:adsm='http://docs.oasis-open.org/odata/ns/metadata' " +
                                    "xmlns='http://www.w3.org/2005/Atom'>\r\n" +
                                  "</feed>",
                        ValidKinds = new AddressableElementKind[] { },
                        ExpectedStatusCode = (kind, contentType) =>
                            {
                                if (kind == AddressableElementKind.Entity && contentType != "application/atom+xml;type=feed")
                                {
                                    // Sending an ATOM payload to an entity end point is valid, but it will fail due to the root element called "feed".
                                    return 400;
                                }

                                if (kind == AddressableElementKind.Entity && contentType == "application/atom+xml;type=feed")
                                {
                                    // Server ignores the type=feed in the content type if it's ATOM content type.
                                    // but this will fail due to the root element called "feed"
                                    return 400;
                                }

                                return 0;
                            },
                    },
                    new PutContentTypePayload
                    {
                        Description = "JSON feed",
                        ContentTypes = new [] { UnitTestsUtil.JsonLightMimeType },
                        Payload = "[]",
                        ValidKinds = new AddressableElementKind[] { },
                    },
                    new PutContentTypePayload
                    {
                        Description = "Empty payload with unrecognized content type.",
                        ContentTypes = new [] { "mime/type", "application/foo", "test/foo" },
                        Payload = "",
                        ValidKinds = new AddressableElementKind[] { },
                    },
                };

                string[] contentTypes = payloads.SelectMany(p => p.ContentTypes).Distinct().ToArray();

                // Tests various content types for various possible PUT targets
                TestUtil.RunCombinations(
                    targets,
                    contentTypes,
                    (target, contentType) =>
                    {
                        TestUtil.RunCombinations(
                            payloads.Where(payload => payload.ContentTypes.Contains(contentType)),
                            payload =>
                            {
                                using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomRowBasedOpenTypesContext), "CustomizeMetadata"))
                                using (CustomDataContext.CreateChangeScope())
                                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                                {
                                    if (payload.SkipWhen != null && payload.SkipWhen(target.Kind, contentType))
                                    {
                                        return;
                                    }

                                    CustomRowBasedOpenTypesContext.CustomizeMetadata = (resourceSets, types, operations, associationSets) =>
                                        {
                                            provider.ResourceType order = types.Where(t => t.Name == "Order").First();
                                            provider.ResourceProperty orderNameProperty = new provider.ResourceProperty("OrderName", provider.ResourcePropertyKind.Primitive, provider.ResourceType.GetPrimitiveResourceType(typeof(string)));
                                            orderNameProperty.CanReflectOnInstanceTypeProperty = false;
                                            order.AddProperty(orderNameProperty);
                                        };

                                    request.DataServiceType = typeof(PutContentTypeContext);
                                    request.HttpMethod = "PUT";
                                    request.RequestUriString = target.Url;
                                    request.RequestContentType = contentType;
                                    request.RequestStream = new MemoryStream(Encoding.UTF8.GetBytes(payload.Payload));
                                    TestUtil.RunCatching(request.SendRequest);
                                    Trace.WriteLine(payload.Description + ", Content-Type: " + contentType + ", Url: " + target.Url + ", Response Status: " + request.ResponseStatusCode.ToString());
                                    Trace.WriteLine(request.GetResponseStreamAsText());
                                    int expectedStatusCode = 0;
                                    if (payload.ExpectedStatusCode != null)
                                    {
                                        expectedStatusCode = payload.ExpectedStatusCode(target.Kind, contentType);
                                    }

                                    if (expectedStatusCode != 0)
                                    {
                                        if (expectedStatusCode == 200)
                                        {
                                            Assert.IsTrue(request.ResponseStatusCode == 200 || request.ResponseStatusCode == 204, "Should have succeeded.");
                                        }
                                        else
                                        {
                                            Assert.AreEqual(expectedStatusCode, request.ResponseStatusCode, "Unexpected status code.");
                                        }
                                    }
                                    else
                                    {
                                        if (target.ValidContentTypes.Contains(contentType))
                                        {
                                            if (payload.ValidKinds.Contains(target.Kind))
                                            {
                                                Assert.IsTrue(request.ResponseStatusCode == 200 || request.ResponseStatusCode == 204, "Should have succeeded.");
                                            }
                                            else
                                            {
                                                Assert.AreEqual(400, request.ResponseStatusCode, "Should have failed to process the payload.");
                                            }
                                        }
                                        else
                                        {
                                            Assert.AreEqual(415, request.ResponseStatusCode, "Should not be recognized as valid content type.");
                                        }
                                    }
                                }
                            });
                    });
            }

            [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdatePutResourceSetPropertyUpdate()
            {
                string jsonPayload = "[ " +
                                    "{" +
                                        "@odata.type:\"#AstoriaUnitTests.Stubs.Order\"," +
                                        "DollarAmount: 5555" +
                                    "}," +
                                    "{" +
                                        "@odata.type:\"#AstoriaUnitTests.Stubs.Order\"," +
                                        "DollarAmount: 6767" +
                                    "}" +
                                   "]";

                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<feed xml:base=\"/\" xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://www.w3.org/2005/Atom\">" +
                        "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                            AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Order).FullName) +
                            "<id>/Orders(0)</id>" +
                            "<content type=\"application/xml\"><adsm:properties>" +
                                "<ads:DollarAmount>5555</ads:DollarAmount>" +
                            "</adsm:properties></content>" +
                        "</entry>" +
                        "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                            AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Order).FullName) +
                            "<id>/Orders(100)</id>" +
                            "<content type=\"application/xml\"><adsm:properties>" +
                                "<ads:DollarAmount>6767</ads:DollarAmount>" +
                            "</adsm:properties></content>" +
                        "</entry>" +
                    "</feed>";

                VerifyInvalidRequestForVariousProviders(jsonPayload, "/Customers(1)/BestFriend/Orders", UnitTestsUtil.JsonLightMimeType, "PUT", 405);
                VerifyInvalidRequestForVariousProviders(atomPayload, "/Customers(1)/BestFriend/Orders", UnitTestsUtil.AtomFormat, "PUT", 405);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdateMergeReferencePropertyUpdate()
            {
                PayloadBuilder payloadBuilder = new PayloadBuilder()
                {
                    TypeName = typeof(Customer).FullName,
                    Uri = "/Customers(0)"
                }
                .AddProperty("Name", "Foo");

                var atomUriAndXPath1 = new KeyValuePair<string, string[]>(
                    "/Customers(1)/BestFriend",
                    new string[] { "/atom:entry[atom:category/@term='#" + typeof(Customer).FullName + "' and atom:id='http://host/Customers(0)' and atom:content/adsm:properties[ads:ID='0' and ads:Name='Foo']]" });

                var atomUriAndXPath2 = new KeyValuePair<string, string[]>(
                    "/Customers(0)",
                    new string[] { "/atom:entry[atom:category/@term='#" + typeof(Customer).FullName + "' and atom:id='http://host/Customers(0)' and atom:content/adsm:properties[ads:ID='0' and ads:Name='Foo']]" });

                var jsonLiteUriAndXPath1 = new KeyValuePair<string, string[]>(
                    "/Customers(1)/BestFriend",
                    new string[] { String.Format("/{0}[ID=0 and Name='Foo']", JsonValidator.ObjectString) });

                var jsonLiteUriAndXPath2 = new KeyValuePair<string, string[]>("/Customers(0)", jsonLiteUriAndXPath1.Value);

                DoUpdatesForVariousProviders("PATCH", "/Customers(1)/BestFriend", UnitTestsUtil.AtomFormat, payloadBuilder, new KeyValuePair<string, string[]>[] { atomUriAndXPath1, atomUriAndXPath2 }, true, true /*verifyUpdateResponse*/);
                DoUpdatesForVariousProviders("PATCH", "/Customers(1)/BestFriend", UnitTestsUtil.JsonLightMimeType, payloadBuilder, new KeyValuePair<string, string[]>[] { jsonLiteUriAndXPath1, jsonLiteUriAndXPath2 }, true, true /*verifyUpdateResponse*/);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod]
            public void UpdatRequiresNoTypeExplicitlyFromIDSP()
            {
                using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomDataServiceProvider), "InvocationTraceCallback"))
                using (CustomRowBasedContext.CreateChangeScope())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    StringBuilder log = new StringBuilder();
                    CustomDataServiceProvider.InvocationTraceCallback = (text) => { log.AppendLine(text); };
                    request.DataServiceType = typeof(CustomRowBasedContext);
                    request.RequestUriString = "/Customers";
                    request.RequestContentType = UnitTestsUtil.AtomFormat;
                    request.SetRequestStreamAsText(@"<entry xml:base='http://host/' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
    <category term='#AstoriaUnitTests.Stubs.Customer' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
      <m:properties>
        <d:ID m:type='Edm.Int32'>1000</d:ID>
        <d:Name>Customer 1000</d:Name>
        <d:Address m:type='AstoriaUnitTests.Stubs.Address'>
          <d:StreetAddress>Line1</d:StreetAddress>
          <d:City>Redmond</d:City>
          <d:State>WA</d:State>
          <d:PostalCode>98052</d:PostalCode>
        </d:Address>
      </m:properties>
    </content>
  </entry>");
                    request.HttpMethod = "POST";
                    request.SendRequest();
                    request.GetResponseStreamAsText();

                    string callLog = log.ToString();
                    TestUtil.AssertContains(callLog, "TryResolveResourceSet");
                    TestUtil.AssertContainsFalse(callLog, "TryResolveResourceType");
                }
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation("setting a reference property to null via a uri that refers to null should work")]
            public void UpdateSetReferencePropertyToNullViaAValidUri()
            {
                string uri = "/Customers(1)";
                string instanceTypeName = typeof(CustomerWithBirthday).FullName;
                PayloadBuilder payloadBuilder = new PayloadBuilder() { TypeName = instanceTypeName }
                    .AddProperty("BestFriend", new PayloadBuilder() { Uri = "/Customers(0)/BestFriend" });

                var atomUriAndXPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>("/Customers(1)", new string[] { "/atom:entry[atom:category/@term='#" + instanceTypeName + "' and atom:id='http://host/Customers(1)']" }),
                    new KeyValuePair<string, string[]>("/Customers(1)?$expand=BestFriend",
                        new string[] { String.Format("/atom:entry/atom:link[@href='Customers(1)/{0}/BestFriend' and adsm:inline[string()='']]", instanceTypeName) } ) };

                var jsonLiteUriAndXPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>("/Customers(1)?$expand=BestFriend",
                        new string[] { String.Format("/{0}/BestFriend[@IsNull='true']", JsonValidator.ObjectString) })};

                DoUpdatesForVariousProviders("PATCH", uri, UnitTestsUtil.AtomFormat, payloadBuilder, atomUriAndXPaths, true, true /*verifyUpdateResponse*/);
                DoUpdatesForVariousProviders("PATCH", uri, UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteUriAndXPaths, true, true /*verifyUpdateResponse*/);
            }

            internal static void DoUpdatesForVariousProviders(string method, string uri, string responseFormat, PayloadBuilder payloadBuilder, string[] xPaths, bool verifyETag)
            {
                DoUpdatesForVariousProviders(method, uri, responseFormat, payloadBuilder, new KeyValuePair<string, string[]>[] { new KeyValuePair<string, string[]>(uri, xPaths) }, verifyETag);
            }

            internal static void DoUpdatesForVariousProvidersWithOpenMissing(string method, string uri, string responseFormat, PayloadBuilder payloadBuilder, string[] xPaths, bool verifyETag, bool verifyResponsePreference = true)
            {
                KeyValuePair<string, string[]>[] uriAndXPathsToVerify = new KeyValuePair<string, string[]>[] { new KeyValuePair<string, string[]>(uri, xPaths) };
                DoUpdatesForVariousProvidersWithOpenMissing(method, uri, responseFormat, payloadBuilder, uriAndXPathsToVerify, verifyETag, verifyResponsePreference);
            }

            internal static void DoUpdatesForVariousProvidersWithOpenMissing(string method, string uri, string responseFormat, PayloadBuilder payloadBuilder, KeyValuePair<string, string[]>[] uriAndXPathsToVerify, bool verifyETag, bool verifyResponsePreference = true)
            {
                DoUpdatesForVariousProviders(
                    method,
                    uri,
                    responseFormat,
                    payloadBuilder,
                    uriAndXPathsToVerify,
                    verifyETag,
                    includeOpenTypesProvider: false,
                    verifyResponsePreference: verifyResponsePreference);
            }

            internal static void DoUpdatesForVariousProviders(
                string method,
                string uri,
                string responseFormat,
                PayloadBuilder payloadBuilder,
                KeyValuePair<string, string[]>[] uriAndXPathsToVerify,
                bool verifyETag,
                bool includeOpenTypesProvider = true,
                bool verifyResponsePreference = true)
            {
                var providerTypes = UnitTestsUtil.ProviderTypes.Where(providerType => includeOpenTypesProvider || providerType != typeof (CustomRowBasedOpenTypesContext));
                DoUpdatesForVariousProviders(providerTypes, method, uri, responseFormat, payloadBuilder, uriAndXPathsToVerify, verifyETag, includeOpenTypesProvider, verifyResponsePreference);
            }

            internal static void DoUpdatesForVariousProviders(
               string method,
               string uri,
               string responseFormat,
               string payload,
               KeyValuePair<string, string[]>[] uriAndXPathsToVerify,
               bool verifyETag,
               bool includeOpenTypesProvider = true,
               bool verifyResponsePreference = true)
            {
                TestUtil.RunCombinations(
                        UnitTestsUtil.ProviderTypes.Where(providerType => includeOpenTypesProvider || providerType != typeof(CustomRowBasedOpenTypesContext)),
                        (providerType) =>
                        {
                            DoUpdatesForVariousProviders(
                                providerType,
                                method,
                                uri,
                                responseFormat,
                                payload,
                                uriAndXPathsToVerify,
                                verifyETag,
                                includeOpenTypesProvider,
                                verifyResponsePreference);
                        });
            }

            internal static void DoUpdatesForVariousProviders(
                IEnumerable<Type> providerTypes,
                string method,
                string uri,
                string responseFormat,
                PayloadBuilder payloadBuilder,
                KeyValuePair<string, string[]>[] uriAndXPathsToVerify,
                bool verifyETag,
                bool includeOpenTypesProvider = true,
                bool verifyResponsePreference = true)
            {
                TestUtil.RunCombinations(
                        providerTypes,
                        (providerType) =>
                        {
                            using (UnitTestsUtil.AppendTypesForOpenProperties(providerType, payloadBuilder, responseFormat))
                            {
                                DoUpdatesForVariousProviders(
                                    providerType,
                                    method,
                                    uri,
                                    responseFormat,
                                    PayloadGenerator.Generate(payloadBuilder, responseFormat),
                                    uriAndXPathsToVerify,
                                    verifyETag,
                                    includeOpenTypesProvider,
                                    verifyResponsePreference);
                            }
                        });
            }

            private static void DoUpdatesForVariousProviders(
               Type providerType,
               string method,
               string uri,
               string responseFormat,
               string payload,
               KeyValuePair<string, string[]>[] uriAndXPathsToVerify,
               bool verifyETag,
               bool includeOpenTypesProvider = true,
               bool verifyResponsePreference = true)
            {
                Debug.Assert(method != null, "method != null");
                Debug.Assert(uri != null, "uri != null");

                using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                {
                    // Set max protocol version to V3 to allow PATCH requests
                    OpenWebDataServiceHelper.MaxProtocolVersion.Value = ODataProtocolVersion.V4;

                    TestUtil.RunCombinations(
                        verifyResponsePreference ? new bool?[] { true, false, null } : new bool?[] { null },
                        new string[] { method },
                        new UnitTestsUtil.SendRequestModifier[] { UnitTestsUtil.SendRequestModifier.None, UnitTestsUtil.SendRequestModifier.UseBatchRequest },
                        (responsePreference, httpMethod, sendRequestModifier) =>
                    {
                        if (responsePreference.HasValue || (String.Equals(responseFormat, UnitTestsUtil.JsonLightMimeType, StringComparison.OrdinalIgnoreCase) && !uri.Contains("$ref")))
                        {
                            if (responsePreference.HasValue && responsePreference.Value)
                            {
                                CustomProviderRequest(
                                    providerType,
                                    uri,
                                    responseFormat,
                                    payload,
                                    uriAndXPathsToVerify,
                                    httpMethod,
                                    verifyETag,
                                    responseHeaders: GetResponseHeadersToVerifyForUpdates(responsePreference, responseFormat, method, uri),
                                    requestHeaders: new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Prefer", "return=representation") },
                                    sendRequestModifier: sendRequestModifier);
                            }
                            else
                            {
                                CustomProviderRequest(
                                    providerType,
                                    uri,
                                    responseFormat,
                                    payload,
                                    uriAndXPathsToVerify,
                                    httpMethod,
                                    verifyETag,
                                    responseHeaders: GetResponseHeadersToVerifyForUpdates(responsePreference, responseFormat, method, uri),
                                    requestHeaders: new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Prefer", "return=minimal") },
                                    sendRequestModifier: sendRequestModifier);
                            }
                        }
                        else
                        {
                            using (TestUtil.MetadataCacheCleaner())
                            {
                                var version = "4.0";

                                var requestHeaders = new List<KeyValuePair<string, string>>();
                                requestHeaders.Add(new KeyValuePair<string, string>("OData-MaxVersion", version));

                                CustomProviderRequest(
                                    providerType,
                                    uri,
                                    responseFormat,
                                    payload,
                                    uriAndXPathsToVerify,
                                    method,
                                    verifyETag,
                                    requestHeaders,
                                    GetResponseHeadersToVerifyForUpdates(responsePreference, responseFormat, method, uri),
                                    sendRequestModifier: sendRequestModifier);
                            }
                        }
                    });
                }
            }

            internal static void DoPrimitiveValueUpdatesForVariousProvider(string payload, string uri, string[] xPaths, string contentType, int requestLength = 0)
            {
                foreach (Type providerType in UnitTestsUtil.ProviderTypes)
                {
                    DoPrimitiveValueUpdatesForProvider(payload, uri, xPaths, providerType, contentType, requestLength);
                }
            }

            internal static void DoPrimitiveValueUpdatesForProvider(string payload, string uri, string[] xPaths, Type providerType, string contentType, int requestLength = 0, string etagUri = null)
            {
                using (UnitTestsUtil.CreateChangeScope(providerType))
                {
                    string newUri = UnitTestsUtil.ConvertUri(providerType, uri);
                    KeyValuePair<string, string>[] headerValues = null;

                    string newETagUri = etagUri == null ? newUri : UnitTestsUtil.ConvertUri(providerType, etagUri);

                    // get etag value
                    string etag = UnitTestsUtil.GetETagFromResponse(providerType, newETagUri, contentType);
                    if (etag != null)
                    {
                        headerValues = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-Match", etag) };
                    }

                    UnitTestsUtil.UpdateAndVerifyPrimitiveProperty(providerType, "PUT", newUri, payload, contentType, headerValues);
                }
            }

            private static IList<KeyValuePair<string, string>> GetResponseHeadersToVerifyForUpdates(bool? responsePreference, string responseFormat, string method, string uri)
            {
                string version = "4.0;";

                var responseHeaders = new List<KeyValuePair<string, string>>();
                bool responseReturned = ResponseWillBeSent(method, responsePreference);
                responseHeaders.Add(new KeyValuePair<string, string>("StatusCode", responseReturned ? "200" : "204"));
                responseHeaders.Add(new KeyValuePair<string, string>("ContentType", responseReturned ? responseFormat : null));
                responseHeaders.Add(new KeyValuePair<string, string>("Version", version));
                if (responsePreference.HasValue)
                {
                    responseHeaders.Add(new KeyValuePair<string, string>("Preference-Applied", responsePreference.Value ? "return=representation" : "return=minimal"));
                }

                return responseHeaders;
            }

            private static bool ResponseWillBeSent(string method, bool? responsePreference)
            {
                return
                    ((method == "POST" && (!responsePreference.HasValue && responsePreference.Value)) ||
                     ((method == "PUT" || method == "PATCH") && responsePreference.HasValue && responsePreference.Value));
            }

            internal static void DoPrimitiveValueUpdates(string payload, string uri, string[] xPaths, Type contextType, string contentType, int requestContentLength)
            {
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.DataServiceType = contextType;
                    request.RequestUriString = uri;
                    request.HttpMethod = "PUT";
                    request.RequestStream = new MemoryStream();
                    request.RequestContentType = contentType;
                    request.RequestContentLength = requestContentLength;
                    StreamWriter writer = new StreamWriter(request.RequestStream);
                    writer.Write(payload);
                    writer.Flush();

                    request.SendRequest();
                }

                UnitTestsUtil.VerifyPrimitiveValue(WebServerLocation.InProcess, contentType, uri, contextType, payload);
            }
            #endregion Update/Put tests

            #region Update/Replace Tests
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod, Variation]
            public void UpdateReplaceTopLevelResourceDirectly()
            {
                string uri = "/Customers(1)";

                #region Payload and XPath
                string jsonOCSPayload = "{" +
                                        "@odata.type:\"#AstoriaUnitTests.Stubs.CustomerWithBirthday\", " +
                                        "\"EditTimeStamp\": \"AAAAAAAADDg=\"" +
                                     "}";

                string jsonPayload = "{" +
                                        "@odata.type:\"#AstoriaUnitTests.Stubs.CustomerWithBirthday\", " +
                                        "GuidValue: \"c1f04664-72fb-4e45-aff2-397a2801ca2a\"" +
                                     "}";

                var jsonXPath = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(uri,
                        new string[] { String.Format("/{0}[odata.type='#{1}' and ID=1 and Name/@IsNull='true']",
                                        JsonValidator.ObjectString,
                                        typeof(CustomerWithBirthday).FullName) })
                             };

                var jsonOpenTypesXPath = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(uri,
                        new string[] { String.Format("/{0}[odata.type='#{1}' and ID=1]",
                                        JsonValidator.ObjectString,
                                        typeof(CustomerWithBirthday).FullName),
                                       String.Format("count(/{0}/Name)=0", JsonValidator.ObjectString) })
                             };

                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                        AtomUpdatePayloadBuilder.GetCategoryXml(typeof(CustomerWithBirthday).FullName) +
                        "<content type=\"application/xml\">" +
                            "<adsm:properties>" +
                                "<ads:GuidValue adsm:type=\"Edm.Guid\">c1f04664-72fb-4e45-aff2-397a2801ca2a</ads:GuidValue>" +
                            "</adsm:properties>" +
                        "</content>" +
                    "</entry>";

                string atomOCSPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                        AtomUpdatePayloadBuilder.GetCategoryXml(typeof(CustomerWithBirthday).FullName) +
                        "<content type=\"application/xml\">" +
                            "<adsm:properties>" +
                                "<ads:EditTimeStamp adsm:type=\"Edm.Binary\">AAAAAAAACCA=</ads:EditTimeStamp>" +
                            "</adsm:properties>" +
                        "</content>" +
                     "</entry>";

                var atomXPath = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(uri,
                        new string[] { "/atom:entry[atom:category/@term='#" + typeof(CustomerWithBirthday).FullName + "' and atom:id='http://host/Customers(1)' and atom:content/adsm:properties[ads:ID='1' and ads:Name/@adsm:null='true']]"})
                };

                var atomOpenTypesXPath = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(uri,
                        new string[] { "/atom:entry[atom:category/@term='#" + typeof(CustomerWithBirthday).FullName + "' and atom:id='http://host/Customers(1)' and atom:content/adsm:properties/ads:ID='1']",
                                       "count(/atom:entry/atom:content/adsm:Properties/ads:Name)=0" })
                };
                #endregion //Payload and XPath

                TestUtil.RunCombinations(UnitTestsUtil.ProviderTypes, (providerType) =>
                {
                    var newJsonPayload = jsonPayload;
                    var newAtomPayload = atomPayload;
                    var newJsonXPath = jsonXPath;
                    var newAtomXPath = atomXPath;

                    if (providerType == typeof(ocs.CustomObjectContext) ||
                        providerType == typeof(efpoco.CustomObjectContextPOCOProxy) ||
                        providerType == typeof(efpoco.CustomObjectContextPOCO))
                    {
                        newJsonPayload = jsonOCSPayload;
                        newAtomPayload = atomOCSPayload;
                    }
                    else if (providerType == typeof(CustomRowBasedOpenTypesContext))
                    {
                        newJsonXPath = jsonOpenTypesXPath;
                        newAtomXPath = atomOpenTypesXPath;
                    }

                    CustomProviderRequest(providerType, uri, UnitTestsUtil.JsonLightMimeType, newJsonPayload, newJsonXPath, "PUT", true);
                    CustomProviderRequest(providerType, uri, UnitTestsUtil.AtomFormat, newAtomPayload, newAtomXPath, "PUT", true);
                });
            }

            #endregion // Update/Replace Tests

            #region IConcurrencyProvider tests
            public class ReflectionProvider : CustomDataContext, Microsoft.OData.Service.Providers.IDataServiceUpdateProvider
            {
                public static int NumOfTimesIConcurrencyProviderInvoked = 0;

                public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> etagValues)
                {
                    Assert.IsTrue(resourceCookie != null, "resourceCookie != null");
                    Assert.IsTrue(etagValues != null, "etagValues can never be null");

                    if (checkForEquality == null)
                    {
                        throw new DataServiceException(400, "Missing If-Match header");
                    }

                    Assert.IsTrue(checkForEquality.Value, "checkForEquality must be always true");
                    NumOfTimesIConcurrencyProviderInvoked++;
                    object resource = this.tokens[(int)resourceCookie].Resource;
                    Assert.IsTrue(resource != null, "resource != null");

                    // There must be etag attribute defined on the type whenever this method is called.
                    ETagAttribute etagAttribute = (ETagAttribute)resource.GetType().GetCustomAttributes(typeof(ETagAttribute), true).Single();
                    foreach (var etag in etagValues)
                    {
                        object propertyValue = resource.GetType().GetProperty(etag.Key, BindingFlags.Public | BindingFlags.Instance).GetValue(resource, null);
                        if (propertyValue == null && etag.Value == null)
                        {
                            continue;
                        }

                        // TODO: this check won't work for byte[].
                        if (propertyValue != null && propertyValue.Equals(etag.Value))
                        {
                            continue;
                        }

                        throw new DataServiceException(412, "ETag values do not match");
                    }
                }
            }

            [TestCategory("Partition1"), TestMethod, Variation("Make sure that the IConcurrencyProvider interface is invoked correctly")]
            public void IConcurrencyProviderTests()
            {
                string jsonPayload = "{" +
                                       "@odata.type:\"#AstoriaUnitTests.Stubs.CustomerWithBirthday\"," +
                                       "Name: \"Foo\"" +
                                    "}";

                string[] jsonXPath = new string[] {
                    String.Format("/{0}[odata.type='#{1}' and ID=1 and Name='Foo']",
                                    JsonValidator.ObjectString,
                                    typeof(CustomerWithBirthday).FullName),
                             };

                string uri = "/Customers(1)";
                Type contextType = typeof(ReflectionProvider);
                using (ReflectionProvider.CreateChangeScope())
                {
                    TestUtil.RunCombinations(new string[] { "PATCH" }, (httpMethod) =>
                    {
                        ReflectionProvider.NumOfTimesIConcurrencyProviderInvoked = 0;
                        string etag = UnitTestsUtil.GetETagFromResponse(contextType, uri, UnitTestsUtil.JsonLightMimeType);
                        KeyValuePair<string, string>[] headers = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-Match", etag) };

                        // Passing an valid etag
                        UnitTestsUtil.SendRequestAndVerifyXPath(
                            jsonPayload, "/Customers(1)",
                            new KeyValuePair<string, string[]>[] { new KeyValuePair<string, string[]>(uri, jsonXPath) },
                            typeof(ReflectionProvider), UnitTestsUtil.JsonLightMimeType,
                            httpMethod, headers, true);
                        Assert.IsTrue(ReflectionProvider.NumOfTimesIConcurrencyProviderInvoked == 1, "IConcurrencyProvider must be invoked exactly once - 1");

                        // Passing '*' as etag
                        ReflectionProvider.NumOfTimesIConcurrencyProviderInvoked = 0;
                        headers = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-Match", "*") };
                        UnitTestsUtil.SendRequestAndVerifyXPath(
                            jsonPayload, "/Customers(1)",
                            new KeyValuePair<string, string[]>[] { new KeyValuePair<string, string[]>(uri, jsonXPath) },
                            typeof(ReflectionProvider), UnitTestsUtil.JsonLightMimeType,
                            httpMethod, headers, true);
                        Assert.IsTrue(ReflectionProvider.NumOfTimesIConcurrencyProviderInvoked == 1, "IConcurrencyProvider must be invoked exactly once - 2");
                    });
                }
            }

            #endregion IConcurrencyProvider tests

            #region Prefer header

            internal class DSPSelfmodifyingResource : DSPResource
            {
                public DSPSelfmodifyingResource(provider.ResourceType resourceType) : base(resourceType) { }

                public override void SetValue(string propertyName, object value)
                {
                    if (propertyName == "TriggerModification")
                    {
                        object etagValue = this.GetValue("ETagProperty");
                        int etag = etagValue == null ? 0 : (int)etagValue;
                        this.SetValue("ETagProperty", etag + 1);
                    }

                    base.SetValue(propertyName, value);
                }
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod()]
            public void PreferHeader_Server()
            {
                DSPMetadata metadata = PreferHeader_CreateMetadata();

                TestUtil.RunCombinations(
                    new[] { typeof(IDataServiceHost), typeof(IDataServiceHost2) },
                    new[] { ODataProtocolVersion.V4 },
                    (hostInterfaceType, maxProtocolVersion) =>
                {
                    DSPSelfmodifyingResource existingItem = new DSPSelfmodifyingResource(metadata.GetResourceType("Item"));
                    existingItem.SetRawValue("ID", 0);
                    existingItem.SetRawValue("ETagProperty", 1);
                    DSPSelfmodifyingResource existingStream = new DSPSelfmodifyingResource(metadata.GetResourceType("Stream"));
                    existingStream.SetRawValue("ID", 0);
                    existingStream.SetRawValue("ETagProperty", 1);

                    bool supportsMR = hostInterfaceType != typeof(IDataServiceHost);
                    DSPServiceDefinition service = this.PreferHeader_CreateService(metadata, existingItem, null, existingStream, null);
                    service.HostInterfaceType = hostInterfaceType;
                    service.DataServiceBehavior.MaxProtocolVersion = maxProtocolVersion;

                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        XDocument existingItemAtom = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Items(0)", "application/atom+xml,application/xml");
                        XDocument existingStreamAtom = !supportsMR ? null : UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Streams(0)", "application/atom+xml,application/xml");

                        IEnumerable<string> httpMethods = new string[] { "POST", "POSTMR", "PUT", "PATCH" };

                        TestUtil.RunCombinations(
                            new string[] { null, "return=representation", "return=minimal", "ReTURN=RePrESenTaTioN", "foo" },
                            httpMethods,
                            new string[] { UnitTestsUtil.AtomFormat }, (preferHeader, httpMethod, format) =>
                            {
                                // Skip MR verification since we use MRs in that entity and as such the request would not work without support on the server
                                if (httpMethod == "POSTMR" && !supportsMR)
                                {
                                    return;
                                }

                                service.ClearChanges();

                                request.RequestVersion = "4.0;";
                                request.RequestMaxVersion = "4.0;";
                                request.Accept = "application/atom+xml,application/xml";
                                request.RequestHeaders["Prefer"] = preferHeader;

                                XDocument existingAtom = httpMethod == "POSTMR" ? existingStreamAtom : existingItemAtom;
                                DSPSelfmodifyingResource existingResource = httpMethod == "POSTMR" ? existingStream : existingItem;

                                this.PreferHeader_SetupRequest(request, httpMethod, format, existingAtom, existingResource, (inputAtom) =>
                                {
                                    var triggerModification = inputAtom.Root.Descendants(UnitTestsUtil.MetadataNamespace + "properties").First().Element(UnitTestsUtil.DataNamespace + "TriggerModification");
                                    triggerModification.Attributes(UnitTestsUtil.MetadataNamespace + "null").Remove();
                                    triggerModification.Value = "trigger" + existingItem.GetETag().Replace('"', '_');
                                });

                                bool serviceAllowsPreferHeader = hostInterfaceType == typeof(IDataServiceHost2);

                                bool? effectiveResponsePreference;
                                this.PreferHeader_ShouldContainResponseBody(request, serviceAllowsPreferHeader, out effectiveResponsePreference);

                                TestUtil.RunCatching(request.SendRequest);

                                DSPSelfmodifyingResource returnedItem;
                                if (httpMethod == "POST" || httpMethod == "POSTMR")
                                {
                                    returnedItem = service.CurrentDataSource.GetResourceSetEntities(httpMethod == "POSTMR" ? "Streams" : "Items").OfType<DSPSelfmodifyingResource>().First(r => (int)r.GetValue("ID") == 1);
                                }
                                else
                                {
                                    returnedItem = existingResource;
                                }

                                this.PreferHeader_VerifyResponse(request, request.ServiceRoot.ToString(), serviceAllowsPreferHeader, (response) =>
                                {
                                    if (httpMethod != "POSTMR") // with POST MR the ETagProperty will not be updated since the payload didn't set any properties (it's just the stream)
                                    {
                                        string etagPropertyValue = returnedItem.GetValue("ETagProperty").ToString();
                                        XElement etagPropertyElement = response.Root.Descendants(UnitTestsUtil.MetadataNamespace + "properties").First().Element(UnitTestsUtil.DataNamespace + "ETagProperty");
                                        Assert.AreEqual(etagPropertyValue, etagPropertyElement.Value, "The updated value was not included in the response.");
                                    }
                                });

                                Assert.AreEqual(returnedItem.GetETag(), request.ResponseETag, "The response ETag does not match the current ETag value on the server.");

                            });
                    }
                });
            }

            internal class PreferHeader_InterceptorsService : DSPDataService
            {
                private static Restorable<bool> interceptItems = new Restorable<bool>();
                public static Restorable<bool> InterceptItems { get { return interceptItems; } }

                public static UpdateOperations LastItemsUpdateOperation = UpdateOperations.None;

                public static bool ItemsQueryInterceptorInvoked = false;

                [QueryInterceptor("Items")]
                public Expression<Func<DSPSelfmodifyingResource, bool>> ItemsQueryInterceptor()
                {
                    ItemsQueryInterceptorInvoked = true;
                    if (InterceptItems.Value)
                    {
                        return (r) => false;
                    }
                    else
                    {
                        return (r) => true;
                    }
                }

                [ChangeInterceptor("Items")]
                public void ItemsChangeInterceptor(DSPSelfmodifyingResource resource, UpdateOperations operation)
                {
                    LastItemsUpdateOperation = operation;
                }
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod(), Variation("Verifies that Prefer header works as expected with ProcessingPipeline, Configuration, Interceptors and access rights.")]
            public void PreferHeader_CrossFeature()
            {
                DSPMetadata metadata = PreferHeader_CreateMetadata();
                DSPSelfmodifyingResource existingItem = new DSPSelfmodifyingResource(metadata.GetResourceType("Item"));
                existingItem.SetRawValue("ID", 0);
                existingItem.SetRawValue("ETagProperty", 1);
                DSPServiceDefinition service = PreferHeader_CreateService(metadata, existingItem, null, null, null);

                ProcessingPipelineCallCount callCount = new ProcessingPipelineCallCount();
                service.ProcessingPipeline.ProcessingRequest += (sender, args) => { callCount.ProcessingRequestCallCount++; };
                service.ProcessingPipeline.ProcessedRequest += (sender, args) => { callCount.ProcessedRequestCallCount++; };
                service.ProcessingPipeline.ProcessingChangeset += (sender, args) => { callCount.ProcessingChangesetCallCount++; };
                service.ProcessingPipeline.ProcessedChangeset += (sender, args) => { callCount.ProcessedChangesetCallCount++; };
                service.DataServiceType = typeof(PreferHeader_InterceptorsService);

                ProcessingPipelineCallCount expectedCallCount = new ProcessingPipelineCallCount()
                {
                    ProcessingRequestCallCount = 1,
                    ProcessedRequestCallCount = 1,
                    ProcessingChangesetCallCount = 1,
                    ProcessedChangesetCallCount = 1
                };

                // Can't get the payload from the service since we sometimes don't allow read rights
                XDocument existingItemAtom = XDocument.Parse("<entry xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>" +
                    "<id>http://host/Items(0)</id>" +
                    "<title type='text'></title>" +
                    "<updated>2010-07-19T14:20:32Z</updated>" +
                    "<author><name /></author>" +
                    "<category term='TestNS.Item' scheme='http://docs.oasis-open.org/odata/ns/scheme' />" +
                    "<content type='application/xml'><m:properties>" +
                        "<d:ID m:type='Edm.Int32'>0</d:ID>" +
                        "<d:ETagProperty m:type='Edm.Int32'>1</d:ETagProperty>" +
                    "</m:properties></content></entry>");

                TestUtil.RunCombinations(
                    UnitTestsUtil.BooleanValues,
                    UnitTestsUtil.BooleanValues,
                    (includeRelationshipLinks, allowReadAccess) =>
                {
                    service.DataServiceBehavior.IncludeRelationshipLinksInResponse = includeRelationshipLinks;
                    service.EntitySetAccessRule = new Dictionary<string, EntitySetRights>() {
                        {"Items", EntitySetRights.AllWrite | (allowReadAccess ? EntitySetRights.AllRead : EntitySetRights.None)}
                    };

                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        request.RegisterForDispose(PreferHeader_InterceptorsService.InterceptItems.Restore());

                        TestUtil.RunCombinations(
                            new string[] { "POST", "PUT", "PATCH" },
                            new string[] { null, "return=representation", "return=minimal" },
                            new string[] { UnitTestsUtil.AtomFormat },
                            UnitTestsUtil.BooleanValues,
                            UnitTestsUtil.BooleanValues,
                            (httpMethod, preferHeader, format, batch, interceptItems) =>
                        {
                            PreferHeader_InterceptorsService.InterceptItems.Value = interceptItems;
                            PreferHeader_InterceptorsService.LastItemsUpdateOperation = UpdateOperations.None;
                            PreferHeader_InterceptorsService.ItemsQueryInterceptorInvoked = false;
                            service.ClearChanges();
                            callCount.Clear();

                            TestWebRequest r = request;
                            if (batch)
                            {
                                r = new InMemoryWebRequest();
                            }

                            r.RequestVersion = "4.0;";
                            r.RequestMaxVersion = "4.0;";
                            r.RequestHeaders["Prefer"] = preferHeader;

                            PreferHeader_SetupRequest(r, httpMethod, format, existingItemAtom, existingItem, null);

                            if (batch)
                            {
                                var batchRequest = new BatchWebRequest();
                                var changeset = new BatchWebRequest.Changeset();
                                changeset.Parts.Add((InMemoryWebRequest)r);
                                batchRequest.Changesets.Add(changeset);
                                batchRequest.SendRequest(request);
                            }
                            else
                            {
                                TestUtil.RunCatching(request.SendRequest);
                            }

                            Assert.AreEqual(httpMethod != "POST", PreferHeader_InterceptorsService.ItemsQueryInterceptorInvoked, "The query interceptor fire or didn't fire when it should have.");

                            if (!allowReadAccess && httpMethod != "POST")
                            {
                                Assert.AreEqual(403, r.ResponseStatusCode, "The request should have failed due to access rights.");
                            }
                            else if (interceptItems && httpMethod != "POST")
                            {
                                Assert.AreEqual(404, r.ResponseStatusCode, "The request should not have found any resource.");
                            }
                            else
                            {
                                PreferHeader_VerifyResponse(r, request.ServiceRoot.ToString(), true, (response) =>
                                    {
                                        var relationshipLinks = response.Root.Elements(UnitTestsUtil.AtomNamespace + "link")
                                            .Where(e => e.Attribute("rel").Value.Contains("http://docs.oasis-open.org/odata/ns/relatedlinks/"));

                                        if (includeRelationshipLinks)
                                        {
                                            Assert.AreEqual(1, relationshipLinks.Count(), "The response should contain a relationship link.");
                                        }
                                        else
                                        {
                                            Assert.AreEqual(0, relationshipLinks.Count(), "The resounrce should not contain any relationship links.");
                                        }
                                    });
                                callCount.AssertEquals(expectedCallCount);

                                Assert.AreEqual(httpMethod == "POST" ? UpdateOperations.Add : UpdateOperations.Change, PreferHeader_InterceptorsService.LastItemsUpdateOperation,
                                    "The change interceptor didn't fire correctly.");
                            }
                        });
                    }
                });
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod(), Variation("Verifies that Prefer header has no effect on requests it should not have.")]
            public void PreferHeader_UnrelatedRequests()
            {
                DSPMetadata metadata = PreferHeader_CreateMetadata();
                var item = new DSPSelfmodifyingResource(metadata.GetResourceType("Item"));
                item.SetRawValue("ID", 0);
                item.SetRawValue("ETagProperty", 0);
                item.SetRawValue("Self", item);
                var collection = new DSPResource(metadata.GetResourceType("Collection"));
                collection.SetRawValue("ID", 0);
                collection.SetRawValue("CollectionProperty", new List<int>() { 1, 2, 3 });
                var stream = new DSPSelfmodifyingResource(metadata.GetResourceType("Stream"));
                stream.SetRawValue("ID", 42);
                var namedStream = new DSPSelfmodifyingResource(metadata.GetResourceType("NamedStream"));
                namedStream.SetRawValue("ID", 42);
                DSPServiceDefinition service = PreferHeader_CreateService(metadata, item, collection, stream, namedStream);
                service.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

                var entityReferencePayload = new XElement(UnitTestsUtil.MetadataNamespace + "ref");
                entityReferencePayload.SetAttributeValue("id", "/Items(0)");

                var testCases = new[] {
                    new {
                        HttpMethod = "GET",
                        RequestUri = "/Items",
                        Payload = "", ContentType = ""
                    },
                    new {
                        HttpMethod = "GET",
                        RequestUri = "/Collection",
                        Payload = "", ContentType = ""
                    },
                    new {
                        HttpMethod = "GET",
                        RequestUri = "/$metadata",
                        Payload = "", ContentType = ""
                    },
                    new {
                        HttpMethod = "GET",
                        RequestUri = "/Items/$count",
                        Payload = "", ContentType = ""
                    },
                    new {
                        HttpMethod = "GET",
                        RequestUri = "/Items(0)/Self/$ref",
                        Payload = "", ContentType = ""
                    },
                    new {
                        HttpMethod = "DELETE",
                        RequestUri = "/Collection(0)/Description/$value",
                        Payload = "", ContentType = ""
                    },
                    new {
                        HttpMethod = "PUT",
                        RequestUri = "/Items(0)/Self/$ref",
                        Payload = entityReferencePayload.ToString(),
                        ContentType = UnitTestsUtil.MimeApplicationXml
                    },
                    new {  // GET MR
                        HttpMethod = "GET",
                        RequestUri = "/Streams(42)/$value",
                        Payload = "", ContentType = ""
                    },
                    new {  // GET named stream
                        HttpMethod = "GET",
                        RequestUri = "/NamedStreams(42)/NamedStream1",
                        Payload = "", ContentType = ""
                    },
                    new {  // PUT named stream
                        HttpMethod = "PUT",
                        RequestUri = "/NamedStreams(42)/NamedStream1",
                        Payload = "another text file",
                        ContentType = UnitTestsUtil.MimeTextPlain
                    },
                };

                using (TestWebRequest request = service.CreateForInProcess())
                {
                    TestUtil.RunCombinations(
                        testCases,
                        new string[] { null, "return=representation", "return=minimal" },
                        (testCase, preferHeader) =>
                    {
                        service.ClearChanges();

                        request.HttpMethod = testCase.HttpMethod;
                        request.RequestUriString = testCase.RequestUri;
                        request.RequestHeaders["Prefer"] = preferHeader;
                        request.RequestVersion = "4.0;";
                        request.RequestMaxVersion = "4.0;";
                        request.RequestContentType = string.IsNullOrEmpty(testCase.ContentType) ? null : testCase.ContentType;
                        string payload = testCase.Payload;
                        if (!string.IsNullOrEmpty(payload) && payload.StartsWith("["))
                        {
                            int i = payload.IndexOf("]");
                            string header = payload.Substring(1, i - 1);
                            string[] headervalue = header.Split(':');
                            request.RequestHeaders[headervalue[0]] = headervalue[1];
                            payload = payload.Substring(i + 1);
                        }

                        if (!string.IsNullOrEmpty(payload))
                        {
                            request.SetRequestStreamAsText(payload);
                        }

                        request.SendRequest();

                        string preferApplied;
                        request.ResponseHeaders.TryGetValue("Preference-Applied", out preferApplied);
                        Assert.IsNull(preferApplied, "No Preference-Applied header should be present on the response.");
                    });
                }
            }

            private DSPMetadata PreferHeader_CreateMetadata()
            {
                DSPMetadata metadata = new DSPMetadata("Test", "TestNS");
                var entityType = metadata.AddEntityType("Item", typeof(DSPSelfmodifyingResource), null, false);
                metadata.AddKeyProperty(entityType, "ID", typeof(int));
                metadata.AddPrimitiveProperty(entityType, "ETagProperty", typeof(int?), true);
                metadata.AddPrimitiveProperty(entityType, "TriggerModification", typeof(string));
                var entitySet = metadata.AddResourceSet("Items", entityType);
                metadata.AddResourceReferenceProperty(entityType, "Self", entitySet, entityType);

                var collectionEntity = metadata.AddEntityType("Collection", null, null, false);
                metadata.AddKeyProperty(collectionEntity, "ID", typeof(int));
                metadata.AddPrimitiveProperty(collectionEntity, "Rating", typeof(int?));
                metadata.AddPrimitiveProperty(collectionEntity, "Description", typeof(string));
                metadata.AddCollectionProperty(collectionEntity, "CollectionProperty", typeof(int?));
                metadata.AddResourceSet("Collection", collectionEntity);

                var streamEntity = metadata.AddEntityType("Stream", typeof(DSPSelfmodifyingResource), null, false);
                metadata.AddKeyProperty(streamEntity, "ID", typeof(int));
                metadata.AddPrimitiveProperty(streamEntity, "ETagProperty", typeof(int?), true);
                metadata.AddPrimitiveProperty(streamEntity, "TriggerModification", typeof(string));
                streamEntity.IsMediaLinkEntry = true;
                metadata.AddResourceSet("Streams", streamEntity);

                var namedStreamEntity = metadata.AddEntityType("NamedStream", typeof(DSPSelfmodifyingResource), null, false);
                metadata.AddKeyProperty(namedStreamEntity, "ID", typeof(int));
                namedStreamEntity.AddProperty(new provider.ResourceProperty("NamedStream1", provider.ResourcePropertyKind.Stream, provider.ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))));
                metadata.AddResourceSet("NamedStreams", namedStreamEntity);

                return metadata;
            }

            private DSPServiceDefinition PreferHeader_CreateService(DSPMetadata metadata, object existingItem, object existingCollection, object existingStream, object existingNamedStream)
            {
                DSPMediaResourceStorage mrStorage = new DSPMediaResourceStorage();
                DSPServiceDefinition service = new DSPServiceDefinition()
                {
                    Metadata = metadata,
                    CreateDataSource = (m) =>
                    {
                        DSPContext context = new DSPContext();
                        context.GetResourceSetEntities("Items").Add(existingItem);

                        if (existingCollection != null)
                        {
                            context.GetResourceSetEntities("Collection").Add(existingCollection);
                        }

                        if (existingStream != null)
                        {
                            DSPMediaResource defaultStream = mrStorage.CreateMediaResource(existingStream, null);
                            defaultStream.ContentType = UnitTestsUtil.MimeTextPlain;
                            defaultStream.GetWriteStream().WriteByte((int)'c');
                            context.GetResourceSetEntities("Streams").Add(existingStream);
                        }

                        if (existingNamedStream != null)
                        {
                            DSPMediaResource namedStream1 = mrStorage.CreateMediaResource(existingNamedStream, m.GetResourceType("NamedStream").GetNamedStreams().First(ns => ns.Name == "NamedStream1"));
                            namedStream1.ContentType = UnitTestsUtil.MimeTextPlain;
                            namedStream1.GetWriteStream().WriteByte((int)'d');
                            context.GetResourceSetEntities("NamedStreams").Add(existingNamedStream);
                        }

                        return context;
                    },
                    Writable = true,
                    HostInterfaceType = typeof(IDataServiceHost2),
                    SupportMediaResource = true,
                    SupportNamedStream = true,
                    MediaResourceStorage = mrStorage
                };

                return service;
            }

            private void PreferHeader_SetupRequest(TestWebRequest request, string httpMethod, string format, XDocument existingItemAtom, DSPResource existingItem, Action<XDocument> modifyInputAtom)
            {
                string realHttpMethod = httpMethod;
                if (realHttpMethod == "POSTMR") { realHttpMethod = "POST"; }

                request.HttpMethod = realHttpMethod;
                request.RequestContentType = httpMethod == "POSTMR" ? UnitTestsUtil.MimeTextPlain : format;
                request.Accept = format;

                XDocument inputAtom = new XDocument(existingItemAtom);
                // Remove all nav property links as those are not supported on some operations
                inputAtom.Root.Elements(UnitTestsUtil.AtomNamespace + "link").Where(e =>
                    e.Attribute("rel").Value.Contains("http://docs.oasis-open.org/odata/ns/related/") ||
                    e.Attribute("rel").Value.Contains("http://docs.oasis-open.org/odata/ns/relatedlinks/")).Remove();
                request.RequestHeaders["Slug"] = null;
                if (httpMethod == "POST")
                {
                    inputAtom.Root.Element(UnitTestsUtil.AtomNamespace + "content").Element(UnitTestsUtil.MetadataNamespace + "properties")
                        .Element(UnitTestsUtil.DataNamespace + "ID").Value = "1";
                    request.IfMatch = null;
                    request.RequestUriString = "/Items";
                }
                else if (httpMethod == "POSTMR")
                {
                    request.RequestHeaders["Slug"] = "1";
                    request.IfMatch = null;
                    request.RequestUriString = "/Streams";
                }
                else
                {
                    request.IfMatch = existingItem.GetETag();
                    request.RequestUriString = "/Items(0)";
                }

                if (httpMethod == "POSTMR")
                {
                    request.SetRequestStreamAsText("some text content");
                }
                else
                {
                    if (modifyInputAtom != null)
                    {
                        modifyInputAtom(inputAtom);
                    }
                    request.SetRequestStreamAsText(inputAtom.ToString());
                }
            }

            private bool PreferHeader_ShouldContainResponseBody(TestWebRequest request, bool considerResponsePreference, out bool? effectiveResponsePreference)
            {
                effectiveResponsePreference = null;
                string preferHeader;
                request.RequestHeaders.TryGetValue("Prefer", out preferHeader);
                if (preferHeader != null && considerResponsePreference)
                {
                    if (string.Equals(preferHeader, "return=minimal", StringComparison.OrdinalIgnoreCase))
                    {
                        effectiveResponsePreference = false;
                    }
                    if (string.Equals(preferHeader, "return=representation", StringComparison.OrdinalIgnoreCase))
                    {
                        effectiveResponsePreference = true;
                    }
                }

                bool expectedPayload = false;
                if (request.HttpMethod == "POST")
                {
                    // For POST the payload is included unless the Prefer header says to not include it and version makes the Prefer header valid
                    expectedPayload = effectiveResponsePreference != false;
                }
                else
                {
                    // For PUT/PATCH the payload is not included unless the Prefer header says to include it and version makes the Prefer header valid
                    expectedPayload = effectiveResponsePreference == true;
                }

                return expectedPayload;
            }

            private void PreferHeader_VerifyResponse(TestWebRequest request, string serviceRoot, bool considerResponsePreference, Action<XDocument> responsePayloadValidation)
            {
                bool? effectiveResponsePreference;
                bool expectedPayload = PreferHeader_ShouldContainResponseBody(request, considerResponsePreference, out effectiveResponsePreference);

                ServiceVersion responseVersion = ServiceVersion.FromHeaderValue(request.ResponseVersion);
                string preferApplied;
                request.ResponseHeaders.TryGetValue("Preference-Applied", out preferApplied);
                string odataEntityId;
                request.ResponseHeaders.TryGetValue("OData-EntityId", out odataEntityId);

                if (request.HttpMethod == "POST")
                {
                    // Substring(1) to remove the first slash
                    // For POST we need to add the (1) to identify the entity posted
                    string itemLocation = serviceRoot + request.RequestUriString.Substring(1) + "(1)";
                    if (effectiveResponsePreference == false)
                    {
                        Assert.AreEqual(204, request.ResponseStatusCode, "POST without response payload should respond with status 204.");
                        Assert.AreEqual(itemLocation, odataEntityId, "Response to POST without payload must include the OData-EntityId header with the value being the URL of the item posted.");
                    }
                    else
                    {
                        Assert.AreEqual(201, request.ResponseStatusCode, "POST with response payload should respond with status 201.");
                        Assert.IsNull(odataEntityId, "Response to POST with payload must NOT include the OData-EntityId header.");
                    }

                    Assert.AreEqual(itemLocation, request.ResponseLocation, "The response to POST should include the Location header and it must have the URL to the item posted.");
                }
                else
                {
                    Assert.AreEqual(expectedPayload ? 200 : 204, request.ResponseStatusCode, "Unexpected response status for PUT/PATCH.");
                    Assert.IsNull(odataEntityId, "Response to anything other than POST must NOT include the OData-EntityId header.");
                }

                if (effectiveResponsePreference.HasValue)
                {
                    Assert.IsTrue(responseVersion.Version >= 40, "Response which was influenced by the Prefer header must have DSV >= 4.0");
                    Assert.AreEqual(effectiveResponsePreference == false ? "return=minimal" : "return=representation", preferApplied,
                        "The preference was applied and thus Preference-Applied header should have been included with the right value.");
                }
                else
                {
                    Assert.IsNull(preferApplied, "No Prefer was sent or it was ignored thus Preference-Applied must not be sent in the response.");
                }

                if (expectedPayload)
                {
                    XDocument response = UnitTestsUtil.GetResponseAsAtomXLinq(request);
                    if (responsePayloadValidation != null)
                    {
                        responsePayloadValidation(response);
                    }
                }
                else
                {
                    string response = request.GetResponseStreamAsText();
                    Assert.IsTrue(string.IsNullOrEmpty(response), "No response should have been sent.");
                }
            }

            #endregion Prefer header
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod()]
            public void AllTypesTest()
            {
                Dictionary<Type, PropertyInfo> typeToPropertyMap = new Dictionary<Type, PropertyInfo>();
                // build a dictionary for the type to properties
                foreach (PropertyInfo property in typeof(AllTypes).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.Name == "ID")
                    {
                        continue;
                    }

                    typeToPropertyMap.Add(property.PropertyType, property);
                }

                int maxSampleValue = 0;
                Dictionary<Type, TypeData> typeToDataMap = new Dictionary<Type, TypeData>();
                foreach (TypeData typeData in TypeData.Values)
                {
                    // Ignore the invalid types
                    if (!typeToPropertyMap.ContainsKey(typeData.ClrType))
                    {
                        continue;
                    }

                    typeToDataMap.Add(typeData.ClrType, typeData);
                    if (maxSampleValue < typeData.SampleValues.Length)
                    {
                        maxSampleValue = typeData.SampleValues.Length;
                    }
                }

                IEdmEntityType entityType = null;
                for (int i = 0; i < maxSampleValue; i++)
                {
                    // Get the metadata for the type
                    if (i == 0)
                    {
                        var model = UnitTestsUtil.LoadMetadataFromDataServiceType(typeof(TypedCustomAllTypesDataContext), null);
                        entityType = model.FindType(typeof(AllTypes).FullName) as IEdmEntityType;
                    }

                    object entityInstance = typeof(AllTypes).GetConstructor(System.Type.EmptyTypes).Invoke(null);
                    PopulateResource(entityType, typeToDataMap, entityInstance, i);

                    InsertEntityAndCompareValues(entityType, entityInstance);
                }

                // create entity with all default values
                AllTypes emptyAllTypes = new AllTypes();
                emptyAllTypes.ID = 1;
                InsertEntityAndCompareValues(entityType, emptyAllTypes);
            }

            // ODataLib was fixed and reports missing type name as an annotation.
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod]
            public void UpdateAtomCategoryTest()
            {
                string[] schemes = new string[]
                {
                    AtomUpdatePayloadBuilder.DataWebSchemeNamespace,
                    "foo",
                    null
                };

                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                    new Dimension("scheme0", schemes),
                    new Dimension("scheme1", schemes),
                    new Dimension("orders", new int[] { 0, 1, 2 }));
                TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                {
                    string scheme0 = (string)values["scheme0"];
                    string scheme1 = (string)values["scheme1"];
                    int orders = (int)values["orders"];
                    using (CustomDataContext.CreateChangeScope())
                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        var atom = UnitTestsUtil.AtomNamespace;
                        var data = UnitTestsUtil.DataNamespace;
                        var metadata = UnitTestsUtil.MetadataNamespace;
                        XElement d = new XElement(atom + "entry",
                            new XElement(atom + "content",
                                new XAttribute("type", "application/xml"),
                                new XElement(metadata + "properties",
                                    new XElement(data + "ID", "1000"))));
                        if (scheme0 != null) d.Add(AtomUpdatePayloadBuilder.GetCategoryXElement(typeof(Customer).FullName, scheme0));
                        if (scheme1 != null) d.Add(AtomUpdatePayloadBuilder.GetCategoryXElement(typeof(Customer).FullName, scheme1));
                        if (orders > 0)
                        {
                            var link = new XElement(
                                atom + "link",
                                new XAttribute("rel", "http://docs.oasis-open.org/odata/ns/related/Orders"),
                                new XAttribute("type", "application/atom+xml;type=feed"),
                                new XAttribute("title", "Orders"));
                            for (int i = 0; i < orders; i++)
                            {
                                link.Add(new XElement(metadata + "inline",
                                    new XElement(atom + "feed",
                                        new XElement(atom + "entry",
                                            new XElement(atom + "content",
                                                new XAttribute("type", "application/xml"),
                                                new XElement(metadata + "properties",
                                                    new XElement(data + "ID", 10000 + i)))))));
                            }
                            d.Add(link);
                        }

                        request.DataServiceType = typeof(CustomDataContext);
                        request.RequestContentType = SerializationFormatData.Atom.MimeTypes[0];
                        string requestText = d.ToString();
                        Trace.WriteLine("Sending request:\r\n" + requestText);
                        request.SetRequestStreamAsText(requestText);
                        request.HttpMethod = "POST";
                        request.RequestUriString = "/Customers";
                        Exception exception = TestUtil.RunCatching(request.SendRequest);
                        // NOTE: incorrect exception thrown when orders > 1, but we'll fix that after V1.
                        TestUtil.AssertExceptionExpected(exception,
                            orders > 1,
                            scheme0 != AtomUpdatePayloadBuilder.DataWebSchemeNamespace && scheme1 != AtomUpdatePayloadBuilder.DataWebSchemeNamespace);
                    }
                });
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition1"), TestMethod]
            public void RoundTrippingTests()
            {
                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<entry xml:base=\"/\" " + TestUtil.CommonPayloadNamespaces + ">" +
                        AtomUpdatePayloadBuilder.GetCategoryXml(typeof(Customer).FullName) +
                        "<content type=\"applicatiOn/xmL\"><adsm:properties>" +
                            "<ads:Name>Foo</ads:Name>" +
                            "<ads:ID>125</ads:ID>" +
                            "<ads:Address adsm:type='" + typeof(Address).FullName + "'>" +
                                "<ads:StreetAddress>Street Number, Street Address</ads:StreetAddress>" +
                                "<ads:City>Redmond</ads:City>" +
                                "<ads:PostalCode>98052</ads:PostalCode>" +
                            "</ads:Address>" +
                        "</adsm:properties></content>" +
                    "</entry>";

                TestUtil.RunCombinations(
                    UnitTestsUtil.ProviderTypes,
                    UnitTestsUtil.ResponseFormats,
                    UnitTestsUtil.BooleanValues,
                    new string[] { "PATCH" },
                    (providerType, responseFormat, refPropertyNull, mergeHttpVerb) =>
                {
                    string payload = atomPayload;

                    using (TestUtil.RestoreStaticValueOnDispose(typeof(BaseTestWebRequest), "HostInterfaceType"))
                    using (UnitTestsUtil.CreateChangeScope(providerType))
                    {
                        // Post a new entity
                        BaseTestWebRequest.HostInterfaceType = typeof(Microsoft.OData.Service.IDataServiceHost2);
                        TestWebRequest request = UnitTestsUtil.GetTestWebRequestInstance(responseFormat, UnitTestsUtil.ConvertUri(providerType, "/Customers"), providerType, null, "POST", UnitTestsUtil.ConvertPayload(providerType, payload));
                        Assert.IsTrue(request.ResponseStatusCode == 201, "The entity must be successfully created");

                        payload = request.GetResponseStreamAsText();
                        string uri = "/Customers(125)";

                        // Round-Tripping PUT and PATCH scenarios when ref property is non-null
                        // Update the entity with PATCH request
                        payload = payload.Replace("Foo", "FooMerge");

                        var ifMatch = new KeyValuePair<string, string>[]
                        {
                            new KeyValuePair<string, string>("If-Match", request.ResponseETag),
                            new KeyValuePair<string, string>("Prefer", "return=representation")
                        };
                        request = UnitTestsUtil.GetTestWebRequestInstance(responseFormat, UnitTestsUtil.ConvertUri(providerType, uri), providerType, ifMatch, mergeHttpVerb, UnitTestsUtil.ConvertPayload(providerType, payload));
                        UnitTestsUtil.VerifyResponseHeaders(request, new[] { new KeyValuePair<string, string>("Preference-Applied", "return=representation") });
                        Assert.IsTrue(request.ResponseStatusCode == 200, "The entity must be successfull updated");

                        // verify that the update actually happened.
                        payload = request.GetResponseStreamAsText();

                        Assert.IsTrue(payload.Contains("FooMerge"), "update happened successfully");
                    }
                });
            }

            #region Helper Methods

            private void InsertEntityAndCompareValues(IEdmEntityType entityType, object entityInstance)
            {
                CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("ResponseFormat", UnitTestsUtil.ResponseFormats));

                TestUtil.RunCombinatorialEngineFail(engine, delegate (Hashtable values)
                {
                    string responseFormat = (string)values["ResponseFormat"];
                    string payload = UnitTestsUtil.GetPayload(entityInstance, entityType, responseFormat);
                    Stream resultStream = null;

                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                    {
                        request.DataServiceType = typeof(TypedCustomDataContext<AllTypes>);
                        request.RequestUriString = "/Values";
                        request.Accept = responseFormat;
                        request.HttpMethod = "POST";
                        request.RequestStream = IOUtil.CreateStream(payload);
                        request.RequestContentType = responseFormat;
                        bool succeeded = false;
                        try
                        {
                            request.SendRequest();
                            resultStream = request.GetResponseStream();
                            succeeded = true;
                        }
                        finally
                        {
                            if (!succeeded)
                            {
                                AstoriaTestLog.WriteLine("Payload sent that produced the error:\r\n" + payload);
                            }
                        }
                    }

                    XmlDocument document = UnitTestsUtil.VerifyXPaths(resultStream, responseFormat, new string[0]);
                    CompareEntityWithPayload(document, entityInstance, entityType, responseFormat);
                });
            }

            private void PopulateResource(IEdmEntityType entityType, Dictionary<Type, TypeData> sampleValues, object instance, int index)
            {
                var properties = entityType.StructuralProperties().ToList();
                for (int i = 0; i < properties.Count; i++)
                {
                    IEdmProperty property = properties[i];
                    PropertyInfo propertyInfo = instance.GetType().GetProperty(property.Name);
                    Type propertyType = propertyInfo.PropertyType;
                    propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                    TypeData sampleValueForType = sampleValues[propertyType];
                    object propertyValue;
                    if (index < sampleValueForType.SampleValues.Length)
                    {
                        propertyValue = sampleValueForType.SampleValues[index];
                    }
                    else
                    {
                        propertyValue = sampleValueForType.SampleValues[sampleValueForType.SampleValues.Length - 1];
                    }

                    // set the property value
                    propertyInfo.SetValue(instance, propertyValue, null);
                }
            }

            private void CompareEntityWithPayload(XmlDocument document, object expectedInstance, IEdmEntityType entityType, string responseFormat)
            {
                foreach (var property in entityType.StructuralProperties())
                {
                    PropertyInfo propertyInfo = expectedInstance.GetType().GetProperty(property.Name);
                    Type propertyType = propertyInfo.PropertyType;
                    propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                    object expectedValue = propertyInfo.GetValue(expectedInstance, null);
                    ComparePropertyValue(document, propertyInfo, responseFormat, expectedValue);
                }
            }

            public static void ComparePropertyValue(XmlDocument document, PropertyInfo property, string responseFormat, object expectedValue)
            {
                UnitTestsUtil.ComparePropertyValue(document, property, responseFormat, expectedValue);
            }

            internal static void VerifyInvalidRequestForVariousProviders(string payload, string uri, string contentFormat, string httpMethodName, int expectedHttpErrorCode, string errorMessage = null)
            {
                UnitTestsUtil.VerifyInvalidRequest(payload, uri, typeof(CustomDataContext), contentFormat, httpMethodName, expectedHttpErrorCode, errorMessage);
            }

            internal static void VerifyInvalidRequestForVariousProviders1(PayloadBuilder payloadBuilder, string uri, string contentFormat, string httpMethodName, int expectedHttpErrorCode, string errorMessage = null)
            {
                UnitTestsUtil.VerifyInvalidRequest(PayloadGenerator.Generate(payloadBuilder, contentFormat), uri, typeof(CustomDataContext), contentFormat, httpMethodName, expectedHttpErrorCode, errorMessage);
            }

            internal static void CustomProviderRequest(Type providerType, string uri, string responseFormat, string payload, string[] xPaths, string httpMethodName, bool verifyETagReturned)
            {
                UnitTestsUtil.CustomProviderRequest(providerType, uri, responseFormat, payload, xPaths, httpMethodName, verifyETagReturned);
            }

            internal static void CustomProviderRequest(
                Type providerType,
                string uri,
                string responseFormat,
                string payload,
                KeyValuePair<string, string[]>[] uriAndXPathsToVerify,
                string httpMethodName,
                bool verifyETagReturned,
                IList<KeyValuePair<string, string>> requestHeaders = null,
                IList<KeyValuePair<string, string>> responseHeaders = null,
                UnitTestsUtil.SendRequestModifier sendRequestModifier = UnitTestsUtil.SendRequestModifier.None)
            {
                UnitTestsUtil.CustomProviderRequest(providerType, uri, responseFormat, payload, uriAndXPathsToVerify, httpMethodName, verifyETagReturned, requestHeaders, responseHeaders, sendRequestModifier);
            }

            internal static void CustomProviderRequest(
                Type providerType,
                string uri,
                string responseFormat,
                PayloadBuilder payloadBuilder,
                KeyValuePair<string, string[]>[] uriAndXPathsToVerify,
                string httpMethodName,
                bool verifyETagReturned,
                IList<KeyValuePair<string, string>> requestHeaders = null,
                IList<KeyValuePair<string, string>> responseHeaders = null,
                UnitTestsUtil.SendRequestModifier sendRequestModifier = UnitTestsUtil.SendRequestModifier.None)
            {
                using (UnitTestsUtil.AppendTypesForOpenProperties(providerType, payloadBuilder, responseFormat))
                {
                    UnitTestsUtil.CustomProviderRequest(providerType, uri, responseFormat, PayloadGenerator.Generate(payloadBuilder, responseFormat), uriAndXPathsToVerify, httpMethodName, verifyETagReturned, requestHeaders, responseHeaders, sendRequestModifier);
                }
            }

            internal static void SetHeaderValues(TestWebRequest request, IEnumerable<KeyValuePair<string, string>> headerValues)
            {
                UnitTestsUtil.SetHeaderValues(request, headerValues);
            }

            #endregion // Helper methods
        }
    }
}
