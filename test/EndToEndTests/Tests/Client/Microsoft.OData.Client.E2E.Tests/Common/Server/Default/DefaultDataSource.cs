//-----------------------------------------------------------------------------
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

        public static IList<Person> People { get; private set; }
        public static IList<Account> Accounts { get; private set; }
        public static IList<Product> Products { get; private set; }
        public static IList<Order> Orders { get; private set; }
        public static IList<ProductDetail> ProductDetails { get; private set; }
        public static IList<Customer> Customers { get; private set; }
        public static IList<OrderDetail> OrderDetails { get; private set; }

        /// <summary>
        /// Populates the data source.
        /// </summary>
        private static void Initialize()
        {
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
                        }
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

            Customers = People.OfType<Customer>().ToList();

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
        }
    }
}
