//---------------------------------------------------------------------
// <copyright file="CustomRowBasedContext.cs" company="Microsoft">
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
    using Microsoft.OData.Service.Providers;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using test = System.Data.Test.Astoria;

    #endregion Namespaces

    /// <summary>
    /// A custom in-memory data context suitable for fast tests that need a custom
    /// data service provider (implemented by CustomDataServiceProvider).
    /// </summary>
    public class CustomRowBasedContext : IDataServiceUpdateProvider, IServiceProvider
    {
        private static List<RowEntityTypeWithIDAsKey> products;
        private static List<RowEntityTypeWithIDAsKey> regions;
        private static List<RowEntityType> orderDetails;

        private static List<RowEntityTypeWithIDAsKey> memberCustomers;
        private static List<RowEntityTypeWithIDAsKey> memberOrders;
        private static List<RowEntityTypeWithIDAsKey> memberProducts;
        private static List<RowEntityTypeWithIDAsKey> memberRegions;
        private static List<RowEntityType> memberOrderDetails;

        private static bool preserveChanges;
        private List<KeyValuePair<object, EntityState>> pendingChanges;
        private Dictionary<int, object> tokens = new Dictionary<int, object>();
        private static IDataServiceMetadataProvider provider;
        protected static IDataServiceQueryProvider queryProvider;

        public static List<RowEntityTypeWithIDAsKey> customers;
        public static List<RowEntityTypeWithIDAsKey> orders;

        public static Action<List<RowEntityTypeWithIDAsKey>> CustomizeCustomers;
        public static Action<List<RowEntityTypeWithIDAsKey>> CustomizeOrders;

        public CustomRowBasedContext()
        {
            if (preserveChanges == false || customers == null)
            {
                provider = null;
                queryProvider = null;
                Debug.Assert((customers == null && orders == null) ||
                             (customers != null && orders != null), "Either the data must be populated or not");

                customers = new List<RowEntityTypeWithIDAsKey>();
                orders = new List<RowEntityTypeWithIDAsKey>();
                products = new List<RowEntityTypeWithIDAsKey>();
                regions = new List<RowEntityTypeWithIDAsKey>();
                orderDetails = new List<RowEntityType>();

                memberCustomers = new List<RowEntityTypeWithIDAsKey>();
                memberOrders = new List<RowEntityTypeWithIDAsKey>();
                memberProducts = new List<RowEntityTypeWithIDAsKey>();
                memberRegions = new List<RowEntityTypeWithIDAsKey>();
                memberOrderDetails = new List<RowEntityType>();

                // initialize data and metadata
                PopulateData();
                if (CustomizeCustomers != null)
                {
                    CustomizeCustomers(customers);
                }
                if (CustomizeOrders != null)
                {
                    CustomizeOrders(orders);
                }

                provider = PopulateMetadata(this);
                queryProvider = (IDataServiceQueryProvider)provider;
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

        internal static void PopulateData()
        {
            int orderCount = 2;
            int customerCount = 3;

            for (int i = 0; i < customerCount; i++)
            {
                RowEntityTypeWithIDAsKey region = new RowEntityTypeWithIDAsKey(CustomRowBasedContext.RegionFullName);
                region.ID = i;
                region.Properties["Name"] = "Region" + i.ToString();
                regions.Add(region);

                RowEntityTypeWithIDAsKey customer = (i % 2 == 0) ? new RowEntityTypeWithIDAsKey(CustomRowBasedContext.CustomerFullName) : new RowEntityTypeWithIDAsKey(CustomRowBasedContext.CustomerWithBirthdayFullName);
                customer.ID = i;
                customer.Properties["Name"] = "Customer " + i.ToString();
                customer.Properties["NameAsHtml"] = "<html><body>" + customer.Properties["Name"] + "</body></html>";
                customer.Properties["BestFriend"] = (i == 0) ? null : customers[i - 1];
                customer.Properties["Region"] = region;

                if (i % 2 != 0)
                {
                    customer.Properties["Birthday"] = DateTime.Today.AddYears(-30);
                }

                customer.Properties.Add("Orders", new List<RowEntityTypeWithIDAsKey>());

                for (int j = 0; j < orderCount; j++)
                {
                    int orderID = i + 100 * j;
                    int productID = i + 100 * j;
                    double orderDollarAmount = Math.Round(20.1 + 10.1 * j, 2);

                    RowEntityTypeWithIDAsKey o = new RowEntityTypeWithIDAsKey(CustomRowBasedContext.OrderFullName) { ID = orderID };
                    o.Properties["DollarAmount"] = orderDollarAmount;
                    o.Properties["OrderDetails"] = new List<RowEntityType>();
                    o.Properties["Customer"] = customer;
                    ((IList<RowEntityTypeWithIDAsKey>)customer.Properties["Orders"]).Add(o);
                    orders.Add(o);

                    RowEntityTypeWithIDAsKey p = new RowEntityTypeWithIDAsKey(CustomRowBasedContext.ProductFullName) { ID = productID };
                    p.Properties["ProductName"] = "Product #" + p.ID.ToString();
                    p.Properties["Discontinued"] = false;
                    p.Properties["OrderDetails"] = new List<RowEntityType>();
                    products.Add(p);

                    RowEntityType od = new RowEntityType(CustomRowBasedContext.OrderDetailsFullName);
                    od.Properties["ProductID"] = p.ID;
                    od.Properties["OrderID"] = o.ID;
                    od.Properties["UnitPrice"] = orderDollarAmount;
                    od.Properties["Quantity"] = (short)1;
                    orderDetails.Add(od);
                    ((IList<RowEntityType>)o.Properties["OrderDetails"]).Add(od);
                    ((IList<RowEntityType>)p.Properties["OrderDetails"]).Add(od);
                }

                customers.Add(customer);

                if (OpenWebDataServiceHelper.EnableBlobServer)
                {
                    DataServiceStreamProvider.Init();
                    using (System.IO.Stream s = System.IO.File.OpenWrite(DataServiceStreamProvider.GetStoragePath(customer)))
                    {
                        byte[] buffer = new byte[] { 1, 2, 3, 4 };
                        s.Write(buffer, 0, 4);
                        s.Close();
                    }
                }
            }
        }

        private object ResourceToToken(object resource)
        {
            int token = this.tokens.Count;
            this.tokens[token] = resource;
            return token;
        }

        private object TokenToResource(object token)
        {
            return this.tokens[(int)token];
        }

        private static bool IsPrimitiveType(Type type)
        {
            return (type.IsPrimitive ||
                    type == typeof(String) ||
                    type == typeof(Guid) ||
                    type == typeof(Decimal) ||
                    type == typeof(DateTime) ||
                    type == typeof(byte[]));
        }

        protected IQueryable<RowEntityTypeWithIDAsKey> Customers
        {
            get { return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(customers.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName); }
        }

        protected IQueryable<RowEntityTypeWithIDAsKey> Orders
        {
            get { return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(orders.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName); }
        }

        private IQueryable<RowEntityTypeWithIDAsKey> Regions
        {
            get { return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(regions.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName); }
        }

        private IQueryable<RowEntityTypeWithIDAsKey> Products
        {
            get { return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(products.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName); }
        }

        private IQueryable<RowEntityType> OrderDetails
        {
            get { return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(orderDetails.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName); }
        }

        private IQueryable<RowEntityTypeWithIDAsKey> MemberCustomers
        {
            get { return memberCustomers.AsQueryable(); }
        }

        private IQueryable<RowEntityTypeWithIDAsKey> MemberOrders
        {
            get { return memberOrders.AsQueryable(); }
        }

        private IQueryable<RowEntityTypeWithIDAsKey> MemberRegions
        {
            get { return memberRegions.AsQueryable(); }
        }

        private IQueryable<RowEntityTypeWithIDAsKey> MemberProducts
        {
            get { return memberProducts.AsQueryable(); }
        }

        private IQueryable<RowEntityType> MemberOrderDetails
        {
            get { return memberOrderDetails.AsQueryable(); }
        }

        public static bool PreserveChanges
        {
            get { return preserveChanges; }
            set { preserveChanges = value; }
        }

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
            if (preserveChanges)
            {
                throw new InvalidOperationException("Changes are already being preserved.");
            }

            ClearData();
            preserveChanges = true;
            return new CustomRowBasedContextPresereChangesScope();
        }

        public static object GetInstance()
        {
            if (preserveChanges == false && provider == null || customers == null)
            {
                new CustomRowBasedContext();
            }

            return queryProvider.CurrentDataSource;
        }

        public static string CustomerFullName = "AstoriaUnitTests.Stubs.Customer";
        public static string OrderFullName = "AstoriaUnitTests.Stubs.Order";
        public static string AddressFullName = "AstoriaUnitTests.Stubs.Address";
        public static string CustomerWithBirthdayFullName = "AstoriaUnitTests.Stubs.CustomerWithBirthday";
        public static string RegionFullName = "AstoriaUnitTests.Stubs.Region";
        public static string ProductFullName = "AstoriaUnitTests.Stubs.Product";
        public static string OrderDetailsFullName = "AstoriaUnitTests.Stubs.OrderDetail";


        #region IDataServiceUpdateProvider Members

        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            RowEntityType targetEntity = (RowEntityType)this.TokenToResource(targetResource);
            RowEntityType entityResourceToBeAdded = (RowEntityType)this.TokenToResource(resourceToBeAdded);

            if (IsCustomerInstance(targetEntity) &&
                propertyName == "Orders" &&
                IsOrderInstance(entityResourceToBeAdded))
            {
                var openProperties = targetEntity.Properties;
                object orders;
                // IF this is the first order that is getting added
                if (!openProperties.TryGetValue(propertyName, out orders))
                {
                    orders = new List<RowEntityTypeWithIDAsKey>();
                    ((IList<RowEntityTypeWithIDAsKey>)orders).Add((RowEntityTypeWithIDAsKey)entityResourceToBeAdded);
                    openProperties[propertyName] = orders;
                }
                else
                {
                    ((IList<RowEntityTypeWithIDAsKey>)openProperties[propertyName]).Add((RowEntityTypeWithIDAsKey)entityResourceToBeAdded);
                }

                return;
            }

            throw new Exception(String.Format("Invalid call AddReferenceToCollection: {0}, {1}, {2}",
                targetEntity.TypeName, propertyName, entityResourceToBeAdded.TypeName));
        }

        public object CreateResource(string containerName, string fullTypeName)
        {
            object resource = null;
            if (containerName == "Customers")
            {
                if (fullTypeName == CustomRowBasedContext.CustomerFullName)
                {
                    resource = new RowEntityTypeWithIDAsKey(CustomRowBasedContext.CustomerFullName);
                }

                if (fullTypeName == CustomRowBasedContext.CustomerWithBirthdayFullName)
                {
                    resource = new RowEntityTypeWithIDAsKey(CustomRowBasedContext.CustomerWithBirthdayFullName);
                }
            }
            else if (containerName == "Orders")
            {
                if (fullTypeName == CustomRowBasedContext.OrderFullName)
                {
                    resource = new RowEntityTypeWithIDAsKey(CustomRowBasedContext.OrderFullName);
                }
            }
            else if (containerName == "Regions")
            {
                if (fullTypeName == CustomRowBasedContext.RegionFullName)
                {
                    resource = new RowEntityType(CustomRowBasedContext.RegionFullName);
                }
            }
            else if (fullTypeName == CustomRowBasedContext.AddressFullName)
            {
                // no need to add this to the pending changelist. Only entities need to be added
                return this.ResourceToToken(new RowComplexType(CustomRowBasedContext.AddressFullName));
            }

            if (resource == null)
            {
                throw new Exception(String.Format("Invalid container name '{0}' or type name specified '{1}'", containerName, fullTypeName));
            }
            else
            {
                this.PendingChanges.Add(new KeyValuePair<object, EntityState>(resource, EntityState.Added));
            }

            return this.ResourceToToken(resource);
        }

        public void DeleteResource(object targetResource)
        {
            targetResource = this.TokenToResource(targetResource);
            this.PendingChanges.Add(new KeyValuePair<object, EntityState>(targetResource, EntityState.Deleted));
        }

        public object GetResource(IQueryable query, string fullTypeName)
        {
            object resource = null;
            foreach (object r in query)
            {
                if (resource != null)
                {
                    throw new Exception("query returning more than one type");
                }

                resource = r;
            }

            if (resource != null && fullTypeName != null && ((RowComplexType)resource).TypeName != fullTypeName)
            {
                throw new System.ArgumentException(String.Format("Invalid uri specified. ExpectedType: '{0}', ActualType: '{1}'", fullTypeName, resource.GetType().FullName));
            }

            if (resource != null)
            {
                return this.ResourceToToken(resource);
            }

            return null;
        }

        public object GetValue(object targetResource, string propertyName)
        {
            targetResource = this.TokenToResource(targetResource);

            // Check for strongly types properties
            PropertyInfo property = targetResource.GetType().GetProperty(propertyName);
            object propertyValue = null;
            if (property != null)
            {
                propertyValue = property.GetValue(targetResource, null);
            }
            else
            {
                RowComplexType complexType = (RowComplexType)targetResource;
                if (!complexType.Properties.TryGetValue(propertyName, out propertyValue))
                {
                    propertyValue = null;
                }
            }

            if (propertyValue != null && !IsPrimitiveType(propertyValue.GetType()))
            {
                propertyValue = this.ResourceToToken(propertyValue);
            }

            return propertyValue;
        }

        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            resourceToBeRemoved = this.TokenToResource(resourceToBeRemoved);

            if (propertyName != "Orders")
            {
                throw new Exception("Invalid Property name '" + propertyName + "' specified");
            }

            IList<RowEntityTypeWithIDAsKey> collection = (IList<RowEntityTypeWithIDAsKey>)this.TokenToResource(GetValue(targetResource, propertyName));
            collection.Remove((RowEntityTypeWithIDAsKey)resourceToBeRemoved);
        }

        public object ResetResource(object resource)
        {
            RowEntityTypeWithIDAsKey existingEntity = (RowEntityTypeWithIDAsKey)this.TokenToResource(resource);
            RowEntityTypeWithIDAsKey newEntity = new RowEntityTypeWithIDAsKey(existingEntity.TypeName);
            newEntity.ID = existingEntity.ID;

            DeleteResource(resource);
            this.PendingChanges.Add(new KeyValuePair<object, EntityState>(newEntity, EntityState.Added));
            return this.ResourceToToken(newEntity);
        }

        public object ResolveResource(object resource)
        {
            return this.TokenToResource(resource);
        }

        public void SaveChanges()
        {
            if (this.pendingChanges == null)
            {
                return;
            }

            foreach (KeyValuePair<object, EntityState> pendingChange in this.pendingChanges)
            {
                RowEntityTypeWithIDAsKey entity = (RowEntityTypeWithIDAsKey)pendingChange.Key;

                // find the entity set for the object
                IList<RowEntityTypeWithIDAsKey> entitySetInstance = GetEntitySet(entity);

                switch (pendingChange.Value)
                {
                    case EntityState.Added:
                        AddResource(entity, true /*throwIfDuplicate*/);
                        break;
                    case EntityState.Deleted:
                        RowEntityTypeWithIDAsKey entityToBeDeleted = DeleteEntity(entitySetInstance, entity, true /*throwIfNotPresent*/);
                        if (IsOrderInstance(entity))
                        {
                            foreach (RowEntityTypeWithIDAsKey customer in customers)
                            {
                                if (customer.Properties.ContainsKey("Orders"))
                                {
                                    IList<RowEntityTypeWithIDAsKey> orders = (IList<RowEntityTypeWithIDAsKey>)customer.Properties["Orders"];
                                    orders.Remove(entityToBeDeleted);
                                }
                            }
                        }
                        if (IsCustomerInstance(entity))
                        {
                            foreach (RowEntityTypeWithIDAsKey customer in customers)
                            {
                                object propertyValue;
                                foreach (string propertyName in new string[] { "BestFriend", "Region" })
                                {
                                    if (customer.Properties.TryGetValue(propertyName, out propertyValue) &&
                                        propertyValue == entityToBeDeleted)
                                    {
                                        customer.Properties[propertyName] = null;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        throw new Exception("Unsupported State");
                }
            }

            this.pendingChanges.Clear();
        }

        public void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            RowEntityTypeWithIDAsKey targetEntity = (RowEntityTypeWithIDAsKey)this.TokenToResource(targetResource);
            RowEntityTypeWithIDAsKey propertyEntity = propertyValue == null ? null : (RowEntityTypeWithIDAsKey)this.TokenToResource(propertyValue);
            if (IsCustomerInstance(targetEntity))
            {
                // here???
                if (propertyName == "BestFriend" &&
                    (propertyValue == null || IsCustomerInstance(propertyEntity)))
                {
                    targetEntity.Properties[propertyName] = propertyEntity;
                    return;
                }
                else if (propertyName == "Region" &&
                    (propertyValue == null || IsRegionInstance(propertyEntity)))
                {
                    targetEntity.Properties[propertyName] = propertyEntity;
                    return;
                }
            }
            else if (IsOrderInstance(targetEntity))
            {
                if (propertyName == "Customer" &&
                    (propertyValue == null || IsCustomerInstance(propertyEntity)))
                {
                    targetEntity.Properties[propertyName] = propertyEntity;
                    return;
                }
            }

            throw new Exception(String.Format("Invalid property name '{0}' or invalid type '{1}' specified", propertyName, propertyEntity.TypeName));
        }

        public void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            targetResource = this.TokenToResource(targetResource);
            ResourceType resourceType = queryProvider.GetResourceType(targetResource);
            if (propertyValue != null && (resourceType.Properties.Single(p => p.Name == propertyName).Kind & ResourcePropertyKind.Primitive) == 0)
            {
                propertyValue = this.TokenToResource(propertyValue);
            }

            RowEntityTypeWithIDAsKey targetEntity = targetResource as RowEntityTypeWithIDAsKey;

            if (targetEntity != null)
            {
                if (IsCustomerInstance(targetEntity))
                {
                    targetEntity.Properties["GuidValue"] = Guid.NewGuid();
                    if (propertyName == "ID")
                    {
                        targetEntity.ID = (int)propertyValue;
                        return;
                    }
                    else if (propertyName == "Name")
                    {
                        if (propertyValue != null && propertyValue.GetType() != typeof(string))
                        {
                            throw new ArgumentException("Name property must be of string type", "propertyValue");
                        }
                        targetEntity.Properties[propertyName] = propertyValue;
                        targetEntity.Properties["NameAsHtml"] = "<html><body>" + propertyValue + "</body></html>";
                        return;
                    }
                    else if (propertyName == "BestFriend")
                    {
                        RowEntityTypeWithIDAsKey propertyEntity = (RowEntityTypeWithIDAsKey)propertyValue;
                        if (propertyValue != null && !IsCustomerInstance(propertyEntity))
                        {
                            throw new ArgumentException("BestFriend property must be of OpenCustomer type", "propertyValue");
                        }
                        targetEntity.Properties[propertyName] = (RowEntityTypeWithIDAsKey)propertyValue;
                        return;
                    }
                    else if (propertyName == "Address")
                    {
                        RowComplexType propertyEntity = (RowComplexType)propertyValue;
                        if (propertyValue != null && propertyEntity.TypeName != CustomRowBasedContext.AddressFullName)
                        {
                            throw new ArgumentException("Address property must be of OpenAddress type", "propertyValue");
                        }
                        targetEntity.Properties[propertyName] = propertyValue;
                        return;
                    }
                    else if (targetEntity.TypeName == CustomRowBasedContext.CustomerWithBirthdayFullName && propertyName == "Birthday")
                    {
                        targetEntity.Properties["Birthday"] = (DateTime)propertyValue;
                        return;
                    }
                    else if (propertyName == "GuidValue")
                    {
                        // ignore, since this is an etag value and its value is already modified
                        return;
                    }
                    else if (propertyName == "NameAsHtml")
                    {
                        // Ignore, since this is a calculated property, based on the name value
                        return;
                    }
                }
                else if (IsOrderInstance(targetEntity))
                {
                    if (propertyName == "ID")
                    {
                        targetEntity.ID = (int)propertyValue;
                        return;
                    }
                    else if (propertyName == "DollarAmount")
                    {
                        targetEntity.Properties["DollarAmount"] = propertyValue;
                        return;
                    }
                }
                else if (IsRegionInstance(targetEntity))
                {
                    if (propertyName == "ID")
                    {
                        targetEntity.ID = (int)propertyValue;
                        return;
                    }
                    else if (propertyName == "Name")
                    {
                        targetEntity.Properties["Name"] = propertyValue;
                        return;
                    }
                }
            }

            RowComplexType targetComplex = (RowComplexType)targetResource;
            if (targetComplex.TypeName == CustomRowBasedContext.AddressFullName)
            {
                if (propertyName == "StreetAddress" ||
                    propertyName == "City" ||
                    propertyName == "State" ||
                    propertyName == "PostalCode")
                {
                    if (propertyValue != null && propertyValue.GetType() != typeof(string))
                    {
                        throw new ArgumentException("OpenAddress properties must be of string type", "propertyValue");
                    }
                    targetComplex.Properties[propertyName] = propertyValue;
                    return;
                }
            }

            throw new Exception(String.Format("Invalid property name '{0}' specified for type '{1}'", propertyName, targetResource.GetType().FullName));
        }

        public void ClearChanges()
        {
            if (this.pendingChanges != null)
            {
                this.pendingChanges.Clear();
            }
        }

        public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            if (checkForEquality == null)
            {
                throw new DataServiceException(400, "Missing If-Match header");
            }

            foreach (var pInfo in concurrencyValues)
            {
                object value = this.GetValue(resourceCookie, pInfo.Key);
                if (!Object.Equals(value, pInfo.Value))
                {
                    throw new DataServiceException(412, String.Format("Precondition failed for property '{0}'", pInfo.Key));
                }
            }
        }

        #endregion

        private static bool IsCustomerInstance(RowEntityType resource)
        {
            return resource.TypeName == CustomRowBasedContext.CustomerFullName ||
                   resource.TypeName == CustomRowBasedContext.CustomerWithBirthdayFullName;
        }

        private static bool IsOrderInstance(RowEntityType resource)
        {
            return resource.TypeName == CustomRowBasedContext.OrderFullName;
        }

        private static bool IsProductInstance(RowEntityType resource)
        {
            return resource.TypeName == CustomRowBasedContext.ProductFullName;
        }

        private static bool IsRegionInstance(RowEntityType resource)
        {
            return resource.TypeName == CustomRowBasedContext.RegionFullName;
        }

        private static bool IsOrderDetailInstance(RowEntityType resource)
        {
            return resource.TypeName == CustomRowBasedContext.OrderDetailsFullName;
        }

        private RowEntityTypeWithIDAsKey GetCustomRowInstance(object resource)
        {
            RowEntityTypeWithIDAsKey entity = resource as RowEntityTypeWithIDAsKey;
            if (entity == null)
            {
                throw new Exception("Invalid Entity instance passed");
            }

            return entity;
        }

        private static IList<RowEntityTypeWithIDAsKey> GetEntitySet(RowEntityTypeWithIDAsKey entity)
        {
            if (IsCustomerInstance(entity))
            {
                return customers;
            }

            if (IsOrderInstance(entity))
            {
                return orders;
            }

            if (IsProductInstance(entity))
            {
                return products;
            }

            if (IsRegionInstance(entity))
            {
                return regions;
            }

            throw new Exception(String.Format("Unexpected EntityType '{0}' encountered", entity.TypeName));
        }

        private static void AddResource(RowEntityTypeWithIDAsKey resource, bool throwIfDuplicate)
        {
            IList<RowEntityTypeWithIDAsKey> entitySetInstance = GetEntitySet(resource);

            foreach (RowEntityTypeWithIDAsKey entity in entitySetInstance)
            {
                // check if there is not another instance with the same id
                if (resource.ID == entity.ID)
                {
                    if (throwIfDuplicate)
                    {
                        throw new Exception(String.Format("Entity with the same key already present. EntityType: '{0}'",
                            resource.GetType().Name));
                    }

                    // if its already there, do not add it to the global context
                    return;
                }
            }
            entitySetInstance.Add(resource);
        }

        private RowEntityTypeWithIDAsKey DeleteEntity(IList<RowEntityTypeWithIDAsKey> collection, RowEntityTypeWithIDAsKey entity, bool throwIfNotPresent)
        {
            RowEntityTypeWithIDAsKey entityToBeDeleted = TryGetEntity(collection, entity);

            if (entityToBeDeleted == null && throwIfNotPresent)
            {
                throw new Exception("No entity found with the given ID");
            }

            if (entityToBeDeleted != null)
            {
                collection.Remove(entity);
            }

            return entityToBeDeleted;
        }

        public static void ClearData()
        {
            CustomRowBasedContext.PreserveChanges = false;
            provider = null;
            queryProvider = null;
            customers = null;
            orders = null;
            regions = null;
        }

        private static RowEntityTypeWithIDAsKey TryGetEntity(IList<RowEntityTypeWithIDAsKey> collection, RowEntityTypeWithIDAsKey entity)
        {
            RowEntityTypeWithIDAsKey matchingEntity = null;

            foreach (RowEntityTypeWithIDAsKey element in collection)
            {
                // check if there is not another instance with the same id
                if (element.ID == entity.ID)
                {
                    matchingEntity = element;
                    break;
                }
            }

            return matchingEntity;
        }

        class CustomRowBasedContextPresereChangesScope : IDisposable
        {
            public void Dispose()
            {
                CustomRowBasedContext.ClearData();
            }
        }

        private static IDataServiceMetadataProvider PopulateMetadata(object dataSourceInstance)
        {
            List<ResourceType> types = new List<ResourceType>(4);
            ResourceType customer = new ResourceType(
                typeof(RowEntityTypeWithIDAsKey),
                ResourceTypeKind.EntityType,
                null, /*baseType*/
                "AstoriaUnitTests.Stubs", /*namespaceName*/
                "Customer",
                false /*isAbstract*/);

            customer.CanReflectOnInstanceType = false;

            ResourceType order = new ResourceType(
                typeof(RowEntityTypeWithIDAsKey),
                ResourceTypeKind.EntityType,
                null,
                "AstoriaUnitTests.Stubs",
                "Order",
                false);

            order.CanReflectOnInstanceType = false;

            ResourceType region = new ResourceType(
                typeof(RowEntityTypeWithIDAsKey),
                ResourceTypeKind.EntityType,
                null, /*baseType*/
                "AstoriaUnitTests.Stubs", /*namespaceName*/
                "Region",
                false /*isAbstract*/);

            region.CanReflectOnInstanceType = false;

            ResourceType address = new ResourceType(
                typeof(RowComplexType),
                ResourceTypeKind.ComplexType,
                null,
                "AstoriaUnitTests.Stubs",
                "Address",
                false);

            address.CanReflectOnInstanceType = false;

            ResourceType product = new ResourceType(
                typeof(RowEntityTypeWithIDAsKey),
                ResourceTypeKind.EntityType,
                null, /*baseType*/
                "AstoriaUnitTests.Stubs", /*namespaceName*/
                "Product",
                false /*isAbstract*/);

            product.CanReflectOnInstanceType = false;

            ResourceType orderDetail = new ResourceType(
                typeof(RowEntityType),
                ResourceTypeKind.EntityType,
                null, /*baseType*/
                "AstoriaUnitTests.Stubs", /*namespaceName*/
                "OrderDetail",
                false /*isAbstract*/);

            orderDetail.CanReflectOnInstanceType = false;

            ResourceType currency = new ResourceType(
                typeof(RowComplexType),
                ResourceTypeKind.ComplexType,
                null,
                "AstoriaUnitTests.Stubs",
                "CurrencyAmount",
                false);

            ResourceType headquarter = new ResourceType(
                typeof(RowComplexType),
                ResourceTypeKind.ComplexType,
                null,
                "AstoriaUnitTests.Stubs",
                "Headquarter",
                false);

            headquarter.CanReflectOnInstanceType = false;

            ResourceSet customerEntitySet = new ResourceSet("Customers", customer);
            ResourceSet orderEntitySet = new ResourceSet("Orders", order);
            ResourceSet regionEntitySet = new ResourceSet("Regions", region);
            ResourceSet productEntitySet = new ResourceSet("Products", product);
            ResourceSet orderDetailEntitySet = new ResourceSet("OrderDetails", orderDetail);

            ResourceSet memberCustomerEntitySet = new ResourceSet("MemberCustomers", customer);
            ResourceSet memberOrderEntitySet = new ResourceSet("MemberOrders", order);
            ResourceSet memberRegionEntitySet = new ResourceSet("MemberRegions", region);
            ResourceSet memberProductEntitySet = new ResourceSet("MemberProducts", product);
            ResourceSet memberOrderDetailEntitySet = new ResourceSet("MemberOrderDetails", orderDetail);

            ResourceProperty keyProperty = new ResourceProperty(
                "ID",
                ResourcePropertyKind.Key | ResourcePropertyKind.Primitive,
                ResourceType.GetPrimitiveResourceType(typeof(int)));

            // populate customer properties
            customer.AddProperty(keyProperty);
            customer.AddProperty(CreateNonClrProperty("Name", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(String))));
            customer.AddProperty(CreateNonClrProperty("BestFriend", ResourcePropertyKind.ResourceReference, customer));
            customer.AddProperty(CreateNonClrProperty("Orders", ResourcePropertyKind.ResourceSetReference, order));
            customer.AddProperty(CreateNonClrProperty("Region", ResourcePropertyKind.ResourceReference, region));
            customer.AddProperty(CreateNonClrProperty("Address", ResourcePropertyKind.ComplexType, address));
            customer.AddProperty(CreateNonClrProperty("GuidValue", ResourcePropertyKind.Primitive | ResourcePropertyKind.ETag, ResourceType.GetPrimitiveResourceType(typeof(Guid))));
            ResourceProperty property = CreateNonClrProperty("NameAsHtml", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string)));
            property.MimeType = "text/html";
            customer.AddProperty(property);

            if (OpenWebDataServiceHelper.EnableBlobServer)
            {
                customer.IsMediaLinkEntry = true;
            }

            // create Customer With Birthday and populate its properties
            ResourceType customerWithBirthday = new ResourceType(
                typeof(RowEntityTypeWithIDAsKey),
                ResourceTypeKind.EntityType,
                customer, /*baseType*/
                "AstoriaUnitTests.Stubs", /*namespaceName*/
                "CustomerWithBirthday",
                false /*isAbstract*/);
            customerWithBirthday.AddProperty(CreateNonClrProperty("Birthday", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(DateTime))));

            customerWithBirthday.CanReflectOnInstanceType = false;

            // populate order properties
            order.AddProperty(keyProperty);
            order.AddProperty(CreateNonClrProperty("DollarAmount", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(double))));
            order.AddProperty(CreateNonClrProperty("OrderDetails", ResourcePropertyKind.ResourceSetReference, orderDetail));
            order.AddProperty(CreateNonClrProperty("CurrencyAmount", ResourcePropertyKind.ComplexType, currency));
            order.AddProperty(CreateNonClrProperty("Customer", ResourcePropertyKind.ResourceReference, customer));

            // populate region properties
            region.AddProperty(keyProperty);
            region.AddProperty(CreateNonClrProperty("Name", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))));
            region.AddProperty(CreateNonClrProperty("Headquarter", ResourcePropertyKind.ComplexType, headquarter));
            
            //populate headquarter properties
            headquarter.AddProperty(CreateNonClrProperty("DrivingDirections", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))));
            headquarter.AddProperty(CreateNonClrProperty("Address", ResourcePropertyKind.ComplexType, address));

            //populate address properties
            address.AddProperty(CreateNonClrProperty("StreetAddress", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))));
            address.AddProperty(CreateNonClrProperty("City", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))));
            address.AddProperty(CreateNonClrProperty("State", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))));
            address.AddProperty(CreateNonClrProperty("PostalCode", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))));

            // create product type and its properties
            product.AddProperty(keyProperty);
            product.AddProperty(CreateNonClrProperty("ProductName", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(String))));
            product.AddProperty(CreateNonClrProperty("Discontinued", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(bool))));
            product.AddProperty(CreateNonClrProperty("OrderDetails", ResourcePropertyKind.ResourceSetReference, orderDetail));

            // create order detail and its properties
            // DEVNOTE: it's very important that ProductID and OrderID are not in alphabetical order. There are tests relying on this.
            orderDetail.AddProperty(CreateNonClrProperty("ProductID", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))));
            orderDetail.AddProperty(CreateNonClrProperty("OrderID", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))));
            orderDetail.AddProperty(CreateNonClrProperty("UnitPrice", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(double))));
            orderDetail.AddProperty(CreateNonClrProperty("Quantity", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(short))));
            
            //populate currency amount properties
            currency.AddProperty(CreateNonClrProperty("Amount", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(decimal))));
            currency.AddProperty(CreateNonClrProperty("CurrencyName", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(String))));

            types.Add(customer);
            types.Add(customerWithBirthday);
            types.Add(order);
            types.Add(region);
            types.Add(address);
            types.Add(product);
            types.Add(orderDetail);
            types.Add(currency);
            types.Add(headquarter);

            List<ResourceSet> containers = new List<ResourceSet>(7);
            containers.Add(customerEntitySet);
            containers.Add(orderEntitySet);
            containers.Add(regionEntitySet);
            containers.Add(productEntitySet);
            containers.Add(orderDetailEntitySet);

            containers.Add(memberCustomerEntitySet);
            containers.Add(memberOrderEntitySet);
            containers.Add(memberRegionEntitySet);
            containers.Add(memberProductEntitySet);
            containers.Add(memberOrderDetailEntitySet);

            List<ServiceOperation> operations = new List<ServiceOperation>(1);
            operations.Add(new ServiceOperation("IntServiceOperation", ServiceOperationResultKind.DirectValue, ResourceType.GetPrimitiveResourceType(typeof(int)), null, "GET", null));
            operations.Add(new ServiceOperation("InsertCustomer", ServiceOperationResultKind.DirectValue, customer, customerEntitySet, "POST",
                new ServiceOperationParameter[] { new ServiceOperationParameter("id", ResourceType.GetPrimitiveResourceType(typeof(int))),
                                                  new ServiceOperationParameter("name", ResourceType.GetPrimitiveResourceType(typeof(string))) }
                                                  ));
            operations.Add(new ServiceOperation("GetCustomerByCity", ServiceOperationResultKind.QueryWithMultipleResults, customer, customerEntitySet, "GET",
                new ServiceOperationParameter[] { new ServiceOperationParameter("city", ResourceType.GetPrimitiveResourceType(typeof(string))) }));
            operations.Add(new ServiceOperation("DoNothingOperation", ServiceOperationResultKind.Void, null, null, "POST", null));
            operations.Add(new ServiceOperation("GetCustomerAddress", ServiceOperationResultKind.DirectValue, address, null, "GET",
                new ServiceOperationParameter[] { new ServiceOperationParameter("id", ResourceType.GetPrimitiveResourceType(typeof(int))) }));
            operations.Add(new ServiceOperation("GetOrderById", ServiceOperationResultKind.DirectValue, order, orderEntitySet, "GET",
                new ServiceOperationParameter[] { new ServiceOperationParameter("id", ResourceType.GetPrimitiveResourceType(typeof(int))) }));

            operations.Add(new ServiceOperation("GetRegionByName", ServiceOperationResultKind.QueryWithMultipleResults, region, regionEntitySet, "GET",
                new ServiceOperationParameter[] { new ServiceOperationParameter("name", ResourceType.GetPrimitiveResourceType(typeof(string))) }));

            operations.Add(new ServiceOperation("AddressServiceOperation", ServiceOperationResultKind.DirectValue, address, null, "GET", null));

            operations.Add(new ServiceOperation("GetAllCustomersQueryable", ServiceOperationResultKind.QueryWithMultipleResults, customer, customerEntitySet, "GET", null));
            operations.Add(new ServiceOperation("GetCustomerByIdQueryable", ServiceOperationResultKind.QueryWithSingleResult, customer, customerEntitySet, "GET", new ServiceOperationParameter[] { new ServiceOperationParameter("id", ResourceType.GetPrimitiveResourceType(typeof(int))) }));
            operations.Add(new ServiceOperation("GetAllCustomersEnumerable", ServiceOperationResultKind.Enumeration, customer, customerEntitySet, "GET", null));
            operations.Add(new ServiceOperation("GetCustomerByIdDirectValue", ServiceOperationResultKind.DirectValue, customer, customerEntitySet, "GET", new ServiceOperationParameter[] { new ServiceOperationParameter("id", ResourceType.GetPrimitiveResourceType(typeof(int))) }));

            operations.Add(new ServiceOperation("GetAllOrdersQueryable", ServiceOperationResultKind.QueryWithMultipleResults, order, orderEntitySet, "GET", null));
            operations.Add(new ServiceOperation("GetOrderByIdQueryable", ServiceOperationResultKind.QueryWithSingleResult, order, orderEntitySet, "GET", new ServiceOperationParameter[] { new ServiceOperationParameter("id", ResourceType.GetPrimitiveResourceType(typeof(int))) }));
            operations.Add(new ServiceOperation("GetAllOrdersEnumerable", ServiceOperationResultKind.Enumeration, order, orderEntitySet, "GET", null));
            operations.Add(new ServiceOperation("GetOrderByIdDirectValue", ServiceOperationResultKind.DirectValue, order, orderEntitySet, "GET", new ServiceOperationParameter[] { new ServiceOperationParameter("id", ResourceType.GetPrimitiveResourceType(typeof(int))) }));

            List<ResourceAssociationSet> associationSets = new List<ResourceAssociationSet>();

            ResourceAssociationSet customer_BestFriend =
                new ResourceAssociationSet(
                    "Customers_BestFriend",
                    new ResourceAssociationSetEnd(customerEntitySet, customer, customer.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "BestFriend")),
                    new ResourceAssociationSetEnd(customerEntitySet, customer, customer.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "BestFriend")));
            associationSets.Add(customer_BestFriend);

            ResourceAssociationSet customer_Order =
                new ResourceAssociationSet(
                    "Customers_Orders",
                    new ResourceAssociationSetEnd(customerEntitySet, customer, customer.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "Orders")),
                    new ResourceAssociationSetEnd(orderEntitySet, order, order.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "Customer")));
            associationSets.Add(customer_Order);

            ResourceAssociationSet customer_Region =
                new ResourceAssociationSet(
                    "Customers_Regions",
                    new ResourceAssociationSetEnd(customerEntitySet, customer, customer.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "Region")),
                    new ResourceAssociationSetEnd(regionEntitySet, region, region.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "Customer")));
            associationSets.Add(customer_Region);

            ResourceAssociationSet order_OrderDetails =
                new ResourceAssociationSet(
                    "Orders_OrderDetails",
                    new ResourceAssociationSetEnd(orderEntitySet, order, order.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "OrderDetails")),
                    new ResourceAssociationSetEnd(orderDetailEntitySet, orderDetail, null));
            associationSets.Add(order_OrderDetails);

            ResourceAssociationSet product_OrderDetails =
                new ResourceAssociationSet(
                    "Products_OrderDetails",
                    new ResourceAssociationSetEnd(productEntitySet, product, product.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "OrderDetails")),
                    new ResourceAssociationSetEnd(orderDetailEntitySet, orderDetail, null));
            associationSets.Add(product_OrderDetails);

            ResourceAssociationSet memberCustomer_BestFriend =
                new ResourceAssociationSet(
                    "MemberCustomers_BestFriend",
                    new ResourceAssociationSetEnd(memberCustomerEntitySet, customer, customer.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "BestFriend")),
                    new ResourceAssociationSetEnd(memberCustomerEntitySet, customer, null));
            associationSets.Add(memberCustomer_BestFriend);

            ResourceAssociationSet memberCustomer_MemberOrder =
                new ResourceAssociationSet(
                    "MemberCustomers_MemberOrders",
                    new ResourceAssociationSetEnd(memberCustomerEntitySet, customer, customer.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "Orders")),
                    new ResourceAssociationSetEnd(memberOrderEntitySet, order, order.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "Customer")));
            associationSets.Add(memberCustomer_MemberOrder);

            ResourceAssociationSet memberCustomer_MemberRegion =
                new ResourceAssociationSet(
                    "MemberCustomers_MemberRegions",
                    new ResourceAssociationSetEnd(memberCustomerEntitySet, customer, customer.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "Region")),
                    new ResourceAssociationSetEnd(memberRegionEntitySet, region, region.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "Customer")));
            associationSets.Add(memberCustomer_MemberRegion);

            ResourceAssociationSet memberOrder_MemberOrderDetails =
                new ResourceAssociationSet(
                    "MemberOrder_MemberOrderDetails",
                    new ResourceAssociationSetEnd(memberOrderEntitySet, order, order.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "OrderDetails")),
                    new ResourceAssociationSetEnd(memberOrderDetailEntitySet, orderDetail, null));
            associationSets.Add(memberOrder_MemberOrderDetails);

            ResourceAssociationSet memberProduct_MemberOrderDetails =
                new ResourceAssociationSet(
                    "MemberProduct_MemberOrderDetails",
                    new ResourceAssociationSetEnd(memberProductEntitySet, product, product.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "OrderDetails")),
                    new ResourceAssociationSetEnd(memberOrderDetailEntitySet, orderDetail, null));
            associationSets.Add(memberProduct_MemberOrderDetails);

            return new CustomDataServiceProvider(containers, types, operations, associationSets, dataSourceInstance);
        }

        private static bool IEnumerableTypeFilter(Type m, object filterCriteria)
        {
            Debug.Assert(m != null, "m != null");
            return m.IsGenericType && m.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        protected static string GetTypeName(object instance, out bool collection)
        {
            collection = false;
            RowComplexType complexType = instance as RowComplexType;
            if (complexType != null)
            {
                return complexType.TypeName;
            }

            Type elementType = GetGenericInterfaceElementType(instance.GetType(), IEnumerableTypeFilter);
            if (elementType != null)
            {
                IList list = (IList)instance;
                if (list.Count > 0)
                {
                    complexType = (RowComplexType)list[0];
                    collection = true;
                    return complexType.TypeName;
                }
            }

            return null;
        }

        internal static Type GetGenericInterfaceElementType(Type type, TypeFilter typeFilter)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(!type.IsGenericTypeDefinition, "!type.IsGenericTypeDefinition");

            if (typeFilter(type, null))
            {
                return type.GetGenericArguments()[0];
            }

            Type[] queriables = type.FindInterfaces(typeFilter, null);
            if (queriables != null && queriables.Length == 1)
            {
                return queriables[0].GetGenericArguments()[0];
            }
            else
            {
                return null;
            }
        }

        internal static ResourceProperty CreateNonClrProperty(string name, ResourcePropertyKind kind, ResourceType propertyType)
        {
            ResourceProperty property = new ResourceProperty(name, kind, propertyType);
            property.CanReflectOnInstanceTypeProperty = false;
            return property;
        }

        #region IServiceProvider Members

        public virtual object GetService(Type serviceType)
        {
            Debug.Assert(provider == queryProvider, "The providers must be the same instance");
            if (serviceType == typeof(IDataServiceMetadataProvider))
            {
                return provider;
            }
            else if (serviceType == typeof(IDataServiceQueryProvider))
            {
                return queryProvider;
            }
            else if (serviceType == typeof(IDataServiceUpdateProvider))
            {
                return this;
            }

            return null;
        }

        #endregion

        #region ServiceOperations

        public int IntServiceOperation()
        {
            return 5;
        }

        public RowEntityTypeWithIDAsKey InsertCustomer(int id, string name)
        {
            RowEntityTypeWithIDAsKey customer = new RowEntityTypeWithIDAsKey(CustomRowBasedContext.CustomerFullName);
            customer.ID = id;
            customer.Properties["Name"] = name;
            AddResource(customer, true);
            return customer;
        }

        public IQueryable<RowEntityTypeWithIDAsKey> GetCustomerByCity(string city)
        {
            return this.Customers.Where(c => (string)((RowComplexType)c.Properties["Address"]).Properties["City"] == city).AsQueryable();
        }

        public void DoNothingOperation()
        {
        }

        public object GetCustomerAddress(int id)
        {
            RowEntityType customer = customers.Where(c => c.ID == id).FirstOrDefault();
            if (customer == null)
            {
                return null;
            }
            return customer.Properties["Address"];
        }

        public RowEntityType GetOrderById(int id)
        {
            return orders.Where(o => o.ID == id).FirstOrDefault();
        }

        public IQueryable<RowEntityTypeWithIDAsKey> GetRegionByName(string name)
        {
            return regions.Where(region => (string)region.Properties["Name"] == name).AsQueryable();
        }

        public RowComplexType AddressServiceOperation()
        {
            RowComplexType address = new RowComplexType(CustomRowBasedContext.AddressFullName);
            address.Properties["StreetAddress"] = "NE 228th";
            address.Properties["City"] = "Sammamish";
            address.Properties["State"] = "WA";
            address.Properties["PostalCode"] = "98074";
            return address;
        }

        public IQueryable<RowEntityTypeWithIDAsKey> GetAllCustomersQueryable()
        {
            return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(customers.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName);
        }

        public IQueryable<RowEntityTypeWithIDAsKey> GetCustomerByIdQueryable(int id)
        {
            return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(customers.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName).Where(c => c.ID == id);
        }

        public IEnumerable<RowEntityTypeWithIDAsKey> GetAllCustomersEnumerable()
        {
            return customers.AsEnumerable();
        }

        public RowEntityTypeWithIDAsKey GetCustomerByIdDirectValue(int id)
        {
            return customers.Single(c => c.ID == id);
        }

        public IQueryable<RowEntityTypeWithIDAsKey> GetAllOrdersQueryable()
        {
            return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(orders.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName);
        }

        public IQueryable<RowEntityTypeWithIDAsKey> GetOrderByIdQueryable(int id)
        {
            return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(orders.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName).Where(c => c.ID == id);
        }

        public IEnumerable<RowEntityTypeWithIDAsKey> GetAllOrdersEnumerable()
        {
            return orders.AsEnumerable();
        }

        public RowEntityTypeWithIDAsKey GetOrderByIdDirectValue(int id)
        {
            return orders.Single(c => c.ID == id);
        }

        #endregion //ServiceOperations
    }

    /// <summary>
    /// Provides an IDataServiceMetadataProvider implementation for 
    /// in-memory tests.
    /// </summary>
    public class CustomDataServiceProvider : IDataServiceMetadataProvider, IDataServiceQueryProvider
    {
        public static int GetQueryRootForResourceSetInvokeCount;
        private static Action<string> invocationTraceCallback;
        private List<ResourceSet> containers;
        private List<ResourceType> types;
        private List<ServiceOperation> operations;
        private List<ResourceAssociationSet> associationSets;
        private object dataContextInstance;

        private static test.Restorable<Action<List<ResourceSet>, List<ResourceType>, List<ServiceOperation>, List<ResourceAssociationSet>>> metadataCustomizer =
            new test.Restorable<Action<List<ResourceSet>, List<ResourceType>, List<ServiceOperation>, List<ResourceAssociationSet>>>();

        // Last chance to perform special modification to the metadata before they are sealed.
        public static test.Restorable<Action<List<ResourceSet>, List<ResourceType>, List<ServiceOperation>, List<ResourceAssociationSet>>> MetadataCustomizer
        { get { return metadataCustomizer; } }

        public CustomDataServiceProvider(List<ResourceSet> containers, List<ResourceType> types, List<ServiceOperation> serviceOperations, List<ResourceAssociationSet> associationSets, object context)
        {
            this.containers = containers;
            this.types = types;
            this.operations = serviceOperations;
            this.associationSets = associationSets;
            this.dataContextInstance = context;
            GetQueryRootForResourceSetInvokeCount = 0;

            if (MetadataCustomizer.Value != null)
            {
                MetadataCustomizer.Value(containers, types, serviceOperations, associationSets);
            }

            foreach (ResourceSet set in this.containers)
            {
                set.SetReadOnly();
            }

            foreach (ResourceType type in this.types)
            {
                type.SetReadOnly();
            }

            foreach (ServiceOperation operation in this.operations)
            {
                operation.SetReadOnly();
            }
        }

        #region IDataServiceProvider Members

        public string ContainerName
        {
            get 
            { 
                Trace("ContainerName");
                return typeof(CustomDataServiceProvider).Name; 
            }
        }

        public string ContainerNamespace
        {
            get 
            {
                Trace("ContainerNamespace");
                return typeof(CustomDataServiceProvider).Namespace;
            }
        }

        public IEnumerable<ResourceSet> ResourceSets
        {
            get
            {
                Trace("ResourceSets");
                return containers;
            }
        }

        public IEnumerable<ServiceOperation> ServiceOperations
        {
            get
            {
                Trace("ServiceOperations");
                return operations;
            }
        }

        public object CurrentDataSource
        {
            get { return this.dataContextInstance; }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void DisposeDataSource()
        {
            // Do nothing
            Trace("DisposeDataSource");
        }

        public ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            foreach (ResourceAssociationSet associationSet in this.associationSets)
            {
                if (associationSet.End1.ResourceSet == resourceSet && associationSet.End1.ResourceType == resourceType && associationSet.End1.ResourceProperty == resourceProperty)
                {
                    return associationSet;
                }
            }

            foreach (ResourceAssociationSet associationSet in this.associationSets)
            {
                if (associationSet.End2.ResourceSet == resourceSet && associationSet.End2.ResourceType == resourceType && associationSet.End2.ResourceProperty == resourceProperty)
                {
                    return associationSet;
                }
            }

            return null;
        }

        public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            Trace("GetOpenPropertyValues - " + target);
            ResourceType resourceType = this.InternalGetResourceType(target);
            if (resourceType.IsOpenType == false)
            {
                throw new DataServiceException(500, "GetOpenPropertyValues should never be called for non open types");
            }

            foreach (KeyValuePair<string, object> propertyInfo in (IDictionary<string, object>)target.GetType().GetProperty("Properties").GetValue(target, null))
            {
                if (propertyInfo.Key == "ID" || propertyInfo.Key == "Orders" || propertyInfo.Key == "BestFriend" || propertyInfo.Key == "Customer" || propertyInfo.Key == "GuidValue")
                {
                    continue;
                }

                yield return propertyInfo;
            }
        }

        public object GetPropertyValue(object target, ResourceProperty resourceProperty)
        {
            Assert.IsTrue(resourceProperty.CanReflectOnInstanceTypeProperty == false, "We should never call GetPropertyValue on reflectable properties");
            return InternalGetPropertyValue(target, resourceProperty.Name);
        }

        public IQueryable GetQueryRootForResourceSet(ResourceSet container)
        {
            GetQueryRootForResourceSetInvokeCount++;
            Trace("GetQueryRootForResourceSet - " + container.Name);
            
            PropertyInfo property = this.dataContextInstance.GetType().GetProperty(container.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            return (IQueryable)property.GetValue(this.dataContextInstance, null);
        }

        public ResourceType GetResourceType(object instance)
        {
            Trace("GetResourceType - " + instance);
            return InternalGetResourceType(instance);
        }

        public bool IsNullPropagationRequired
        {
            get { return true; }
        }

        public bool TryResolveResourceSet(string name, out ResourceSet resourceSet)
        {
            Trace("TryResolveResourceSet - " + name);
            resourceSet = this.containers.Where<ResourceSet>(c => c.Name == name).FirstOrDefault<ResourceSet>();
            return resourceSet != null;
        }

        public bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
        {
            Trace("TryResolveServiceOperation - " + name);
            serviceOperation = this.operations.Where<ServiceOperation>(s => s.Name == name).FirstOrDefault<ServiceOperation>();
            return serviceOperation != null;
        }

        public bool TryResolveResourceType(string name, out ResourceType resourceType)
        {
            Trace("TryResolveResourceType - " + name);
            resourceType = this.types.Where<ResourceType>(t => t.FullName == name).FirstOrDefault<ResourceType>();
            return resourceType != null;
        }

        public IEnumerable<ResourceType> Types
        {
            get
            {
                return types;
            }
        }

        public object InvokeServiceOperation(ServiceOperation operation, object[] parameters)
        {
            MethodInfo methodInfo = this.dataContextInstance.GetType().GetMethod(operation.Name);
            if (methodInfo == null)
            {
                throw new Exception(String.Format("Method with name '{0}' not found on the context instance", operation.Name));
            }

            return methodInfo.Invoke(this.dataContextInstance, parameters);
        }

        public object GetOpenPropertyValue(object target, string propertyName)
        {
            ResourceType resourceType = this.GetResourceType(target);
            if (resourceType.IsOpenType == false)
            {
                throw new DataServiceException(400, String.Format("'{0}' is not an open type. HEnce this method should not have been called", resourceType.FullName));
            }

            return InternalGetPropertyValue(target, propertyName);
        }

        public bool HasDerivedTypes(ResourceType resourceType)
        {
            Trace("HasDerivedTypes - " + resourceType.Name);
            var e = this.InternalGetDerivedTypes(resourceType);
            return e != null && e.Count() > 0;
        }

        public IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType)
        {
            Trace("GetDerivedTypes - " + resourceType.Name);
            return this.InternalGetDerivedTypes(resourceType);
        }

        #endregion

        public static Action<string> InvocationTraceCallback
        {
            get { return invocationTraceCallback; }
            set { invocationTraceCallback = value; }
        }

        internal static bool IsAssignableFrom(ResourceType baseType, ResourceType derivedType)
        {
            while (derivedType != null)
            {
                if (derivedType == baseType)
                {
                    return true;
                }

                derivedType = derivedType.BaseType;
            }

            return false;
        }

        public static object InternalGetPropertyValue(object target, string propertyName)
        {
            var propertyCollection = (IDictionary<string, object>)target.GetType().GetProperty("Properties").GetValue(target, null);
            object propertyValue;
            propertyCollection.TryGetValue(propertyName, out propertyValue);
            return propertyValue;
        }

        private static bool IsDerivedType(ResourceType baseType, ResourceType derivedType)
        {
            while (derivedType.BaseType != null)
            {
                if (derivedType.BaseType == baseType)
                {
                    return true;
                }
                derivedType = derivedType.BaseType;
            }

            return false;
        }

        private static void Trace(string text)
        {
            if (invocationTraceCallback != null)
            {
                invocationTraceCallback(text);
            }
        }

        private IEnumerable<ResourceType> InternalGetDerivedTypes(ResourceType resourceType)
        {
            foreach (ResourceType type in this.types)
            {
                if (type.BaseType != null && IsDerivedType(resourceType, type))
                {
                    yield return type;
                }
            }
        }

        private ResourceType InternalGetResourceType(object instance)
        {
            ParameterModifier[] parameterModifiers = new ParameterModifier[1];
            parameterModifiers[0] = new ParameterModifier(2);
            parameterModifiers[0][0] = false; // not out
            parameterModifiers[0][1] = true;  // out

            MethodInfo method = this.dataContextInstance.GetType().GetMethod(
                "GetTypeName",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                null,
                new Type[] { typeof(object), Type.GetType("System.Boolean&") },
                parameterModifiers);

            object[] parameters = new object[2];
            parameters[0] = instance;
            string typeName = (string)method.Invoke(instance, parameters);
            ResourceType resourceType = types.Where<ResourceType>(r => r.FullName == typeName).FirstOrDefault<ResourceType>();
            return resourceType;
        }
    }

    public class RowComplexType
    {
        public static readonly String TypeNamePropertyName = "TypeName";
        private readonly string typeName;
        public IDictionary<string, object> Properties { get; set; }
        public string TypeName { get { return this.typeName; } }

        public RowComplexType(string typeName)
        {
            if (String.IsNullOrEmpty(typeName)) throw new ArgumentNullException("typeName");
            this.typeName = typeName;
            this.Properties = new Dictionary<string, object>();

            if (typeName == CustomRowBasedContext.CustomerFullName || typeName == CustomRowBasedContext.CustomerWithBirthdayFullName)
            {
                if (!OpenWebDataServiceHelper.EnableFriendlyFeeds || typeName == CustomRowBasedContext.CustomerFullName)
                {
                    RowComplexType address = new RowComplexType(CustomRowBasedContext.AddressFullName);
                    address.Properties["StreetAddress"] = "Line1";
                    address.Properties["City"] = "Redmond";
                    address.Properties["State"] = "WA";
                    address.Properties["PostalCode"] = "98052";
                    this.Properties.Add("Address", address);
                }
            }
        }
    }

    public class RowEntityType : RowComplexType
    {
        public RowEntityType(string typeName)
            : base(typeName)
        {
        }
    }

    public class RowEntityTypeWithIDAsKey : RowEntityType
    {
        public int ID { get; set; }

        public RowEntityTypeWithIDAsKey(string typeName)
            : base(typeName)
        {
            if (typeName == CustomRowBasedContext.CustomerFullName ||
                typeName == CustomRowBasedContext.CustomerWithBirthdayFullName)
            {
                this.Properties["GuidValue"] = Guid.NewGuid();
                this.Properties["NameAsHtml"] = "<html><body></body></html>";
            }

            if (typeName == CustomRowBasedContext.CustomerWithBirthdayFullName)
            {
                // Since this is a value type, this should never be null
                this.Properties["Birthday"] = DateTime.Today.AddYears(-30);
            }
        }
    }
}
