//---------------------------------------------------------------------
// <copyright file="JsonUpdatePayloadBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria.Util;

    public class JsonUpdatePayloadBuilder : UpdatePayloadBuilder
    {
        private string _resourceType;
        private Stack<string> _typeStack;

        public JsonUpdatePayloadBuilder(Workspace workspace, RequestVerb requestVerb)
            : base(workspace, requestVerb)
        {
            _typeStack = new Stack<string>(8);
        }

        #region TraceLog code
        #endregion
        public override string Build(ExpNode node)
        {
            return this.Visit(null, node);
        }


        protected virtual string Visit(ExpNode caller, ExpNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (node is KeyedResourceInstance && (!(node is AssociationResourceInstance)))
            {
                KeyedResourceInstance e = (KeyedResourceInstance)node;
                _resourceType = this.GetResourceType(e);

                _typeStack.Push(_resourceType);

                string properties = null;
                if (this.RequestVerb == RequestVerb.Post
                    && e.ResourceInstanceKey != null)
                {
                    foreach (ResourceInstanceProperty resourceProperty in e.ResourceInstanceKey.KeyProperties)
                    {
                        if (properties != null)
                            properties += "," + this.Visit(e, resourceProperty);
                        else
                            properties = this.Visit(e, resourceProperty);
                    }
                }
                foreach (ResourceInstanceProperty resourceProperty in e.Properties)
                {
                    if (properties != null)
                        properties += "," + this.Visit(e, resourceProperty);
                    else
                        properties = this.Visit(e, resourceProperty);
                }

                _resourceType = _typeStack.Pop();


                string typeAttribute = null;

                //if( Workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr )
                //    typeAttribute = String.Format("type:\"{0}\" ", e.ResourceTypeName); 
                //else
                    typeAttribute = String.Format("type:\"{0}.{1}\" ", this.Workspace.ContextNamespace, e.TypeName); 
                
                string uriAttribute = null;
                string metadataString = null;
                if (e.ResourceInstanceKey != null && this.RequestVerb != RequestVerb.Post)
                {
                    uriAttribute = String.Format("uri:\"{0}\"", CreateCanonicalUri(e.ResourceInstanceKey));
                }

                if (uriAttribute != null)
                {
                    if (e.IncludeTypeMetadataHint)
                    {
                        metadataString = String.Format("__metadata: {{ {0}, {1}}}", typeAttribute, uriAttribute);
                    }
                    else
                    {
                        metadataString = String.Format("__metadata: {{ {0} }}", uriAttribute);
                    }
                }
                else
                {
                    if (e.IncludeTypeMetadataHint)
                    {
                        metadataString = String.Format("__metadata: {{ {0} }}", typeAttribute);
                    }
                }

                //if (uriAttribute != null && e.IncludeTypeMetadataHint)
                //    metadataString = String.Format("__metadata: {{ {0}, {1}}}", typeAttribute, uriAttribute);
                //else if (e.IncludeTypeMetadataHint && uriAttribute == null)
                //    metadataString = String.Format("__metadata: {{ {0} }}", typeAttribute);

                string payload = null;
                if (properties == null)
                    payload = ApplyErrors(node, String.Format("{{{0}}}", metadataString));
                else
                    if (metadataString != null)
                    {
                        payload = ApplyErrors(node, String.Format("{{{0}, {1}}}", metadataString, properties));
                    }
                    else
                    {
                        payload = ApplyErrors(node, String.Format("{{{0}}}", properties));
                    }
                return payload;
            }
            else if (node is AssociationResourceInstance)
            {
                AssociationResourceInstance e = (AssociationResourceInstance)node;

                if (e.Operation == AssociationOperation.Add)
                {
                    string uri = CreateCanonicalUri(e.ResourceInstanceKey);
                    string payload=null;
                    if (caller == null)
                        payload = "{ uri:\"" + uri + "\"}";
                    else if (e.IncludeTypeInBind)
                        payload = String.Format("__metadata: {{ uri:\"{0}\", type:\"{1}.{2}\"}}", uri, this.Workspace.ContextNamespace, e.ResourceInstanceKey.ResourceTypeName);
                    else
                        payload = String.Format("__metadata: {{ uri:\"{0}\"}}", uri);
                    return ApplyErrors(node, payload);
                }
                else
                {
                    string navString = null;
                    if (caller != null)
                        navString = e.Name + ": null";
                    else
                        navString = "{ uri: null }";
                    return ApplyErrors(node, navString);
                }

            }
            /*
        else if (node is ResourceInstanceKey)
        {
            ResourceInstanceKey e = (ResourceInstanceKey)node;
            if (RequestVerb == RequestVerb.Post)
            {
                string payload = String.Format("__metadata: {{ type:\"{0}.{1}\" }}", this.Workspace.ContextNamespace, e.ResourceTypeName);
                if (e.KeyProperties != null)
                {
                    foreach (ResourceInstanceProperty resourceProperty in e.KeyProperties)
                    {
                        payload += String.Format(", {0}", this.Visit(e, resourceProperty));
                    }
                }
                return ApplyErrors(node, payload);
            }
            else if (RequestVerb == RequestVerb.Put)
            {
                string keyValues = WriteCommaDelimitedKeyValues(e);

                string payloadUri = String.Format("/{0}({1})", e.ResourceSetName, keyValues);
                return ApplyErrors(node, String.Format("__metadata: {{uri:\"{0}\" }}", payloadUri));
            }
            throw new ArgumentException("Request Verb is incorrect can't build an update payload:" + this.RequestVerb.ToString());
        }*/
            else if (node is ResourceInstanceSimpleProperty)
            {
                ResourceInstanceSimpleProperty e = (ResourceInstanceSimpleProperty)node;
                Type clrType = e.ClrType;
                object val = e.PropertyValue;

                if (val != null)
                    clrType = val.GetType();

                if (e.CreateDollarValue)
                {
                    if (val == null)
                        return null;
                    else if (clrType == typeof(byte[]))
                        return (new System.Text.UTF8Encoding()).GetString((byte[])val);
                    else
                        return AstoriaUnitTests.Data.TypeData.XmlValueFromObject(val);
                }

                string jsonStringValue;

                if (clrType == typeof(DateTime) && val is DateTime)
                {
                    if (e.UseTickCountForJsonDateTime)
                        jsonStringValue = "'" + JsonPrimitiveTypesUtil.GetJsonDateTimeStringValue((DateTime)val) + "'";
                    else
                        jsonStringValue = JsonPrimitiveTypesUtil.DateTimeToString(val);
                }
                else
                {
                    jsonStringValue = JsonPrimitiveTypesUtil.PrimitiveToString(val, clrType);

                    if (clrType == typeof(double) || clrType == typeof(float))
                    {
                        // PrimitiveToString will lose the trailing .0 if its a whole number
                        long temp;
                        if (long.TryParse(jsonStringValue, out temp))
                            jsonStringValue += ".0";
                    }
                }

                if (caller == null)
                {
                    return "{" + ApplyErrors(node, String.Format("{0}: {1}", e.Name, jsonStringValue)) + "}";
                }
                else
                {
                    return ApplyErrors(node, String.Format("{0}: {1}", e.Name, jsonStringValue));
                }
            }
            else if (node is ResourceInstanceComplexProperty)
            {
                ResourceInstanceComplexProperty e = (ResourceInstanceComplexProperty)node;
                string properties = null;

                if(e.IncludeTypeMetadataHint)
                    properties += String.Format("__metadata: {{ type:\"{0}.{1}\" }}", this.Workspace.ContextNamespace, e.TypeName);

                foreach (ResourceInstanceProperty resourceProperty in e.ComplexResourceInstance.Properties)
                {
                    if (string.IsNullOrEmpty(properties))
                        properties += this.Visit(e, resourceProperty);
                    else
                        properties += "," + this.Visit(e, resourceProperty);
                }
                string results =null;
                if (caller == null)
                    results = "{" + e.Name + ": {" + properties + "}" + "}";
                else
                    results = e.Name + ": {" + properties + "}";
                return ApplyErrors(node, results);
            }
            else if (node is ResourceInstanceNavRefProperty)
            {
                ResourceInstanceNavRefProperty e = (ResourceInstanceNavRefProperty)node;
                AssociationResourceInstance associationResourceInstance = e.TreeNode as AssociationResourceInstance;
                string navString = null;
                if ((associationResourceInstance != null && associationResourceInstance.Operation != AssociationOperation.Remove))
                {
                    navString = e.Name + ": {";
                    navString += this.Visit(e, e.TreeNode);
                    navString += "}";
                }
                else if (associationResourceInstance != null && associationResourceInstance.Operation == AssociationOperation.Remove)
                {
                    if (caller != null)
                        navString = e.Name + ": null";
                    else
                        navString = "null";
                }
                else
                {
                    navString = e.Name + ": ";
                    navString += this.Visit(e, e.TreeNode);
                }
                return ApplyErrors(node, navString);
            }
            else if (node is ResourceInstanceNavColProperty)
            {
                ResourceInstanceNavColProperty e = (ResourceInstanceNavColProperty)node;

                //string navString = String.Format("{0}: {", e.Name);
                string navString = e.Name + ": [";
                foreach (NamedNode namedNode in e.Collection.NodeList)
                {
                    if (!(namedNode is AssociationResourceInstance))
                        navString += this.Visit(e, namedNode) + ",";
                    else
                        navString += "{" + this.Visit(e, namedNode) + "},";
                }
                navString = navString.TrimEnd(',');
                navString += "]";

                return ApplyErrors(node, navString);
            }
            
            else
            {
                throw new Exception("Unknown node type: " + node.GetType());
            }
        }
        
        private static string ApplyErrors(Node node, string text)
        {
            return PayloadSyntaxError.ApplyErrorsToNodeText(node, text);
        }

        private string GetResourceType(KeyedResourceInstance e)
        {
            string resourceType = e.ResourceSetName;
            if (resourceType == null)
                resourceType = e.ResourceInstanceKey.ResourceSetName;

            return resourceType;
        }
    }
}

// "{ __metadata: { type: \"northwind.Region\" }, RegionId: 55, RegionDesctripyion: \"foo\" }"
