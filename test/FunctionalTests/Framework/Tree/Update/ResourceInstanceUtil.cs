//---------------------------------------------------------------------
// <copyright file="ResourceInstanceUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace System.Data.Test.Astoria
{
    public class ResourceInstanceUtil
    {
        private static ResourceInstanceKey TryCreateUniqueResourceInstanceKey(ResourceContainer container, ResourceType resType, KeyExpressions relatedForeignKeys)
        {
            Workspace workspace = container.Workspace;
            List<ResourceInstanceSimpleProperty> keyProperties = new List<ResourceInstanceSimpleProperty>();

            Dictionary<ResourceProperty, ResourceProperty> foreignKeyMap
                = resType.Key.Properties.OfType<ResourceProperty>()
                .Where(p => p.ForeignKeys.Any())
                .ToDictionary(p => p, p => p.ForeignKeys.First().PrimaryKey.Properties.OfType<ResourceProperty>().First());
            Dictionary<string, ResourceProperty> reverseForeignKeyMap = new Dictionary<string, ResourceProperty>();
            foreach (KeyValuePair<ResourceProperty, ResourceProperty> pair in foreignKeyMap)
                reverseForeignKeyMap[pair.Value.Name] = pair.Key;

            Dictionary<ResourceProperty, object> propertyValues = new Dictionary<ResourceProperty, object>();

            List<ResourceProperty> constrainedProperties = new List<ResourceProperty>();
            foreach(ResourceProperty property in resType.Key.Properties.OfType<ResourceProperty>())
            {
                if(foreignKeyMap.ContainsKey(property))
                    constrainedProperties.Add(property);
                else
                {
                    NodeValue obj = (property.Type as PrimitiveType).CreateRandomValueForFacets(property.Facets);
                    propertyValues[property] = obj.ClrValue;
                }
            }

            foreach (ResourceProperty currentProperty in constrainedProperties)
            {
                if (propertyValues.ContainsKey(currentProperty))
                    continue;

                ResourceProperty foreignProperty = foreignKeyMap[currentProperty];

                ResourceContainer foreignContainer = container.FindDefaultRelatedContainer(foreignProperty.ResourceType);
                KeyExpression foreignKey = relatedForeignKeys.Where(k => k.ResourceContainer == foreignContainer).FirstOrDefault();
                if (foreignKey == null)
                {
                    KeyExpressions foreignKeys = workspace.GetAllExistingKeysOfType(foreignContainer, foreignProperty.ResourceType);
                    while (foreignKey == null && foreignKeys.Any())
                    {
                        foreignKey = foreignKeys.Choose();

                        // ensure that for every property in the key, it matches any local values
                        for (int i = 0; i < foreignKey.Properties.Length; i++)
                        {
                            string keyPropertyName = foreignKey.Properties[i].Name;
                            ResourceProperty localProperty;
                            if (reverseForeignKeyMap.TryGetValue(keyPropertyName, out localProperty))
                            {
                                object keyValue = foreignKey.Values[i].ClrValue;
                                object localValue;
                                if (propertyValues.TryGetValue(localProperty, out localValue))
                                {
                                    if (localValue != keyValue)
                                    {
                                        foreignKeys.Remove(foreignKey);
                                        foreignKey = null;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (foreignKey == null)
                        AstoriaTestLog.FailAndThrow("Could not find an appropriate foreign key");
                    relatedForeignKeys.Add(foreignKey);
                }

                for (int i = 0; i < foreignKey.Properties.Length; i++)
                {
                    NodeProperty p = foreignKey.Properties[i];

                    if (p.Name == foreignProperty.Name)
                        propertyValues[currentProperty] = foreignKey.Values[i].ClrValue;
                    else if (p.ForeignKeys.Count() > 0)
                    {
                        string foreign = p.ForeignKeys.First().PrimaryKey.Properties.OfType<ResourceProperty>().First().Name;

                        ResourceProperty localProperty;
                        if (reverseForeignKeyMap.TryGetValue(foreign, out localProperty))
                            propertyValues[localProperty] = foreignKey.Values[i].ClrValue;
                    }
                }
            }

            foreach(ResourceProperty property in resType.Key.Properties.OfType<ResourceProperty>())
                if(!propertyValues.ContainsKey(property))
                    propertyValues[property] = (object)null;

            foreach (KeyValuePair<ResourceProperty, object> pair in propertyValues)
            {
                keyProperties.Add(new ResourceInstanceSimpleProperty(pair.Key.Facets, pair.Key.Name, pair.Value));
            }

            ResourceInstanceKey resourceInstanceKey = new ResourceInstanceKey(container, resType, keyProperties.ToArray());
            return resourceInstanceKey;
        }
        public static ResourceInstanceKey CreateUniqueKey(ResourceContainer container, ResourceType resType)
        {
            return CreateUniqueKey(container, resType, new KeyExpressions());
        }

        internal static ResourceInstanceKey CreateUniqueKey(ResourceContainer container, ResourceType resType, KeyExpressions relatedForeignKeys)
        {
            return CreateUniqueKey(container, resType, relatedForeignKeys, null);
        }

        internal static ResourceInstanceKey CreateUniqueKey(ResourceContainer container, ResourceType resType, KeyExpressions relatedForeignKeys, KeyExpressions existingKeys)
        {
            KeyExpressions possibleRelatedForeignKeys = new KeyExpressions();
            Workspace workspace = container.Workspace;
            int keysGenerated = 0;
            bool keyTrying = true;
            ResourceInstanceKey resourceInstanceKey = null;
            do
            {
                possibleRelatedForeignKeys = new KeyExpressions();
                resourceInstanceKey = TryCreateUniqueResourceInstanceKey(container, resType, possibleRelatedForeignKeys);

                KeyExpression keyExpression = resourceInstanceKey.CreateKeyExpression(container, resType);

                // need to make sure its not a duplicate
                //
                if (existingKeys == null)
                {
                    KeyedResourceInstance o = workspace.GetSingleResourceByKey(keyExpression);

                    if (o == null)
                        keyTrying = false;
                }
                else
                {
                    keyTrying = existingKeys.Contains(keyExpression);
                }

                keysGenerated++;
                if (keysGenerated > 25)
                    throw new Microsoft.Test.ModuleCore.TestFailedException("Unable to create a unique key");
            }
            while (keyTrying);
            relatedForeignKeys.Add(possibleRelatedForeignKeys);
            return resourceInstanceKey;
        }
        public static KeyedResourceInstance CreateKeyedResourceInstance(KeyExpression exp, ResourceContainer container, params ResourceInstanceProperty[] properties)
        {
            ResourceType resType = exp.Properties.OfType<ResourceProperty>().First().ResourceType;
            ResourceInstanceKey instanceKey = ResourceInstanceKey.ConstructResourceInstanceKey(exp);
            return new KeyedResourceInstance(instanceKey, properties);
        }
        public static KeyedResourceInstance CreateKeyedResourceInstanceByClone(ResourceContainer container, ResourceType resourceType)
        {
            return CreateKeyedResourceInstanceByClone(container, resourceType, false);
        }

        public static ResourceInstanceProperty CreateProperty(ResourceProperty rp)
        {
            return CreateProperty(rp, false);
        }

        public static ResourceInstanceProperty CreateProperty(ResourceProperty rp, bool createDollarValueVersion)
        {
            if (createDollarValueVersion == true && rp.IsComplexType)
                throw new ArgumentException("Cannot do a $Value of a ComplexType");
            if (rp.IsComplexType)
            {
                return new ResourceInstanceComplexProperty(rp.Type.Name,
                    rp.Name, CreateComplexResourceInstance((ComplexType)rp.Type));
            }
            else if (!rp.IsNavigation)
            {
                NodeValue nodeValue = Resource.CreateValue(rp);
                return new ResourceInstanceSimpleProperty(rp.Name, nodeValue, createDollarValueVersion);
            }
            else
                throw new ArgumentException("Property has to be a complex type or simple property");
        }
   
        public static ComplexResourceInstance CreateComplexResourceInstance(ComplexType type)
        {
            List<ResourceInstanceProperty> instanceProperties = new List<ResourceInstanceProperty>();
            foreach (ResourceProperty childProperty in type.Properties)
            {
                ResourceInstanceProperty resourceInstanceProperty = null;
                if (childProperty.IsComplexType)
                {
                    resourceInstanceProperty = new ResourceInstanceComplexProperty(childProperty.Type.Name, childProperty.Name, CreateComplexResourceInstance((ComplexType)childProperty.Type));
                }
                else
                {
                    NodeValue nodeValue = Resource.CreateValue(childProperty);
                    resourceInstanceProperty = new ResourceInstanceSimpleProperty(childProperty.Name, nodeValue);
                }
                instanceProperties.Add(resourceInstanceProperty);
            }
            ComplexResourceInstance complexInstance = new ComplexResourceInstance(type.Name, instanceProperties.ToArray());
            return complexInstance;
        }

        public static KeyedResourceInstance CreateKeyedResourceInstanceByClone(ResourceContainer container, ResourceType resourceType, bool excludeRelationships)
        {
            Workspace workspace = container.Workspace;
            //Clone for an existing resource, and update its key
            KeyExpression keyExpression = workspace.GetRandomExistingKey(container, resourceType);

            if (keyExpression == null)
                return null;

            KeyedResourceInstance dataObject = workspace.GetSingleResourceByKey(keyExpression);
            if (dataObject == null)
                return null;

            ResourceInstanceKey key = null;
            
            // if there are any non-server-generated key properties, then we need to build the key
            if (keyExpression.ResourceType.Properties.Any(p => p.PrimaryKey != null && !p.Facets.ServerGenerated))
            {
                ResourceType newResourceType = container.ResourceTypes.Where(rt => rt.Name == dataObject.TypeName).FirstOrDefault();
                key = CreateUniqueKey(container, newResourceType);
            }

            //Foreach property in dataObject create a ResourceProperty
            List<ResourceInstanceProperty> properties = new List<ResourceInstanceProperty>();
            properties.AddRange(dataObject.Properties.OfType<ResourceInstanceSimpleProperty>().ToArray());
            properties.AddRange(dataObject.Properties.OfType<ResourceInstanceComplexProperty>().ToArray());
            if (!excludeRelationships)
                properties.AddRange(CloneRequiredRelationships(container, keyExpression.ResourceType, keyExpression));

            KeyedResourceInstance keyResourceInstance = null;
            if (key != null)
                keyResourceInstance = new KeyedResourceInstance(key, properties.ToArray());
            else
                keyResourceInstance = new KeyedResourceInstance(dataObject.ResourceSetName, dataObject.TypeName, properties.ToArray());
            return keyResourceInstance;

        }
        public static ResourceInstanceProperty CreateRefInstanceProperty(KeyExpression keyExp, ResourceContainer container, ResourceProperty navProperty)
        {
            ResourceType navResourceType = navProperty.Type as ResourceType;
            ResourceInstanceKey resourceInstanceKey = ResourceInstanceKey.ConstructResourceInstanceKey(keyExp);
            return new ResourceInstanceNavRefProperty(navProperty.Name, new AssociationResourceInstance(resourceInstanceKey, AssociationOperation.Add));
        }
        public static ResourceInstanceProperty CreateCollectionInstanceProperty(KeyExpressions keyExps, ResourceContainer container, ResourceProperty navProperty)
        {
            ResourceType navResourceType = (navProperty.Type as CollectionType).SubType as ResourceType;
            List<AssociationResourceInstance> associationNodes = new List<AssociationResourceInstance>();
            foreach (KeyExpression associatedKey in keyExps)
            {
                ResourceInstanceKey resourceInstanceKey = ResourceInstanceKey.ConstructResourceInstanceKey(associatedKey);
                associationNodes.Add(new AssociationResourceInstance(resourceInstanceKey, AssociationOperation.Add));
            }
            if (associationNodes.Count > 0)
            {
                ResourceInstanceNavColProperty navigationProperty = new ResourceInstanceNavColProperty(navProperty.Name, associationNodes.ToArray());
                return navigationProperty;
            }
            return null;
        }
        private static List<ResourceInstanceProperty> CloneRequiredRelationships(ResourceContainer container, ResourceType resourceType, KeyExpression keyExpression)
        {
            List<ResourceInstanceProperty> properties = new List<ResourceInstanceProperty>();
            //Foreach Navigation Property in dataObject create a bind 
            Dictionary<ResourceProperty, KeyExpressions> navigationProperties = GetAllAssociatedKeys(container, resourceType, keyExpression);
            foreach (ResourceProperty navProperty in navigationProperties.Keys)
            {
                //ResourceAssociationEnd otherEnd = navProperty.ResourceAssociation.GetOtherEnd(navProperty);
                ResourceType navPropertyResourceType = (navProperty.Type is CollectionType ? (navProperty.Type as CollectionType).SubType : navProperty.Type) as ResourceType;
                ResourceContainer otherContainer = container.FindDefaultRelatedContainer(navProperty);

                bool foreignKeyViolation = false;
                foreach (ResourceProperty otherProperty in otherContainer.BaseType.Key.Properties)
                {
                    if (otherProperty.ForeignKeys.Count() > 0)
                    {
                        ResourceType otherType = otherProperty.ForeignKeys.First().PrimaryKey.Properties.OfType<ResourceProperty>().First().ResourceType;
                        if (otherType == container.BaseType)
                        {
                            foreignKeyViolation = true;
                            break;
                        }
                    }
                }
                if (foreignKeyViolation)
                    continue;

                KeyExpressions keyExpressions = navigationProperties[navProperty];
                if (navProperty.Type is ResourceType)
                {
                    if (keyExpressions.Count > 0)
                    {
                        properties.Add(CreateRefInstanceProperty(keyExpressions[0], otherContainer, navProperty));
                    }
                }
                else
                {
                    ResourceInstanceProperty property = CreateCollectionInstanceProperty(keyExpressions, otherContainer, navProperty);
                    if (property != null)
                    {
                        properties.Add(property);
                    }
                }

            }
            return properties;
        }
        public static Dictionary<ResourceProperty, KeyExpressions> GetAllAssociatedKeys(ResourceContainer container, ResourceType resourceType, KeyExpression keyExpression)
        {
            Workspace workspace = container.Workspace;
            Dictionary<ResourceProperty, KeyExpressions> listOfKeyExpressions = new Dictionary<ResourceProperty, KeyExpressions>();

            foreach (ResourceProperty resourceProperty in resourceType.Properties)
            {
                if (resourceProperty.IsNavigation)
                {
                    KeyExpressions keys = workspace.GetExistingAssociatedKeys(container, resourceProperty, keyExpression);
                    listOfKeyExpressions.Add(resourceProperty, keys);
                }
            }
            return listOfKeyExpressions;
        }

        private static ComplexResourceInstance CloneObjectToComplexResourceInstance(ComplexType complexType, object o)
        {
            IEnumerable<ResourceProperty> resourceProperties = complexType.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation == false).ToArray();
            return new ComplexResourceInstance(complexType.Name, CloneObjectToResourceInstanceProperties(resourceProperties.ToArray(), o).ToArray());
        }
        internal static KeyedResourceInstance CreateKeyedResourceInstanceByExactClone(ResourceContainer container, ResourceType resourceType, object dataObject)
        {
            List<ResourceInstanceProperty> properties = new List<ResourceInstanceProperty>();
            List<ResourceInstanceProperty> keyProperties = new List<ResourceInstanceProperty>();
            properties.AddRange(CloneObjectToResourceInstanceProperties(resourceType.Properties.OfType<ResourceProperty>().Where(rp => rp.PrimaryKey == null && rp.IsNavigation == false), dataObject));
            keyProperties.AddRange(CloneObjectToResourceInstanceProperties(resourceType.Properties.OfType<ResourceProperty>().Where(rp => rp.PrimaryKey != null && rp.IsNavigation == false), dataObject));

            ResourceInstanceKey key = new ResourceInstanceKey(container, resourceType, keyProperties.ToArray());
            KeyedResourceInstance keyResourceInstance = new KeyedResourceInstance(key, properties.ToArray());
            return keyResourceInstance;

        }
        private static List<ResourceInstanceProperty> CloneObjectToResourceInstanceProperties(IEnumerable<ResourceProperty> properties, object o)
        {
            List<ResourceInstanceProperty> instanceProperties = new List<ResourceInstanceProperty>();
            foreach (ResourceProperty resProperty in properties)
            {
                if (resProperty.IsNavigation)
                    continue;
                object propertyObject = o.GetType().InvokeMember(resProperty.Name, System.Reflection.BindingFlags.GetProperty, null, o, new object[] { });
                ResourceInstanceProperty resourceInstanceProperty = null;
                if (resProperty.Type is ComplexType)
                {
                    ComplexResourceInstance complexInstance = CloneObjectToComplexResourceInstance(resProperty.Type as ComplexType, propertyObject);
                    resourceInstanceProperty = new ResourceInstanceComplexProperty(resProperty.Type.Name, resProperty.Name, complexInstance);
                }
                else
                {
                    resourceInstanceProperty = new ResourceInstanceSimpleProperty(resProperty.Name, new NodeValue(propertyObject, resProperty.Type));
                }
                instanceProperties.Add(resourceInstanceProperty);
            }
            return instanceProperties;
        }

        internal static KeyedResourceInstance CreateKeyedResourceInstanceFromPayloadObject(ResourceContainer container, ResourceType resourceType, PayloadObject payloadObject)
        {
            List<ResourceInstanceProperty> properties = new List<ResourceInstanceProperty>();
            List<ResourceInstanceProperty> keyProperties = new List<ResourceInstanceProperty>();

            foreach (ResourceProperty property in resourceType.Properties.OfType<ResourceProperty>())
            {
                if (property.IsNavigation)
                    continue;

                if (property.IsComplexType)
                {
                    PayloadComplexProperty fromPayload = payloadObject[property.Name] as PayloadComplexProperty;
                    properties.Add(ConvertComplexPayloadObjectToComplexProperty(property, fromPayload));
                }
                else
                {
                    string stringValue;
                    if (payloadObject.PayloadProperties.Any(p => p.Name == property.Name))
                    {
                        PayloadSimpleProperty fromPayload = payloadObject[property.Name] as PayloadSimpleProperty;
                        stringValue = fromPayload.Value;
                    }
                    else
                    {
                        if (!payloadObject.CustomEpmMappedProperties.TryGetValue(property.Name, out stringValue))
                            stringValue = null;
                    }

                    ResourceInstanceProperty newProperty = null;
                    object val = CommonPayload.DeserializeStringToObject(stringValue, property.Type.ClrType, false, payloadObject.Format);
                    newProperty = new ResourceInstanceSimpleProperty(property.Name, new NodeValue(val, property.Type));

                    if (property.PrimaryKey != null)
                        keyProperties.Add(newProperty);
                    else
                        properties.Add(newProperty);
                }
            }

            ResourceInstanceKey key = new ResourceInstanceKey(container, resourceType, keyProperties.ToArray());
            return new KeyedResourceInstance(key, properties.ToArray());

        }
        private static ResourceInstanceComplexProperty ConvertComplexPayloadObjectToComplexProperty(ResourceProperty rp, PayloadComplexProperty payloadComplexProperty)
        {
            List<ResourceInstanceProperty> properties = new List<ResourceInstanceProperty>();
            foreach (ResourceProperty childProperty in (rp.Type as ComplexType).Properties.OfType<ResourceProperty>())
            {
                if (childProperty.IsComplexType)
                {
                    PayloadComplexProperty fromPayload = payloadComplexProperty.PayloadProperties[childProperty.Name] as PayloadComplexProperty;
                    properties.Add(ConvertComplexPayloadObjectToComplexProperty(childProperty, fromPayload));
                }
                else
                {
                    PayloadSimpleProperty fromPayload = payloadComplexProperty.PayloadProperties[childProperty.Name] as PayloadSimpleProperty;
                    if (fromPayload != null)
                    {
                        ResourceInstanceProperty newProperty = null;
                        object val = CommonPayload.DeserializeStringToObject(fromPayload.Value, childProperty.Type.ClrType, false, fromPayload.ParentObject.Format);
                        newProperty = new ResourceInstanceSimpleProperty(childProperty.Name, new NodeValue(val, childProperty.Type));
                        properties.Add(newProperty);
                    }
                }
            }
            ComplexResourceInstance complexResourceInstance = new ComplexResourceInstance(rp.Type.Name, properties.ToArray());
            return new ResourceInstanceComplexProperty(rp.Type.Name, rp.Name, complexResourceInstance);
        }

        public static KeyExpression ConvertToKeyExpression(ResourceInstanceKey key, Workspace workspace)
        {
            ResourceContainer container = workspace.ServiceContainer.ResourceContainers[key.ResourceSetName];
            if (container == null)
                return null;
            ResourceType type = container.ResourceTypes.FirstOrDefault(rt => rt.Name == key.ResourceTypeName);
            if (type == null)
                return null;
            return key.CreateKeyExpression(container, type);
        }
    }
}
