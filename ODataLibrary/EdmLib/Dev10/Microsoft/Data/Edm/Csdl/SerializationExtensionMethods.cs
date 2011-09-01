//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Xml;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Csdl
{
    /// <summary>
    /// Contains extension methods for <see cref="IEdmModel"/> interfaces that are useful to serialization.
    /// </summary>
    public static class SerializationExtensionMethods
    {
        /// <summary>
        /// Gets the value for the EDMX version of the <paramref name="model"/>.
        /// </summary>
        /// <param name="model">Model the version has been set for.</param>
        /// <returns>The version.</returns>
        public static Version GetEdmxVersion(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotation<Version>(EdmConstants.InternalUri, CsdlConstants.EdmxVersionAnnotation);
        }

        /// <summary>
        /// Sets a value of EDMX version attribute of the <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The model the version should be set for.</param>
        /// <param name="version">The version.</param>
        public static void SetEdmxVersion(this IEdmModel model, Version version)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            model.SetAnnotation(EdmConstants.InternalUri, CsdlConstants.EdmxVersionAnnotation, version);
        }

        /// <summary>
        /// Gets the namespace name of a CSDL schema hosting the <paramref name="container"/> during CSDL serialization.
        /// </summary>
        /// <param name="container">Entity container the namespace has been set for.</param>
        /// <returns>The schema namespace.</returns>
        public static string GetEntityContainerSchemaNamespace(this IEdmEntityContainer container)
        {
            EdmUtil.CheckArgumentNull(container, "container");
            return container.GetAnnotation<string>(EdmConstants.InternalUri, CsdlConstants.EntityContainerSchemaNamespaceAnnotation);
        }

        /// <summary>
        /// Sets the namespace name of a CSDL schema hosting the <paramref name="container"/> during CSDL serialization.
        /// </summary>
        /// <param name="container">The entity container the namespace should be set for.</param>
        /// <param name="schemaNamespace">The schema namespace.</param>
        public static void SetEntityContainerSchemaNamespace(this IEdmEntityContainer container, string schemaNamespace)
        {
            EdmUtil.CheckArgumentNull(container, "container");
            container.SetAnnotation(EdmConstants.InternalUri, CsdlConstants.EntityContainerSchemaNamespaceAnnotation, schemaNamespace);
        }

        /// <summary>
        /// Sets an annotation on the IEdmModel to notify the serializer of preferred prefix mappings for xml namespaces.
        /// </summary>
        /// <param name="model">Reference to the calling object.</param>
        /// <param name="mappings">XmlNamespaceManage containing mappings between namespace prefixes and xml namespaces.</param>
        public static void SetNamespacePrefixMappings(this IEdmModel model, XmlNamespaceManager mappings)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            model.SetAnnotation(EdmConstants.InternalUri, EdmConstants.NamespacePrefixLocalName, mappings);
        }
        
        /// <summary>
        /// Gets the preferred prefix mappings for xml namespaces from an IEdmModel
        /// </summary>
        /// <param name="model">Reference to the calling object.</param>
        /// <returns>Namespace prefixes that exist on the model.</returns>
        public static XmlNamespaceManager GetNamespacePrefixMappings(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotation<XmlNamespaceManager>(EdmConstants.InternalUri, EdmConstants.NamespacePrefixLocalName);
        }

        /// <summary>
        /// Sets a value for the DataServiceVersion attribute in an EDMX artifact.
        /// </summary>
        /// <param name="model">The model the attribute should be set for.</param>
        /// <param name="version">The value of the attribute.</param>
        public static void SetDataServiceVersion(this IEdmModel model, Version version)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            model.SetAnnotation(EdmConstants.InternalUri, EdmConstants.DataServiceVersion, version);
        }

        /// <summary>
        /// Gets the value for the DataServiceVersion attribute used during EDMX serialization.
        /// </summary>
        /// <param name="model">Model the attribute has been set for.</param>
        /// <returns>Value of the attribute.</returns>
        public static Version GetDataServiceVersion(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotation<Version>(EdmConstants.InternalUri, EdmConstants.DataServiceVersion);
        }

        /// <summary>
        /// Sets a value for the MaxDataServiceVersion attribute in an EDMX artifact.
        /// </summary>
        /// <param name="model">The model the attribute should be set for.</param>
        /// <param name="version">The value of the attribute.</param>
        public static void SetMaxDataServiceVersion(this IEdmModel model, Version version)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            model.SetAnnotation(EdmConstants.InternalUri, EdmConstants.MaxDataServiceVersion, version);
        }

        /// <summary>
        /// Gets the value for the MaxDataServiceVersion attribute used during EDMX serialization.
        /// </summary>
        /// <param name="model">Model the attribute has been set for</param>
        /// <returns>Value of the attribute.</returns>
        public static Version GetMaxDataServiceVersion(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotation<Version>(EdmConstants.InternalUri, EdmConstants.MaxDataServiceVersion);
        }
    }
}
