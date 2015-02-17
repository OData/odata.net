//---------------------------------------------------------------------
// <copyright file="CustomMetadataWorkspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Microsoft.OData.Client;
using System.Collections;
using System.Data.Test.Astoria.CustomData.Runtime;

namespace System.Data.Test.Astoria.CustomData.Metadata
{
    // --- CODE SNIPPET START ---

    //
    // CustomMetadataException class
    //
    #region CustomMetadataException class

    public class CustomMetadataException : Exception
    {
        #region Constructors

        public CustomMetadataException()
        {
        }

        public CustomMetadataException(string message)
            : base(message)
        {
        }

        #endregion
    }

    #endregion

    //
    // CustomMetadataWorkspace class
    //
    #region CustomMetadataWorkspace class

    public class CustomMetadataWorkspace
    {
        #region Private fields

        private IDictionary<string, CustomEntitySetType> _entitySets
                                                        = new Dictionary<string, CustomEntitySetType>();

        private IDictionary<Type, CustomEntityType> _clrObjectTypeMap
                                                        = new Dictionary<Type, CustomEntityType>();

        #endregion

        #region Constructor

        public CustomMetadataWorkspace()
        {
        }

        #endregion

        #region Properties

        public ICollection<CustomEntityType> EntityTypes
        {
            get { return _clrObjectTypeMap.Values; }
        }

        public ICollection<CustomEntitySetType> EntitySets
        {
            get { return _entitySets.Values; }
        }

        #endregion

        #region Methods

        public void ValidateAndLock()
        {
        }

        public bool TryGetEntitySet(string name, out CustomEntitySetType entitySet)
        {
            CustomUtils.CheckArgumentNotNull(name, "name");
            if (_entitySets.TryGetValue(name, out entitySet))
            {
                return true;
            }
            return false;
        }

        public CustomEntitySetType GetEntitySet(string name)
        {
            CustomEntitySetType entitySet;
            if (TryGetEntitySet(name, out entitySet))
            {
                return entitySet;
            }

            throw new CustomMetadataException(String.Format("EntitySet '{0}' does not not exist.", name));
        }

        public bool TryGetEntityType(Type clrType, out CustomEntityType entityType)
        {
            CustomUtils.CheckArgumentNotNull(clrType, "clrType");
            if (_clrObjectTypeMap.TryGetValue(clrType, out entityType))
            {
                return true;
            }
            return false;
        }

        public CustomEntityType GetEntityType(Type clrType)
        {
            CustomEntityType entityType;
            if (TryGetEntityType(clrType, out entityType))
            {
                return entityType;
            }
            
            throw new CustomMetadataException(String.Format(
                            "No EntityType mapping has been registred for CLR object type '{0}'.",
                            clrType.Name
                        ));
        }

        public void Add(CustomEntitySetType entitySet)
        {
            CustomEntitySetType otherEntitySet;
            if (_entitySets.TryGetValue(entitySet.Name, out otherEntitySet))
            {
                throw new CustomMetadataException(String.Format(
                                "EntitySet with the same name '{0}' has already been registred" +
                                " as EntitySet for '{1}'",
                                entitySet.Name,
                                entitySet.BaseElementType
                            ));
            }
            _entitySets[entitySet.Name] = entitySet;
        }

        public void Add(CustomEntityType entityType)
        {
            CustomEntityType otherEntityType;
            if(_clrObjectTypeMap.TryGetValue(entityType.ClrObjectType, out otherEntityType))
            {
                throw new CustomMetadataException(String.Format(
                                "CLR object type '{0}' is already mapped to entity type '{0}'.",
                                entityType.ClrObjectType,
                                otherEntityType
                            ));
            }
            _clrObjectTypeMap[entityType.ClrObjectType] = entityType;
        }

        #endregion
    }

    #endregion

    //
    // CustomEntitySetType class
    //
    #region CustomEntitySetType class

    public class CustomEntitySetType
    {
        #region Private fields

        private readonly CustomEntityType _baseElementType;

        private readonly string _name;

        #endregion

        #region Constructors

        public CustomEntitySetType(
                        CustomEntityType baseElementType,
                        string name
                    )
        {
            CustomUtils.CheckArgumentNotNull(baseElementType, "baseElementType");
            CustomUtils.CheckArgumentNotNull(name, "name");

            _baseElementType = baseElementType;
            _name = name;
        }

        #endregion

        #region Properties

        public CustomEntityType BaseElementType
        {
            get { return _baseElementType; }
        }

        public string Name
        {
            get { return _name; }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            CustomEntitySetType other = obj as CustomEntitySetType;
            return null == other ? false : Name.Equals(other.Name) && BaseElementType.Equals(other.BaseElementType);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ BaseElementType.GetHashCode();
        }

        #endregion
    }

    #endregion

    //
    // CustomEntityType class
    //
    #region CustomEntityType class

    public class CustomEntityType
    {
        #region Private fields

        private readonly Type _clrObjectType;

        private readonly ICollection<CustomMemberType> _members;

        private readonly ICollection<CustomPropertyType> _propeties;
        private readonly ICollection<CustomPropertyType> _primaryKeyProperties;

        private readonly ICollection<CustomNavigationPropertyType> _navigationProperties;

        #endregion

        #region Constructors

        public CustomEntityType(
                        Type clrObjectType,
                        IEnumerable<CustomPropertyType> propeties,
                        IEnumerable<CustomNavigationPropertyType> navigationProperties
                    )
        {            
            _clrObjectType = clrObjectType;

            _propeties = propeties.ToList().AsReadOnly(); ;
            _navigationProperties = navigationProperties.ToList().AsReadOnly();

            List<CustomMemberType> allMemberList = new List<CustomMemberType>();
            allMemberList.AddRange(_propeties.ToArray());
            allMemberList.AddRange(_navigationProperties.ToArray());

            _members = allMemberList.AsReadOnly();

            _primaryKeyProperties = propeties.Where(p => p.IsPrimaryKey)
                                                    .Select(p => p).ToList().AsReadOnly();
        }

        #endregion

        #region Properties

        public Type ClrObjectType
        {
            get { return _clrObjectType; }
        }

        public string Name
        {
            get { return _clrObjectType.Name; }
        }

        public ICollection<CustomMemberType> Members
        {
            get { return _members; }
        }

        public ICollection<CustomPropertyType> Propeties
        {
            get { return _propeties; }
        }

        public ICollection<CustomPropertyType> PrimaryKeyProperties
        {
            get { return _primaryKeyProperties; }
        }

        public ICollection<CustomNavigationPropertyType> NavigationProperties
        {
            get { return _navigationProperties; }
        }

        #endregion

        #region Methods

        public bool IsAssignableFrom(CustomEntityType otherEntityType)
        {
            return _clrObjectType.IsAssignableFrom(otherEntityType._clrObjectType);
        }

        #endregion

        #region ToString / Equals / GetHashCode

        public override string ToString()
        {
            return _clrObjectType.Name;
        }

        public override bool Equals(object obj)
        {
            CustomEntityType other = obj as CustomEntityType;
            return null == other ? false : _clrObjectType.Equals(other._clrObjectType);
        }

        public override int GetHashCode()
        {
            return _clrObjectType.GetHashCode();
        }

        #endregion
    }

    #endregion

    //
    // CustomMemberType class
    //
    #region CustomMemberType class

    public class CustomMemberType
    {
        #region Private fields

        private string _name;
        private readonly string[] _nestedComplexTypePropertyPath;
        private readonly PropertyInfo _clrPropertyInfo;        

        private bool _isNullable;
        private bool _isReadOnly;

        #endregion

        #region Constructors

        public CustomMemberType(
                        PropertyInfo clrPropertyInfo,
                        bool isNullable,
                        bool isReadOnly
                 )
            : this(null, clrPropertyInfo, isNullable, isReadOnly)
        {
        }

        public CustomMemberType(
                        string[] nestedComplexTypePropertyPath,
                        PropertyInfo clrPropertyInfo,
                        bool isNullable,
                        bool isReadOnly
                    )
        {
            _nestedComplexTypePropertyPath = nestedComplexTypePropertyPath;
            _clrPropertyInfo = clrPropertyInfo;
            _isNullable = isNullable;
            _isReadOnly = isReadOnly;

            _name = BuildMemberName();
        }

        #endregion

        #region Properties

        public Type ClrType
        {
            get { return _clrPropertyInfo.PropertyType; }
        }

        public PropertyInfo ClrPropertyInfo
        {
            get { return _clrPropertyInfo; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsNullable
        {
            get { return _isNullable; }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
        }

        #endregion

        #region Methods

        public object GetValue(object entityObject)
        {
            Debug.Assert(_clrPropertyInfo.GetGetMethod() != null);
            return _clrPropertyInfo.GetValue(
                                GetNestedComplexTypePropertyPathIfSpecified(entityObject),
                                null
                            );
        }

        public void SetValue(object entityObject, object value)
        {
            if (null == _clrPropertyInfo.GetSetMethod(true))
            {
                throw new CustomMetadataException(String.Format(
                                "Property '{0}' has no setter method available. Don't you try " +
                                "to set readonly property like primary or foreign key?",
                                this.ToString()
                            ));
            }

            _clrPropertyInfo.SetValue(
                            GetNestedComplexTypePropertyPathIfSpecified(entityObject),
                            value,
                            null
                        );
        }

        private object GetNestedComplexTypePropertyPathIfSpecified(object entityObject)
        {
            if (null == _nestedComplexTypePropertyPath)
            {
                return entityObject;
            }

            object propertyOwner = entityObject;
            foreach (string token in _nestedComplexTypePropertyPath)
            {
                propertyOwner = propertyOwner.GetType().GetProperty(token).GetValue(propertyOwner, null);
                Debug.Assert(null != propertyOwner, "ComplexType property cannot be null");
            }
            return propertyOwner;
        }

        private string BuildMemberName()
        {
            StringBuilder buf = new StringBuilder();
            if (null != _nestedComplexTypePropertyPath)
            {
                foreach (string token in _nestedComplexTypePropertyPath)
                {
                    buf.Append(token);
                    buf.Append(".");
                }
            }
            buf.Append(_clrPropertyInfo.Name);
            return buf.ToString();
        }

        public override string ToString()
        {
            return _name;
        }

        #endregion

        #region Equals / GetHashCode

        public override bool Equals(object obj)
        {
            CustomMemberType other = obj as CustomMemberType;
            if (null != other)
            {
                return _clrPropertyInfo.Equals(other._clrPropertyInfo);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _clrPropertyInfo.GetHashCode();
        }

        #endregion
    }

    #endregion

    //
    // CustomForeignKeyConstraint class
    //
    #region CustomForeignKeyConstraint class

    public class CustomForeignKeyConstraint
    {
        #region Private fields

        private CustomEntityType _targetEntity;
        private CustomPropertyType _targetPrimaryKeyProperty;
        private bool _readonly;

        #endregion

        #region Constructors

        public CustomForeignKeyConstraint()
        {
        }

        public void DefferedInitialization(
                            CustomEntityType targetEntity,
                            CustomPropertyType targetPrimaryKeyProperty
                        )
        {
            Debug.Assert(!_readonly);

            _targetEntity = targetEntity;
            _targetPrimaryKeyProperty = targetPrimaryKeyProperty;

            _readonly = true;
        }

        #endregion

        #region Properties

        public CustomEntityType TargetEntity
        {
            get { return _targetEntity; }
        }

        public CustomPropertyType TargetPrimaryKeyProperty
        {
            get { return _targetPrimaryKeyProperty; }
        }

        #endregion
    }

    #endregion

    //
    // CustomPropertyType class
    //
    #region CustomPropertyType class

    public class CustomPropertyType : CustomMemberType
    {
        #region Private fields

        private readonly int? _maximumLength;
        private readonly bool _isStoreGenerated;
        private readonly bool _isPrimaryKey;
        private readonly CustomForeignKeyConstraint _foreignKeyConstraint;

        #endregion

        #region Constructors

        public CustomPropertyType(
                    string[] nestedComplexTypePropertyPath,
                    PropertyInfo clrPropertyInfo,
                    bool isNullable,
                    bool isReadOnly,
                    int? maximumLength,
                    bool isStoreGenerated,
                    bool isPrimaryKey,
                    CustomForeignKeyConstraint foreignKeyConstraint
                )
            : base(nestedComplexTypePropertyPath, clrPropertyInfo, isNullable, isReadOnly)
        {
            _maximumLength = maximumLength;
            _isStoreGenerated = isStoreGenerated;
            _isPrimaryKey = isPrimaryKey;
            _foreignKeyConstraint = foreignKeyConstraint;
        }

        #endregion

        #region Properties

        public int? MaximumLength
        {
            get { return _maximumLength; }
        }

        public bool IsStoreGenerated
        {
            get { return _isStoreGenerated; }
        }

        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
        }

        public CustomForeignKeyConstraint ForeignKeyConstraint
        {
            get { return _foreignKeyConstraint; }
        }

        #endregion
    }

    #endregion

    //
    // CustomMultiplicity enum
    //
    #region CustomMultiplicity enum

    public enum CustomMultiplicity
    {
        One,
        Many
    }

    #endregion

    //
    // CustomRelationshipEndType class
    //
    #region CustomRelationshipEndType class

    public class CustomRelationshipEndType
    {
        #region Private fields

        private bool _isPrimaryEnd;
        private CustomEntitySetType _entitySetType;
        private CustomEntityType _entityType;
        private CustomNavigationPropertyType _relatedProperty;
        private CustomMultiplicity _multiplicity;
        private bool _readonly;

        #endregion

        #region Constructors

        public CustomRelationshipEndType()
        {
        }

        public void DefferedInitialization(
                    bool isPrimaryEnd,
                    CustomEntitySetType entitySetType,
                    CustomEntityType entityType,
                    CustomNavigationPropertyType relatedProperty,
                    CustomMultiplicity multiplicity                   
                )
        {
            Debug.Assert(!_readonly);

            _isPrimaryEnd = isPrimaryEnd;
            _entitySetType = entitySetType;
            _entityType = entityType;
            _relatedProperty = relatedProperty;
            _multiplicity = multiplicity;

            _readonly = true;
        }

        #endregion

        #region Properties

        public CustomEntitySetType EntitySetType
        {
            get { return _entitySetType; }
        }

        public CustomEntityType EntityType
        {
            get { return _entityType; }
        }

        public CustomNavigationPropertyType RelatedProperty
        {
            get { return _relatedProperty; }
        }

        public CustomMultiplicity Multiplicity
        {
            get { return _multiplicity; }
        }

        public bool IsPrimaryEnd
        {
            get { return _isPrimaryEnd; }
        }

        #endregion
    }

    #endregion

    //
    // CustomNavigationPropertyType class
    //
    #region CustomNavigationPropertyType class

    public class CustomNavigationPropertyType : CustomMemberType
    {
        #region Private fields

        private readonly CustomRelationshipEndType _from;

        private readonly CustomRelationshipEndType _to;

        #endregion

        #region Constructors

        public CustomNavigationPropertyType(
                            string[] nestedComplexTypePropertyPath,
                            PropertyInfo clrPropertyInfo,
                            bool isNullable,
                            bool isReadOnly,
                            CustomRelationshipEndType from,                            
                            CustomRelationshipEndType to                           
                        )
            : base(nestedComplexTypePropertyPath, clrPropertyInfo, isNullable, isReadOnly)
        {
            _from = from;
            _to = to;
        }

        #endregion

        #region Properties

        public CustomRelationshipEndType From
        {
            get { return _from; }
        }

        public CustomRelationshipEndType To
        {
            get { return _to; }
        }

        #endregion
    }

    #endregion

    //
    // AnnotationBasedCustomMetadataBuilder class
    //
    #region AnnotationBasedCustomMetadataBuilder class

    public class AnnotationBasedCustomMetadataBuilder
    {
        #region Private fields

        private ICollection<Type> _clrObjectTypeCollection;

        #endregion

        #region Public construction methods

        public static CustomMetadataWorkspace BuildMetadataWorkspace(ICollection<Type> clrObjectTypeCollection)
        {
            return (new AnnotationBasedCustomMetadataBuilder(clrObjectTypeCollection)).Build();
        }

        #endregion

        #region Implementation

        private AnnotationBasedCustomMetadataBuilder(ICollection<Type> clrObjectTypeCollection)
        {
            _clrObjectTypeCollection = clrObjectTypeCollection;
        }

        private CustomMetadataWorkspace Build()
        {
            CustomMetadataWorkspace workspace = new CustomMetadataWorkspace();

            ICollection<DeferredForeignKeyMetadata> deferredForeignKeys
                                                        = new List<DeferredForeignKeyMetadata>();

            ICollection<DeferredRelationshipMetadata> deferredRelationships
                                                        = new List<DeferredRelationshipMetadata>();

            ICollection<CustomEntitySetType> collectedEntitySets = new List<CustomEntitySetType>();

            foreach (Type clrObjectType in _clrObjectTypeCollection)
            {
                ICollection<EntityMemberCandidate> memberCandidates = CollectFlatMemberCandidateList(clrObjectType);

                ICollection<CustomPropertyType> properties = GetEntityProperties(
                                                                        clrObjectType,
                                                                        memberCandidates,
                                                                        deferredForeignKeys
                                                                    );

                ICollection<CustomNavigationPropertyType> navigationProperties
                                                            = GetEntityNavigationProperties(
                                                                                clrObjectType,
                                                                                memberCandidates,
                                                                                deferredRelationships
                                                                            );

                CustomEntityType entityType = new CustomEntityType(
                                                            clrObjectType,
                                                            properties,
                                                            navigationProperties
                                                        );

                CollectEntitySets(clrObjectType, entityType, collectedEntitySets);

                workspace.Add(entityType);

                if (memberCandidates.Count > 0)
                {
                    /*
                     * warn user about unmapped properties?
                    Debug.Fail(String.Format(
                                    "Found unmapped properties [{0}] in CLR object '{1}'.",
                                    CustomUtils.FormatCsvString(memberCandidates.Select(m => m.ToString())), clrObjectType.Name
                            ));
                     */
                }
            }

            foreach (CustomEntitySetType entitySet in collectedEntitySets)
            {
                workspace.Add(entitySet);
            }

            FixUpDeferredForeignKeyMetadata(workspace, deferredForeignKeys);
            FixUpDeferredRelationshipMetadata(workspace, deferredRelationships);

            workspace.ValidateAndLock();
            return workspace;
        }

        private void FixUpDeferredForeignKeyMetadata(
                                    CustomMetadataWorkspace workspace,
                                    IEnumerable<DeferredForeignKeyMetadata> deferredForeignKeys
                                )
        {
            foreach (DeferredForeignKeyMetadata metadata in deferredForeignKeys)
            {
                CustomEntityType targetEntity;
                if (!workspace.TryGetEntityType(metadata.Attribute.TargetEntityType, out targetEntity))
                {
                    ThrowUnableToFindPartOfForeignKey(
                                    "entity type mapped to CLR object",
                                    metadata.Attribute.TargetEntityType.Name,
                                    metadata
                                );
                }

                var targetPrimaryKey = targetEntity.PrimaryKeyProperties
                                            .Where(pk => pk.Name == metadata.Attribute.TargetKeyPropertyName)
                                            .Select(pk => pk)
                                            .SingleOrDefault();
                if (null == targetPrimaryKey)
                {
                    ThrowUnableToFindPartOfForeignKey(
                                    "primary key",
                                    metadata.Attribute.TargetKeyPropertyName,
                                    metadata
                                );
                }

                metadata.Consraint.DefferedInitialization(targetEntity, targetPrimaryKey);
            }
        }

        private static void ThrowUnableToFindPartOfForeignKey(
                                            string part,
                                            string matching,
                                            DeferredForeignKeyMetadata inForeignKey
                                        )
        {
            throw new CustomMetadataException(String.Format(
                                "Unable to find {0} '{1}' referenced in foreign key '{2}'.",
                                part,
                                matching,
                                inForeignKey
                            ));
        }

        private void FixUpDeferredRelationshipMetadata(
                                    CustomMetadataWorkspace workspace,
                                    IEnumerable<DeferredRelationshipMetadata> deferredRelationships
                                )
        {
            foreach (DeferredRelationshipMetadata metadata in deferredRelationships)
            {
                CustomEntityType entityType;
                if (!workspace.TryGetEntityType(metadata.ClrObjectType, out entityType))
                {
                    ThrowUnableToFindPartOfRelationship("entity type mapped to CLR object", metadata.ClrObjectType.Name, metadata);
                }

                CustomEntitySetType entitySetType;
                if (null == metadata.Attribute.EntitySetName)
                {
                    entitySetType = InferEntitySetFromEntityType(workspace, entityType, metadata.ToString());
                }
                else
                {
                    if (!workspace.TryGetEntitySet(metadata.Attribute.EntitySetName, out entitySetType))
                    {
                        ThrowUnableToFindPartOfRelationship("entity set", metadata.Attribute.EntitySetName, metadata);
                    }
                    Debug.Assert(entitySetType.BaseElementType.IsAssignableFrom(entityType));
                }

                var multiplicity = metadata.Attribute.Multiplicity;
                var navProperty = metadata.NavigationProperty;
                bool isPrimaryEnd = metadata.Attribute.IsPrimaryEnd;

                var relatedEndMultiplicity = metadata.Attribute.RelatedEndMultiplicity;

                CustomEntityType relatedEndEntityType;
                if (null == metadata.Attribute.RelatedEndEntityType)
                {
                    if (CustomMultiplicity.Many == relatedEndMultiplicity)
                    {
                        Debug.Assert(typeof(IEnumerable).IsAssignableFrom(navProperty.ClrType));
                        if ((typeof(ICollection<>).IsAssignableFrom(navProperty.ClrType.GetGenericTypeDefinition())))
                        {
                            relatedEndEntityType = workspace.GetEntityType(navProperty.ClrType.GetGenericArguments()[0]);
                        }
                        else
                        {
                            throw new CustomMetadataException(String.Format(
                                                "Cannot infer type of collection returned by navigation " +
                                                "property '{0}' because it is not generic collection. Use " +
                                                "generic collection or specify the type of the related entity.",
                                                navProperty
                                            ));
                        }
                    }
                    else
                    {
                        Debug.Assert(!typeof(IEnumerable).IsAssignableFrom(navProperty.ClrType));
                        relatedEndEntityType = workspace.GetEntityType(navProperty.ClrType);
                    }
                }
                else
                {
                    if (!workspace.TryGetEntityType(metadata.Attribute.RelatedEndEntityType, out relatedEndEntityType))
                    {
                        ThrowUnableToFindPartOfRelationship("entity type mapped to CLR object",
                                                                metadata.Attribute.RelatedEndEntityType.Name, metadata);
                    }
                }

                CustomEntitySetType relatedEndEntitySetType;
                if (null == metadata.Attribute.RelatedEndEntitySetName)
                {
                    relatedEndEntitySetType = InferEntitySetFromEntityType(workspace, relatedEndEntityType, metadata.ToString());
                }
                else
                {
                    if (!workspace.TryGetEntitySet(metadata.Attribute.RelatedEndEntitySetName, out relatedEndEntitySetType))
                    {
                        ThrowUnableToFindPartOfRelationship("entity set", metadata.Attribute.RelatedEndEntitySetName, metadata);
                    }
                    Debug.Assert(relatedEndEntitySetType.BaseElementType.IsAssignableFrom(relatedEndEntityType));
                }

                CustomNavigationPropertyType relatedEndProperty = null;

                if (null != metadata.Attribute.RelatedEndPropertyName)
                {
                    relatedEndProperty =
                                relatedEndEntityType.NavigationProperties
                                    .Where(p => p.Name == metadata.Attribute.RelatedEndPropertyName)
                                    .Select(p => p)
                                    .SingleOrDefault();

                    if (null == relatedEndProperty)
                    {
                        ThrowUnableToFindPartOfRelationship("other end navigation property",
                                                metadata.Attribute.RelatedEndPropertyName, metadata);
                    }
                }

                metadata.NavigationProperty.From.DefferedInitialization(
                                                            isPrimaryEnd,
                                                            entitySetType,
                                                            entityType,
                                                            navProperty,
                                                            multiplicity
                                                    );

                metadata.NavigationProperty.To.DefferedInitialization(
                                                            !isPrimaryEnd,
                                                            relatedEndEntitySetType,
                                                            relatedEndEntityType,
                                                            relatedEndProperty,
                                                            relatedEndMultiplicity
                                                    );

                if (null != metadata.NavigationProperty.To.RelatedProperty)
                {
                    var fromMember = metadata.NavigationProperty.To;
                    if (fromMember.RelatedProperty.From.EntityType != null)
                    {
                        /*
                        Debug.Assert(fromMember.RelatedProperty.To.RelatedProperty == metadata.NavigationProperty);
                        Debug.Assert(fromMember.RelatedProperty.To.EntityType == metadata.NavigationProperty.From.EntityType);
                        Debug.Assert(fromMember.RelatedProperty.To.Multiplicity == metadata.NavigationProperty.From.Multiplicity);
                        Debug.Assert(fromMember.RelatedProperty.To.IsPrimaryEnd == metadata.NavigationProperty.From.IsPrimaryEnd);
                        */
                    }
                }

                if (CustomMultiplicity.Many == navProperty.To.Multiplicity)
                {
                    Debug.Assert(typeof(IEnumerable).IsAssignableFrom(navProperty.ClrType)); ;
                }
            }
        }

        private static void ThrowUnableToFindPartOfRelationship(
                                    string part,
                                    string matching,
                                    DeferredRelationshipMetadata inRelationship
                                )
        {
            throw new CustomMetadataException(String.Format(
                            "Unable to find {0} '{1}' referenced in navigation property '{2}'.",
                            part,
                            matching,
                            inRelationship
                        ));
        }

        private CustomEntitySetType InferEntitySetFromEntityType(
                                                CustomMetadataWorkspace metadata,
                                                CustomEntityType entityType,
                                                string context
                                            )
        {
            var candidateEntitySets = metadata.EntitySets.Where(es => es.BaseElementType
                                            .IsAssignableFrom(entityType))
                                            .Select(es => es);

            if (candidateEntitySets.Count() == 1)
            {
                return candidateEntitySets.First();
            }
            throw new CustomMetadataException(String.Format(
                            "Cannot infer type of entity set from given entity type '{0}' " +
                            "in the context of '{1}' because there are either no or several " +
                            "entity sets with suitable base element type: [{2}]. Specify " +
                            "which entity set to use.",
                            entityType,
                            context,
                            CustomUtils.FormatCsvString(candidateEntitySets.Select(e => e.Name))
                        ));
        }

        private void CollectEntitySets(
                                Type clrObjectType,
                                CustomEntityType entityType,
                                ICollection<CustomEntitySetType> collectedEntitySets
                            )
        {
            var entitySetAttribute = TryGetAttribute<CustomTargetEntitySetAttribute>(clrObjectType, false);
            if (null != entitySetAttribute)
            {
                foreach (string name in entitySetAttribute.EntitySetNames)
                {
                    collectedEntitySets.Add(new CustomEntitySetType(entityType, name));
                }
            }
        }

        private ICollection<CustomNavigationPropertyType>
                                GetEntityNavigationProperties(
                                            Type clrObjectType,
                                            ICollection<EntityMemberCandidate> candidateList,
                                            ICollection<DeferredRelationshipMetadata> deferredRelationships
                                        )
        {
            var collectedNavidationProperties = new List<CustomNavigationPropertyType>();
            var candidateArray = candidateList.ToArray();

            foreach (var navigationPropertyCandidate in candidateArray)
            {
                var navigationPropertyAttribute =
                                    TryGetAttribute<CustomNavigationPropertyAttribute>(
                                                                navigationPropertyCandidate.ClrProperty
                                                    );

                if (null == navigationPropertyAttribute)
                {
                    continue;
                }

                CustomRelationshipEndType from = new CustomRelationshipEndType();
                CustomRelationshipEndType to = new CustomRelationshipEndType();

                CustomNavigationPropertyType navigationProperty =
                            new CustomNavigationPropertyType(
                                        navigationPropertyCandidate.NestedComplexTypePropertyPath,
                                        navigationPropertyCandidate.ClrProperty,
                                        navigationPropertyAttribute.IsNullable,
                                        navigationPropertyAttribute.IsReadOnly,

                                        from,
                                        to
                                    );

                collectedNavidationProperties.Add(navigationProperty);
                candidateList.Remove(navigationPropertyCandidate);

                deferredRelationships.Add(
                                new DeferredRelationshipMetadata(
                                        clrObjectType,
                                        navigationPropertyAttribute,
                                        navigationProperty
                                    )
                            );
            }

            return collectedNavidationProperties;
        }


        private ICollection<CustomPropertyType>
                                    GetEntityProperties(
                                                Type clrObjectType,
                                                ICollection<EntityMemberCandidate> candidateList,
                                                ICollection<DeferredForeignKeyMetadata> deferredForeignKeys
                                            )
        {
            var primaryKeyNamesAttribute = TryGetAttribute<KeyAttribute>(
                                                                    clrObjectType);
            if (null == primaryKeyNamesAttribute || 0 == primaryKeyNamesAttribute.KeyNames.Count)
            {
                throw new CustomMetadataException(String.Format(
                                "EntityType mapping for CLR object type '{0}' is incomplete " +
                                "because it does not define any primary keys. Use {1} to " +
                                "specify at least one primary key.",
                                clrObjectType.Name,
                                typeof(KeyAttribute).Name
                            ));
            }
            ICollection<string> primaryKeyNames = new List<string>(primaryKeyNamesAttribute.KeyNames);
            Debug.Assert(primaryKeyNames.Count > 0);

            var collectedEntityProperties = new List<CustomPropertyType>();
            var candidateArray = candidateList.ToArray();

            foreach (var propertyCandidate in candidateArray)
            {
                PropertyInfo clrProperty = propertyCandidate.ClrProperty;

                var propertyAttribute = TryGetAttribute<CustomPropertyAttribute>(clrProperty);
                if (null == propertyAttribute)
                {
                    continue;
                }

                var isComplexTypeProperty = TryGetAttribute<CustomComplexTypeAttribute>(
                                                    clrProperty.PropertyType) != null;
                
                var foreignKeyAttribute = TryGetAttribute<CustomForeignKeyConstraintAttribute>(
                                                                clrProperty);

                CustomForeignKeyConstraint foreignKeyConstraint = (null == foreignKeyAttribute ? null
                                                    : new CustomForeignKeyConstraint());

                bool isPrimaryKey = primaryKeyNames.Remove(clrProperty.Name);
                CustomPropertyType entityProperty = new CustomPropertyType(
                                            propertyCandidate.NestedComplexTypePropertyPath,
                                            clrProperty,
                                            propertyAttribute.IsNullable,
                                            propertyAttribute.IsReadOnly,
                                            propertyAttribute.HasMaximumLength ?
                                                    propertyAttribute.MaximumLength : (int?)null,
                                            propertyAttribute.IsStoreGenerated,
                                            isPrimaryKey,
                                            foreignKeyConstraint
                                        );

                collectedEntityProperties.Add(entityProperty);
                candidateList.Remove(propertyCandidate);

                if (null != foreignKeyConstraint)
                {
                    deferredForeignKeys.Add(
                                new DeferredForeignKeyMetadata(
                                                clrObjectType,
                                                entityProperty,
                                                foreignKeyAttribute,
                                                foreignKeyConstraint
                                            )
                            );
                }
            }

            if (primaryKeyNames.Count > 0)
            {
                throw new CustomMetadataException(String.Format(
                                "The following primary key properties [{0}] defined in " +
                                "{1} were not resolved to entity properties. Make sure " +
                                "that the listed properties are defined and annotated with {1}.",
                                CustomUtils.FormatCsvString(primaryKeyNames),
                                typeof(KeyAttribute).Name,
                                typeof(CustomPropertyAttribute).Name
                            ));
            }

            return collectedEntityProperties;
        }

        #endregion

        #region Other static helpers

        private static AttributeType TryGetAttribute<AttributeType>(MemberInfo forMember)
        {
            return forMember.GetCustomAttributes(typeof(AttributeType), true)
                        .Select(a => (AttributeType)a).SingleOrDefault();
        }

        private static AttributeType TryGetAttribute<AttributeType>(MemberInfo forMember, bool inherit)
        {
            return forMember.GetCustomAttributes(typeof(AttributeType), inherit)
                        .Select(a => (AttributeType)a).SingleOrDefault();
        }

        private static ICollection<EntityMemberCandidate> CollectFlatMemberCandidateList(
                                                               Type clrObjectType
                                                            )
        {
            IList<string> currentComplexTypePropertyPath = new List<string>();
            ICollection<EntityMemberCandidate> collectedCandidates =
                                                new List<EntityMemberCandidate>();

            CollectFlatMemberCandidateList(currentComplexTypePropertyPath, clrObjectType,
                                        collectedCandidates);

            return collectedCandidates;
        }

        private static void CollectFlatMemberCandidateList(
                                        IList<string> currentComplexTypePropertyPath,
                                        Type clrType,
                                        ICollection<EntityMemberCandidate> collectedCandidates
                                    )
        {
            foreach (PropertyInfo clrProperty in clrType.GetProperties())
            {
                if (clrProperty.PropertyType.GetCustomAttributes(
                                typeof(CustomComplexTypeAttribute), true).Length != 0)
                {
                    currentComplexTypePropertyPath.Add(clrProperty.Name);
                    CollectFlatMemberCandidateList(
                                        currentComplexTypePropertyPath,
                                        clrProperty.PropertyType,
                                        collectedCandidates
                                    );
                    currentComplexTypePropertyPath.RemoveAt(currentComplexTypePropertyPath.Count - 1);
                }
                else
                {
                    collectedCandidates.Add(
                                    new EntityMemberCandidate(
                                                currentComplexTypePropertyPath.Count > 0 ?
                                                    currentComplexTypePropertyPath.ToArray() : null,
                                                clrProperty
                                            )
                                );
                }
            }
        }

        #endregion

        #region Private inner classes

        private class EntityMemberCandidate
        {
            private string _fullPropertyPath = null;

            public readonly string[] NestedComplexTypePropertyPath;
            public readonly PropertyInfo ClrProperty;

            public EntityMemberCandidate(
                            string[] nestedComplexTypePropertyPath,
                            PropertyInfo clrProperty
                        )
            {
                this.NestedComplexTypePropertyPath = nestedComplexTypePropertyPath;
                this.ClrProperty = clrProperty;
            }

            public override string ToString()
            {
                if (null == _fullPropertyPath)
                {
                    StringBuilder buf = new StringBuilder();
                    if (null != NestedComplexTypePropertyPath)
                    {
                        foreach (string token in NestedComplexTypePropertyPath)
                        {
                            buf.Append(token);
                            buf.Append(".");
                        }
                    }
                    buf.Append(ClrProperty.Name);
                    _fullPropertyPath = buf.ToString();
                }
                return _fullPropertyPath;
            }
        }

        private class DeferredForeignKeyMetadata
        {
            public readonly Type ClrObjectType;
            public readonly CustomPropertyType EntityProperty;
            public readonly CustomForeignKeyConstraintAttribute Attribute;
            public readonly CustomForeignKeyConstraint Consraint;

            public DeferredForeignKeyMetadata(
                            Type clrObjectType,
                            CustomPropertyType entityProperty,
                            CustomForeignKeyConstraintAttribute attribute,
                            CustomForeignKeyConstraint consraint
                        )
            {
                this.ClrObjectType = clrObjectType;
                this.EntityProperty = entityProperty;
                this.Attribute = attribute;
                this.Consraint = consraint;
            }

            public override string ToString()
            {
                return String.Format("{0}.{1} -> {2}.{3}", ClrObjectType.Name, EntityProperty.Name,
                                Attribute.TargetEntityType, Attribute.TargetKeyPropertyName);
            }
        }

        private class DeferredRelationshipMetadata
        {
            public readonly Type ClrObjectType;
            public readonly CustomNavigationPropertyAttribute Attribute;
            public readonly CustomNavigationPropertyType NavigationProperty;

            public DeferredRelationshipMetadata(
                            Type clrObjectType,
                            CustomNavigationPropertyAttribute attribute,
                            CustomNavigationPropertyType navigationProperty
                        )
            {
                this.ClrObjectType = clrObjectType;
                this.Attribute = attribute;
                this.NavigationProperty = navigationProperty;
            }

            public override string ToString()
            {
                return String.Format("{0}.{1} <-> {2}.{3}", ClrObjectType.Name, NavigationProperty, Attribute.RelatedEndEntityType,
                        null == Attribute.RelatedEndPropertyName ? "<n/a>" : Attribute.RelatedEndPropertyName);
            }
        }

        #endregion
    }

    #endregion

    // --- CODE SNIPPET END ---
}
