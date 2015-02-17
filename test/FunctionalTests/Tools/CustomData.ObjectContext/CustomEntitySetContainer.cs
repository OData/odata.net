//---------------------------------------------------------------------
// <copyright file="CustomEntitySetContainer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Data.Test.Astoria.CustomData.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization.Formatters;

namespace System.Data.Test.Astoria.CustomData.Runtime
{
    // --- CODE SNIPPET START ---

    //
    // CustomEntitySetContainer class
    //
    #region CustomEntitySetContainer class

    public class CustomEntitySetContainer : IDisposable
    {
        #region Private fields

        private CustomMetadataWorkspace _metadata = null;

        private IDictionary<string, CustomEntitySet> _entitySetMap
                                                = new Dictionary<string, CustomEntitySet>();

        private bool _defaultDataCreated = false;

        private bool _readOnly = false;

        private bool _disableStoreValidation = false;

        private DateTime _lastWriteTime = DateTime.MinValue;

        private DataContractSerializer _dataContractSerializer = null;

        #endregion

        #region Public constructors

        public CustomEntitySetContainer(
                        CustomMetadataWorkspace metadataWorkspace
                    )
        {
            CustomUtils.CheckArgumentNotNull(metadataWorkspace, "metadataWorkspace");
            _metadata = metadataWorkspace;

            foreach (CustomEntitySetType entitySetType in MetadataWorkspace.EntitySets)
            {
                CreateQuery(entitySetType.Name);
            }
        }

        #endregion

        #region Properties

        public CustomMetadataWorkspace MetadataWorkspace
        {
            get { return _metadata; }
        }

        public bool DisableStoreValidation
        {
            get { return _disableStoreValidation; }
            set { _disableStoreValidation = true; }
        }

        public DateTime LastWriteTime
        {
            get { return _lastWriteTime; }

            protected set
            {
                Debug.Assert(_lastWriteTime <= value);
                _lastWriteTime = value;
            }
        }

        protected bool IsReadOnly
        {
            get { return _readOnly; }
            private set { _readOnly = value; }
        }

        #endregion

        #region Query methods

        public IQueryable<EntityObjectType> CreateQueryOfT<EntityObjectType>(string entitySetName)
        {
            CustomUtils.CheckArgumentNotNull(entitySetName, "entitySetName");

            CustomEntitySetType entitySetType = _metadata.GetEntitySet(entitySetName);
            CustomEntityType entityType = _metadata.GetEntityType(typeof(EntityObjectType));

            if (!entitySetType.BaseElementType.IsAssignableFrom(entityType))
            {
                throw new InvalidOperationException(String.Format(
                                    "The base element type '{0}' of the entity set '{1}' is not compatible with " +
                                    "the queried entity type '{2}'.",
                                    entitySetType.BaseElementType, entitySetName, entityType
                                ));

            }

            CustomEntitySet entitySet = GetEntitySetByName(entitySetName);

            Debug.Assert(null != entitySet);
            Debug.Assert(typeof(IEnumerable<EntityObjectType>).IsAssignableFrom(entitySet.GetType()));

            return ((IEnumerable<EntityObjectType>)entitySet).AsQueryable();
        }

        public IQueryable CreateQuery(string entitySetName)
        {
            CustomUtils.CheckArgumentNotNull(entitySetName, "entitySetName");

            CustomEntitySet entitySet = GetEntitySetByName(entitySetName);

            Debug.Assert(null != entitySet);
            Debug.Assert(entitySet is IEnumerable);

            return ((IEnumerable)entitySet).AsQueryable();
        }

        #endregion

        #region Adding / Updating / Deleting entity objects

        public void Add(string entitySetName, object entityObject)
        {
            CheckNotReadOnly();

            CustomEntitySet entitySet = GetEntitySetByName(entitySetName);
            entitySet.Add(entityObject);
        }

        public void Update(string entitySetName, object entityObject)
        {
            CheckNotReadOnly();

            CustomEntitySet entitySet = GetEntitySetByName(entitySetName);
            entitySet.Update(entityObject);
        }

        public void Delete(string entitySetName, object entityObject)
        {
            CheckNotReadOnly();

            CustomEntitySet entitySet = GetEntitySetByName(entitySetName);
            entitySet.Remove(entityObject);
        }

        public virtual void Flush()
        {
            CheckNotReadOnly();
            _lastWriteTime = DateTime.Now;
        }

        #endregion

        #region Data objects initialization / clean up

        public void PreAtachExistingDataObjects(CustomObjectContext objectContext)
        {
            if (!_defaultDataCreated)
            {
                CreateAndAttachDefaultDataObjects(objectContext);
                objectContext.SaveAllChanges();
                _defaultDataCreated = true;
            }
            else
            {
                foreach (var entitySetEntry in _entitySetMap.Values)
                {
                    foreach (object entityObject in entitySetEntry.EntityObjects)
                    {
                        objectContext.AddObject(entitySetEntry.EntitySetType.Name, entityObject);
                    }
                }
                objectContext.AcceptAllChanges();
            }
        }

        protected virtual void CreateAndAttachDefaultDataObjects(CustomObjectContext objectContext)
        {
        }

        public virtual void ResetData()
        {
            _defaultDataCreated = false;
            _entitySetMap.Clear();
            _lastWriteTime = DateTime.MinValue;
        }

        #endregion

        #region Misc protected methods

        protected virtual CustomEntitySet GetEntitySetByName(string entitySetName)
        {
            CustomEntitySet entitySet;
            if (_entitySetMap.TryGetValue(entitySetName, out entitySet))
            {
                return entitySet;
            }

            CustomEntitySetType entitySetType = MetadataWorkspace.GetEntitySet(entitySetName);
            Type entitySetOfT = typeof(CustomEntitySetOfT<>).MakeGenericType(entitySetType.BaseElementType.ClrObjectType);

            entitySet = (CustomEntitySet)Activator.CreateInstance(entitySetOfT, MetadataWorkspace, entitySetType, this);
            return (_entitySetMap[entitySetName] = entitySet);
        }

        protected virtual IEnumerable<CustomEntitySet> GetAllEntitySets()
        {
            return _entitySetMap.Values;
        }

        #endregion

        #region Serialization

        //
        // WARNING these serialization methods are quick hack for Astoria Test Framework to let it
        // read in-memory data on the server side from the test client
        //

        public virtual void WriteDataObjectsUnsafe(Stream stream)
        {
            CustomUtils.CheckArgumentNotNull(stream, "stream");

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            var entitySetsToSerialize = GetAllEntitySets().ToDictionary(es => es.EntitySetType.Name);
            GetDataContractSerializer().WriteObject(stream, entitySetsToSerialize);
        }

        protected internal virtual void ReloadDataObjectsUnsafe(Stream stream)
        {
            CheckNotReadOnly();

            CustomUtils.CheckArgumentNotNull(stream, "stream");
            Debug.Assert(null != _metadata);

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            DataContractSerializer dataContractSerializer = GetDataContractSerializer();
            var deserializedEntitySets = (IDictionary<string, CustomEntitySet>) dataContractSerializer.ReadObject(stream);

            _entitySetMap.Clear();
            foreach (var entitySetEntry in deserializedEntitySets)
            {
                var entitySetType = MetadataWorkspace.EntitySets
                                                        .Where(et => et.Name == entitySetEntry.Key)
                                                        .Select(et => et)
                                                        .SingleOrDefault();
                Debug.Assert(null != entitySetType);

                // the metadata information for entity sets was not serialized, so we need
                // to fix it up immediately after the entity set is deserialized
                var entitySet = entitySetEntry.Value;
                entitySet.MetadataWorkspace = MetadataWorkspace;
                entitySet.EntitySetType = entitySetType;
                entitySet.ParentContainer = this;

                _entitySetMap.Add(entitySetType.Name, entitySet);
            }

            _defaultDataCreated = true;
        }

        public static ContainerType CreateReadOnlyContainerFromStream<ContainerType>(Stream stream)
                                        where ContainerType : CustomEntitySetContainer, new()
        {
            ContainerType container = new ContainerType();
            container.ReloadDataObjectsUnsafe(stream);

            container.IsReadOnly = true;
            return container;
        }

        #endregion

        #region Serialization helpers

        private DataContractSerializer GetDataContractSerializer()
        {
            if (null != _dataContractSerializer)
            {
                return _dataContractSerializer;
            }

            List<Type> knownTypes = new List<Type>();
            knownTypes.AddRange(MetadataWorkspace.EntityTypes.Select(e => e.ClrObjectType));

            foreach (var entitySetBaseElementType in MetadataWorkspace.EntitySets.Select(e => e.BaseElementType.ClrObjectType))
            {
                knownTypes.Add(typeof(CustomEntitySetOfT<>).MakeGenericType(entitySetBaseElementType));
            }

            foreach (var entityObjectType in MetadataWorkspace.EntityTypes.Select(e => e.ClrObjectType))
            {
                knownTypes.Add(typeof(HashSet<>).MakeGenericType(entityObjectType));
            }            


            _dataContractSerializer = new DataContractSerializer(
                                                typeof(IDictionary<string, CustomEntitySet>),
                                                knownTypes,
                                                Int32.MaxValue,     // maxItemsInObjectGraph
                                                false,              // ignoreExtensionDataObject (if rely on DataContract.IsReference
                                                                    //      property to preserve object references)
                                                false,              // preserveObjectReferences
                                                new DataContractSurrogate()
                                            );
            return _dataContractSerializer;
        }

        internal class DataContractSurrogate : IDataContractSurrogate
        {
            #region Serialization and Deserialization methods

            public Type GetDataContractType(Type type)
            {
                if (typeof(CustomObjectCollectionBase).IsAssignableFrom(type))
                {
                    Debug.Assert(type.IsGenericType);

                    Type genericTypeDefinition = type.GetGenericTypeDefinition();
                    Debug.Assert(
                                typeof(CustomOneToManyObjectCollection<,>).IsAssignableFrom(genericTypeDefinition) ||
                                typeof(CustomManyToManyObjectCollection<,>).IsAssignableFrom(genericTypeDefinition)
                            );

                    Debug.Assert(2 == type.GetGenericArguments().Length);
                    Type storedEntityType = type.GetGenericArguments()[1];

                    return typeof(HashSet<>).MakeGenericType(storedEntityType);
                }
                return type;
            }

            public object GetObjectToSerialize(object obj, Type targetType)
            {
                if (obj is CustomObjectCollectionBase)
                {
                    return ((CustomObjectCollectionBase) obj).InnerCollection;
                }
                return obj;
            }

            public object GetDeserializedObject(object obj, Type targetType)
            {
                if (
                        targetType.IsGenericType &&
                        typeof(ICollection<>).IsAssignableFrom(targetType.GetGenericTypeDefinition()) &&
                        obj.GetType().IsArray
                    )
                {
                    Type storedEntityType = obj.GetType().GetElementType();
                    Type hashSetType = typeof(HashSet<>).MakeGenericType(storedEntityType);
                    return Activator.CreateInstance(hashSetType, (IEnumerable) obj);
                }
                
                return obj;
            }

            #endregion

            #region Default implementation

            public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
            { return null; }

            public object GetCustomDataToExport(Type clrType, Type dataContractType)
            { return null; }

            public void GetKnownCustomDataTypes(System.Collections.ObjectModel.Collection<Type> customDataTypes)
            { }

            public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
            { return null; }

            public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
            { return typeDeclaration; }

            #endregion
        }

        #endregion

        #region IDisposable members

        public virtual void Dispose()
        {
        }

        #endregion

        #region Protected helper methods

        protected void CheckNotReadOnly()
        {
            if (_readOnly)
            {
                throw new InvalidOperationException("Invalid operation on read-only entity set container.");
            }
        }

        #endregion
    }

    #endregion

    //
    // FileBasedCustomEntitySetContainer class
    //
    #region FileBasedCustomEntitySetContainer class


    public class FileBasedCustomEntitySetContainer : CustomEntitySetContainer
    {
        #region Private fields

        private string _backendFileName;

        #endregion

        #region Public constructors

        public FileBasedCustomEntitySetContainer(
                    CustomMetadataWorkspace metadataWorkspace
                )
            : base(metadataWorkspace)
        {
        }

        public FileBasedCustomEntitySetContainer(
                            CustomMetadataWorkspace metadataWorkspace,
                            string backendFileName
                        )
            : base(metadataWorkspace)
        {
            CustomUtils.CheckArgumentNotNull(backendFileName, "backendFileName");
            _backendFileName = backendFileName;
        }

        #endregion

        #region Flush method

        public override void Flush()
        {
            CheckNotReadOnly();

            if (null != _backendFileName)
            {
                File.Delete(_backendFileName);
                using (FileStream fs = new FileStream(_backendFileName, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
                {
                    WriteDataObjectsUnsafe(fs);
                }
                LastWriteTime = DateTime.Now;
            }
        }

        #endregion

        #region Factory methods

        public static ContainerType CreateReadOnlyContainerFromFile<ContainerType>(string fileName)
                                        where ContainerType : CustomEntitySetContainer, new()
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return CreateReadOnlyContainerFromStream<ContainerType>(fs);
            }
        }

        #endregion

        #region IDisposable members

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion

    }

    #endregion

    //
    // CustomEntitySet class
    //
    #region CustomEntitySet class

    [DataContract(IsReference = true)]
    public abstract class CustomEntitySet : IEnumerable
    {
        #region Private fields

        private CustomMetadataWorkspace _metadata;

        private CustomEntitySetType _entitySetType;

        private CustomEntitySetContainer _parentContainer;

        [DataMember]
        private Dictionary<string, long> _primitiveSequenceValues = new Dictionary<string, long>();

        [DataMember]
        private DateTime _dateTimeSequenceValue = DateTime.MinValue;

        #endregion

        #region Constructors

        internal CustomEntitySet(bool yesISwearThatIUseThisConstructorForInternalSerializationOnly)
        {
            Debug.Assert(yesISwearThatIUseThisConstructorForInternalSerializationOnly);
        }

        public CustomEntitySet(
                        CustomMetadataWorkspace metadata,
                        CustomEntitySetType entitySetType,
                        CustomEntitySetContainer parentContainer
                    )
        {
            Debug.Assert(null != metadata);
            Debug.Assert(null != entitySetType);

            _metadata = metadata;
            _entitySetType = entitySetType;
            _parentContainer = parentContainer;
        }

        #endregion

        #region Properties

        public CustomMetadataWorkspace MetadataWorkspace
        {
            get { return _metadata; }
            internal set { _metadata = value; }
        }

        public CustomEntitySetType EntitySetType
        {
            get { return _entitySetType; }
            internal set { _entitySetType = value; }
        }

        public CustomEntitySetContainer ParentContainer
        {
            get { return _parentContainer; }
            internal set { _parentContainer = value; }
        }

        public abstract int Count
        {
            get;
        }

        #endregion

        #region Data access methods

        public abstract IEnumerable EntityObjects
        {
            get;
        }

        public void Add(object entityObject)
        {
            CustomUtils.CheckArgumentNotNull(entityObject, "entityObject");
            CustomEntityType entityType = CheckTypeCompatibility(entityObject);

            AddInternal(entityObject);
            RunPostUpdateStoreTriggers(entityObject, entityType);
        }

        public void Remove(object entityObject)
        {
            CustomUtils.CheckArgumentNotNull(entityObject, "entityObject");
            CustomEntityType entityType = CheckTypeCompatibility(entityObject);

            RemoveInternal(entityObject);
        }

        public void Update(object entityObject)
        {
            CustomUtils.CheckArgumentNotNull(entityObject, "entityObject");
            CustomEntityType entityType = CheckTypeCompatibility(entityObject);

            UpdateInternal(entityObject);
            RunPostUpdateStoreTriggers(entityObject, entityType);
        }

        protected abstract void AddInternal(object entityObject);

        protected abstract void RemoveInternal(object entityObject);

        protected abstract void UpdateInternal(object entityObject);

        #endregion

        #region Private methods

        private CustomEntityType CheckTypeCompatibility(object entityObject)
        {
            Debug.Assert(null != entityObject);

            CustomEntityType entityType = MetadataWorkspace.GetEntityType(entityObject.GetType());
            if (!EntitySetType.BaseElementType.IsAssignableFrom(entityType))
            {
                throw new InvalidOperationException(String.Format(
                                    "The base element type '{0}' of the entity set '{1}' is not compatible with " +
                                    "the entity type '{2}' of the object '{3}'.",
                                    EntitySetType.BaseElementType, EntitySetType.Name, entityType, entityObject
                                ));

            }
            return entityType;
        }

        #endregion

        #region Store triggers simulation

        protected void RunPostUpdateStoreTriggers(object entityObject, CustomEntityType entityType)
        {
            AssignStoreGeneratedValues(entityObject, entityType);

            if (!_parentContainer.DisableStoreValidation)
            {
                ValidatePrimitiveConstraints(entityObject, entityType);
            }
        }

        private void AssignStoreGeneratedValues(object entityObject, CustomEntityType entityType)
        {
            foreach (var property in entityType.Propeties.Where(p => p.IsStoreGenerated == true))
            {
                if (property.IsReadOnly || null != property.ForeignKeyConstraint)
                {
                    continue;
                }

                if (property.IsPrimaryKey)
                {
                    object currKeyValue = property.GetValue(entityObject);
                    if (!CustomEntityKey.IsDefaultKeyValue(property, currKeyValue))
                    {
                        continue;
                    }
                }

                Debug.Assert(null == property.ForeignKeyConstraint);

                object generatedValue = null;
                if (property.ClrType.IsPrimitive)
                {
                    generatedValue = NextPrimitiveSequenceValue(property.ClrType);
                }
                else if (property.ClrType == typeof(Guid))
                {
                    generatedValue = Guid.NewGuid();
                }
                else if (property.ClrType == typeof(DateTime))
                {
                    generatedValue = NextDateTimeSequenceValue();
                }
                else if (property.ClrType == typeof(String))
                {
                    string strValue = Guid.NewGuid().ToString();
                    if (property.MaximumLength.HasValue && strValue.Length > property.MaximumLength)
                    {
                        strValue = strValue.Substring(0, property.MaximumLength.Value);
                    }
                    generatedValue = strValue;
                }
                else if (
                            property.ClrType == typeof(Decimal) ||
                            property.ClrType == typeof(Single) ||
                            property.ClrType == typeof(Double)
                        )
                {
                    generatedValue = Convert.ChangeType(
                                            NextPrimitiveSequenceValue(typeof(Int32)),
                                            property.ClrType
                                        );
                }
                else
                {
                    Debug.Fail("Unsupported key type", property.ClrType.Name);
                }

                property.SetValue(entityObject, generatedValue);
            }
        }

        private void ValidatePrimitiveConstraints(object entityObject, CustomEntityType entityType)
        {
            foreach (var member in entityType.Members)
            {
                object currValue = member.GetValue(entityObject);

                if (null == currValue)
                {
                    if (member.IsNullable)
                    {
                        continue;
                    }
                    else
                    {
                        throw new CustomStoreConstraintViolationException(String.Format(
                                        "Property '{0}' of the object '{1}' cannot be null.",
                                        member, entityObject
                                    ));
                    }
                }

                if (member is CustomPropertyType)
                {
                    var property = (CustomPropertyType)member;
                    Debug.Assert(!(property.IsPrimaryKey && property.IsNullable));

                    if (property.IsPrimaryKey && null == property.ForeignKeyConstraint)
                    {
                        if (CustomEntityKey.IsDefaultKeyValue(property, currValue))
                        {
                            throw new CustomStoreConstraintViolationException(String.Format(
                                            "Primary key property '{0}.{1}' of the object '{2}' " +
                                            "cannot be set to its default value {3} when saved. " +
                                            "Only primary key properties being foreign keys are " +
                                            "allowed to have its default values. Did you forget to" +
                                            "annotate the property with '{4}'?",
                                            entityType, member, entityObject, currValue,
                                            typeof(Metadata.CustomForeignKeyConstraintAttribute).Name
                                        ));
                        }
                    }

                    if (property.MaximumLength.HasValue)
                    {
                        int actualLength;
                        bool lengthFound = TryGetLength(currValue, out actualLength);

                        Debug.Assert(lengthFound);

                        if (actualLength > property.MaximumLength.Value)
                        {
                            throw new CustomStoreConstraintViolationException(String.Format(
                                            "Length of the property '{0}.{1}' of the object '{2}' " +
                                            "must be less or equal to {3}. The actual value is {4}.",
                                            entityType, member, entityObject, property.MaximumLength.Value,
                                            actualLength
                                        ));
                        }
                    }
                }
            }
        }

        private bool TryGetLength(object obj, out int length)
        {
            Debug.Assert(null != obj);
            var property = obj.GetType().GetProperty("Length");

            if (null == property || null == property.GetGetMethod())
            {
                length = 0;
                return false;
            }

            length = (int)property.GetValue(obj, null);
            return true;
        }

        protected DateTime NextDateTimeSequenceValue()
        {
            _dateTimeSequenceValue += TimeSpan.FromDays(1);
            return _dateTimeSequenceValue;
        }

        protected object NextPrimitiveSequenceValue(Type primitiveType)
        {
            Debug.Assert(primitiveType.IsPrimitive);

            long currValue;
            if (!_primitiveSequenceValues.TryGetValue(primitiveType.Name, out currValue))
            {
                long startValue = 0;

#if !_NOT_ASTORIA_TEST_FRAMEWORK
                // stupid workaround to avoid exceptions about key conflicts with
                // some of the Astoria tests trying to set specific ID like 1 or 100
                // before adding new object
                if (
                    typeof(Int16) == primitiveType ||
                    typeof(Int32) == primitiveType ||
                    typeof(Int64) == primitiveType
                )
                {
                    startValue = 12345;
                }
#endif

                _primitiveSequenceValues[primitiveType.Name] = currValue = startValue;
            }

            try
            {
                long nextValue = currValue + 1;
                _primitiveSequenceValues[primitiveType.Name] = nextValue;
                return Convert.ChangeType(nextValue, primitiveType);
            }
            catch (OverflowException ex)
            {
                throw new CustomStoreException(
                                String.Format(
                                    "Store has ran out of available values for sequence of type '{0}'.",
                                    primitiveType
                                ),
                                ex
                            );
            }
        }

        #endregion

        #region IEnumerable members

        public IEnumerator GetEnumerator()
        {
            return GetEnumeratorInternal();
        }

        public abstract IEnumerator GetEnumeratorInternal();

        #endregion
    }

    #endregion

    //
    // CustomEntitySetOfT<EntityObjectType> class
    //
    #region CustomEntitySetOfT<EntityObjectType> class

    [DataContract(IsReference = true)]
    public class CustomEntitySetOfT<EntityObjectType>
                        : CustomEntitySet
                        , IEnumerable<EntityObjectType>
    {
        #region Private fields

        [DataMember]
        internal HashSet<EntityObjectType> _innerSet = new HashSet<EntityObjectType>();

        #endregion

        #region Constructos

        public CustomEntitySetOfT(
                    CustomMetadataWorkspace metadata,
                    CustomEntitySetType entitySetType,
                    CustomEntitySetContainer parentContainer
                )
            : base(metadata, entitySetType, parentContainer)
        {
        }

        internal CustomEntitySetOfT()
            : base(true)
        {
        }

        #endregion

        #region Properties

        public override int Count
        {
            get { return _innerSet.Count; }
        }

        #endregion

        #region Data access methods

        public override IEnumerable EntityObjects
        {
            get
            {
                Debug.Assert(_innerSet is IEnumerable);
                return ((IEnumerable)_innerSet);
            }
        }

        protected override void AddInternal(object entityObject)
        {
            if (!_innerSet.Add((EntityObjectType)entityObject))
            {
                throw new CustomStoreException(String.Format(
                                "Entity object '{0}' is already added to the entity set '{1}'",
                                entityObject, EntitySetType.Name
                            ));
            }
        }

        protected override void RemoveInternal(object entityObject)
        {
            bool removed = _innerSet.Remove((EntityObjectType)entityObject);
            if (!removed)
            {
                ThrowEntityObjectNotExist(entityObject);
            }
        }

        protected override void UpdateInternal(object entityObject)
        {
            if (!_innerSet.Contains((EntityObjectType)entityObject))
            {
                ThrowEntityObjectNotExist(entityObject);
            }
        }

        private void ThrowEntityObjectNotExist(object entityObject)
        {
            throw new CustomStoreObjectNotFoundException(String.Format(
                            "Entity object '{0}' does not exist in the entity set '{1}'.",
                            entityObject, EntitySetType.Name
                        ));
        }

        #endregion

        #region IEnumerable members

        public new IEnumerator<EntityObjectType> GetEnumerator()
        {
            return _innerSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumeratorInternal();
        }

        public override IEnumerator GetEnumeratorInternal()
        {
            return _innerSet.GetEnumerator();
        }

        #endregion
    }

    #endregion

    //
    // CustomStoreException class
    //
    #region CustomStoreException class

    public class CustomStoreException : Exception
    {
        public CustomStoreException()
        {
        }

        public CustomStoreException(string message)
            : base(message)
        {
        }

        public CustomStoreException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    #endregion

    //
    // CustomStoreObjectDoesNotExistException class
    //
    #region CustomStoreObjectDoesNotExistException class

    public class CustomStoreObjectNotFoundException : CustomStoreException
    {
        public CustomStoreObjectNotFoundException()
        {
        }

        public CustomStoreObjectNotFoundException(string message)
            : base(message)
        {
        }
    }

    #endregion

    //
    // CustomStoreConstraintViolationException class
    //
    #region CustomStoreConstraintViolationException class

    public class CustomStoreConstraintViolationException : CustomStoreException
    {
        public CustomStoreConstraintViolationException()
        {
        }

        public CustomStoreConstraintViolationException(string message)
            : base(message)
        {
        }
    }

    #endregion

    // --- CODE SNIPPET END ---
}
