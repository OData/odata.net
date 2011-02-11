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

namespace System.Data.OData.Atom
{
    #region Namespaces.
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml;
    #endregion Namespaces.

    /// <summary>
    /// Writer for the EPM custom-only. Writes the EPM custom mapping properties into XmlWriter.
    /// </summary>
    internal static class EpmCustomWriter
    {
        /// <summary>
        /// Writes the custom mapped EPM properties to an XML writer which is expected to be positioned such to write
        /// a child element of the entry element.
        /// </summary>
        /// <param name="writer">The XmlWriter to write to.</param>
        /// <param name="epmTargetTree">The EPM target tree to use.</param>
        /// <param name="epmValueCache">The entry properties value cache to use to access the properties.</param>
        /// <param name="resourceType">The resource type of the entry.</param>
        /// <param name="metadata">The metadata provider to use.</param>
        internal static void WriteEntryEpm(
            XmlWriter writer,
            EpmTargetTree epmTargetTree,
            EntryPropertiesValueCache epmValueCache,
            ResourceType resourceType,
            DataServiceMetadataProviderWrapper metadata)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(epmTargetTree != null, "epmTargetTree != null");
            Debug.Assert(epmValueCache != null, "epmValueCache != null");
            Debug.Assert(resourceType != null, "For any EPM to exist the metadata must be available.");

            // If there are no custom mappings, just return null.
            EpmTargetPathSegment customRootSegment = epmTargetTree.NonSyndicationRoot;
            Debug.Assert(customRootSegment != null, "EPM Target tree must always have non-syndication root.");
            if (customRootSegment.SubSegments.Count == 0)
            {
                return;
            }

            foreach (EpmTargetPathSegment targetSegment in customRootSegment.SubSegments)
            {
                Debug.Assert(!targetSegment.IsAttribute, "Target segments under the custom root must be for elements only.");

                string alreadyDeclaredPrefix = null;
                WriteElementEpm(writer, targetSegment, epmValueCache, resourceType, metadata, ref alreadyDeclaredPrefix);
            }
        }

        /// <summary>
        /// Writes an EPM element target.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="targetSegment">The target segment describing the element to write.</param>
        /// <param name="epmValueCache">The entry properties value cache to use to access the properties.</param>
        /// <param name="resourceType">The resource type of the entry.</param>
        /// <param name="metadata">The metadata provider to use.</param>
        /// <param name="alreadyDeclaredPrefix">The name of the prefix if it was already declared.</param>
        private static void WriteElementEpm(
            XmlWriter writer, 
            EpmTargetPathSegment targetSegment, 
            EntryPropertiesValueCache epmValueCache, 
            ResourceType resourceType, 
            DataServiceMetadataProviderWrapper metadata,
            ref string alreadyDeclaredPrefix)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(targetSegment != null && !targetSegment.IsAttribute, "Only element target segments are supported by this method.");

            // If the prefix is null, the WCF DS will still write it as the default namespace, so we need it to be an empty string.
            string elementPrefix = targetSegment.SegmentNamespacePrefix ?? string.Empty;
            writer.WriteStartElement(elementPrefix, targetSegment.SegmentName, targetSegment.SegmentNamespaceUri);

            // Write out the declaration explicitly only if the prefix is not empty (just like the WCF DS does)
            if (elementPrefix.Length > 0)
            {
                WriteNamespaceDeclaration(writer, targetSegment, ref alreadyDeclaredPrefix);
            }

            // Serialize the sub segment attributes first
            foreach (EpmTargetPathSegment subSegment in targetSegment.SubSegments)
            {
                if (subSegment.IsAttribute)
                {
                    WriteAttributeEpm(writer, subSegment, epmValueCache, resourceType, metadata, ref alreadyDeclaredPrefix);
                }
            }

            if (targetSegment.HasContent)
            {
                Debug.Assert(!targetSegment.SubSegments.Any(subSegment => !subSegment.IsAttribute), "If the segment has a content, it must not have any element children.");

                string textPropertyValue = GetEntryPropertyValueAsText(targetSegment, epmValueCache, resourceType, metadata);

                // TODO: In V3 we should check for textPropertyValue == null and write out the m:null in that case.
                Debug.Assert(textPropertyValue != null, "Null property value should not get here, the GetEntryPropertyValueAsText should take care of that for now.");

                writer.WriteString(textPropertyValue);
            }
            else
            {
                // Serialize the sub segment elements now
                foreach (EpmTargetPathSegment subSegment in targetSegment.SubSegments)
                {
                    if (!subSegment.IsAttribute)
                    {
                        WriteElementEpm(writer, subSegment, epmValueCache, resourceType, metadata, ref alreadyDeclaredPrefix);
                    }
                }
            }

            // Close the element
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes an EPM attribute target.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="targetSegment">The target segment describing the attribute to write.</param>
        /// <param name="epmValueCache">The entry properties value cache to use to access the properties.</param>
        /// <param name="resourceType">The resource type of the entry.</param>
        /// <param name="metadata">The metadata provider to use.</param>
        /// <param name="alreadyDeclaredPrefix">The name of the prefix if it was already declared.</param>
        private static void WriteAttributeEpm(
            XmlWriter writer, 
            EpmTargetPathSegment targetSegment, 
            EntryPropertiesValueCache epmValueCache, 
            ResourceType resourceType, 
            DataServiceMetadataProviderWrapper metadata,
            ref string alreadyDeclaredPrefix)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(targetSegment != null && targetSegment.IsAttribute, "Only attribute target segments are supported by this method.");
            Debug.Assert(targetSegment.HasContent, "Attribute target segments must have content.");

            string textPropertyValue = GetEntryPropertyValueAsText(targetSegment, epmValueCache, resourceType, metadata);
            Debug.Assert(textPropertyValue != null, "Text value of a property mapped to attribute must not be null, the GetEntryPropertyValueAsText should take care of that.");

            // If the prefix is null, the WCF DS will still write it as the default namespace, so we need it to be an empty string.
            string attributePrefix = targetSegment.SegmentNamespacePrefix ?? string.Empty;
            writer.WriteAttributeString(attributePrefix, targetSegment.AttributeName, targetSegment.SegmentNamespaceUri, textPropertyValue);

            // Write out the declaration explicitely only if the prefix is not empty (just like the WCF DS does)
            if (attributePrefix.Length > 0)
            {
                WriteNamespaceDeclaration(writer, targetSegment, ref alreadyDeclaredPrefix);
            }
        }

        /// <summary>
        /// Writes a namespace declaration attribute for the namespace required by the target segment.
        /// </summary>
        /// <param name="writer">The writer to write the declaration to.</param>
        /// <param name="targetSegment">The target segment to write the declaration for.</param>
        /// <param name="alreadyDeclaredPrefix">The name of the prefix if it was already declared.</param>
        private static void WriteNamespaceDeclaration(
            XmlWriter writer,
            EpmTargetPathSegment targetSegment,
            ref string alreadyDeclaredPrefix)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(targetSegment != null, "targetSegment != null");

            Debug.Assert(
                alreadyDeclaredPrefix == null || alreadyDeclaredPrefix == targetSegment.SegmentNamespacePrefix,
                "Found a subsegment with different prefix than the parent segment. The custom EPM writer is not ready to handle that.");

            if (alreadyDeclaredPrefix == null)
            {
                writer.WriteAttributeString(
                    AtomConstants.XmlnsNamespacePrefix,
                    targetSegment.SegmentNamespacePrefix,
                    AtomConstants.XmlNamespacesNamespace,
                    targetSegment.SegmentNamespaceUri);

                alreadyDeclaredPrefix = targetSegment.SegmentNamespacePrefix;
            }
        }

        /// <summary>
        /// Given a target segment the method returns the text value of the property mapped to that segment to be used in EPM.
        /// </summary>
        /// <param name="targetSegment">The target segment to read the value for.</param>
        /// <param name="epmValueCache">The entry EPM value cache to use.</param>
        /// <param name="resourceType">The resource type of the entry being processed.</param>
        /// <param name="metadata">The metadata provider to use.</param>
        /// <returns>The test representation of the value, or the method throws if the text representation was not possible to obtain.</returns>
        private static string GetEntryPropertyValueAsText(
            EpmTargetPathSegment targetSegment, 
            EntryPropertiesValueCache epmValueCache, 
            ResourceType resourceType,
            DataServiceMetadataProviderWrapper metadata)
        {
            Debug.Assert(targetSegment != null, "targetSegment != null");
            Debug.Assert(targetSegment.HasContent, "The target segment to read property for must have content.");
            Debug.Assert(targetSegment.EpmInfo != null, "The EPM info must be available on the target segment to read its property.");
            Debug.Assert(epmValueCache != null, "epmValueCache != null");
            Debug.Assert(resourceType != null, "resourceType != null");

            bool nullOnParentProperty;
            object propertyValue = targetSegment.EpmInfo.ReadEntryPropertyValue(epmValueCache, resourceType, metadata, out nullOnParentProperty);
            if (propertyValue == null)
            {
                // TODO: In V3 when we use new format for null values using the m:null attribute we need to check here
                // if we're mapping into an attribute, in which case we should fail (can't write null to attribute in V3)
                // or if we're writing into element return true null, to use the m:null attribute.
                // In V2 nulls are written out as empty string always (and they're written into content as well)
                return string.Empty;
            }
            else
            {
                return EpmWriterUtils.GetPropertyValueAsText(propertyValue);
            }
        }
    }
}
