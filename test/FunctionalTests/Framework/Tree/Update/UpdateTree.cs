//---------------------------------------------------------------------
// <copyright file="UpdateTree.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Data.Test.Astoria.Util;

    public enum RequestVerb
    {
        Get, Put, Post, Delete, Options, Unknown, Patch
    }

    public static class RequestVerbExtensions
    {
        public static string ToHttpMethod(this RequestVerb verb)
        {
            return verb.ToString().ToUpperInvariant();
        }
    }

    public enum AssociationOperation
    {
        Add, Remove
    }


    public class NamedNode : ExpNode
    {
        public NamedNode(string name)
            : base(name)
        {
        }

        public override NodeType Type
        {
            get { return null; }
        }
    }

    public class ResourceBodyTree : NamedNode
    {
        public ResourceBodyTree(string name)
            : base(name)
        {
        }
    }

    public class ResourceInstanceKey : NamedNode
    {
        private string _resourceSetName;
        private string _resourceTypeName;
        private ResourceInstanceProperties _keyProperties;

        public ResourceInstanceKey(ResourceContainer container, ResourceType resourceType, params ResourceInstanceProperty[] keyProperties)
            : this(container.Name, resourceType.Name, keyProperties)
        {
        }

        public ResourceInstanceKey(string resourceSetName, string resourceTypeName, params ResourceInstanceProperty[] keyProperties)
            : base("ResourceInstanceKey")
        {
            _resourceSetName = resourceSetName;
            _resourceTypeName = resourceTypeName;
            _keyProperties = new ResourceInstanceProperties(keyProperties);
        }
        public KeyExpression CreateKeyExpression(ResourceContainer container, ResourceType resourceType)
        {
            List<ResourceProperty> properties = new List<ResourceProperty>();
            List<NodeValue> values = new List<NodeValue>();
            foreach (ResourceInstanceSimpleProperty sp in this.KeyProperties)
            {

                ResourceProperty resProperty = resourceType.Properties.OfType<ResourceProperty>().Where(p => p.Name.Equals(sp.Name)).Single();
                properties.Add(resProperty);
                values.Add(new NodeValue(sp.PropertyValue, resProperty.Type));
            }
            return Exp.Key(container, resourceType, properties.ToArray(), values.ToArray());
        }
        public void CreateFormattedResourceKeyInformation(out string keyProperties, out string keyPropertyTypes, out string keyPropertyValues)
        {
            keyProperties = "";
            keyPropertyTypes = "";
            keyPropertyValues = "";

            foreach (ResourceInstanceSimpleProperty property in this.KeyProperties)
            {
                if (keyProperties == "")
                {
                    keyProperties = property.Name;
                    keyPropertyTypes = property.ClrType.ToString();
                    //TODO:Need to fix this up later
                    keyPropertyValues = JsonPrimitiveTypesUtil.PrimitiveToString(property.PropertyValue, property.ClrType);
                }
                else
                {
                    keyProperties = keyProperties + "," + property.Name;
                    keyPropertyTypes = keyPropertyTypes + "," + property.ClrType.ToString();
                    //TODO:Need to fix this up later
                    keyPropertyValues = keyPropertyValues + "," + JsonPrimitiveTypesUtil.PrimitiveToString(property.PropertyValue, property.ClrType);
                }
            }
        }
        public static ResourceInstanceKey ConstructResourceInstanceKey(KeyExpression keyExpression)
        {
            List<ResourceInstanceSimpleProperty> keyProperties = new List<ResourceInstanceSimpleProperty>();
            for (int i = 0; i < keyExpression.Properties.Count(); i++)
            {
                NodeProperty p = keyExpression.Properties.ElementAt(i);
                NodeValue nodeVal = keyExpression.Values.ElementAt(i);
                keyProperties.Add(new ResourceInstanceSimpleProperty(p.Name, nodeVal.ClrValue));
            }

            ResourceInstanceKey instanceKey = new ResourceInstanceKey(keyExpression.ResourceContainer, keyExpression.ResourceType, keyProperties.ToArray());
            return instanceKey;

        }
        public static ResourceInstanceKey ConstructResourceInstanceKey(string setName, string typeName, string keyPropertyNames, string keyPropertyTypeNames, string keyValues)
        {
            string[] keyPropertiesArr = keyPropertyNames.Split(',');
            string[] keyPropertyTypeNamesArr = keyPropertyTypeNames.Split(',');
            string[] keyValuesArr = keyValues.Split(',');
            Type[] keyPropertyTypes = new Type[keyPropertyTypeNamesArr.Count()];
            //Convert typenames to actual types
            for (int i = 0; i < keyPropertyTypeNamesArr.Count(); i++)
            {
                keyPropertyTypes[i] = System.Type.GetType(keyPropertyTypeNamesArr[i]);
            }

            //Create keyProperties
            List<ResourceInstanceSimpleProperty> keyProperties = new List<ResourceInstanceSimpleProperty>();
            for (int j = 0; j < keyProperties.Count; j++)
            {
                object o = JsonPrimitiveTypesUtil.StringToPrimitive(keyValuesArr[j], keyPropertyTypes[j]);
                keyProperties.Add(new ResourceInstanceSimpleProperty(keyPropertiesArr[j], o));
            }
            return new ResourceInstanceKey(setName, typeName, keyProperties.ToArray());
        }
        public string ResourceSetName
        {
            [DebuggerStepThrough]
            get { return _resourceSetName; }
            set { _resourceSetName = value; }
        }

        public string ResourceTypeName
        {
            [DebuggerStepThrough]
            get { return _resourceTypeName; }
            set { _resourceTypeName = value; }
        }

        public ResourceInstanceProperties KeyProperties
        {
            [DebuggerStepThrough]
            get { return _keyProperties; }
        }

        public override int GetHashCode()
        {
            int result = 0;
            foreach (ResourceInstanceSimpleProperty property in this.KeyProperties)
            {
                result ^= property.Name.GetHashCode() ^ property.PropertyValue.GetHashCode();
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            ResourceInstanceKey otherKey = obj as ResourceInstanceKey;
            if (otherKey != null)
                return this.Equals(otherKey);
            return false;
        }

        public bool Equals(ResourceInstanceKey key)
        {
            foreach (ResourceInstanceSimpleProperty sp in key.KeyProperties)
            {
                bool partialKeyValueFound = false;
                foreach (ResourceInstanceSimpleProperty sp2 in this.KeyProperties)
                {
                    if (sp2.Name == sp.Name && sp.PropertyValue.Equals(sp2.PropertyValue))
                    {
                        partialKeyValueFound = true;
                        break;
                    }
                }
                if (!partialKeyValueFound)
                    return false;
            }
            return true;
        }
    }

    public class ComplexResourceInstance : ResourceBodyTree
    {
        public ComplexResourceInstance(string typeName, params ResourceInstanceProperty[] properties)
            : base("ResourceInstance")
        {
            Properties = new ResourceInstanceProperties(properties);
            TypeName = typeName;
        }

        public ResourceInstanceProperties Properties
        {
            get;
            private set;
        }

        public override object Clone()
        {
            ComplexResourceInstance result = (ComplexResourceInstance)base.Clone();
            result.Properties = (ResourceInstanceProperties)this.Properties.Clone();
            return result;
        }

        public string TypeName
        {
            get;
            private set;
        }
    }

    public class KeyedResourceInstance : ComplexResourceInstance
    {
        public KeyedResourceInstance(ResourceInstanceKey key, params ResourceInstanceProperty[] properties)
            : base(key.ResourceTypeName, properties)
        {
            TestUtil.CheckArgumentElementsNotNull(properties, "properties");

            ResourceInstanceKey = key;
            ResourceSetName = key.ResourceSetName;
            IncludeTypeMetadataHint = true;
        }

        public bool IncludeTypeMetadataHint
        {
            get;
            set;
        }

        public KeyedResourceInstance(string resSetName, string resType, params ResourceInstanceProperty[] properties)
            : base(resType, properties)
        {
            TestUtil.CheckArgumentElementsNotNull(properties, "properties");

            ResourceSetName = resSetName;
            IncludeTypeMetadataHint = true;
        }

        public KeyedResourceInstance(ResourceContainer container, ResourceType resType, params ResourceInstanceProperty[] properties) :
            this(container.Name, resType.Name, properties.Where(p => !resType.Key.Properties.Any(keyProp => keyProp.Name == p.Name)).ToArray())
        {
            ResourceInstanceKey = new ResourceInstanceKey(container, resType,
                properties.Where(p => resType.Key.Properties.Any(keyProp => keyProp.Name == p.Name)).ToArray());
        }

        public string ResourceSetName
        {
            get;
            private set;
        }

        public ResourceInstanceProperties KeyProperties
        {
            get
            {
                if (ResourceInstanceKey == null)
                {
                    ResourceInstanceKey = new ResourceInstanceKey(ResourceSetName, TypeName);
                }
                return ResourceInstanceKey.KeyProperties;
            }
        }

        public ResourceInstanceKey ResourceInstanceKey
        {
            get;
            set;
        }
    }

    public class AssociationResourceInstance : KeyedResourceInstance
    {
        private AssociationOperation _operation;
        private bool _includeTypeInBind = true;
        public AssociationResourceInstance(ResourceInstanceKey key, AssociationOperation operation)
            : base(key)
        {
            _operation = operation;
        }

        public AssociationOperation Operation
        {
            get { return _operation; }
        }
        public bool IncludeTypeInBind
        {
            get { return _includeTypeInBind; }
            set { _includeTypeInBind = value; }
        }
    }

    public abstract class ResourceInstanceProperty : ResourceBodyTree
    {
        public ResourceInstanceProperty(string propertyName)
            : base(propertyName)
        {
            IncludeTypeMetadataHint = true;
        }

        public bool IncludeTypeMetadataHint
        {
            get;
            set;
        }
    }

    public class ResourceInstanceComplexProperty : ResourceInstanceProperty
    {
        ComplexResourceInstance _complexResourceInstance;
        private string _typeName;

        public ResourceInstanceComplexProperty(string typeName, string propertyName, ComplexResourceInstance complexResourceInstance)
            : base(propertyName)
        {
            _complexResourceInstance = complexResourceInstance;
            _typeName = typeName;
        }

        public string TypeName
        {
            get { return _typeName; }
        }

        public ComplexResourceInstance ComplexResourceInstance
        {
            [DebuggerStepThrough]
            get { return _complexResourceInstance; }

            [DebuggerStepThrough]
            set { _complexResourceInstance = value; }
        }
    }

    public class ResourceInstanceSimpleProperty : ResourceInstanceProperty
    {
        public ResourceInstanceSimpleProperty(string propertyName, object propertyValue, bool createDollarValue)
            : base(propertyName)
        {
            PropertyValue = propertyValue;
            ClrType = (propertyValue == null) ? null : propertyValue.GetType();
            CreateDollarValue = createDollarValue;

            // TODO: make this random
            UseTickCountForJsonDateTime = false; //AstoriaTestProperties.Random.Next(2) == 0;
        }

        public ResourceInstanceSimpleProperty(string propertyName, NodeValue nodeValue, bool createDollarValue)
            : base(propertyName)
        {
            PropertyValue = nodeValue.ClrValue;
            ClrType = nodeValue.Type.ClrType;
            CreateDollarValue = createDollarValue;

            // TODO: make this random
            UseTickCountForJsonDateTime = false; //AstoriaTestProperties.Random.Next(2) == 0;
        }

        public ResourceInstanceSimpleProperty(string propertyName, object propertyValue)
            : this(propertyName, propertyValue, false)
        {
        }

        public ResourceInstanceSimpleProperty(NodeFacets facets, string propertyName, object propertyValue)
            : this(propertyName, propertyValue, false)
        {
            base.Facets.Add(facets);
        }


        public ResourceInstanceSimpleProperty(string propertyName, NodeValue nodeValue)
            : this(propertyName, nodeValue, false)
        {
        }

        public bool CreateDollarValue
        {
            get;
            set;
        }

        public object PropertyValue
        {
            get;
            set;
        }

        public Type ClrType
        {
            get;
            private set;
        }

        public bool UseTickCountForJsonDateTime
        {
            get;
            set;
        }
    }

    public abstract class ResourceInstanceNavProperty : ResourceInstanceProperty
    {
        public ResourceInstanceNavProperty(string propertyName)
            : base(propertyName)
        {
        }
    }
    public class ResourceInstanceNavRefProperty : ResourceInstanceNavProperty
    {
        private ResourceBodyTree _updateTreeNode;
        public ResourceInstanceNavRefProperty(string propertyName, ResourceBodyTree updateTreeNode)
            : base(propertyName)
        {
            _updateTreeNode = updateTreeNode;
        }

        public ResourceBodyTree TreeNode
        {
            [DebuggerStepThrough]
            get { return _updateTreeNode; }
        }
    }
    public class ResourceInstanceNavColProperty : ResourceInstanceNavProperty
    {
        private ResourceInstanceCollection _resourceInstanceCollection;
        public ResourceInstanceNavColProperty(string propertyName, params ResourceBodyTree[] updateTrees)
            : base(propertyName)
        {
            TestUtil.CheckArgumentElementsNotNull(updateTrees, "updateTrees");
            _resourceInstanceCollection = new ResourceInstanceCollection(updateTrees);
        }

        public ResourceInstanceCollection Collection
        {
            [DebuggerStepThrough]
            get { return _resourceInstanceCollection; }
        }
    }
    public class ResourceInstanceCollection : ResourceBodyTree
    {
        ResourceBodyTrees _updateTreeNode;
        public ResourceInstanceCollection(params ResourceBodyTree[] updateTree)
            : base("ResourceInstance")
        {
            TestUtil.CheckArgumentElementsNotNull(updateTree, "updateTree");
            _updateTreeNode = new ResourceBodyTrees(this, updateTree);
        }

        public ResourceBodyTrees NodeList
        {
            [DebuggerStepThrough]
            get { return _updateTreeNode; }
        }
    }

    public class ResourceBodyTrees : List<ResourceBodyTree>
    {
        private NamedNode _parent;

        public ResourceBodyTrees(NamedNode parent)
        {
            _parent = parent;
        }

        public ResourceBodyTrees(NamedNode parent, ResourceBodyTree[] updateTrees)
        {
            TestUtil.CheckArgumentElementsNotNull(updateTrees, "updateTrees");

            _parent = parent;
            this.AddRange(updateTrees);
        }
    }

    public class ResourceInstanceProperties : List<ResourceInstanceProperty>, ICloneable
    {
        public ResourceInstanceProperties()
            : base()
        { }

        public ResourceInstanceProperties(IEnumerable<ResourceInstanceProperty> properties)
            : base(properties)
        { }

        public ResourceInstanceProperties(params ResourceInstanceProperty[] properties)
            : this(properties.AsEnumerable())
        { }

        public object Clone()
        {
            return new ResourceInstanceProperties(this);
        }

        public ResourceInstanceProperty this[string name]
        {
            get
            {
                return this.FirstOrDefault(p => p.Name == name);
            }
            set
            {
                if (value.Name != name)
                {
                    AstoriaTestLog.FailAndThrow("Cannot add property with wrong name");
                }

                this.Add(value);
            }
        }

        public new void Add(ResourceInstanceProperty property)
        {
            ResourceInstanceProperty p = this[property.Name];
            if (p != null)
            {
                this.Remove(p);
            }

            base.Add(property);
        }
    }
}
