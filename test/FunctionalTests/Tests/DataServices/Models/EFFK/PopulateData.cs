//---------------------------------------------------------------------
// <copyright file="PopulateData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.EFFK
{
    #region Namespaces

    using System;
    using System.Data.EntityClient;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Text;

    #endregion Namespaces

    public static class PopulateData
    {
#if USELOCALDB
        private static string ConnectionString = @"server=(LocalDB)\MSSQLLocalDB;integrated security=true;";
#else
        private static string ConnectionString = @"server=.\sqlexpress;integrated security=true;";
#endif

        private static EntityConnection entityConnection;

        public static EntityConnection EntityConnection
        {
            get
            {
                if (entityConnection == null)
                {
                    string connectionString = "metadata=res://Astoria.EFFKModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=e0f48a93b2ce01ce/;provider=System.Data.SqlClient;provider connection string=\"" + ConnectionString.Replace("\"", "\"\"") + "\";";
                    entityConnection = new EntityConnection(connectionString);
                }

                Trace.WriteLine("PopulateData.EntityConnection=" + entityConnection.ConnectionString);
                return entityConnection;
            }
            set
            {
                entityConnection = value;
            }
        }

        internal static EntityConnection CreateTableAndPopulateData(Type contextType)
        {
            // clear the old connection
            entityConnection = null;
            EntityConnection.Open();
            CreateTable((SqlConnection)EntityConnection.StoreConnection);
            CustomObjectContextPOCOProxy context = (CustomObjectContextPOCOProxy)Activator.CreateInstance(contextType);
            Populate(context);
            return EntityConnection;
        }

        public static void ClearConnection()
        {
            entityConnection = null;
        }

        private static void ExecuteSqlCommand(SqlConnection connection, string query)
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();
        }

        private static void Populate(CustomObjectContextPOCOProxy context)
        {
            Customer[] customers = new Customer[3];
            for (int i = 0; i < customers.Length; i++)
            {
                Customer customer = (i % 2 == 0) ? new Customer() : new CustomerWithBirthday();
                customer.ID = i;
                customer.Name = "Customer " + i.ToString();
                customer.Concurrency = i.ToString();

                customer.Address = new Address()
                {
                    StreetAddress = "Line1",
                    City = "Redmond",
                    State = "WA",
                    PostalCode = "98052"
                };

                if (customer.GetType() == typeof(CustomerWithBirthday))
                {
                    ((CustomerWithBirthday)customer).Birthday = DateTime.Now;
                }

                if (i > 0)
                {
                    customer.BestFriend = customers[i - 1];
                }

                Order o1 = new Order();
                o1.ID = i;
                o1.DollarAmount = 20.1;

                Order o2 = new Order();
                o2.ID = i + 100;
                o2.DollarAmount = 30.2;

                customer.Orders.Add(o1);
                customer.Orders.Add(o2);
                context.AddObject("CustomObjectContext.Customers", customer);
                customers[i] = customer;
            }

            CustomerBlob[] customerBlobs = new CustomerBlob[3];
            for (int i = 0; i < customerBlobs.Length; i++)
            {
                CustomerBlob customer = (i % 2 == 0) ? new CustomerBlob() : new CustomerBlobWithBirthday();
                customer.ID = i;
                customer.Name = "CustomerBlob " + i.ToString();
                customer.Concurrency = i.ToString();

                if (customer.GetType() == typeof(CustomerBlobWithBirthday))
                {
                    ((CustomerBlobWithBirthday)customer).Birthday = DateTime.Now;
                }

                context.AddObject("CustomObjectContext.CustomerBlobs", customer);
                customerBlobs[i] = customer;
            }

            context.SaveChanges();
        }

        private static void CreateTable(SqlConnection connection)
        {
            ExecuteSqlCommand(connection, @"create table #Customers(
                                            Id int primary key,
                                            Name nvarchar(max),
                                            Concurrency nchar(5),
                                            EditTimeStamp timestamp,
                                            Birthday datetime, 
                                            Address nvarchar(512), 
                                            City nvarchar(100),
                                            State nvarchar(100),
                                            PostalCode nvarchar(100),
                                            GuidValue uniqueidentifier,
                                            BestFriend int references #Customers(Id) null)");

            ExecuteSqlCommand(connection, @"create table #CustomerBlobs(
                                            Id int primary key,
                                            Name nvarchar(max),
                                            Concurrency nchar(5),
                                            EditTimeStamp timestamp,
                                            Birthday datetime, 
                                            Address nvarchar(512), 
                                            City nvarchar(100),
                                            State nvarchar(100),
                                            PostalCode nvarchar(100),
                                            GuidValue uniqueidentifier)");

            ExecuteSqlCommand(connection, @"create table #Orders(
                                            Id int primary key, 
                                            CustomerId int references #Customers(Id) null, 
                                            DollarAmount float)");

            ExecuteSqlCommand(connection, @"create table ""#Order Details""(
                                            OrderID int references #Orders(Id),
                                            ProductID int,
                                            UnitPrice money,
                                            Quantity smallint,
                                            Discount real,
                                            PRIMARY KEY (OrderID,ProductID)
                                            )");

            ExecuteSqlCommand(connection, @"create table #Offices(
                                            Id int primary key, 
                                            OfficeNumber int, 
                                            FloorNumber smallint, 
                                            BuildingName nvarchar(30))");

            ExecuteSqlCommand(connection, @"create table #Workers(
                                            Id int primary key references #Offices(Id), 
                                            FirstName nvarchar(30), 
                                            LastName nvarchar(30), 
                                            MiddleName nvarchar(30) null)");
        }

        internal static void DumpTable(string tableName)
        {
            SqlCommand command = new SqlCommand(String.Format("select * from {0}", tableName), (SqlConnection)EntityConnection.StoreConnection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        builder.AppendFormat("{0}=={1},", reader.GetName(i), reader.GetValue(i));
                    }

                    Trace.WriteLine(builder.ToString());
                }
            }
        }
    }
}
