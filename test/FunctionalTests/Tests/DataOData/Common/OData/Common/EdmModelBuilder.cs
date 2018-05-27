//---------------------------------------------------------------------
// <copyright file="EdmModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Helper methods for working with annotations on a model.
    /// </summary>
    public static class EdmModelBuilder
    {
        /// <summary>
        /// Loads a model from the resource.
        /// </summary>
        /// <param name="resourceAssembly">The assembly that stores the resource.</param>
        /// <param name="name">The name of the resource file to load.</param>
        /// <param name="references">The model references to add.</param>
        /// <returns>The loaded model.</returns>
        public static IEdmModel LoadModelFromResource(Assembly resourceAssembly, string name, params IEdmModel[] references)
        {
            Stream modelStream = resourceAssembly.GetManifestResourceStream(name);
            using (XmlReader reader = XmlReader.Create(modelStream))
            {
                IEdmModel model;
                IEnumerable<EdmError> errors;
                if (!SchemaReader.TryParse(new XmlReader[] { reader }, references, out model, out errors))
                {
                    throw new AssertionFailedException("Model loading failed: " + string.Join("\r\n", errors.Select(e => e.ErrorLocation.ToString() + ": " + e.ErrorMessage)));
                }

                return model;
            }
        }

        /// <summary>
        /// Loads the OData terms from the resource.
        /// </summary>
        /// <returns>The loaded model with the OData terms.</returns>
        public static IEdmModel LoadODataTermModel()
        {
            return LoadModelFromResource(typeof(EdmModelBuilder).Assembly, "Microsoft.Test.Taupo.OData.Models.OData.csdl");
        }

        /// <summary>
        /// Builds a model with annotations.
        /// </summary>
        /// <param name="annotationXml">The annotation element(s) as XML string.</param>
        /// <param name="references">The model references to add.</param>
        /// <returns>The loaded model.</returns>
        /// <remarks>The <paramref name="annotationXml"/> string must not include the wrapping schema element.</remarks>
        public static IEdmModel BuildAnnotationModel(string annotationXml, params IEdmModel[] references)
        {
            string annotationSchema =
                "<Schema Namespace='TestModelStandardAnnotations' xmlns='http://docs.oasis-open.org/odata/ns/edm'>" +
                    annotationXml +
                "</Schema>";

            using (XmlReader reader = XmlReader.Create(new StringReader(annotationSchema)))
            {
                IEdmModel model;
                IEnumerable<EdmError> errors;
                if (!SchemaReader.TryParse(new XmlReader[] { reader }, references, out model, out errors))
                {
                    throw new AssertionFailedException("Model loading failed: " + string.Join("\r\n", errors.Select(e => e.ErrorLocation.ToString() + ": " + e.ErrorMessage)));
                }

                return model;
            }
        }
    }
}
