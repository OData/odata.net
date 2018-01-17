//---------------------------------------------------------------------
// <copyright file="Serializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Serializes the request data into the given format using the given message writer.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Class needs refactoring.")]
    public class Serializer
    {
        /// <summary>where to pull the changes from</summary>
        private readonly RequestInfo requestInfo;

        /// <summary>The property converter to use for creating ODataProperty instances.</summary>
        private readonly ODataPropertyConverter propertyConverter;

        /// <summary>The save changes option.</summary>
        private readonly SaveChangesOptions options;

        /// <summary>The option to send entity parameters.</summary>
        private readonly EntityParameterSendOption sendOption;

        /// <summary>
        /// Creates a new instance of the Serializer.
        /// </summary>
        /// <param name="requestInfo">the request info.</param>
        internal Serializer(RequestInfo requestInfo)
        {
            Debug.Assert(requestInfo != null, "requestInfo != null");
            this.requestInfo = requestInfo;
            this.propertyConverter = new ODataPropertyConverter(this.requestInfo);
        }

        /// <summary>
        /// Creates a new instance of the Serializer.
        /// </summary>
        /// <param name="requestInfo">the request info.</param>
        /// <param name="sendOption">The option to send entity operation parameters.</param>
        internal Serializer(RequestInfo requestInfo, EntityParameterSendOption sendOption)
            : this(requestInfo)
        {
            this.sendOption = sendOption;
        }

        /// <summary>
        /// Creates a new instance of the Serializer.
        /// </summary>
        /// <param name="requestInfo">the request info.</param>
        /// <param name="options">the save change options.</param>
        internal Serializer(RequestInfo requestInfo, SaveChangesOptions options)
            : this(requestInfo)
        {
            this.options = options;
        }

        /// <summary>
        /// Gets the string of keys used in URI.
        /// </summary>
        /// <param name="context">Wrapping context instance.</param>
        /// <param name="keys">The dictionary containing key pairs.</param>
        /// <returns>The string of keys.</returns>
        public static string GetKeyString(DataServiceContext context, Dictionary<string, object> keys)
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            if (keys.Count == 1)
            {
                return serializer.ConvertToEscapedUriValue(keys.First().Key, keys.First().Value);
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var keyPair in keys)
                {
                    stringBuilder.Append(keyPair.Key);
                    stringBuilder.Append(UriHelper.EQUALSSIGN);
                    stringBuilder.Append(serializer.ConvertToEscapedUriValue(keyPair.Key, keyPair.Value));
                    stringBuilder.Append(UriHelper.COMMA);
                }

                stringBuilder.Remove(stringBuilder.Length - 1, 1);
                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Gets the string of parameters used in URI.
        /// </summary>
        /// <param name="context">Wrapping context instance.</param>
        /// <param name="parameters">Parameters of function.</param>
        /// <returns>The string of parameters.</returns>
        public static string GetParameterString(DataServiceContext context, params OperationParameter[] parameters)
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(UriHelper.LEFTPAREN);
            foreach (var key in parameters)
            {
                stringBuilder.Append(key.Name);
                stringBuilder.Append(UriHelper.EQUALSSIGN);
                stringBuilder.Append(serializer.ConvertToEscapedUriValue(key.Name, key.Value));
                stringBuilder.Append(UriHelper.COMMA);
            }

            if (parameters.Any())
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }

            stringBuilder.Append(UriHelper.RIGHTPAREN);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Seriliaze the parameter value to string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Parameter value string.</returns>
        internal static string GetParameterValue(DataServiceContext context, OperationParameter parameter)
        {
            var requestInfo = new RequestInfo(context);
            var serializer = new Serializer(requestInfo);
            UriEntityOperationParameter entityParameter = parameter as UriEntityOperationParameter;
            return serializer.ConvertToEscapedUriValue(parameter.Name, parameter.Value, entityParameter != null && entityParameter.UseEntityReference);
        }

        /// <summary>
        /// Creates an instance of ODataMessageWriter.
        /// </summary>
        /// <param name="requestMessage">Instance of IODataRequestMessage.</param>
        /// <param name="requestInfo">RequestInfo containing information about the client settings.</param>
        /// <param name="isParameterPayload">true if the writer is intended to for a parameter payload, false otherwise.</param>
        /// <returns>An instance of ODataMessageWriter.</returns>
        internal static ODataMessageWriter CreateMessageWriter(ODataRequestMessageWrapper requestMessage, RequestInfo requestInfo, bool isParameterPayload)
        {
            var writerSettings = requestInfo.WriteHelper.CreateSettings(requestMessage.IsBatchPartRequest, requestInfo.Context.EnableWritingODataAnnotationWithoutPrefix);
            return requestMessage.CreateWriter(writerSettings, isParameterPayload);
        }

        /// <summary>
        /// Creates an ODataResource for the given EntityDescriptor and fills in its ODataLib metadata.
        /// </summary>
        /// <param name="entityDescriptor">The entity descriptor.</param>
        /// <param name="serverTypeName">Name of the server type.</param>
        /// <param name="entityType">The client-side entity type.</param>
        /// <param name="clientFormat">The current client format.</param>
        /// <returns>An odata entry with its metadata filled in.</returns>
        internal static ODataResource CreateODataEntry(EntityDescriptor entityDescriptor, string serverTypeName, ClientTypeAnnotation entityType, DataServiceClientFormat clientFormat)
        {
            ODataResource entry = new ODataResource();

            // If the client type name is different from the server type name, then add SerializationTypeNameAnnotation
            // which tells ODataLib to write the type name in the annotation in the payload.
            if (entityType.ElementTypeName != serverTypeName)
            {
                entry.TypeAnnotation = new ODataTypeAnnotation(serverTypeName);
            }

            // We always need to write the client type name, since this is the type name used by ODataLib
            // to resolve the entity type using EdmModel.FindSchemaElement.
            entry.TypeName = entityType.ElementTypeName;

            if (entityDescriptor.IsMediaLinkEntry || entityType.IsMediaLinkEntry)
            {
                // Since we are already enabled EnableWcfDataServicesClientBehavior in the writer settings,
                // setting the MediaResource value will tell ODataLib to write MLE payload, irrespective of
                // what the metadata says.
                entry.MediaResource = new ODataStreamReferenceValue();
            }

            return entry;
        }

        /// <summary>
        /// Writes the body operation parameters associated with a ServiceAction. For each BodyOperationParameter:
        /// 1. calls ODataPropertyConverter  to convert CLR object into ODataValue/primitive values.
        /// 2. then calls ODataParameterWriter to write the ODataValue/primitive values.
        /// </summary>
        /// <param name="operationParameters">The list of operation parameters to write.</param>
        /// <param name="requestMessage">The OData request message used to write the operation parameters.</param>
        internal void WriteBodyOperationParameters(List<BodyOperationParameter> operationParameters, ODataRequestMessageWrapper requestMessage)
        {
            Debug.Assert(requestMessage != null, "requestMessage != null");
            Debug.Assert(operationParameters != null, "operationParameters != null");
            Debug.Assert(operationParameters.Any(), "operationParameters.Any()");

            using (ODataMessageWriter messageWriter = Serializer.CreateMessageWriter(requestMessage, this.requestInfo, true /*isParameterPayload*/))
            {
                ODataParameterWriter parameterWriter = messageWriter.CreateODataParameterWriter(null);
                parameterWriter.WriteStart();

                foreach (BodyOperationParameter operationParameter in operationParameters)
                {
                    if (operationParameter.Value == null)
                    {
                        parameterWriter.WriteValue(operationParameter.Name, operationParameter.Value);
                    }
                    else
                    {
                        ClientEdmModel model = this.requestInfo.Model;
                        IEdmType edmType = model.GetOrCreateEdmType(operationParameter.Value.GetType());
                        Debug.Assert(edmType != null, "edmType != null");

                        switch (edmType.TypeKind)
                        {
                            case EdmTypeKind.Collection:
                                {
                                    this.WriteCollectionValueInBodyOperationParameter(parameterWriter, operationParameter, (IEdmCollectionType)edmType);
                                    break;
                                }

                            case EdmTypeKind.Complex:
                            case EdmTypeKind.Entity:
                                {
                                    Debug.Assert(model.GetClientTypeAnnotation(edmType).ElementType != null, "model.GetClientTypeAnnotation(edmType).ElementType != null");
                                    ODataResourceWrapper entry = this.CreateODataResourceFromEntityOperationParameter(model.GetClientTypeAnnotation(edmType), operationParameter.Value);
                                    Debug.Assert(entry != null, "entry != null");
                                    var entryWriter = parameterWriter.CreateResourceWriter(operationParameter.Name);
                                    ODataWriterHelper.WriteResource(entryWriter, entry);
                                    break;
                                }

                            case EdmTypeKind.Primitive:
                                object primitiveValue = ODataPropertyConverter.ConvertPrimitiveValueToRecognizedODataType(operationParameter.Value, operationParameter.Value.GetType());
                                parameterWriter.WriteValue(operationParameter.Name, primitiveValue);
                                break;

                            case EdmTypeKind.Enum:
                                ODataEnumValue tmp = this.propertyConverter.CreateODataEnumValue(
                                    model.GetClientTypeAnnotation(edmType).ElementType,
                                    operationParameter.Value,
                                    false);
                                parameterWriter.WriteValue(operationParameter.Name, tmp);

                                break;
                            default:
                                // EdmTypeKind.Row
                                // EdmTypeKind.EntityReference
                                throw new NotSupportedException(Strings.Serializer_InvalidParameterType(operationParameter.Name, edmType.TypeKind));
                        }
                    } // else
                } // foreach

                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            }
        }

        /// <summary>
        /// Write the entry element.
        /// </summary>
        /// <param name="entityDescriptor">The entity.</param>
        /// <param name="relatedLinks">Collection of links related to the entity.</param>
        /// <param name="requestMessage">The OData request message.</param>
        internal void WriteEntry(EntityDescriptor entityDescriptor, IEnumerable<LinkDescriptor> relatedLinks, ODataRequestMessageWrapper requestMessage)
        {
            ClientEdmModel model = this.requestInfo.Model;
            ClientTypeAnnotation entityType = model.GetClientTypeAnnotation(model.GetOrCreateEdmType(entityDescriptor.Entity.GetType()));
            using (ODataMessageWriter messageWriter = Serializer.CreateMessageWriter(requestMessage, this.requestInfo, false /*isParameterPayload*/))
            {
                ODataWriterWrapper entryWriter = ODataWriterWrapper.CreateForEntry(messageWriter, this.requestInfo.Configurations.RequestPipeline);

                // Get the server type name using the type resolver or from the entity descriptor
                string serverTypeName = this.requestInfo.GetServerTypeName(entityDescriptor);

                var entry = CreateODataEntry(entityDescriptor, serverTypeName, entityType, this.requestInfo.Format);
                if (serverTypeName == null)
                {
                    serverTypeName = this.requestInfo.InferServerTypeNameFromServerModel(entityDescriptor);
                }

                IEnumerable<ClientPropertyAnnotation> properties;
                if ((!Util.IsFlagSet(this.options, SaveChangesOptions.ReplaceOnUpdate) &&
                    entityDescriptor.State == EntityStates.Modified &&
                    entityDescriptor.PropertiesToSerialize.Any()) ||
                    (Util.IsFlagSet(this.options, SaveChangesOptions.PostOnlySetProperties) &&
                    entityDescriptor.State == EntityStates.Added))
                {
                    properties = entityType.PropertiesToSerialize().Where(prop => entityDescriptor.PropertiesToSerialize.Contains(prop.PropertyName));
                }
                else
                {
                    properties = entityType.PropertiesToSerialize();
                }

                entry.Properties = this.propertyConverter.PopulateProperties(entityDescriptor.Entity, serverTypeName, properties);

                entryWriter.WriteStart(entry, entityDescriptor.Entity);

                this.WriteNestedComplexProperties(entityDescriptor.Entity, serverTypeName, properties, entryWriter);

                if (EntityStates.Added == entityDescriptor.State)
                {
                    this.WriteNestedResourceInfo(entityDescriptor, relatedLinks, entryWriter);
                }

                entryWriter.WriteEnd(entry, entityDescriptor.Entity);
            }
        }

        /// <summary>
        /// Write the instances for the given set of OData nested resource.
        /// </summary>
        /// <param name="entity">Instance of the resource which is getting serialized.</param>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="properties">The properties to write.</param>
        /// <param name="odataWriter">The writer used write the properties.</param>
        internal void WriteNestedComplexProperties(object entity, string serverTypeName, IEnumerable<ClientPropertyAnnotation> properties, ODataWriterWrapper odataWriter)
        {
            Debug.Assert(properties != null, "properties != null");
            var populatedProperties = properties.Where(p => p.IsComplex || p.IsComplexCollection);

            var nestedComplexProperties = this.propertyConverter.PopulateNestedComplexProperties(entity, serverTypeName, populatedProperties, null);
            foreach (var property in nestedComplexProperties)
            {
                WriteNestedResourceInfo(odataWriter, property);
            }
        }

        /// <summary>
        /// Writes a navigation link.
        /// </summary>
        /// <param name="entityDescriptor">The entity</param>
        /// <param name="relatedLinks">The links related to the entity</param>
        /// <param name="odataWriter">The ODataWriter used to write the navigation link.</param>
        internal void WriteNestedResourceInfo(EntityDescriptor entityDescriptor, IEnumerable<LinkDescriptor> relatedLinks, ODataWriterWrapper odataWriter)
        {
            // TODO: create instance of odatawriter.
            // TODO: send clientType once, so that we dont need entity descriptor
            Debug.Assert(EntityStates.Added == entityDescriptor.State, "entity not added state");

            Dictionary<string, List<LinkDescriptor>> groupRelatedLinks = new Dictionary<string, List<LinkDescriptor>>(EqualityComparer<string>.Default);
            foreach (LinkDescriptor end in relatedLinks)
            {
                List<LinkDescriptor> linkDescriptorsList = null;
                if (!groupRelatedLinks.TryGetValue(end.SourceProperty, out linkDescriptorsList))
                {
                    linkDescriptorsList = new List<LinkDescriptor>();
                    groupRelatedLinks.Add(end.SourceProperty, linkDescriptorsList);
                }

                linkDescriptorsList.Add(end);
            }

            ClientTypeAnnotation clientType = null;
            foreach (var grlinks in groupRelatedLinks)
            {
                if (null == clientType)
                {
                    ClientEdmModel model = this.requestInfo.Model;
                    clientType = model.GetClientTypeAnnotation(model.GetOrCreateEdmType(entityDescriptor.Entity.GetType()));
                }

                bool isCollection = clientType.GetProperty(grlinks.Key, UndeclaredPropertyBehavior.ThrowException).IsEntityCollection;
                bool started = false;

                foreach (LinkDescriptor end in grlinks.Value)
                {
                    Debug.Assert(!end.ContentGeneratedForSave, "already saved link");
                    end.ContentGeneratedForSave = true;
                    Debug.Assert(null != end.Target, "null is DELETE");

                    ODataNestedResourceInfo navigationLink = new ODataNestedResourceInfo();
                    navigationLink.Url = this.requestInfo.EntityTracker.GetEntityDescriptor(end.Target).GetLatestEditLink();
                    Debug.Assert(Uri.IsWellFormedUriString(UriUtil.UriToString(navigationLink.Url), UriKind.Absolute), "Uri.IsWellFormedUriString(targetEditLink, UriKind.Absolute)");

                    navigationLink.IsCollection = isCollection;
                    navigationLink.Name = grlinks.Key;

                    if (!started)
                    {
                        odataWriter.WriteNestedResourceInfoStart(navigationLink);
                        started = true;
                    }

                    odataWriter.WriteNestedResourceInfoStart(navigationLink, end.Source, end.Target);
                    odataWriter.WriteEntityReferenceLink(new ODataEntityReferenceLink() { Url = navigationLink.Url }, end.Source, end.Target);
                    odataWriter.WriteNestedResourceInfoEnd(navigationLink, end.Source, end.Target);
                }

                odataWriter.WriteNestedResourceInfoEnd();
            }
        }

#if DEBUG
        /// <summary>
        /// Writes an entity reference link.
        /// </summary>
        /// <param name="binding">The link descriptor.</param>
        /// <param name="requestMessage">The request message used for writing the payload.</param>
        /// <param name="isBatch">True if batch, false otherwise.</param>
        internal void WriteEntityReferenceLink(LinkDescriptor binding, ODataRequestMessageWrapper requestMessage, bool isBatch)
#else
        /// <summary>
        /// Writes an entity reference link.
        /// </summary>
        /// <param name="binding">The link descriptor.</param>
        /// <param name="requestMessage">The request message used for writing the payload.</param>
        internal void WriteEntityReferenceLink(LinkDescriptor binding, ODataRequestMessageWrapper requestMessage)
#endif
        {
            using (ODataMessageWriter messageWriter = Serializer.CreateMessageWriter(requestMessage, this.requestInfo, false /*isParameterPayload*/))
            {
                EntityDescriptor targetResource = this.requestInfo.EntityTracker.GetEntityDescriptor(binding.Target);

                Uri targetReferenceLink = targetResource.GetLatestIdentity();

                if (targetReferenceLink == null)
                {
#if DEBUG
                    Debug.Assert(isBatch, "we should be cross-referencing entities only in batch scenarios");
#endif
                    targetReferenceLink = UriUtil.CreateUri("$" + targetResource.ChangeOrder.ToString(CultureInfo.InvariantCulture), UriKind.Relative);
                }

                ODataEntityReferenceLink referenceLink = new ODataEntityReferenceLink();
                referenceLink.Url = targetReferenceLink;
                messageWriter.WriteEntityReferenceLink(referenceLink);
            }
        }

        /// <summary>
        /// Enumerates through the list of URI operation parameters and creates a new Uri with the uri operation parameters written as query string of the new Uri.
        /// </summary>
        /// <param name="requestUri">The Uri used to construct the new Uri.</param>
        /// <param name="operationParameters">The non-empty list of uri parameters which will be converted to query string.</param>
        /// <returns>Uri containing the uri parameters as query string.</returns>
        internal Uri WriteUriOperationParametersToUri(Uri requestUri, List<UriOperationParameter> operationParameters)
        {
            Debug.Assert(operationParameters != null && operationParameters.Any(), "OperationParameters was null or empty");
            Debug.Assert(requestUri != null, "request_uri != null");

            UriBuilder uriBuilder = new UriBuilder(requestUri);
            StringBuilder pathBuilder = new StringBuilder();
            pathBuilder.Append(uriBuilder.Path);
            string lastSeg = uriBuilder.Path.Substring(uriBuilder.Path.LastIndexOf('/') + 1);

            StringBuilder queryBuilder = new StringBuilder();
            String uriString = UriUtil.UriToString(uriBuilder.Uri);

            if (!string.IsNullOrEmpty(uriBuilder.Query))
            {
                Debug.Assert(uriBuilder.Query[0] == UriHelper.QUESTIONMARK, "uriBuilder.Query[0] == UriHelper.QUESTIONMARK");

                // Don't append the '?', as later when we call setter on the Query, the '?' will be automatically added.
                queryBuilder.Append(uriBuilder.Query.Substring(1));
                queryBuilder.Append(UriHelper.AMPERSAND);
            }

            if (!lastSeg.Contains(Char.ToString(UriHelper.ATSIGN)))
            {
                pathBuilder.Append(UriHelper.LEFTPAREN);
            }
            else
            {
                if (pathBuilder.ToString().EndsWith(Char.ToString(UriHelper.RIGHTPAREN), StringComparison.OrdinalIgnoreCase))
                {
                    pathBuilder.Remove(pathBuilder.Length - 1, 1);
                    pathBuilder.Append(UriHelper.COMMA);
                }
            }

            foreach (UriOperationParameter op in operationParameters)
            {
                Debug.Assert(op != null, "op != null");
                Debug.Assert(!string.IsNullOrEmpty(op.Name), "!string.IsNullOrEmpty(op.ParameterName)");

                string paramName = op.Name.Trim();

                // if the parameter name is an alias, make sure that the URI contains it.
                if (paramName.StartsWith(Char.ToString(UriHelper.ATSIGN), StringComparison.OrdinalIgnoreCase) && !uriString.Contains(paramName))
                {
                    throw new DataServiceRequestException(Strings.Serializer_UriDoesNotContainParameterAlias(op.Name));
                }

                if (paramName.StartsWith(Char.ToString(UriHelper.ATSIGN), StringComparison.OrdinalIgnoreCase))
                {
                    // name=value&
                    queryBuilder.Append(paramName);
                    queryBuilder.Append(UriHelper.EQUALSSIGN);
                    queryBuilder.Append(this.ConvertToEscapedUriValue(paramName, op.Value));
                    queryBuilder.Append(UriHelper.AMPERSAND);
                }

                string value = this.ConvertToEscapedUriValue(paramName, op.Value);

                // non-primitive value, use alias.
                if (!UriHelper.IsPrimitiveValue(value))
                {
                    // name = @name
                    pathBuilder.Append(paramName);
                    pathBuilder.Append(UriHelper.EQUALSSIGN);
                    pathBuilder.Append(UriHelper.ENCODEDATSIGN);
                    pathBuilder.Append(paramName);
                    pathBuilder.Append(UriHelper.COMMA);

                    // @name = value&
                    queryBuilder.Append(UriHelper.ENCODEDATSIGN);
                    queryBuilder.Append(paramName);
                    queryBuilder.Append(UriHelper.EQUALSSIGN);
                    queryBuilder.Append(value);
                    queryBuilder.Append(UriHelper.AMPERSAND);
                }
                else
                {
                    // primitive value, do not use alias.
                    pathBuilder.Append(paramName);
                    pathBuilder.Append(UriHelper.EQUALSSIGN);
                    pathBuilder.Append(value);
                    pathBuilder.Append(UriHelper.COMMA);
                }
            }

            // remove the last extra comma.
            if (pathBuilder.ToString().EndsWith(Char.ToString(UriHelper.COMMA), StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(pathBuilder.ToString().EndsWith(Char.ToString(UriHelper.COMMA), StringComparison.OrdinalIgnoreCase), "Uri was expected to end with an ampersand.");
                pathBuilder.Remove(pathBuilder.Length - 1, 1);
            }

            pathBuilder.Append(UriHelper.RIGHTPAREN);

            // remove the last extra ampersand.
            if (queryBuilder.ToString().EndsWith(Char.ToString(UriHelper.AMPERSAND), StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(queryBuilder.ToString().EndsWith(Char.ToString(UriHelper.AMPERSAND), StringComparison.OrdinalIgnoreCase), "Uri was expected to end with an ampersand.");
                queryBuilder.Remove(queryBuilder.Length - 1, 1);
            }

            uriBuilder.Path = pathBuilder.ToString();
            uriBuilder.Query = queryBuilder.ToString();

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Writes collection value in body operation parameter.
        /// </summary>
        /// <param name="parameterWriter">The odata parameter writer.</param>
        /// <param name="operationParameter">The operation parameter.</param>
        /// <param name="edmCollectionType">The edm collection type.</param>
        private void WriteCollectionValueInBodyOperationParameter(ODataParameterWriter parameterWriter, BodyOperationParameter operationParameter, IEdmCollectionType edmCollectionType)
        {
            ClientEdmModel model = this.requestInfo.Model;

            var elementTypeKind = edmCollectionType.ElementType.TypeKind();

            if (elementTypeKind == EdmTypeKind.Entity || elementTypeKind == EdmTypeKind.Complex)
            {
                ODataWriter feedWriter = parameterWriter.CreateResourceSetWriter(operationParameter.Name);
                feedWriter.WriteStart(new ODataResourceSet());

                IEnumerator enumerator = ((ICollection)operationParameter.Value).GetEnumerator();

                while (enumerator.MoveNext())
                {
                    Object collectionItem = enumerator.Current;
                    if (collectionItem == null)
                    {
                        if (elementTypeKind == EdmTypeKind.Complex)
                        {
                            feedWriter.WriteStart((ODataResource)null);
                            feedWriter.WriteEnd();
                            continue;
                        }
                        else
                        {
                            throw new NotSupportedException(Strings.Serializer_NullCollectionParamterItemValue(operationParameter.Name));
                        }
                    }

                    IEdmType edmItemType = model.GetOrCreateEdmType(collectionItem.GetType());
                    Debug.Assert(edmItemType != null, "edmItemType != null");

                    if (edmItemType.TypeKind != EdmTypeKind.Entity && edmItemType.TypeKind != EdmTypeKind.Complex)
                    {
                        throw new NotSupportedException(Strings.Serializer_InvalidCollectionParamterItemType(operationParameter.Name, edmItemType.TypeKind));
                    }

                    Debug.Assert(model.GetClientTypeAnnotation(edmItemType).ElementType != null, "edmItemType.GetClientTypeAnnotation().ElementType != null");
                    ODataResourceWrapper entry = this.CreateODataResourceFromEntityOperationParameter(model.GetClientTypeAnnotation(edmItemType), collectionItem);
                    Debug.Assert(entry != null, "entry != null");
                    ODataWriterHelper.WriteResource(feedWriter, entry);
                }

                feedWriter.WriteEnd();
                feedWriter.Flush();
            }
            else
            {
                ODataCollectionWriter collectionWriter = parameterWriter.CreateCollectionWriter(operationParameter.Name);
                ODataCollectionStart odataCollectionStart = new ODataCollectionStart();
                collectionWriter.WriteStart(odataCollectionStart);

                IEnumerator enumerator = ((ICollection)operationParameter.Value).GetEnumerator();

                while (enumerator.MoveNext())
                {
                    Object collectionItem = enumerator.Current;
                    if (collectionItem == null)
                    {
                        collectionWriter.WriteItem(null);
                        continue;
                    }

                    IEdmType edmItemType = model.GetOrCreateEdmType(collectionItem.GetType());
                    Debug.Assert(edmItemType != null, "edmItemType != null");

                    switch (edmItemType.TypeKind)
                    {
                        case EdmTypeKind.Primitive:
                            {
                                object primitiveItemValue = ODataPropertyConverter.ConvertPrimitiveValueToRecognizedODataType(collectionItem, collectionItem.GetType());
                                collectionWriter.WriteItem(primitiveItemValue);
                                break;
                            }

                        case EdmTypeKind.Enum:
                            {
                                ODataEnumValue enumTmp = this.propertyConverter.CreateODataEnumValue(model.GetClientTypeAnnotation(edmItemType).ElementType, collectionItem, false);
                                collectionWriter.WriteItem(enumTmp);
                                break;
                            }

                        default:
                            // EdmTypeKind.Entity
                            // EdmTypeKind.Row
                            // EdmTypeKind.EntityReference
                            throw new NotSupportedException(Strings.Serializer_InvalidCollectionParamterItemType(operationParameter.Name, edmItemType.TypeKind));
                    }
                }

                collectionWriter.WriteEnd();
                collectionWriter.Flush();
            }
        }

        private static void WriteResourceSet(ODataWriterWrapper writer, ODataResourceSetWrapper resourceSetWrapper)
        {
            writer.WriteStart(resourceSetWrapper.ResourceSet);

            if (resourceSetWrapper.Resources != null)
            {
                foreach (var resourceWrapper in resourceSetWrapper.Resources)
                {
                    WriteResource(writer, resourceWrapper);
                }
            }

            writer.WriteEnd();
        }

        private static void WriteResource(ODataWriterWrapper writer, ODataResourceWrapper resourceWrapper)
        {
            if (resourceWrapper.Resource == null)
            {
                writer.WriteStartResource(resourceWrapper.Resource);
            }
            else
            {
                writer.WriteStart(resourceWrapper.Resource, resourceWrapper.Instance);
            }

            if (resourceWrapper.NestedResourceInfoWrappers != null)
            {
                foreach (var nestedResourceInfoWrapper in resourceWrapper.NestedResourceInfoWrappers)
                {
                    WriteNestedResourceInfo(writer, nestedResourceInfoWrapper);
                }
            }

            if (resourceWrapper.Resource == null)
            {
                writer.WriteEnd();
            }
            else
            {
                writer.WriteEnd(resourceWrapper.Resource, resourceWrapper.Instance);
            }
        }

        private static void WriteNestedResourceInfo(ODataWriterWrapper writer, ODataNestedResourceInfoWrapper nestedResourceInfo)
        {
            writer.WriteNestedResourceInfoStart(nestedResourceInfo.NestedResourceInfo);

            if (nestedResourceInfo.NestedResourceOrResourceSet != null)
            {
                WriteItem(writer, nestedResourceInfo.NestedResourceOrResourceSet);
            }

            writer.WriteNestedResourceInfoEnd();
        }

        private static void WriteItem(ODataWriterWrapper writer, ODataItemWrapper odataItemWrapper)
        {
            var odataResourceWrapper = odataItemWrapper as ODataResourceWrapper;
            if (odataResourceWrapper != null)
            {
                WriteResource(writer, odataResourceWrapper);
                return;
            }

            var odataResourceSetWrapper = odataItemWrapper as ODataResourceSetWrapper;
            if (odataResourceSetWrapper != null)
            {
                WriteResourceSet(writer, odataResourceSetWrapper);
            }
        }

        /// <summary>
        /// Converts a <see cref="UriOperationParameter"/> value to an escaped string for use in a Uri. Wraps the call to ODL's ConvertToUriLiteral and escapes the results.
        /// </summary>
        /// <param name="paramName">The name of the <see cref="UriOperationParameter"/>. Used for error reporting.</param>
        /// <param name="value">The value of the <see cref="UriOperationParameter"/>.</param>
        /// <param name="useEntityReference">If true, use entity reference, instead of entity to serialize the parameter.</param>
        /// <returns>A string representation of <paramref name="value"/> for use in a Url.</returns>
        private string ConvertToEscapedUriValue(string paramName, object value, bool useEntityReference = false)
        {
            Debug.Assert(!string.IsNullOrEmpty(paramName), "!string.IsNullOrEmpty(paramName)");

            // Literal values with single quotes need special escaping due to System.Uri changes in behavior between .NET 4.0 and 4.5.
            // We need to ensure that our escaped values do not change between those versions, so we need to escape values differently when they could contain single quotes.
            bool needsSpecialEscaping = false;
            object valueInODataFormat = ConvertToODataValue(paramName, value, ref needsSpecialEscaping, useEntityReference);

            // When calling Execute() to invoke an Action, the client doesn't support parsing the target url
            // to determine which IEdmOperationImport to pass to the ODL writer. So the ODL writer is
            // serializing the parameter payload without metadata. Setting the model to null so ODL doesn't
            // do unecessary validations when writing without metadata.
            string literal = ODataUriUtils.ConvertToUriLiteral(valueInODataFormat, CommonUtil.ConvertToODataVersion(this.requestInfo.MaxProtocolVersionAsVersion), null /* edmModel */);

            // The value from ConvertToUriValue will not be escaped, but will already contain literal delimiters like single quotes, so we
            // need to use our own escape method that will preserve those characters instead of directly calling Uri.EscapeDataString that may escape them.
            // This is only necessary for primitives and nulls because the other structures are serialized using the JSON format and it uses double quotes
            // which have always been escaped.
            if (needsSpecialEscaping)
            {
                return DataStringEscapeBuilder.EscapeDataString(literal);
            }

            return Uri.EscapeDataString(literal);
        }

        /// <summary>
        /// Converts the object to ODataValue, the result could be null, the original primitive object, ODataNullValue,
        /// ODataEnumValue, ODataCollectionValue, ODataResource, ODataEntityReferenceLinks, ODataEntityReferenceLinks, or
        /// a list of ODataResource.
        /// </summary>
        /// <param name="paramName">The name of the <see cref="UriOperationParameter"/>. Used for error reporting.</param>
        /// <param name="value">The value of the <see cref="UriOperationParameter"/>.</param>
        /// <param name="needsSpecialEscaping">True if the result need special escaping.</param>
        /// <param name="useEntityReference">If true, use entity reference, instead of entity to serialize the parameter.</param>
        /// <returns>The converted result.</returns>
        private object ConvertToODataValue(string paramName, object value, ref bool needsSpecialEscaping, bool useEntityReference)
        {
            Object valueInODataFormat = null;

            if (value == null)
            {
                needsSpecialEscaping = true;
            }
            else if (value is ODataNullValue)
            {
                valueInODataFormat = value;
                needsSpecialEscaping = true;
            }
            else
            {
                ClientEdmModel model = this.requestInfo.Model;
                IEdmType edmType = model.GetOrCreateEdmType(value.GetType());
                Debug.Assert(edmType != null, "edmType != null");
                ClientTypeAnnotation typeAnnotation = model.GetClientTypeAnnotation(edmType);
                Debug.Assert(typeAnnotation != null, "typeAnnotation != null");
                switch (edmType.TypeKind)
                {
                    case EdmTypeKind.Primitive:
                        valueInODataFormat = value;
                        needsSpecialEscaping = true;
                        break;

                    case EdmTypeKind.Enum:
                        string typeNameInEdm = this.requestInfo.GetServerTypeName(model.GetClientTypeAnnotation(edmType));
                        valueInODataFormat =
                            new ODataEnumValue(
                                ClientTypeUtil.GetEnumValuesString(value.ToString(), typeAnnotation.ElementType),
                                typeNameInEdm ?? typeAnnotation.ElementTypeName);
                        needsSpecialEscaping = true;

                        break;

                    case EdmTypeKind.Collection:
                        IEdmCollectionType edmCollectionType = edmType as IEdmCollectionType;
                        Debug.Assert(edmCollectionType != null, "edmCollectionType != null");
                        IEdmTypeReference itemTypeReference = edmCollectionType.ElementType;
                        Debug.Assert(itemTypeReference != null, "itemTypeReference != null");
                        ClientTypeAnnotation itemTypeAnnotation =
                            model.GetClientTypeAnnotation(itemTypeReference.Definition);
                        Debug.Assert(itemTypeAnnotation != null, "itemTypeAnnotation != null");

                        valueInODataFormat = ConvertToCollectionValue(paramName, value, itemTypeAnnotation, useEntityReference);
                        break;

                    case EdmTypeKind.Complex:
                    case EdmTypeKind.Entity:
                        Debug.Assert(edmType.TypeKind == EdmTypeKind.Complex || value != null, "edmType.TypeKind == EdmTypeKind.Complex || value != null");
                        Debug.Assert(typeAnnotation != null, "typeAnnotation != null");
                        valueInODataFormat = ConvertToEntityValue(value, typeAnnotation.ElementType, useEntityReference);
                        break;

                    default:
                        // EdmTypeKind.Row
                        // EdmTypeKind.EntityReference
                        throw new NotSupportedException(Strings.Serializer_InvalidParameterType(paramName, edmType.TypeKind));
                }

                Debug.Assert(valueInODataFormat != null, "valueInODataFormat != null");
            }

            return valueInODataFormat;
        }

        /// <summary>
        /// Converts the object to ODataCollectionValue, ODataEntityReferenceLinks, or
        /// a list of ODataResource.
        /// </summary>
        /// <param name="paramName">The name of the <see cref="UriOperationParameter"/>. Used for error reporting.</param>
        /// <param name="value">The value of the <see cref="UriOperationParameter"/>.</param>
        /// <param name="itemTypeAnnotation">The client type annotation of the value.</param>
        /// <param name="useEntityReference">If true, use entity reference, instead of entity to serialize the parameter.</param>
        /// <returns>The converted result.</returns>
        private object ConvertToCollectionValue(string paramName, object value, ClientTypeAnnotation itemTypeAnnotation, bool useEntityReference)
        {
            object valueInODataFormat;

            switch (itemTypeAnnotation.EdmType.TypeKind)
            {
                case EdmTypeKind.Primitive:
                case EdmTypeKind.Enum:
                    valueInODataFormat = this.propertyConverter.CreateODataCollection(itemTypeAnnotation.ElementType, null, value, null, false, false);
                    break;
                case EdmTypeKind.Complex:
                case EdmTypeKind.Entity:
                    if (useEntityReference)
                    {
                        var list = value as IEnumerable;
                        var links = (from object o in list
                                     select new ODataEntityReferenceLink()
                                     {
                                         Url = this.requestInfo.EntityTracker.GetEntityDescriptor(o).GetLatestIdentity(),
                                     }).ToList();

                        valueInODataFormat = new ODataEntityReferenceLinks()
                        {
                            Links = links,
                        };
                    }
                    else
                    {
                        valueInODataFormat = this.propertyConverter.CreateODataEntries(
                            itemTypeAnnotation.ElementType, value);
                    }

                    break;

                default:
                    throw new NotSupportedException(Strings.Serializer_InvalidCollectionParamterItemType(paramName, itemTypeAnnotation.EdmType.TypeKind));
            }

            return valueInODataFormat;
        }

        /// <summary>
        /// Converts the object to ODataResource or ODataEntityReferenceLink.
        /// </summary>
        /// <param name="value">The value of the <see cref="UriOperationParameter"/>.</param>
        /// <param name="elementType">The type of the value</param>
        /// <param name="useEntityReference">If true, use entity reference, instead of entity to serialize the parameter.</param>
        /// <returns>The converted result.</returns>
        private object ConvertToEntityValue(object value, Type elementType, bool useEntityReference)
        {
            object valueInODataFormat;

            if (!useEntityReference)
            {
                valueInODataFormat = this.propertyConverter.CreateODataEntry(elementType, value);

                ODataResource entry = (ODataResource)valueInODataFormat;
                if (entry.TypeAnnotation == null ||
                    string.IsNullOrEmpty(entry.TypeAnnotation.TypeName))
                {
                    throw Error.InvalidOperation(Strings.DataServiceException_GeneralError);
                }
            }
            else
            {
                EntityDescriptor resource = this.requestInfo.EntityTracker.GetEntityDescriptor(value);
                Uri link = resource.GetLatestIdentity();
                valueInODataFormat = new ODataEntityReferenceLink()
                {
                    Url = link,
                };
            }

            return valueInODataFormat;
        }

        /// <summary>
        /// Creates an ODataResource using some properties extracted from an entity operation parameter.
        /// </summary>
        /// <param name="clientTypeAnnotation">The client type annotation of the entity.</param>
        /// <param name="parameterValue">The Clr value of the entity.</param>
        /// <returns>The ODataResource created.</returns>
        private ODataResourceWrapper CreateODataResourceFromEntityOperationParameter(ClientTypeAnnotation clientTypeAnnotation, object parameterValue)
        {
            ClientPropertyAnnotation[] properties = new ClientPropertyAnnotation[0];
            if (sendOption == EntityParameterSendOption.SendOnlySetProperties)
            {
                try
                {
                    var descripter = this.requestInfo.Context.EntityTracker.GetEntityDescriptor(parameterValue);
                    properties = clientTypeAnnotation.PropertiesToSerialize().Where(p => descripter.PropertiesToSerialize.Contains(p.PropertyName)).ToArray();
                }
                catch (InvalidOperationException)
                {
                    throw Error.InvalidOperation(Strings.Context_MustBeUsedWith("EntityParameterSendOption.SendOnlySetProperties", "DataServiceCollection"));
                }
            }

            return this.propertyConverter.CreateODataResourceWrapper(clientTypeAnnotation.ElementType, parameterValue, properties);
        }
    }
}
