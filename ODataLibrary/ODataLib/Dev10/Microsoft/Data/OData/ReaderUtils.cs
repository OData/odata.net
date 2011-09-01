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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for reading OData content.
    /// </summary>
    internal static class ReaderUtils
    {
        /// <summary>
        /// Empty list of association links.
        /// </summary>
        private static readonly ReadOnlyEnumerable<ODataAssociationLink> EmptyAssociationLinksList = new ReadOnlyEnumerable<ODataAssociationLink>();

        /// <summary>
        /// Creates a new <see cref="ODataEntry"/> instance to return to the user.
        /// </summary>
        /// <returns>The newly created entry.</returns>
        /// <remarks>The method populates the Properties property with an empty read only enumeration.</remarks>
        internal static ODataEntry CreateNewEntry()
        {
            DebugUtils.CheckNoExternalCallers();

            return new ODataEntry
            {
                Properties = new ReadOnlyEnumerable<ODataProperty>(),
                AssociationLinks = EmptyAssociationLinksList,
            };
        }

        /// <summary>
        /// Given the value of the specified collection as a list which can be modified.
        /// This methods returns the internal List of the collection.
        /// </summary>
        /// <typeparam name="T">The type of the item in the collection get.</typeparam>
        /// <param name="collection">The value of the collection to get the list for.</param>
        /// <param name="collectionName">The name of the collection to report in case there's an error.</param>
        /// <returns>The underlying list to modify.</returns>
        internal static List<T> GetSourceListOfEnumerable<T>(IEnumerable<T> collection, string collectionName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(collection != null, "collection != null");
            Debug.Assert(!string.IsNullOrEmpty(collectionName), "collectionName must not be null or empty.");

            ReadOnlyEnumerable<T> readonlyCollection = collection as ReadOnlyEnumerable<T>;
            if (readonlyCollection == null)
            {
                throw new ODataException(Strings.ReaderUtils_EnumerableModified(collectionName));
            }

            List<T> collectionList = readonlyCollection.SourceList;
            Debug.Assert(collectionList != null, "collectionList != null");

            return collectionList;
        }

        /// <summary>
        /// Given the value of the Properties collection previously populated by CreateNewEntry
        /// this methods returns the internal List of properties which can be modified to add new properties.
        /// </summary>
        /// <param name="properties">The value of the Properties property to get the list for.</param>
        /// <returns>The underlying list of properties to modify.</returns>
        internal static List<ODataProperty> GetPropertiesList(IEnumerable<ODataProperty> properties)
        {
            DebugUtils.CheckNoExternalCallers();
            return GetSourceListOfEnumerable(properties, "Properties");
        }

        /// <summary>
        /// Adds a property to the Properties collection which must have been previously populated by CreateNewEntry.
        /// </summary>
        /// <param name="properties">The value of the Properties property to add the property to.</param>
        /// <param name="propertyToAdd">The property to add.</param>
        internal static void AddPropertyToPropertiesList(IEnumerable<ODataProperty> properties, ODataProperty propertyToAdd)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(propertyToAdd != null, "propertyToAdd != null");
            ReaderUtils.GetPropertiesList(properties).Add(propertyToAdd);
        }

        /// <summary>
        /// Adds an association link to an entry.
        /// </summary>
        /// <param name="entry">The entry to add the association link to.</param>
        /// <param name="associationLink">The association link to add.</param>
        internal static void AddAssociationLinkToEntry(ODataEntry entry, ODataAssociationLink associationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(associationLink != null, "associationLink != null");

            if (object.ReferenceEquals(entry.AssociationLinks, EmptyAssociationLinksList))
            {
                entry.AssociationLinks = new ReadOnlyEnumerable<ODataAssociationLink>();
            }

            ReaderUtils.GetSourceListOfEnumerable(entry.AssociationLinks, "AssociationLinks").Add(associationLink);
        }
    }
}
