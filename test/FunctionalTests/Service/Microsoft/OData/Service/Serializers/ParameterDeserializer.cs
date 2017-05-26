//---------------------------------------------------------------------
// <copyright file="ParameterDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Implements deserializer for Action parameters payload.
    /// </summary>
    internal sealed class ParameterDeserializer : ODataMessageReaderDeserializer
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ParameterDeserializer"/>.
        /// </summary>
        /// <param name="update">true if we're reading an update operation; false if not.</param>
        /// <param name="dataService">Data service for which the deserializer will act.</param>
        /// <param name="tracker">Tracker to use for modifications.</param>
        /// <param name="requestDescription">The request description to use.</param>
        internal ParameterDeserializer(bool update, IDataService dataService, UpdateTracker tracker, RequestDescription requestDescription)
            : base(update, dataService, tracker, requestDescription, false /*enableODataServerBehavior*/)
        {
        }

        /// <summary>
        /// Reads the Action parameters payload and returns the WCF DS value representation of each parameter.
        /// </summary>
        /// <param name="segmentInfo">Info about the parameters payload to read.</param>
        /// <returns>The WCF DS representation of the parameters read.</returns>
        protected override object Read(SegmentInfo segmentInfo)
        {
            Debug.Assert(segmentInfo != null, "segmentInfo != null");
            Debug.Assert(
                segmentInfo.TargetSource == RequestTargetSource.ServiceOperation &&
                segmentInfo.Operation != null &&
                segmentInfo.Operation.Kind == OperationKind.Action,
                "The ParametersDeserializer should only be called for an Action segment.");

            IEdmOperation operation = this.GetOperation(segmentInfo.Operation);
            ODataParameterReader reader = this.MessageReader.CreateODataParameterReader(operation);
            AssertReaderFormatIsExpected(this.MessageReader, ODataFormat.Json);

            Dictionary<string, object> parameters = new Dictionary<string, object>(EqualityComparer<string>.Default);
            ResourceType parameterResourceType;
            object convertedParameterValue;
            while (reader.Read())
            {
                if (reader.State == ODataParameterReaderState.Completed)
                {
                    break;
                }

                switch (reader.State)
                {
                    case ODataParameterReaderState.Value:
                        parameterResourceType = segmentInfo.Operation.Parameters.Single(p => p.Name == reader.Name).ParameterType;
                        convertedParameterValue = this.ConvertValue(reader.Value, ref parameterResourceType);
                        break;

                    case ODataParameterReaderState.Collection:
                        ODataCollectionReader collectionReader = reader.CreateCollectionReader();
                        parameterResourceType = segmentInfo.Operation.Parameters.Single(p => p.Name == reader.Name).ParameterType;
                        Debug.Assert(parameterResourceType.ResourceTypeKind == ResourceTypeKind.Collection, "parameterResourceType.ResourceTypeKind == ResourceTypeKind.Collection");
                        convertedParameterValue = this.ConvertValue(ParameterDeserializer.ReadCollectionParameterValue(collectionReader), ref parameterResourceType);
                        break;

                    default:
                        Debug.Assert(false, "Unreachable code path in Read().");
                        throw new InvalidOperationException(Microsoft.OData.Service.Strings.DataServiceException_GeneralError);
                }

                parameters.Add(reader.Name, convertedParameterValue);
            }

            // ODataLib allows nullable parameters to be missing from the payload. When that happens, we use null for the parameter value.
            foreach (IEdmOperationParameter parameterMetadata in operation.Parameters.Skip(operation.IsBound ? 1 : 0))
            {
                object value;
                if (!parameters.TryGetValue(parameterMetadata.Name, out value))
                {
                    Debug.Assert(parameterMetadata.Type.IsNullable, "ODataParameterReader should only allows nullable parameters to be missing from the payload.");
                    parameters.Add(parameterMetadata.Name, null);
                }
            }

            return parameters;
        }

        /// <summary>
        /// Reads the items from a collection and return it as an ODataCollectionValue.
        /// </summary>
        /// <param name="collectionReader">Collection reader to read from.</param>
        /// <returns>An ODataCollectionValue instance containing all items in the collection.</returns>
        private static ODataCollectionValue ReadCollectionParameterValue(ODataCollectionReader collectionReader)
        {
            Debug.Assert(collectionReader != null, "collectionReader != null");
            Debug.Assert(collectionReader.State == ODataCollectionReaderState.Start, "The collection reader should not have been used.");

            List<object> collectionItems = new List<object>();
            while (collectionReader.Read())
            {
                if (collectionReader.State == ODataCollectionReaderState.Completed)
                {
                    break;
                }

                switch (collectionReader.State)
                {
                    case ODataCollectionReaderState.CollectionStart:
                    case ODataCollectionReaderState.CollectionEnd:
                        break;

                    case ODataCollectionReaderState.Value:
                        collectionItems.Add(collectionReader.Item);
                        break;

                    default:
                        Debug.Assert(false, "Unreachable code path in ReadCollectionParameterValue().");
                        throw new InvalidOperationException(Microsoft.OData.Service.Strings.DataServiceException_GeneralError);
                }
            }

            ODataCollectionValue result = new ODataCollectionValue();
            result.Items = collectionItems;
            return result;
        }
    }
}
