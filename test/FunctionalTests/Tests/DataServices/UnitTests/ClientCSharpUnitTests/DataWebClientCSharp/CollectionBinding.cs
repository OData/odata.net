//---------------------------------------------------------------------
// <copyright file="CollectionBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.DataWebClientCSharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    /// <summary>
    /// Tests class for databinding of collection properties on the client
    /// </summary>
    [TestClass]
    public class CollectionBinding
    {
        private static CollectionValues intCollectionValues;
        private static CollectionValues stringCollectionValues;
        private static CollectionValues complexWithPrimitivesCollectionValue;
        private static CollectionValues complexWithNestedComplexCollectionValues;

        private static TestInfo[] allTestInfos;

        // Initializes standard test data for different data types used by various tests
        [ClassInitialize]        
        public static void CreateCollectionValues(TestContext context)
        {
            int counter = 1;

            intCollectionValues = new CollectionValues(counter++, counter++, counter++, counter++);
            stringCollectionValues = new CollectionValues("StringSingle", "StringDuplicate", "StringExtra", "StringNew");
            complexWithPrimitivesCollectionValue = 
                new CollectionValues(
                    new ComplexTypeWithPrimitives(counter++),
                    new ComplexTypeWithPrimitives(counter++),
                    new ComplexTypeWithPrimitives(counter++),
                    new ComplexTypeWithPrimitives(counter++));
            complexWithNestedComplexCollectionValues =
                new CollectionValues(
                    new ComplexTypeWithNestedComplexType(counter++, counter++),
                    new ComplexTypeWithNestedComplexType(counter++, counter++),
                    new ComplexTypeWithNestedComplexType(counter++, counter++),
                    new ComplexTypeWithNestedComplexType(counter++, counter++));

            allTestInfos = new TestInfo[] {
                // Entity with non-nullable primitive collection type
                new TestInfo {
                    EntityType = typeof(EntityWithCollection<ObservableCollection<int>>),
                    CollectionType = typeof(ObservableCollection<int>),
                    CollectionItemType = typeof(int)
                },
                // Entity with nullable<T> collection type
                new TestInfo {
                    EntityType = typeof(EntityWithCollection<ObservableCollection<int?>>),
                    CollectionType = typeof(ObservableCollection<int?>),
                    CollectionItemType = typeof(int?)
                },
                // Entity with string collection type
                new TestInfo { 
                    EntityType = typeof(EntityWithCollection<ObservableCollection<string>>),
                    CollectionType = typeof(ObservableCollection<string>),
                    CollectionItemType = typeof(string)
                },
                // Entity with collection of simple complex type
                new TestInfo {
                    EntityType = typeof(EntityWithCollection<ObservableCollection<ComplexTypeWithPrimitives>>),
                    CollectionType = typeof(ObservableCollection<ComplexTypeWithPrimitives>),
                    CollectionItemType = typeof(ComplexTypeWithPrimitives)
                },
                // Entity with collection of complex types with nested complex type
                new TestInfo {
                    EntityType = typeof(EntityWithCollection<ObservableCollection<ComplexTypeWithNestedComplexType>>),
                    CollectionType = typeof(ObservableCollection<ComplexTypeWithNestedComplexType>),
                    CollectionItemType = typeof(ComplexTypeWithNestedComplexType)
                },
                // Entity with complex type with collection of primitives
                new TestInfo {
                    EntityType = typeof(EntityWithComplex<ComplexTypeWithCollection<int>>),
                    CollectionType = typeof(ObservableCollection<int>),
                    CollectionItemType = typeof(int)
                },
                // Entity with complex type with collection of simple complex types
                new TestInfo {
                    EntityType = typeof(EntityWithComplex<ComplexTypeWithCollection<ComplexTypeWithPrimitives>>),
                    CollectionType = typeof(ObservableCollection<ComplexTypeWithPrimitives>),
                    CollectionItemType = typeof(ComplexTypeWithPrimitives)
                },
                // Entity with complex type with collection of complex type with nested complex type
                new TestInfo {
                    EntityType = typeof(EntityWithComplex<ComplexTypeWithCollection<ComplexTypeWithNestedComplexType>>),
                    CollectionType = typeof(ObservableCollection<ComplexTypeWithNestedComplexType>),
                    CollectionItemType = typeof(ComplexTypeWithNestedComplexType)
                },
                // Entity with complex type with a collection of complex types with a collection of primitives
                new TestInfo {
                    EntityType = typeof(EntityWithComplex<ComplexTypeWithCollection<ComplexTypeWithCollection<int>>>),
                    CollectionType = typeof(ObservableCollection<int>),
                    CollectionItemType = typeof(int)
                },
                // Entity with collection of complex type with a collection of complex types with a collection of primitives
                new TestInfo {
                    EntityType = typeof(EntityWithCollection<ObservableCollection<ComplexTypeWithCollection<ComplexTypeWithCollection<int>>>>),
                    CollectionType = typeof(ObservableCollection<int>),
                    CollectionItemType = typeof(int)
                },
                // Entity with complex type with a collection of complex types with a collection of simple complex types
                new TestInfo {
                    EntityType = typeof(EntityWithComplex<ComplexTypeWithCollection<ComplexTypeWithCollection<ComplexTypeWithPrimitives>>>),
                    CollectionType = typeof(ObservableCollection<ComplexTypeWithPrimitives>),
                    CollectionItemType = typeof(ComplexTypeWithPrimitives)
                },
                // Entity with collection of complex type with a collection of complex types with a collection of simple complex types
                new TestInfo {
                    EntityType = typeof(EntityWithCollection<ObservableCollection<ComplexTypeWithCollection<ComplexTypeWithCollection<ComplexTypeWithPrimitives>>>>),
                    CollectionType = typeof(ObservableCollection<ComplexTypeWithPrimitives>),
                    CollectionItemType = typeof(ComplexTypeWithPrimitives)
                },
                // Entity with complex type with a collection of complex types with a collection of complex types with nested complex type
                new TestInfo {
                    EntityType = typeof(EntityWithComplex<ComplexTypeWithCollection<ComplexTypeWithCollection<ComplexTypeWithNestedComplexType>>>),
                    CollectionType = typeof(ObservableCollection<ComplexTypeWithNestedComplexType>),
                    CollectionItemType = typeof(ComplexTypeWithNestedComplexType)
                },
                // Entity with collection of complex type with a collection of complex types with a collection of complex types with nested complex type
                new TestInfo {
                    EntityType = typeof(EntityWithCollection<ObservableCollection<ComplexTypeWithCollection<ComplexTypeWithCollection<ComplexTypeWithNestedComplexType>>>>),
                    CollectionType = typeof(ObservableCollection<ComplexTypeWithNestedComplexType>),
                    CollectionItemType = typeof(ComplexTypeWithNestedComplexType)
                },
                // Entity with non-generic non-nullable primitive collection type
                new TestInfo { 
                    EntityType = typeof(EntityWithCollection<IntCollection>),
                    CollectionType = typeof(IntCollection),
                    CollectionItemType = typeof(int)
                },
                // Entity with non-generic Nullable<T> collection type
                new TestInfo { 
                    EntityType = typeof(EntityWithCollection<NullableIntCollection>),
                    CollectionType = typeof(NullableIntCollection),
                    CollectionItemType = typeof(int?)
                },                
                // Entity with non-generic string collection type
                new TestInfo { 
                    EntityType = typeof(EntityWithCollection<StringCollection>),
                    CollectionType = typeof(StringCollection),
                    CollectionItemType = typeof(string)
                },
                // Entity with non-generic complex collection type
                // This is sufficient to test that a non-generic collection type for complex types works, no need to test with all combinations of complex types
                new TestInfo {
                    EntityType = typeof(EntityWithCollection<ComplexCollection>),
                    CollectionType = typeof(ComplexCollection),
                    CollectionItemType = typeof(ComplexTypeWithPrimitives)
                },
            };
        }

        #region Test cases
        [Ignore] // Remove Atom
        // [TestMethod]
        public void CollectionMembershipChanged()
        {
            // Modify membership on collection property
            RunTestCases(allTestInfos, typeof(MembershipTest<,,>));
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void ComplexTypePropertyChanges()
        {
            // Modify properties on complex objects in collection
            // Run only on test infos that have test collection that contain complex types
            TestInfo[] complexTestInfos = allTestInfos.Where(tti => typeof(ComplexTypeBase).IsAssignableFrom(tti.CollectionItemType)).ToArray();
            RunTestCases(complexTestInfos, typeof(ComplexPropertyChangesTest<,,>));
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void ErrorOnMultipleReferencesToSameCollection()
        {
            // Negative: Multiple references to same collection in binding graph
            var entityTypes = new Type[]
            {
                // Entity with collection of primitives
                typeof(EntityWithCollection<IntCollection>),
                // Entity with collection of complex types
                typeof(EntityWithCollection<ComplexCollection>),
                // Entity with complex type with collection of primitives
                typeof(EntityWithComplex<ComplexTypeWithCollection<string>>),
                // Entity with collection of complex type with collection of primitives
                typeof(EntityWithCollection<ObservableCollection<ComplexTypeWithCollection<int>>>)
            };

            TestUtil.RunCombinations(entityTypes, (entityType) =>
            {
                EntityBase entity1 = (EntityBase)Activator.CreateInstance(entityType);
                entity1.ID = 1;
                Assert.IsNotNull(entity1.TestCollection, "Expected TestCollection to be automatically created by the parent type");
                EntityBase entity2 = (EntityBase)Activator.CreateInstance(entityType);
                entity2.ID = 2;
                Assert.IsNotNull(entity2.TestCollection, "Expected TestCollection to be automatically created by the parent type");

                this.GetType()
                    .GetMethod("ErrorOnMultipleReferencesToSameCollection", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entityType)
                    .Invoke(this, new object[] { entity1, entity2 });
            });
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void ErrorOnCollectionDoesntImplementINotifyCollectionChanged()
        {
            // Negative: Collection type doesn't implement ICollectionChanged
            var testInfos = new TestInfo[] {
                // Entity with primitive type collection that doesn't implement INotifyCollectionChanged
                new TestInfo {
                    EntityType = typeof(EntityWithCollection<List<int>>),
                    CollectionType = typeof(List<int>),
                    CollectionItemType = typeof(int)
                },
                // Entity with complex type collection that doesn't implement INotifyCollectionChanged
                new TestInfo {
                    EntityType = typeof(EntityWithCollection<List<ComplexTypeWithPrimitives>>),
                    CollectionType = typeof(List<ComplexTypeWithPrimitives>),
                    CollectionItemType = typeof(ComplexTypeWithPrimitives)
                },
                // Entity with collection of complex type with a collection that doesn't implement INotifyCollectionChanged
                new TestInfo {
                    EntityType = typeof(EntityWithCollection<ObservableCollection<ComplexTypeWithCollectionBase<List<int>>>>),
                    CollectionType = typeof(List<int>),
                    CollectionItemType = typeof(int)
                },
                // Entity with a complex type with a primitive type collection that doesn't implement INotifyCollectionChanged
                new TestInfo {
                    EntityType = typeof(EntityWithComplex<ComplexTypeWithCollectionBase<List<int>>>),
                    CollectionType = typeof(List<int>),
                    CollectionItemType = typeof(int)
                },
                // Entity with a complex type with a collection of complex type with a collection that doesn't implement INotifyCollectionChanged
                new TestInfo {
                    EntityType = typeof(EntityWithComplex<ComplexTypeWithCollection<ComplexTypeWithCollectionBase<List<int>>>>),
                    CollectionType = typeof(List<int>),
                    CollectionItemType = typeof(int)
                },
            };

            RunTestCases(testInfos, typeof(ErrorOnCollectionChangedNotImplemented<,,>));
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void ErrorOnComplexTypeDoesntImplementINotifyPropertyChanged()
        {
            // Negative: Complex type doesn't implement INotifyPropertyChanged
            var entityTypes = new Type[]
            {
                // Entity with collection of complex type that doesn't implement INPC
                typeof(EntityWithCollection<ObservableCollection<ComplexTypeWithoutINPC>>),
                // Entity with complex type with collection of complex type that doesn't implement INPC
                typeof(EntityWithComplex<ComplexTypeWithCollection<ComplexTypeWithoutINPC>>),
                // Entity with collection of complex type with collection of complex type that doesn't implement INPC
                typeof(EntityWithCollection<ObservableCollection<ComplexTypeWithCollection<ComplexTypeWithoutINPC>>>)
            };

            TestUtil.RunCombinations(entityTypes, (entityType) =>
            {
                EntityBase entity = (EntityBase)Activator.CreateInstance(entityType);
                entity.ID = 1;
                Assert.IsNotNull(entity.TestCollection, "Expected TestCollection to be automatically created by the parent type");

                this.GetType()
                    .GetMethod("ErrorOnComplexTypeDoesntImplementINotifyPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entityType)
                    .Invoke(this, new object[] { entity });
            });
        }

        [Ignore] // The case is incorrect that it always add ComplexTypeWithNestedComplexType, but this property may not exist in some type.
        // [TestMethod]
        public void ErrorOnInvalidItemsInCollection()
        {
            // Negative: Invalid items in collection
            // Run only on test infos that have test collection that contain ComplexTypeWithPrimitives
            TestInfo[] complexWithPrimitivesInfos = allTestInfos.Where(tti => typeof(ComplexTypeWithPrimitives) == tti.CollectionItemType).ToArray();
            RunTestCases(complexWithPrimitivesInfos, typeof(InvalidItemsTest<,,>));
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void ErrorOnCollectionChangedWithNullArgs()
        {
            // Negative: CollectionChanged with null arguments
            using (TestWebRequest host = TestWebRequest.CreateForInProcessWcf())
            using (PlaybackService.OverridingPlayback.Restore())
            {
                host.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                host.StartService();

                DataServiceContext ctx = new DataServiceContext(new Uri(host.BaseUri), ODataProtocolVersion.V4);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();
                var dsc = new DataServiceCollection<EntityWithCollection<IntCollection>>(ctx);
                EntityWithCollection<IntCollection> entity = new EntityWithCollection<IntCollection>();
                dsc.Add(entity);

                string expectedExceptionMessage = "Value cannot be null.\r\nParameter name: {0}";
                
                // Fire CollectionChanged event with null sender parameter
                try
                {
                    entity.Collection.FireEventWithNull(senderNull: true, eventArgsNull: false);
                    Assert.Fail("Expected exception was not thrown.");
                }
                catch (ArgumentNullException ex)
                {

                    Assert.AreEqual(
                        String.Format(expectedExceptionMessage, "sender"),
                        ex.Message);
                }

                // Fire CollectionChanged event with null event args parameter
                try
                {
                    entity.Collection.FireEventWithNull(senderNull: false, eventArgsNull: true);
                    Assert.Fail("Expected exception was not thrown.");
                }
                catch (ArgumentNullException ex)
                {

                    Assert.AreEqual(
                        String.Format(expectedExceptionMessage, "e"),
                        ex.Message);
                }
            }
        }
        #endregion

        #region Entity and Complex types used by the tests

        // Base class that implements INotifyPropertyChanged
        public abstract class PropertyChangedBase : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string property)
            {
                if ((this.PropertyChanged != null))
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }
        }
        
        public abstract class EntityBase : PropertyChangedBase
        {
            public int ID
            {
                get
                {
                    return id;
                }
                set
                {
                    id = value;
                    OnPropertyChanged("ID");
                }
            }
            private int id;

            internal abstract object TestCollection { get; set; }
            internal abstract string EntityPayload { get; }
        }

        [EntitySet("Entities")]
        public class EntityWithCollection<CollectionType> : EntityBase
        {
            private bool hasNestedCollection;

            public EntityWithCollection()
            {
                hasNestedCollection = HasNestedCollection(typeof(CollectionType));
            }

            public CollectionType Collection
            {
                get
                {
                    return collection;
                }
                set
                {
                    collection = value;
                    OnPropertyChanged("Collection");
                }
            }
            private CollectionType collection = Activator.CreateInstance<CollectionType>();

            // The collection for test operations could be at some other level of the hierarchy
            internal override object TestCollection
            {
                get
                {
                    // If our collection item type is a complex type with a collection, get the test collection from it, otherwise use the top-level collection
                    if (hasNestedCollection)
                    {
                        ComplexTypeBase firstCollectionItem = GetFirstComplexCollectionItem(this.Collection);
                        return firstCollectionItem.TestCollection;
                    }
                    else
                    {
                        return this.Collection;
                    }
                }
                set
                {
                    // If our collection item type is a complex type with a collection, pass the test collection on to it to be set, otherwise set the top-level collection.
                    if (hasNestedCollection)
                    {
                        ComplexTypeBase firstCollectionItem = GetFirstComplexCollectionItem(this.Collection);
                        firstCollectionItem.TestCollection = value;
                    }
                    else
                    {
                        this.Collection = (CollectionType)value;
                    }
                }
            }

            internal override string EntityPayload
            {
                get
                {
                    return CreateCollectionPayloadFragment(this.Collection);
                }
            }
        }

        [EntitySet("Entities")]
        public class EntityWithComplex<ComplexType> : EntityBase
            where ComplexType : ComplexTypeBase
        {
            public ComplexType Complex
            {
                get
                {
                    if (complex == null)
                    {
                        complex = Activator.CreateInstance<ComplexType>();
                    }
                    return complex;
                }
                set
                {
                    complex = value;
                    OnPropertyChanged("Complex");
                }
            }
            private ComplexType complex;

            internal override object TestCollection
            {
                get
                {
                    return Complex.TestCollection;
                }
                set
                {
                    Complex.TestCollection = value;
                }
            }

            internal override string EntityPayload
            {
                get
                {
                    return String.Format(@"<d:Complex>{0}</d:Complex>", this.Complex.ToString());
                }
            }
        }
        
        // Base class for complex types, tests expect all complex types to be derived from this class
        public abstract class ComplexTypeBase : PropertyChangedBase, ICloneable
        {
            // Test complex types are cloneable because that ensures tests can copy objects in order to avoid duplicate object references in a collection
            public object Clone()
            {
                Type sourceType = this.GetType();
                object clone = Activator.CreateInstance(sourceType);
                foreach (var prop in sourceType.GetProperties())
                {
                    object propValue = prop.GetValue(this, null);
                    ComplexTypeBase complexValue = propValue as ComplexTypeBase;
                    if (complexValue != null)
                    {
                        propValue = complexValue.Clone();
                    }
                    prop.SetValue(clone, propValue, null);
                }
                return clone;
            }

            // Used to create Atom payload for complex types
            public override string ToString()
            {
                StringBuilder complexTypePayloadFragment = new StringBuilder();
                foreach (var prop in this.GetType().GetProperties())
                {
                    object propValue = prop.GetValue(this, null);
                    if (prop.Name == "Collection")
                    {
                        complexTypePayloadFragment.AppendLine(CreateCollectionPayloadFragment(propValue));
                    }
                    else
                    {
                        complexTypePayloadFragment.AppendFormat("<d:{0}>{1}</d:{0}>", prop.Name, propValue ?? "null");
                    } 
                }
                return complexTypePayloadFragment.ToString();
            }

            // Returns the collection on the object that should be used for the test operations.
            // Could be the collection directly on the object, or nested somewhere on another object in the hierarchy.
            // The derived type determines which is the correct collection to use.
            internal virtual object TestCollection
            {
                get
                {
                    Assert.Fail("Test error: Attempt to get a collection property on a type that does not have a collection");
                    return null;
                }
                set
                {
                    Assert.Fail("Test error: Attempt to set a collection property on a type that does not have a collection.");
                }
            }

            // Modifies a primitive property on the complex type
            public virtual void ModifyPrimitive()
            {
                Assert.Fail("Test error: Derived complex type failed to override ModifyPrimitive");
            }
        }

        // Simple complex type class that just has primitive properties
        public class ComplexTypeWithPrimitives : ComplexTypeBase
        {
            public ComplexTypeWithPrimitives()
            {
            }

            public ComplexTypeWithPrimitives(int intVal)
            {
                intPrimitive = intVal;
                stringPrimitive = String.Format("String{0}", intVal);
            }

            public int IntPrimitive
            {
                get
                {
                    return intPrimitive;
                }
                set
                {
                    intPrimitive = value;
                    OnPropertyChanged("IntPrimitive");
                }
            }
            private int intPrimitive;

            public string StringPrimitive
            {
                get
                {
                    return stringPrimitive;
                }
                set
                {
                    stringPrimitive = value;
                    OnPropertyChanged("StringPrimitive");
                }
            }
            private string stringPrimitive;

            public override bool Equals(object obj)
            {
                if (Object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj == null || this.GetType() != obj.GetType())
                {
                    return false;
                }

                ComplexTypeWithPrimitives other = (ComplexTypeWithPrimitives)obj;
                return this.IntPrimitive == other.IntPrimitive && this.StringPrimitive == other.StringPrimitive;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override void ModifyPrimitive()
            {
                StringPrimitive = StringPrimitive + "Modified";
            }            
        }

        // Complex type that has primitives and a collection of the specified type
        public class ComplexTypeWithCollectionBase<CollectionType> : ComplexTypeWithPrimitives
        {
            private bool hasNestedCollection;

            public ComplexTypeWithCollectionBase()
            {
                hasNestedCollection = HasNestedCollection(typeof(CollectionType));
            }

            public CollectionType Collection
            {
                get
                {
                    return collection;
                }
                set
                {
                    collection = value;
                    OnPropertyChanged("Collection");
                }
            }
            private CollectionType collection = Activator.CreateInstance<CollectionType>();

            // The collection for test operations could be at some other level of the hierarchy
            internal override object TestCollection
            {
                get
                {
                    // If our collection item type is a complex type with a collection, get the test collection from it, otherwise use the top-level collection
                    if (hasNestedCollection)
                    {
                        ComplexTypeBase firstCollectionItem = GetFirstComplexCollectionItem(this.Collection);
                        return firstCollectionItem.TestCollection;
                    }
                    else
                    {
                        return this.Collection;
                    }
                }
                set
                {
                    // If our collection item type is a complex type with a collection, pass the test collection on to it to be set, otherwise set the top-level collection.
                    if (hasNestedCollection)
                    {
                        ComplexTypeBase firstCollectionItem = GetFirstComplexCollectionItem(this.Collection);
                        firstCollectionItem.TestCollection = value;
                    }
                    else
                    {
                        this.Collection = (CollectionType)value;
                    }
                }
            }

            public override void ModifyPrimitive()
            {
                // This class contains a collection, which means that the test collection will always be at least one level down
                Assert.Fail("ComplexTypeWithCollection should never be the target of a test that needs to use ModifyPrimitive");
            }
        }

        public class ComplexTypeWithCollection<CollectionItemType> : ComplexTypeWithCollectionBase<ObservableCollection<CollectionItemType>>
        { }

        // Complex type that has a nested simple complex type
        public class ComplexTypeWithNestedComplexType : ComplexTypeWithPrimitives
        {
            public ComplexTypeWithNestedComplexType()
            {
            }

            public ComplexTypeWithNestedComplexType(int intVal, int nestedIntVal)
                : base(intVal)
            {
                nestedComplex = new ComplexTypeWithPrimitives(nestedIntVal);
            }

            public ComplexTypeWithPrimitives NestedComplex
            {
                get
                {
                    if (nestedComplex == null)
                    {
                        nestedComplex = new ComplexTypeWithPrimitives(100);
                    }
                    return nestedComplex;
                }
                set
                {
                    nestedComplex = value;
                    OnPropertyChanged("NestedComplex");
                }
            }
            private ComplexTypeWithPrimitives nestedComplex;

            public override bool Equals(object obj)
            {
                bool baseEquals = base.Equals(obj);
                if (baseEquals)
                {
                    ComplexTypeWithNestedComplexType other = (ComplexTypeWithNestedComplexType)obj;
                    return this.NestedComplex.Equals(other.NestedComplex);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override void ModifyPrimitive()
            {
                NestedComplex.ModifyPrimitive();
            }
        }

        // Complex type that doesn't implement INotifyPropertyChanged
        public class ComplexTypeWithoutINPC
        {
            public int IntProp { get; set; }
        }

        #endregion

        #region Custom collection types that can be used for collection properties

        // Custom collection base class that implements INotifyCollectionChanged but not ICollection<T>
        public abstract class CollectionChangedBaseCollection<T> : INotifyCollectionChanged
        {
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            protected void OnCollectionAdd(T newItem)
            {
                if (CollectionChanged != null)
                {
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem));
                }
            }

            protected void OnCollectionRemove(T oldItem)
            {
                if (CollectionChanged != null)
                {
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem));
                }
            }

            protected void OnCollectionClear()
            {
                if (CollectionChanged != null)
                {
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }

            // Helper method to allow event to be fired with null parameter values
            public void FireEventWithNull(bool senderNull, bool eventArgsNull)
            {
                if (CollectionChanged != null)
                {
                    CollectionChanged(senderNull ? null : this, eventArgsNull ? null : new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        // Custom derived collection type that implements ICollection<T> and uses the base class for firing the collection changed events
        // This class meets the criteria for a type that can be used for a collection property.
        public class CustomCollection<T> : CollectionChangedBaseCollection<T>, ICollection<T>
        {
            private List<T> _list = new List<T>();

            public void Add(T item)
            {
                _list.Add(item);
                OnCollectionAdd(item);
            }

            public void Clear()
            {
                _list.Clear();
                OnCollectionClear();
            }

            public bool Contains(T item)
            {
                return _list.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                _list.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get
                {
                    return _list.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return ((ICollection<T>)_list).IsReadOnly;
                }
            }

            public bool Remove(T item)
            {
                _list.Remove(item);
                // Always fire the event and report that the item was removed even if it was not in the list.
                // This is intentionally different from ObservableCollection, in order to verify this behavior works as well.
                // Expectation is that if the event is fired, the DataServiceCollection will track this as a change to the entity.
                OnCollectionRemove(item);
                return true;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _list.GetEnumerator();
            }
        }

        // Non-generic derived collection types that can be used for a collection property
        public class IntCollection : CustomCollection<int> { }
        public class NullableIntCollection : CustomCollection<int?> { }
        public class StringCollection : CustomCollection<string> { }
        public class ComplexCollection : CustomCollection<ComplexTypeWithPrimitives> { }

        #endregion

        #region Helper classes and methods for test data

        // Holds individual test values for a collection
        public struct CollectionValues
        {
            public CollectionValues(object singleValue, object duplicateValue, object extraValue, object newValue)
            {
                SingleValue = singleValue;
                DuplicateValue = duplicateValue;
                ExtraValue = extraValue;
                NewValue = newValue;
            }

            public readonly object SingleValue;
            public readonly object DuplicateValue;
            public readonly object ExtraValue;
            public readonly object NewValue;
        }

        // Creates an Atom payload fragment for a collection
        private static string CreateCollectionPayloadFragment(object collection)
        {
            string edmCollectionItemTypeName = GetEdmCollectionItemTypeName(collection.GetType());

            StringBuilder collectionPayload = new StringBuilder();
            collectionPayload.AppendFormat(
                @"<d:Collection m:type=""Collection({0})"">",
                edmCollectionItemTypeName);
            foreach (object collectionValue in (IEnumerable)collection)
            {
                collectionPayload.AppendFormat(@"<m:element>{0}</m:element>", collectionValue);
            }
            collectionPayload.AppendLine(@"</d:Collection>");
            return collectionPayload.ToString();
        }

        // Creates a full Atom payload for a single entity, using the specified fragment to build any entity properties except ID, which is already included
        private static string CreateAtomPayload(string payloadFragment, string baseUri)
        {
            return String.Format(
@"HTTP/1.1 200 OK
Server: ASP.NET Development Server/10.0.0.0
Date: Wed, 27 Jan 2010 18:06:26 GMT
X-AspNet-Version: 4.0.30107
OData-Version: 4.0;
Set-Cookie: ASP.NET_SessionId=d0ieqfv0tr5pfq4rqafszurj; path=/; HttpOnly
Cache-Control: no-cache
Content-Type: application/atom+xml;charset=utf-8
Connection: Close

<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<feed xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <title type=""text"">Entities</title>
  <id>{0}/Entities</id>
  <updated>2010-01-27T18:06:26Z</updated>
  <link rel=""self"" title=""Entities"" href=""Entities"" />
  <entry>
    <id>{0}/Entities(5)</id>
    <title type=""text""></title>
    <updated>2010-01-27T18:06:26Z</updated>
    <author>
      <name />
    </author>
    <link rel=""edit"" title=""Entity"" href=""Entities(5)"" />
    <category term=""#WebApplication1.Entity"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <content type=""application/xml"">
      <m:properties>
        <d:ID m:type=""Edm.Int32"">5</d:ID>
        {1}
      </m:properties>
    </content>
  </entry>
</feed>",
            baseUri,
            payloadFragment);
        }

        // Creates a basic successful HTTP response that can be used for a response to SaveChanges calls
        private static string CreateSimpleHTTPResponse(string uri)
        {
            return
                "HTTP/1.1 200 OK\r\n" +
                "Content-Length: 0\r\n" +
                "Location: " + uri + "/\r\n" +
                "\r\n";
        }

        #endregion

        #region Helper methods for entity and complex type classes

        // Gets the first item out of a collection of complex types
        private static ComplexTypeBase GetFirstComplexCollectionItem(object collection)
        {
            object firstCollectionItem = null;
            foreach (object item in (IEnumerable)collection)
            {
                firstCollectionItem = item;
                break;
            }

            if (firstCollectionItem == null)
            {
                // There were no items in the collection, so add one now
                Type collectionType = collection.GetType();
                firstCollectionItem = Activator.CreateInstance(GetCollectionItemType(collectionType));
                collectionType.GetMethod("Add").Invoke(collection, new object[] { firstCollectionItem });
            }

            return (ComplexTypeBase)firstCollectionItem;
        }

        private static Type GetCollectionItemType(Type collectionType)
        {
            Type collectionItemType = null;
            foreach (var itf in collectionType.GetInterfaces())
            {
                if (itf.IsGenericType && typeof(ICollection<>) == itf.GetGenericTypeDefinition())
                {
                    Assert.IsNull(collectionItemType, "Test error: Found multiple implementations of ICollection<T> on collection type {0}", collectionType);
                    collectionItemType = itf.GetGenericArguments()[0];
                }
            }
            Assert.IsNotNull(collectionItemType, "Test error: Couldn't find ICollection<T> implementation on collection type {0}", collectionType);
            return collectionItemType;
        }

        private static string GetEdmCollectionItemTypeName(Type collectionType)
        {
            Type collectionItemType = GetCollectionItemType(collectionType);
            if (collectionItemType == typeof(int) || collectionItemType == typeof(int?))
            {
                return "Edm.Int32";
            }
            else if (collectionItemType == typeof(string))
            {
                return "Edm.String";
            }
            else
            {
                Assert.IsTrue(typeof(ComplexTypeBase).IsAssignableFrom(collectionItemType), "Test error: Unexpected collection item type found, {0}", collectionItemType);
                return collectionItemType.FullName;
            }
        }

        private static bool HasNestedCollection(Type collectionType)
        {
            Type collectionItemType = GetCollectionItemType(collectionType);
            while (collectionItemType != null)
            {
                if (collectionItemType.IsGenericType && typeof(ComplexTypeWithCollectionBase<>) == collectionItemType.GetGenericTypeDefinition())
                {
                    return true;
                }
                collectionItemType = collectionItemType.BaseType;
            }
            return false;
        }

        private static void VerifyObject(DataServiceContext ctx, object entity, EntityStates state)
        {
            var descriptor = ctx.GetEntityDescriptor(entity);
            Assert.IsNotNull(descriptor, "Entity not being tracked by the context.");
            Assert.AreEqual(state, descriptor.State, "Entity descriptor state is different.");
        }

        #endregion

        #region Helper classes and methods for running test cases

        // Holds entity and collection type information for a test case
        public class TestInfo
        {
            // Entity type to use for the test
            public Type EntityType { get; set; }

            // Collection type to use for the test (may be a nested collection)
            public Type CollectionType { get; set; }

            // Type of the item in CollectionType
            public Type CollectionItemType
            {
                get
                {
                    return collectionItemType;
                }
                set
                {
                    collectionItemType = value;

                    if (collectionItemType == typeof(int) || collectionItemType == typeof(int?))
                    {
                        collectionValues = intCollectionValues;
                    }
                    else if (collectionItemType == typeof(string))
                    {
                        collectionValues = stringCollectionValues;
                    }
                    else if (collectionItemType == typeof(ComplexTypeWithPrimitives))
                    {
                        collectionValues = complexWithPrimitivesCollectionValue;
                    }
                    else if (collectionItemType == typeof(ComplexTypeWithNestedComplexType))
                    {
                        collectionValues = complexWithNestedComplexCollectionValues;
                    }
                    else
                    {
                        Assert.Fail("Test error: No collection values found for type {0}", collectionItemType);
                    }
                }
            }
            private Type collectionItemType;

            // Test values in the collection, based on the collectionItemTypes
            public CollectionValues CollectionValues
            {
                get
                {
                    return collectionValues;
                }
            }
            private CollectionValues collectionValues;
        }

        private void RunTestCases(TestInfo[] testInfos, Type testCaseType)
        {
            using (TestWebRequest host = TestWebRequest.CreateForInProcessWcf())
            using (PlaybackService.OverridingPlayback.Restore())
            {
                host.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                host.StartService();

                TestUtil.RunCombinations(testInfos, (testInfo) =>
                {
                    Type testType = testCaseType.MakeGenericType(testInfo.EntityType, testInfo.CollectionType, testInfo.CollectionItemType);
                    object testCase = Activator.CreateInstance(testType, host, testInfo.CollectionValues);
                    testType.GetMethod("RunTests").Invoke(testCase, null);
                });
            }
        }

        // Base test class that contains common functionality
        private abstract class CollectionTestBase<EntityType, CollectionType, CollectionItemType>
            where CollectionType : ICollection<CollectionItemType>, new()
            where EntityType : EntityBase
        {
            private readonly TestWebRequest host;
            private readonly CollectionValues collectionValues;
            private Func<EntityChangedParams, bool> entityChangedHandler;
            private readonly bool isComplexCollection;
            
            private DataServiceContext context;

            protected CollectionTestBase(TestWebRequest webhost, CollectionValues values)
            {
                // Tests are expecting certain data types, so verify we are only using those types
                Assert.IsTrue(
                    typeof(CollectionItemType) == typeof(int) ||
                    typeof(CollectionItemType) == typeof(int?) ||
                    typeof(CollectionItemType) == typeof(string) ||
                    typeof(ComplexTypeBase).IsAssignableFrom(typeof(CollectionItemType)),
                    "Test error: The only supported test collection item types are int, int?, string, and ComplexTypeBase-derived types.");

                host = webhost;
                collectionValues = values;
                isComplexCollection = typeof(ComplexTypeBase).IsAssignableFrom(typeof(CollectionItemType));
            }

            public void RunTests()
            {
                var entityGetters = new Func<EntityType>[] { MaterializeEntity, CreateEntity };
                TestUtil.RunCombinations(entityGetters, (entityGetter) =>
                {
                    RunTestOnEntity(entityGetter);
                });
            }

            protected abstract void RunTestOnEntity(Func<EntityType> getEntity);

            protected DataServiceContext Context
            {
                get
                {
                    return this.context;
                }
            }

            protected bool IsComplexCollection
            {
                get
                {
                    return this.isComplexCollection;
                }
            }

            protected void SetEntityChangedHandler(Func<EntityChangedParams, bool> entityChangedHandler)
            {
                this.entityChangedHandler = entityChangedHandler;
            }

            protected EntityType Setup(Func<EntityType> getEntity, Exception expectedLoadException = null)
            {
                // Create payload
                string entityPayload = CreateEntity().EntityPayload;
                PlaybackService.OverridingPlayback.Value = CreateAtomPayload(entityPayload, host.BaseUri);
                // Create and set test context
                context = new DataServiceContext(new Uri(host.BaseUri), ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                // Create DataServiceCollection for the specified entity type
                DataServiceCollection<EntityType> dsc = new DataServiceCollection<EntityType>(context, null, TrackingMode.AutoChangeTracking, null, this.entityChangedHandler, null);
                // Get an entity with a collection property to use for the test and load it into the DataServiceCollection
                try
                {
                    dsc.Load(getEntity());
                    if (expectedLoadException != null)
                    {
                        Assert.Fail("Expected exception was not thrown.");
                    }
                }
                catch (Exception ex)
                {
                    if (expectedLoadException != null)
                    {
                        Assert.AreEqual(expectedLoadException.GetType(), ex.GetType(), "Wrong exception type.");
                        Assert.AreEqual(expectedLoadException.Message, ex.Message, "Wrong exception message.");
                    }
                    else
                    {
                        throw;
                    }
                }
                EntityType entity = dsc.Single();
                // Reset the playback response to a simple success response so it won't interfere with subsequent SaveChanges calls
                PlaybackService.OverridingPlayback.Value = CreateSimpleHTTPResponse(host.BaseUri);
                // Return the entity to use for the test
                return entity;
            }

            private EntityType MaterializeEntity()
            {
                return context.Execute<EntityType>(new Uri("Entities", UriKind.Relative)).Single();
            }

            private EntityType CreateEntity()
            {
                EntityType newEntity = Activator.CreateInstance<EntityType>();
                newEntity.ID = 5;
                newEntity.TestCollection = CreateCollection();
                return newEntity;
            }

            protected CollectionType CreateCollection()
            {
                CollectionType newCollection = new CollectionType();
                if (IsComplexCollection)
                {
                    // Clone objects so we don't get duplicate object references in the collection
                    newCollection.Add((CollectionItemType)((ComplexTypeBase)collectionValues.SingleValue).Clone());
                    newCollection.Add((CollectionItemType)((ComplexTypeBase)collectionValues.DuplicateValue).Clone());
                    newCollection.Add((CollectionItemType)((ComplexTypeBase)collectionValues.ExtraValue).Clone());
                    newCollection.Add((CollectionItemType)((ComplexTypeBase)collectionValues.DuplicateValue).Clone());
                }
                else
                {
                    newCollection.Add((CollectionItemType)collectionValues.SingleValue);
                    newCollection.Add((CollectionItemType)collectionValues.DuplicateValue);
                    newCollection.Add((CollectionItemType)collectionValues.ExtraValue);
                    newCollection.Add((CollectionItemType)collectionValues.DuplicateValue);
                }

                return newCollection;
            }

            protected CollectionItemType CreateItemAndAddToCollection(object collection)
            {
                CollectionItemType newCollectionItem = CreateNewItem();
                Assert.IsNotNull(newCollectionItem, "Test error: Expected new item to have been created.");
                ((CollectionType)collection).Add(newCollectionItem);
                return newCollectionItem;
            }

            private CollectionItemType CreateNewItem()
            {
                CollectionItemType newCollectionItem;
                if (IsComplexCollection)
                {
                    newCollectionItem = (CollectionItemType)((ComplexTypeBase)collectionValues.NewValue).Clone();
                }
                else
                {
                    newCollectionItem = (CollectionItemType)collectionValues.NewValue;
                }
                return newCollectionItem;
            }

            protected CollectionItemType RemoveFirstItemFromCollection(CollectionType collection)
            {
                CollectionItemType removedItem = GetFirstCollectionItem(collection);
                bool removed = collection.Remove(removedItem);
                Assert.IsTrue(removed, "Test error: Expected Remove to have removed an item from the collection.");
                return removedItem;
            }

            // Get the first item from the collection, or add one if the collection is empty
            protected CollectionItemType GetFirstCollectionItem(CollectionType collection)
            {
                CollectionItemType firstCollectionItem;
                if (collection.Count > 0)
                {
                    firstCollectionItem = collection.First();
                }
                else
                {
                    // If there was nothing in the collection yet, add an item
                    firstCollectionItem = CreateNewItem();
                    collection.Add(firstCollectionItem);
                }
                return firstCollectionItem;
            }
        }

        // Test class for verifying behavior of membership changes to a collection (add/remove/etc)
        private class MembershipTest<EntityType, CollectionType, CollectionItemType> : CollectionTestBase<EntityType, CollectionType, CollectionItemType>
            where CollectionType : ICollection<CollectionItemType>, INotifyCollectionChanged, new()
            where EntityType : EntityBase
        {
            private bool collectionChanged;
            private bool entityChanged;
            private readonly Func<CollectionType, bool>[] collectionActions;

            public MembershipTest(TestWebRequest webhost, CollectionValues collectionValues)
                : base(webhost, collectionValues)
            {
                SetEntityChangedHandler(Entity_PropertyChanged);

                if (IsComplexCollection)
                {
                    // Ensure that the type implements its own Equals (not inherited from a base type) since the tests will rely on this method
                    Assert.AreEqual(typeof(CollectionItemType), typeof(CollectionItemType).GetMethod("Equals").DeclaringType, "Complex type class should override the Equals method");
                }

                // In all cases below, two complex types are considered to have the same value if their properties are the same.
                // These are all positive cases, so the collection should never contain multiple references to the same object instance.
                collectionActions = new Func<CollectionType, bool>[]
                {
                    // Add value that is not already in the collection
                    collection => {
                        VerifyCollectionIsNotEmpty(collection);
                        CollectionItemType collectionTestValue = (CollectionItemType)collectionValues.NewValue;
                        VerifyValueNotInCollection(collection, collectionTestValue);
                        collection.Add(collectionTestValue);
                        return true;
                    },
                    // Add value that is already in the collection
                    collection => {
                        VerifyCollectionIsNotEmpty(collection);
                        CollectionItemType collectionTestValue = (CollectionItemType)collectionValues.SingleValue;
                        VerifyValueIsInCollection(collection, collectionTestValue);
                        collection.Add(collectionTestValue);
                        return true;
                    },
                    // Remove value that is in the collection one time
                    collection => {
                        VerifyCollectionIsNotEmpty(collection);
                        CollectionItemType collectionTestValue = (CollectionItemType)collectionValues.SingleValue;
                        CollectionItemType itemInCollection = GetItemInCollection(collection, collectionTestValue);
                        VerifyValueCountInCollection(collection, collectionTestValue, 1);
                        // Remove the actual instance from the collection so that this has a real effect for complex types
                        collection.Remove(itemInCollection);
                        return true;
                    },
                    // Remove value that is in the collection multiple times
                    collection => {
                        VerifyCollectionIsNotEmpty(collection);
                        CollectionItemType collectionTestValue = (CollectionItemType)collectionValues.DuplicateValue;
                        CollectionItemType itemInCollection = GetItemInCollection(collection, collectionTestValue);
                        VerifyValueCountInCollection(collection, collectionTestValue, 2);
                        // Remove the actual instance from the collection so that this has a real effect for complex types
                        collection.Remove(itemInCollection);
                        return true;
                    },
                    // Remove value that is not in the collection
                    collection => {
                        VerifyCollectionIsNotEmpty(collection);
                        CollectionItemType collectionTestValue = (CollectionItemType)collectionValues.NewValue;
                        VerifyValueNotInCollection(collection, collectionTestValue);
                        // Return value is false for ObservableCollection but true for our custom collection type,
                        // so just rely on this value to determine whether or not the collection is expected to have changed.
                        return collection.Remove(collectionTestValue);
                    },
                    // Clear the collection
                    collection => {
                        VerifyCollectionIsNotEmpty(collection);
                        collection.Clear();
                        return true;
                    },
                    // Replace an item in the collection (method only implemented for ObservableCollection, otherwise it's a no-op)
                    collection => {
                        ObservableCollection<CollectionItemType> observable = collection as ObservableCollection<CollectionItemType>;
                        if (observable != null)
                        {
                            VerifyCollectionIsNotEmpty(collection);
                            CollectionItemType collectionTestValue = (CollectionItemType)collectionValues.NewValue;
                            VerifyValueNotInCollection(collection, collectionTestValue);
                            observable[0] = collectionTestValue;
                            return true;
                        }
                        return false;
                    },
                    // Move an item in the collection (method only implemented for ObservableCollection, otherwise it's a no-op)
                    collection => {
                        ObservableCollection<CollectionItemType> observable = collection as ObservableCollection<CollectionItemType>;
                        if (observable != null)
                        {
                            VerifyCollectionIsNotEmpty(collection);
                            observable.Move(0, 1);
                            return true;
                        }
                        return false;
                    },
                };
            }

            protected override void RunTestOnEntity(Func<EntityType> getEntity)
            {
                TestUtil.RunCombinations(collectionActions, (collectionAction) =>
                {
                    EntityType entity = Setup(getEntity);
                    RunCollectionActionOnCollection(entity, collectionAction);
                });
            }

            // Core method that performs the test operations and verification
            private void RunCollectionActionOnCollection(EntityType entity, Func<CollectionType, bool> collectionAction)
            {
                // Hook up event listener on the collection
                object testCollection = entity.TestCollection; 
                AddCollectionChangedListener(testCollection);
                
                //
                // Test 1: Perform the action (add/remove/etc on collection) and verify entity state
                //
                PerformCollectionActionAndVerifyEntityState(entity, collectionAction);
                this.Context.SaveChanges();

                //
                // Test 2: Replace the entire collection with a new instance, verify the entity is updated
                //
                VerifyObject(this.Context, entity, EntityStates.Unchanged);
                ResetEventFlags();
                object previousCollection = entity.TestCollection;
                entity.TestCollection = CreateCollection();
                Assert.AreNotSame(previousCollection, entity.TestCollection, "Expected the current entity collection instance to be different from the previous value.");
                VerifyObject(this.Context, entity, EntityStates.Modified);
                Assert.IsTrue(this.entityChanged, "Expected Entity_PropertyChanged handler to have been called");
                this.Context.SaveChanges();

                //
                // Test 3: Verify that a change to the disconnected collection does not cause the entity to be updated, but an operation on the new collection does cause an update
                //
                PerformCollectionAction(previousCollection, collection => { CreateItemAndAddToCollection(previousCollection); return true; }, isCollectionConnected:false);
                // Update the test event listener to listen on the new collection and not the old one
                AddCollectionChangedListener(entity.TestCollection);
                RemoveCollectionChangedListener(previousCollection);
                // This will verify that the entity is in the unchanged state before the action is performed, so we know
                // the previous action on the disconnected object did not cause an update.
                PerformCollectionActionAndVerifyEntityState(entity, collectionAction);
            }

            private CollectionItemType GetItemInCollection(CollectionType collection, CollectionItemType searchItem)
            {
                CollectionItemType foundItem;
                Assert.IsTrue(TryGetItemInCollection(collection, searchItem, out foundItem), "Test error: Expected to find the specified value in the collection.");
                return foundItem;
            }

            private bool TryGetItemInCollection(CollectionType collection, CollectionItemType searchItem, out CollectionItemType foundValue)
            {
                if (collection.Any(u => u.Equals((object)searchItem)))
                {
                    foundValue = collection.First(u => u.Equals((object)searchItem));
                    if (this.IsComplexCollection)
                    {
                        Assert.AreNotSame(searchItem, foundValue, "Test error: Item in collection should be a different object than the test item.");
                    }
                    return true;
                }
                else
                {
                    foundValue = default(CollectionItemType);
                    return false;
                }
            }

            private void VerifyValueIsInCollection(CollectionType collection, CollectionItemType searchItem)
            {
                CollectionItemType foundItem;
                Assert.IsTrue(TryGetItemInCollection(collection, searchItem, out foundItem), "Test error: Expected to find the specified value in collection.");
            }

            private void VerifyValueNotInCollection(CollectionType collection, CollectionItemType searchItem)
            {
                CollectionItemType foundItem;
                Assert.IsFalse(TryGetItemInCollection(collection, searchItem, out foundItem), "Test error: Expected not to find the specified value in collection.");
            }

            private void VerifyCollectionIsNotEmpty(CollectionType collection)
            {
                Assert.IsTrue(collection.Count > 1, "Test error: Test collection should always have more than one item.");
            }

            private void VerifyValueCountInCollection(CollectionType collection, CollectionItemType searchValue, int expectedCount)
            {
                Assert.AreEqual(expectedCount, collection.Count(u => u.Equals((object)searchValue)));
            }

            private void AddCollectionChangedListener(object collection)
            {
                // Remove it first to ensure we are not already listening on this collection
                RemoveCollectionChangedListener(collection);
                ((CollectionType)collection).CollectionChanged += Collection_CollectionChanged;
            }

            private void RemoveCollectionChangedListener(object collection)
            {
                ((CollectionType)collection).CollectionChanged -= Collection_CollectionChanged;
            }

            private void PerformCollectionActionAndVerifyEntityState(EntityType entity, Func<CollectionType, bool> collectionAction)
            {
                // Verify the object is not yet changed
                VerifyObject(this.Context, entity, EntityStates.Unchanged);

                bool collectionWasChanged = PerformCollectionAction(entity.TestCollection, collectionAction);

                // If the operation had an actual change, the entity should now be updated
                // In some cases, actions on the collection do not cause updates (e.g. Remove() on a value not in the collection)
                VerifyObject(this.Context, entity, collectionWasChanged ? EntityStates.Modified : EntityStates.Unchanged);
            }

            private bool PerformCollectionAction(object collection, Func<CollectionType, bool> collectionAction, bool isCollectionConnected = true)
            {
                ResetEventFlags();

                // Update the membership in the collection
                bool shouldChangeCollection = collectionAction((CollectionType)collection);
                Assert.AreEqual(shouldChangeCollection, this.collectionChanged, "Collection action did not have the expected effect on the collection.");
                // If the collection is not connected to the entity, the entity should not get updated even if the collection changes
                Assert.AreEqual(shouldChangeCollection && isCollectionConnected, this.entityChanged, "Collection action did not have the expected effect on the entity.");

                return shouldChangeCollection;
            }

            private void ResetEventFlags()
            {
                this.collectionChanged = false;
                this.entityChanged = false;
            }

            private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                this.collectionChanged = true;
            }

            private bool Entity_PropertyChanged(EntityChangedParams entityChangedParams)
            {
                this.entityChanged = true;
                return false;
            }
        }

        // Test class for verifying behavior of changes to complex type properties in a collection
        private class ComplexPropertyChangesTest<EntityType, CollectionType, CollectionItemType> : CollectionTestBase<EntityType, CollectionType, CollectionItemType>
            where CollectionType : ICollection<CollectionItemType>,  new()
            where EntityType : EntityBase
            where CollectionItemType : ComplexTypeBase
        {
            public ComplexPropertyChangesTest(TestWebRequest webhost, CollectionValues collectionData)
                : base(webhost, collectionData)
            {
            }

            protected override void RunTestOnEntity(Func<EntityType> getEntity)
            {
                EntityType entity = Setup(getEntity);
                RunModifyPropertyTest(entity);
            }

            // Core method that performs the test operations and verification
            private void RunModifyPropertyTest(EntityType entity)
            {
                //
                // Test 1: Modify primitive property and verify entity is updated
                //
                ModifyPrimitiveAndVerifyEntityState(entity);
                this.Context.SaveChanges();

                //
                // Test 2: If the collection contains a complex type with a nested complex type,
                //         replace nested complex instance and verify entity is updated
                //
                if (typeof(CollectionItemType) == typeof(ComplexTypeWithNestedComplexType))
                {
                    // Verify the object is not yet changed
                    VerifyObject(this.Context, entity, EntityStates.Unchanged);

                    // Replace the complex object
                    ComplexTypeWithNestedComplexType complex = GetFirstCollectionItem((CollectionType)entity.TestCollection) as ComplexTypeWithNestedComplexType;
                    ComplexTypeWithPrimitives previousNested = complex.NestedComplex;
                    complex.NestedComplex = new ComplexTypeWithPrimitives();
                    
                    // Entity should now be updated
                    VerifyObject(this.Context, entity, EntityStates.Modified);
                    this.Context.SaveChanges();

                    // Verify that changes to the previous object do not cause the entity to be updated
                    ModifyPrimitiveAndVerifyEntityState(entity, previousNested, shouldModify: false);

                    // Verify that changes to the new object do cause the entity to be updated
                    ModifyPrimitiveAndVerifyEntityState(entity, complex.NestedComplex);
                    this.Context.SaveChanges();
                }

                //
                // Test 3: Modify again after SaveChanges and verify entity is updated again
                //
                ModifyPrimitiveAndVerifyEntityState(entity);
                this.Context.SaveChanges();

                //
                // Test 4: Add a new item to the collection and verify changes to it cause the entity to be updated
                //
                // Verify the object is starting in the unchanged state
                VerifyObject(this.Context, entity, EntityStates.Unchanged);
                CollectionItemType newItem = CreateItemAndAddToCollection(entity.TestCollection);
                // Since we just added the new item to the tracked entity, the entity will be updated again, just verify
                VerifyObject(this.Context, entity, EntityStates.Modified);
                // SaveChanges to move the entity back to Unchanged state
                this.Context.SaveChanges();
                // Change the new item and verify it causes the entity to be updated again
                ModifyPrimitiveAndVerifyEntityState(entity, newItem);
                this.Context.SaveChanges();
                
                //
                // Test 5: Remove an item from collection and verify changes to it do not cause the entity to be updated
                //
                // Verify the object is starting in the unchanged state
                VerifyObject(this.Context, entity, EntityStates.Unchanged);
                CollectionItemType removedItem = RemoveFirstItemFromCollection((CollectionType)entity.TestCollection);
                // Since we just remove an item from the tracked entity, the entity will be updated again, just verify
                VerifyObject(this.Context, entity, EntityStates.Modified);
                // SaveChanges to move the entity back to Unchanged state
                this.Context.SaveChanges();
                // Change the removed item and verify it does not cause the entity to be updated
                ModifyPrimitiveAndVerifyEntityState(entity, removedItem, shouldModify : false);
            }            

            private void ModifyPrimitiveAndVerifyEntityState(EntityType entity)
            {
                // If no specific complex object needs to be updated, just use the first item in the collection
                ModifyPrimitiveAndVerifyEntityState(entity, GetFirstCollectionItem((CollectionType)entity.TestCollection));
            }

            private void ModifyPrimitiveAndVerifyEntityState(EntityType entity, ComplexTypeBase complex, bool shouldModify = true)
            {
                // Verify the object is not yet changed
                VerifyObject(this.Context, entity, EntityStates.Unchanged);
                
                complex.ModifyPrimitive();

                // Property has been changed, verify if the entity state is as expected
                VerifyObject(this.Context, entity, shouldModify ? EntityStates.Modified : EntityStates.Unchanged);
            }
        }

        // Test class for verifying an error occurs when a collection type doesn't implement INotifyCollectionChanged
        private class ErrorOnCollectionChangedNotImplemented<EntityType, CollectionType, CollectionItemType> : CollectionTestBase<EntityType, CollectionType, CollectionItemType>
            where CollectionType : ICollection<CollectionItemType>, new()
            where EntityType : EntityBase
        {
            public ErrorOnCollectionChangedNotImplemented(TestWebRequest webhost, CollectionValues collectionData)
                : base(webhost, collectionData)
            {
            }

            protected override void RunTestOnEntity(Func<EntityType> getEntity)
            {
                Setup(
                    getEntity,
                    new InvalidOperationException(
                        String.Format(
                            "An attempt to track an entity or complex type failed because the entity or complex type contains a collection property of type '{0}' that does not implement the INotifyCollectionChanged interface.",
                            typeof(CollectionType))));
            }
        }

        // Test class for verifying behavior when collection contains invalid items (null, invalid type, etc)
        private class InvalidItemsTest<EntityType, CollectionType, CollectionItemType> : CollectionTestBase<EntityType, CollectionType, CollectionItemType>
            where CollectionType : ICollection<CollectionItemType>, INotifyCollectionChanged, new()
            where EntityType : EntityBase
            where CollectionItemType : ComplexTypeWithPrimitives
        {
            private readonly Action<EntityType, CollectionItemType>[] addItemActions;

            public InvalidItemsTest(TestWebRequest webhost, CollectionValues collectionValues)
                : base(webhost, collectionValues)
            {
                addItemActions = new Action<EntityType, CollectionItemType>[]
                {
                    // Add item to the collection
                    (entity, item) => {
                        ((CollectionType)entity.TestCollection).Add(item);
                    },
                    // Create a collection containing item and set it on the entity
                    (entity, item) => {
                        CollectionType newCollection = new CollectionType();
                        newCollection.Add(item);
                        entity.TestCollection = newCollection;
                    }
                };
            }

            protected override void RunTestOnEntity(Func<EntityType> getEntity)
            {
                object[] testItems = new object[] { null, new ComplexTypeWithNestedComplexType() };
                TestUtil.RunCombinations(addItemActions, testItems, (addItemAction, testItem) =>
                {
                    EntityType entity = Setup(getEntity);

                    // Exception should not occur when adding the item to the collection, since we can only do validation after the item is already in the collection.
                    CollectionItemType collectionItem = (CollectionItemType)testItem;
                    addItemAction(entity, collectionItem);
                    CollectionType testCollection = (CollectionType)entity.TestCollection;
                    Assert.IsTrue(testCollection.Contains(collectionItem), "Expected collection to contain item that was just added.");
                    VerifyObject(Context, entity, EntityStates.Modified);

                    try
                    {
                        // Failure should only occur during SaveChanges once the invalid item is detected
                        // Now we support collection to accept derived type
                        Context.SaveChanges();
                    }
                    catch (DataServiceRequestException ex)
                    {
                        string expectedExceptionMessage;
                        InvalidOperationException actualException = ex.InnerException as InvalidOperationException;
                        if (testItem == null)
                        {
                            expectedExceptionMessage = "An item in the collection property has a null value. Collection properties that contain items with null values are not supported.";
                            Assert.IsNotNull(actualException, "Wrong exception type");
                            Assert.IsTrue(actualException.Message.Contains(expectedExceptionMessage), "Message: '{0}' does not contain expected '{1}'", expectedExceptionMessage, actualException.Message);
                        }
                        else
                        {
                            Assert.Fail("This code path should not be reached");
                        }
                    }

                    // Now remove the item and verify we can save successfully
                    ((CollectionType)entity.TestCollection).Remove(collectionItem);
                    Assert.IsFalse(testCollection.Contains(collectionItem), "Expected item to have been removed from collection");
                    VerifyObject(Context, entity, EntityStates.Modified);
                    Context.SaveChanges();

                    // Set the collection property to null and verify no exception occurs until SaveChanges
                    VerifyObject(Context, entity, EntityStates.Unchanged);
                    CollectionType previousCollection = (CollectionType)entity.TestCollection;
                    Assert.IsNotNull(previousCollection, "Collection should not be null yet");
                    entity.TestCollection = null;
                    Assert.IsNull(entity.TestCollection, "Collection should be null now");
                    VerifyObject(Context, entity, EntityStates.Modified);

                    try
                    {
                        // Since the collection property is now null, SaveChanges should fail
                        Context.SaveChanges();
                        Assert.Fail("Expected exception did not occur");
                    }
                    catch (DataServiceRequestException ex)
                    {
                        string expectedExceptionMessage = "The value of the property 'Collection' is null. Properties that are a collection type of primitive or complex types cannot be null.";
                        InvalidOperationException actualException = ex.InnerException as InvalidOperationException;
                        Assert.IsNotNull(actualException, "Wrong exception type");
                        Assert.IsTrue(actualException.Message.Contains(expectedExceptionMessage), "Message: '{0}' does not contain expected '{1}'", expectedExceptionMessage, actualException.Message);      
                    }

                    // Set the collection property to a new non-null value and verify we can successfully save again
                    Assert.IsNull(entity.TestCollection, "Collection should still be null");
                    entity.TestCollection = CreateCollection();
                    VerifyObject(Context, entity, EntityStates.Modified);
                    Context.SaveChanges();

                    // Verify that changes to the previous collection still have no effect on the entity state
                    previousCollection.Add((CollectionItemType)new ComplexTypeWithPrimitives());
                    VerifyObject(Context, entity, EntityStates.Unchanged);
                });
            }
        }

        #endregion
    }
}    
