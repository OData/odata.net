//---------------------------------------------------------------------
// <copyright file="CustomRowBasedOpenTypesContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.Stubs
{
    using System.Xml;

    public class CustomRowBasedOpenTypesContext: IDataServiceUpdateProvider, IServiceProvider
    {
        private static bool preserveChanges;
        private List<KeyValuePair<object, EntityState>> pendingChanges;
        private static IDataServiceMetadataProvider provider;
        private static IDataServiceQueryProvider queryProvider;

        public static List<RowComplexType> customers;
        public static List<RowComplexType> orders;

        public static Action<List<ResourceSet>, List<ResourceType>, List<ServiceOperation>, List<ResourceAssociationSet>> CustomizeMetadata;
        public static Action<List<RowComplexType>> CustomizeCustomers;
        public static Action<List<RowComplexType>> CustomizeOrders;

        public CustomRowBasedOpenTypesContext()
        {
            if (preserveChanges == false || customers == null)
            {
                provider = null;
                queryProvider = null;
                Debug.Assert((customers == null && orders == null) ||
                             (customers != null && orders != null), "Either the data must be populated or not");
                customers = new List<RowComplexType>();
                orders = new List<RowComplexType>();

                // Initialize metadata and data
                provider = PopulateMetadata(this);
                queryProvider = (IDataServiceQueryProvider)provider;
                PopulateData();
                if (CustomizeCustomers != null)
                {
                    CustomizeCustomers(customers);
                }
                if (CustomizeOrders != null)
                {
                    CustomizeOrders(orders);
                }
            }
        }

        private List<KeyValuePair<object, EntityState>> PendingChanges
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

        private void PopulateData()
        {
            int orderCount = 2;
            int customerCount = 3;

            for (int i = 0; i < customerCount; i++)
            {
                RowComplexType customer = (i % 2 == 0) ? new RowComplexType(CustomRowBasedContext.CustomerFullName) : new RowComplexType(CustomRowBasedContext.CustomerWithBirthdayFullName);
                customer.Properties["ID"] = i;
                customer.Properties["GuidValue"] = Guid.NewGuid();
                customer.Properties["Name"] = "Customer " + i.ToString();
                customer.Properties["BestFriend"] = (i == 0) ? null : customers[i - 1];

                if (i % 2 != 0)
                {
                    customer.Properties["Birthday"] = DateTime.Today.AddYears(-30);
                }

                customer.Properties.Add("Orders", new List<RowComplexType>());

                for (int j = 0; j < orderCount; j++)
                {
                    int orderID = i + 100 * j;
                    double orderDollarAmount = Math.Round(20.1 + 10.1 * j, 2);

                    RowComplexType o = new RowComplexType(CustomRowBasedContext.OrderFullName);
                    o.Properties["ID"] = orderID;
                    o.Properties["DollarAmount"] = orderDollarAmount;
                    o.Properties["GuidValue"] = Guid.NewGuid();
                    ((IList<RowComplexType>)customer.Properties["Orders"]).Add(o);
                    ((RowComplexType)o).Properties["Customer"] = customer;
                    orders.Add(o);
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

        protected virtual IQueryable<RowComplexType> Customers
        {
            get { return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(customers.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName); }
        }

        protected virtual IQueryable<RowComplexType> Orders
        {
            get { return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(orders.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName); }
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
            if (provider == null && preserveChanges == false || customers == null)
            {
                new CustomRowBasedOpenTypesContext();
            }

            return queryProvider.CurrentDataSource;
        }

        #region IUpdatable Members

        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            RowComplexType targetEntity = (RowComplexType)targetResource;
            RowComplexType entityResourceToBeAdded = (RowComplexType)resourceToBeAdded;

            if (IsCustomerInstance(targetEntity) &&
                propertyName == "Orders" &&
                IsOrderInstance(entityResourceToBeAdded))
            {
                var openProperties = targetEntity.Properties;
                object orders;
                // IF this is the first order that is getting added
                if (!openProperties.TryGetValue(propertyName, out orders))
                {
                    orders = new List<RowComplexType>();
                    ((IList<RowComplexType>)orders).Add(entityResourceToBeAdded);
                    openProperties[propertyName] = orders;
                }
                else
                {
                    ((IList<RowComplexType>)openProperties[propertyName]).Add(entityResourceToBeAdded);
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
                    resource = new RowComplexType(CustomRowBasedContext.CustomerFullName);
                }
                if (fullTypeName == CustomRowBasedContext.CustomerWithBirthdayFullName)
                {
                    resource = new RowComplexType(CustomRowBasedContext.CustomerWithBirthdayFullName);
                }
            }
            else if (containerName == "Orders")
            {
                if (fullTypeName == CustomRowBasedContext.OrderFullName)
                {
                    resource = new RowComplexType(CustomRowBasedContext.OrderFullName);
                }
            }
            else if (fullTypeName == CustomRowBasedContext.AddressFullName)
            {
                // no need to add this to the pending changelist. Only entities need to be added
                return new RowComplexType(CustomRowBasedContext.AddressFullName);
            }

            if (resource == null)
            {
                throw new Exception(String.Format("Invalid container name '{0}' or type name specified '{1}'", containerName, fullTypeName));
            }
            else
            {
                this.PendingChanges.Add(new KeyValuePair<object, EntityState>(resource, EntityState.Added));
            }

            return resource;
        }

        public void DeleteResource(object targetResource)
        {
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

            return resource;
        }

        public object GetValue(object targetResource, string propertyName)
        {
            // Check for strongly types properties
            PropertyInfo property = targetResource.GetType().GetProperty(propertyName);

            if (property != null)
            {
                return property.GetValue(targetResource, null);
            }

            RowComplexType complexType = (RowComplexType)targetResource;
            return complexType.Properties[propertyName];
        }

        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            if (propertyName != "Orders")
            {
                throw new Exception("Invalid Property name '" + propertyName + "' specified");
            }

            IList<RowComplexType> collection = (IList<RowComplexType>)GetValue(targetResource, propertyName);
            collection.Remove((RowComplexType)resourceToBeRemoved);
        }

        public object ResetResource(object resource)
        {
            RowComplexType existingEntity = (RowComplexType)resource;
            RowComplexType newEntity = new RowComplexType(existingEntity.TypeName);
            newEntity.Properties["ID"] = GetKey(existingEntity);

            DeleteResource(resource);
            this.PendingChanges.Add(new KeyValuePair<object, EntityState>(newEntity, EntityState.Added));
            return newEntity;
        }

        public object ResolveResource(object resource)
        {
            return resource;
        }

        public void SaveChanges()
        {
            if (this.pendingChanges == null)
            {
                return;
            }

            foreach (KeyValuePair<object, EntityState> pendingChange in this.pendingChanges)
            {
                RowComplexType entity = (RowComplexType)pendingChange.Key;

                // find the entity set for the object
                IList<RowComplexType> entitySetInstance = GetEntitySet(entity);

                switch (pendingChange.Value)
                {
                    case EntityState.Added:
                        AddResource(entity, true /*throwIfDuplicate*/);
                        break;
                    case EntityState.Deleted:
                        RowComplexType entityToBeDeleted = DeleteEntity(entitySetInstance, entity, true /*throwIfNotPresent*/);
                        if (IsOrderInstance(entity))
                        {
                            foreach (RowComplexType customer in customers)
                            {
                                IList<RowComplexType> orders = (IList<RowComplexType>)customer.Properties["Orders"];
                                orders.Remove(entityToBeDeleted);
                            }
                        }
                        if (IsCustomerInstance(entity))
                        {
                            foreach (RowComplexType customer in customers)
                            {
                                object propertyValue;
                                if (customer.Properties.TryGetValue("BestFriend", out propertyValue) &&
                                    propertyValue == entityToBeDeleted)
                                {
                                    customer.Properties["BestFriend"] = null;
                                }
                            }

                            foreach (RowComplexType order in orders)
                            {
                                object propertyValue;
                                if (order.Properties.TryGetValue("Customer", out propertyValue) &&
                                    propertyValue == entityToBeDeleted)
                                {
                                    order.Properties["Customer"] = null;
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
            RowComplexType targetEntity = (RowComplexType)targetResource;
            RowComplexType propertyEntity = (RowComplexType)propertyValue;
            if (IsCustomerInstance(targetEntity))
            {
                if (propertyName == "BestFriend" && 
                    (propertyValue == null || IsCustomerInstance(propertyEntity)))
                {
                    targetEntity.Properties[propertyName] = propertyValue;
                    return;
                }
            }

            if (IsOrderInstance(targetEntity))
            {
                if (propertyName == "Customer" &&
                    (propertyValue == null || IsCustomerInstance(propertyEntity)))
                {
                    targetEntity.Properties[propertyName] = propertyValue;
                    return;
                }
            }

            throw new Exception(String.Format("Invalid property name '{0}' or invalid type '{1}' specified", propertyName, propertyEntity.TypeName));
        }

        public virtual void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            RowComplexType targetEntity = targetResource as RowComplexType;
            if (targetEntity != null)
            {
                if (IsCustomerInstance(targetEntity))
                {
                    targetEntity.Properties["GuidValue"] = Guid.NewGuid();
                    if (propertyName == "ID")
                    {
                        if (propertyValue == null || propertyValue.GetType() != typeof(int))
                        {
                            throw new ArgumentException("ID property must be of int type", "propertyValue");
                        }
                        targetEntity.Properties[propertyName] = (int)propertyValue;
                        return;
                    }
                    else if (propertyName == "Name")
                    {
                        if (propertyValue != null && propertyValue.GetType() != typeof(string))
                        {
                            throw new ArgumentException("Name property must be of string type", "propertyValue");
                        }
                        targetEntity.Properties[propertyName] = propertyValue;
                        return;
                    }
                    else if (propertyName == "GuidValue")
                    {
                        if (propertyValue != null && propertyValue.GetType() != typeof(Guid))
                        {
                            throw new ArgumentException("GuidValue property must be of Guid type", "propertyValue");
                        }
                        targetEntity.Properties[propertyName] = propertyValue;
                        return;
                    }
                    else if (propertyName == "BestFriend")
                    {
                        RowComplexType propertyEntity = (RowComplexType)propertyValue;
                        if (propertyValue != null && !IsCustomerInstance(propertyEntity))
                        {
                            throw new ArgumentException("BestFriend property must be of OpenCustomer type", "propertyValue");
                        }
                        targetEntity.Properties[propertyName] = (RowComplexType)propertyValue;
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
                        if (propertyValue.GetType() == typeof(string))
                        {
                            targetEntity.Properties["Birthday"] = XmlConvert.ToDateTimeOffset((string)propertyValue);
                        }
                        else
                        {
                            targetEntity.Properties["Birthday"] = (DateTimeOffset)propertyValue;
                        }

                        return;
                    }
                }
                else if (IsOrderInstance(targetEntity))
                {
                    if (propertyName == "ID")
                    {
                        targetEntity.Properties["ID"] = (int)propertyValue;
                        return;
                    }
                    else if (propertyName == "DollarAmount")
                    {
                        targetEntity.Properties["DollarAmount"] = propertyValue;
                        return;
                    }
                    else if (propertyName == "Customer")
                    {
                        RowComplexType propertyEntity = (RowComplexType)propertyValue;
                        if (propertyValue != null && !IsCustomerInstance(propertyEntity))
                        {
                            throw new ArgumentException("Order.Customer property must be of OpenCustomer type", "propertyValue");
                        }
                        targetEntity.Properties[propertyName] = (RowComplexType)propertyValue;
                        return;
                    }
                    else if (propertyName == "OpenProperty")
                    {
                        targetEntity.Properties["OpenProperty"] = propertyValue;
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

            throw new Exception(String.Format("Invalid property name '{0}' specified for type '{1}'", propertyName, ((RowComplexType)targetResource).TypeName));
        }

        public void ClearChanges()
        {
            this.pendingChanges.Clear();
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

        protected static bool IsCustomerInstance(RowComplexType resource)
        {
            return resource.TypeName == CustomRowBasedContext.CustomerFullName ||
                   resource.TypeName == CustomRowBasedContext.CustomerWithBirthdayFullName;
        }

        protected static bool IsOrderInstance(RowComplexType resource)
        {
            return resource.TypeName == CustomRowBasedContext.OrderFullName;
        }

        private RowComplexType GetCustomRowInstance(object resource)
        {
            RowComplexType entity = resource as RowComplexType;
            if (entity == null || !entity.Properties.ContainsKey("ID"))
            {
                throw new Exception("Invalid Entity instance passed");
            }

            return entity;
        }

        private static IList<RowComplexType> GetEntitySet(RowComplexType entity)
        {
            if (IsCustomerInstance(entity))
            {
                return customers;
            }

            if (IsOrderInstance(entity))
            {
                return orders;
            }

            throw new Exception(String.Format("Unexpected EntityType '{0}' encountered", entity.TypeName));
        }

        private static void AddResource(RowComplexType resource, bool throwIfDuplicate)
        {
            IList<RowComplexType> entitySetInstance = GetEntitySet(resource);

            foreach (RowComplexType entity in entitySetInstance)
            {
                // check if there is not another instance with the same id
                if (GetKey(resource) == GetKey(entity))
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

        private RowComplexType DeleteEntity(IList<RowComplexType> collection, RowComplexType entity, bool throwIfNotPresent)
        {
            RowComplexType entityToBeDeleted = TryGetEntity(collection, entity);

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
            customers = null;
            orders = null;
        }

        private static RowComplexType TryGetEntity(IList<RowComplexType> collection, RowComplexType entity)
        {
            RowComplexType matchingEntity = null;

            foreach (RowComplexType element in collection)
            {
                // check if there is not another instance with the same id
                if (object.Equals(GetKey(element), GetKey(entity)))
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
                CustomRowBasedOpenTypesContext.PreserveChanges = false;
                provider = null;
                queryProvider = null;
                CustomRowBasedOpenTypesContext.ClearData();
            }
        }

        private static IDataServiceMetadataProvider PopulateMetadata(object dataSourceInstance)
        {
            List<ResourceType> types = new List<ResourceType>(4);

            ResourceProperty keyProperty = CustomRowBasedContext.CreateNonClrProperty(
                "ID",
                ResourcePropertyKind.Key | ResourcePropertyKind.Primitive,
                ResourceType.GetPrimitiveResourceType(typeof(int)));

            ResourceType customer = new ResourceType(
                typeof(RowComplexType),
                ResourceTypeKind.EntityType,
                null, /*baseType*/
                "AstoriaUnitTests.Stubs", /*namespaceName*/
                "Customer",
                false /*isAbstract*/);
            customer.IsOpenType = true;
            customer.AddProperty(keyProperty);
            customer.AddProperty(CreateNonClrNonOpenProperty("GuidValue", ResourcePropertyKind.Primitive | ResourcePropertyKind.ETag, ResourceType.GetPrimitiveResourceType(typeof(Guid))));
            customer.CanReflectOnInstanceType = false;
            
            ResourceType customerWithBirthday = new ResourceType(
                typeof(RowComplexType),
                ResourceTypeKind.EntityType,
                customer, /*baseType*/
                "AstoriaUnitTests.Stubs", /*namespaceName*/
                "CustomerWithBirthday",
                false /*isAbstract*/);
            customerWithBirthday.IsOpenType = true;
            customerWithBirthday.CanReflectOnInstanceType = false;

            ResourceType order = new ResourceType(
                typeof(RowComplexType),
                ResourceTypeKind.EntityType,
                null,
                "AstoriaUnitTests.Stubs",
                "Order",
                false);
            order.IsOpenType = true;
            order.AddProperty(keyProperty);
            order.CanReflectOnInstanceType = false;

            if (OpenWebDataServiceHelper.EnableBlobServer)
            {
                customer.IsMediaLinkEntry = true;
            }

            ResourceType address = new ResourceType(
                typeof(RowComplexType),
                ResourceTypeKind.ComplexType,
                null,
                "AstoriaUnitTests.Stubs",
                "Address",
                false);
            address.CanReflectOnInstanceType = false;
            address.AddProperty(CreateNonClrNonOpenProperty("StreetAddress", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))));
            address.AddProperty(CreateNonClrNonOpenProperty("City", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))));
            address.AddProperty(CreateNonClrNonOpenProperty("State", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))));
            address.AddProperty(CreateNonClrNonOpenProperty("PostalCode", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))));

            ResourceType unusedType = new ResourceType(
                typeof(RowComplexType),
                ResourceTypeKind.ComplexType,
                null,
                "AstoriaUnitTests.Stubs",
                "UnusedType",
                false);
            unusedType.CanReflectOnInstanceType = false;
            unusedType.AddProperty(CreateNonClrNonOpenProperty("Data", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))));

            types.Add(customer);
            types.Add(customerWithBirthday);
            types.Add(order);
            types.Add(address);
            types.Add(unusedType);

            ResourceSet customersEntitySet = new ResourceSet("Customers", customer);
            ResourceSet ordersEntitySet = new ResourceSet("Orders", order);
            customer.AddProperty(CreateNonClrNonOpenProperty("BestFriend", ResourcePropertyKind.ResourceReference, customer));
            customer.AddProperty(CreateNonClrNonOpenProperty("Orders", ResourcePropertyKind.ResourceSetReference, order));
            order.AddProperty(CreateNonClrNonOpenProperty("Customer", ResourcePropertyKind.ResourceReference, customer));

            List<ResourceSet> containers = new List<ResourceSet>(2);
            containers.Add(customersEntitySet);
            containers.Add(ordersEntitySet);

            List<ServiceOperation> operations = new List<ServiceOperation>();
            operations.Add(new ServiceOperation("IntServiceOperation", ServiceOperationResultKind.DirectValue, ResourceType.GetPrimitiveResourceType(typeof(int)), null, "GET", null));
            operations.Add(new ServiceOperation("InsertCustomer", ServiceOperationResultKind.DirectValue, customer, customersEntitySet, "POST",
                new ServiceOperationParameter[] { new ServiceOperationParameter("id", ResourceType.GetPrimitiveResourceType(typeof(int))),
                                                  new ServiceOperationParameter("name", ResourceType.GetPrimitiveResourceType(typeof(string))) }
                                                  ));
            operations.Add(new ServiceOperation("GetCustomerByCity", ServiceOperationResultKind.QueryWithMultipleResults, customer, customersEntitySet, "GET",
                new ServiceOperationParameter[] { new ServiceOperationParameter("city", ResourceType.GetPrimitiveResourceType(typeof(string))) }));

            operations.Add(new ServiceOperation("GetAllCustomersQueryable", ServiceOperationResultKind.QueryWithMultipleResults, customer, customersEntitySet, "GET", null));
            operations.Add(new ServiceOperation("GetCustomerByIdQueryable", ServiceOperationResultKind.QueryWithSingleResult, customer, customersEntitySet, "GET", new ServiceOperationParameter[] { new ServiceOperationParameter("id", ResourceType.GetPrimitiveResourceType(typeof(int))) }));
            operations.Add(new ServiceOperation("GetAllCustomersEnumerable", ServiceOperationResultKind.Enumeration, customer, customersEntitySet, "GET", null));
            operations.Add(new ServiceOperation("GetCustomerByIdDirectValue", ServiceOperationResultKind.DirectValue, customer, customersEntitySet, "GET", new ServiceOperationParameter[] { new ServiceOperationParameter("id", ResourceType.GetPrimitiveResourceType(typeof(int))) }));

            operations.Add(new ServiceOperation("GetAllOrdersQueryable", ServiceOperationResultKind.QueryWithMultipleResults, order, ordersEntitySet, "GET", null));
            operations.Add(new ServiceOperation("GetOrderByIdQueryable", ServiceOperationResultKind.QueryWithSingleResult, order, ordersEntitySet, "GET", new ServiceOperationParameter[] { new ServiceOperationParameter("id", ResourceType.GetPrimitiveResourceType(typeof(int))) }));
            operations.Add(new ServiceOperation("GetAllOrdersEnumerable", ServiceOperationResultKind.Enumeration, order, ordersEntitySet, "GET", null));
            operations.Add(new ServiceOperation("GetOrderByIdDirectValue", ServiceOperationResultKind.DirectValue, order, ordersEntitySet, "GET", new ServiceOperationParameter[] { new ServiceOperationParameter("id", ResourceType.GetPrimitiveResourceType(typeof(int))) }));

            List<ResourceAssociationSet> associationSets = new List<ResourceAssociationSet>();

            ResourceAssociationSet customer_BestFriend =
                new ResourceAssociationSet(
                    "Customers_BestFriend",
                    new ResourceAssociationSetEnd(customersEntitySet, customer, customer.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "BestFriend")),
                    new ResourceAssociationSetEnd(customersEntitySet, customer, customer.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "BestFriend")));
            associationSets.Add(customer_BestFriend);

            ResourceAssociationSet customer_Order =
                new ResourceAssociationSet(
                    "Customers_Orders",
                    new ResourceAssociationSetEnd(customersEntitySet, customer, customer.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "Orders")),
                    new ResourceAssociationSetEnd(ordersEntitySet, order, order.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == "Customer")));

            associationSets.Add(customer_Order);

            if (CustomizeMetadata != null)
            {
                CustomizeMetadata(containers, types, operations, associationSets);
            }

            return new CustomDataServiceProvider(containers, types, operations, associationSets, dataSourceInstance);
        }

        internal static ResourceProperty CreateNonClrNonOpenProperty(string name, ResourcePropertyKind kind, ResourceType propertyType)
        {
            ResourceProperty property = new ResourceProperty(name, kind, propertyType);
            property.CanReflectOnInstanceTypeProperty = false;
            return property;
        }

        private static bool IEnumerableTypeFilter(Type m, object filterCriteria)
        {
            Debug.Assert(m != null, "m != null");
            return m.IsGenericType && m.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static Type GetGenericInterfaceElementType(Type type, TypeFilter typeFilter)
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

        private static object GetKey(RowComplexType entityType)
        {
            object keyValue;
            if (entityType.Properties.TryGetValue("ID", out keyValue))
            {
                return keyValue;
            }

            throw new Exception("Missing key value");
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

        #region Service Operations
        public int IntServiceOperation()
        {
            return 5;
        }

        public RowComplexType InsertCustomer(int id, string name)
        {
            RowComplexType customer = new RowComplexType(CustomRowBasedContext.CustomerFullName);
            customer.Properties["ID"] = id;
            customer.Properties["Name"] = name;
            AddResource(customer, true);
            return customer;
        }

        public IQueryable<RowComplexType> GetCustomerByCity(string city)
        {
            return this.Customers.Where(c => (string)((RowComplexType)c.Properties["Address"]).Properties["City"] == city).AsQueryable();
        }

        public IQueryable<RowComplexType> GetAllCustomersQueryable()
        {
            return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(customers.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName);
        }

        public IQueryable<RowComplexType> GetCustomerByIdQueryable(int id)
        {
            return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(customers.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName).Where(c => (int)c.Properties["ID"] == id);
        }

        public IEnumerable<RowComplexType> GetAllCustomersEnumerable()
        {
            return customers.AsEnumerable();
        }

        public RowComplexType GetCustomerByIdDirectValue(int id)
        {
            return customers.Single(c => (int)c.Properties["ID"] == id);
        }

        public IQueryable<RowComplexType> GetAllOrdersQueryable()
        {
            return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(orders.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName);
        }

        public IQueryable<RowComplexType> GetOrderByIdQueryable(int id)
        {
            return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(orders.AsQueryable(), queryProvider, RowComplexType.TypeNamePropertyName).Where(c => (int)c.Properties["ID"] == id);
        }

        public IEnumerable<RowComplexType> GetAllOrdersEnumerable()
        {
            return orders.AsEnumerable();
        }

        public RowComplexType GetOrderByIdDirectValue(int id)
        {
            return orders.Single(c => (int)c.Properties["ID"] == id);
        }

        #endregion Service Operations

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServiceMetadataProvider))
            {
                return provider;
            }
            if (serviceType == typeof(IDataServiceQueryProvider))
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

        public static Action<DataServiceConfiguration> CustomInitializeService;

        public static void InitializeService(DataServiceConfiguration config)
        {
            if (CustomInitializeService != null)
            {
                CustomInitializeService(config);
            }
        }
    }

    public class OpenTypeContextWithReflectionConfig
    {
        public static bool NullOnGetOpenPropertyValues { get; set; }
    }

    public class OpenTypeContextWithReflection<T> : IDataServiceUpdateProvider, IServiceProvider, IDataServiceMetadataProvider, IDataServiceQueryProvider
    {
        private ResourceType rt;
        private ResourceSet rs;
        private Dictionary<int, object> tokens;
        private List<T> values;
        private int tokenIndex;
        private string dictionaryProperty;

        public static event EventHandler ValuesRequested;

        /// <summary>Clears all event handlers.</summary>
        public static void ClearHandlers()
        {
            EventHandler handler = ValuesRequested;
            ValuesRequested = null;
            if (handler != null)
            {
                Trace.WriteLine("Clearing handler " + handler.ToString());
            }
        }

        public void SetValues(object[] newValues)
        {
            foreach (object value in newValues)
            {
                this.values.Add((T)value);
                this.tokens.Add(tokenIndex++, (T)value);
            }
        }

        public OpenTypeContextWithReflection()
        {
            rt = new ResourceType(typeof(T), ResourceTypeKind.EntityType, null, "AstoriaUnitTests.Stubs", "TypeOfValues", false);
            foreach (PropertyInfo pi in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (typeof(IDictionary<string, object>).IsAssignableFrom(pi.PropertyType))
                {
                    this.dictionaryProperty = pi.Name;
                    continue;
                }

                ResourcePropertyKind rpk = ResourcePropertyKind.Primitive;
                if (pi.Name.Contains("ID"))
                {
                    rpk |= ResourcePropertyKind.Key;
                }

                ResourceType primitiveType = ResourceType.GetPrimitiveResourceType(pi.PropertyType);
                if (primitiveType != null)
                {
                    ResourceProperty rp = new ResourceProperty(pi.Name, rpk, ResourceType.GetPrimitiveResourceType(pi.PropertyType));
                    rp.CanReflectOnInstanceTypeProperty = true;
                    rt.AddProperty(rp);
                }
            }

            rt.IsOpenType = true;

            rs = new ResourceSet("Values", rt);

            rs.SetReadOnly();

            this.tokens = new Dictionary<int, object>();
            this.values = new List<T>();
            this.tokenIndex = 0;
        }

        #region IDataServiceUpdateProvider Members

        public object CreateResource(string containerName, string fullTypeName)
        {
            Debug.Assert(fullTypeName == rt.FullName, "Only resource created should be of rt type.");
            ConstructorInfo ci = typeof(T).GetConstructor(Type.EmptyTypes);
            T instance = (T)ci.Invoke(new object[0]);

            int tokenToReturn = this.tokenIndex;
            this.values.Add(instance);
            this.tokens.Add(this.tokenIndex++, instance);
            return tokenToReturn;
        }

        public object GetResource(IQueryable query, string fullTypeName)
        {
            Debug.Assert(fullTypeName == rt.FullName || fullTypeName == null, "Only resource created should be of rt type.");
            foreach (T instance in query)
            {
                if (this.tokens.Values.Contains(instance))
                {
                    return this.tokens.Single(x => Object.ReferenceEquals(x.Value, instance)).Key;
                }
                else
                {
                    int tokenToReturn = this.tokenIndex;
                    this.tokens.Add(this.tokenIndex++, instance);
                    return tokenToReturn;
                }
            }

            return null;
        }

        public object ResetResource(object resource)
        {
            object targetResource = this.tokens[(int)resource];
            IDictionary<string, object> openProperties = (IDictionary<string, object>)targetResource
                            .GetType()
                            .GetProperty(dictionaryProperty, BindingFlags.Public | BindingFlags.Instance)
                            .GetValue(targetResource, null);
            openProperties.Clear();
            return resource;
        }

        public void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            targetResource = this.tokens[(int)targetResource];
            
            PropertyInfo property = targetResource.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                IDictionary<string, object> openProperties = (IDictionary<string, object>)targetResource
                                .GetType()
                                .GetProperty(dictionaryProperty, BindingFlags.Public | BindingFlags.Instance)
                                .GetValue(targetResource, null);

                openProperties[propertyName] = propertyValue;
            }
            else
            {
                property.SetValue(targetResource, propertyValue, null);
            }
        }

        public object GetValue(object targetResource, string propertyName)
        {
            targetResource = this.tokens[(int)targetResource];
            return this.GetValueHelper(targetResource, propertyName);
        }

        public void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            throw new NotImplementedException();
        }

        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            throw new NotImplementedException();
        }

        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            throw new NotImplementedException();
        }

        public void DeleteResource(object targetResource)
        {
            this.tokens.Remove((int)targetResource);
        }

        public void SaveChanges()
        {
        }

        public object ResolveResource(object resource)
        {
            return this.tokens[(int)resource];
        }

        public void ClearChanges()
        {
            throw new NotImplementedException();
        }

        public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            if (checkForEquality == null)
            {
                throw new DataServiceException("Missing If-Match header");
            }

            foreach (var pInfo in concurrencyValues)
            {
                object pValue = this.GetValue(resourceCookie, pInfo.Key);

                if (!Object.Equals(pValue, pInfo.Value))
                {
                    throw new DataServiceException(412, String.Format("Etag value not match for property '{0}'", pInfo.Key));
                }
            }
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IUpdatable) ||
                serviceType == typeof(IDataServiceMetadataProvider) ||
                serviceType == typeof(IDataServiceQueryProvider))
            {
                return this;
            }

            return null;
        }

        #endregion

        #region IDataServiceProvider Members

        public object CurrentDataSource
        {
            get { return this; }
            set
            {
                throw new NotSupportedException();
            }
        }

        public bool IsNullPropagationRequired
        {
            get { return true; }
        }

        public string ContainerNamespace
        {
            get { return "AstoriaUnitTests.Stubs"; }
        }

        public string ContainerName
        {
            get { return "CustomOpenTypeContextWithSingleType"; }
        }

        public IEnumerable<ResourceSet> ResourceSets
        {
            get { return new List<ResourceSet> { this.rs }; }
        }

        public IEnumerable<ResourceType> Types
        {
            get { return new List<ResourceType> { this.rt }; }
        }

        public IEnumerable<ServiceOperation> ServiceOperations
        {
            get { return Enumerable.Empty<ServiceOperation>(); }
        }

        public void DisposeDataSource()
        {
        }

        public bool TryResolveResourceSet(string name, out ResourceSet rs)
        {
            rs = this.rs;
            return true;
        }

        public ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            throw new NotImplementedException();
        }

        public IQueryable GetQueryRootForResourceSet(ResourceSet resourceSet)
        {
            if (ValuesRequested != null)
            {
                ValuesRequested(this, EventArgs.Empty);
            }
            else
            {
                PopulatingValuesEventArgs<T> args = new PopulatingValuesEventArgs<T>("Values", this.values);
                StaticCallbackManager<PopulatingValuesEventArgs<T>>.FireEvent(this, args);
                if (args.Values != values)
                {
                    values = args.Values;
                }

                foreach (T instance in values)
                {
                    if (!this.tokens.Values.Any(x => Object.ReferenceEquals(x, instance)))
                    {
                        this.tokens.Add(this.tokenIndex++, instance);
                    }
                }
            }

            return AstoriaUnitTests.Tests.UnitTestsUtil.GetQueryable(this.tokens.Values.Cast<T>().AsQueryable(), this, null);
        }

        public bool TryResolveResourceType(string name, out ResourceType rt)
        {
            if (this.rt.FullName == name)
            {
                rt = this.rt;
            }
            else
            {
                rt = null;
            }

            return rt != null;
        }

        public ResourceType GetResourceType(object target)
        {
            return this.rt;
        }

        public IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType)
        {
            return Enumerable.Empty<ResourceType>();
        }

        public bool HasDerivedTypes(ResourceType resourceType)
        {
            return false;
        }

        public bool TryResolveServiceOperation(string name, out ServiceOperation so)
        {
            so = null;
            return false;
        }

        public object GetPropertyValue(object target, ResourceProperty resourceProperty)
        {
            Assert.IsTrue(resourceProperty.CanReflectOnInstanceTypeProperty == false, "We should never call GetPropertyValue on reflectable properties");
            return this.GetValueHelper(target, resourceProperty.Name);
        }

        public object GetOpenPropertyValue(object target, string propertyName)
        {
            return this.GetValueHelper(target, propertyName);
        }

        public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            if (OpenTypeContextWithReflectionConfig.NullOnGetOpenPropertyValues)
            {
                return null;
            }

            IDictionary<string, object> openProperties = (IDictionary<string, object>)target
                            .GetType()
                            .GetProperty(dictionaryProperty, BindingFlags.Public | BindingFlags.Instance)
                            .GetValue(target, null);

            // Remove the non-open properties from here
            openProperties.Remove("GuidValue");
            return openProperties;
        }

        public object InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
        {
            throw new NotImplementedException();
        }

        #endregion

        private object GetValueHelper(object targetResource, string propertyName)
        {
            PropertyInfo property = targetResource.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                IDictionary<string, object> openProperties = (IDictionary<string, object>)targetResource
                                .GetType()
                                .GetProperty(dictionaryProperty, BindingFlags.Public | BindingFlags.Instance)
                                .GetValue(targetResource, null);

                object result;
                if (openProperties.TryGetValue(propertyName, out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return property.GetValue(targetResource, null);
            }
        }

    }

    public class Person
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
    }

    public class PropertyOwner
    {
        public string MiddleName { get; set; }
    }

    public class Context : IServiceProvider
    {
        private static IDataServiceMetadataProvider provider;
        private static IDataServiceQueryProvider queryProvider;

        public Context()
        {
            if (provider == null)
            {
                provider = PopulateMetadata(this);
                queryProvider = (IDataServiceQueryProvider)provider;
            }
        }

        private IDataServiceMetadataProvider PopulateMetadata(Context context)
        {
            List<ResourceType> types = new List<ResourceType>(2);

            ResourceType person = new ResourceType(typeof(Person), ResourceTypeKind.EntityType, null, "AtoriaUnitTests.Stubs.Sample", "Person", false);
            person.IsOpenType = true;
            person.CanReflectOnInstanceType = true;
            types.Add(person);

            ResourceProperty key = new ResourceProperty("ID", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, ResourceType.GetPrimitiveResourceType(typeof(int)));
            key.CanReflectOnInstanceTypeProperty = true;
            person.AddProperty(key);

            ResourceType propertyowner = new ResourceType(typeof(PropertyOwner), ResourceTypeKind.EntityType, person, "AtoriaUnitTests.Stubs.Sample", "PropertyOwner", false);
            propertyowner.CanReflectOnInstanceType = true;

            types.Add(propertyowner);

            ResourceSet PeopleEntitySet = new ResourceSet("People", person);

            ResourceProperty PersonFirstNameProperty = new ResourceProperty("FirstName", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(String)));
            PersonFirstNameProperty.CanReflectOnInstanceTypeProperty = true;
            person.AddProperty(PersonFirstNameProperty);

            ResourceProperty PersonLastNameProperty = new ResourceProperty("LastName", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(String)));
            PersonLastNameProperty.CanReflectOnInstanceTypeProperty = false;
            person.AddProperty(PersonLastNameProperty);

            ResourceProperty PersonMiddleNameProperty = new ResourceProperty("MiddleName", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(String)));
            PersonMiddleNameProperty.CanReflectOnInstanceTypeProperty = false;
            person.AddProperty(PersonMiddleNameProperty);

            return new CustomDataServiceProvider(new List<ResourceSet> { PeopleEntitySet }, types, new List<ServiceOperation>(), new List<ResourceAssociationSet>(), context);
        }

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServiceMetadataProvider))
            {
                return provider;
            }
            if (serviceType == typeof(IDataServiceQueryProvider))
            {
                return queryProvider;
            }

            return null;
        }

        #endregion
    }
}
