//---------------------------------------------------------------------
// <copyright file="AstoriaAttributeToODataLibAnnotationConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
using Microsoft.Test.Taupo.Astoria.Contracts.OData;
using Microsoft.Test.Taupo.Common;
using Microsoft.Test.Taupo.OData.Contracts;
using Microsoft.Test.Taupo.Platforms;

namespace Microsoft.Test.Taupo.OData.Common
{
    /// <summary>
    /// Interface that looks at serializable CSDL annotations (attribute annotations) and converts them to 
    /// ODataLib product annotations (ODataEntityPropertyMappings) on an <see cref="IEdmModel"/>.
    /// </summary>
    [ImplementationName(typeof(IAttributeToODataLibAnnotationConverter), "Default")]
    public class AstoriaAttributeToODataLibAnnotationConverter
    {
        /// <summary>The separator character in entity property mapping attribute names.</summary>
        private const char UnderScore = '_';

        /// <summary>
        /// A dictionary that maps the token following FC_ in attribute names to an index
        /// </summary>
        private Dictionary<string, int> suffixAndIndices = new Dictionary<string, int>();

        /// <summary>
        /// Gets an index which indicates the position of this annotation in the list of annotations on the EntityType.
        /// </summary>
        /// <param name="attributeName">Attribute information from which to figure out the index</param>
        /// <returns>An index indicating the position of this annotation in the list of annotations on the EntityType</returns>
        private int GetAnnotationIndex(string attributeName)
        {
            ExceptionUtilities.CheckArgumentNotNull(attributeName, "attribute");
            string localName = attributeName.Replace("FC_", null);
            ExceptionUtilities.CheckObjectNotNull(localName, "Local Name of the attribute should not be null");

            if (!localName.Contains(UnderScore))
            {
                return 0;
            }
            else
            {
                string suffix = localName.Substring(localName.IndexOf(UnderScore) + 1);

                // See if the suffix exists in the dictionary, if it does return corresponding index,
                // or Create a new index which is the count of the dictionary and add the suffix and index pair to it
                // and return the index.
                if (this.suffixAndIndices.ContainsKey(suffix))
                {
                    return this.suffixAndIndices[suffix];
                }
                else
                {
                    int newIndex = this.suffixAndIndices.Count + 1;
                    this.suffixAndIndices.Add(suffix, newIndex);
                    return newIndex;
                }
            }
        }

        /// <summary>
        /// Populates test annotation values from the serializable annotation.
        /// </summary>
        /// <param name="feedMappingAnnotation">Test annotation to populate values for.</param>
        /// <param name="serializableAnnotation">Serializable annotation to populate values from.</param>
        /// <param name="propertyName">The name of the property to create the mapping for; null for mappings on the type.</param>
        private void PopulateTestAnnotationValues(PropertyMappingAnnotation feedMappingAnnotation, IEdmDirectValueAnnotation serializableAnnotation, string propertyName)
        {
            string localName = serializableAnnotation.Name;
            ExceptionUtilities.CheckObjectNotNull(localName, "localName cannot be null");

            if (localName.Count(c => c == UnderScore) == 2)
            {
                localName = localName.Remove(localName.LastIndexOf(UnderScore));
            }

            IEdmStringValue edmStringValue = serializableAnnotation.Value as IEdmStringValue;
            ExceptionUtilities.CheckObjectNotNull(edmStringValue, "edmStringValue cannot be null");
            string value = edmStringValue.Value;
            ExceptionUtilities.CheckObjectNotNull(value, "value cannot be null");

            switch (localName)
            {
                case ODataConstants.TargetPathAttribute:
                    feedMappingAnnotation.TargetPath = this.HandleTargetPath(value, feedMappingAnnotation);
                    break;
                case ODataConstants.SourcePathAttribute:
                    if (value == null)
                    {
                        feedMappingAnnotation.SourcePath = propertyName;
                    }
                    else if (propertyName != null && !value.StartsWith(propertyName))
                    {
                        feedMappingAnnotation.SourcePath = propertyName + "/" + value;
                    }
                    else
                    {
                        feedMappingAnnotation.SourcePath = value;
                    }

                    break;
                case ODataConstants.ContentKindAttribute:
                    feedMappingAnnotation.SyndicationTextContentKind = ODataExtensions.FromTextContentKindAttributeString(value);
                    break;
                case ODataConstants.KeepInContentAttribute:
                    feedMappingAnnotation.KeepInContent = bool.Parse(value);
                    break;
                case ODataConstants.NSPrefixAttribute:
                    feedMappingAnnotation.TargetNamespacePrefix = value;
                    break;
                case ODataConstants.NSUriAttribute:
                    feedMappingAnnotation.TargetNamespaceUri = value;
                    break;
                default:
                    throw new TaupoArgumentException(
                        string.Format(CultureInfo.InvariantCulture, "Attribute Name:{0} does not match one of Attribute names for Customizing feeds", localName));
            }

            // if there is no source path, use the property name
            if (feedMappingAnnotation.SourcePath == null)
            {
                feedMappingAnnotation.SourcePath = propertyName;
            }
        }

        /// <summary>
        /// Converts attribute value to value for Target Path for FeedMappingAnnotation
        /// </summary>
        /// <param name="attributeValue">attribute value to get the target path value from</param>
        /// <param name="feedMappingAnnotation">Feed Mapping Annotation</param>
        /// <returns>Target Path Value</returns>
        private string HandleTargetPath(string attributeValue, PropertyMappingAnnotation feedMappingAnnotation)
        {
            // The goal here is to return the Target Path Value expected by the test annotation. It uses the same values as 
            // SyndicationItemProperty enumeration found in the Microsoft.OData.Service.Common.

            // if the attribute value in one of the Atom mapping we need to strip the "Syndication" from the beginning and parse it as enum.
            // if not it will be a custom property
            string value = attributeValue.Replace(ODataConstants.EpmAttributePrefix, null);
            Astoria.Contracts.SyndicationItemProperty targetValue;
            if (EnumExtensionMethods.TryParse<Astoria.Contracts.SyndicationItemProperty>(value, false, out targetValue))
            {
                feedMappingAnnotation.SyndicationItemProperty = targetValue;
                return value;
            }
            else
            {
                feedMappingAnnotation.SyndicationItemProperty = Astoria.Contracts.SyndicationItemProperty.CustomProperty;
                return attributeValue;
            }
        }

       
        private static T ParseIntoEnum<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            return (T)Enum.Parse(typeof(T), value, false);
        }
    }
}
