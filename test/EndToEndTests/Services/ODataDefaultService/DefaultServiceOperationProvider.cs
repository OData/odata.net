//---------------------------------------------------------------------
// <copyright file="DefaultServiceOperationProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    public class DefaultServiceOperationProvider : ODataReflectionOperationProvider
    {
        public void ResetDataSource()
        {
            var dataSource = DataSourceManager.GetCurrentDataSource<DefaultWCFSvcDataSource>();
            dataSource.Reset();
            dataSource.Initialize();
        }

        public PaymentInstrument GetDefaultPI(Account account)
        {
            if (account.MyPaymentInstruments != null && account.MyPaymentInstruments.Count > 0)
            {
                PaymentInstrument pi = account.MyPaymentInstruments[0];
                string relativePath = "MyPaymentInstruments(" + pi.PaymentInstrumentID + ")";
                string parentPath = "Accounts(" + account.AccountID + ")";
                QueryContext.CanonicalUri = new Uri(ServiceConstants.ServiceBaseUri, parentPath + "/" + relativePath);

                return pi;
            }
            return null;
        }

        public PaymentInstrument RefreshDefaultPI(Account account, DateTimeOffset newDate)
        {
            if (account.MyPaymentInstruments != null && account.MyPaymentInstruments.Count > 0)
            {
                PaymentInstrument pi = account.MyPaymentInstruments[0];
                pi.CreatedDate = newDate;
                string relativePath = "MyPaymentInstruments(" + pi.PaymentInstrumentID + ")";
                string parentPath = "Accounts(" + account.AccountID + ")";
                QueryContext.CanonicalUri = new Uri(ServiceConstants.ServiceBaseUri, parentPath + "/" + relativePath);

                return pi;
            }
            return null;
        }

        public double GetActualAmount(GiftCard giftCard, double bonusRate)
        {
            return giftCard.Amount * (1 + bonusRate);
        }

        public Int32 GetEmployeesCount(Company company)
        {
            return company.Employees.Count;
        }

        public Int64 IncreaseRevenue(Company company, Int64 increaseCount)
        {
            company.Revenue += increaseCount;
            return company.Revenue;
        }

        public Int64 IncreaseRevenue(Company company)
        {
            company.Revenue += 1000;
            return company.Revenue;
        }

        public AccessLevel AddAccessRight(Product product, AccessLevel accessRight)
        {
            product.UserAccess = product.UserAccess == null ? accessRight : product.UserAccess | accessRight;
            return product.UserAccess.Value;
        }

        public Color GetDefaultColor()
        {
            return Color.Red;
        }

        public void Discount(int percentage)
        {
            foreach (var product in GetRootQuery("Products") as IEnumerable<Product>)
            {
                product.UnitPrice = product.UnitPrice * (1 - percentage / 100.0f);
            }
        }

        public Collection<Product> Discount(Collection<Product> products, int percentage)
        {
            foreach (var product in products)
            {
                product.UnitPrice = product.UnitPrice * (1 - percentage / 100.0f);
            }
            return products;
        }

        public Person ResetAddress(Person person, List<Address> addresses, int index)
        {
            person.HomeAddress = addresses[index];
            return person;
        }

        public Order PlaceOrder(Customer customer, Order order)
        {
            return order;
        }

        public IList<Order> PlaceOrders(Customer customer, IList<Order> orders)
        {
            return orders;
        }

        public IList<AbstractEntity> GetOrderAndOrderDetails(Customer customer)
        {
            var list = new List<AbstractEntity>();
            list.AddRange(customer.Orders);
            foreach (var order in customer.Orders)
            {
                foreach (var od in order.OrderDetails)
                {
                    od.EntitySetName = "OrderDetails";
                    list.Add(od);
                }
            }

            return list;
        }

        public Collection<string> ResetBossEmail(List<string> emails)
        {
            var boss = GetRootQuery("Boss") as Customer;
            boss.Emails = new Collection<string>(emails);
            return boss.Emails;
        }

        public Address ResetBossAddress(Address address)
        {
            var boss = GetRootQuery("Boss") as Customer;
            boss.HomeAddress = address;
            return boss.HomeAddress;
        }

        //TODO: if parameter is nullable<int> in parameter, the url parser will parse it as nullable = false;
        public Collection<ProductDetail> GetProductDetails(Product product, int count)
        {
            if (/*count == null || */product.Details.Count < count)
            {
                return product.Details;
            }
            else
            {
                return new Collection<ProductDetail>(product.Details.Take(count).ToList());
            }
        }

        public Product GetRelatedProduct(ProductDetail productDetail)
        {
            return productDetail.RelatedProduct;
        }

        public Date GetShipDate(Order order)
        {
            return order.ShipDate;
        }

        public TimeOfDay GetShipTime(Order order)
        {
            return order.ShipTime;
        }

        public bool CheckShipDate(Order order, Date date)
        {
            return order.ShipDate == date;
        }

        public bool CheckShipTime(Order order, TimeOfDay time)
        {
            return order.ShipTime == time;
        }

        public Order ChangeShipTimeAndDate(Order order, Date date, TimeOfDay time)
        {
            order.ShipTime = time;
            order.ShipDate = date;
            return order;
        }

        public Person GetPerson(Address address)
        {
            IEnumerable<Person> persons = GetRootQuery("People") as IEnumerable<Person>;
            foreach (var person in persons)
            {
                if (person.HomeAddress.City.Equals(address.City, StringComparison.OrdinalIgnoreCase))
                {
                    return person;
                }
            }
            return null;
        }

        //TODO: Unbound Function overload is not supported now, 
        //public Person GetPerson(string city)
        public Person GetPerson2(string city)
        {
            IEnumerable<Person> persons = GetRootQuery("People") as IEnumerable<Person>;
            foreach (var person in persons)
            {
                if (person.HomeAddress.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                {
                    return person;
                }
            }
            return null;
        }

        public Collection<Product> GetAllProducts()
        {
            return GetRootQuery("Products") as Collection<Product>;
        }

        public Collection<string> GetBossEmails(int start, int count)
        {
            var boss = GetRootQuery("Boss") as Customer;
            Collection<string> emails = new Collection<string>();
            if (boss.Emails.Count >= start + 1)
            {
                for (int i = start; i < count && i < boss.Emails.Count; i++)
                {
                    emails.Add(boss.Emails[i]);
                }
            }
            return emails;
        }

        public Collection<string> GetProductsByAccessLevel(AccessLevel accessLevel)
        {
            IEnumerable<Product> products = GetRootQuery("Products") as IEnumerable<Product>;
            Collection<string> result = new Collection<string>();
            foreach (var p in products)
            {
                if ((p.UserAccess & accessLevel) == accessLevel)
                {
                    result.Add(p.Name);
                }
            }
            return result;
        }

        public HomeAddress GetHomeAddress(Person person)
        {
            if (person.HomeAddress is HomeAddress)
            {
                return person.HomeAddress as HomeAddress;
            }
            return new HomeAddress()
            {
                City = person.HomeAddress.City,
                Street = person.HomeAddress.Street,
                PostalCode = person.HomeAddress.PostalCode,
                FamilyName = string.Empty
            };
        }

        public AccountInfo GetAccountInfo(Account account)
        {
            return account.AccountInfo;
        }

        public void ChangeLabourUnionName(LabourUnion labourUnion, string name)
        {
            labourUnion.Name = name;
        }

        public Employee GetSeniorEmployees(Collection<Employee> employees)
        {
            if (employees == null || employees.Count == 0)
            {
                return null;
            }
            return employees.OrderBy(e => e.DateHired).FirstOrDefault();
        }

        private static object GetRootQuery(string propertyName)
        {
            var dataSource = DataSourceManager.GetCurrentDataSource<DefaultWCFSvcDataSource>();
            return dataSource.GetType().GetProperty(propertyName).GetValue(dataSource, null);
        }
    }
}
