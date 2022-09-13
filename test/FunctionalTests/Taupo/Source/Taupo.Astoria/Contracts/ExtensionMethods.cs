//---------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Extension methods that make writing astoria tests easier, mostly pieces that connect various 
    /// Odata and Http components together
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Determines if an operationParameterBindingKind
        /// </summary>
        /// <param name="operationParameterBindingKind">Operation Parameter Binding Kind</param>
        /// <returns>Whether its bound or not</returns>
        public static bool IsBound(this OperationParameterBindingKind operationParameterBindingKind)
        {
            if (operationParameterBindingKind != OperationParameterBindingKind.Never)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Constructs a strong etag using property values from the given instance
        /// </summary>
        /// <param name="converter">The converter to extend</param>
        /// <param name="instance">The instance to get property values from</param>
        /// <returns>A strong etag</returns>
        public static string ConstructStrongETag(this IODataLiteralConverter converter, QueryStructuralValue instance)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");

            var entityType = instance.Type as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Given structural instance was not an entity. Type was: {0}", instance.Type);

            var properties = entityType.EntityType.AllProperties.Where(p => p.Annotations.OfType<ConcurrencyTokenAnnotation>().Any()).ToList();
            if (!properties.Any())
            {
                return null;
            }

            var values = properties.Select(p => instance.GetScalarValue(p.Name));

            // TODO: pad any fixed-length fields
            var pieces = values.Select(v => Uri.EscapeDataString(converter.SerializePrimitive(v.Value)));
            var etag = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", string.Join(",", pieces.ToArray()));

            return etag;
        }

        /// <summary>
        /// Constructs a weak etag using property values from the given instance
        /// </summary>
        /// <param name="converter">The converter to extend</param>
        /// <param name="instance">The instance to get property values from</param>
        /// <returns>A weak etag</returns>
        public static string ConstructWeakETag(this IODataLiteralConverter converter, QueryStructuralValue instance)
        {
            // the other method will check that the instance is non null
            string strongETag = converter.ConstructStrongETag(instance);
            if (strongETag == null)
            {
                return null;
            }

            return "W/" + strongETag;
        }

        /// <summary>
        /// Generates an OData uri from the given query, with then given service root uri
        /// </summary>
        /// <param name="converter">The converter to extend</param>
        /// <param name="query">The query to use when generating the query</param>
        /// <param name="serviceUri">The service root uri</param>
        /// <returns>An OData uri based on the query</returns>
        public static ODataUri GenerateUriFromQuery(this IQueryToODataUriConverter converter, QueryExpression query, Uri serviceUri)
        {
            ExceptionUtilities.CheckArgumentNotNull(query, "query");
            ExceptionUtilities.CheckArgumentNotNull(serviceUri, "serviceUri");

            var uri = converter.ComputeUri(query);
            uri.Segments.Insert(0, new ServiceRootSegment(serviceUri));
            return uri;
        }

        /// <summary>
        /// Appends the key values according to the rules for formatting a key expression in an OData uri
        /// </summary>
        /// <param name="converter">The literal converter for formatting key values</param>
        /// <param name="builder">The string builder to append to</param>
        /// <param name="keyValues">The key values to append</param>
        public static void AppendKeyExpression(this IODataLiteralConverter converter, StringBuilder builder, IEnumerable<NamedValue> keyValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(converter, "converter");
            ExceptionUtilities.CheckArgumentNotNull(builder, "builder");
            ExceptionUtilities.CheckArgumentNotNull(keyValues, "keyValues");

            var keyValuesList = keyValues.ToList();

            builder.Append('(');
            if (keyValuesList.Count == 1)
            {
                builder.Append(Uri.EscapeDataString(converter.SerializePrimitive(keyValues.Single().Value)));
            }
            else
            {
                bool first = true;
                foreach (var keyValue in keyValuesList)
                {
                    if (!first)
                    {
                        builder.Append(',');
                    }
                    else
                    {
                        first = false;
                    }

                    builder.Append(Uri.EscapeDataString(keyValue.Name));
                    builder.Append('=');
                    builder.Append(Uri.EscapeDataString(converter.SerializePrimitive(keyValue.Value)));
                }
            }

            builder.Append(')');
        }

        /// <summary>
        /// Executes the Request Synchronously/Asynchronously based on the <paramref name="runAsync"/> value
        /// </summary>
        /// <param name="request">The Request to run</param>
        /// <param name="runAsync">A value indicating whether the request should run asynchronously</param>
        /// <param name="continuation">The continuation to call after the response is obtained</param>
        /// <param name="onResponse">The action to call with the response</param>
        /// <typeparam name="TResponseType">The type of the response object</typeparam>
        public static void GetResponse<TResponseType>(this WebRequest request, bool runAsync, IAsyncContinuation continuation, Action<TResponseType> onResponse) where TResponseType : WebResponse
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");
            ExceptionUtilities.CheckArgumentNotNull(onResponse, "onResponse");

            AsyncHelpers.HandleException(
                continuation,
                continuation2 => AsyncHelpers.InvokeSyncOrAsyncMethodCall<TResponseType>(
                    continuation2,
                    runAsync,
                    () => (TResponseType)request.GetResponse(),
                    c => request.BeginGetResponse(c, null),
                    r => (TResponseType)request.EndGetResponse(r),
                    onResponse),
                (WebException webException) => onResponse((TResponseType)webException.Response));
        }

        /// <summary>
        /// Converts test enum to product enum.
        /// </summary>
        /// <typeparam name="TOriginal">Type of original enum</typeparam>
        /// <typeparam name="TResult">Type of result enum</typeparam>
        /// <param name="original">the original enum value</param>
        /// <returns>The corresponding result enum value</returns>
        public static TResult ConvertEnum<TOriginal, TResult>(TOriginal original)
            where TOriginal : struct
            where TResult : struct
        {
            var stringValue = Enum.GetName(typeof(TOriginal), original);
            return (TResult)Enum.Parse(typeof(TResult), stringValue, false);
        }

        /// <summary>
        /// Indicates whether or not a query expression represents a root level entity set query
        /// </summary>
        /// <param name="expression">the query expression</param>
        /// <returns>whether or not the expression represents a service operation call</returns>
        public static bool IsRootEntitySetQuery(this QueryExpression expression)
        {
            var rootExpression = expression as QueryRootExpression;
            var collectionType = expression.ExpressionType as QueryCollectionType;
            return rootExpression != null && collectionType != null && collectionType.ElementType is QueryEntityType;
        }

        /// <summary>
        /// Generate client side code for calling service operations
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="rootExpression">The root code expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public static CodeExpression GenerateClientCode(this QueryCustomFunctionCallExpression expression, CodeExpression rootExpression)
        {
            // Generate this:
            //     return base.CreateQuery<entityType>("<Service Operation Name>").AddQueryOption("arg1", argValue1);
            var entityType = expression.ExpressionType as QueryEntityType;
            if (entityType == null)
            {
                var collectionType = expression.ExpressionType as QueryCollectionType;
                if (collectionType != null)
                {
                    entityType = collectionType.ElementType as QueryEntityType;
                }
            }

            ExceptionUtilities.CheckObjectNotNull(entityType, "Non-entity service operations are not supported");
            var codeExpression = rootExpression.Call("CreateQuery", new CodeTypeReference[] { Code.TypeRef(entityType.ClrType) }, Code.Primitive(expression.Function.Name));
            for (int i = 0; i < expression.Arguments.Count; ++i)
            {
                var functionParameter = expression.Function.Parameters[i];
                var argValue = (expression.Arguments[i] as QueryConstantExpression).ScalarValue;
                if (argValue != null)
                {
                    codeExpression = codeExpression.Call("AddQueryOption", Code.Primitive(functionParameter.Name), Code.Primitive(argValue.Value));
                }
            }

            return codeExpression;
        }

        /// <summary>
        /// Deep clones an XNode
        /// </summary>
        /// <param name="node">the XNode to clone</param>
        /// <returns>a deep clone of the XNode</returns>
        public static XNode DeepClone(this XNode node)
        {
            ExceptionUtilities.CheckArgumentNotNull(node, "node");

            XElement element = node as XElement;
            if (element != null)
            {
                return new XElement(element);
            }

            XDocument document = node as XDocument;
            if (document != null)
            {
                return new XDocument(document);
            }

            XComment comment = node as XComment;
            if (comment != null)
            {
                return new XComment(comment);
            }

            XDocumentType documentType = node as XDocumentType;
            if (documentType != null)
            {
                return new XDocumentType(documentType);
            }

            XText text = node as XText;
            if (text != null)
            {
                return new XText(text);
            }

            XProcessingInstruction processingInstruction = node as XProcessingInstruction;
            if (processingInstruction != null)
            {
                return new XProcessingInstruction(processingInstruction);
            }

            throw new TaupoInvalidOperationException("Unknown node type : " + node.GetType());
        }

        /// <summary>
        /// Adds the annotation if no equivalent annotation is found.
        /// </summary>
        /// <typeparam name="TAnnotation">The type of the annotation.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="annotationToAdd">The annotation to add.</param>
        public static void AddAnnotationIfNotExist<TAnnotation>(this IAnnotatable<TAnnotation> value, TAnnotation annotationToAdd)
            where TAnnotation : class, IEquatable<TAnnotation>
        {
            value.AddAnnotationIfNotExist<TAnnotation, TAnnotation>(annotationToAdd);
        }

        /// <summary>
        /// Adds the annotation if no equivalent annotation is found.
        /// </summary>
        /// <typeparam name="TAnnotationBase">The base type of the annotation base.</typeparam>
        /// <typeparam name="TAnnotation">The specific type of the annotation.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="annotationToAdd">The annotation to add.</param>
        public static void AddAnnotationIfNotExist<TAnnotationBase, TAnnotation>(this IAnnotatable<TAnnotationBase> value, TAnnotation annotationToAdd)
            where TAnnotationBase : class
            where TAnnotation : TAnnotationBase, IEquatable<TAnnotation>
        {
            value.AddAnnotationIfNotExist<TAnnotationBase, TAnnotation, TAnnotation>(annotationToAdd);
        }

        /// <summary>
        /// Adds the annotation if no equivalent annotation is found.
        /// </summary>
        /// <typeparam name="TAnnotationBase">The base type of the annotation.</typeparam>
        /// <typeparam name="TEquatable">The equatable type of the annotation.</typeparam>
        /// <typeparam name="TAnnotation">The specific type of the annotation.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="annotationToAdd">The annotation to add.</param>
        internal static void AddAnnotationIfNotExist<TAnnotationBase, TEquatable, TAnnotation>(this IAnnotatable<TAnnotationBase> value, TAnnotation annotationToAdd)
            where TAnnotationBase : class
            where TEquatable : TAnnotationBase, IEquatable<TEquatable>
            where TAnnotation : TEquatable
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");
            if (!value.Annotations.OfType<TAnnotation>().Any(a => a.Equals(annotationToAdd)))
            {
                value.Annotations.Add(annotationToAdd);
            }
        }
    }
}
