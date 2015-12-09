//---------------------------------------------------------------------
// <copyright file="UrlConventionsConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;

namespace Microsoft.OData.Core.Tests
{
    public class UrlConventionsConstants
    {
        /// <summary>
        /// The namespace which 'Annotations' and 'ValueAnnotation' elements appear in.
        /// </summary>
        public const string AnnotationsNamespace = "http://docs.oasis-open.org/odata/ns/edm";

        /// <summary>
        /// The namespace of the term to use when building value annotations for indicating the conventions used.
        /// </summary>
        public const string ConventionTermNamespace = "Com.Microsoft.OData.Service.Conventions.V1";

        /// <summary>
        /// The name of the term to use when building value annotations for indicating the conventions used.
        /// </summary>
        public const string ConventionTermName = "UrlConventions";

        /// <summary>
        /// The string value to use when building value annotations for indicating that the key-as-segment convention is being used.
        /// </summary>
        public const string KeyAsSegmentAnnotationValueString = "KeyAsSegment";

        /// <summary>
        /// The name of the request header for indicating what conventions are being used.
        /// </summary>
        public const string UrlConventionHeaderName = "DataServiceUrlConventions";

        /// <summary>
        /// The term to use for building annotations for indicating the conventions used.
        /// </summary>
        public static readonly EdmTerm ConventionTerm = new EdmTerm(ConventionTermNamespace, ConventionTermName, EdmPrimitiveTypeKind.String);

        /// <summary>
        /// The value to use when building value annotations for indicating that the key-as-segment convention is being used.
        /// </summary>
        public static readonly EdmStringConstant KeyAsSegmentAnnotationValue = new EdmStringConstant(KeyAsSegmentAnnotationValueString);
    }
}