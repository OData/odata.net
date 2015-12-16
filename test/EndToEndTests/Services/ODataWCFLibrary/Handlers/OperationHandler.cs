//---------------------------------------------------------------------
// <copyright file="OperationHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;

    public class OperationHandler : RequestHandler
    {
        public OperationHandler(RequestHandler other, HttpMethod httpMethod)
            : base(other, httpMethod)
        {
            if (httpMethod != HttpMethod.GET && httpMethod != HttpMethod.POST)
            {
                throw new ArgumentException("The HttpMethod in OperationHandler must be GET or POST.");
            }
        }

        public override void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            object result;

            if (this.HttpMethod == HttpMethod.GET)
            {
                // we cannot invoke same action request multiple times, so only Functions are allowed to do the server-driven paging
                this.QueryContext.InitializeServerDrivenPaging(this.PreferenceContext);

                result = this.ProcessFunction();

                if (this.PreferenceContext.MaxPageSize.HasValue)
                {
                    responseMessage.AddPreferenceApplied(string.Format("{0}={1}", ServiceConstants.Preference_MaxPageSize, this.QueryContext.appliedPageSize.Value));
                }
            }
            else
            {
                // TODO: currently ETag feature does not support action operation

                result = this.ProcessAction(requestMessage);
            }

            if (result == null)
            {
                // Protocol 9.1.4 Response Code 204 No Content
                // A request returns 204 No Content if the requested resource has the null value, 
                // or if the service applies a return=minimal preference. In this case, the response body MUST be empty.
                ResponseWriter.WriteEmptyResponse(responseMessage);

                return;
            }

            using (var messageWriter = this.CreateMessageWriter(responseMessage))
            {
                if (this.QueryContext.Target.TypeKind == EdmTypeKind.None ||
                    this.QueryContext.Target.TypeKind == EdmTypeKind.EntityReference ||
                    this.QueryContext.Target.ElementTypeKind == EdmTypeKind.EntityReference)
                {
                    throw Utility.BuildException(HttpStatusCode.NotImplemented, "Unsupported return type in operation.", null);
                }
                else if (this.QueryContext.Target.TypeKind == EdmTypeKind.Entity || this.QueryContext.Target.ElementTypeKind == EdmTypeKind.Entity)
                {
                    ODataWriter resultWriter;

                    IEdmNavigationSource entitySource = this.QueryContext.OperationResultSource ?? this.QueryContext.Target.NavigationSource;

                    if (this.QueryContext.Target.TypeKind == EdmTypeKind.Collection)
                    {
                        IEdmEntitySetBase entitySet = (IEdmEntitySetBase)entitySource;

                        resultWriter = messageWriter.CreateODataFeedWriter(entitySet, (IEdmEntityType)this.QueryContext.Target.ElementType);
                        ResponseWriter.WriteFeed(resultWriter, (IEdmEntityType)this.QueryContext.Target.ElementType, result as IEnumerable, entitySet, ODataVersion.V4, this.QueryContext.QuerySelectExpandClause, this.QueryContext.TotalCount, null, this.QueryContext.NextLink, this.RequestHeaders);
                    }
                    else
                    {
                        if (this.HttpMethod == HttpMethod.GET)
                        {
                            var currentETag = Utility.GetETagValue(result);
                            // if the current entity has ETag field
                            if (currentETag != null)
                            {
                                string requestETag;
                                if (Utility.TryGetIfNoneMatch(this.RequestHeaders, out requestETag) && (requestETag == ServiceConstants.ETagValueAsterisk || requestETag == currentETag))
                                {
                                    ResponseWriter.WriteEmptyResponse(responseMessage, HttpStatusCode.NotModified);
                                    return;
                                }

                                responseMessage.SetHeader(ServiceConstants.HttpHeaders.ETag, currentETag);
                            }
                        }

                        resultWriter = messageWriter.CreateODataEntryWriter(entitySource, (IEdmEntityType)this.QueryContext.Target.Type);
                        ResponseWriter.WriteEntry(resultWriter, result, entitySource, ODataVersion.V4, this.QueryContext.QuerySelectExpandClause, this.RequestHeaders);
                    }
                }
                else
                {
                    ODataProperty property = new ODataProperty() { Name = "value", Value = ODataObjectModelConverter.CreateODataValue(result) };
                    messageWriter.WriteProperty(property);
                }
            }
        }

        private object ProcessFunction()
        {
            return this.QueryContext.ResolveQuery(this.DataSource);
        }

        private object ProcessAction(IODataRequestMessage requestMessage)
        {
            IEdmOperation operation;
            var lastSegment = this.QueryContext.QueryPath.LastSegment;

            if (lastSegment is OperationSegment)
            {
                OperationSegment operationSegment = (OperationSegment)lastSegment;
                operation = operationSegment.Operations.First();
            }
            else
            {
                OperationImportSegment operationImportSegment = (OperationImportSegment)lastSegment;
                operation = operationImportSegment.OperationImports.First().Operation;
            }

            Expression[] parameters = this.ProcessActionInvokePostBody(requestMessage, operation);
            this.QueryContext.ActionInvokeParameters = parameters;

            return this.QueryContext.ResolveQuery(this.DataSource);
        }

        private Expression[] ProcessActionInvokePostBody(IODataRequestMessage message, IEdmOperation operation)
        {
            using (var messageReader = new ODataMessageReader(message, this.GetReaderSettings(), this.DataSource.Model))
            {
                List<Expression> parameterValues = new List<Expression>();
                var parameterReader = messageReader.CreateODataParameterReader(operation);

                while (parameterReader.Read())
                {
                    switch (parameterReader.State)
                    {
                        case ODataParameterReaderState.Value:
                            {
                                object clrValue = ODataObjectModelConverter.ConvertPropertyValue(parameterReader.Value);
                                parameterValues.Add(Expression.Constant(clrValue));
                                break;
                            }
                        case ODataParameterReaderState.Collection:
                            {
                                ODataCollectionReader collectionReader = parameterReader.CreateCollectionReader();
                                object clrValue = ODataObjectModelConverter.ConvertPropertyValue(ODataObjectModelConverter.ReadCollectionParameterValue(collectionReader));
                                parameterValues.Add(Expression.Constant(clrValue, clrValue.GetType()));
                                break;
                            }
                        case ODataParameterReaderState.Entry:
                            {
                                var entryReader = parameterReader.CreateEntryReader();
                                object clrValue = ODataObjectModelConverter.ConvertPropertyValue(ODataObjectModelConverter.ReadEntryParameterValue(entryReader));
                                parameterValues.Add(Expression.Constant(clrValue, clrValue.GetType()));
                                break;
                            }
                        case ODataParameterReaderState.Feed:
                            {
                                IList collectionList = null;
                                var feedReader = parameterReader.CreateFeedReader();
                                while (feedReader.Read())
                                {
                                    if (feedReader.State == ODataReaderState.EntryEnd)
                                    {
                                        object clrItem = ODataObjectModelConverter.ConvertPropertyValue(feedReader.Item);
                                        if (collectionList == null)
                                        {
                                            Type itemType = clrItem.GetType();
                                            Type listType = typeof(List<>).MakeGenericType(new[] { itemType });
                                            collectionList = (IList)Utility.QuickCreateInstance(listType);
                                        }

                                        collectionList.Add(clrItem);
                                    }
                                }

                                parameterValues.Add(Expression.Constant(collectionList, collectionList.GetType()));
                                break;
                            }
                    }
                }

                return parameterValues.ToArray();
            }
        }
    }
}
