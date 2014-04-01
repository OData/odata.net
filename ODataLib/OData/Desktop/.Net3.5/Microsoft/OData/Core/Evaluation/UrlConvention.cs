//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client
#else
#if ASTORIA_SERVER
namespace Microsoft.OData.Service
#else
#if ODATALIB
namespace Microsoft.OData.Core.Evaluation
#else
namespace Microsoft.OData.Service.Design
#endif
#endif
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
#if ASTORIA_DESIGN
    using System.Xml.Linq;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Annotations;
    using Microsoft.OData.Edm.Expressions;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Annotations;
    using Microsoft.OData.Edm.Library.Values;

    /// <summary>
    /// Component for representing the url convention in use by the server, client, or codegen.
    /// </summary>
    internal sealed class UrlConvention
    {
        /// <summary>
        /// The namespace of the term to use when building value annotations for indicating the conventions used.
        /// </summary>
        private const string ConventionTermNamespace = "Com.Microsoft.OData.Service.Conventions.V1";

        /// <summary>
        /// The name of the term to use when building value annotations for indicating the conventions used.
        /// </summary>
        private const string ConventionTermName = "UrlConventions";

        /// <summary>
        /// Default url convention
        /// </summary>
        private const string DefaultConventionName = "Default";

        /// <summary>
        /// The string value for indicating that the key-as-segment convention is being used in annotations and headers.
        /// </summary>
        private const string KeyAsSegmentConventionName = "KeyAsSegment";

        /// <summary>
        /// The name of the request header for indicating what conventions are being used.
        /// </summary>
        private const string UrlConventionHeaderName = "DataServiceUrlConventions";

#if ASTORIA_SERVER
        /// <summary>
        /// The term to use for building value annotations for indicating the conventions used.
        /// </summary>
        private static readonly IEdmValueTerm ConventionTerm = new EdmTerm(ConventionTermNamespace, ConventionTermName, EdmPrimitiveTypeKind.String);

        /// <summary>
        /// The value to use when building value annotations for indicating that the key-as-segment convention is being used.
        /// </summary>
        private static readonly IEdmExpression KeyAsSegmentAnnotationValue = new EdmStringConstant(KeyAsSegmentConventionName);
#endif
        /// <summary>
        /// Whether to generate entity keys as '/'-delimited segments instead of using parenthesis.
        /// </summary>
        private readonly bool generateKeyAsSegment;

        /// <summary>
        /// Prevents a default instance of the <see cref="UrlConvention"/> class from being created.
        /// </summary>
        /// <param name="generateKeyAsSegment">Whether keys should be generated as segments.</param>
        private UrlConvention(bool generateKeyAsSegment)
        {
            this.generateKeyAsSegment = generateKeyAsSegment;
        }

        /// <summary>
        /// Whether to generate entity keys as '/'-delimited segments instead of using parenthesis.
        /// </summary>
        internal bool GenerateKeyAsSegment
        {
            get
            {
#if ODATALIB
#endif
                return this.generateKeyAsSegment;
            }
        }

        /// <summary>
        /// Helper for creating an instance with explicit value. Should only be called from unit tests.
        /// </summary>
        /// <param name="generateKeyAsSegment">Whether keys should be generated as segments.</param>
        /// <returns>A new UrlConvention instance with the given value.</returns>
        internal static UrlConvention CreateWithExplicitValue(bool generateKeyAsSegment)
        {
#if ODATALIB
#endif
            return new UrlConvention(generateKeyAsSegment);
        }

#if ASTORIA_SERVER
        /// <summary>
        /// Creates an instance of <see cref="UrlConvention"/> based on the data services' configuration and operation context.
        /// </summary>
        /// <param name="dataService">The data service.</param>
        /// <returns>The url convention for the service.</returns>
        internal static UrlConvention Create(IDataService dataService)
        {
            Debug.Assert(dataService != null, "dataService != null");
            Debug.Assert(dataService.Configuration != null, "dataService.Configuration != null");
            Debug.Assert(dataService.OperationContext != null, "dataService.OperationContext != null");
            Debug.Assert(dataService.OperationContext.RequestMessage != null, "dataService.OperationContext.RequestMessage != null");
            return CreateFromUserInput(dataService.Configuration.DataServiceBehavior, dataService.OperationContext.RequestMessage.GetCustomHeaderIfAvailable);
        }

        /// <summary>
        /// Helper for creating an instance directly from the relevant user input. Should only be called from unit tests.
        /// </summary>
        /// <param name="dataServiceBehavior">The data service behavior from configuration.</param>
        /// <param name="getCustomHeaderValue">The callback for getting custom header values.</param>
        /// <returns>A new UrlConvention instance based on the user input.</returns>
        internal static UrlConvention CreateFromUserInput(DataServiceBehavior dataServiceBehavior, Func<string, string> getCustomHeaderValue)
        {
            Debug.Assert(dataServiceBehavior != null, "dataServiceBehavior != null");
            Debug.Assert(getCustomHeaderValue != null, "getCustomHeaderValue != null");
            return new UrlConvention(dataServiceBehavior.GenerateKeyAsSegment && IsKeyAsSegment(getCustomHeaderValue(UrlConventionHeaderName)));
        }
#endif

#if ASTORIA_DESIGN
        /// <summary>
        /// Gets the url convention for an entity container based on its vocabulary annotations.
        /// </summary>
        /// <param name="edmxOrCsdlDocument">The edmx or csdl document that defines the model the container belongs to.</param>
        /// <param name="containerName">The name of the container to get the url convention for.</param>
        /// <returns>The url convention of the container.</returns>
        internal static UrlConvention ForEntityContainer(XDocument edmxOrCsdlDocument, string containerName)
        {
            bool keyAsSegment = HasKeyAsSegmentAnnotation(edmxOrCsdlDocument, containerName);
            return CreateWithExplicitValue(keyAsSegment);
        }
#endif

#if ASTORIA_SERVER
        /// <summary>
        /// Builds the annotations needed to indicate the supported url conventions based on the service's configuration.
        /// </summary>
        /// <param name="dataServiceBehavior">The data service behavior.</param>
        /// <param name="model">The service's model.</param>
        /// <returns>The annotations to add to the model.</returns>
        internal static IEnumerable<IEdmValueAnnotation> BuildMetadataAnnotations(DataServiceBehavior dataServiceBehavior, IEdmModel model)
        {
            Debug.Assert(dataServiceBehavior != null, "dataServiceBehavior != null");
            Debug.Assert(model != null, "model != null");

            if (dataServiceBehavior.GenerateKeyAsSegment)
            {
                yield return new EdmAnnotation(model.EntityContainer, ConventionTerm, KeyAsSegmentAnnotationValue);
            }
        }

        /// <summary>
        /// Determines whether or not the value is known.
        /// </summary>
        /// <param name="headerValue">The value to check.</param>
        /// <returns>True if the value is 'KeyAsSegment'.; false if 'Default'.; throw an exception otherwise.</returns>
        private static bool IsKeyAsSegment(string headerValue)
        {
            switch (headerValue)
            {
                case KeyAsSegmentConventionName:
                    return true;
                case DefaultConventionName:
                case null:  // no such header
                    return false;
                default:
                    throw DataServiceException.CreateBadRequestError(Strings.UrlConvention_BadRequestIfUnknown(headerValue, UrlConventionHeaderName, DefaultConventionName, KeyAsSegmentConventionName));
            }
        }
#endif

#if ODATALIB
        /// <summary>
        /// Gets the url convention for the given model based on its vocabulary annotations in the entity container of the model.
        /// </summary>
        /// <param name="model">The model the entity container belongs to.</param>
        /// <returns>The url convention of the model.</returns>
        internal static UrlConvention ForModel(IEdmModel model)
        {
            Debug.Assert(model != null, "model != null");
            IEdmEntityContainer container = model.EntityContainer;
            return CreateWithExplicitValue(model.FindVocabularyAnnotations(container).OfType<IEdmValueAnnotation>().Any(IsKeyAsSegmentUrlConventionAnnotation));
        }

        /// <summary>
        /// Gets the url convention for the given user setting and type context.
        /// </summary>
        /// <param name="keyAsSegment">true if keys should go in seperate segments in auto-generated URIs, false if they should go in parentheses.
        /// A null value means the user hasn't specified a preference and we should look for an annotation in the entity container, if available.</param>
        /// <param name="typeContext">The type context for the entry or feed being written.</param>
        /// <returns>The convention to use when generating URLs.</returns>
        internal static UrlConvention ForUserSettingAndTypeContext(bool? keyAsSegment, IODataFeedAndEntryTypeContext typeContext)
        {
            Debug.Assert(typeContext != null, "typeContext != null");

            // The setting from the user is an override, so check if it was set.
            if (keyAsSegment.HasValue)
            {
                return CreateWithExplicitValue(keyAsSegment.Value);
            }

            return typeContext.UrlConvention;
        }

        /// <summary>
        /// Determines whether or not the annotation indicates the 'KeyAsSegment' url-convention.
        /// </summary>
        /// <param name="annotation">The annotation to check.</param>
        /// <returns>True if the annotation indicates the 'KeyAsSegment' url convention; false otherwise.</returns>
        private static bool IsKeyAsSegmentUrlConventionAnnotation(IEdmValueAnnotation annotation)
        {
            return annotation != null && IsUrlConventionTerm(annotation.Term) && IsKeyAsSegment(annotation.Value);
        }

        /// <summary>
        /// Determines whether or not the value is 'KeyAsSegment'.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is 'KeyAsSegment'.; false otherwise.</returns>
        private static bool IsKeyAsSegment(IEdmExpression value)
        {
            return value != null && value.ExpressionKind == EdmExpressionKind.StringConstant && ((IEdmStringConstantExpression)value).Value == KeyAsSegmentConventionName;
        }

        /// <summary>
        /// Determines whether or not the term is the url-convention term.
        /// </summary>
        /// <param name="term">The term to check.</param>
        /// <returns>True if the term is the url-convention term.; false otherwise.</returns>
        private static bool IsUrlConventionTerm(IEdmTerm term)
        {
            return term != null && (term.Name == ConventionTermName && term.Namespace == ConventionTermNamespace);
        }
#endif

#if ASTORIA_CLIENT
        /// <summary>
        /// Adds the required headers for the url convention.
        /// </summary>
        /// <param name="requestHeaders">The request headers to add to.</param>
        internal void AddRequiredHeaders(HeaderCollection requestHeaders)
        {
            Debug.Assert(requestHeaders != null, "requestHeaders != null");
            if (this.GenerateKeyAsSegment)
            {
                requestHeaders.SetHeader(UrlConventionHeaderName, KeyAsSegmentConventionName);
            }
        }
#endif

#if ASTORIA_DESIGN
        /// <summary>
        /// Determines whether the model contains a value annotation indicating that the container with the given name uses the 'KeyAsSegment' url convention.
        /// </summary>
        /// <param name="edmxOrCsdlDocument">The edmx or csdl document.</param>
        /// <param name="containerName">The name of the entity container.</param>
        /// <returns>Whether the container uses the key-as-segment convention.</returns>
        private static bool HasKeyAsSegmentAnnotation(XDocument edmxOrCsdlDocument, string containerName)
        {
            // <Annotations Target="AstoriaUnitTests.Tests.KeyAsSegmentContext">
            //   <Annotation Term="Com.Microsoft.OData.Service.Conventions.V1.UrlConventions" String="KeyAsSegment" />
            // </Annotations>
            const string TermValue = ConventionTermNamespace + "." + ConventionTermName;
            XNamespace annotationsNamespace = XNamespace.Get("http://docs.oasis-open.org/odata/ns/edm");

            // 1) Get all 'Annotations' elements...
            // 2) which target a container with the given full name or unqualified name...
            // 3) get all 'Annotation' elements...
            // 4) and see if any of them have the right term and value.
            bool containerNameHasNamespace = containerName.Contains(".");
            return edmxOrCsdlDocument
                .Descendants(annotationsNamespace.GetName("Annotations"))                
                .Where(annotations => HasAnnotationWithValue(annotations, "Target", value => value == containerName || (!containerNameHasNamespace && value.EndsWith("." + containerName, StringComparison.Ordinal))))
                .SelectMany(annotations => annotations.Elements(annotationsNamespace.GetName("Annotation")))
                .Any(annotation => HasAnnotationWithValue(annotation, "Term", TermValue) && HasAnnotationWithValue(annotation, "String", KeyAsSegmentConventionName));
        }

        /// <summary>
        /// Returns whether the given element has an annotation with the given name/value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="annotationName">The annotation name.</param>
        /// <param name="annotationValue">The annotation value.</param>
        /// <returns>True if an annotation with the name/value was found, false otherwise.</returns>
        private static bool HasAnnotationWithValue(XElement element, XName annotationName, string annotationValue)
        {
            return HasAnnotationWithValue(element, annotationName, value => value == annotationValue);
        }

        /// <summary>
        /// Returns whether the given element has an annotation with the given name that passes the given predicate.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="annotationName">The annotation name.</param>
        /// <param name="checkAnnotationValue">The callback to check the annotation's value.</param>
        /// <returns>True if an annotation with the name and a suitable value was found, false otherwise.</returns>
        private static bool HasAnnotationWithValue(XElement element, XName annotationName, Func<string, bool> checkAnnotationValue)
        {
            XAttribute annotation = element.Attribute(annotationName);
            return annotation != null && checkAnnotationValue(annotation.Value);
        }
#endif
    }
}
