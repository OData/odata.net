//---------------------------------------------------------------------
// <copyright file="CompoundKeyContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.CompoundKeyContext
{
    #region Namespaces

    using System;
    using System.Data.EntityClient;
    using System.Data.SqlClient;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Text;
    using AstoriaUnitTests.ObjectContextStubs.Hidden;

    #endregion Namespaces

    public static class CompoundKeyContext
    {
        private static string ConnectionString = @"server=" + DataUtil.DefaultDataSource + @";integrated security=true;";
        private static EntityConnection entityConnection;

        public static EntityConnection EntityConnection
        {
            get
            {
                if (entityConnection == null)
                {
                    string connectionString = "metadata=" + TestUtil.ServerUnitTestSamples + "\\stubs\\CompoundKeyContext\\;provider=System.Data.SqlClient;provider connection string=\"" + ConnectionString.Replace("\"", "\"\"") + "\";";
                    entityConnection = new EntityConnection(connectionString);
                }
                Trace.WriteLine("CompoundKeyContext.EntityConnection=" + entityConnection.ConnectionString);
                return entityConnection;
            }
            set
            {
                entityConnection = value;
            }
        }

        public static EntityConnection CreateTableAndPopulateData()
        {
            EntityConnection.Open();
            CreateTable((SqlConnection)entityConnection.StoreConnection);
            Populate((SqlConnection)entityConnection.StoreConnection);
            return EntityConnection;
        }

        private static void ExecuteSqlCommand(SqlConnection connection, string query)
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();
        }

        private static void CreateTable(SqlConnection connection)
        {
            ExecuteSqlCommand(connection, @"create table #Orders(
                                            bKey int NOT NULL,
                                            AKey int NOT NULL,
                                            Customer_Id int NOT NULL
                                            )");
            ExecuteSqlCommand(connection, @"create table #Customers(
                                            Id int NOT NULL
                                            )");
            ExecuteSqlCommand(connection, @"alter table #Orders
                                            add constraint PK_Orders
                                            primary key clustered (bKey, AKey ASC)
                                            ");
            ExecuteSqlCommand(connection, @"alter table #Customers
                                            add constraint PK_Customers
                                            primary key clustered (Id ASC)
                                            ");
            ExecuteSqlCommand(connection, @"alter table #Orders
                                            add constraint FK_CustomerOrder
                                            foreign key (Customer_Id)
                                            references Customers (Id)
                                            ");
        }

        private static void Populate(SqlConnection connection)
        {
        }
    }
}
