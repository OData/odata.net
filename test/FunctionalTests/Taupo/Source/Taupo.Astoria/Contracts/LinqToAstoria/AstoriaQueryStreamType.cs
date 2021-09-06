//---------------------------------------------------------------------
// <copyright file="AstoriaQueryStreamType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Represents a stream type in a QueryType hierarchy.
    /// </summary>
    public class AstoriaQueryStreamType : QueryStreamType, IQueryClrType
    {
        private static string defaultStream = "DefaultStream$" + Guid.NewGuid().ToString("N");
        private object collectionType;
        
        /// <summary>
        /// Initializes a new instance of the AstoriaQueryStreamType class.
        /// </summary>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        public AstoriaQueryStreamType(IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationStrategy)
        {
        }

        /// <summary>
        /// Gets the name of the default stream property
        /// </summary>
        public static string DefaultStreamPropertyName
        {
            get
            {
                return defaultStream;
            }
        }

        /// <summary>
        /// Gets the null value for a given type.
        /// </summary>
        public new AstoriaQueryStreamValue NullValue
        {
            get { return this.CreateValue(null); }
        }

        /// <summary>
        /// Gets the CLR type represented by this type
        /// </summary>
        public Type ClrType
        {
            get
            {
                return typeof(DataServiceStreamLink);
            }
        }

        /// <summary>
        /// Creates the stream value for this type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Newly created value.</returns>
        public AstoriaQueryStreamValue CreateValue(byte[] value)
        {
            return new AstoriaQueryStreamValue(this, value, null, this.EvaluationStrategy);
        }

        /// <summary>
        /// Creates the stream value for this type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="descriptor">The stream descriptor</param>
        /// <returns>Newly created value.</returns>
        public AstoriaQueryStreamValue CreateValue(byte[] value, WrappedStreamDescriptor descriptor)
        {
            var streamLink = descriptor.StreamLink;
            return this.CreateValue(value, streamLink.ContentType, streamLink.ETag, streamLink.EditLink, streamLink.SelfLink);
        }

        /// <summary>
        /// Creates the stream value for this type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="contentType"> The content type of the Named Stream</param>
        /// <param name="streamETag"> The eTag for the Named Stream</param>
        /// <param name="editLink"> The edit link to the Named Stream</param>
        /// <param name="selfLink"> The self link to the Named Stream</param>
        /// <returns>Newly created value.</returns>
        public AstoriaQueryStreamValue CreateValue(byte[] value, string contentType, string streamETag, Uri editLink, Uri selfLink)
        {
            AstoriaQueryStreamValue streamValue = this.CreateValue(value);
            streamValue.ContentType = contentType;
            streamValue.ETag = streamETag;
            streamValue.SelfLink = selfLink;
            streamValue.EditLink = editLink;
            return streamValue;
        }

        /// <summary>
        /// Creates the null value with error information attached.
        /// </summary>
        /// <param name="error">The error information.</param>
        /// <returns>Newly created value.</returns>
        public new AstoriaQueryStreamValue CreateErrorValue(QueryError error)
        {
            return new AstoriaQueryStreamValue(this, null, error, this.EvaluationStrategy);
        }

        /// <summary>
        /// Determines whether the type can be assigned from another.
        /// </summary>
        /// <param name="queryType">Type to assign from.</param>
        /// <returns>True if assignment is possible, false otherwise.</returns>
        public override bool IsAssignableFrom(QueryType queryType)
        {
            return queryType is AstoriaQueryStreamType;
        }

        /// <summary>
        /// Creates the typed collection where element type is the current primitive type.
        /// </summary>
        /// <returns>Collection of the given type.</returns>
        public new QueryCollectionType<AstoriaQueryStreamType> CreateCollectionType()
        {
            return (QueryCollectionType<AstoriaQueryStreamType>)this.CreateCollectionTypeInternal();
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query type.</param>
        /// <returns>The result of visiting this query type.</returns>
        public override TResult Accept<TResult>(IQueryTypeVisitor<TResult> visitor)
        {
            throw new TaupoNotSupportedException("Do not support Visitor for Stream type yet.");
        }

        /// <summary>
        /// Creates the typed collection where element type is the current primitive type.
        /// </summary>
        /// <returns>Collection of the given type.</returns>
        protected override QueryCollectionType CreateCollectionTypeInternal()
        {
            if (this.collectionType == null)
            {
                this.collectionType = QueryCollectionType.Create(this);
            }

            return (QueryCollectionType)this.collectionType;
        }

        /// <summary>
        /// Gets the non-strongly typed null value.
        /// </summary>
        /// <returns>Null value.</returns>
        protected override QueryValue GetNullValueInternal()
        {
            return this.NullValue;
        }

        /// <summary>
        /// Creates the non-strongly typed error value.
        /// </summary>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <returns>Created error value.</returns>
        protected override QueryValue CreateErrorValueInternal(QueryError evaluationError)
        {
            return this.CreateErrorValue(evaluationError);
        }
    }
}
