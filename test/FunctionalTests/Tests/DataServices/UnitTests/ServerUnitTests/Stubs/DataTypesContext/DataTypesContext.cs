//---------------------------------------------------------------------
// <copyright file="DataTypesContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.DataTypesContext
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

    public static class DataTypesContext
    {
        private static string ConnectionString = @"server=" + DataUtil.DefaultDataSource + @";integrated security=true;";
        private static EntityConnection entityConnection;

        public static EntityConnection EntityConnection
        {
            get
            {
                if (entityConnection == null)
                {
                    string connectionString = "metadata=" + TestUtil.ServerUnitTestSamples + "\\stubs\\DataTypesContext\\;provider=System.Data.SqlClient;provider connection string=\"" + ConnectionString.Replace("\"", "\"\"") + "\";";
                    entityConnection = new EntityConnection(connectionString);
                }
                Trace.WriteLine("DataTypesContext.EntityConnection=" + entityConnection.ConnectionString);
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
            ExecuteSqlCommand(connection, @"create table #DataTypes(
                                            ID int NOT NULL primary key,
                                            Integer int NULL,
                                            Float float(53) NULL,
                                            Decimal decimal(18, 0) NULL, 
                                            String nvarchar(MAX) NULL, 
                                            Date datetime NULL,
                                            Boolean bit NULL,
                                            UniqueId uniqueidentifier NULL 
                                            )");
        }

        private static void Populate(SqlConnection connection)
        {
            ExecuteSqlCommand(connection, @"INSERT INTO #DataTypes ( [ID], [Integer], [Float], [Decimal], [String], [Date], [Boolean], [UniqueId] ) " +
                @"VALUES ( 1, 42, 1.234, 128, 'Full', '1/1/2007', 'False', '{41500D96-1A29-4723-B8FC-88812A2BA5F1}' )");
            ExecuteSqlCommand(connection, @"INSERT INTO #DataTypes ( [ID], [Integer], [Float], [Decimal], [String], [Date], [Boolean], [UniqueId] ) " +
                @"VALUES ( 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL )");
            ExecuteSqlCommand(connection, @"INSERT INTO #DataTypes ( [ID], [Integer], [Float], [Decimal], [String], [Date], [Boolean], [UniqueId] ) " +
                @"VALUES ( 3, 100, 5, 512, 'Large', '2/2/2008', 'True', '{31500D96-1A29-4723-B8FC-88812A2BA5F1}' )");
            //DataType[] dts = new DataType[] {
            //    new DataType() { ID = 1, Integer = 42, Float = 1.234, Decimal = 128, String = "Full", 
            //        Date = DateTime.Parse("1/1/2007", System.Globalization.CultureInfo.InvariantCulture),
            //        Boolean = false, UniqueId = new Guid("{41500D96-1A29-4723-B8FC-88812A2BA5F1}")
            //    },
            //    new DataType() { ID = 2, Integer = null, Float = null, Decimal = null, String = null,
            //        Date = null,
            //        Boolean = null, UniqueId = null
            //    },
            //    new DataType() { ID = 3, Integer = 100, Float = 5, Decimal = 512, String = "Large",
            //        Date = DateTime.Parse("2/2/2008", System.Globalization.CultureInfo.InvariantCulture),
            //        Boolean = true, UniqueId = new Guid("{31500D96-1A29-4723-B8FC-88812A2BA5F1}")
            //    }
            //};

            //ctx.DataTypes.AttachAll(dts);
            //ctx.SubmitChanges();

            //DataTypes[] dts = new DataTypes[] {
            //    new DataTypes() { ID = 1, Integer = 42, Float = 1.234, Decimal = 128, String = "Full", 
            //        Date = DateTime.Parse("1/1/2007", System.Globalization.CultureInfo.InvariantCulture),
            //        Boolean = false, UniqueId = new Guid("{41500D96-1A29-4723-B8FC-88812A2BA5F1}")
            //    },
            //    new DataTypes() { ID = 2, Integer = null, Float = null, Decimal = null, String = null,
            //        Date = null,
            //        Boolean = null, UniqueId = null
            //    },
            //    new DataTypes() { ID = 3, Integer = 100, Float = 5, Decimal = 512, String = "Large",
            //        Date = DateTime.Parse("2/2/2008", System.Globalization.CultureInfo.InvariantCulture),
            //        Boolean = true, UniqueId = new Guid("{31500D96-1A29-4723-B8FC-88812A2BA5F1}")
            //    }
            //};
            //foreach (DataTypes dt in dts)
            //{
            //    ctx.AddToDataTypes(dt);
            //}
            //ctx.SaveChanges();
        }
    }
}
