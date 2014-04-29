//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Serializes the request data into the given format using the given message writer.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Class needs refactoring.")]
    internal class Serializer
    {
        /// <summary>where to pull the changes from</summary>
        private readonly RequestInfo requestInfo;

        /// <summary>The property converter to use for creating ODataProperty instances.</summary>
        private readonly ODataPropertyConverter propertyConverter;

        /// <summary>The save changes option.</summary>
        private readonly SaveChangesOptions options;

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
        /// <param name="options">the save change options.</param>
        internal Serializer(RequestInfo requestInfo, SaveChangesOptions options)
            : this(requestInfo)
        {
            this.options = options;
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
            var writerSettings = requestInfo.WriteHelper.CreateSettings(requestMessage.IsBatchPartRequest, requestInfo.Context.EnableAtom);
            return requestMessage.CreateWriter(writerSettings, isParameterPayload);
        }

        /// <summary>
        /// Creates an ODataEntry for the given EntityDescriptor and fills in its ODataLib metadata.
        /// </summary>
        /// <param name="entityDescriptor">The entity descriptor.</param>
        /// <param name="serverTypeName">Name of the server type.</param>
        /// <param name="entityType">The client-side entity type.</param>
        /// <param name="clientFormat">The current client format.</param>
        /// <returns>An odata entry with its metadata filled in.</returns>
        internal static ODataEntry CreateODataEntry(EntityDescriptor entityDescriptor, string serverTypeName, ClientTypeAnnotation entityType, DataServiceClientFormat clientFormat)
        {
            ODataEntry entry = new ODataEntry();

            // If the client type name is different from the server type name, then add SerializationTypeNameAnnotation
            // which tells ODataLib to write the type name in the annotation in the payload.
            if (entityType.ElementTypeName != serverTypeName)
            {
                entry.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = serverTypeName });
            }

            // We always need to write the client type name, since this is the type name used by ODataLib
            // to resolve the entity type using EdmModel.FindSchemaElement.
            entry.TypeName = entityType.ElementTypeName;

            // Continue to send the entry's ID in update payloads in Atom for compatibility with V1-V3,
            // but for JSON-Light we do not want the extra information on the wire.
            if (clientFormat.UsingAtom && EntityStates.Modified == entityDescriptor.State)
            {
                // <id>http://host/service/entityset(key)</id>
                entry.Id = entityDescriptor.GetLatestIdentity();
            }

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
                ODataParameterWriter parameterWriter = messageWriter.CreateODataParameterWriter((IEdmOperation)null);
                parameterWriter.WriteStart();

                foreach (OperationParameter operationParameter in operationParameters)
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
                                    // TODO: just call ODataPropertyConverter.CreateODataCollection()
                                    IEnumerator enumerator = ((ICollection)operationParameter.Value).GetEnumerator();
                                    ODataCollectionWriter collectionWriter = parameterWriter.CreateCollectionWriter(operationParameter.Name);
                                    ODataCollectionStart odataCollectionStart = new ODataCollectionStart();
                                    collectionWriter.WriteStart(odataCollectionStart);

                                    while (enumerator.MoveNext())
                                    {
                                        Object collectionItem = enumerator.Current;
                                        if (collectionItem == null)
                                        {
                                            throw new NotSupportedException(Strings.Serializer_NullCollectionParamterItemValue(operationParameter.Name));
                                        }

                                        IEdmType edmItemType = model.GetOrCreateEdmType(collectionItem.GetType());
                                        Debug.Assert(edmItemType != null, "edmItemType != null");

                                        switch (edmItemType.TypeKind)
                                        {
                                            case EdmTypeKind.Complex:
                                                {
                                                    Debug.Assert(model.GetClientTypeAnnotation(edmItemType).ElementType != null, "edmItemType.GetClientTypeAnnotation().ElementType != null");
                                                    ODataComplexValue complexValue = this.propertyConverter.CreateODataComplexValue(
                                                        model.GetClientTypeAnnotation(edmItemType).ElementType,
                                                        collectionItem,
                                                        null /*propertyName*/,
                                                        false /*isCollectionItem*/,
                                                        null /*visitedComplexTypeObjects*/);

                                                    Debug.Assert(complexValue != null, "complexValue != null");
                                                    collectionWriter.WriteItem(complexValue);
                                                    break;
                                                }

                                            case EdmTypeKind.Primitive:
                                                {
                                                    object primitiveItemValue = ODataPropertyConverter.ConvertPrimitiveValueToRecognizedODataType(collectionItem, collectionItem.GetType());
                                                    collectionWriter.WriteItem(primitiveItemValue);
                                                    break;
                                                }

                                            case EdmTypeKind.Enum:
                                                {
                                                    ODataEnumValue enumTmp = this.propertyConverter.CreateODataEnumValue(
                                                        model.GetClientTypeAnnotation(edmItemType).ElementType,
                                                        collectionItem,
                                                        false);
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
                                    break;
                                }

                            case EdmTypeKind.Complex:
                                {
                                    Debug.Assert(model.GetClientTypeAnnotation(edmType).ElementType != null, "model.GetClientTypeAnnotation(edmType).ElementType != null");
                                    ODataComplexValue complexValue = this.propertyConverter.CreateODataComplexValue(
                                        model.GetClientTypeAnnotation(edmType).ElementType,
                                        operationParameter.Value,
                                        null /*propertyName*/,
                                        false /*isCollectionItemType*/,
                                        null /*visitedComplexTypeObjects*/);

                                    Debug.Assert(complexValue != null, "complexValue != null");
                                    parameterWriter.WriteValue(operationParameter.Name, complexValue);
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
                                // EdmTypeKind.Entity
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

                if (EntityStates.Added == entityDescriptor.State)
                {
                    this.WriteNavigationLink(entityDescriptor, relatedLinks, entryWriter);
                }

                entryWriter.WriteEnd(entry, entityDescriptor.Entity);
            }
        }

        /// <summary>
        /// Writes a navigation link.
        /// </summary>
        /// <param name="entityDescriptor">The entity</param>
        /// <param name="relatedLinks">The links related to the entity</param>
        /// <param name="odataWriter">The ODataWriter used to write the navigation link.</param>
        internal void WriteNavigationLink(EntityDescriptor entityDescriptor, IEnumerable<LinkDescriptor> relatedLinks, ODataWriterWrapper odataWriter)
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

                bool isCollection = clientType.GetProperty(grlinks.Key, false).IsEntityCollection;
                bool started = false;

                foreach (LinkDescriptor end in grlinks.Value)
                {
                    Debug.Assert(!end.ContentGeneratedForSave, "already saved link");
                    end.ContentGeneratedForSave = true;
                    Debug.Assert(null != end.Target, "null is DELETE");

                    ODataNavigationLink navigationLink = new ODataNavigationLink();
                    navigationLink.Url = this.requestInfo.EntityTracker.GetEntityDescriptor(end.Target).GetLatestEditLink();
                    Debug.Assert(Uri.IsWellFormedUriString(UriUtil.UriToString(navigationLink.Url), UriKind.Absolute), "Uri.IsWellFormedUriString(targetEditLink, UriKind.Absolute)");

                    navigationLink.IsCollection = isCollection;
                    navigationLink.Name = grlinks.Key;

                    if (!started)
                    {
                        odataWriter.WriteNavigationLinksStart(navigationLink);
                        started = true;
                    }

                    odataWriter.WriteNavigationLinkStart(navigationLink, end.Source, end.Target);
                    odataWriter.WriteEntityReferenceLink(new ODataEntityReferenceLink() { Url = navigationLink.Url }, end.Source, end.Target);
                    odataWriter.WriteNavigationLinkEnd(navigationLink, end.Source, end.Target);
                }

                odataWriter.WriteNavigationLinksEnd();
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
                else if (paramName.StartsWith(Char.ToString(UriHelper.ATSIGN), StringComparison.OrdinalIgnoreCase))
                {
                    // name=value&
                    queryBuilder.Append(paramName);
                    queryBuilder.Append(UriHelper.EQUALSSIGN);
                    queryBuilder.Append(this.ConvertToEscapedUriValue(paramName, op.Value));
                    queryBuilder.Append(UriHelper.AMPERSAND);
                }
                else
                {
                    // name=value,
                    pathBuilder.Append(paramName);
                    pathBuilder.Append(UriHelper.EQUALSSIGN);
                    pathBuilder.Append(this.ConvertToEscapedUriValue(paramName, op.Value));
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
        /// Converts a <see cref="UriOperationParameter"/> value to an escaped string for use in a Uri. Wraps the call to ODL's ConvertToUriLiteral and escapes the results. 
        /// </summary>
        /// <param name="paramName">The name of the <see cref="UriOperationParameter"/>. Used for error reporting.</param>
        /// <param name="value">The value of the <see cref="UriOperationParameter"/>.</param>
        /// <returns>A string representation of <paramref name="value"/> for use in a Url.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0018:SystemUriEscapeDataStringRule", Justification = "Values being escaped are known not to contain single quotes.")]
        private string ConvertToEscapedUriValue(string paramName, object value)
        {
            Debug.Assert(!string.IsNullOrEmpty(paramName), "!string.IsNullOrEmpty(paramName)");
            Object valueInODataFormat = null;

            // Literal values with single quotes need special escaping due to System.Uri changes in behavior between .NET 4.0 and 4.5.
            // We need to ensure that our escaped values do not change between those versions, so we need to escape values differently when they could contain single quotes.
            bool needsSpecialEscaping = false;
            if (value == null)
            {
                needsSpecialEscaping = true;
            }
            else
            {
                if (value is ODataNullValue)
                {
                    valueInODataFormat = value;
                    needsSpecialEscaping = true;
                }
                else
                {
                    ClientEdmModel model = this.requestInfo.Model;
                    IEdmType edmType = model.GetOrCreateEdmType(value.GetType());
                    Debug.Assert(edmType != null, "edmType != null");

                    switch (edmType.TypeKind)
                    {
                        case EdmTypeKind.Primitive:
                            valueInODataFormat = value;
                            needsSpecialEscaping = true;
                            break;

                        case EdmTypeKind.Enum:
                            {
                                ClientTypeAnnotation typeAnnotation = model.GetClientTypeAnnotation(edmType);
                                string typeNameInEdm = this.requestInfo.GetServerTypeName(model.GetClientTypeAnnotation(edmType));
                                MemberInfo member = typeAnnotation.ElementType.GetMember(value.ToString()).FirstOrDefault();
                                if (member == null)
                                {
                                    throw new NotSupportedException(Strings.Serializer_InvalidEnumMemberValue(typeAnnotation.ElementType.Name, value.ToString()));
                                }

                                string memberValue = ClientTypeUtil.GetServerDefinedName(member);

                                valueInODataFormat = new ODataEnumValue(memberValue, typeNameInEdm ?? typeAnnotation.ElementTypeName);
                                needsSpecialEscaping = true;
                            }

                            break;

                        case EdmTypeKind.Complex:
                            {
                                ClientTypeAnnotation typeAnnotation = model.GetClientTypeAnnotation(edmType);
                                Debug.Assert(typeAnnotation != null, "typeAnnotation != null");
                                valueInODataFormat = this.propertyConverter.CreateODataComplexValue(typeAnnotation.ElementType, value, null /*propertyName*/, false /*isCollectionItemType*/, null /*visitedComplexTypeObjects*/);

                                // When using JsonVerbose to format query string parameters for Actions, 
                                // we cannot write out Complex values in the URI without the type name of the complex type in the JSON payload.
                                // If this value is null, the client has to set the ResolveName property on the DataServiceContext instance.
                                ODataComplexValue complexValue = (ODataComplexValue)valueInODataFormat;
                                SerializationTypeNameAnnotation serializedTypeNameAnnotation = complexValue.GetAnnotation<SerializationTypeNameAnnotation>();
                                if (serializedTypeNameAnnotation == null || string.IsNullOrEmpty(serializedTypeNameAnnotation.TypeName))
                                {
                                    throw Error.InvalidOperation(Strings.DataServiceException_GeneralError);
                                }
                            }

                            break;

                        case EdmTypeKind.Collection:
                            IEdmCollectionType edmCollectionType = edmType as IEdmCollectionType;
                            Debug.Assert(edmCollectionType != null, "edmCollectionType != null");
                            IEdmTypeReference itemTypeReference = edmCollectionType.ElementType;
                            Debug.Assert(itemTypeReference != null, "itemTypeReference != null");
                            ClientTypeAnnotation itemTypeAnnotation = model.GetClientTypeAnnotation(itemTypeReference.Definition);
                            Debug.Assert(itemTypeAnnotation != null, "itemTypeAnnotation != null");

                            switch (itemTypeAnnotation.EdmType.TypeKind)
                            {
                                // We only support primitive, Enum or complex type as a collection item type.
                                case EdmTypeKind.Primitive:
                                case EdmTypeKind.Enum:
                                case EdmTypeKind.Complex:
                                    break;

                                default:
                                    throw new NotSupportedException(Strings.Serializer_InvalidCollectionParamterItemType(paramName, itemTypeAnnotation.EdmType.TypeKind));
                            }

                            valueInODataFormat = this.propertyConverter.CreateODataCollection(
                                itemTypeAnnotation.ElementType,
                                null /*propertyName*/,
                                value,
                                null /*visitedComplexTypeObjects*/);
                            break;

                        default:
                            // EdmTypeKind.Entity
                            // EdmTypeKind.Row
                            // EdmTypeKind.EntityReference
                            throw new NotSupportedException(Strings.Serializer_InvalidParameterType(paramName, edmType.TypeKind));
                    }
                }

                Debug.Assert(valueInODataFormat != null, "valueInODataFormat != null");
            }

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
    }
}
