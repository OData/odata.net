//---------------------------------------------------------------------
// <copyright file="DbContextTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------


namespace AstoriaUnitTests.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.SqlClient;
    using System.Data.Test.Astoria;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/875
    [Ignore] // Remove Atom
    // [TestClass]
    public class DbContextTest
    {
        static readonly Type ContextType;
        static readonly string DatabaseName;

        const string ODataNamespace = "http://docs.oasis-open.org/odata/ns/data";

        static DbContextTest()
        {
            ContextType = typeof(ElevatorDbContext);
            DatabaseName = ContextType.FullName;
            Database.DefaultConnectionFactory =
                new SqlConnectionFactory(@"server=" + DataUtil.DefaultDataSource + @";integrated security=true;");
        }

        [ClassInitialize]
        public static void CreateAndInitDatabaseWithCodeFirst(TestContext context)
        {
            using (SqlConnection connection = CreateAndOpenSqlConnection())
            {
                DropDatabaseIfExists(connection);
                InitializeServiceAndDatabase(connection);
            }
        }

        [ClassCleanup]
        public static void DropDatabase()
        {
            using (SqlConnection connection = CreateAndOpenSqlConnection())
            {
                DropDatabaseIfExists(connection);
            }
        }

        [TestCategory("Partition2"), TestMethod]
        public void BasicQueryTests()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = ContextType;
                request.Accept = "application/atom+xml,application/xml";
                request.ForceVerboseErrors = true;

                SendRequestAndAssertExistence(request, "/Elevators", 
                    "/atom:feed/atom:entry/atom:content/adsm:properties[ads:ID=1 and ads:Location='NE'] | /atom:entry/atom:link[@href='12345']",
                    "/atom:feed/atom:entry/atom:content/adsm:properties[ads:ID=2 and ads:Location='NW'] | /atom:entry/atom:link[@href='67890']",
                    "/atom:feed/atom:entry/atom:content/adsm:properties[ads:ID=3 and ads:Location='E'] | /atom:entry/atom:link[@href='02468']"
                );

                SendRequestAndAssertExistence(request, "/ElevatorControlSystems?$expand=UpQueue,DownQueue,FloorCalls",
                    "/atom:feed/atom:entry/atom:link[@title='UpQueue']/adsm:inline/atom:feed/atom:entry/atom:link[@href='Elevators(2)']",
                    "/atom:feed/atom:entry/atom:link[@title='DownQueue']/adsm:inline/atom:feed/atom:entry/atom:link[@href='Elevators(1)']",
                    "/atom:feed/atom:entry/atom:link[@title='FloorCalls']/adsm:inline/atom:feed/atom:entry/atom:link[@href='FloorCalls(1)']",
                    "/atom:feed/atom:entry/atom:link[@title='FloorCalls']/adsm:inline/atom:feed/atom:entry/atom:link[@href='FloorCalls(2)']"
                );

                SendRequestAndAssertExistence(request, "/FloorCalls?$filter=Floor gt 3&$select=Floor");
            }
        }

        [TestCategory("Partition2"), TestMethod]
        public void BasicUpdateTests()
        {
            ICollection<Tuple<string, string, string, string>> payloadsAndVerifications = new List<Tuple<string, string, string, string>>
            {
                new Tuple<string, string, string, string>(
                    "/Elevators", "/Elevators(4)",
                    "{ @odata.type: \"AstoriaUnitTests.Tests.Elevator\", ID: 4, SerialNumber: \"98765\", Location: \"W\" }",
                    "/atom:entry/atom:content/adsm:properties[ads:ID=4 and ads:Location='W'] | /atom:entry/atom:link[@href='98765']"),
                new Tuple<string, string, string, string>(
                    "/FloorCalls", "/FloorCalls(3)",
                    "{ @odata.type: \"AstoriaUnitTests.Tests.FloorCall\", ID: 3, Floor: 2, Up: \"true\", Down: \"true\" }",
                    "/atom:entry/atom:content/adsm:properties[ads:ID=3 and ads:Up='true' and ads:Down='true'] | /atom:entry/atom:category[@term='2']"),
            };

            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = ContextType;
                request.Accept = "application/atom+xml,application/xml";
                request.ForceVerboseErrors = true;
                
                request.RequestContentType = SerializationFormatData.JsonLight.MimeTypes[0] + ";charset=iso-8859-1";

                TestUtil.RunCombinations<Tuple<string, string, string, string>>(payloadsAndVerifications, (testcase) =>
                {
                    string collectionUri = testcase.Item1;
                    string singleUri = testcase.Item2;
                    string payload = testcase.Item3;
                    string verifyXPath = testcase.Item4;

                    request.HttpMethod = "POST";
                    request.RequestStream = IOUtil.CreateStream(payload);
                    SendRequestAndAssertExistence(request, collectionUri, verifyXPath);

                    request.HttpMethod = "PATCH";
                    request.RequestUriString = singleUri;
                    request.RequestStream = IOUtil.CreateStream(payload);
                    request.SendRequest();

                    request.HttpMethod = "PUT";
                    request.RequestUriString = singleUri;
                    request.RequestStream = IOUtil.CreateStream(payload);
                    request.SendRequest();

                    request.HttpMethod = "DELETE";
                    request.RequestUriString = singleUri;
                    request.SendRequest();
                });
            }
        }

        [TestCategory("Partition2"), TestMethod]
        public void DbEntityValidationFailureProducesUsableMessage()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = ContextType;
                request.ForceVerboseErrors = true;
                request.RequestContentType = UnitTestsUtil.AtomFormat;

                string uri = "/Elevators(2)";
                XName elementName = XName.Get("SerialNumber", ODataNamespace);
                const string changedValue = "123456";

                request.HttpMethod = "GET";
                request.Accept = "application/atom+xml,application/xml";
                request.RequestStream = null;
                request.RequestUriString = uri;
                request.SendRequest();

                XDocument entry = request.GetResponseStreamAsXDocument();

                XElement element = entry.Descendants(elementName).Single();

                element.Value = changedValue;

                request.HttpMethod = "PATCH";
                request.Accept = "application/atom+xml,application/xml";
                request.RequestStream = IOUtil.CreateStream(entry.ToString(SaveOptions.DisableFormatting));

                System.Net.WebException exception = TestUtil.RunCatching<System.Net.WebException>(() => request.SendRequest());

                Assert.IsNotNull(exception, "Expected an exception, but none occurred.");
                Assert.IsNotNull(exception.InnerException, "Expected an inner exception, but found none");
                Assert.AreEqual("The field SerialNumber must be a string with a minimum length of 5 and a maximum length of 5.", exception.InnerException.Message, "Didn't get the expected error message");
            }
        }

        static void SendRequestAndAssertExistence(TestWebRequest request, string requestUri, params string[] xpaths)
        {
            request.RequestUriString = requestUri;
            request.SendRequest();
            XmlDocument responseXml = request.GetResponseStreamAsXmlDocument();

            foreach (string xpath in xpaths)
            {
                TestUtil.AssertSelectSingleElement(responseXml, xpath);
            }
        }

        #region Utilities
        const string DatabaseNameParameter = "@databaseName";
        const string FilePathParameter = "@filePath{0}";

        const string DropDatabaseSql = @"
if exists (select * from sys.databases where name=N'{0}')
begin
  alter database [{0}] set single_user with rollback immediate;
  drop database [{0}];
end";

        static void InitializeServiceAndDatabase(SqlConnection connection)
        {
            // Set up the service and issue a request - Code First will create a database based on the model
            using (TestWebRequest request = CreateTestWebRequest("/"))
            {
                request.Accept = "application/atom+xml,application/xml";
                request.SendRequest();
                XmlDocument response = request.GetResponseStreamAsXmlDocument();

                TestUtil.AssertSelectNodes(response, "//app:collection[@href='Elevators']");
                TestUtil.AssertSelectNodes(response, "//app:collection[@href='FloorCalls']");
                TestUtil.AssertSelectNodes(response, "//app:collection[@href='ElevatorControlSystems']");
            }
        }

        static void DropDatabaseIfExists(SqlConnection connection)
        {
            string dropDatabaseSql = string.Format(CultureInfo.InvariantCulture, DropDatabaseSql, DatabaseName);
            using (SqlCommand command = new SqlCommand(dropDatabaseSql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        static TestWebRequest CreateTestWebRequest(string uriString)
        {
            TestWebRequest request = TestWebRequest.CreateForInProcess();
            request.ForceVerboseErrors = true;
            request.DataServiceType = ContextType;
            request.RequestUriString = uriString;
            return request;
        }
        
        static SqlConnection CreateAndOpenSqlConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = DataUtil.DefaultDataSource,
                InitialCatalog = "master",
                IntegratedSecurity = true
            };

            SqlConnection connection = new SqlConnection(builder.ToString());
            connection.Open();
            return connection;
        }

        #endregion
    }

    public class Elevator
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(5, MinimumLength=5)]
        public string SerialNumber { get; set; }

        public string Location { get; set; }

        public uint CurrentFloor { get; set; }
    }

    public class ElevatorControlSystem
    {
        public int ID { get; set; }

        public ICollection<Elevator> UpQueue { get; set; }

        public ICollection<Elevator> DownQueue { get; set; }

        public ICollection<FloorCall> FloorCalls { get; set; }
    }

    public class FloorCall
    {
        public int ID { get; set; }

        public int Floor { get; set; }

        public bool Up { get; set; }

        public bool Down { get; set; }
    }

    public class ElevatorDbContext : DbContext
    {
        public DbSet<Elevator> Elevators { get; set; }

        public DbSet<FloorCall> FloorCalls { get; set; }

        public DbSet<ElevatorControlSystem> ElevatorControlSystems { get; set; }

        public ElevatorDbContext()
        {
            if (this.Elevators.Count() == 0)
            {
                Elevator elevator1 = new Elevator { ID = 1, CurrentFloor = 10, Location = "NE", SerialNumber = "12345" };
                Elevator elevator2 = new Elevator { ID = 2, CurrentFloor = 2, Location = "NW", SerialNumber = "67890" };
                Elevator elevator3 = new Elevator { ID = 3, CurrentFloor = 3, Location = "E", SerialNumber = "02468" };
                this.Elevators.Add(elevator1);
                this.Elevators.Add(elevator2);
                this.Elevators.Add(elevator3);

                FloorCall call1 = new FloorCall { ID = 1, Floor = 9, Up = true };
                FloorCall call2 = new FloorCall { ID = 1, Floor = 3, Up = true, Down = true };
                this.FloorCalls.Add(call1);
                this.FloorCalls.Add(call2);

                ElevatorControlSystem system = new ElevatorControlSystem();
                system.UpQueue = new List<Elevator>();
                system.UpQueue.Add(elevator2);
                system.DownQueue = new List<Elevator>();
                system.DownQueue.Add(elevator1);
                system.FloorCalls = new List<FloorCall>();
                system.FloorCalls.Add(call1);
                system.FloorCalls.Add(call2);
                this.ElevatorControlSystems.Add(system);

                this.SaveChanges();
            }
        }
    }
}

