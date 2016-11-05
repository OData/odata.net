//---------------------------------------------------------------------
// <copyright file="DefaultWCFSvcDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    /// <summary>
    /// The class implements an in memory data source that provides IQueryables of model types
    /// </summary>
    public class DefaultWCFSvcDataSource : ODataReflectionDataSource
    {
        private EntityCollection<Customer> customers;
        private EntityCollection<Employee> employees;

        public DefaultWCFSvcDataSource()
        {
            this.OperationProvider = new DefaultServiceOperationProvider();
        }

        #region Singleton Resources

        /// <summary>
        /// Returns Boss.
        /// </summary>
        public Person Boss
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns VIP Customer.
        /// </summary>
        public Customer VipCustomer
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the type Company.
        /// </summary>
        public Company Company
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns PublicCompany.
        /// </summary>
        public PublicCompany PublicCompany
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns LabourUnion.
        /// </summary>
        public LabourUnion LabourUnion
        {
            get;
            private set;
        }
        /// <summary>
        /// Returns DefaultStoredPI.
        /// </summary>
        public StoredPI DefaultStoredPI
        {
            get;
            private set;
        }

        #endregion

        #region Entity Set Resources

        /// <summary>
        /// Returns an entity set of Customer.
        /// </summary>
        public EntityCollection<Person> People
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns an entity set of Customer.
        /// </summary>
        public EntityCollection<Customer> Customers
        {
            get
            {
                this.customers.Clear();
                this.customers.AddRange(this.People.OfType<Customer>());

                return this.customers;
            }
        }

        /// <summary>
        /// Returns an entity set of Employee.
        /// </summary>
        public EntityCollection<Employee> Employees
        {
            get
            {
                this.employees.Clear();
                this.employees.AddRange(this.People.OfType<Employee>());

                return this.employees;
            }
        }

        /// <summary>
        /// Returns an entity set of Product.
        /// </summary>
        public EntityCollection<Product> Products
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns an entity set of ProductDetail.
        /// </summary>
        public EntityCollection<ProductDetail> ProductDetails
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns an entity set of ProductReview.
        /// </summary>
        public EntityCollection<ProductReview> ProductReviews
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns an entity set of Order.
        /// </summary>
        public EntityCollection<Order> Orders
        {
            get;
            private set;
        }

        public EntityCollection<Calendar> Calendars
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns an entity set of OrderDetail.
        /// </summary>
        public EntityCollection<OrderDetail> OrderDetails
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns an entity set of Account.
        /// </summary>
        public EntityCollection<Account> Accounts
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns an entity set of StoredPI.
        /// </summary>
        public EntityCollection<StoredPI> StoredPIs
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns an entity set of SubscriptionTemplates.
        /// </summary>
        public EntityCollection<Subscription> SubscriptionTemplates
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns an entity set of Department.
        /// </summary>
        public EntityCollection<Department> Departments
        {
            get;
            private set;
        }
        #endregion

        public override void Reset()
        {
            this.People = new EntityCollection<Person>();
            this.Orders = new EntityCollection<Order>();
            this.Calendars = new EntityCollection<Calendar>();
            this.Products = new EntityCollection<Product>();
            this.OrderDetails = new EntityCollection<OrderDetail>();
            this.ProductDetails = new EntityCollection<ProductDetail>();
            this.ProductReviews = new EntityCollection<ProductReview>();
            this.Accounts = new EntityCollection<Account>();
            this.StoredPIs = new EntityCollection<StoredPI>();
            this.SubscriptionTemplates = new EntityCollection<Subscription>();
            this.Departments = new EntityCollection<Department>();

            this.customers = new EntityCollection<Customer>();
            this.employees = new EntityCollection<Employee>();
        }

        /// <summary>
        /// Populates the data source.
        /// </summary>
        public override void Initialize()
        {
            this.Boss = new Customer()
            {
                FirstName = "Jill",
                LastName = "Jones",
                Numbers = new Collection<string>(),
                Emails = new Collection<string>(),
                PersonID = 2,
                Birthday = new DateTimeOffset(new DateTime(1983, 1, 15)),
                City = "Sydney",
                Home = GeographyPoint.Create(15.0, 161.8),
                TimeBetweenLastTwoOrders = new TimeSpan(2),
                Annotations = new List<InstanceAnnotationType>() { new IsBossAnnotation(this.GetType().Namespace, true) },
            };

            this.VipCustomer = new Customer()
            {
                FirstName = "Bob",
                MiddleName = "Vat",
                LastName = "Cat",
                Numbers = new Collection<string> { "111-111-1111" },
                Emails = new Collection<string> { "abc@abc.com" },
                PersonID = 1,
                Birthday = new DateTimeOffset(new DateTime(1957, 4, 3)),
                City = "London",
                Home = GeographyPoint.Create(32.1, 23.1),
                TimeBetweenLastTwoOrders = new TimeSpan(1),
                HomeAddress = new Address()
                {
                    City = "London",
                    PostalCode = "98052",
                    Street = "1 Microsoft Way"
                }
            };

            this.Company = new Company()
            {
                CompanyID = 0,
                CompanyCategory = CompanyCategory.IT,
                Revenue = 100000,
                Name = "MS",
                Address = new CompanyAddress()
                {
                    City = "Redmond",
                    PostalCode = "98052",
                    Street = "1 Microsoft Way",
                    CompanyName = "Microsoft"
                }
            };

            this.PublicCompany = new PublicCompany
            {
                CompanyID = 1,
                CompanyCategory = CompanyCategory.IT,
                Name = "MS2",
                Address = new Address
                {
                    City = "Redmond",
                    PostalCode = "98052",
                    Street = "1 Microsoft Way"
                },
                Revenue = 100000,
                StockExchange = "NASDAQ",
                Assets = 
                {
                    new Asset()
                    {
                        AssetID = 0,
                        Name = "Dell",
                        Number = 100
                    },

                    new Asset()
                    {
                        AssetID = 1,
                        Name = "ThinkPad",
                        Number = 200
                    }                        
                },
                Club = new Club
                {
                    ClubID = 0,
                    Name = "health Club"
                }
            };

            this.LabourUnion = new LabourUnion
            {
                LabourUnionID = 0,
                Name = "MS Labour Union"
            };
            this.PublicCompany.LabourUnion = LabourUnion;

            this.DefaultStoredPI = new StoredPI()
            {
                StoredPIID = 800,
                PIName = "The Default Stored PI",
                PIType = "CreditCard",
                CreatedDate = new DateTimeOffset(new DateTime(2013, 12, 31))
            };

            this.People.AddRange(new List<Person>()
            {
                new Customer()
                {
                    FirstName = "Bob",
                    LastName = "Cat",
                    Numbers = new Collection<string> { "111-111-1111", "012", "310", "bca", "ayz" },
                    Emails = new Collection<string> { "abc@abc.com" },
                    PersonID = 1,
                    Birthday = new DateTimeOffset(new DateTime(1957, 4, 3)),
                    City = "London",
                    Home = GeographyPoint.Create(32.1, 23.1), 
                    TimeBetweenLastTwoOrders = new TimeSpan(1),
                    HomeAddress = new HomeAddress()
                    {
                        City = "Tokyo",
                        PostalCode = "98052",
                        Street = "1 Microsoft Way",
                        FamilyName = "Cats",
                        Annotations = new List<InstanceAnnotationType>()
                        {
                            new AddressTypeAnnotation(this.GetType().Namespace, "Home"),
                            new CityInfoAnnotation(this.GetType().Namespace, new CityInformation(){CountryRegion = "Japan", IsCapital = true}, "City"),
                        },
                    },
                    Addresses = new Collection<Address>
                    {
                        new HomeAddress()
                        {
                            City = "Tokyo",
                            PostalCode = "98052",
                            Street = "1 Microsoft Way",
                            FamilyName = "Cats",
                            Annotations = new List<InstanceAnnotationType>()
                            {
                                new AddressTypeAnnotation(this.GetType().Namespace, "Home"),
                                new CityInfoAnnotation(this.GetType().Namespace, new CityInformation(){CountryRegion = "Japan", IsCapital = true}, "City"),
                            },
                        },
                        new Address()
                        {
                            City = "Shanghai",
                            PostalCode = "200000",
                            Street = "999 Zixing Road",
                            Annotations = new List<InstanceAnnotationType>()
                            {
                                new AddressTypeAnnotation(this.GetType().Namespace, "Company"),
                                new CityInfoAnnotation(this.GetType().Namespace, new CityInformation(){CountryRegion = "China", IsCapital = true}, "City"),
                            },
                        },
                    },
                    Annotations = new List<InstanceAnnotationType>(){new DisplayNameAnnotation(this.GetType().Namespace, "EmailAddresses", "Emails")}
                },
                new Customer()
                {
                    FirstName = "Jill",
                    LastName = "Jones",
                    Numbers = new Collection<string>(),
                    Emails = new Collection<string>(),
                    PersonID = 2,
                    Birthday = new DateTimeOffset(new DateTime(1983, 1, 15)),
                    City = "Sydney",
                    Home = GeographyPoint.Create(15.0, 161.8),
                    TimeBetweenLastTwoOrders = new TimeSpan(2)
                },
                new Employee()
                {
                    FirstName = "Jacob",
                    LastName = "Zip",
                    Numbers = new Collection<string> { "333-333-3333" },
                    Emails = new Collection<string> { null },
                    PersonID = 3,
                    DateHired = new DateTimeOffset(new DateTime(2010, 12, 13)),
                    Home = GeographyPoint.Create(15.0, 161.8),
                    Office = GeographyPoint.Create(15.0, 162),
                    HomeAddress = new HomeAddress()
                    {
                        City = "Sydney",
                        PostalCode = "98052",
                        Street = "1 Microsoft Way",
                        FamilyName = "Zips",
                        Annotations = new List<InstanceAnnotationType>(){new AddressTypeAnnotation(this.GetType().Namespace, "Working")},
                    },
                    Addresses = new Collection<Address>
                    {
                        new Address()
                        {
                            City = "Shanghai2",
                            PostalCode = "200000",
                            Street = "B01, 999 Zixing Road",
                            Annotations = new List<InstanceAnnotationType>()
                            {
                                new AddressTypeAnnotation(this.GetType().Namespace, "Company"),
                                new CityInfoAnnotation(this.GetType().Namespace, new CityInformation(){CountryRegion = "China", IsCapital = true}, "City"),
                            },
                        }
                    }
                },
                new Employee()
                {
                    FirstName = "Elmo",
                    LastName = "Rogers",
                    Numbers = new Collection<string> { "444-444-4444", "555-555-5555", "666-666-6666" },
                    Emails = new Collection<string> { "def@def.org", "lmn@lmn.com","max@max.com","test@test.com" },
                    PersonID = 4,
                    DateHired = new DateTimeOffset(new DateTime(2008, 3, 27)),
                    Home = GeographyPoint.Create(-15.0, -61.8),
                    Office = GeographyPoint.Create(-15.0, -62),
                    Addresses = new Collection<Address>
                    {
                        new Address()
                        {
                            City = "Shanghai2",
                            PostalCode = "200000",
                            Street = "B01, 999 Zixing Road",
                            Annotations = new List<InstanceAnnotationType>()
                            {
                                new AddressTypeAnnotation(this.GetType().Namespace, "Company"),
                                new CityInfoAnnotation(this.GetType().Namespace, new CityInformation(){CountryRegion = "China", IsCapital = true}, "City"),
                            },
                        }
                    }
                },
                new Person()
                {
                    FirstName = "Peter",
                    LastName = "Bee",
                    MiddleName = null,
                    Numbers = new Collection<string> { "555-555-5555" },
                    Emails = new Collection<string> { "def@test.msn" },
                    PersonID = 5,
                    Home = GeographyPoint.Create(-16.0, -261.8),
                    Addresses = new Collection<Address>
                    {
                        new HomeAddress()
                        {
                            City = "Tokyo",
                            PostalCode = "98052",
                            Street = "2 Microsoft Way",
                            FamilyName = "Cats",
                            Annotations = new List<InstanceAnnotationType>()
                            {
                                new AddressTypeAnnotation(this.GetType().Namespace, "Home"),
                                new CityInfoAnnotation(this.GetType().Namespace, new CityInformation(){CountryRegion = "Japan", IsCapital = true}, "City"),
                            },
                        },
                        new Address()
                        {
                            City = "Shanghai",
                            PostalCode = "200000",
                            Street = "999 Zixing Road",
                            Annotations = new List<InstanceAnnotationType>()
                            {
                                new AddressTypeAnnotation(this.GetType().Namespace, "Company"),
                                new CityInfoAnnotation(this.GetType().Namespace, new CityInformation(){CountryRegion = "China", IsCapital = true}, "City"),
                            },
                        },
                    }
                }
            });

            this.People[0].Parent = this.People[1];
            this.People[1].Parent = this.People[3];
            this.People[2].Parent = this.People[3];

            this.Products.AddRange(new List<Product>()
            {
                new Product()
                {
                        Name = "Cheetos",
                        ProductID = 5,
                        QuantityInStock = 100,
                        QuantityPerUnit = "100g Bag",
                        UnitPrice = 3.24f, 
                        Discontinued = true,
                        SkinColor = Color.Red,
                        CoverColors = new Collection<Color>(){ Color.Green, Color.Blue, Color.Blue },
                        UserAccess = AccessLevel.None,
                        Annotations = new List<InstanceAnnotationType>(){new DisplayNameAnnotation(this.GetType().Namespace, "Cheetos", "Name")}
                },
                new Product()
                {
                        Name = "Mushrooms",
                        ProductID = 6,
                        QuantityInStock = 100,
                        QuantityPerUnit = "Pound",
                        UnitPrice = 3.24f,
                        Discontinued = false,
                        SkinColor = Color.Blue,
                        CoverColors = new Collection<Color>(){ Color.Red, Color.Blue },
                        UserAccess = AccessLevel.ReadWrite,
                },
                new Product()
                {
                        Name = "Apple",
                        ProductID = 7,
                        QuantityInStock = 20,
                        QuantityPerUnit = "Pound",
                        UnitPrice = 0.35f,
                        Discontinued = false,
                        SkinColor = Color.Red,
                        CoverColors = new Collection<Color>(){ Color.Blue },
                        UserAccess = AccessLevel.Read,
                },
                new Product()
                {
                        Name = "Car",
                        ProductID = 8,
                        QuantityInStock = 300,
                        QuantityPerUnit = "Pound",
                        UnitPrice = 28000f,
                        Discontinued = false,
                        SkinColor = Color.Red,
                        CoverColors = new Collection<Color>(){ Color.Red, Color.Red, Color.Blue },
                        UserAccess = AccessLevel.Execute,
                },
                new Product()
                {
                        Name = "Computer",
                        ProductID = 9,
                        QuantityInStock = 1000,
                        QuantityPerUnit = "Pound",
                        UnitPrice = 1250f,
                        Discontinued = false,
                        SkinColor = Color.Green,
                        CoverColors = new Collection<Color>(){ Color.Green, Color.Blue },
                        UserAccess = AccessLevel.Read,
                }
            });

            this.ProductDetails.AddRange(new List<ProductDetail>
            {
                new ProductDetail()
                {
                    ProductDetailID = 1,
                    ProductName = "Candy",
                    Description = "sweet snack"
                },
                new ProductDetail()
                {
                    ProductDetailID = 2,
                    ProductName = "CheeseCake",
                    Description = "Cheese-flavored snack"
                },
                new ProductDetail()
                {
                    ProductDetailID = 3,
                    ProductName = "CokeCola",
                    Description = "suger soft drink"
                },
                new ProductDetail()
                {
                    ProductDetailID = 4,
                    ProductName = "CokeCola Zero",
                    Description = "0 suger soft drink"
                },
                new ProductDetail()
                {
                    ProductDetailID = 5,
                    ProductName = "Mustard",
                    Description = "spicy snack"
                }, 
                new ProductDetail()
                {
                    ProductDetailID = 6,
                    ProductName = "Gatorade",
                    Description = "fitness drink!"
                },
            });

            this.Products[0].Details.Add(this.ProductDetails[1]);
            this.Products[0].Details.Add(this.ProductDetails[2]);
            this.Products[0].Details.Add(this.ProductDetails[3]);
            this.Products[0].Details.Add(this.ProductDetails[4]);
            this.Products[0].Details.Add(this.ProductDetails[5]);

            this.Products[1].Details.Add(this.ProductDetails[0]);

            this.ProductDetails[0].RelatedProduct = this.Products[1];

            this.ProductDetails[1].RelatedProduct = this.Products[0];
            this.ProductDetails[2].RelatedProduct = this.Products[0];
            this.ProductDetails[3].RelatedProduct = this.Products[0];
            this.ProductDetails[4].RelatedProduct = this.Products[0];
            this.ProductDetails[5].RelatedProduct = this.Products[0];

            this.ProductReviews.AddRange(new List<ProductReview>()
            {
                new ProductReview()
                {
                    ProductID = 6,
                    ProductDetailID = 1,
                    ReviewTitle = "Amazing product",
                    RevisionID = 1,
                    Author = "Joe Doe",
                    Comment = "It's very tasty"
                },
                new ProductReview()
                {
                    ProductID = 5,
                    ProductDetailID = 2,
                    ReviewTitle = "So so",
                    RevisionID = 1,
                    Author = "Food lover",
                    Comment = "Not so good as other brands"
                },
                new ProductReview()
                {
                    ProductID = 5,
                    ProductDetailID = 2,
                    ReviewTitle = "Good",
                    RevisionID = 1,
                    Author = "Dude",
                    Comment = "Tasty good!"
                },
                new ProductReview()
                {
                    ProductID = 5,
                    ProductDetailID = 2,
                    ReviewTitle = "Special",
                    RevisionID = 1,
                    Author = "Andy",
                    Comment = "It's Special!"
                },
            });

            this.ProductDetails[0].Reviews.Add(this.ProductReviews[0]);
            this.ProductDetails[0].Reviews.Add(this.ProductReviews[1]);
            this.ProductDetails[0].Reviews.Add(this.ProductReviews[2]);
            this.ProductDetails[0].Reviews.Add(this.ProductReviews[3]);
            this.ProductDetails[1].Reviews.Add(this.ProductReviews[1]);
            this.ProductDetails[1].Reviews.Add(this.ProductReviews[2]);
            this.ProductDetails[1].Reviews.Add(this.ProductReviews[3]);

            this.Calendars.AddRange(new List<Calendar>()
            {
                new Calendar()
                {
                    Day = new Date(2015, 11, 11)
                },
                new Calendar()
                {
                    Day = new Date(2015, 11, 12)
                }
            });

            this.Orders.AddRange(new List<Order>()
            {
                new Order()
                {
                    OrderID = 7,
                    CustomerForOrder = this.People.OfType<Customer>().ElementAt(1),
                    LoggedInEmployee = this.People.OfType<Employee>().ElementAt(0),
                    OrderDate = new DateTimeOffset(2011, 5, 29, 14, 21, 12, TimeSpan.FromHours(-8)),
                    ShelfLife = new TimeSpan(1),
                    OrderShelfLifes = new Collection<TimeSpan>(){new TimeSpan(1)},
                    ShipDate = new Date(2014, 8, 31),
                    ShipTime = new TimeOfDay(12, 40, 5, 50),
                },
                new Order()
                {
                    OrderID = 8,
                    CustomerForOrder = this.People.OfType<Customer>().ElementAt(0),
                    LoggedInEmployee = this.People.OfType<Employee>().ElementAt(1),
                    OrderDate = new DateTimeOffset(2011, 3, 4, 16, 3, 57, TimeSpan.FromHours(-8)),
                    ShelfLife = new TimeSpan(1),
                    OrderShelfLifes = new Collection<TimeSpan>(){new TimeSpan(1)},
                    ShipDate = new Date(2014, 8, 12),
                    ShipTime = new TimeOfDay(6, 5, 30, 0),
                },
                new Order()
                {
                    OrderID = 9,
                    CustomerForOrder = this.People.OfType<Customer>().ElementAt(1),
                    LoggedInEmployee = this.People.OfType<Employee>().ElementAt(1),
                    OrderDate = new DateTimeOffset(2011, 1, 4, 18, 3, 57, TimeSpan.FromHours(-8)),
                    ShelfLife = new TimeSpan(1),
                    OrderShelfLifes = new Collection<TimeSpan>(){new TimeSpan(1)},
                    ShipDate = new Date(2014, 6, 12),
                    ShipTime = new TimeOfDay(4, 5, 30, 0),
                }
            });

            (this.People[0] as Customer).Orders.Add(this.Orders[1]);
            (this.People[1] as Customer).Orders.AddRange(new List<Order> { this.Orders[0], this.Orders[2] });
            (this.VipCustomer as Customer).Orders.AddRange(new List<Order> { this.Orders[0], this.Orders[1] });

            this.OrderDetails.AddRange(new List<OrderDetail>()
            {
                new OrderDetail()
                {
                    OrderID = Orders[0].OrderID,
                    AssociatedOrder = Orders[0],
                    ProductID = Products[0].ProductID,
                    ProductOrdered = Products[0],
                    Quantity = 50,
                    UnitPrice = Products[0].UnitPrice,
                    OrderPlaced = DateTimeOffset.Now
                },
                new OrderDetail()
                {
                    OrderID = Orders[0].OrderID,
                    AssociatedOrder = Orders[0],
                    ProductID = Products[1].ProductID,
                    ProductOrdered = Products[1],
                    Quantity = 2,
                    UnitPrice = Products[1].UnitPrice,
                    OrderPlaced = new DateTimeOffset()
                },
                new OrderDetail()
                {
                    OrderID = Orders[1].OrderID,
                    AssociatedOrder = Orders[1],
                    ProductID = Products[1].ProductID,
                    ProductOrdered = Products[1],
                    Quantity = 5,
                    UnitPrice = Products[1].UnitPrice,
                    OrderPlaced = new DateTimeOffset(new DateTime(2000, 10, 11))
                }
            });

            this.Orders[0].OrderDetails.Add(this.OrderDetails[0]);
            this.Orders[0].OrderDetails.Add(this.OrderDetails[1]);
            this.Orders[1].OrderDetails.Add(this.OrderDetails[2]);

            (this.People[2] as Employee).CompanyID = this.Company.CompanyID;
            (this.People[3] as Employee).CompanyID = this.Company.CompanyID;
            (this.People[2] as Employee).Company = this.Company;
            (this.People[3] as Employee).Company = this.Company;
            this.Company.Employees.AddRange(new List<Employee> { this.People[2] as Employee, this.People[3] as Employee });
            this.Company.VipCustomer = VipCustomer;
            this.VipCustomer.Company = Company;

            this.Departments.AddRange(new List<Department>()
            {
                new Department()
                {
                    DepartmentID = 1,
                    Name = "D1",
                    Company = Company
                },
                new Department()
                {
                    DepartmentID = 2,
                    Name = "D2",
                    Company = Company
                }
            });
            this.Company.Departments.AddRange(new List<Department> { this.Departments[0], this.Departments[1] });
            this.Company.CoreDepartment = new Department()
            {
                DepartmentID = 3,
                Name = "D3",
                Company = Company
            };

            this.StoredPIs.AddRange(new List<StoredPI>()
            {
                new StoredPI()
                {
                    StoredPIID = 801,
                    PIName = "Default",
                    PIType = "CreditCard",
                    CreatedDate = new DateTimeOffset(new DateTime(2013, 1, 30))
                },
                new StoredPI()
                {
                    StoredPIID = 802,
                    PIName = "New One",
                    PIType = "AliPay",
                    CreatedDate = new DateTimeOffset(new DateTime(2013, 2, 1))
                },
                new StoredPI()
                {
                    StoredPIID = 803,
                    PIName = "Backup",
                    PIType = "BankAccountPay",
                    CreatedDate = new DateTimeOffset(new DateTime(2012, 12, 2))
                },
                new StoredPI()
                {
                    StoredPIID = 804,
                    PIName = "Active One",
                    PIType = "CreditCard",
                    CreatedDate = new DateTimeOffset(new DateTime(2012, 12, 26))
                },
                new StoredPI()
                {
                    StoredPIID = 805,
                    PIName = "Chris' PI",
                    PIType = "AliPay",
                    CreatedDate = new DateTimeOffset(new DateTime(2013, 2, 2))
                },
                new StoredPI()
                {
                    StoredPIID = 806,
                    PIName = "Allen's PI",
                    PIType = "StoredValue",
                    CreatedDate = new DateTimeOffset(new DateTime(2013, 12, 26))
                }
            });

            this.Accounts.AddRange(new List<Account>()
            {
                new Account()
                {
                    AccountID = 101,
                    CountryRegion = "US",
                    AccountInfo = new AccountInfo()
                    {
                        FirstName = "Alex",
                        LastName = "Green"
                    },
                    MyGiftCard = new GiftCard()
                    {
                        GiftCardID = 301,
                        GiftCardNO = "AAA123A",
                        Amount = 19.9,
                        ExperationDate = new DateTimeOffset(new DateTime(2013, 12, 30))
                    },
                    MyPaymentInstruments =
                    {
                        new PaymentInstrument()
                        {
                            PaymentInstrumentID = 101901,
                            FriendlyName = "101 first PI",
                            CreatedDate = new DateTimeOffset(new DateTime(2012, 11, 1)),
                            TheStoredPI = this.StoredPIs.GetEntity((it)=> it.StoredPIID == 802),
                            BackupStoredPI = DefaultStoredPI
                        },
                        new CreditCardPI()
                        {
                            PaymentInstrumentID = 101902,
                            FriendlyName = "101 frist credit PI",
                            CreatedDate = new DateTimeOffset(new DateTime(2012, 11, 1)),
                            TheStoredPI = this.StoredPIs.GetEntity((it)=> it.StoredPIID == 801),
                            BackupStoredPI = DefaultStoredPI,
                            CVV = "234",
                            CardNumber = "6000000000000000",
                            HolderName = "Alex",
                            Balance = 100.00,
                            ExperationDate = new DateTimeOffset(new DateTime(2022, 11, 1)),
                            CreditRecords = new List<CreditRecord>()
                            {
                                new CreditRecord()
                                {
                                    CreditRecordID = 1,
                                    IsGood = true,
                                    Reason = "Shopping",
                                    CreatedDate = new DateTimeOffset(new DateTime(2012, 11, 1))
                                },
                                new CreditRecord()
                                {
                                    CreditRecordID = 2,
                                    IsGood = false,
                                    Reason = "Rental",
                                    CreatedDate = new DateTimeOffset(new DateTime(2012, 11, 1))
                                }
                            }
                        },
                        new CreditCardPI()
                        {
                            PaymentInstrumentID = 101903,
                            FriendlyName = "101 second credit PI",
                            CreatedDate = new DateTimeOffset(new DateTime(2012, 11, 1)),
                            TheStoredPI = this.StoredPIs.GetEntity((it)=> it.StoredPIID == 804),
                            BackupStoredPI = DefaultStoredPI,
                            CVV = "012",
                            CardNumber = "8000000000000000",
                            HolderName = "James",
                            Balance = 300.00,
                            ExperationDate = new DateTimeOffset(new DateTime(2022, 10, 2)),
                            CreditRecords = new List<CreditRecord>()
                            {
                                new CreditRecord()
                                {
                                    CreditRecordID = 1,
                                    IsGood = true,
                                    Reason = "Shopping",
                                    CreatedDate = new DateTimeOffset(new DateTime(2012, 11, 1))
                                },
                                new CreditRecord()
                                {
                                    CreditRecordID = 2,
                                    IsGood = false,
                                    Reason = "Rental",
                                    CreatedDate = new DateTimeOffset(new DateTime(2012, 11, 1))
                                }
                            }
                        }
                    },
                    ActiveSubscriptions = new EntityCollection<Subscription>()
                    {
                        new Subscription()
                        {
                            SubscriptionID = 10001011,
                            TemplateGuid = "748F8F95-6683-4D30-85EE-00E05CC3A627",
                            Title = "XBox Premium",
                            Category = "Monthly",
                            CreatedDate = new DateTimeOffset(new DateTime(2013, 1, 5))
                        },
                        new Subscription()
                        {
                            SubscriptionID = 10001012,
                            TemplateGuid =  "748F8F95-6683-4D30-85EE-00E05CC3A627",
                            Title = "XBox Premium",
                            Category = "Monthly",
                            CreatedDate = new DateTimeOffset(new DateTime(2013, 2, 5))
                        },
                        new Subscription()
                        {
                            SubscriptionID = 10011013,
                            TemplateGuid = "D51862EA-7917-4817-867E-D3A3BA402865",
                            Title = "Windows Store Premium",
                            Category = "Yearly",
                            CreatedDate = new DateTimeOffset(new DateTime(2013, 5, 11))
                        }
                    }
                },
                new Account()
                {
                    AccountID = 102,
                    CountryRegion = "GB",
                    AccountInfo = new AccountInfo()
                    {
                        FirstName = "James",
                        LastName = "Bunder"
                    },
                    MyGiftCard = new GiftCard()
                    {
                        GiftCardID = 302,
                        GiftCardNO = "BBA12BB",
                        Amount = 200,
                        ExperationDate = new DateTimeOffset(new DateTime(2014, 12, 30))
                    },
                },
                new Account()
                {
                    AccountID = 103,
                    CountryRegion = "CN",
                    AccountInfo = new AccountInfo()
                    {
                        FirstName = "Adam",
                        LastName = "Homes"
                    },
                    MyGiftCard = new GiftCard()
                    {
                        GiftCardID = 303,
                        GiftCardNO = "AAB124A",
                        Amount = 1.9,
                        ExperationDate = new DateTimeOffset(new DateTime(2013, 12, 30))
                    },
                    MyPaymentInstruments =
                    {
                        new PaymentInstrument()
                        {
                            PaymentInstrumentID = 103901,
                            FriendlyName = "103 frist PI",
                            CreatedDate = new DateTimeOffset(new DateTime(2013, 10, 1)),
                            TheStoredPI = this.StoredPIs.GetEntity((it)=> it.StoredPIID == 802),
                            BackupStoredPI = DefaultStoredPI,
                            BillingStatements =
                            {
                                new Statement()
                                {
                                    StatementID =  103901001,
                                    TransactionType = "OnlinePurchase",
                                    TransactionDescription = "Digital goods: App",
                                    Amount = 100.0
                                },
                                new Statement()
                                {
                                    StatementID =  103901002,
                                    TransactionType = "OnlinePurchase",
                                    TransactionDescription = "Amazon purchase",
                                    Amount = 125.0
                                }
                            }
                        },
                        new PaymentInstrument()
                        {
                            PaymentInstrumentID = 103902,
                            FriendlyName = "103 second PI",
                            CreatedDate = new DateTimeOffset(new DateTime(2013, 1, 1)),
                            TheStoredPI = this.StoredPIs.GetEntity((it)=> it.StoredPIID == 803),
                            BackupStoredPI = DefaultStoredPI
                        },
                        new PaymentInstrument()
                        {
                            PaymentInstrumentID = 103905,
                            FriendlyName = "103 new PI",
                            CreatedDate = new DateTimeOffset(new DateTime(2013, 10, 29)),
                            TheStoredPI = this.StoredPIs.GetEntity((it)=> it.StoredPIID == 805),
                            BackupStoredPI = DefaultStoredPI
                        },
                        new PaymentInstrument()
                        {
                            PaymentInstrumentID = 101910,
                            FriendlyName = "103 backup PI",
                            CreatedDate = new DateTimeOffset(new DateTime(2013, 6, 15)),
                            TheStoredPI = this.StoredPIs.GetEntity((it)=> it.StoredPIID == 805),
                            BackupStoredPI = DefaultStoredPI
                        }
                    },
                    ActiveSubscriptions = new EntityCollection<Subscription>()
                    {
                        new Subscription()
                        {
                            SubscriptionID = 10021031,
                            TemplateGuid = "262176C4-1139-4BD5-91F7-0C30CFF1E033",
                            Title = "Windows Live",
                            Category = "Yearly",
                            CreatedDate = new DateTimeOffset(new DateTime(2013, 2, 15)),
                            QualifiedAccountID = 103
                        },
                        new Subscription()
                        {
                            SubscriptionID = 10021032,
                            TemplateGuid = "262176C4-1139-4BD5-91F7-0C30CFF1E033",
                            Title = "Windows Live",
                            Category = "Yearly",
                            CreatedDate = new DateTimeOffset(new DateTime(2012, 2, 9)),
                            QualifiedAccountID = 103
                        },
                        new Subscription()
                        {
                            SubscriptionID = 10031031,
                            TemplateGuid = "A31F4B0E-7FAD-4F1B-83FE-55DC37CFD4DF",
                            Title = "OneDrive",
                            Category = "Yearly",
                            CreatedDate = new DateTimeOffset(new DateTime(2012, 12, 15)),
                            QualifiedAccountID = 103
                        },
                    }
                },
                new Account()
                {
                    AccountID = 104,
                    CountryRegion = "CN",
                    AccountInfo = new AccountInfo()
                    {
                        FirstName = "Adrian",
                        LastName = "Green"
                    },
                },
                new Account()
                {
                    AccountID = 105,
                    CountryRegion = "US",
                    AccountInfo = new AccountInfo()
                    {
                        FirstName = "Lily",
                        LastName = "Green"
                    },
                    MyGiftCard = new GiftCard()
                    {
                        GiftCardID = 305,
                        GiftCardNO = "AAA124D",
                        Amount = 1.9,
                        ExperationDate = new DateTimeOffset(new DateTime(2014, 12, 30))
                    },
                },
                new Account()
                {
                    AccountID = 106,
                    CountryRegion = "CN",
                    AccountInfo = new AccountInfo()
                    {
                        FirstName = "Allen",
                        LastName = "Ivorson"
                    },
                },
                new Account()
                {
                    AccountID = 107,
                    CountryRegion = "FR",
                    AccountInfo = new AccountInfo()
                    {
                        FirstName = "Albert",
                        LastName = "Ivorson"
                    },
                    MyGiftCard = new GiftCard()
                    {
                        GiftCardID = 306,
                        GiftCardNO = "AAA124E",
                        Amount = 19.9,
                        ExperationDate = new DateTimeOffset(new DateTime(2013, 12, 30))
                    },
                }
            });

            Accounts[0].AccountInfo.OpenProperties["MiddleName"] = "Hood";
            Accounts[0].AccountInfo.OpenProperties["FavoriteColor"] = Color.Red;
            Accounts[0].AccountInfo.OpenProperties["Address"] = new Address
            {
                City = "a",
                Street = "b",
                PostalCode = "c"
            };

            this.SubscriptionTemplates.AddRange(new List<Subscription>()
            {
                new Subscription()
                {
                    SubscriptionID = 1000,
                    TemplateGuid = "748F8F95-6683-4D30-85EE-00E05CC3A627",
                    Title = "XBox Premium",
                    Category = "Monthly",
                    CreatedDate = new DateTimeOffset(new DateTime(2005, 1, 5)),
                    QualifiedAccountID = 101
                },
                new Subscription()
                {
                    SubscriptionID = 1001,
                    TemplateGuid = "D51862EA-7917-4817-867E-D3A3BA402865",
                    Title = "Windows Store Premium",
                    Category = "Yearly",
                    CreatedDate = new DateTimeOffset(new DateTime(2005, 2, 11)),
                    QualifiedAccountID = 101
                },
                new Subscription()
                {
                    SubscriptionID = 1002,
                    TemplateGuid = "262176C4-1139-4BD5-91F7-0C30CFF1E033",
                    Title = "Windows Live",
                    Category = "Yearly",
                    CreatedDate = new DateTimeOffset(new DateTime(2005, 2, 11)),
                    QualifiedAccountID = 103
                },
                new Subscription()
                {
                    SubscriptionID = 1003,
                    TemplateGuid = "A31F4B0E-7FAD-4F1B-83FE-55DC37CFD4DF",
                    Title = "OneDrive",
                    Category = "Yearly",
                    CreatedDate = new DateTimeOffset(new DateTime(2005, 2, 11)),
                    QualifiedAccountID = 103
                },
                new Subscription()
                {
                    SubscriptionID = 1004,
                    TemplateGuid = "3B3887C4-D383-4095-A066-F4BBAE6E87AD",
                    Title = "OCP",
                    Category = "Yearly",
                    CreatedDate = new DateTimeOffset(new DateTime(2005, 2, 11)),
                    QualifiedAccountID = 103
                }
            });
        }

        protected override IEdmModel CreateModel()
        {
            return DefaultInMemoryModel.CreateODataServiceModel("Microsoft.Test.OData.Services.ODataWCFService");
        }
    }
}