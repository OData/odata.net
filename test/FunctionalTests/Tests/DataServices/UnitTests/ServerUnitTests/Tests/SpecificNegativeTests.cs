//---------------------------------------------------------------------
// <copyright file="SpecificNegativeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// this attribute is specified elsewhere in this assembly as well, but
// included here to ensure that it is present as a test here requires it
[assembly: global::System.Data.Objects.DataClasses.EdmSchema]

namespace AstoriaUnitTests.Tests
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AstoriaUnitTests.Data;
    #endregion Namespaces

    [TestClass]
    public class SpecificNegativeTests
    {
        const string AtomNamespace = "http://www.w3.org/2005/Atom";

        // TODO: Re-enable server unit test: AstoriaUnitTests.Tests.SpecificNegativeTests.CodeFirstInEdmSchemaAssembly
        //[TestMethod]
        public void CodeFirstInEdmSchemaAssembly()
        {
            // This test attempts to use code first with entity classes in an assembly with the EdmSchema attribute applied to it.
            string databaseName = typeof(TestDbContext).FullName;
            using (SqlConnection connection = CreateAndOpenSqlConnection())
            {
                DropDatabaseIfExists(connection, databaseName);
            }

            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.ForceVerboseErrors = true;
                request.DataServiceType = typeof(TestDbContext);
                request.RequestUriString = "/";

                Exception e = TestUtil.RunCatching(request.SendRequest);
                Assert.IsNotNull(e, "An exception should have been caught. If EF now supports the scenario where code first classes " +
                    "can exist in an EdmSchema assembly, make the appropriate code changes and modify this test.");

                // The Code First pipeline uses CLR types to build the model, which in turn means that Code First knows the
                // assemblies used by the model. Therefore, Code First calls LoadFromAssembly for all these assemblies when
                // it creates the ObjectContext. DbContext makes a clone of this ObjectContext for use when running
                // the database initializer and calls LoadFromAssembly on this cloned context. What this means is that the
                // attributed o-space types are discovered because that’s the way LoadFromAssembly works. However, since these
                // o-space types don’t have any c-space types in the model the EF provider will raise an exception that when it
                // tries to match the o-space types to c-space types.
                // Our ObjectContextServiceProvider will recieve the following error from EF when it tries to get the ObjectContext from
                // the DbContext.
                Assert.AreEqual("Could not find the conceptual model type for 'AstoriaUnitTests.CompoundKeyContext.EdmCompoundKeyContext.Customer'.", e.Message);
            }

            using (SqlConnection connection = CreateAndOpenSqlConnection())
            {
                DropDatabaseIfExists(connection, databaseName);
            }
        }

        #region Utilities
        const string DatabaseNameParameter = "@databaseName";

        const string DropDatabaseSql = @"
if exists (select * from sys.databases where name=N'{0}')
begin
  alter database [{0}] set single_user with rollback immediate;
  drop database [{0}];
end";

        static void DropDatabaseIfExists(SqlConnection connection, string databaseName)
        {
            string dropDatabaseSql = string.Format(CultureInfo.InvariantCulture, DropDatabaseSql, databaseName);
            using (SqlCommand command = new SqlCommand(dropDatabaseSql, connection))
            {
                command.ExecuteNonQuery();
            }
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

    public class DbContextCustomer
    {
        public int ID { get; set; }

        public string Name { get; set; }
    }

    public class TestDbContext : DbContext
    {
        public DbSet<DbContextCustomer> Customers { get; set; }
    }
}

