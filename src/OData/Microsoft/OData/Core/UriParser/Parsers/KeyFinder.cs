//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Find the key from a previous key segment and use it to construct the current key
    /// </summary>
    internal static class KeyFinder
    {
        /// <summary>
        /// Find any related keys from the parent key segment, if it exists, and add them to the raw key values that
        /// we already have from the uri.
        /// </summary>
        /// <param name="rawKeyValuesFromUri">The raw key values as we've parsed them from the uri.</param>
        /// <param name="targetEntityKeyProperties">The list of key properties on the target entity.</param>
        /// <param name="currentNavigationProperty">The current navigation property that we're trying to follow using the raw key values</param>
        /// <param name="keySegmentOfParentEntity">The key segment of the parent entity in this path, if it exists. Null otherwise</param>
        /// <returns>A new SegmentArgumentParser with any keys that were found added to its list of NamedValues.</returns>
        /// <throws>Thorws if the input currentNavigationProperty is null.</throws>
        public static SegmentArgumentParser FindAndUseKeysFromRelatedSegment(SegmentArgumentParser rawKeyValuesFromUri, IEnumerable<IEdmStructuralProperty> targetEntityKeyProperties, IEdmNavigationProperty currentNavigationProperty, KeySegment keySegmentOfParentEntity)
        {
            ExceptionUtils.CheckArgumentNotNull(currentNavigationProperty, "currentNavigationProperty");
            ExceptionUtils.CheckArgumentNotNull(rawKeyValuesFromUri, "rawKeyValuesFromUri");

            ReadOnlyCollection<IEdmStructuralProperty> targetKeyPropertyList = targetEntityKeyProperties != null ? new ReadOnlyCollection<IEdmStructuralProperty>(targetEntityKeyProperties.ToList()) : new ReadOnlyCollection<IEdmStructuralProperty>(new List<IEdmStructuralProperty>());

            // should only get here if the number of raw parameters from the uri is different than the number of key properties for the target entity.
            Debug.Assert(rawKeyValuesFromUri.ValueCount < targetKeyPropertyList.Count(), "rawKeyValuesFromUri.ValueCount < targetEntityKeyProperties.Count()");

            // if the raw key from the uri has positional values, there must be only one of them
            // its important to cache this value here because we'll change it when we add new 
            // named values below (the implementation of AreValuesNamed is just namedValues !=null)
            bool hasPositionalValues = !rawKeyValuesFromUri.AreValuesNamed;
            if (hasPositionalValues && rawKeyValuesFromUri.ValueCount > 1)
            {
                return rawKeyValuesFromUri;
            }

            if (keySegmentOfParentEntity == null)
            {
                return rawKeyValuesFromUri;
            }

            // TODO: p2 merge the below 2 pieces of codes
            // find out if any target entity key properties have referential constraints that link them to the previous rawKeyValuesFromUri.
            List<EdmReferentialConstraintPropertyPair> keysFromReferentialIntegrityConstraint = ExtractMatchingPropertyPairsFromNavProp(currentNavigationProperty, targetKeyPropertyList).ToList();

            foreach (EdmReferentialConstraintPropertyPair keyFromReferentialIntegrityConstraint in keysFromReferentialIntegrityConstraint)
            {
                KeyValuePair<string, object> valueFromParent = keySegmentOfParentEntity.Keys.SingleOrDefault(x => x.Key == keyFromReferentialIntegrityConstraint.DependentProperty.Name);
                if (valueFromParent.Key != null)
                {
                    // if the key from the referential integrity constraint is one of the target key properties
                    // and that key property isn't already populated in the raw key values from the uri, then
                    // we set that value to the value from the parent key segment.
                    if (targetKeyPropertyList.Any(x => x.Name == keyFromReferentialIntegrityConstraint.PrincipalProperty.Name))
                    {
                        rawKeyValuesFromUri.AddNamedValue(
                            keyFromReferentialIntegrityConstraint.PrincipalProperty.Name,
                            valueFromParent.Value.ToString());
                    }
                }
            }

            // also need to look to see if any nav props exist in the target set that refer back to this same set, which might have 
            // referential constraints also.
            keysFromReferentialIntegrityConstraint.Clear();
            IEdmNavigationProperty reverseNavProp = currentNavigationProperty.Partner;
            if (reverseNavProp != null)
            {
                keysFromReferentialIntegrityConstraint.AddRange(ExtractMatchingPropertyPairsFromReversedNavProp(reverseNavProp, targetKeyPropertyList));
            }

            foreach (EdmReferentialConstraintPropertyPair keyFromReferentialIntegrityConstraint in keysFromReferentialIntegrityConstraint)
            {
                KeyValuePair<string, object> valueFromParent = keySegmentOfParentEntity.Keys.SingleOrDefault(x => x.Key == keyFromReferentialIntegrityConstraint.PrincipalProperty.Name);
                if (valueFromParent.Key != null)
                {
                    // if the key from the referential integrity constraint is one of the target key properties
                    // and that key property isn't already populated in the raw key values from the uri, then
                    // we set that value to the value from the parent key segment.
                    if (targetKeyPropertyList.Any(x => x.Name == keyFromReferentialIntegrityConstraint.DependentProperty.Name))
                    {
                        rawKeyValuesFromUri.AddNamedValue(
                            keyFromReferentialIntegrityConstraint.DependentProperty.Name,
                            valueFromParent.Value.ToString());
                    }
                }
            }

            // if we had a positional value before, then we need to add that value as a new named value.
            // the name that we choose will be the only value from the target entity key properties
            // that isn't already set in the NamedValues list.
            if (hasPositionalValues)
            {
                if (rawKeyValuesFromUri.NamedValues != null)
                {
                    List<IEdmStructuralProperty> unassignedProperties = targetKeyPropertyList.Where(x => !rawKeyValuesFromUri.NamedValues.ContainsKey(x.Name)).ToList();

                    if (unassignedProperties.Count == 1)
                    {
                        rawKeyValuesFromUri.AddNamedValue(unassignedProperties[0].Name, rawKeyValuesFromUri.PositionalValues[0]);
                    }
                    else
                    {
                        return rawKeyValuesFromUri;
                    }

                    // clear out the positional value so that we keep a consistent state in the 
                    // raw keys from uri.
                    rawKeyValuesFromUri.PositionalValues.Clear();
                }
                else
                {
                    return rawKeyValuesFromUri;
                }
            }

            return rawKeyValuesFromUri;
        }

        /// <summary>
        /// Find any referential constraint property pairs in a given nav prop that match any of the provided key properties.
        /// </summary>
        /// <param name="currentNavigationProperty">The navigation property to search</param>
        /// <param name="targetKeyPropertyList">The list of key properties that we're searching for</param>
        /// <returns>All referential constraint property pairs that match the list of target key properties.</returns>
        private static IEnumerable<EdmReferentialConstraintPropertyPair> ExtractMatchingPropertyPairsFromNavProp(IEdmNavigationProperty currentNavigationProperty, IEnumerable<IEdmStructuralProperty> targetKeyPropertyList)
        {
            if (currentNavigationProperty != null && currentNavigationProperty.ReferentialConstraint != null)
            {
                // currentNavigationProperty.ReferentialConstraint has mapping of source property(dependent)-> target referencedProperty(principal)
                // so check PrincipalProperty against targetKeyPropertyList
                return currentNavigationProperty.ReferentialConstraint.PropertyPairs.Where(x => targetKeyPropertyList.Any(y => y == x.PrincipalProperty));
            }
            else
            {
                return new List<EdmReferentialConstraintPropertyPair>();
            }
        }

        /// <summary>
        /// Find any referential constraint property pairs in a given reversed nav prop that match any of the provided key properties.
        /// </summary>
        /// <param name="currentNavigationProperty">The navigation property to search</param>
        /// <param name="targetKeyPropertyList">The list of key properties that we're searching for</param>
        /// <returns>All referential constraint property pairs that match the list of target key properties.</returns>
        private static IEnumerable<EdmReferentialConstraintPropertyPair> ExtractMatchingPropertyPairsFromReversedNavProp(IEdmNavigationProperty currentNavigationProperty, IEnumerable<IEdmStructuralProperty> targetKeyPropertyList)
        {
            if (currentNavigationProperty != null && currentNavigationProperty.ReferentialConstraint != null)
            {
                // currentNavigationProperty.ReferentialConstraint has mapping of target property(dependent)-> source referencedProperty(principal)
                // so check DependentProperty against targetKeyPropertyList
                return currentNavigationProperty.ReferentialConstraint.PropertyPairs.Where(x => targetKeyPropertyList.Any(y => y == x.DependentProperty));
            }
            else
            {
                return new List<EdmReferentialConstraintPropertyPair>();
            }
        }
    }
}
