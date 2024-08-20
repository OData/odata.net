﻿//-----------------------------------------------------------------------------
// <copyright file="DefaultDataSource.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.Spatial;
using System.Collections.ObjectModel;

namespace Microsoft.OData.Client.E2E.Tests.Common.Server.Default
{
    public class DefaultDataSource
    {
        static DefaultDataSource()
        {
            Initialize();
        }

        public static Person Boss { get; private set; }
        public static Customer VipCustomer { get; private set; }
        public static Company Company { get; private set; }
        public static PublicCompany PublicCompany { get; private set; }
        public static LabourUnion LabourUnion { get; private set; }
        public static StoredPI DefaultStoredPI { get; private set; }
        public static IList<Person> People { get; private set; }
        public static IList<Account> Accounts { get; private set; }
        public static IList<Product> Products { get; private set; }
        public static IList<Order> Orders { get; private set; }
        public static IList<ProductDetail> ProductDetails { get; private set; }
        public static IList<Customer> Customers { get; private set; }
        public static IList<Employee> Employees { get; private set; }
        public static IList<OrderDetail> OrderDetails { get; private set; }
        public static IList<ProductReview> ProductReviews { get; private set; }
        public static IList<Calendar> Calendars { get; private set; }
        public static IList<Department> Departments { get; private set; }
        public static IList<StoredPI> StoredPIs { get; private set; }
        public static IList<Subscription> SubscriptionTemplates { get; private set; }

        /// <summary>
        /// Populates the data source.
        /// </summary>
        public static void Initialize()
        {
            Boss = new Customer()
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
            };

            VipCustomer = new Customer()
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

            Company = new Company()
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

            PublicCompany = new PublicCompany
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
                Assets = new List<Asset>()
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

            LabourUnion = new LabourUnion
            {
                LabourUnionID = 0,
                Name = "MS Labour Union"
            };

            PublicCompany.LabourUnion = LabourUnion;

            DefaultStoredPI = new StoredPI()
            {
                StoredPIID = 800,
                PIName = "The Default Stored PI",
                PIType = "CreditCard",
                CreatedDate = new DateTimeOffset(new DateTime(2013, 12, 31))
            };

            People = new List<Person>()
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
                        FamilyName = "Cats"
                    },
                    Addresses = new Collection<Address>
                    {
                        new HomeAddress()
                        {
                            City = "Tokyo",
                            PostalCode = "98052",
                            Street = "1 Microsoft Way",
                            FamilyName = "Cats"
                        },
                        new Address()
                        {
                            City = "Shanghai",
                            PostalCode = "200000",
                            Street = "999 Zixing Road"
                        },
                    }
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
                        FamilyName = "Zips"
                    },
                    Addresses = new Collection<Address>
                    {
                        new Address()
                        {
                            City = "Shanghai2",
                            PostalCode = "200000",
                            Street = "B01, 999 Zixing Road"
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
                            Street = "B01, 999 Zixing Road"
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
                            FamilyName = "Cats"
                        },
                        new Address()
                        {
                            City = "Shanghai",
                            PostalCode = "200000",
                            Street = "999 Zixing Road"
                        },
                    }
                }
            };

            Customers = People.OfType<Customer>().ToList();
            Employees = People.OfType<Employee>().ToList();

            People[0].Parent = People[1];
            People[1].Parent = People[3];
            People[2].Parent = People[3];

            Products = new List<Product>()
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
                    UserAccess = AccessLevel.None
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
            };

            ProductDetails = new List<ProductDetail>
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
            };

            Products[0].Details = new List<ProductDetail>() { ProductDetails[1], ProductDetails[2], ProductDetails[3], ProductDetails[4], ProductDetails[5] };

            Products[1].Details = new List<ProductDetail>() { ProductDetails[0] };

            ProductDetails[0].RelatedProduct =  Products[1];

            ProductDetails[1].RelatedProduct = Products[0];
            ProductDetails[2].RelatedProduct = Products[0];
            ProductDetails[3].RelatedProduct = Products[0];
            ProductDetails[4].RelatedProduct = Products[0];
            ProductDetails[5].RelatedProduct = Products[0];

            ProductReviews = new List<ProductReview>()
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
            };

            ProductDetails[0].Reviews = new List<ProductReview>() { ProductReviews[0] };
            ProductDetails[0].Reviews = new List<ProductReview>() { ProductReviews[1] };
            ProductDetails[0].Reviews = new List<ProductReview>() { ProductReviews[2] };
            ProductDetails[0].Reviews = new List<ProductReview>() { ProductReviews[3] };
            ProductDetails[1].Reviews = new List<ProductReview>() { ProductReviews[1] };
            ProductDetails[1].Reviews = new List<ProductReview>() { ProductReviews[2] };
            ProductDetails[1].Reviews = new List<ProductReview>() { ProductReviews[3] };

            Calendars = new List<Calendar>()
            {
                new Calendar()
                {
                    Day = new Date(2015, 11, 11)
                },
                new Calendar()
                {
                    Day = new Date(2015, 11, 12)
                }
            };

            Orders = new List<Order>()
            {
                new Order()
                {
                    OrderID = 7,
                    CustomerForOrder = People.OfType<Customer>().ElementAt(1),
                    LoggedInEmployee = People.OfType<Employee>().ElementAt(0),
                    OrderDate = new DateTimeOffset(2011, 5, 29, 14, 21, 12, TimeSpan.FromHours(-8)),
                    ShelfLife = new TimeSpan(1),
                    OrderShelfLifes = new Collection<TimeSpan>(){new TimeSpan(1)},
                    ShipDate = new Date(2014, 8, 31),
                    ShipTime = new TimeOfDay(12, 40, 5, 50),
                },
                new Order()
                {
                    OrderID = 8,
                    CustomerForOrder = People.OfType<Customer>().ElementAt(0),
                    LoggedInEmployee = People.OfType<Employee>().ElementAt(1),
                    OrderDate = new DateTimeOffset(2011, 3, 4, 16, 3, 57, TimeSpan.FromHours(-8)),
                    ShelfLife = new TimeSpan(1),
                    OrderShelfLifes = new Collection<TimeSpan>(){new TimeSpan(1)},
                    ShipDate = new Date(2014, 8, 12),
                    ShipTime = new TimeOfDay(6, 5, 30, 0),
                },
                new Order()
                {
                    OrderID = 9,
                    CustomerForOrder = People.OfType<Customer>().ElementAt(1),
                    LoggedInEmployee = People.OfType<Employee>().ElementAt(1),
                    OrderDate = new DateTimeOffset(2011, 1, 4, 18, 3, 57, TimeSpan.FromHours(-8)),
                    ShelfLife = new TimeSpan(1),
                    OrderShelfLifes = new Collection<TimeSpan>(){new TimeSpan(1)},
                    ShipDate = new Date(2014, 6, 12),
                    ShipTime = new TimeOfDay(4, 5, 30, 0),
                }
            };

            (People[0] as Customer).Orders = new List<Order>() { Orders[1] };
            (People[1] as Customer).Orders = new List<Order> { Orders[0], Orders[2] };
            (VipCustomer as Customer).Orders = new List<Order> { Orders[0], Orders[1] };

            OrderDetails = new List<OrderDetail>()
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
            };

            Orders[0].OrderDetails = new List<OrderDetail>() { OrderDetails[0] };
            Orders[0].OrderDetails = new List<OrderDetail>() { OrderDetails[1] };
            Orders[1].OrderDetails = new List<OrderDetail> { OrderDetails[2] };

            (People[2] as Employee).CompanyID = Company.CompanyID;
            (People[3] as Employee).CompanyID = Company.CompanyID;
            (People[2] as Employee).Company = Company;
            (People[3] as Employee).Company = Company;
            Company.Employees = new List<Employee>() { People[2] as Employee, People[3] as Employee };
            Company.VipCustomer = VipCustomer;
            VipCustomer.Company = Company;

            Departments = new List<Department>()
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
            };

            Company.Departments = new List<Department> { Departments[0], Departments[1] };
            Company.CoreDepartment = new Department()
            {
                DepartmentID = 3,
                Name = "D3",
                Company = Company
            };

            StoredPIs = new List<StoredPI>()
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
            };

            Accounts = new List<Account>()
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
                    MyPaymentInstruments = new List<PaymentInstrument>()
                    {
                        new PaymentInstrument()
                        {
                            PaymentInstrumentID = 101901,
                            FriendlyName = "101 first PI",
                            CreatedDate = new DateTimeOffset(new DateTime(2012, 11, 1)),
                        },
                        new CreditCardPI()
                        {
                            PaymentInstrumentID = 101902,
                            FriendlyName = "101 frist credit PI",
                            CreatedDate = new DateTimeOffset(new DateTime(2012, 11, 1)),
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
                    ActiveSubscriptions = new List<Subscription>()
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
                    MyPaymentInstruments = new List<PaymentInstrument>()
                    {
                        new PaymentInstrument()
                        {
                            PaymentInstrumentID = 103901,
                            FriendlyName = "103 frist PI",
                            CreatedDate = new DateTimeOffset(new DateTime(2013, 10, 1)),
                            BillingStatements = new List<Statement>()
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
                            CreatedDate = new DateTimeOffset(new DateTime(2013, 1, 1))
                        },
                        new PaymentInstrument()
                        {
                            PaymentInstrumentID = 103905,
                            FriendlyName = "103 new PI",
                            CreatedDate = new DateTimeOffset(new DateTime(2013, 10, 29))
                        },
                        new PaymentInstrument()
                        {
                            PaymentInstrumentID = 101910,
                            FriendlyName = "103 backup PI",
                            CreatedDate = new DateTimeOffset(new DateTime(2013, 6, 15))
                        }
                    },
                    ActiveSubscriptions = new List<Subscription>()
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

            };
            Accounts[0].AccountInfo.DynamicProperties["MiddleName"] = "Hood";
            Accounts[0].AccountInfo.DynamicProperties["FavoriteColor"] = Color.Red;
            Accounts[0].AccountInfo.DynamicProperties["Address"] = new Address
            {
                City = "a",
                Street = "b",
                PostalCode = "c"
            };

            SubscriptionTemplates = new List<Subscription>()
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
            };
        }

        /// <summary>
        /// Resets the data source
        /// </summary>
        public static void ResetDataSource()
        {
            Boss = new Person();
            VipCustomer = new Customer();
            Company = new Company();
            PublicCompany = new PublicCompany();
            LabourUnion = new LabourUnion();
            DefaultStoredPI = new StoredPI();
            People.Clear();
            Accounts.Clear();
            Products.Clear();
            Orders.Clear();
            ProductDetails.Clear();
            Customers.Clear();
            Employees.Clear();
            OrderDetails.Clear();
            ProductReviews.Clear();
            Calendars.Clear();
            Departments.Clear();
            StoredPIs.Clear();
            SubscriptionTemplates.Clear();
        }
    }
}
