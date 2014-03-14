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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used to merge Atom metadata from EPM with those specified through annotations.
    /// </summary>
    internal static class ODataAtomWriterMetadataEpmMergeUtils
    {
        /// <summary>
        /// Merges custom and EPM ATOM metadata.
        /// </summary>
        /// <param name="customEntryMetadata">The custom ATOM metadata, or null if there were no custom ATOM metadata.</param>
        /// <param name="epmEntryMetadata">The EPM ATOM metadata, or null if there are no EPM mappings to syndication targets.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance configuring the writer.</param>
        /// <returns>The merged ATOM metadata to write to the output.</returns>
        /// <remarks>The merge means that if one of the sides has null, the other is used, otherwise if both are non-null
        /// we verify that the values are the same, otherwise we throw.</remarks>
        internal static AtomEntryMetadata MergeCustomAndEpmEntryMetadata(AtomEntryMetadata customEntryMetadata, AtomEntryMetadata epmEntryMetadata, ODataWriterBehavior writerBehavior)
        {
            DebugUtils.CheckNoExternalCallers();

            if (customEntryMetadata != null && writerBehavior.FormatBehaviorKind == ODataBehaviorKind.WcfDataServicesClient)
            {
                // ODataLib supports specifying ATOM metadata for entries even in the WCF DS client mode;
                // WCF DS itself will never do this but ODataLib clients using this mode might. We have
                // to convert potential Published and Updated values to the internal string properties
                // used in WCF DS client mode.
                if (customEntryMetadata.Updated.HasValue)
                {
                    customEntryMetadata.UpdatedString = ODataAtomConvert.ToAtomString(customEntryMetadata.Updated.Value);
                }

                if (customEntryMetadata.Published.HasValue)
                {
                    customEntryMetadata.PublishedString = ODataAtomConvert.ToAtomString(customEntryMetadata.Published.Value);
                }
            }

            AtomEntryMetadata simpleMergeResult;
            if (TryMergeIfNull(customEntryMetadata, epmEntryMetadata, out simpleMergeResult))
            {
                return simpleMergeResult;
            }

            // We will be modifying the EPM metadata adding the custom into it
            // The reason is that with EPM we can be sure that the enumerations are of type List (since they were created like that) and thus
            // we can safely add items into those without reallocating the collections.

            // Merge Title
            epmEntryMetadata.Title = MergeAtomTextValue(customEntryMetadata.Title, epmEntryMetadata.Title, "Title");

            // Merge Summary
            epmEntryMetadata.Summary = MergeAtomTextValue(customEntryMetadata.Summary, epmEntryMetadata.Summary, "Summary");

            // Merge Rights
            epmEntryMetadata.Rights = MergeAtomTextValue(customEntryMetadata.Rights, epmEntryMetadata.Rights, "Rights");

            if (writerBehavior.FormatBehaviorKind == ODataBehaviorKind.WcfDataServicesClient)
            {
                // Merge PublishedString
                epmEntryMetadata.PublishedString = MergeTextValue(customEntryMetadata.PublishedString, epmEntryMetadata.PublishedString, "PublishedString");

                // Merge UpdatedString
                epmEntryMetadata.UpdatedString = MergeTextValue(customEntryMetadata.UpdatedString, epmEntryMetadata.UpdatedString, "UpdatedString");
            }
            else
            {
                // Merge Published
                epmEntryMetadata.Published = MergeDateTimeValue(customEntryMetadata.Published, epmEntryMetadata.Published, "Published");

                // Merge Updated
                epmEntryMetadata.Updated = MergeDateTimeValue(customEntryMetadata.Updated, epmEntryMetadata.Updated, "Updated");
            }

            // Merge authors
            epmEntryMetadata.Authors = MergeSyndicationMapping<AtomPersonMetadata>(customEntryMetadata.Authors, epmEntryMetadata.Authors);

            // Merge contributors
            epmEntryMetadata.Contributors = MergeSyndicationMapping<AtomPersonMetadata>(customEntryMetadata.Contributors, epmEntryMetadata.Contributors);

            // Merge Categories
            epmEntryMetadata.Categories = MergeSyndicationMapping<AtomCategoryMetadata>(customEntryMetadata.Categories, epmEntryMetadata.Categories);

            // Merge Links
            epmEntryMetadata.Links = MergeSyndicationMapping<AtomLinkMetadata>(customEntryMetadata.Links, epmEntryMetadata.Links);

            // Source
            // Copy the source element over from custom metadata since EPM doesn't use it yet.
            Debug.Assert(epmEntryMetadata.Source == null, "Once EPM actually writes to source element, implement the merge with custom metadata here.");
            epmEntryMetadata.Source = customEntryMetadata.Source;

            return epmEntryMetadata;
        }

        /// <summary>
        /// Merges enumerations of person metadata.
        /// </summary>
        /// <param name="customValues">The enumeration of custom person metadata.</param>
        /// <param name="epmValues">The enumeration of EPM person metadata.</param>
        /// <typeparam name="T">The type of syndication mapping, one of AtomLinkMetadata, AtomCategoryMetadata, AtomPersonMetadata, </typeparam>
        /// <returns>The merged enumeration.</returns>
        private static IEnumerable<T> MergeSyndicationMapping<T>(
            IEnumerable<T> customValues, 
            IEnumerable<T> epmValues)
        {
            IEnumerable<T> simpleMergeResult;
            if (TryMergeIfNull(customValues, epmValues, out simpleMergeResult))
            {
                return simpleMergeResult;
            }

            // The EPM must always have the syndication mappings as a List<T> so let's use that list to do the merge in.
            List<T> epmList = (List<T>)epmValues;

            // We must enumerate the custom values exactly once (we guarantee that), so walk those first.
            foreach (T mapping in customValues)
            {
                // There's no reliable way to correlate syndication mapping from one list to a syndication mapping in the other list.
                // So instead of trying to be clever and cause confusion we will simply merge the lists by adding one to the other.

                // Add that mapping to the list
                epmList.Add(mapping);
            }

            return epmList;
        }

        /// <summary>
        /// Merges ATOM text values.
        /// </summary>
        /// <param name="customValue">The custom value.</param>
        /// <param name="epmValue">The EPM value.</param>
        /// <param name="propertyName">The name of the ATOM property which holds the text value, used for error reporting.</param>
        /// <returns>The merged ATOM text value.</returns>
        private static AtomTextConstruct MergeAtomTextValue(AtomTextConstruct customValue, AtomTextConstruct epmValue, string propertyName)
        {
            AtomTextConstruct simpleMergeResult;
            if (TryMergeIfNull(customValue, epmValue, out simpleMergeResult))
            {
                return simpleMergeResult;
            }

            if (customValue.Kind != epmValue.Kind)
            {
                throw new ODataException(Strings.ODataAtomMetadataEpmMerge_TextKindConflict(propertyName, customValue.Kind.ToString(), epmValue.Kind.ToString()));
            }

            if (string.CompareOrdinal(customValue.Text, epmValue.Text) != 0)
            {
                throw new ODataException(Strings.ODataAtomMetadataEpmMerge_TextValueConflict(propertyName, customValue.Text, epmValue.Text));
            }

            return epmValue;
        }

        /// <summary>
        /// Merges text values.
        /// </summary>
        /// <param name="customValue">The custom value.</param>
        /// <param name="epmValue">The EPM value.</param>
        /// <param name="propertyName">The name of the ATOM property which holds the text value, used for error reporting.</param>
        /// <returns>The merged text value.</returns>
        private static string MergeTextValue(string customValue, string epmValue, string propertyName)
        {
            string simpleMergeResult;
            if (TryMergeIfNull(customValue, epmValue, out simpleMergeResult))
            {
                return simpleMergeResult;
            }

            if (string.CompareOrdinal(customValue, epmValue) != 0)
            {
                throw new ODataException(Strings.ODataAtomMetadataEpmMerge_TextValueConflict(propertyName, customValue, epmValue));
            }

            return epmValue;
        }

        /// <summary>
        /// Merges date time offset values.
        /// </summary>
        /// <param name="customValue">The custom value.</param>
        /// <param name="epmValue">The EPM value.</param>
        /// <param name="propertyName">The name of the ATOM property which holds the value, used for error reporting.</param>
        /// <returns>The merged date time offset value.</returns>
        private static DateTimeOffset? MergeDateTimeValue(DateTimeOffset? customValue, DateTimeOffset? epmValue, string propertyName)
        {
            DateTimeOffset? simpleMergeResult;
            if (TryMergeIfNull(customValue, epmValue, out simpleMergeResult))
            {
                return simpleMergeResult;
            }

            if (customValue != epmValue)
            {
                throw new ODataException(Strings.ODataAtomMetadataEpmMerge_TextValueConflict(propertyName, customValue.ToString(), epmValue.ToString()));
            }

            return epmValue;
        }

        /// <summary>
        /// Tries to merge custom and EPM values if one of them is null.
        /// </summary>
        /// <typeparam name="T">The type of the value to merge.</typeparam>
        /// <param name="customValue">The custom value.</param>
        /// <param name="epmValue">The EPM value.</param>
        /// <param name="result">The merge value if the merge was possible.</param>
        /// <returns>true if one of the values was null and thus the other was returned in <paramref name="result"/>;
        /// false if both were not null and thus full merge will have to be performed.</returns>
        private static bool TryMergeIfNull<T>(T customValue, T epmValue, out T result) where T : class
        {
            if (customValue == null)
            {
                result = epmValue;
                return true;
            }

            if (epmValue == null)
            {
                result = customValue;
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Tries to merge custom and EPM values if one of them is null.
        /// </summary>
        /// <typeparam name="T">The type of the value to merge.</typeparam>
        /// <param name="customValue">The custom value.</param>
        /// <param name="epmValue">The EPM value.</param>
        /// <param name="result">The merge value if the merge was possible.</param>
        /// <returns>true if one of the values was null and thus the other was returned in <paramref name="result"/>;
        /// false if both were not null and thus full merge will have to be performed.</returns>
        private static bool TryMergeIfNull<T>(Nullable<T> customValue, Nullable<T> epmValue, out Nullable<T> result) where T : struct
        {
            if (customValue == null)
            {
                result = epmValue;
                return true;
            }

            if (epmValue == null)
            {
                result = customValue;
                return true;
            }

            result = null;
            return false;
        }
    }
}
