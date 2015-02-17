//---------------------------------------------------------------------
// <copyright file="CustomObjectContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.EFFK
{
    using System;
    using System.Linq;
    using System.Data.EntityClient;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Service;
    using System.IO;

    public class CustomObjectContextPOCOProxy : System.Data.Objects.ObjectContext, IDataServiceStreamProvider
    {
        /// <summary>
        /// Initializes a new CustomObjectContext object using the connection string found in the 'CustomObjectContext' section of the application configuration file.
        /// </summary>
        public CustomObjectContextPOCOProxy() :
            base(PopulateData.EntityConnection)
        {
            this.ContextOptions.ProxyCreationEnabled = true;
        }

        public static IDisposable CreateChangeScope()
        {
            PopulateData.CreateTableAndPopulateData(typeof(CustomObjectContextPOCOProxy));
            return new CustomObjectContextChangeScope();
        }

        protected class CustomObjectContextChangeScope : IDisposable
        {
            public void Dispose()
            {
                PopulateData.ClearConnection();
            }
        }

        public static readonly string DummyContentType = "DummyType/DummySubType";
        public static readonly Uri DummyReadStreamUri = new Uri("http://localhost/dummyuri/", UriKind.Absolute);
        #region IDataServiceStreamProvider Members

        public int StreamBufferSize
        {
            get { return 1; }
        }

        public Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            throw new NotImplementedException();
        }

        public Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            throw new NotImplementedException();
        }

        public void DeleteStream(object entity, DataServiceOperationContext operationContext)
        {
            throw new NotImplementedException();
        }

        public string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
        {
            return DummyContentType;
        }

        public Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
        {
            return DummyReadStreamUri;
        }

        public string GetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            return null;
        }

        public string ResolveType(string entitySetName, DataServiceOperationContext operationContext)
        {
            if (entitySetName == "CustomerBlobs")
            {
                return typeof(CustomerBlob).Name;
            }
            else
            {
                throw new ArgumentException("Unrecognized entity set name", "entitySetName");
            }
        }

        #endregion
    }

    public class CustomObjectContextPOCO : CustomObjectContextPOCOProxy
    {
        /// <summary>
        /// Initializes a new CustomObjectContext object using the connection string found in the 'CustomObjectContext' section of the application configuration file.
        /// </summary>
        public CustomObjectContextPOCO() :
            base()
        {
            this.ContextOptions.ProxyCreationEnabled = false;
        }

        public new static IDisposable CreateChangeScope()
        {
            PopulateData.CreateTableAndPopulateData(typeof(CustomObjectContextPOCO));
            return new CustomObjectContextChangeScope();
        }
    }

    public class Customer
    {
        public Customer()
        {
            this.Orders = new HashSet<Order>();
            this.Address = new Address()
            {
                StreetAddress = "Line1",
                City = "Redmond",
                State = "WA",
                PostalCode = "98052"
            };
        }

        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Concurrency { get; set; }
        public virtual byte[] EditTimeStamp { get; set; }
        public virtual Guid? GuidValue { get; set; }
        public virtual Address Address { get; set; }
        public virtual HashSet<Order> Orders { get; private set; }
        public virtual Customer BestFriend { get; set; }
    }

    public class Order
    {
        public virtual int ID { get; set; }
        public virtual double DollarAmount { get; set; }
        public virtual int? CustomerId { get; set; }
        public virtual Customer Customers { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }
    }

    public class OrderDetail
    {
        public virtual int OrderID { get; set; }
        public virtual int ProductID { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual Int16 Quantity { get; set; }
        public virtual Single Discount { get; set; }
        public virtual Order Order { get; set; }
    }

    public class CustomerWithBirthday : Customer
    {
        public CustomerWithBirthday()
            : base()
        {
            // This is what EF generated code does to make sure the default datetime is a valid value in SQL
            this.Birthday = new DateTime(624235248000000000, DateTimeKind.Unspecified);
        }

        public virtual DateTime Birthday { get; set; }
    }

    public class Address
    {
        public virtual string StreetAddress { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string PostalCode { get; set; }
    }

    public class CustomerBlob
    {
        public CustomerBlob()
        {
            this.Address = new Address()
            {
                StreetAddress = "Line1",
                City = "Redmond",
                State = "WA",
                PostalCode = "98052"
            };
        }

        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Concurrency { get; set; }
        public virtual byte[] EditTimeStamp { get; set; }
        public virtual Guid? GuidValue { get; set; }
        public virtual Address Address { get; set; }
    }

    public class CustomerBlobWithBirthday : CustomerBlob
    {
        public virtual DateTime Birthday { get; set; }
    }

    public class Office
    {
        public virtual Int32 ID { get; set; }
        public virtual Int32 OfficeNumber { get; set; }
        public virtual Int16 FloorNumber { get; set; }
        public virtual String BuildingName { get; set; }
        public virtual Worker Worker { get; set; }
    }

    public class Worker
    {
        public virtual Int32 ID { get; set; }
        public virtual String FirstName { get; set; }
        public virtual String LastName { get; set; }
        public virtual String MiddleName { get; set; }
        public virtual Office Office { get; set; }
    }
}

