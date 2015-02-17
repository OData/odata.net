//---------------------------------------------------------------------
// <copyright file="ODataUriParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// A collection of helper methods for parsing OData uri's and segments
    /// </summary>
    public static class ODataUriParser
    {
        /// <summary>
        /// Parses a collection of segment paths using the format expected for the '$expand' and '$select' query options
        /// </summary>
        /// <param name="type">The entity type the paths start from</param>
        /// <param name="toParse">The string to parse</param>
        /// <returns>The parsed collection of segment paths</returns>
        public static ODataUriSegmentPathCollection ParseSegmentPathCollection(EntityType type, string toParse)
        {
            ExceptionUtilities.CheckArgumentNotNull(toParse, "toParse");

            ODataUriSegmentPathCollection segmentPathCollection = new ODataUriSegmentPathCollection();
            foreach (string path in toParse.Trim().Split(','))
            {
                IList<ODataUriSegment> segmentPath = new List<ODataUriSegment>();

                IEnumerable<MemberProperty> possibleProperties = type.AllProperties;
                IEnumerable<NavigationProperty> possibleNavigations = type.AllNavigationProperties;

                foreach (string segment in path.Trim().Split('/'))
                {
                    EntityType matchingType;
                    if (TryGetEntityTypeInHierarchy(type, segment, out matchingType))
                    {
                        segmentPath.Add(new EntityTypeSegment(matchingType));
                        continue;
                    }

                    MemberProperty property = null;
                    if (possibleProperties != null)
                    {
                        property = possibleProperties.SingleOrDefault(p => p.Name == segment);
                    }

                    NavigationProperty navigation = null;
                    if (possibleNavigations != null)
                    {
                        navigation = possibleNavigations.SingleOrDefault(p => p.Name == segment);
                    }

                    if (property == null && navigation == null)
                    {
                        possibleProperties = null;
                        SystemSegment systemSegment;
                        if (SystemSegment.TryGet(segment, out systemSegment))
                        {
                            segmentPath.Add(systemSegment);
                        }
                        else
                        {
                            segmentPath.Add(new UnrecognizedSegment(segment));
                        }
                    }
                    else if (property != null)
                    {
                        possibleNavigations = null;

                        ComplexDataType complexType = property.PropertyType as ComplexDataType;
                        if (complexType != null)
                        {
                            possibleProperties = complexType.Definition.Properties;
                        }
                        else
                        {
                            possibleProperties = null;
                            possibleNavigations = null;
                        }

                        segmentPath.Add(new PropertySegment(property));
                    }
                    else
                    {
                        possibleProperties = navigation.ToAssociationEnd.EntityType.AllProperties;
                        possibleNavigations = navigation.ToAssociationEnd.EntityType.AllNavigationProperties;
                        segmentPath.Add(new NavigationSegment(navigation));
                    }
                }

                segmentPathCollection.Add(segmentPath);
            }

            return segmentPathCollection;
        }

        private static bool TryGetEntityTypeInHierarchy(EntityType type, string fullName, out EntityType matchingType)
        {
            if (type.Model == null)
            {
                matchingType = null;
                return false;
            }

            matchingType = type.Model.EntityTypes.Where(t => t.IsKindOf(type) || t == type).SingleOrDefault(t => t.FullName == fullName);
            return matchingType != null;
        }
    }
}
