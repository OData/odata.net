//---------------------------------------------------------------------
// <copyright file="DeleteLinkTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests
{
    using System;
    using System.ComponentModel;
    using Microsoft.OData.Edm;
    using Xunit;

    public class DeleteLinkTests
    {
        private const string NamespaceName = "Microsoft.OData.Client.TDDUnitTests.Tests";
        private const string ServiceUri = "http://tempuri.svc";
        private EdmModel model;
        private TestDataServiceContext dataServiceContext;

        public DeleteLinkTests()
        {
            this.InitializeEdmModel();
            this.dataServiceContext = new TestDataServiceContext(new Uri(ServiceUri), this.model);
        }

        [Theory]
        [InlineData(DeleteLinkUriOption.DollarIdQueryParam, "http://tempuri.svc/Customers(1)/Orders/$ref?$id=http://tempuri.svc/Orders(1)")]
        [InlineData(DeleteLinkUriOption.RelatedKeyAsSegment, "http://tempuri.svc/Customers(1)/Orders(1)/$ref")]
        public void ExpectedDeleteLinkUriShouldBeGenerated(DeleteLinkUriOption deleteLinkUriOption, string expectedUri)
        {
            this.dataServiceContext.DeleteLinkUriOption = deleteLinkUriOption;

            var customer = new Customer { Id = 1 };
            var order = new Order { Id = 1 };

            var customerCollection = new DataServiceCollection<Customer>(
                dataServiceContext, new[] { customer },
                TrackingMode.AutoChangeTracking,
                "Customers",
                null,
                null);
            var orderCollection = new DataServiceCollection<Order>(
                dataServiceContext,
                new[] { order },
                TrackingMode.AutoChangeTracking,
                "Orders",
                null,
                null);

            this.dataServiceContext.DeleteLink(customer, "Orders", order);
            var saveResult = new TestSaveResult(this.dataServiceContext, "SaveChanges", SaveChangesOptions.None, null, null);

            // The API does not offer an easy way to grap the created request and inspect the Uri so we ride on an extensibility hook
            this.dataServiceContext.SendingRequest2 += (sender, args) =>
            {
                Assert.Equal(expectedUri, args.RequestMessage.Url.AbsoluteUri);
            };

            // If SendingRequest2 event if not fired, an exception is thrown and the test will fail
            saveResult.CreateRequestAndFireSendingEvent();
        }

        private void InitializeEdmModel()
        {
            model = new EdmModel();

            var orderEntityType = new EdmEntityType(NamespaceName, "Order");
            orderEntityType.AddKeys(orderEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            model.AddElement(orderEntityType);

            var customerEntityType = new EdmEntityType(NamespaceName, "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var ordersNavProperty = customerEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Orders",
                    Target = orderEntityType,
                    TargetMultiplicity = EdmMultiplicity.Many
                });
            model.AddElement(customerEntityType);

            var entityContainer = new EdmEntityContainer(NamespaceName, "Container");
            model.AddElement(entityContainer);

            var orderEntitySet = entityContainer.AddEntitySet("Orders", orderEntityType);
            var customerEntitySet = entityContainer.AddEntitySet("Customers", customerEntityType);
            customerEntitySet.AddNavigationTarget(ordersNavProperty, orderEntitySet);
        }

        [Key("Id")]
        internal partial class Customer : BaseEntityType, INotifyPropertyChanged
        {
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
            private int _Id;
            partial void OnIdChanging(int value);
            partial void OnIdChanged();

            public virtual DataServiceCollection<Order> Orders
            {
                get
                {
                    return this._Orders;
                }
                set
                {
                    this.OnOrdersChanging(value);
                    this._Orders = value;
                    this.OnOrdersChanged();
                    this.OnPropertyChanged("Orders");
                }
            }
            private DataServiceCollection<Order> _Orders = new DataServiceCollection<Order>(null, TrackingMode.None);
            partial void OnOrdersChanging(DataServiceCollection<Order> value);
            partial void OnOrdersChanged();

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string property)
            {
                if ((this.PropertyChanged != null))
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }
        }

        [Key("Id")]
        internal partial class Order : BaseEntityType, INotifyPropertyChanged
        {
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
            private int _Id;
            partial void OnIdChanging(int value);
            partial void OnIdChanged();

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string property)
            {
                if ((this.PropertyChanged != null))
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }
        }

        internal partial class TestDataServiceContext : DataServiceContext
        {
            public TestDataServiceContext(Uri serviceRoot, IEdmModel serviceModel) :
                    base(serviceRoot, ODataProtocolVersion.V4)
            {
                this.Format.UseJson(serviceModel);
            }
        }

        internal class TestSaveResult : SaveResult
        {
            public TestSaveResult(DataServiceContext context, string method, SaveChangesOptions options, AsyncCallback callback, object state)
                : base(context, method, options, callback, state)
            {
            }

            internal void CreateRequestAndFireSendingEvent()
            {
                if (this.ChangedEntries.Count > 0)
                {
                    if (this.ChangedEntries[0] is LinkDescriptor descriptor)
                    {
                        var requestMessageWrapper = this.CreateRequest(descriptor);
                        requestMessageWrapper.FireSendingEventHandlers(descriptor);

                        return;
                    }
                }

                throw new Exception(); // Throw exception to signal unexpected outcome
            }
        }
    }
}
