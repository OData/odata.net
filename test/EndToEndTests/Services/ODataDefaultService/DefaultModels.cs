//---------------------------------------------------------------------
// <copyright file="DefaultModels.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    public class Address : ClrObject
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }

    public class HomeAddress : Address
    {
        public string FamilyName { get; set; }
    }

    public class CompanyAddress : Address
    {
        public string CompanyName { get; set; }
    }

    public class CityInformation : ClrObject
    {
        public string CountryRegion { get; set; }
        public bool IsCapital { get; set; }
    }

    /// <summary>
    /// The class represents the Color model type, which is an enum type
    /// </summary>
    public enum Color
    {
        Red = 1,
        Green = 2,
        Blue = 4
    }

    /// <summary>
    /// The class represents the AccessLevel model type, which is a flags enum type
    /// </summary>
    [Flags]
    public enum AccessLevel
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4,
        ReadWrite = 3
    }

    /// <summary>
    /// The class represents company category, which is an enum type
    /// </summary>
    public enum CompanyCategory
    {
        IT = 0,
        Communication = 1,
        Electronics = 2,
        Others = 4
    }

    /// <summary>
    /// The class represents the Person model type.
    /// </summary>
    public class Person : ClrObject
    {
        private Person parent;

        public int PersonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public Collection<string> Numbers { get; set; }
        public Collection<string> Emails { get; set; }
        public Collection<Address> Addresses { get; set; }
        public Address HomeAddress { get; set; }
        public GeographyPoint Home { get; set; }

        public Person()
        {
            Addresses = new Collection<Address>();
        }

        public Person Parent
        {
            get
            {
                if (DataSourceManager.GetCurrentDataSource<DefaultWCFSvcDataSource>().People.Contains(this.parent))
                {
                    DeletionContext.Current.BindAction(this.parent, () => this.parent = null);
                }
                else
                {
                    this.parent = null;
                }

                return this.parent;
            }
            set
            {
                DataSourceManager.GetCurrentDataSource<DefaultWCFSvcDataSource>().People.TryAdd(value);
                this.parent = value;
            }
        }

        public void SetNumbers(ODataCollectionValue values)
        {
            this.Numbers = new Collection<string>();
            foreach (var item in values.Items)
            {
                this.Numbers.Add(item.ToString());
            }
        }

        public void SetEmails(ODataCollectionValue values)
        {
            this.Emails = new Collection<string>();
            foreach (var item in values.Items)
            {
                this.Emails.Add(item != null ? item.ToString() : null);
            }
        }
    }

    /// <summary>
    /// The class represents the Customer model type.
    /// </summary>
    public class Customer : Person
    {
        private EntityCollection<Order> orders;

        public Customer()
        {
            orders = new EntityCollection<Order>(DataSourceManager.GetCurrentDataSource<DefaultWCFSvcDataSource>().Orders);
        }

        public string City { get; set; }
        public DateTimeOffset Birthday { get; set; }
        public TimeSpan TimeBetweenLastTwoOrders { get; set; }
        public Company Company { get; set; }

        public EntityCollection<Order> Orders
        {
            get
            {
                return this.orders.Cleanup();
            }
        }
    }

    public abstract class AbstractEntity : ClrObject
    {
        
    }

    /// <summary>
    /// The class represents the Order model type.
    /// </summary>
    public class Order : AbstractEntity
    {
        private EntityCollection<OrderDetail> orderDetails;

        public Order()
        {
            orderDetails = new EntityCollection<OrderDetail>(DataSourceManager.GetCurrentDataSource<DefaultWCFSvcDataSource>().OrderDetails);
        }

        public int OrderID { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public Employee LoggedInEmployee { get; set; }
        public Customer CustomerForOrder { get; set; }
        public TimeSpan? ShelfLife { get; set; }
        public Collection<TimeSpan> OrderShelfLifes { get; set; }
        public Date ShipDate { get; set; }
        public TimeOfDay ShipTime { get; set; }

        public EntityCollection<OrderDetail> OrderDetails
        {
            get
            {
                return this.orderDetails.Cleanup();
            }
        }

        public void SetOrderShelfLifes(ODataCollectionValue values)
        {
            this.OrderShelfLifes = new Collection<TimeSpan>();
            foreach (var item in values.Items)
            {
                this.OrderShelfLifes.Add((TimeSpan)item);
            }
        }
    }

    /// <summary>
    /// The class represents the Calendar model type.
    /// </summary>
    public class Calendar : AbstractEntity
    {
        public Calendar()
        {
        }

        public Date Day { get; set; }
    }

    /// <summary>
    /// The class represents the OrderDetail model type.
    /// </summary>
    public class OrderDetail : AbstractEntity
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public DateTimeOffset OrderPlaced { get; set; }
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }
        public Product ProductOrdered { get; set; }
        public Order AssociatedOrder { get; set; }
    }

    /// <summary>
    /// The class represents the Product model type.
    /// </summary>
    public class Product : ClrObject
    {
        private EntityCollection<ProductDetail> details;

        public Product()
        {
            details = new EntityCollection<ProductDetail>(DataSourceManager.GetCurrentDataSource<DefaultWCFSvcDataSource>().ProductDetails);
        }

        public int ProductID { get; set; }
        public string Name { get; set; }
        public string QuantityPerUnit { get; set; }
        public float UnitPrice { get; set; }
        public int QuantityInStock { get; set; }
        public bool Discontinued { get; set; }
        public Color? SkinColor { get; set; }
        public Collection<Color> CoverColors { get; set; }
        public AccessLevel? UserAccess { get; set; }

        public EntityCollection<ProductDetail> Details
        {
            get
            {
                return this.details.Cleanup();
            }
        }

        public void SetCoverColors(ODataCollectionValue values)
        {
            this.CoverColors = new Collection<Color>();
            foreach (var item in values.Items)
            {
                ODataEnumValue enumItem = item as ODataEnumValue;
                this.CoverColors.Add((Color)Enum.Parse(typeof(Color), enumItem.Value));
            }
        }
    }

    /// <summary>
    /// The class represents the ProductDetail model type.
    /// </summary>
    public class ProductDetail : ClrObject
    {
        private EntityCollection<ProductReview> reviews;

        public ProductDetail()
        {
            reviews = new EntityCollection<ProductReview>(DataSourceManager.GetCurrentDataSource<DefaultWCFSvcDataSource>().ProductReviews);
        }

        public int ProductDetailID { get; set; }
        public string ProductName { get; set; }
        [SearchField]
        public string Description { get; set; }
        public Product RelatedProduct { get; set; }

        public int ProductID
        {
            get
            {
                if (this.RelatedProduct != null)
                {
                    return this.RelatedProduct.ProductID;
                }

                return 0;
            }
        }

        public EntityCollection<ProductReview> Reviews
        {
            get
            {
                return this.reviews.Cleanup();
            }
        }
    }

    /// <summary>
    /// The class represents the ProductReview model type.
    /// </summary>
    public class ProductReview : ClrObject
    {
        public int ProductID { get; set; }
        public int ProductDetailID { get; set; }
        public string ReviewTitle { get; set; }
        public int RevisionID { get; set; }
        [SearchField]
        public string Comment { get; set; }
        public string Author { get; set; }
    }

    /// <summary>
    /// The class represents the Employee model type.
    /// </summary>
    public class Employee : Person
    {
        public DateTimeOffset DateHired { get; set; }
        public GeographyPoint Office { get; set; }
        public int CompanyID { get; set; }
        public Company Company { get; set; }
    }

    /// <summary>
    /// The class represents the Company model type.
    /// </summary>
    public class Company : OpenClrObject
    {
        private EntityCollection<Department> departments;
        private Customer vipCusomter;
        private Department coreDepartment;
        public Company()
        {
            this.Employees = new EntityCollection<Employee>(DataSourceManager.GetCurrentDataSource<DefaultWCFSvcDataSource>().Employees);
            this.departments = new EntityCollection<Department>(DataSourceManager.GetCurrentDataSource<DefaultWCFSvcDataSource>().Departments);
        }

        public int CompanyID { get; set; }
        public CompanyCategory CompanyCategory { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public Int64 Revenue { get; set; }
        public EntityCollection<Employee> Employees { get; private set; }
        public Department CoreDepartment
        {
            get
            {
                DeletionContext.Current.BindAction(this.coreDepartment, () => this.coreDepartment = null);
                return this.coreDepartment;
            }
            set
            {
                this.coreDepartment = value;
            }
        }
        public Customer VipCustomer
        {
            get
            {
                DeletionContext.Current.BindAction(this.vipCusomter, () => this.vipCusomter = null);
                return this.vipCusomter;
            }
            set
            {
                this.vipCusomter = value;
            }
        }

        public EntityCollection<Department> Departments
        {
            get
            {
                return this.departments.Cleanup();
            }
        }
    }

    /// <summary>
    /// The class represents the PublicCompany model type.
    /// </summary>
    public class PublicCompany : Company
    {
        public PublicCompany()
        {
            this.Assets = new EntityCollection<Asset>();
        }
        public string StockExchange { get; set; }
        public EntityCollection<Asset> Assets { get; set; }
        public Club Club { get; set; }
        public LabourUnion LabourUnion { get; set; }
    }

    /// <summary>
    /// The class represents the Club model type.
    /// </summary>
    public class Club : ClrObject
    {
        public int ClubID { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// The class represents the LabourUnion model type.
    /// </summary>
    public class LabourUnion : ClrObject
    {
        public int LabourUnionID { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// The class represents the Asset model type.
    /// </summary>
    public class Asset : ClrObject
    {
        public int AssetID { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }

    /// <summary>
    /// The class represents the Department model type.
    /// </summary>
    public class Department : ClrObject
    {
        public int DepartmentID { get; set; }
        public string Name { get; set; }
        public string DepartmentNO { get; set; }
        public Company Company { get; set; }
    }

    #region For Containment
    /// <summary>
    /// The class represents the Account model type.
    /// </summary>
    public class Account : ClrObject
    {
        public Account()
        {
            this.MyPaymentInstruments = new EntityCollection<PaymentInstrument>();
            this.ActiveSubscriptions = new EntityCollection<Subscription>();
        }
        private GiftCard myGiftCard;

        public int AccountID { get; set; }
        public string CountryRegion { get; set; }
        public AccountInfo AccountInfo { get; set; }

        public EntityCollection<PaymentInstrument> MyPaymentInstruments { get; private set; }
        public EntityCollection<Subscription> ActiveSubscriptions { get; set; }
        public GiftCard MyGiftCard
        {
            get
            {
                DeletionContext.Current.BindAction(this.myGiftCard, () => this.myGiftCard = null);
                return this.myGiftCard;
            }
            set
            {
                this.myGiftCard = value;
            }
        }
    }

    /// <summary>
    /// The class represents the GiftCard model type.
    /// </summary>
    public class GiftCard : ClrObject
    {
        public int GiftCardID { get; set; }
        public string GiftCardNO { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset ExperationDate { get; set; }
        public string OwnerName { get; set; }
    }

    /// <summary>
    /// The class represents the PaymentInstrument model type.
    /// </summary>
    public class PaymentInstrument : ClrObject
    {
        private StoredPI theStoredPI;
        private StoredPI backupStoredPI;

        public PaymentInstrument()
        {
            this.BillingStatements = new EntityCollection<Statement>();
        }

        public int PaymentInstrumentID { get; set; }
        public string FriendlyName { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public EntityCollection<Statement> BillingStatements { get; private set; }

        public StoredPI TheStoredPI
        {
            get
            {
                DeletionContext.Current.BindAction(this.theStoredPI, () => { this.theStoredPI = null; });
                return this.theStoredPI;
            }

            set
            {
                this.theStoredPI = value;
            }
        }

        public StoredPI BackupStoredPI
        {
            get
            {
                DeletionContext.Current.BindAction(this.backupStoredPI, () => { this.backupStoredPI = null; });
                return this.backupStoredPI;
            }
            set
            {
                this.backupStoredPI = value;
            }
        }
    }


    /// <summary>
    /// The class represents the CreditCard model type.
    /// </summary>
    public class CreditCardPI : PaymentInstrument
    {
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string HolderName { get; set; }
        public double Balance { get; set; }
        public DateTimeOffset ExperationDate { get; set; }
        public List<CreditRecord> CreditRecords { get; set; }
    }

    /// <summary>
    /// The class represents the StoredPI model type.
    /// </summary>
    public class StoredPI : ClrObject
    {
        public int StoredPIID { get; set; }
        public string PIName { get; set; }
        public string PIType { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }

    /// <summary>
    /// The class represents the Statement model type.
    /// </summary>
    public class Statement : ClrObject
    {
        public int StatementID { get; set; }
        public string TransactionType { get; set; }
        public string TransactionDescription { get; set; }
        public double Amount { get; set; }
    }

    /// <summary>
    /// The class represents the CreditRecord model type.
    /// </summary>
    public class CreditRecord : ClrObject
    {
        public int CreditRecordID { get; set; }
        public bool IsGood { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }

    /// <summary>
    /// The class represents the Subscription model type.
    /// </summary>
    public class Subscription : ClrObject
    {
        public int SubscriptionID { get; set; }
        public string TemplateGuid { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public int QualifiedAccountID { get; set; }
    }

    /// <summary>
    /// The class represents the AccountInfo model type.
    /// </summary>
    public class AccountInfo : OpenClrObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    #endregion

    #region Annotation
    public class IsBossAnnotation : InstanceAnnotationType
    {
        public IsBossAnnotation(string ns, bool value)
            : base(ns, "IsBoss", value, null)
        {
        }
    }

    public class AddressTypeAnnotation : InstanceAnnotationType
    {
        public AddressTypeAnnotation(string ns, string value)
            : base(ns, "AddressType", value, null)
        {
        }
    }

    public class CityInfoAnnotation : InstanceAnnotationType
    {
        public CityInfoAnnotation(string ns, CityInformation value, string target)
            : base(ns, "CityInfo", value, target)
        {
        }
    }

    public class DisplayNameAnnotation : InstanceAnnotationType
    {
        public DisplayNameAnnotation(string ns, string value, string target)
            : base(ns, "DisplayName", value, target)
        {
        }
    }
    #endregion
}
