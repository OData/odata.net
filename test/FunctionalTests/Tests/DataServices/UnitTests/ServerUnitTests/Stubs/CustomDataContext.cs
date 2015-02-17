//---------------------------------------------------------------------
// <copyright file="CustomDataContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using AstoriaUnitTests.StubsOtherNs;
    using AstoriaUnitTests.Tests;
    using Microsoft.OData.Service.Providers;

    #endregion Namespaces

    public class Address
    {
        private string streetAddress;
        private string city;
        private string state;
        private string postalCode;

        public string StreetAddress
        {
            get { return this.streetAddress; }
            set { this.streetAddress = value; }
        }

        public string City
        {
            get { return this.city; }
            set { this.city = value; }
        }

        public string State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        public string PostalCode
        {
            get { return this.postalCode; }
            set { this.postalCode = value; }
        }
    }

    public class CurrencyAmount
    {
        public decimal Amount { get; set; }
        public string CurrencyName { get; set; }
    }

    public class Order : IEquatable<Order>
    {
        private int id;
        private double dollarAmount;
        private ListTracking<OrderDetail> orderDetails;

        public Order()
        {
            this.orderDetails = new ListTracking<OrderDetail>();
        }

        public Order(int id, double dollarAmount)
            : this()
        {
            this.id = id;
            this.dollarAmount = dollarAmount;
        }

        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public double DollarAmount
        {
            get { return this.dollarAmount; }
            set { this.dollarAmount = value; }
        }

        public Customer Customer
        {
            get;
            set;
        }

        public CurrencyAmount CurrencyAmount { get; set; }

        public ListTracking<OrderDetail> OrderDetails
        {
            get { return this.orderDetails; }
            set { this.orderDetails = (ListTracking<OrderDetail>)value; }
        }

        #region IEquatable<Order> Members

        public bool Equals(Order other)
        {
            return (this.ID == other.ID);
        }

        #endregion

        public void Clone(object resource)
        {
            Order newOrder = resource as Order;

            if (newOrder == null)
            {
                throw new Exception("Type mismatch in Clone");
            }

            if (this.ID != newOrder.ID)
            {
                throw new Exception("Key Values do not match in Clone");
            }

            newOrder.DollarAmount = this.DollarAmount;
            newOrder.OrderDetails = this.OrderDetails;
        }
    }

    [ETag("GuidValue")]
    [MimeType("NameAsHtml", "text/html")]
    public class Customer : IEquatable<Customer>
    {
        private Address address;
        private int id;
        private string name;
        private ICollection<Order> orders;
        private Customer bestFriend;
        private Type accessException;
        private Guid guidValue;

        public Customer()
        {
            this.address = new Address();
            this.address.StreetAddress = "Line1";
            this.address.City = "Redmond";
            this.address.State = "WA";
            this.address.PostalCode = "98052";
            this.orders = new ListTracking<Order>();
            this.ModifyTimestamp();
            // Trace.WriteLine(String.Format("New Customer object created with timestamp '{0}'", this.guidValue));
        }

        public Customer BestFriend
        {
            get { return this.bestFriend; }
            set
            {
                this.bestFriend = value;
            }
        }

        public Address Address
        {
            get { return this.address; }
            set { this.address = value; }
        }

        public Guid GuidValue
        {
            get { return this.guidValue; }
            set { this.guidValue = value; }
        }

        public int ID
        {
            get
            {
                if (this.AccessException != null)
                {
                    if (this.AccessException == typeof(DataServiceException))
                    {
                        throw new DataServiceException(400, "Customer.ID", "Customer.ID accessed", "en-GB",
                            new InvalidOperationException("This exception purposefully thrown."));
                    }

                    throw (Exception)Activator.CreateInstance(this.AccessException);
                }

                return this.id;
            }

            set { this.id = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string NameAsHtml
        {
            get { return "<html><body>" + this.Name + "</body></html>"; }
            set { }
        }

        public ICollection<Order> Orders
        {
            get { return this.orders; }
            set { this.orders = value; }
        }

        /// <summary>Type of exception to be thrown on key accessor.</summary>
        internal Type AccessException
        {
            get { return this.accessException; }
            set { this.accessException = value; }
        }

        internal void ModifyTimestamp()
        {
            this.guidValue = Guid.NewGuid();
        }

        #region IEquatable<Customer> Members

        public bool Equals(Customer other)
        {
            return (this.ID == other.ID);
        }

        #endregion

        public virtual void Clone(object resource)
        {
            Customer newCustomer = resource as Customer;

            if (newCustomer == null)
            {
                throw new Exception("Type mismatch in Clone");
            }

            if (this.ID != newCustomer.ID)
            {
                throw new Exception("Key Values do not match in Clone");
            }

            newCustomer.Address = this.Address;
            newCustomer.BestFriend = this.BestFriend;
            newCustomer.GuidValue = this.GuidValue;
            newCustomer.Name = this.Name;
            newCustomer.NameAsHtml = this.NameAsHtml;
            newCustomer.Orders = this.Orders;
        }
    }

    public class CustomerWithBirthday : Customer
    {
        private DateTimeOffset birthday;

        public CustomerWithBirthday()
            : base()
        {
            this.birthday = DateTime.Today.AddYears(-30);
        }

        public DateTimeOffset Birthday
        {
            get { return this.birthday; }
            set { this.birthday = value; }
        }

        public override void Clone(object resource)
        {
            base.Clone(resource);
            CustomerWithBirthday newCustomer = resource as CustomerWithBirthday;
            if (newCustomer == null)
            {
                throw new Exception("Type mismatch in Clone");
            }

            newCustomer.Birthday = this.Birthday;
        }
    }

    public class CustomerWithoutProperties : CustomerWithBirthday
    {
    }

    public class Product : IEquatable<Product>
    {
        public Product()
        {
            this.OrderDetails = new ListTracking<OrderDetail>();
        }

        public Product(int id, string productName, bool discontinued)
            : this()
        {
            this.ID = id;
            this.ProductName = productName;
            this.Discontinued = discontinued;
        }

        public int ID { get; set; }
        public string ProductName { get; set; }
        public bool Discontinued { get; set; }
        public ListTracking<OrderDetail> OrderDetails { get; set; }

        #region IEquatable<Product> Members

        public bool Equals(Product other)
        {
            return (this.ID == other.ID);
        }

        #endregion

        public void Clone(object resource)
        {
            Product newProduct = resource as Product;

            if (newProduct == null)
            {
                throw new Exception("Type mismatch in Clone");
            }

            if (this.ID != newProduct.ID)
            {
                throw new Exception("Key Values do not match in Clone");
            }

            newProduct.ID = this.ID;
            newProduct.ProductName = this.ProductName;
            newProduct.Discontinued = this.Discontinued;
            newProduct.OrderDetails = this.OrderDetails;
        }
    }

    [Key("OrderID", "ProductID")]
    public class OrderDetail : IEquatable<OrderDetail>
    {
        public OrderDetail()
        {
        }

        public OrderDetail(int orderID, int productID, double unitPrice, short quantity)
        {
            this.OrderID = orderID;
            this.ProductID = productID;
            this.UnitPrice = unitPrice;
            this.Quantity = quantity;
        }

        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public double UnitPrice { get; set; }
        public short Quantity { get; set; }

        #region IEquatable<OrderDetail> Members

        public bool Equals(OrderDetail other)
        {
            return (this.OrderID == other.OrderID && this.ProductID == other.ProductID);
        }

        #endregion

        public void Clone(object resource)
        {
            OrderDetail newOrderDetails = resource as OrderDetail;

            if (newOrderDetails == null)
            {
                throw new Exception("Type mismatch in Clone");
            }

            if (this.OrderID != newOrderDetails.OrderID || this.ProductID != newOrderDetails.ProductID)
            {
                throw new Exception("Key Values do not match in Clone");
            }

            newOrderDetails.OrderID = this.OrderID;
            newOrderDetails.ProductID = this.ProductID;
            newOrderDetails.UnitPrice = this.UnitPrice;
            newOrderDetails.Quantity = this.Quantity;
        }
    }

    public class EmptySetEntity
    {
        public int ID { get; set; }
    }

    /// <summary>A custom in-memory data context suitable for fast tests.</summary>
    /// <remarks>
    /// The schema has Customers, Orders, Regions, Products and OrderDetails.
    /// Customers.Orders ->> Orders
    /// Customers.BestFriend -> Customers
    /// Orders.OrderDetails ->> OrderDetails
    /// Products.OrderDetails ->> OrderDetails
    /// 
    /// Interesting bits:
    /// - Customer has CustomerWithBirthday as a subtype for some elements.
    /// - Customer has Address complex type.
    /// - Customers may throw an exception when accessed, if configured to do so.
    /// - Changes may be ignored or preserved.
    /// - Data is populated on demand, with customers 1-n (every other one with birthday),
    ///   two orders per customer (with id #custid and #custid+100), BestFriend pointing
    ///   to previous customer starting from id #2.
    /// </remarks>
    [IgnoreProperties("CustomerNames")]
    public class CustomDataContext : IUpdatable
    {
        private static object[] lastParameters;
        public static List<Customer> customers;
        private static List<Order> orders;
        private static List<Region> regions;
        private static List<Product> products;
        private static List<OrderDetail> orderDetails;
        private List<KeyValuePair<object, EntityState>> pendingChanges;
        private static bool? preserveChanges;

        public static Action<CustomDataContext> ModifyDefaultData;
        public Dictionary<int, ResourceInfo> tokens = new Dictionary<int, ResourceInfo>();

        /// <summary>Creates data if the internal lists haven't been initialized yet.</summary>
        private void CreateDataIfEmpty()
        {
            if (CustomDataContext.PreserveChanges == false || customers == null)
            {
                // Trace.WriteLine("Creating data for CustomDataContext in AppDomain #" + AppDomain.CurrentDomain.Id);
                CreateDefaultData();

                if (ModifyDefaultData != null)
                {
                    ModifyDefaultData(this);
                }

                AddExceptionTriggers();
            }

            if (customers != null)
            {
                Debug.Assert(orders != null, "orders != null");
                Debug.Assert(regions != null, "regions != null");
                Debug.Assert(products != null, "products != null");
                Debug.Assert(orderDetails != null, "orderDetails != null");
                return;
            }
        }

        private static void AddExceptionTriggers()
        {
            Type exceptionType;
            if (TestWebRequest.TryGetTestArgument(ExceptionTypeArgument, out exceptionType))
            {
                bool exceptionAtEnd;
                if (!TestWebRequest.TryGetTestArgument(ExceptionAtEndArgument, out exceptionAtEnd))
                {
                    exceptionAtEnd = false;
                }

                if (exceptionAtEnd)
                {
                    customers.Last().AccessException = exceptionType;
                }
                else
                {
                    customers.First().AccessException = exceptionType;
                }
            }
        }

        private static void CreateDefaultData()
        {
            // Trace.WriteLine("CreateDefaultData called - " + new StackTrace().ToString());
            CustomDataContext.customers = null;
            CustomDataContext.orders = null;
            CustomDataContext.regions = null;
            CustomDataContext.products = null;
            CustomDataContext.orderDetails = null;

            LocalWebClearPending = true;
            int customerCount;
            int orderCount;
            int odCount;

            if (!TestWebRequest.TryGetTestArgument<int>(CustomerCountArgument, out customerCount))
            {
                customerCount = 3;
            }
            if (!TestWebRequest.TryGetTestArgument<int>(OrderCountArgument, out orderCount))
            {
                orderCount = 2;
            }
            if (!TestWebRequest.TryGetTestArgument<int>(OrderDetailsCountArgument, out odCount))
            {
                odCount = 1;
            }

            customers = new List<Customer>(customerCount);
            orders = new List<Order>();
            products = new List<Product>();
            orderDetails = new List<OrderDetail>();

            for (int i = 0; i < customerCount; i++)
            {
                Customer customer = (i % 2 == 0) ? new Customer() : new CustomerWithBirthday();
                customer.ID = i;
                customer.Name = "Customer " + i.ToString();

                for (int j = 0; j < orderCount; j++)
                {
                    int orderID = i + 100 * j;
                    double orderDollarAmount = Math.Round(20.1 + 10.1 * j, 2);

                    Order o = new Order(orderID, orderDollarAmount);

                    customer.Orders.Add(o);
                    orders.Add(o);
                    o.Customer = customer;

                    for (int k = 0; k < odCount; k++)
                    {
                        int productID = i + 100 * j + 1000 * k;
                        Product p = new Product(productID, "Product #" + (productID).ToString(), false);
                        products.Add(p);

                        OrderDetail od = new OrderDetail(orderID, productID, orderDollarAmount, 1);
                        o.OrderDetails.Add(od);
                        p.OrderDetails.Add(od);
                        orderDetails.Add(od);
                    }
                }

                if (i > 0)
                {
                    customer.BestFriend = customers[i - 1];
                }

                customers.Add(customer);
            }

            regions = new List<Region>{
                    new Region(1, "Eastern"),
                    new Region(2, "Western")
                };
        }

        public List<Customer> InternalCustomersList
        {
            get
            {
                if (customers == null)
                {
                    CreateDataIfEmpty();
                }
                return customers;
            }
            set
            {
                customers = value;
            }
        }

        /// <summary>Whether clearing data is pending on a different AppDomain.</summary>
        public static bool LocalWebClearPending { get; set; }

        public static bool PreserveChanges
        {
            get
            {
                bool preserveChangesArgument;
                if (TestWebRequest.TryGetTestArgument<bool>(PreserveChangesArgument, out preserveChangesArgument))
                {
                    return preserveChangesArgument;
                }
                else if (CustomDataContext.preserveChanges.HasValue)
                {
                    return CustomDataContext.preserveChanges.Value;
                }
                else
                {
                    return false;
                }
            }
            set { CustomDataContext.preserveChanges = value; }
        }

        public IQueryable<Customer> Customers
        {
            get
            {
                CreateDataIfEmpty();
                // Trace.WriteLine("Accessing Customers from AppDomain #" + AppDomain.CurrentDomain.Id + " with " + customers.Count + " customers.");
                return customers.AsQueryable<Customer>();
            }
        }

        /// <summary>
        /// Use this to test empty set behaviour
        /// </summary>
        public IQueryable<EmptySetEntity> EmptySet
        {
            get { return new EmptySetEntity[0].AsQueryable(); }
        }

        public IQueryable<Order> Orders
        {
            get
            {
                CreateDataIfEmpty();
                return orders.AsQueryable();
            }
        }

        public IQueryable<string> CustomerNames
        {
            get
            {
                return new string[] { "Customer A", "Customer B", "Customer C" }.AsQueryable<string>();
            }
        }

        public IQueryable<Region> Regions
        {
            get
            {
                CreateDataIfEmpty();
                return regions.AsQueryable();
            }
        }

        public IQueryable<Product> Products
        {
            get
            {
                CreateDataIfEmpty();
                return products.AsQueryable();
            }
        }

        public IQueryable<OrderDetail> OrderDetails
        {
            get
            {
                CreateDataIfEmpty();
                return orderDetails.AsQueryable();
            }
        }

        internal List<KeyValuePair<object, EntityState>> PendingChanges
        {
            get
            {
                if (pendingChanges == null)
                {
                    pendingChanges = new List<KeyValuePair<object, EntityState>>();
                }

                return pendingChanges;
            }
        }

        /// <summary>Test argument used to control number of customers in context.</summary>
        public const string CustomerCountArgument = "CustomDataContext.CustomerCount";

        /// <summary>Test argument used to control number of orders per customer in context.</summary>
        public const string OrderCountArgument = "CustomDataContext.OrderCountArgument";

        /// <summary>Test argument used to control number of orders per customer in context.</summary>
        public const string OrderDetailsCountArgument = "CustomDataContext.OrderDetailsCountArgument";

        /// <summary>
        /// Test argument used to control the type of exception to be thrown 
        /// by a customer accesor.
        /// </summary>
        /// <remarks>If missing, no exception is thrown.</remarks>
        public const string ExceptionTypeArgument = "CustomDataContext.ExceptionType";

        /// <summary>
        /// Test argument used to control whether the customer throwing an 
        /// exception is at the end of the list.
        /// </summary>
        public const string ExceptionAtEndArgument = "CustomDataContext.ExceptionAtEnd";

        /// <summary>
        /// Test argument used to control whether changes should be preserved
        /// from one invocation to the next.
        /// </summary>
        public const string PreserveChangesArgument = "CustomDataContext.PreserveChangesArgument";

        /// <summary>Refreshes the data context and starts preserving changes.</summary>
        /// <returns>An object that can be disposed to end the scope.</returns>
        /// <remarks>
        /// The pattern of usage for this method is as follows:
        /// <code>
        /// using (CustomDataContext.CreateChangeScope) {
        ///   // Changes will be preserved here.
        /// }
        /// </code>
        /// </remarks>
        public static IDisposable CreateChangeScope()
        {
            if (preserveChanges.HasValue && preserveChanges.Value)
            {
                throw new InvalidOperationException("Changes are already being preserved.");
            }

            ClearData();
            preserveChanges = true;
            return new CustomDataContextPresereChangesScope();
        }

        #region Helper methods for ServiceOperation code gen.

        public void VoidSingleMethod()
        {
        }

        public void VoidMultipleMethod()
        {
        }

        public string StringSingleMethod()
        {
            return "String";
        }

        public IEnumerable<string> StringMultipleMethod()
        {
            return new string[] { "s0", "s1" };
        }

        public IEnumerable<string> StringEnumerableSingleMethod()
        {
            return new string[] { "String0" };
        }

        public IEnumerable<string> StringEnumerableMultipleMethod()
        {
            return new string[] { "String0", "String1" };
        }

        public IQueryable<string> StringQueryableSingleMethod()
        {
            return new List<string>(StringEnumerableSingleMethod()).AsQueryable();
        }

        public IQueryable<string> StringQueryableMultipleMethod()
        {
            return new List<string>(StringEnumerableMultipleMethod()).AsQueryable();
        }

        public Customer CustomerSingleMethod()
        {
            return this.Customers.First();
        }

        public IQueryable<Customer> CustomerMultipleMethod()
        {
            return null;
        }

        public IEnumerable<Customer> CustomerEnumerableSingleMethod()
        {
            return this.Customers.Take(1);
        }

        public IEnumerable<Customer> CustomerEnumerableMultipleMethod()
        {
            return this.Customers.Take(3);
        }

        public IQueryable<Customer> CustomerQueryableSingleMethod()
        {
            return this.Customers.AsQueryable().Take(1);
        }

        public IQueryable<Customer> CustomerQueryableMultipleMethod()
        {
            return this.Customers.AsQueryable().Take(3);
        }

        #endregion Helper methods for ServiceOperation code gen.

        public static void SetLastParameters(object[] values)
        {
            lastParameters = values;
            if (lastParameters != null)
            {
                lastParameters = (object[])lastParameters.Clone();
            }
        }

        internal static object[] LastParameters
        {
            get
            {
                return (object[])lastParameters.Clone();
            }
        }

        #region IUpdatable Members

        public virtual object CreateResource(string containerName, string fullTypeName)
        {
            object objectToBeAdded = null;

            if (containerName == "Customers")
            {
                if (fullTypeName == typeof(Customer).FullName)
                {
                    objectToBeAdded = new Customer();
                }
                else if (fullTypeName == typeof(CustomerWithBirthday).FullName)
                {
                    objectToBeAdded = new CustomerWithBirthday();
                }
                else if (fullTypeName == typeof(CustomerWithoutProperties).FullName)
                {
                    objectToBeAdded = new CustomerWithoutProperties();
                }
            }
            else if (containerName == "Orders")
            {
                if (fullTypeName == typeof(Order).FullName)
                {
                    objectToBeAdded = new Order();
                }
            }
            else
                if (containerName == "Regions")
                {
                    if (fullTypeName == typeof(Region).FullName)
                    {
                        objectToBeAdded = new Region();
                    }
                }
                else if (containerName == "Products")
                {
                    if (fullTypeName == typeof(Product).FullName)
                    {
                        objectToBeAdded = new Product();
                    }
                }
                else if (containerName == "OrderDetails")
                {
                    if (fullTypeName == typeof(OrderDetail).FullName)
                    {
                        objectToBeAdded = new OrderDetail();
                    }
                }
                else if (fullTypeName == typeof(Address).FullName)
                {
                    objectToBeAdded = new Address();
                }
                else if (fullTypeName == typeof(CurrencyAmount).FullName)
                {
                    objectToBeAdded = new CurrencyAmount();
                }

            if (objectToBeAdded == null)
            {
                throw new Exception(String.Format("Invalid container name : '{0}' or invalid type name: '{1}'", containerName, fullTypeName));
            }


            if (containerName != null)
            {
                this.AddCreatedObject(objectToBeAdded);
            }

            return this.CreateToken(objectToBeAdded);
        }

        protected void AddCreatedObject(object objectToBeAdded)
        {
            this.PendingChanges.Add(new KeyValuePair<object, EntityState>(objectToBeAdded, EntityState.Added));
        }

        protected object CreateToken(object objectToBeAdded)
        {
            int token = this.tokens.Count;
            this.tokens.Add(token, new ResourceInfo(objectToBeAdded));
            return token;
        }

        public object GetResource(IQueryable query, string fullTypeName)
        {
            object resource = null;

            foreach (object r in query)
            {
                if (resource != null)
                {
                    throw new ArgumentException(String.Format("Invalid Uri specified. The query '{0}' must refer to a single resource", query.ToString()));
                }

                resource = r;
            }

            if (resource != null)
            {
                if (fullTypeName != null && resource.GetType().FullName.Replace('+', '_') != fullTypeName)
                {
                    throw new System.ArgumentException(String.Format("Invalid uri specified. ExpectedType: '{0}', ActualType: '{1}'", fullTypeName, resource.GetType().FullName));
                }

                int token = this.tokens.Count;
                this.tokens.Add(token, new ResourceInfo(resource));
                return token;
            }

            return null;
        }

        public object ResetResource(object resource)
        {
            Debug.Assert(resource != null, "resource != null");
            resource = this.tokens[(int)resource].Resource;

            Type type = resource.GetType();
            object newResource = type.GetConstructor(Type.EmptyTypes).Invoke(null);
            if (typeof(Customer).IsAssignableFrom(type))
            {
                ((Customer)newResource).ID = ((Customer)resource).ID;
            }
            else if (typeof(Order).IsAssignableFrom(type))
            {
                ((Order)newResource).ID = ((Order)resource).ID;
            }
            else if (typeof(Product).IsAssignableFrom(type))
            {
                ((Product)newResource).ID = ((Product)resource).ID;
            }
            else if (typeof(OrderDetail).IsAssignableFrom(type))
            {
                ((OrderDetail)newResource).OrderID = ((OrderDetail)resource).OrderID;
                ((OrderDetail)newResource).ProductID = ((OrderDetail)resource).ProductID;
            }
            else
            {
                throw new Exception("Unknown Type : " + type.FullName);
            }

            // find the entity set for the object
            IList entitySetInstance = GetEntitySet(type);
            DeleteEntity(entitySetInstance, resource, true /*throwIfNotPresent*/);
            AddResource(newResource, true /*throwIfDuplicate*/);
            this.tokens.Add(tokens.Count, new ResourceInfo(newResource));
            return this.tokens.Count - 1;
        }

        public object GetValue(object token, string propertyName)
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(token, typeof(int));
            object targetResource = this.tokens[(int)token].Resource;
            PropertyInfo propertyInfo = targetResource.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            object propertyValue = propertyInfo.GetValue(targetResource, null);
            if (!IsPrimitiveType(propertyInfo.PropertyType) && propertyValue != null)
            {
                if (!this.tokens.Exists(e => e.Value.Resource == propertyValue))
                {
                    int newToken = this.tokens.Count;
                    this.tokens.Add(newToken, new ResourceInfo(propertyValue));
                    propertyValue = newToken;
                }
                else
                {
                    propertyValue = this.tokens.Single(e => e.Value.Resource == propertyValue).Key;
                }
            }

            return propertyValue;
        }

        public void SetValue(object token, string propertyName, object propertyValue)
        {
            ResourceInfo ri = this.tokens[(int)token];
            if (!ri.CanPrimitivePropertyBeUpdated)
            {
                throw new DataServiceException(500, "Primitive Properties must be updated before Nav Properties");
            }
            object targetResource = ri.Resource;
            PropertyInfo property = targetResource.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);


            if (IsPrimitiveType(property.PropertyType) ||
                propertyValue == null)
            {
                property.SetValue(targetResource, propertyValue, null);
            }
            else
            {
                object actualPropertyValue = this.tokens[(int)propertyValue].Resource;
                property.SetValue(targetResource, actualPropertyValue, null);
            }

            if (targetResource.GetType().IsAssignableFrom(typeof(Customer)))
            {
                Customer customer = (Customer)targetResource;
                customer.ModifyTimestamp();
                // Trace.WriteLine(String.Format("'{0}' got modified. New timestamp value: '{1}'", propertyName, customer.GuidValue));
            }
        }

        public void SetReference(object token, string propertyName, object propertyValueToken)
        {
            ResourceInfo ri = this.tokens[(int)token];
            object declaringResource = ri.Resource;
            ri.CanPrimitivePropertyBeUpdated = false;
            object propertyValue;
            if (propertyValueToken == null)
            {
                propertyValue = null;
            }
            else
            {
                propertyValue = this.tokens[(int)propertyValueToken].Resource;
            }

            if (propertyValue != null && !typeof(Customer).IsAssignableFrom(propertyValue.GetType()))
            {
                throw new DataServiceException(
                    400,
                    DataServicesResourceUtil.GetString("BadRequest_ErrorInSettingPropertyValue", "BestFriend"));
            }

            declaringResource.GetType().InvokeMember(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty,
                null,
                declaringResource,
                new object[] { propertyValue });
        }

        public void AddReferenceToCollection(object token, string propertyName, object elementResourceToken)
        {
            ResourceInfo ri = this.tokens[(int)token];
            object targetObject = ri.Resource;
            ri.CanPrimitivePropertyBeUpdated = false;

            object element = this.tokens[(int)elementResourceToken].Resource;
            object propertyValue = targetObject.GetType().InvokeMember(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                null,
                targetObject,
                null);

            try
            {
                propertyValue.GetType().InvokeMember(
                    "Add",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                    null,
                    propertyValue,
                    new object[] { element });
            }
            catch (MissingMethodException)
            {
                throw new DataServiceException(
                    400,
                    DataServicesResourceUtil.GetString("BadRequest_ErrorInSettingPropertyValue", "Orders"));
            }
        }

        public void RemoveReferenceFromCollection(object token, string propertyName, object elementResourceToken)
        {
            ResourceInfo ri = this.tokens[(int)token];
            object targetObject = ri.Resource;
            ri.CanPrimitivePropertyBeUpdated = false;

            object element = this.tokens[(int)elementResourceToken].Resource;
            object propertyValue = targetObject.GetType().InvokeMember(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                null,
                targetObject,
                null);

            try
            {
                propertyValue.GetType().InvokeMember(
                    "Remove",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                    null,
                    propertyValue,
                    new object[] { element });
            }
            catch (MissingMethodException)
            {
                throw new DataServiceException(
                    400,
                    DataServicesResourceUtil.GetString("BadRequest_ErrorInSettingPropertyValue", "Orders"));
            }
        }

        public virtual void DeleteResource(object token)
        {
            object objectToBeDeleted = this.tokens[(int)token].Resource;
            this.PendingChanges.Add(new KeyValuePair<object, EntityState>(objectToBeDeleted, EntityState.Deleted));
        }

        public void SaveChanges()
        {
            if (this.pendingChanges == null)
            {
                return;
            }

            foreach (KeyValuePair<object, EntityState> pendingChange in this.PendingChanges)
            {
                // find the entity set for the object
                IList entitySetInstance = GetEntitySet(pendingChange.Key.GetType());

                switch (pendingChange.Value)
                {
                    case EntityState.Added:
                        AddResource(pendingChange.Key, true /*throwIfDuplicate*/);
                        break;
                    case EntityState.Deleted:
                        DeleteEntity(entitySetInstance, pendingChange.Key, true /*throwIfNotPresent*/);
                        if (typeof(Customer).IsAssignableFrom(pendingChange.Key.GetType()))
                        {
                            foreach (Customer customer in this.Customers)
                            {
                                if (customer.BestFriend != null &&
                                    customer.BestFriend.Equals((Customer)pendingChange.Key))
                                {
                                    customer.BestFriend = null;
                                }
                            }

                            foreach (Order o in this.Orders)
                            {
                                if (o.Customer != null &&
                                    o.Customer.Equals((Customer)pendingChange.Key))
                                {
                                    o.Customer = null;
                                }
                            }
                        }
                        else if (pendingChange.Key.GetType() == typeof(Order))
                        {
                            foreach (Customer customer in this.Customers)
                            {
                                DeleteEntity(customer.Orders, pendingChange.Key, false /*throwIfNotPresent*/);
                            }
                            foreach (OrderDetail od in ((Order)pendingChange.Key).OrderDetails)
                            {
                                IList orderDetailInstance = GetEntitySet(typeof(OrderDetail));
                                DeleteEntity(orderDetailInstance, od, true /*throwIfNotPresent*/);
                                foreach (Product p in this.Products)
                                {
                                    DeleteEntity(p.OrderDetails, od, false /*throwIfNotPresent*/);
                                }
                            }
                        }
                        else
                            if (pendingChange.Key.GetType() == typeof(OrderDetail))
                            {
                                foreach (Customer customer in this.Customers)
                                {
                                    foreach (Order o in customer.Orders)
                                    {
                                        DeleteEntity(o.OrderDetails, pendingChange.Key, false /*throwIfNotPresent*/);
                                    }
                                }
                                foreach (Order o in this.Orders)
                                {
                                    DeleteEntity(o.OrderDetails, pendingChange.Key, false /*throwIfNotPresent*/);
                                }
                                foreach (Product p in this.Products)
                                {
                                    DeleteEntity(p.OrderDetails, pendingChange.Key, false /*throwIfNotPresent*/);
                                }
                            }
                        break;
                    default:
                        throw new Exception("Unsupported State");
                }
            }

            this.pendingChanges.Clear();
            // Trace.WriteLine("Changes saved on AppDomain #" + AppDomain.CurrentDomain.Id + ", with customer count " + customers.Count);
        }

        public object ResolveResource(object resource)
        {
            return this.tokens[(int)resource].Resource;
        }

        public void ClearChanges()
        {
            if (null != this.pendingChanges)
            {
                this.pendingChanges.Clear();
            }
        }

        #endregion

        private static bool IsPrimitiveType(Type type)
        {
            return (type.IsPrimitive ||
                    type == typeof(String) ||
                    type == typeof(Guid) ||
                    type == typeof(Decimal) ||
                    type == typeof(DateTime) ||
                    type == typeof(DateTimeOffset) ||
                    type == typeof(byte[]));
        }

        private static IList GetEntitySet(Type entityType)
        {
            if (customers == null)
            {
                CreateDefaultData();
            }

            if (typeof(Customer).IsAssignableFrom(entityType))
            {
                return customers;
            }

            if (entityType == typeof(Order))
            {
                return orders;
            }

            if (entityType == typeof(Region))
            {
                return regions;
            }

            if (entityType == typeof(Product))
            {
                return products;
            }

            if (entityType == typeof(OrderDetail))
            {
                return orderDetails;
            }

            throw new Exception("Unexpected EntityType encountered");
        }

        internal static void AddResource(object resource, bool throwIfDuplicate)
        {
            IList entitySetInstance = GetEntitySet(resource.GetType());

            foreach (object entity in entitySetInstance)
            {
                // check if there is not another instance with the same id
                if (Equal(resource, entity))
                {
                    if (throwIfDuplicate)
                    {
                        throw new DataServiceException(400, String.Format("Entity with the same key already present. EntityType: '{0}'",
                            resource.GetType().Name));
                    }

                    // if its already there, do not add it to the global context
                    return;
                }
            }
            entitySetInstance.Add(resource);
        }

        private void DeleteEntity(IEnumerable collection, object entity, bool throwIfNotPresent)
        {
            object entityToBeDeleted = TryGetEntity(collection, entity);

            if (entityToBeDeleted == null && throwIfNotPresent)
            {
                throw new Exception("No entity found with the given ID");
            }

            if (entityToBeDeleted != null)
            {
                // Make sure that property type implements ICollection<T> If yes, then call remove method on it to remove the
                // resource
                Type elementType = TestUtil.GetTypeParameter(collection.GetType(), typeof(ICollection<>), 0);
                typeof(ICollection<>).MakeGenericType(elementType).InvokeMember(
                                                "Remove",
                                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                                null,
                                                collection,
                                                new object[] { entityToBeDeleted });
            }
        }

        public static void ClearData()
        {
            CustomDataContext.PreserveChanges = false;
            LocalWebClearPending = true;
            customers = null;
            orders = null;
            regions = null;
            products = null;
            orderDetails = null;
        }

        private static bool Equal(object resource1, object resource2)
        {
            if (resource1.GetType() != resource2.GetType())
            {
                return false;
            }

            // check if there is not another instance with the same id
            return (bool)resource1.GetType().InvokeMember("Equals",
                                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod,
                                        null,
                                        resource1,
                                        new object[] { resource2 });
        }

        internal static bool IsExistingEntity(object resource)
        {
            IList entitySet = GetEntitySet(resource.GetType());

            foreach (object entity in entitySet)
            {
                if (entity == resource)
                {
                    return true;
                }
            }

            return false;
        }

        private static object TryGetEntity(IEnumerable collection, object entity)
        {
            object matchingEntity = null;

            foreach (object element in collection)
            {
                // check if there is not another instance with the same id
                if (Equal(element, entity))
                {
                    matchingEntity = element;
                    break;
                }
            }

            return matchingEntity;
        }

        private static object CreateTypeWithKey(Type type, IEnumerable<KeyValuePair<string, object>> keyValues)
        {
            // Create the new instance of the type
            ConstructorInfo emptyConstructor = type.GetConstructor(Type.EmptyTypes);
            object entity = emptyConstructor.Invoke(null);

            foreach (KeyValuePair<string, object> keyInfo in keyValues)
            {
                PropertyInfo property = type.GetProperty(keyInfo.Key, BindingFlags.Instance | BindingFlags.Public);
                property.SetValue(entity, keyInfo.Value, null);
            }

            return entity;
        }

        class CustomDataContextPresereChangesScope : IDisposable
        {
            public void Dispose()
            {
                CustomDataContext.ClearData();
            }
        }

        public class ResourceInfo
        {
            public object Resource { get; set; }

            // This property is used to track that all primitive properties are updated before updating nav properties.
            public bool CanPrimitivePropertyBeUpdated { get; set; }

            public ResourceInfo(object resource)
            {
                this.Resource = resource;
                this.CanPrimitivePropertyBeUpdated = true;
            }
        }
    }

    // Custom typed data context implementation that is not an instance of a generic type since that produces
    // invalid EDM names that cannot be loaded via EdmItemCollection for validation.
    public sealed class TypedCustomAllTypesDataContext : TypedCustomDataContext<AllTypes>
    {
    }

    public class ChangeScope : IDisposable
    {
        private Type contextType;
        private ChangeScope(Type contextType)
        {
            this.contextType = contextType;
        }

        public static ChangeScope GetChangeScope(Type contextType)
        {
            contextType.GetMethod("CreateChangeScope", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, null);
            return new ChangeScope(contextType);
        }

        #region IDisposable Members
        public void Dispose()
        {
            this.contextType.GetMethod("ClearData", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, null);
        }
        #endregion
    }

    public class ListTracking<T> : ICollection<T>
    {
        private List<T> collection;

        internal ListTracking()
        {
            this.collection = new List<T>();
        }

        #region ICollection<T> Members

        public void Add(T item)
        {
            this.collection.Add(item);
        }

        public void Clear()
        {
            this.collection.Clear();
        }

        public bool Contains(T item)
        {
            return this.collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i + arrayIndex] = collection[i];
            }
        }

        public int Count
        {
            get { return this.collection.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(T item)
        {
            return this.collection.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }

    public class CircularReferenceType
    {
        private CircularReferenceType otherInstance;
        private int data;

        public CircularReferenceType OtherInstance
        {
            get { return this.otherInstance; }
            set { this.otherInstance = value; }
        }

        public int Data
        {
            get { return this.data; }
            set { this.data = value; }
        }
    }

    public class TypedReferenceComplexObject<TMember>
    {
        private TMember member;

        public TMember Member
        {
            get { return this.member; }
            set { this.member = value; }
        }
    }

    /// <summary>A generic type that looks (by convention) like an entity type.</summary>
    /// <typeparam name="TKey">Type of ID member.</typeparam>
    /// <typeparam name="TMember">Type of data member.</typeparam>
    public class TypedEntity<TKey, TMember> : System.ComponentModel.INotifyPropertyChanged
    {
        private TKey id;
        private TMember member;

        public TKey ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public static int StaticProperty
        {
            get { return 100; }
            set { }
        }

        public TMember Member
        {
            get { return this.member; }
            set
            {
                this.member = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Member"));
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>A generic type that looks (by convention) like an entity type.</summary>
    /// <typeparam name="TKey">Type of ID member.</typeparam>
    /// <typeparam name="TMember">Type of first data member.</typeparam>
    /// <typeparam name="TMember2">Type of second data member.</typeparam>
    public class DoubleMemberTypedEntity<TKey, TMember, TMember2>
    {
        private TKey id;
        private TMember member;
        private TMember2 member2;

        public TKey ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public TMember Member
        {
            get { return this.member; }
            set { this.member = value; }
        }

        public TMember2 Member2
        {
            get { return this.member2; }
            set { this.member2 = value; }
        }
    }

    /// <summary>
    /// A generic type that looks (by convention) like an entity type and has 
    /// references to values of its own type.
    /// </summary>
    /// <typeparam name="TKey">Type of ID member.</typeparam>
    /// <typeparam name="TMember">Type of data member.</typeparam>
    public class SelfReferenceTypedEntity<TKey, TMember>
    {
        public TKey ID { get; set; }
        public TMember Member { get; set; }
        public List<SelfReferenceTypedEntity<TKey, TMember>> Collection { get; set; }
        public DataServiceCollection<SelfReferenceTypedEntity<TKey, TMember>> DSC { get; set; }
        public SelfReferenceTypedEntity<TKey, TMember> Reference { get; set; }
    }

    [Key("SecondKey", "FirstKey")]
    public class DoubleKeyTypedEntity<TFirstKey, TSecondKey, TMember>
    {
        private TFirstKey firstKey;
        private TSecondKey secondKey;
        private TMember member;

        public TSecondKey SecondKey
        {
            get { return this.secondKey; }
            set { this.secondKey = value; }
        }

        public TFirstKey FirstKey
        {
            get { return this.firstKey; }
            set { this.firstKey = value; }
        }

        public TMember Member
        {
            get { return this.member; }
            set { this.member = value; }
        }
    }

    public class TypedCustomDataContextWithConcurrencyProvider<T> : TypedCustomDataContext<T>, IDataServiceUpdateProvider
    {
        #region IConcurrencyProvider Members

        public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            if (checkForEquality == null)
            {
                throw new DataServiceException(400, "Missing If-Match header");
            }

            Debug.Assert(checkForEquality.Value, "If-None-Match header is not supported for Update/Delete scenarios");
            foreach (var etagProperty in concurrencyValues)
            {
                object propertyValue = ((IUpdatable)this).GetValue(resourceCookie, etagProperty.Key);
                if ((propertyValue == null && etagProperty.Value == null) ||
                    propertyValue != null && ComparePropertyValue(propertyValue, etagProperty.Value))
                {
                    continue;
                }

                throw new DataServiceException(412, String.Format("'{0}' property value did not match. Actual: '{1}', Expected: '{2}'", etagProperty.Key, etagProperty.Value, propertyValue));
            }
        }

        #endregion

        private bool ComparePropertyValue(object value1, object value2)
        {
            if (value1.GetType() == typeof(byte[]))
            {
                byte[] v1 = (byte[])value1;
                byte[] v2 = (byte[])value2;
                for (int i = 0; i < v1.Length; i++)
                {
                    if (!v1[i].Equals(v2[i]))
                    {
                        return false;
                    }
                }
            }
            else if (!value1.Equals(value2))
            {
                return false;
            }

            return true;
        }
    }

    public class BadMetadataProviderNavPropInBase
    {
        public class EmployeeBase
        {
            public int ID { get; set; }
            public Manager MyManager { get; set; }
        }

        public class Employee : EmployeeBase { }
        public class Manager : EmployeeBase { }

        public IQueryable<Employee> Employees { get; set; }
        public IQueryable<Manager> Managers { get; set; }
    }

    public class BadMetadataProvider
    {
        public class A
        {
            public int ID { get; set; }
        }

        public class B : A
        {
            public C nonLeafRefernce { get; set; }
        }

        public class C
        {
            public int ID { get; set; }
        }

        public class D : C
        {
        }

        List<D> _publicD { get; set; }

        public IQueryable<D> Dset
        {
            get { return _publicD.AsQueryable<D>(); }
        }

        List<B> _publicB { get; set; }

        public IQueryable<B> Bset
        {
            get { return _publicB.AsQueryable<B>(); }
        }

        List<A> _publicA { get; set; }

        public BadMetadataProvider()
        {
        }
    }

    /// <summary>Use this type to have a typed data service that surfaced protected virtuals as events.</summary>
    /// <typeparam name="T"></typeparam>
    public class TypedDataService<T> : DataService<T>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            StaticCallbackManager<InitializeServiceArgs>.FireEvent(null, new InitializeServiceArgs() { Config = config });
        }

        protected override void HandleException(HandleExceptionArgs args)
        {
            base.HandleException(args);
            StaticCallbackManager<HandleExceptionEventArgs>.FireEvent(
                this,
                new HandleExceptionEventArgs() { Args = args });
        }
    }

    public class ApplyingExpansionsArgs : EventArgs
    {
        public IQueryable Queryable { get; set; }
        public ICollection<ExpandSegmentCollection> ExpandPaths { get; set; }
    }

    public class HandleExceptionEventArgs : EventArgs
    {
        public HandleExceptionArgs Args { get; set; }
    }

    public class InitializeServiceArgs : EventArgs
    {
        public IDataServiceConfiguration Config { get; set; }
    }

    public class AllTypes
    {
        private int id;
        private byte[] binaryType;
        private bool boolType;
        private byte byteType;
        private DateTimeOffset dateTimeOffsetType;
        private decimal decimalType;
        private Double doubleType;
        private Guid guidType;
        private Int16 int16Type;
        private Int32 int32Type;
        private Int64 int64Type;
        private SByte sbyteType;
        private Single singleType;
        private string stringType;
        private TimeSpan timeSpanType;
        private Nullable<bool> nullableBoolType;
        private Nullable<byte> nullableByteType;
        private Nullable<DateTimeOffset> nullableDateTimeOffsetType;
        private Nullable<decimal> nullableDecimalType;
        private Nullable<Double> nullableDoubleType;
        private Nullable<Guid> nullableGuidType;
        private Nullable<Int16> nullableInt16Type;
        private Nullable<Int32> nullableInt32Type;
        private Nullable<Int64> nullableInt64Type;
        private Nullable<SByte> nullableSByteType;
        private Nullable<Single> nullableSingleType;
        private Nullable<TimeSpan> nullableTimeSpanType;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public byte[] BinaryType
        {
            get { return this.binaryType; }
            set { this.binaryType = value; }
        }

        public bool BoolType
        {
            get { return this.boolType; }
            set { this.boolType = value; }
        }

        public Byte ByteType
        {
            get { return this.byteType; }
            set { this.byteType = value; }
        }

        public DateTimeOffset DateTimeOffsetType
        {
            get { return this.dateTimeOffsetType; }
            set { this.dateTimeOffsetType = value; }
        }

        public decimal DecimalType
        {
            get { return this.decimalType; }
            set { this.decimalType = value; }
        }

        public Double DoubleType
        {
            get { return this.doubleType; }
            set { this.doubleType = value; }
        }

        public Guid GuidType
        {
            get { return this.guidType; }
            set { this.guidType = value; }
        }

        public Int16 Int16Type
        {
            get { return this.int16Type; }
            set { this.int16Type = value; }
        }

        public Int32 Int32Type
        {
            get { return this.int32Type; }
            set { this.int32Type = value; }
        }

        public Int64 Int64Type
        {
            get { return this.int64Type; }
            set { this.int64Type = value; }
        }

        private static Dictionary<Type, string> namesForTypes;

        public static string PropertyNameForType(Type type)
        {
            TestUtil.CheckArgumentNotNull(type, "type");
            if (namesForTypes == null)
            {
                namesForTypes = new Dictionary<Type, string>();
                foreach (PropertyInfo p in typeof(AllTypes).GetProperties())
                {
                    if (p.Name == "ID") continue;
                    namesForTypes.Add(p.PropertyType, p.Name);
                }
            }
            return namesForTypes[type];
        }

        public System.Data.Linq.Binary LinqBinaryType { get; set; }
        public System.Xml.Linq.XElement LinqXElementType { get; set; }

        public SByte SByteType
        {
            get { return this.sbyteType; }
            set { this.sbyteType = value; }
        }

        public Single SingleType
        {
            get { return this.singleType; }
            set { this.singleType = value; }
        }

        public string StringType
        {
            get { return this.stringType; }
            set { this.stringType = value; }
        }

        public TimeSpan TimeSpanType
        {
            get { return this.timeSpanType; }
            set { this.timeSpanType = value; }
        }

        //[CLSCompliantAttribute(false)]
        //public UInt16 UInt16Type
        //{
        //    get { return this.uint16Type; }
        //    set { this.uint16Type = value; }
        //}

        //[CLSCompliantAttribute(false)]
        //public UInt32 UInt32Type
        //{
        //    get { return this.uint32Type; }
        //    set { this.uint32Type = value; }
        //}

        //[CLSCompliantAttribute(false)]
        //public UInt64 UInt64Type
        //{
        //    get { return this.uint64Type; }
        //    set { this.uint64Type = value; }
        //}

        public Nullable<bool> NullableBoolType
        {
            get { return this.nullableBoolType; }
            set { this.nullableBoolType = value; }
        }

        public Nullable<Byte> NullableByteType
        {
            get { return this.nullableByteType; }
            set { this.nullableByteType = value; }
        }

        public Nullable<DateTimeOffset> NullableDateTimeOffsetType
        {
            get { return this.nullableDateTimeOffsetType; }
            set { this.nullableDateTimeOffsetType = value; }
        }

        public Nullable<decimal> NullableDecimalType
        {
            get { return this.nullableDecimalType; }
            set { this.nullableDecimalType = value; }
        }

        public Nullable<Double> NullableDoubleType
        {
            get { return this.nullableDoubleType; }
            set { this.nullableDoubleType = value; }
        }

        public Nullable<Guid> NullableGuidType
        {
            get { return this.nullableGuidType; }
            set { this.nullableGuidType = value; }
        }

        public Nullable<Int16> NullableInt16Type
        {
            get { return this.nullableInt16Type; }
            set { this.nullableInt16Type = value; }
        }

        public Nullable<Int32> NullableInt32Type
        {
            get { return this.nullableInt32Type; }
            set { this.nullableInt32Type = value; }
        }

        public Nullable<Int64> NullableInt64Type
        {
            get { return this.nullableInt64Type; }
            set { this.nullableInt64Type = value; }
        }

        public Nullable<SByte> NullableSByteType
        {
            get { return this.nullableSByteType; }
            set { this.nullableSByteType = value; }
        }

        public Nullable<Single> NullableSingleType
        {
            get { return this.nullableSingleType; }
            set { this.nullableSingleType = value; }
        }

        public Nullable<TimeSpan> NullableTimeSpanType
        {
            get { return this.nullableTimeSpanType; }
            set { this.nullableTimeSpanType = value; }
        }
    }

    [IgnoreProperties("OpenProps")]
    public class AllTypesWithReferences : AllTypes
    {
        public AllTypesWithReferences()
            : base()
        {
            this.OpenProps = new Dictionary<string, object>();
        }

        public IDictionary<string, object> OpenProps { get; private set; }

        public static string PropertyNameForReferenceType(Type type)
        {
            if (type == null)
            {
                return "null";
            }
            else if (type == typeof(object))
            {
                return "OpenProp";
            }
            else
            {
                return AllTypes.PropertyNameForType(type);
            }
        }
    }

    [Key("ActualID", "ActualID1")]
    public class MultiplePropertiesSatisfyingKeyCriteria
    {
        public int ID { get; set; }

        public int MultiplePropertiesSatisfyingKeyCriteriaID { get; set; }

        public string ActualID { get; set; }
        public string ActualID1 { get; set; }
    }

    [Key("ActualID")]
    public class MultiplePropertiesSatisfyingKeyCriteria1
    {
        public string ActualID { get; set; }

        public int ID { get; set; }

        public int MultiplePropertiesSatisfyingKeyCriteriaID { get; set; }
    }

    #region InvalidTypes

    #region Type with same property name
    public class InvalidBaseType_DuplicatePropertyName
    {
        private int id;
        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }
    }

    public class InvalidDerivedType_DuplicatePropertyName : InvalidBaseType_DuplicatePropertyName
    {
        public new string ID { get { return null; } }
    }
    #endregion
    #endregion

}

namespace AstoriaUnitTests.StubsOtherNs
{
    using System;

    public class Region : IEquatable<Region>
    {
        private int id;
        private string name;

        public Region()
        {
        }

        public Region(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
        public int ID { get { return this.id; } set { this.id = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }

        #region IEquatable<Region> Members

        public bool Equals(Region other)
        {
            return (this.ID == other.ID);
        }

        #endregion

        public void Clone(object resource)
        {
            Region newRegion = resource as Region;

            if (newRegion == null)
            {
                throw new Exception("Type mismatch in Clone");
            }

            if (this.ID != newRegion.ID)
            {
                throw new Exception("Key Values do not match in Clone");
            }

            newRegion.Name = this.Name;
        }
    }
}

