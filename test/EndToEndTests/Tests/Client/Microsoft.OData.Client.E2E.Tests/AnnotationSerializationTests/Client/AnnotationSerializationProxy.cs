//---------------------------------------------------------------------
// <copyright file="AnnotationSerializationProxy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;

namespace Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models
{
    /// <summary>
    /// There are no comments for OrderSingle in the schema.
    /// </summary>
    [global::Microsoft.OData.Client.OriginalNameAttribute("OrderSingle")]
    public partial class OrderSingle : global::Microsoft.OData.Client.DataServiceQuerySingle<Order>
    {
        /// <summary>
        /// Initialize a new OrderSingle object.
        /// </summary>
        public OrderSingle(global::Microsoft.OData.Client.DataServiceContext context, string path)
            : base(context, path) { }

        /// <summary>
        /// Initialize a new OrderSingle object.
        /// </summary>
        public OrderSingle(global::Microsoft.OData.Client.DataServiceContext context, string path, bool isComposable)
            : base(context, path, isComposable) { }

        /// <summary>
        /// Initialize a new OrderSingle object.
        /// </summary>
        public OrderSingle(global::Microsoft.OData.Client.DataServiceQuerySingle<Order> query)
            : base(query) { }

    }
    /// <summary>
    /// There are no comments for Order in the schema.
    /// </summary>
    /// <KeyProperties>
    /// Id
    /// </KeyProperties>
    [global::Microsoft.OData.Client.Key("Id")]
    [global::Microsoft.OData.Client.OriginalNameAttribute("Order")]
    public partial class Order : global::Microsoft.OData.Client.BaseEntityType, global::System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Create a new Order object.
        /// </summary>
        /// <param name="ID">Initial value of Id.</param>
        /// <param name="status">Initial value of Status.</param>
        /// <param name="amount">Initial value of Amount.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public static Order CreateOrder(int ID, global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderStatus status, decimal amount)
        {
            Order order = new Order();
            order.Id = ID;
            order.Status = status;
            order.Amount = amount;
            return order;
        }
        /// <summary>
        /// There are no comments for Property Id in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("Id")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Id is required.")]
        public virtual int Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                this.OnIdChanging(value);
                this._Id = value;
                this.OnIdChanged();
                this.OnPropertyChanged("Id");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private int _Id;
        partial void OnIdChanging(int value);
        partial void OnIdChanged();
        /// <summary>
        /// There are no comments for Property Status in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("Status")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Status is required.")]
        public virtual global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderStatus Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                this.OnStatusChanging(value);
                this._Status = value;
                this.OnStatusChanged();
                this.OnPropertyChanged("Status");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderStatus _Status;
        partial void OnStatusChanging(global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderStatus value);
        partial void OnStatusChanged();
        /// <summary>
        /// There are no comments for Property StatusHistory in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("StatusHistory")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "StatusHistory is required.")]
        public virtual global::System.Collections.ObjectModel.ObservableCollection<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderStatus> StatusHistory
        {
            get
            {
                return this._StatusHistory;
            }
            set
            {
                this.OnStatusHistoryChanging(value);
                this._StatusHistory = value;
                this.OnStatusHistoryChanged();
                this.OnPropertyChanged("StatusHistory");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::System.Collections.ObjectModel.ObservableCollection<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderStatus> _StatusHistory = new global::System.Collections.ObjectModel.ObservableCollection<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderStatus>();
        partial void OnStatusHistoryChanging(global::System.Collections.ObjectModel.ObservableCollection<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderStatus> value);
        partial void OnStatusHistoryChanged();
        /// <summary>
        /// There are no comments for Property Tags in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("Tags")]
        public virtual global::System.Collections.ObjectModel.ObservableCollection<string> Tags
        {
            get
            {
                return this._Tags;
            }
            set
            {
                this.OnTagsChanging(value);
                this._Tags = value;
                this.OnTagsChanged();
                this.OnPropertyChanged("Tags");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::System.Collections.ObjectModel.ObservableCollection<string> _Tags = new global::System.Collections.ObjectModel.ObservableCollection<string>();
        partial void OnTagsChanging(global::System.Collections.ObjectModel.ObservableCollection<string> value);
        partial void OnTagsChanged();
        /// <summary>
        /// There are no comments for Property Amount in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("Amount")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Amount is required.")]
        public virtual decimal Amount
        {
            get
            {
                return this._Amount;
            }
            set
            {
                this.OnAmountChanging(value);
                this._Amount = value;
                this.OnAmountChanged();
                this.OnPropertyChanged("Amount");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private decimal _Amount;
        partial void OnAmountChanging(decimal value);
        partial void OnAmountChanged();
        /// <summary>
        /// There are no comments for Property ShippingAddress in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("ShippingAddress")]
        public virtual global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Address ShippingAddress
        {
            get
            {
                return this._ShippingAddress;
            }
            set
            {
                this.OnShippingAddressChanging(value);
                this._ShippingAddress = value;
                this.OnShippingAddressChanged();
                this.OnPropertyChanged("ShippingAddress");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Address _ShippingAddress;
        partial void OnShippingAddressChanging(global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Address value);
        partial void OnShippingAddressChanged();
        /// <summary>
        /// There are no comments for Property WarehouseAddresses in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("WarehouseAddresses")]
        public virtual global::System.Collections.ObjectModel.ObservableCollection<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Address> WarehouseAddresses
        {
            get
            {
                return this._WarehouseAddresses;
            }
            set
            {
                this.OnWarehouseAddressesChanging(value);
                this._WarehouseAddresses = value;
                this.OnWarehouseAddressesChanged();
                this.OnPropertyChanged("WarehouseAddresses");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::System.Collections.ObjectModel.ObservableCollection<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Address> _WarehouseAddresses = new global::System.Collections.ObjectModel.ObservableCollection<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Address>();
        partial void OnWarehouseAddressesChanging(global::System.Collections.ObjectModel.ObservableCollection<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Address> value);
        partial void OnWarehouseAddressesChanged();
        /// <summary>
        /// There are no comments for Property DynamicProperties in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("DynamicProperties")]
        public virtual global::System.Collections.Generic.IDictionary<string, object> DynamicProperties
        {
            get
            {
                return this._DynamicProperties;
            }
            set
            {
                this.OnDynamicPropertiesChanging(value);
                this._DynamicProperties = value;
                this.OnDynamicPropertiesChanged();
                this.OnPropertyChanged("DynamicProperties");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::System.Collections.Generic.IDictionary<string, object> _DynamicProperties = new global::System.Collections.Generic.Dictionary<string, object>();
        partial void OnDynamicPropertiesChanging(global::System.Collections.Generic.IDictionary<string, object> value);
        partial void OnDynamicPropertiesChanged();
        /// <summary>
        /// This event is raised when the value of the property is changed
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// The value of the property is changed
        /// </summary>
        /// <param name="property">property name</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        protected virtual void OnPropertyChanged(string property)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
    }
    /// <summary>
    /// There are no comments for Address in the schema.
    /// </summary>
    [global::Microsoft.OData.Client.OriginalNameAttribute("Address")]
    public partial class Address : global::System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Create a new Address object.
        /// </summary>
        /// <param name="street">Initial value of Street.</param>
        /// <param name="city">Initial value of City.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public static Address CreateAddress(string street, global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.City city)
        {
            Address address = new Address();
            address.Street = street;
            if ((city == null))
            {
                throw new global::System.ArgumentNullException("city");
            }
            address.City = city;
            return address;
        }
        /// <summary>
        /// There are no comments for Property Street in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("Street")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Street is required.")]
        public virtual string Street
        {
            get
            {
                return this._Street;
            }
            set
            {
                this.OnStreetChanging(value);
                this._Street = value;
                this.OnStreetChanged();
                this.OnPropertyChanged("Street");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private string _Street;
        partial void OnStreetChanging(string value);
        partial void OnStreetChanged();
        /// <summary>
        /// There are no comments for Property City in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("City")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "City is required.")]
        public virtual global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.City City
        {
            get
            {
                return this._City;
            }
            set
            {
                this.OnCityChanging(value);
                this._City = value;
                this.OnCityChanged();
                this.OnPropertyChanged("City");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.City _City;
        partial void OnCityChanging(global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.City value);
        partial void OnCityChanged();
        /// <summary>
        /// There are no comments for Property DynamicProperties in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("DynamicProperties")]
        public virtual global::System.Collections.Generic.IDictionary<string, object> DynamicProperties
        {
            get
            {
                return this._DynamicProperties;
            }
            set
            {
                this.OnDynamicPropertiesChanging(value);
                this._DynamicProperties = value;
                this.OnDynamicPropertiesChanged();
                this.OnPropertyChanged("DynamicProperties");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::System.Collections.Generic.IDictionary<string, object> _DynamicProperties = new global::System.Collections.Generic.Dictionary<string, object>();
        partial void OnDynamicPropertiesChanging(global::System.Collections.Generic.IDictionary<string, object> value);
        partial void OnDynamicPropertiesChanged();
        /// <summary>
        /// This event is raised when the value of the property is changed
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// The value of the property is changed
        /// </summary>
        /// <param name="property">property name</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        protected virtual void OnPropertyChanged(string property)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
    }
    /// <summary>
    /// There are no comments for City in the schema.
    /// </summary>
    [global::Microsoft.OData.Client.OriginalNameAttribute("City")]
    public partial class City : global::System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Create a new City object.
        /// </summary>
        /// <param name="name">Initial value of Name.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public static City CreateCity(string name)
        {
            City city = new City();
            city.Name = name;
            return city;
        }
        /// <summary>
        /// There are no comments for Property Name in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("Name")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Name is required.")]
        public virtual string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this.OnNameChanging(value);
                this._Name = value;
                this.OnNameChanged();
                this.OnPropertyChanged("Name");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private string _Name;
        partial void OnNameChanging(string value);
        partial void OnNameChanged();
        /// <summary>
        /// There are no comments for Property DynamicProperties in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("DynamicProperties")]
        public virtual global::System.Collections.Generic.IDictionary<string, object> DynamicProperties
        {
            get
            {
                return this._DynamicProperties;
            }
            set
            {
                this.OnDynamicPropertiesChanging(value);
                this._DynamicProperties = value;
                this.OnDynamicPropertiesChanged();
                this.OnPropertyChanged("DynamicProperties");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::System.Collections.Generic.IDictionary<string, object> _DynamicProperties = new global::System.Collections.Generic.Dictionary<string, object>();
        partial void OnDynamicPropertiesChanging(global::System.Collections.Generic.IDictionary<string, object> value);
        partial void OnDynamicPropertiesChanged();
        /// <summary>
        /// This event is raised when the value of the property is changed
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// The value of the property is changed
        /// </summary>
        /// <param name="property">property name</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        protected virtual void OnPropertyChanged(string property)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
    }
    /// <summary>
    /// There are no comments for State in the schema.
    /// </summary>
    [global::Microsoft.OData.Client.OriginalNameAttribute("State")]
    public partial class State : global::System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Create a new State object.
        /// </summary>
        /// <param name="name">Initial value of Name.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public static State CreateState(string name)
        {
            State state = new State();
            state.Name = name;
            return state;
        }
        /// <summary>
        /// There are no comments for Property Name in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("Name")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Name is required.")]
        public virtual string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this.OnNameChanging(value);
                this._Name = value;
                this.OnNameChanged();
                this.OnPropertyChanged("Name");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private string _Name;
        partial void OnNameChanging(string value);
        partial void OnNameChanged();
        /// <summary>
        /// This event is raised when the value of the property is changed
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// The value of the property is changed
        /// </summary>
        /// <param name="property">property name</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        protected virtual void OnPropertyChanged(string property)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
    }
    /// <summary>
    /// There are no comments for VipOrderSingle in the schema.
    /// </summary>
    [global::Microsoft.OData.Client.OriginalNameAttribute("VipOrderSingle")]
    public partial class VipOrderSingle : global::Microsoft.OData.Client.DataServiceQuerySingle<VipOrder>
    {
        /// <summary>
        /// Initialize a new VipOrderSingle object.
        /// </summary>
        public VipOrderSingle(global::Microsoft.OData.Client.DataServiceContext context, string path)
            : base(context, path) { }

        /// <summary>
        /// Initialize a new VipOrderSingle object.
        /// </summary>
        public VipOrderSingle(global::Microsoft.OData.Client.DataServiceContext context, string path, bool isComposable)
            : base(context, path, isComposable) { }

        /// <summary>
        /// Initialize a new VipOrderSingle object.
        /// </summary>
        public VipOrderSingle(global::Microsoft.OData.Client.DataServiceQuerySingle<VipOrder> query)
            : base(query) { }

    }
    /// <summary>
    /// There are no comments for VipOrder in the schema.
    /// </summary>
    /// <KeyProperties>
    /// Id
    /// </KeyProperties>
    [global::Microsoft.OData.Client.Key("Id")]
    [global::Microsoft.OData.Client.OriginalNameAttribute("VipOrder")]
    public partial class VipOrder : Order
    {
        /// <summary>
        /// Create a new VipOrder object.
        /// </summary>
        /// <param name="ID">Initial value of Id.</param>
        /// <param name="status">Initial value of Status.</param>
        /// <param name="amount">Initial value of Amount.</param>
        /// <param name="trackingNumber">Initial value of TrackingNumber.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public static VipOrder CreateVipOrder(int ID, global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderStatus status, decimal amount, int trackingNumber)
        {
            VipOrder vipOrder = new VipOrder();
            vipOrder.Id = ID;
            vipOrder.Status = status;
            vipOrder.Amount = amount;
            vipOrder.TrackingNumber = trackingNumber;
            return vipOrder;
        }
        /// <summary>
        /// There are no comments for Property TrackingNumber in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("TrackingNumber")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "TrackingNumber is required.")]
        public virtual int TrackingNumber
        {
            get
            {
                return this._TrackingNumber;
            }
            set
            {
                this.OnTrackingNumberChanging(value);
                this._TrackingNumber = value;
                this.OnTrackingNumberChanged();
                this.OnPropertyChanged("TrackingNumber");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private int _TrackingNumber;
        partial void OnTrackingNumberChanging(int value);
        partial void OnTrackingNumberChanged();
    }
    /// <summary>
    /// There are no comments for VipAddress in the schema.
    /// </summary>
    [global::Microsoft.OData.Client.OriginalNameAttribute("VipAddress")]
    public partial class VipAddress : Address
    {
        /// <summary>
        /// Create a new VipAddress object.
        /// </summary>
        /// <param name="name">Initial value of Name.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public static VipAddress CreateVipAddress(string street, global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.City city)
        {
            VipAddress vipAddress = new VipAddress();
            vipAddress.Street = street;
            if ((city == null))
            {
                throw new global::System.ArgumentNullException("city");
            }
            vipAddress.City = city;
            return vipAddress;
        }
        /// <summary>
        /// There are no comments for Property PostalCode in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("PostalCode")]
        public virtual string PostalCode
        {
            get
            {
                return this._PostalCode;
            }
            set
            {
                this.OnPostalCodeChanging(value);
                this._PostalCode = value;
                this.OnPostalCodeChanged();
                this.OnPropertyChanged("PostalCode");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private string _PostalCode;
        partial void OnPostalCodeChanging(string value);
        partial void OnPostalCodeChanged();
    }
    /// <summary>
    /// There are no comments for VipCity in the schema.
    /// </summary>
    [global::Microsoft.OData.Client.OriginalNameAttribute("VipCity")]
    public partial class VipCity : City
    {
        /// <summary>
        /// Create a new VipCity object.
        /// </summary>
        /// <param name="name">Initial value of Name.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public static VipCity CreateVipCity(string name)
        {
            VipCity vipCity = new VipCity();
            vipCity.Name = name;
            return vipCity;
        }
        /// <summary>
        /// There are no comments for Property AreaCode in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("AreaCode")]
        public virtual string AreaCode
        {
            get
            {
                return this._AreaCode;
            }
            set
            {
                this.OnAreaCodeChanging(value);
                this._AreaCode = value;
                this.OnAreaCodeChanged();
                this.OnPropertyChanged("AreaCode");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private string _AreaCode;
        partial void OnAreaCodeChanging(string value);
        partial void OnAreaCodeChanged();
    }
    /// <summary>
    /// There are no comments for VipState in the schema.
    /// </summary>
    [global::Microsoft.OData.Client.OriginalNameAttribute("VipState")]
    public partial class VipState : State
    {
        /// <summary>
        /// Create a new VipState object.
        /// </summary>
        /// <param name="name">Initial value of Name.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public static VipState CreateVipState(string name)
        {
            VipState vipState = new VipState();
            vipState.Name = name;
            return vipState;
        }
        /// <summary>
        /// There are no comments for Property TwoLetterCode in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("TwoLetterCode")]
        public virtual string TwoLetterCode
        {
            get
            {
                return this._TwoLetterCode;
            }
            set
            {
                this.OnTwoLetterCodeChanging(value);
                this._TwoLetterCode = value;
                this.OnTwoLetterCodeChanged();
                this.OnPropertyChanged("TwoLetterCode");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private string _TwoLetterCode;
        partial void OnTwoLetterCodeChanging(string value);
        partial void OnTwoLetterCodeChanged();
    }
    /// <summary>
    /// There are no comments for OrderStatus in the schema.
    /// </summary>
    [global::Microsoft.OData.Client.OriginalNameAttribute("OrderStatus")]
    public enum OrderStatus
    {
        [global::Microsoft.OData.Client.OriginalNameAttribute("Pending")]
        Pending = 0,
        [global::Microsoft.OData.Client.OriginalNameAttribute("Processing")]
        Processing = 1,
        [global::Microsoft.OData.Client.OriginalNameAttribute("Shipped")]
        Shipped = 2,
        [global::Microsoft.OData.Client.OriginalNameAttribute("Delivered")]
        Delivered = 3,
        [global::Microsoft.OData.Client.OriginalNameAttribute("Cancelled")]
        Cancelled = 4,
        [global::Microsoft.OData.Client.OriginalNameAttribute("Returned")]
        Returned = 5
    }
    /// <summary>
    /// Class containing all extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get an entity of type global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Order as global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="_keys">dictionary with the names and values of keys</param>
        public static global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Order> _source, global::System.Collections.Generic.IDictionary<string, object> _keys)
        {
            return new global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
        /// <summary>
        /// Get an entity of type global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Order as global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Order> _source,
            int id)
        {
            global::System.Collections.Generic.IDictionary<string, object> _keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.OrderSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
        /// <summary>
        /// Get an entity of type global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrder as global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrderSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="_keys">dictionary with the names and values of keys</param>
        public static global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrderSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrder> _source, global::System.Collections.Generic.IDictionary<string, object> _keys)
        {
            return new global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrderSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
        /// <summary>
        /// Get an entity of type global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrder as global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrderSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrderSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrder> _source,
            int id)
        {
            global::System.Collections.Generic.IDictionary<string, object> _keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrderSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
        /// <summary>
        /// Cast an entity of type global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Order to its derived type global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrder
        /// </summary>
        /// <param name="_source">source entity</param>
        public static global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrderSingle CastToVipOrder(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Order> _source)
        {
            global::Microsoft.OData.Client.DataServiceQuerySingle<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrder> query = _source.CastTo<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrder>();
            return new global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.VipOrderSingle(_source.Context, query.GetPath(null));
        }
    }
}
namespace Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Default
{
    /// <summary>
    /// There are no comments for Container in the schema.
    /// </summary>
    [global::Microsoft.OData.Client.OriginalNameAttribute("Container")]
    public partial class Container : global::Microsoft.OData.Client.DataServiceContext
    {
        /// <summary>
        /// Initialize a new Container object.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public Container(global::System.Uri serviceRoot) :
                this(serviceRoot, global::Microsoft.OData.Client.ODataProtocolVersion.V4)
        {
        }

        /// <summary>
        /// Initialize a new Container object.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public Container(global::System.Uri serviceRoot, global::Microsoft.OData.Client.ODataProtocolVersion protocolVersion) :
                base(serviceRoot, protocolVersion)
        {
            this.ResolveName = new global::System.Func<global::System.Type, string>(this.ResolveNameFromType);
            this.ResolveType = new global::System.Func<string, global::System.Type>(this.ResolveTypeFromName);
            this.OnContextCreated();
            this.Format.LoadServiceModel = GeneratedEdmModel.GetInstance;
            this.Format.UseJson();
        }
        partial void OnContextCreated();
        /// <summary>
        /// Since the namespace configured for this service reference
        /// in Visual Studio is different from the one indicated in the
        /// server schema, use type-mappers to map between the two.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        protected global::System.Type ResolveTypeFromName(string typeName)
        {
            global::System.Type resolvedType = this.DefaultResolveType(typeName, "Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models", "Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models");
            if ((resolvedType != null))
            {
                return resolvedType;
            }
            resolvedType = this.DefaultResolveType(typeName, "Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Default", "Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Default");
            if ((resolvedType != null))
            {
                return resolvedType;
            }
            return null;
        }
        /// <summary>
        /// Since the namespace configured for this service reference
        /// in Visual Studio is different from the one indicated in the
        /// server schema, use type-mappers to map between the two.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        protected string ResolveNameFromType(global::System.Type clientType)
        {
            global::Microsoft.OData.Client.OriginalNameAttribute originalNameAttribute = (global::Microsoft.OData.Client.OriginalNameAttribute)global::System.Linq.Enumerable.SingleOrDefault(global::Microsoft.OData.Client.Utility.GetCustomAttributes(clientType, typeof(global::Microsoft.OData.Client.OriginalNameAttribute), true));
            if (clientType.Namespace.Equals("Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models", global::System.StringComparison.Ordinal))
            {
                if (originalNameAttribute != null)
                {
                    return string.Concat("Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.", originalNameAttribute.OriginalName);
                }
                return string.Concat("Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models.", clientType.Name);
            }
            if (clientType.Namespace.Equals("Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Default", global::System.StringComparison.Ordinal))
            {
                if (originalNameAttribute != null)
                {
                    return string.Concat("Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Default.", originalNameAttribute.OriginalName);
                }
                return string.Concat("Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Default.", clientType.Name);
            }
            if (originalNameAttribute != null)
            {
                return clientType.Namespace + "." + originalNameAttribute.OriginalName;
            }
            return clientType.FullName;
        }
        /// <summary>
        /// There are no comments for Orders in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("Orders")]
        public virtual global::Microsoft.OData.Client.DataServiceQuery<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Order> Orders
        {
            get
            {
                if ((this._Orders == null))
                {
                    this._Orders = base.CreateQuery<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Order>("Orders");
                }
                return this._Orders;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::Microsoft.OData.Client.DataServiceQuery<global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Order> _Orders;
        /// <summary>
        /// There are no comments for Orders in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public virtual void AddToOrders(global::Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Client.Models.Order order)
        {
            base.AddObject("Orders", order);
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private abstract class GeneratedEdmModel
        {
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
            private static global::Microsoft.OData.Edm.IEdmModel ParsedModel = LoadModelFromString();

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
            private const string filePath = @"AnnotationSerializationCsdl.xml";

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
            public static global::Microsoft.OData.Edm.IEdmModel GetInstance()
            {
                return ParsedModel;
            }
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
            private static global::Microsoft.OData.Edm.IEdmModel LoadModelFromString()
            {
                global::System.Xml.XmlReader reader = CreateXmlReader();
                try
                {
                    global::System.Collections.Generic.IEnumerable<global::Microsoft.OData.Edm.Validation.EdmError> errors;
                    global::Microsoft.OData.Edm.IEdmModel edmModel;

                    if (!global::Microsoft.OData.Edm.Csdl.CsdlReader.TryParse(reader, true, out edmModel, out errors))
                    {
                        global::System.Text.StringBuilder errorMessages = new global::System.Text.StringBuilder();
                        foreach (var error in errors)
                        {
                            errorMessages.Append(error.ErrorMessage);
                            errorMessages.Append("; ");
                        }
                        throw new global::System.InvalidOperationException(errorMessages.ToString());
                    }

                    return edmModel;
                }
                finally
                {
                    ((global::System.IDisposable)(reader)).Dispose();
                }
            }
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
            private static global::System.Xml.XmlReader CreateXmlReader(string edmxToParse)
            {
                return global::System.Xml.XmlReader.Create(new global::System.IO.StringReader(edmxToParse));
            }

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
            private static global::System.Xml.XmlReader CreateXmlReader()
            {
                try
                {
                    var assembly = global::System.Reflection.Assembly.GetExecutingAssembly();
                    // If multiple resource names end with the file name, select the shortest one.
                    var resourcePath = global::System.Linq.Enumerable.First(
                        global::System.Linq.Enumerable.OrderBy(
                            global::System.Linq.Enumerable.Where(assembly.GetManifestResourceNames(), name => name.EndsWith(filePath)),
                            filteredName => filteredName.Length));
                    global::System.IO.Stream stream = assembly.GetManifestResourceStream(resourcePath);
                    return global::System.Xml.XmlReader.Create(new global::System.IO.StreamReader(stream));
                }
                catch (global::System.Xml.XmlException e)
                {
                    throw new global::System.Xml.XmlException("Failed to create an XmlReader from the stream. Check if the resource exists.", e);
                }
            }
        }
    }
}