//---------------------------------------------------------------------
// <copyright file="ODataReflectionUpdateProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;

    public class ODataReflectionUpdateProvider : IODataUpdateProvider
    {
        private readonly List<Action> pendingChanges = new List<Action>();

        public virtual object Create(string fullTypeName, object source)
        {
            if (string.IsNullOrEmpty(fullTypeName)) throw new ArgumentNullException("fullTypeName");
            if (source == null) throw new ArgumentNullException("source");

            var type = EdmClrTypeUtils.GetInstanceType(fullTypeName);
            var instance = Utility.QuickCreateInstance(type);
            this.pendingChanges.Add(() => ((IList)source).Add(instance));

            return instance;
        }

        public virtual void CreateLink(object parent, string propertyName, object target)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");
            if (target == null) throw new ArgumentNullException("target");

            this.pendingChanges.Add(() =>
            {
                var collection = (IList)parent.GetType().GetProperty(propertyName).GetValue(parent, null);
                collection.Add(target);
            });
        }

        public virtual void Delete(object target)
        {
            if (target == null) throw new ArgumentNullException("target");

            this.pendingChanges.Add(() => DeletionContext.Current.ExecuteAction(target));
        }

        public virtual void DeleteLink(object parent, string propertyName, object target)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

            if (target == null)
            {
                // single-valued navigation property
                this.pendingChanges.Add(() => parent.GetType().GetProperty(propertyName).SetValue(parent, null, null));
            }
            else
            {
                // collection-valued navigation property
                this.pendingChanges.Add(() =>
                {
                    var collection = (IList)parent.GetType().GetProperty(propertyName).GetValue(parent, null);
                    collection.Remove(target);
                });
            }
        }

        public virtual void Update(object target, string propertyName, object propertyValue)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

            this.pendingChanges.Add(() => UpdateCore(target, propertyName, propertyValue));
        }

        public virtual void UpdateLink(object parent, string propertyName, object target)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");
            if (target == null) throw new ArgumentNullException("target");

            this.pendingChanges.Add(() => parent.GetType().GetProperty(propertyName).SetValue(parent, target, null));
        }

        public virtual void UpdateETagValue(object target)
        {
            if (target == null) throw new ArgumentNullException("target");

            this.pendingChanges.Add(() =>
            {
                var propertyName = Utility.GetETagPropertyName(target);
                var propertyInfo = target.GetType().GetProperty(propertyName);
                // assumes the type of ETag field is Int64 (long)
                var newValue = DateTime.UtcNow.Ticks;
                propertyInfo.SetValue(target, newValue, null);
            });
        }

        public virtual void SaveChanges()
        {
            foreach (var change in this.pendingChanges)
            {
                change();
            }

            this.pendingChanges.Clear();
        }

        public virtual void ClearChanges()
        {
            this.pendingChanges.Clear();
        }

        private static void UpdateCore(object target, string propertyName, object propertyValue)
        {
            var odataCollectionValue = propertyValue as ODataCollectionValue;
            var odataEnumValue = propertyValue as ODataEnumValue;
            var odataPrimitiveValue = propertyValue as ODataPrimitiveValue;

            if (odataCollectionValue != null)
            {
                var property = target.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    var collection = Utility.QuickCreateInstance(property.PropertyType);

                    foreach (var item in odataCollectionValue.Items)
                    {
                        var itemType = property.PropertyType.GetGenericArguments().Single();
                        odataCollectionValue = item as ODataCollectionValue;
                        object collectionItem = null;
                        if (odataCollectionValue != null)
                        {
                            // TODO, check should support this type or not
                            throw new NotImplementedException();
                        }
                        else
                        {
                            collectionItem = ODataObjectModelConverter.ConvertPropertyValue(item, itemType);
                        }

                        property.PropertyType.GetMethod("Add").Invoke(collection, new object[] { collectionItem });
                    }

                    property.SetValue(target, collection, null);

                    return;
                }
            }
            else
            {
                var property = target.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    property.SetValue(target, ODataObjectModelConverter.ConvertPropertyValue(propertyValue, property.PropertyType), null);
                    return;
                }
            }

            var openClrObject = target as OpenClrObject;
            if (openClrObject != null)
            {
                var structuredType = (IEdmStructuredType)EdmClrTypeUtils.GetEdmType(DataSourceManager.GetCurrentDataSource().Model, openClrObject);

                //check if the edmType is an open type
                if (structuredType.IsOpen)
                {
                    if (odataCollectionValue != null)
                    {
                        // Collection of Edm.String
                        if (odataCollectionValue.TypeName == "Collection(Edm.String)")
                        {
                            var collection = new Collection<string>();
                            foreach (var it in odataCollectionValue.Items)
                            {
                                collection.Add(it as string);
                            }
                            openClrObject.OpenProperties[propertyName] = collection;
                        }
                        else
                        {
                            // TODO: handle other types.
                            throw new NotImplementedException();
                        }
                    }
                    else if (odataEnumValue != null)
                    {
                        var type = EdmClrTypeUtils.GetInstanceType(odataEnumValue.TypeName);
                        openClrObject.OpenProperties[propertyName] = ODataObjectModelConverter.ConvertPropertyValue(propertyValue, type);
                    }
                    else if (odataPrimitiveValue != null)
                    {
                        openClrObject.OpenProperties[propertyName] = odataPrimitiveValue.Value;
                    }
                    else
                    {
                        openClrObject.OpenProperties[propertyName] = propertyValue;
                    }
                }
            }
        }
    }
}
