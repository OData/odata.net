//---------------------------------------------------------------------
// <copyright file="ReferentialConstraintsResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Data
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;

    /// <summary>
    /// Resolves referential constraint in the <see cref="EntityContainerData"/>.
    /// </summary>
    [ImplementationName(typeof(IReferentialConstraintsResolver), "Default")]
    public class ReferentialConstraintsResolver : IReferentialConstraintsResolver
    {
        /// <summary>
        ///  Resolves referential constraint in the <see cref="EntityContainerData"/>.
        /// </summary>
        /// <param name="data">The entity container data where to resolve referential constraints.</param>
        public void ResolveReferentialConstraints(EntityContainerData data)
        {
            var resolver = new ConstraintsResolver(data);
            resolver.Resolve();
        }

        /// <summary>
        /// Resolves referential constraints for the entity container data.
        /// </summary>
        private class ConstraintsResolver
        {
            private EntityContainerData data;
            private Dictionary<EntitySet, List<PropertyWithConstraints>> entitySetToConstraintsMap;
            private List<KeyValuePair<EntitySetDataRow, MemberProperty>> resolved;
            private List<KeyValuePair<EntitySetDataRow, MemberProperty>> visited;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConstraintsResolver"/> class.
            /// </summary>
            /// <param name="data">The entity container data to resolve referential constraints.</param>
            public ConstraintsResolver(EntityContainerData data)
            {
                this.data = data;
                this.Initialize();
            }

            /// <summary>
            /// Resolves referential constraints.
            /// </summary>
            public void Resolve()
            {
                this.resolved = new List<KeyValuePair<EntitySetDataRow, MemberProperty>>();
                this.visited = new List<KeyValuePair<EntitySetDataRow, MemberProperty>>();

                foreach (var kv in this.entitySetToConstraintsMap)
                {
                    EntitySet entitySet = kv.Key;
                    List<PropertyWithConstraints> propertiesWithConstraints = kv.Value;
                    foreach (EntitySetDataRow row in this.data[entitySet].Rows)
                    {
                        foreach (PropertyWithConstraints p in propertiesWithConstraints)
                        {
                            this.ResolveConstraints(row, p);
                        }
                    }
                }
            }

            private static string GetReferentialConstraintCycleDescription(List<KeyValuePair<EntitySetDataRow, MemberProperty>> cycle, int indexCycleStartsWith)
            {
                var builder = new StringBuilder();
                string format = "{{EntitySet='{0}', EntityType='{1}', Property='{2}'}}";
                for (int i = indexCycleStartsWith; i < cycle.Count; i++)
                {
                    KeyValuePair<EntitySetDataRow, MemberProperty> item = cycle[i];
                    builder.AppendFormat(CultureInfo.InvariantCulture, format, item.Key.Parent.EntitySet.Name, item.Key.EntityType.Name, item.Value.Name);
                    builder.Append(" <-- ");
                }

                KeyValuePair<EntitySetDataRow, MemberProperty> lastItem = cycle[indexCycleStartsWith];
                builder.AppendFormat(format, lastItem.Key.Parent.EntitySet.Name, lastItem.Key.EntityType.Name, lastItem.Value.Name);
                builder.Append(".");

                return builder.ToString();
            }

            private static string GetOverlappingForeignKeyDescription(IEnumerable<PropertyConstraint> constraints)
            {
                var builder = new StringBuilder();

                PropertyConstraint firstConstraint = constraints.First();
                builder.AppendFormat(CultureInfo.InvariantCulture, "Dependent {{EntitySet = {0}, Property = {1}}} <--- Contributing principals: ", firstConstraint.DependentEntitySet.Name, firstConstraint.DependentProperty.Name);

                string separator = string.Empty;
                foreach (PropertyConstraint constraint in constraints)
                {
                    builder.Append(separator);
                    builder.AppendFormat(CultureInfo.InvariantCulture, "{{EntitySet = {0}, Property = {1}}}", constraint.PrincipalEntitySet.Name, constraint.PrincipalProperty.Name);
                    separator = ", ";
                }

                builder.Append(".");

                return builder.ToString();
            }

            private void Initialize()
            {
                this.entitySetToConstraintsMap = new Dictionary<EntitySet, List<PropertyWithConstraints>>();

                foreach (AssociationSet associationSet in this.data.EntityContainer.AssociationSets
                    .Where(s => s.AssociationType.ReferentialConstraint != null && this.data[s].Rows.Count > 0))
                {
                    ReferentialConstraint refConstraint = associationSet.AssociationType.ReferentialConstraint;

                    EntitySet dependentEntitySet = associationSet.Ends.Where(e => e.AssociationEnd == refConstraint.DependentAssociationEnd).Select(e => e.EntitySet).Single();

                    List<PropertyWithConstraints> propertiesWithConstraints;
                    if (!this.entitySetToConstraintsMap.TryGetValue(dependentEntitySet, out propertiesWithConstraints))
                    {
                        propertiesWithConstraints = new List<PropertyWithConstraints>();
                        this.entitySetToConstraintsMap[dependentEntitySet] = propertiesWithConstraints;
                    }

                    for (int i = 0; i < refConstraint.DependentProperties.Count; i++)
                    {
                        MemberProperty dependentProperty = refConstraint.DependentProperties[i];
                        PropertyWithConstraints propertyWithConstraints = propertiesWithConstraints.Where(p => p.Property == dependentProperty).SingleOrDefault();

                        if (propertyWithConstraints == null)
                        {
                            propertyWithConstraints = new PropertyWithConstraints(dependentProperty);
                            propertiesWithConstraints.Add(propertyWithConstraints);
                        }

                        propertyWithConstraints.AddConstraint(new PropertyConstraint(associationSet, i));
                    }
                }
            }

            private bool ResolveConstraintValue(EntitySetDataRow dependentRow, MemberProperty property, PropertyConstraint constraint, out object value)
            {
                value = null;

                List<AssociationSetDataRow> associationRows = this.data[constraint.AssociationSet].Rows.Where(
                            r => ReferenceEquals(r.GetRoleKey(constraint.DependentRoleName), dependentRow.Key)).ToList();

                if (associationRows.Count == 0)
                {
                    return false;
                }

                if (associationRows.Count > 1)
                {
                    throw new TaupoInvalidOperationException(string.Format(
                     CultureInfo.InvariantCulture,
                     "Multiplicity constraint violated: the association set '{0}' has multiple rows with the same role key for the dependent role '{1}'.",
                     constraint.AssociationSet.Name,
                     constraint.DependentRoleName));
                }

                EntityDataKey principalKey = associationRows[0].GetRoleKey(constraint.PrincipalRoleName);

                EntitySetDataRow principalRow = this.data[constraint.PrincipalEntitySet].Rows.Where(r => ReferenceEquals(r.Key, principalKey)).SingleOrDefault();

                if (principalRow != null)
                {
                    List<PropertyConstraint> constraints;
                    if (this.TryGetConstraints(principalRow, constraint.PrincipalProperty, out constraints))
                    {
                        var item = new KeyValuePair<EntitySetDataRow, MemberProperty>(dependentRow, property);
                        this.visited.Add(item);
                        value = this.ResolvePropertyValue(principalRow, constraint.PrincipalProperty, constraints);
                        this.visited.Remove(item);
                    }
                    else
                    {
                        value = principalRow[constraint.PrincipalProperty.Name];
                    }
                }
                else if (constraint.PrincipalProperty.IsPrimaryKey)
                {
                    value = principalKey[constraint.PrincipalProperty.Name];
                }

                return true;
            }

            private void ResolveConstraints(EntitySetDataRow row, PropertyWithConstraints propertyWithConstraints)
            {
                List<PropertyConstraint> constraints = propertyWithConstraints.GetConstraints(row.EntityType);

                if (constraints.Count > 0)
                {
                    this.ResolvePropertyValue(row, propertyWithConstraints.Property, constraints);
                }
            }

            private object ResolvePropertyValue(EntitySetDataRow row, MemberProperty property, List<PropertyConstraint> constraints)
            {
                if (this.resolved.Any(kv => kv.Key == row && kv.Value == property))
                {
                    return row[property.Name];
                }

                KeyValuePair<EntitySetDataRow, MemberProperty> sameItem = this.visited.Where(kv => kv.Key == row && kv.Value == property).SingleOrDefault();
                if (sameItem.Key != null)
                {
                    int indexCycleStartsWith = this.visited.IndexOf(sameItem);
                    List<object> values = this.visited.Where((kv, i) => i >= indexCycleStartsWith).Select(kv => kv.Key[kv.Value.Name]).Distinct(ValueComparer.Instance).ToList();
                    if (values.Count > 1)
                    {
                        throw new TaupoInvalidOperationException("Referential constraint cycle detected: " + GetReferentialConstraintCycleDescription(this.visited, indexCycleStartsWith));
                    }

                    return values[0];
                }

                var candidates = new List<object>();
                foreach (PropertyConstraint constraint in constraints)
                {
                    object candidate;
                    if (this.ResolveConstraintValue(row, property, constraint, out candidate))
                    {
                        candidates.Add(candidate);
                    }
                }

                if (candidates.Distinct(ValueComparer.Instance).Count() > 1)
                {
                    throw new TaupoInvalidOperationException("Overlapping foreign keys with conflicting values detected: " + GetOverlappingForeignKeyDescription(constraints));
                }

                object value;
                if (candidates.Count > 0)
                {
                    value = candidates[0];
                    if (!ValueComparer.Instance.Equals(value, row[property.Name]))
                    {
                        row[property.Name] = value;
                    }
                }
                else
                {
                    value = row[property.Name];
                }

                this.resolved.Add(new KeyValuePair<EntitySetDataRow, MemberProperty>(row, property));

                return value;
            }

            private bool TryGetConstraints(EntitySetDataRow row, MemberProperty property, out List<PropertyConstraint> constraints)
            {
                constraints = null;

                List<PropertyWithConstraints> propertiesWithConstraints;
                if (this.entitySetToConstraintsMap.TryGetValue(row.Parent.EntitySet, out propertiesWithConstraints))
                {
                    constraints = propertiesWithConstraints
                        .Where(p => p.Property == property)
                        .Select(p => p.GetConstraints(row.EntityType)).SingleOrDefault();

                    if (constraints != null && constraints.Count == 0)
                    {
                        constraints = null;
                    }
                }

                return constraints != null;
            }
        }

        /// <summary>
        /// Represents property constraint.
        /// </summary>
        private class PropertyConstraint
        {
            private int ordinal;

            /// <summary>
            /// Initializes a new instance of the <see cref="PropertyConstraint"/> class.
            /// </summary>
            /// <param name="associationSet">The association set.</param>
            /// <param name="ordinal">The ordinal of the constraint in the <see cref="ReferentialConstraint"/> for the association set.</param>
            public PropertyConstraint(AssociationSet associationSet, int ordinal)
            {
                this.AssociationSet = associationSet;
                this.ordinal = ordinal;

                AssociationSetEnd principalEnd = associationSet.Ends.Where(e => e.AssociationEnd == associationSet.AssociationType.ReferentialConstraint.PrincipalAssociationEnd).Single();
                
                this.PrincipalEntitySet = principalEnd.EntitySet;
                this.DependentEntitySet = associationSet.GetOtherEnd(principalEnd).EntitySet;
            }

            /// <summary>
            /// Gets the association set this constraint applies to.
            /// </summary>
            /// <value>The association set.</value>
            public AssociationSet AssociationSet { get; private set; }

            /// <summary>
            /// Gets the principal property of the constraint.
            /// </summary>
            /// <value>The principal property.</value>
            public MemberProperty PrincipalProperty
            {
                get { return this.AssociationSet.AssociationType.ReferentialConstraint.PrincipalProperties[this.ordinal]; }
            }

            /// <summary>
            /// Gets the dependent property of the constraint.
            /// </summary>
            /// <value>The dependent property.</value>
            public MemberProperty DependentProperty
            {
                get { return this.AssociationSet.AssociationType.ReferentialConstraint.DependentProperties[this.ordinal]; }
            }

            /// <summary>
            /// Gets the principal entity set.
            /// </summary>
            /// <value>The principal entity set.</value>
            public EntitySet PrincipalEntitySet { get; private set; }

            /// <summary>
            /// Gets the dependent entity set.
            /// </summary>
            /// <value>The dependent entity set.</value>
            public EntitySet DependentEntitySet { get; private set; }

            /// <summary>
            /// Gets the type of the dependent entity.
            /// </summary>
            /// <value>The type of the dependent entity.</value>
            public EntityType DependentEntityType
            {
                get { return this.AssociationSet.AssociationType.ReferentialConstraint.DependentAssociationEnd.EntityType; }
            }

            /// <summary>
            /// Gets the name of the dependent role.
            /// </summary>
            /// <value>The name of the dependent role.</value>
            public string DependentRoleName
            {
                get { return this.AssociationSet.AssociationType.ReferentialConstraint.DependentAssociationEnd.RoleName; }
            }

            /// <summary>
            /// Gets the name of the principal role.
            /// </summary>
            /// <value>The name of the principal role.</value>
            public string PrincipalRoleName
            {
                get { return this.AssociationSet.AssociationType.ReferentialConstraint.PrincipalAssociationEnd.RoleName; }
            }
        }

        /// <summary>
        /// Represents a property with constraints.
        /// </summary>
        private class PropertyWithConstraints
        {
            private List<PropertyConstraint> constraints;

            /// <summary>
            /// Initializes a new instance of the <see cref="PropertyWithConstraints"/> class.
            /// </summary>
            /// <param name="property">The property that has constraints.</param>
            public PropertyWithConstraints(MemberProperty property)
            {
                this.Property = property;
                this.constraints = new List<PropertyConstraint>();
            }

            /// <summary>
            /// Gets the property.
            /// </summary>
            /// <value>The property.</value>
            public MemberProperty Property { get; private set; }

            /// <summary>
            /// Adds the constraint for the property.
            /// </summary>
            /// <param name="constraint">The constraint.</param>
            public void AddConstraint(PropertyConstraint constraint)
            {
                this.constraints.Add(constraint);
            }

            /// <summary>
            /// Gets all constraints for the property for the specified dependent entity type.
            /// </summary>
            /// <param name="dependentEntityType">Type of the dependent entity.</param>
            /// <returns>All constraints for the property for the specified dependent entity type.</returns>
            public List<PropertyConstraint> GetConstraints(EntityType dependentEntityType)
            {
                return this.constraints.Where(c => dependentEntityType.IsKindOf(c.DependentEntityType)).ToList();
            }
        }
    }
}