//---------------------------------------------------------------------
// <copyright file="StreamServices.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Xml;
    using System.Xml.Linq;       
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;
    
    /// <summary>
    /// Service that provides data populations for streams.
    /// </summary>
    [ImplementationName(typeof(IStreamsServices), "Default")]
    public class StreamServices : IStreamsServices
    {
        /// <summary>
        /// Phone reflection issue fixup, please do not change.
        /// The Phone clr's support for open generic types is limited and won't let application code refer to derived types as generic parameters.
        /// </summary>
        private List<object> streamsToUpdate;

        /// <summary>
        /// Phone reflection issue fixup, please do not change.
        /// The Phone clr's support for open generic types is limited and won't let application code refer to derived types as generic parameters.
        /// </summary>
        private Dictionary<object, object> queryStructuralValueToRootQueryLookup;

        /// <summary>
        /// Phone reflection issue fixup, please do not change.
        /// The Phone clr's support for open generic types is limited and won't let application code refer to derived types as generic parameters.
        /// </summary>
        private List<object> queryStructuralValuesToAddStreamsTo;

        /// <summary>
        /// Gets or sets the service descriptor.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AstoriaWorkspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets the query repository.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public QueryRepository QueryRepository { get; set; }

        /// <summary>
        /// Gets or sets random number generator to be used by tests.
        /// </summary>
        [InjectDependency]
        public IRandomNumberGenerator Random { get; set; }

        /// <summary>
        /// Gets or sets the component to convert QueryExpressions to Uris
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryToUriStringConverter QueryExpressionToUriConverter { get; set; }

        /// <summary>
        /// Gets or sets the payload element converter to read Named Stream links from response payload
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IXmlToPayloadElementConverter XmlToPayloadConverter { get; set; }

        /// <summary>
        /// Gets or sets the synchronizer to use after updating the streams
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAsyncDataSynchronizer Synchronizer { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency(IsRequired = true)]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use asynchronouse APIs
        /// </summary>
        [InjectTestParameter("Asynchronous", DefaultValueDescription = "False", HelpText = "Whether to use asynchronous API")]
        public bool Asynchronous { get; set; }

        /// <summary>
        /// Generates named stream data for the test
        /// </summary>
        /// <param name="continuation">continuation token</param>
        public virtual void PopulateStreamsData(IAsyncContinuation continuation)
        {
            if (!this.Workspace.StreamsDataAlreadyAdd)
            {
                ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");

                this.streamsToUpdate = new List<object>();
                this.queryStructuralValueToRootQueryLookup = new Dictionary<object, object>();
                this.queryStructuralValuesToAddStreamsTo = new List<object>();

                this.FindEntitiesWithStreams(this.QueryRepository);

                AsyncHelpers.RunActionSequence(continuation, this.GetEntityInstances, this.UpdateStreams, this.SynchronizeEntities, this.MarkStreamServicesDataSetupOnWorkspace);
            }
            else
            {
                continuation.Continue();
            }
        }

        private static void SendStreamValueToServer(IAsyncContinuation continuation, bool async, string editLinkString, string contentType, string etag, byte[] streamData)
        {
            HttpWebRequest streamPutRequest;

            streamPutRequest = (HttpWebRequest)HttpWebRequest.Create(editLinkString);
            // using verb-tunneling
            streamPutRequest.Method = "POST";
            streamPutRequest.Headers[HttpHeaders.HttpMethod] = HttpVerb.Put.ToHttpMethod();

            streamPutRequest.ContentType = contentType;
            if (etag != null)
            {
                streamPutRequest.Headers[HttpHeaders.IfMatch] = etag;
            }

            streamPutRequest.AllowWriteStreamBuffering = false;
            streamPutRequest.ContentLength = streamData.Length;
            streamPutRequest.BeginGetRequestStream(
               (requestStreamResult) =>
               {
                   var requestStream = streamPutRequest.EndGetRequestStream(requestStreamResult);
                   requestStream.Write(streamData, 0, streamData.Length);
                   requestStream.Close();
                   streamPutRequest.GetResponse<HttpWebResponse>(
                       async,
                       continuation,
                       (streamResponse) =>
                       {
                           ExceptionUtilities.Assert(streamResponse.StatusCode == HttpStatusCode.NoContent, "Named stream update failed");
                           continuation.Continue();
                       });
               },
               null);
        }

        private void MarkStreamServicesDataSetupOnWorkspace(IAsyncContinuation continuation)
        {
            this.Workspace.StreamsDataAlreadyAdd = true;
            continuation.Continue();
        }

        private void FindEntitiesWithStreams(QueryRepository repository)
        {
            // Lambdas inside a Linq to objects query cannot refer to extension methods defined on types derived from non-system types.
            var streamRootQueries = repository.RootQueries.Collections<QueryStructuralType>();

            foreach (var rootQuery in streamRootQueries)
            {
                QueryEntityType type = rootQuery.ExpressionType.ElementType as QueryEntityType;
                ExceptionUtilities.CheckObjectNotNull(type, "Type was not an entity type. Type was {0}", type.StringRepresentation);
                if (rootQuery.Expression.IsRootEntitySetQuery())
                {
                    if (!type.Properties.Streams().Any())
                    {
                        continue;
                    }

                    var entitySetRightsAnnotation = type.EntitySet.Annotations.OfType<EntitySetRightsAnnotation>().SingleOrDefault();
                    
                    // Skipping streams generation if EntitySetRights are not all
                    if (entitySetRightsAnnotation != null && entitySetRightsAnnotation.Value != EntitySetRights.All)
                    {
                        continue;
                    }

                    var existing = this.QueryRepository.DataSet[type.EntitySet.Name];
                    ExceptionUtilities.Assert(existing != null, "Existing collection should not be null");

                    int i = 0;
                    foreach (var qv in existing.Elements)
                    {
                        var qsv = qv as QueryStructuralValue;
                        ExceptionUtilities.CheckObjectNotNull(qsv, "Should only be query structural value");

                        if (qsv.Type.Properties.Streams().Any())
                        {
                            i++;
                            this.queryStructuralValuesToAddStreamsTo.Add(qsv);
                            this.queryStructuralValueToRootQueryLookup.Add(qsv, rootQuery.Expression);
                        }
                    }

                    if (i > 0)
                    {
                        this.Logger.WriteLine(LogLevel.Verbose, "Adding streams to '{0}' entity Instances in Entity Set '{1}'", i, type.EntitySet.Name);
                    }
                }
            }
        }

        private void GetEntityInstances(IAsyncContinuation continuation)
        {
            AsyncHelpers.AsyncForEach(this.queryStructuralValuesToAddStreamsTo.Cast<QueryStructuralValue>(), continuation, this.QueryEntityInstanceAndGenerateStreams);
        }

        private void UpdateStreams(IAsyncContinuation continuation)
        {
            AsyncHelpers.AsyncForEach(this.streamsToUpdate.Cast<StreamData>(), continuation, this.UpdateStreams);
        }

        private void SynchronizeEntities(IAsyncContinuation continuation)
        {
            AsyncHelpers.AsyncForEach(this.queryStructuralValuesToAddStreamsTo.Cast<QueryStructuralValue>(), continuation, (e, c) => this.Synchronizer.SynchronizeEntity(c, e));
        }

        private void UpdateStreams(StreamData streamData, IAsyncContinuation continuation)
        {
            SendStreamValueToServer(
                continuation,
                this.Asynchronous,
                streamData.EditLink,
                streamData.ContentType,
                streamData.ETag,
                streamData.Content);
        }

        private void QueryEntityInstanceAndGenerateStreams(QueryStructuralValue queryStructuralValue, IAsyncContinuation continuation)
        {
            var rootQuery = (QueryExpression)this.queryStructuralValueToRootQueryLookup[queryStructuralValue];
            var query = ODataQueryTestCase.GetExistingEntityQuery(rootQuery, queryStructuralValue);
            var requestUriString = this.QueryExpressionToUriConverter.ComputeUri(query);
            requestUriString = UriHelpers.ConcatenateUriSegments(this.Workspace.ServiceUri.OriginalString, requestUriString);

            HttpWebRequest requestForExistingEntity = (HttpWebRequest)HttpWebRequest.Create(requestUriString);

            requestForExistingEntity.GetResponse<HttpWebResponse>(
                this.Asynchronous,
                continuation,
                (response) =>
                {
                    ProcessEntityAndGenerateStreams(queryStructuralValue, continuation, response);
                });
        }

        private void ProcessEntityAndGenerateStreams(QueryStructuralValue queryStructuralValue, IAsyncContinuation continuation, HttpWebResponse response)
        {
            ExceptionUtilities.Assert(response.StatusCode == HttpStatusCode.OK, "Error generating stream data, response code incorrect:" + response.StatusCode.ToString());
            var responseValue = new StreamReader(response.GetResponseStream()).ReadToEnd();

            try
            {
                var existingEntityInXML = XElement.Parse(responseValue);
                var feedInstance = this.XmlToPayloadConverter.ConvertToPayloadElement(existingEntityInXML) as EntitySetInstance;
                ExceptionUtilities.CheckObjectNotNull(feedInstance, "Error generating stream data, cannot deserialize response:" + existingEntityInXML);

                var type = queryStructuralValue.Type as QueryEntityType;
                ExceptionUtilities.CheckObjectNotNull(type, "Type was not an entity type. Type was {0}", type.StringRepresentation);

                Func<AstoriaQueryStreamValue, bool> valueFilter = v => v.IsNull || v.Value.Length == 0;
                Func<QueryProperty, bool> propertyFilter = p => p.IsStream() && valueFilter(queryStructuralValue.GetStreamValue(p.Name));
                var streamPropertiesToUpdate = type.Properties.Where(propertyFilter).ToList();

                if (feedInstance.Count != 0)
                {
                    var instance = feedInstance.SingleOrDefault();
                    EntityInstance entityInstance = instance as EntityInstance;
                    if (entityInstance != null)
                    {
                        ExceptionUtilities.CheckObjectNotNull(entityInstance, "Payload did not contain a single entity instance");

                        var baseAddressAnnotation = feedInstance.Annotations.OfType<XmlBaseAnnotation>().SingleOrDefault();

                        foreach (var streamProperty in streamPropertiesToUpdate)
                        {
                            var streamPropertyType = streamProperty.PropertyType as AstoriaQueryStreamType;
                            ExceptionUtilities.CheckObjectNotNull(streamPropertyType, "PropertyType is not an AstoriaQueryStreamType!", streamProperty.PropertyType);

                            var streamData = this.GenerateStreamData(entityInstance, baseAddressAnnotation, streamProperty);
                            this.streamsToUpdate.Add(streamData);
                        }
                    }
                }

                continuation.Continue();
            }
            catch (XmlException)
            {
                this.Logger.WriteLine(LogLevel.Error, "Error in Xml payload:" + responseValue);

                throw;
            }
        }

        private StreamData GenerateStreamData(EntityInstance entityInstance, XmlBaseAnnotation baseAddressAnnotation, QueryProperty streamProperty)
        {
            string editLinkValue;
            string contentType;
            string etag;
            if (streamProperty.Name == AstoriaQueryStreamType.DefaultStreamPropertyName)
            {
                editLinkValue = entityInstance.StreamEditLink;
                contentType = entityInstance.StreamContentType;
                etag = entityInstance.StreamETag;
            }
            else
            {
                var namedStream = entityInstance.Properties.OfType<NamedStreamInstance>().SingleOrDefault(p => p.Name == streamProperty.Name);
                ExceptionUtilities.CheckObjectNotNull(namedStream, "Could not find name stream '{0}' in entity payload", streamProperty.Name);
                editLinkValue = namedStream.EditLink;
                contentType = namedStream.EditLinkContentType;
                etag = namedStream.ETag;
            }

            if (contentType == null)
            {
                // TODO: add atom/json?
                contentType = this.Random.ChooseFrom(new[] { MimeTypes.TextPlain, MimeTypes.ApplicationOctetStream, MimeTypes.ApplicationHttp });
            }

            var streamData = new StreamData()
            {
                ContentType = contentType,
                ETag = etag,
                Content = this.Random.NextBytes(this.Random.Next(100)),
            };

            if (editLinkValue != null)
            {
                var editLink = new Uri(editLinkValue, UriKind.RelativeOrAbsolute);
                if (!editLink.IsAbsoluteUri)
                {
                    ExceptionUtilities.CheckObjectNotNull(baseAddressAnnotation, "Cannot construct absolute uri for edit link because xml:base annotation was missing");
                    editLink = new Uri(new Uri(baseAddressAnnotation.Value), editLinkValue);
                }

                streamData.EditLink = editLink.AbsoluteUri;
            }

            return streamData;
        }
    }
}
