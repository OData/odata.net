//---------------------------------------------------------------------
// <copyright file="PopulateData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.ObjectContextStubs
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

    public static class PopulateData
    {
        private static string ConnectionString = @"server=" + DataUtil.DefaultDataSource + @";integrated security=true;";
        private static EntityConnection entityConnection;

        public static Action<Customer[], CustomObjectContext> CustomersCustomizer;

        public static EntityConnection EntityConnection
        {
            get
            {
                if (entityConnection == null)
                {
                    string connectionString = "metadata=" + TestUtil.ServerUnitTestSamples + "\\stubs\\CustomObjectContext\\;provider=System.Data.SqlClient;provider connection string=\"" + ConnectionString.Replace("\"", "\"\"") + "\";";
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

        public static EntityConnection CreateTableAndPopulateData()
        {
            // clear the old connection
            entityConnection = null;
            EntityConnection.Open();
            CreateTable((SqlConnection)EntityConnection.StoreConnection);
            Populate(new CustomObjectContext());
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

        private static void Populate(CustomObjectContext context)
        {
            Customer[] customers = new Customer[3];
            for (int i = 0; i < customers.Length; i++)
            {
                Customer customer = (i % 2 == 0) ? new Customer() : new CustomerWithBirthday();
                customer.ID = i;
                customer.Name = "Customer " + i.ToString();
                customer.Concurrency = i.ToString();
                customer.EditTimeStamp = new byte[] {2, 10};
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
                context.AddToCustomers(customer);
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

                context.AddToCustomerBlobs(customer);
                customerBlobs[i] = customer;
            }

            if (CustomersCustomizer != null)
            {
                CustomersCustomizer(customers, context);
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
                                            BestFriend int references Customers(Id) null)");

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
                                            CustomerId int references Customers(Id) null, 
                                            DollarAmount float)");
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

        public static EntityConnection CreateTableAndPopulateDataForManyToMany()
        {
            // clear the old connection
            entityConnection = null;
            EntityConnection.Open();
            CreateManyToManyTable((SqlConnection)EntityConnection.StoreConnection);
            Populate(new CustomManyToManyContainer());
            return EntityConnection;
        }

        public static EntityConnection CreateTableAndPopulateDataForEpm()
        {
            // clear the old connection
            entityConnection = null;
            EntityConnection.Open();
            CreateEpmTable((SqlConnection)EntityConnection.StoreConnection);
            Populate(new EpmCustomObjectContext());
            return EntityConnection;
        }

        private static void CreateManyToManyTable(SqlConnection connection)
        {
            ExecuteSqlCommand(connection, @"create table #Persons(
                                            Id int primary key,
                                            Name nvarchar(50))");

            ExecuteSqlCommand(connection, @"create table #Homes(
                                            Id int primary key,
                                            Address nvarchar(100))");

            ExecuteSqlCommand(connection, @"create table #person_home_link_table(
                                            PersonId int not null,
                                            HomeId int not null
                                            Constraint PK_person_home_link_table PRIMARY KEY (PersonId, HomeId))");
        }

        private static void CreateEpmTable(SqlConnection connection)
        {
            ExecuteSqlCommand(connection, @"create table #EpmCustomers(
                                            Id int primary key,
                                            Name nvarchar(max),
                                            Birthday datetime, 
                                            Address nvarchar(512), 
                                            City nvarchar(100),
                                            State nvarchar(100),
                                            PostalCode nvarchar(100))");

            ExecuteSqlCommand(connection, @"create table #EpmOrders(
                                            Id int primary key, 
                                            CustomerId int references Customers(Id) null, 
                                            DollarAmount float)");

            ExecuteSqlCommand(connection, @"create table #EpmProducts(
                                            ProductID int primary key, 
                                            ProductName nvarchar(40) not null,
                                            QuantityPerUnit nvarchar(20),
                                            UnitPrice money,
                                            UnitsInStock smallint,
                                            UnitsOnOrder smallint,
                                            ReorderLevel smallint,
                                            Discontinued bit not null)");
        }

        private static void Populate(CustomManyToManyContainer context)
        {
            for (int i = 0; i < 3; i++)
            {
                Person person = new Person();
                person.Id = i;
                person.Name = "Person " + i.ToString();

                Home o1 = new Home();
                o1.Id = i;
                o1.Address = "somehere in Hawaii";

                Home o2 = new Home();
                o2.Id = i + 100;
                o2.Address = "somewhere in Florida";

                person.Homes.Add(o1);
                person.Homes.Add(o2);
                context.AddToPersons(person);
            }
            context.SaveChanges();
        }

        private static void Populate(EpmCustomObjectContext context)
        {
            AstoriaUnitTests.ObjectContextStubs.Types.EpmCustomer[] customers = new AstoriaUnitTests.ObjectContextStubs.Types.EpmCustomer[3];
            for (int i = 0; i < customers.Length; i++)
            {
                AstoriaUnitTests.ObjectContextStubs.Types.EpmCustomer customer = (i % 2 == 0) ? new AstoriaUnitTests.ObjectContextStubs.Types.EpmCustomer() : new AstoriaUnitTests.ObjectContextStubs.Types.EpmCustomerWithBirthday();
                customer.ID = i;
                customer.Name = "Customer " + i.ToString();
                
                if (customer.GetType() == typeof(AstoriaUnitTests.ObjectContextStubs.Types.EpmCustomerWithBirthday))
                {
                    ((AstoriaUnitTests.ObjectContextStubs.Types.EpmCustomerWithBirthday)customer).Birthday = DateTime.Now;
                }

                AstoriaUnitTests.ObjectContextStubs.Types.EpmOrder o1 = new AstoriaUnitTests.ObjectContextStubs.Types.EpmOrder();
                o1.ID = i;
                o1.DollarAmount = 20.1;

                AstoriaUnitTests.ObjectContextStubs.Types.EpmOrder o2 = new AstoriaUnitTests.ObjectContextStubs.Types.EpmOrder();
                o2.ID = i + 100;
                o2.DollarAmount = 30.2;

                customer.Orders.Add(o1);
                customer.Orders.Add(o2);
                context.AddToEpmCustomers(customer);
                customers[i] = customer;
            }

            AstoriaUnitTests.ObjectContextStubs.Types.EpmProduct p = new AstoriaUnitTests.ObjectContextStubs.Types.EpmProduct();
            p.ProductID = 1;
            p.ProductName = "PotatoChips";
            p.Discontinued = false;
            context.AddToEpmProducts(p);

            context.SaveChanges();
        }
    }
}
