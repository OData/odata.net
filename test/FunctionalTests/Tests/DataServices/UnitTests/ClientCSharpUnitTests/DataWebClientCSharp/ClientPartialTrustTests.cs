//---------------------------------------------------------------------
// <copyright file="ClientPartialTrustTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Client;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using AstoriaUnitTests.Stubs.CustomDataClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// .net 2.0 behaviour
[assembly:SecurityRules(SecurityRuleSet.Level1)]

namespace AstoriaUnitTests.DataWebClientCSharp
{
    public class NarrowCustomer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Order> Orders { get; set; }
    }

    [Serializable]
    internal class ClientPartialTrustTestContext : MarshalByRefObject
    {
        /// <summary>
        /// Visual Studio test engine is security critical and therefore cannot be used
        /// in a partial trust sandbox. Use this instead
        /// </summary>
        /// <param name="condition">The condition to assert on</param>
        /// <param name="message">The failure message</param>
        private void Assert(bool condition, string message)
        {
            if (!condition)
                throw new Exception(message);
        }
        
        public void PartialTrustFailure()
        {
            FieldInfo fi = typeof(Int32).GetField("m_value", BindingFlags.Instance | BindingFlags.NonPublic);
            object value = fi.GetValue(1);
            Console.WriteLine("value: {0}", value);
        }

        public void ClientProjectionPartialTrust(Uri serviceRoot)
        {
            DataServiceContext ctx = new DataServiceContext(serviceRoot);
            //ctx.EnableAtom = true;
            //ctx.Format.UseAtom();
            ctx.MergeOption = MergeOption.OverwriteChanges;

            var cust = ctx.CreateQuery<Customer>("Customers").Select(c => new Customer()
            {
                ID = c.ID,
                Name = c.Name,
                Address = c.Address,
                Orders = c.Orders
            }).FirstOrDefault();
            
            Assert("Customer 0" == cust.Name, "Incorrect Customer Name");
            Assert(2 == cust.Orders.Count, "Incorrect Orders Count");

            // EnumerateAsElementType<Order>
            var custEnumerateOrders = ctx.CreateQuery<Customer>("Customers").Select(
                c => new Customer()
                {
                    ID = c.ID,
                    Orders = new DataServiceCollection<Order>(c.Orders.Select(o => o))
                }).FirstOrDefault();
            Assert(2 == custEnumerateOrders.Orders.Count, "Incorrect Orders Count");

            var custEnumerateOrders2 = ctx.CreateQuery<Customer>("Customers").Select(
                c => new Customer()
                {
                    ID = c.ID,
                    Orders = new DataServiceCollection<Order>(c.Orders.Select(o =>
                        new Order()
                        {
                            DollarAmount=o.DollarAmount,
                            Customer = o.Customer,
                            OrderDetails = new DataServiceCollection<OrderDetail>(o.OrderDetails.Select(od =>
                                new OrderDetail()
                                {
                                    OrderID=od.OrderID,
                                    ProductID=od.ProductID
                                }))                                
                        }))
                }).FirstOrDefault();
            Assert(2 == custEnumerateOrders2.Orders.Count, "Incorrect Orders Count");

            var order1 = ctx.CreateQuery<Order>("Orders").Select(
                o => new Order()
                {
                    ID = o.ID,
                    OrderDetails = new DataServiceCollection<OrderDetail>(o.OrderDetails)
                }).FirstOrDefault();

            Assert(1 == order1.OrderDetails.Count, "Incorrect order details count");

            var order2 = ctx.CreateQuery<Order>("Orders").Select(o => new Order()
            {
                Customer = o.Customer
            }).FirstOrDefault();
            Assert("Customer 0" == order2.Customer.Name, "Incorrect customer name");
        }

        public void ClientProjectionPartialTrustAnonymousType(Uri serviceRoot)
        {
            DataServiceContext ctx = new DataServiceContext(serviceRoot);
            //ctx.EnableAtom = true;
            //ctx.Format.UseAtom();
            var q = ctx.CreateQuery<Customer>("Customers").Select(c => new
            {
                ID = c.ID,
                Name = c.Name,
                NameLength = c.Name.Length - 5,
                Address = c.Address
            });

            var cust = q.FirstOrDefault();
            Assert("Customer 0" == cust.Name, "Incorrect Customer Name");
            Assert(5 == cust.NameLength, "Length is incorrect");
            
            var order = ctx.CreateQuery<Order>("Orders").Select(o => new 
            {
                ID = o.ID,
                Customer = new
                {
                    Name = o.Customer.Name + " test"
                }
            }).FirstOrDefault();
            Assert("Customer 0 test" == order.Customer.Name, "Incorrect customer name");

            var o1 = ctx.CreateQuery<Customer>("Customers").Select(c => new { c }).FirstOrDefault();
            Assert("Customer 0" == o1.c.Name, "Incorrect customer name");
            
            // nested anonymous type -> IEnumable<_anonymous>
            var c1 = ctx.CreateQuery<Customer>("Customers").Select(c => new
            {
                ID = c.ID,
                Orders = c.Orders.Select(o => new { ID = o.ID })
            }).FirstOrDefault();
            Assert(2 == c1.Orders.Count(), "incorrect orders count");
        }
        
        public void ClientProjectionPartialTrustRef(Uri serviceRoot)
        {
            DataServiceContext ctx = new DataServiceContext(serviceRoot);
            //ctx.EnableAtom = true;
            //ctx.Format.UseAtom();
            var q = ctx.CreateQuery<Order>("Orders").Where(o => o.ID == 0).Select(o => new NarrowCustomer()
            {
                ID = o.Customer.ID,
                Name = o.Customer.Name,
                Orders = o.Customer.Orders.ToList()
            });

            var cust = q.FirstOrDefault();
            Assert("Customer 0" == cust.Name, "Incorrect Customer Name");
        }

        public void ClientNamedStreamProjectionTest(Uri serviceRoot)
        {
            {
                // Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
                DataServiceContext context = new DataServiceContext(serviceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                var q = from s in context.CreateQuery<StreamType>("MySet1")
                            select new
                            {
                                ID = s.ID,
                                StreamLink = s.Stream1,
                                Collection = (from c in s.Collection
                                              select new
                                              {
                                                  ID = c.ID,
                                                  StreamLink = c.ColStream,
                                                  Collection = (from c1 in c.Collection
                                                                select new
                                                                {
                                                                    ID = c1.ID,
                                                                    StreamLink = c1.RefStream1
                                                                }).ToList()
                                              }).ToList()
                            };

                Assert(q.ToString() == serviceRoot.AbsoluteUri + "/MySet1?$expand=Collection($select=ID),Collection($select=ColStream),Collection($expand=Collection($select=ID)),Collection($expand=Collection($select=RefStream1))&$select=ID,Stream1", "make sure the right uri is produced by the linq translator");

                foreach (var o in q)
                {
                    Assert(o.StreamLink.EditLink.AbsoluteUri == serviceRoot.AbsoluteUri + "/photolink", "the stream url for Stream1 must be populated correctly");
                    Assert(o.Collection.First().StreamLink.EditLink.AbsoluteUri == serviceRoot.AbsoluteUri + "/outerCollectionStreamLink1", "the stream url of the collection stream must be populated correctly - index 0");
                    Assert(o.Collection.First().Collection.First().StreamLink.EditLink.AbsoluteUri == serviceRoot.AbsoluteUri + "/innerCollectionStreamLink1", "the stream url of the collection stream must be populated correctly - index 0 - index 0");
                    Assert(o.Collection.First().Collection.ElementAt(1).StreamLink.EditLink.AbsoluteUri == serviceRoot.AbsoluteUri + "/innerCollectionStreamLink2", "the stream url of the collection stream must be populated correctly - index 0 - index 1");
                }

                Assert(context.Entities.Count == 0, "there should be no entities tracked by the context");
            }
        }
    }

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    // [TestClass]
    public class ClientPartialTrustTests
    {
        private static AppDomain mediumTrustDomain;

        #region Test Setup

        public static AppDomain GetMediumTrustDomain()
        {
            if (mediumTrustDomain == null)
            {
                var setup = new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory };
                var permissions = new System.Security.PermissionSet(null);
                // MEDIUM TRUST PERMISSION SET:
                //<IPermission
                //        class="DnsPermission"
                //        version="1"
                //        Unrestricted="true"
                // />
                permissions.AddPermission(new System.Net.DnsPermission(System.Security.Permissions.PermissionState.Unrestricted));
                //<IPermission
                //        class="EnvironmentPermission"
                //        version="1"
                //        Read="TEMP;TMP;USERNAME;OS;COMPUTERNAME"
                // />
                permissions.AddPermission(new EnvironmentPermission(EnvironmentPermissionAccess.Read, "TEMP;TMP;USERNAME;OS;COMPUTERNAME"));
                //<IPermission
                //        class="FileIOPermission"
                //        version="1"
                //        Read="$AppDir$"
                //        Write="$AppDir$"
                //        Append="$AppDir$"
                //        PathDiscovery="$AppDir$"
                // />
                string appDir = Directory.GetCurrentDirectory();
                permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read, appDir));
                permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Write, appDir));
                permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Append, appDir));
                permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery, appDir));
                //<IPermission
                //        class="IsolatedStorageFilePermission"
                //        version="1"
                //        Allowed="AssemblyIsolationByUser"
                //        UserQuota="9223372036854775807"
                // />
                permissions.AddPermission(new IsolatedStorageFilePermission(PermissionState.Unrestricted) { UsageAllowed = IsolatedStorageContainment.AssemblyIsolationByUser, UserQuota = 9223372036854775807 });
                //<IPermission
                //        class="PrintingPermission"
                //        version="1"
                //        Level="DefaultPrinting"
                // />
                // DEVNOTE(pqian): System.Drawing.Printing.PrintingPermission - not astoria related
                //<IPermission
                //        class="SecurityPermission"
                //        version="1"
                //        Flags="Execution, ControlThread, ControlPrincipal, RemotingConfiguration"
                ///>
                permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.SerializationFormatter));
                //<IPermission
                //        class="SmtpPermission"
                //        version="1"
                //        Access="Connect"
                // />
                permissions.AddPermission(new System.Net.Mail.SmtpPermission(System.Net.Mail.SmtpAccess.Connect));
                //<IPermission
                //        class="SqlClientPermission"
                //        version="1"
                //        Unrestricted="true"
                ///>
                permissions.AddPermission(new System.Data.SqlClient.SqlClientPermission(PermissionState.Unrestricted));
                //<IPermission
                //        class="TypeDescriptorPermission"
                //        version="1"
                //        Unrestricted="true"
                // />
                permissions.AddPermission(new TypeDescriptorPermission(PermissionState.Unrestricted));
                //<IPermission
                //        class="WebPermission"
                //        version="1"
                //        Unrestricted="true"
                // />
                permissions.AddPermission(new System.Net.WebPermission(PermissionState.Unrestricted));
                //<IPermission
                //        class="ReflectionPermission"
                //        version="1"
                //        Flags="RestrictedMemberAccess"/>
                permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess));

                mediumTrustDomain = AppDomain.CreateDomain("Partial Trust AppDomain: " + DateTime.Now.Ticks, null, setup, permissions);
            }

            return mediumTrustDomain;
        }

        private ClientPartialTrustTestContext GetRunningSandbox(AppDomain domain)
        {
            return (ClientPartialTrustTestContext)domain.CreateInstanceFromAndUnwrap(typeof(ClientPartialTrustTestContext).Assembly.Location, typeof(ClientPartialTrustTestContext).FullName);
        }

        #endregion

        /* 
         * DEVNOTE(pqian)
         * Regarding how to test client functionality in partially trusted AppDomains:
         * A medium trust domain is already present. Follow the same pattern to create other partial trust domains if needed.
         * The test code is executed in a sandbox, all arguments passed into the sandbox must be serializable
         * Limit what happens in the box to CLIENT CODE ONLY. Do the test and service setup outside of the sandbox and then pass it in.
         * Within the sandbox code you cannot use Visual Studio Unit Test Assembly to assert due to the lack of security permission. 
         * Use the assert in the test context instead.
         */
        [Ignore] // Remove Atom
        [TestMethod]
        public void ClientProjectionPartialTrust()
        {
            using (AstoriaUnitTests.Stubs.TestWebRequest request = AstoriaUnitTests.Stubs.TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(AstoriaUnitTests.Stubs.CustomDataContext);
                request.StartService();
                GetRunningSandbox(GetMediumTrustDomain()).ClientProjectionPartialTrust(request.ServiceRoot);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void ClientProjectionPartialTrustAnonymousType()
        {
            using (AstoriaUnitTests.Stubs.TestWebRequest request = AstoriaUnitTests.Stubs.TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(AstoriaUnitTests.Stubs.CustomDataContext);
                request.StartService();
                GetRunningSandbox(GetMediumTrustDomain()).ClientProjectionPartialTrustAnonymousType(request.ServiceRoot);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void ClientProjectionPartialTrustRef()
        {
            using (AstoriaUnitTests.Stubs.TestWebRequest request = AstoriaUnitTests.Stubs.TestWebRequest.CreateForInProcessWcf())
            {
                request.DataServiceType = typeof(AstoriaUnitTests.Stubs.CustomDataContext);
                request.StartService();
                GetRunningSandbox(GetMediumTrustDomain()).ClientProjectionPartialTrustRef(request.ServiceRoot);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void ClientNamedStreamProjection()
        {
            using (AstoriaUnitTests.Stubs.TestWebRequest request = AstoriaUnitTests.Stubs.TestWebRequest.CreateForInProcessWcf())
            using (AstoriaUnitTests.Stubs.PlaybackService.OverridingPlayback.Restore())
            {
                request.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                request.StartService();

                string serviceRoot = request.ServiceRoot.AbsoluteUri;

                string innerCollectionXml =
                FeedStart +
                AnyEntry(serviceRoot, "e4", "<d:ID>4</d:ID>", LinkEditNamedStream("RefStream1", "image/jpeg", serviceRoot + "/innerCollectionStreamLink1")) +
                AnyEntry(serviceRoot, "e5", "<d:ID>5</d:ID>", LinkEditNamedStream("RefStream1", "image/jpeg", serviceRoot + "/innerCollectionStreamLink2")) +
                "</feed>";

                string outerCollectionXml =
                    FeedStart +
                    AnyEntry(serviceRoot, "e2", "<d:ID>2</d:ID>", LinkEditNamedStream("ColStream", "image/jpeg",  serviceRoot + "/outerCollectionStreamLink1"), LinkFeed("Collection", innerCollectionXml)) +
                    "</feed>";

                string xml =
                    FeedStart +
                    AnyEntry(serviceRoot, "e1", "<d:ID>1</d:ID>",
                             LinkFeed("Collection", outerCollectionXml) +
                             LinkEditNamedStream("Stream1", "image/jpeg", serviceRoot + "/photolink")) +
                    "</feed>";

                AstoriaUnitTests.Stubs.PlaybackService.OverridingPlayback.Value = AstoriaUnitTests.Stubs.PlaybackService.ConvertToPlaybackServicePayload(null, xml);
                GetRunningSandbox(GetMediumTrustDomain()).ClientNamedStreamProjectionTest(request.ServiceRoot);
            }
        }

        #region Payload Builder Functions
        internal const string FeedStart = "<feed xml:base='http://localhost/' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>";

        internal static string AnyEntry(string contextRoot, string id, string properties, params string[] links)
        {
            string payload = "<entry><id>" + contextRoot + id + "</id>" +
                "<link rel='edit' href='" + id + "'/>" +
                "<content type='application/xml'><m:properties>" + properties +
                "</m:properties></content>";

            foreach (string l in links)
                payload += l;

            return payload + "</entry>";
        }

        internal static string LinkEditNamedStream(string name, string contentType, string url)
        {
            return String.Format("<link rel='http://docs.oasis-open.org/odata/ns/edit-media/{0}' type='{1}' title='{0}' href='{2}' />",
               name,
               contentType,
               url);
        }

        internal static string LinkEntry(string name, string content)
        {
            return
                "<link rel='http://docs.oasis-open.org/odata/ns/related/" + name +
                "' type='application/atom+xml;type=entry' title='" + name + "' href='foo'>" +
                "<m:inline>" + content + "</m:inline></link>";
        }

        internal static string LinkFeed(string name, string content)
        {
            return
                "<link rel='http://docs.oasis-open.org/odata/ns/related/" + name +
                "' type='application/atom+xml;type=feed' title='" + name + "' href='foo'>" +
                "<m:inline>" + content + "</m:inline></link>";
        }
        #endregion Payload Builder Functions
    }

    public class StreamType
    {
        public StreamType()
        {
            this.Collection = new List<StreamType>();
        }

        public int ID { get; set; }
        public StreamType Ref { get; set; }
        public List<StreamType> Collection { get; set; }
        public DataServiceStreamLink StreamLink { get; set; }
        public DataServiceStreamLink Stream1 { get; set; }
        public DataServiceStreamLink RefStream1 { get; set; }
        public DataServiceStreamLink ColStream { get; set; }
    }
}
