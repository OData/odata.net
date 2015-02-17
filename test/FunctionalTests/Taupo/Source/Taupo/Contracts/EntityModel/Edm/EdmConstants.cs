//---------------------------------------------------------------------
// <copyright file="EdmConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Edm
{
    using System.Xml.Linq;

    /// <summary>
    /// Constants that are specific to our Edm implementation
    /// For example, XmlNamespaces in csdl, edmx
    /// </summary>
    public static class EdmConstants
    {
        /// <summary>
        /// Initializes static members of the EdmConstants class.
        /// </summary>
        static EdmConstants()
        {
            EdmxOasisNamespace = "http://docs.oasis-open.org/odata/ns/edmx";

            CsdlOasisNamespace = "http://docs.oasis-open.org/odata/ns/edm";

            AnnotationNamespace = "http://schemas.microsoft.com/ado/2009/02/edm/annotation";
            CodegenNamespace = "http://schemas.microsoft.com/ado/2006/04/codegeneration";

            TaupoAnnotationsNamespace = "http://microsoft.com/taupo/annotations/";
        }

        /// <summary>
        /// Gets the XML namespace for EDMX v4.0
        /// </summary>
        public static XNamespace EdmxOasisNamespace { get; private set; }

        /// <summary>
        /// Gets the XML namespace for CSDL Oasis.
        /// </summary>
        public static XNamespace CsdlOasisNamespace { get; private set; }

        /// <summary>
        /// Gets the XML namespace for product annotations in CSDL.
        /// </summary>
        public static XNamespace AnnotationNamespace { get; private set; }

        /// <summary>
        /// Gets the XML namespace for Codegen.
        /// </summary>
        public static XNamespace CodegenNamespace { get; private set; }

        /// <summary>
        /// Gets the XML namespace for Taupo-specific annotations in CSDL.
        /// </summary>
        public static XNamespace TaupoAnnotationsNamespace { get; private set; }
    }
}
