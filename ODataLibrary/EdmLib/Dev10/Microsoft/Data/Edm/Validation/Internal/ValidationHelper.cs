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

using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Validation.Internal
{
    internal static class ValidationHelper
    {
        internal static bool IsEdmSystemNamespace(string namespaceName)
        {
            return (namespaceName == EdmConstants.TransientNamespace ||
                    namespaceName == EdmConstants.EdmNamespace ||
                    namespaceName == EdmConstants.ClrPrimitiveTypeNamespace);
        }

        internal static bool AreRelationshipEndsEqual(KeyValuePair<IEdmAssociationSet, IEdmAssociationSetEnd> left, KeyValuePair<IEdmAssociationSet, IEdmAssociationSetEnd> right)
        {
            return left.Key.Association.IsEquivalentTo(right.Key.Association) && left.Value.IsEquivalentTo(right.Value);
        }

        internal static bool TypeDefinesNewConcurrencyProperties(IEdmEntityType entityType)
        {
            foreach (var property in entityType.DeclaredStructuralProperties())
            {
                if (property.Type != null)
                {
                    if (property.ConcurrencyMode != EdmConcurrencyMode.None)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool TryGetTypeSubtypeAndSubtypeSet(IEdmEntityType entityType, Dictionary<IEdmEntityType, IEdmEntitySet> baseEntitySetTypes, out IEdmEntitySet set)
        {
            IEdmEntityType baseType = entityType.BaseEntityType();
            while (baseType != null)
            {
                if (baseEntitySetTypes.TryGetValue(baseType, out set))
                {
                    return true;
                }

                baseType = baseType.BaseEntityType();
            }

            set = null;
            return false;
        }

        internal static bool AddMemberNameToHashSet(IEdmNamedElement item, HashSetInternal<string> memberNameList, ValidationContext context, EdmErrorCode errorCode, string errorString, bool suppressError)
        {
            IEdmSchemaElement schemaElement = item as IEdmSchemaElement;
            string name = (schemaElement != null)? schemaElement.FullName(): item.Name;
            if (!memberNameList.Add(name))
            {
                if (!suppressError)
                {
                    context.AddError(item.Location(), errorCode, errorString);
                }

                return false;
            }

            return true;
        }

        internal static bool AllPropertiesAreNullable(IEnumerable<IEdmStructuralProperty> properties)
        {
            return properties.Where(p => !p.Type.IsNullable).Count() == 0;
        }

        internal static bool HasNullableProperty(IEnumerable<IEdmStructuralProperty> properties)
        {
            return properties.Where(p => p.Type.IsNullable).Count() > 0;
        }

        internal static bool PropertySetIsSubset(IEnumerable<IEdmStructuralProperty> set, IEnumerable<IEdmStructuralProperty> subset)
        {
            return subset.Except(set).Count() <= 0;
        }

        internal static bool PropertySetsAreEquivalent(IEnumerable<IEdmStructuralProperty> set1, IEnumerable<IEdmStructuralProperty> set2)
        {
            if(set1.Count() != set2.Count())
            {
                return false;
            }

            IEnumerator<IEdmStructuralProperty> set2Enum = set2.GetEnumerator();
            foreach (IEdmStructuralProperty prop1 in set1)
            {
                set2Enum.MoveNext();
                if (prop1 != set2Enum.Current)
                {
                    return false;
                }
            }

            return true;
        }

        // If there is something structurally wrong with the Referential Constraint, running rules on it would just be adding noise.
        internal static bool ReferentialConstraintReadyForValidation(IEdmReferentialConstraint referentialConstraint)
        {
            if (referentialConstraint.IsBad())
            {
                return false;
            }

            return !referentialConstraint.PrincipalEnd.IsBad();
        }
    }
}
