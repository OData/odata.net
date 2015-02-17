//---------------------------------------------------------------------
// <copyright file="ObjectStructuralDataAdapter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.Objects;

    /// <summary>
    /// Object structural data adapter uses object format and provides manipulation methods 
    /// over structural data in form of key-value pairs where key is a member path and value is a member value. 
    /// </summary>
    internal class ObjectStructuralDataAdapter : IStructuralDataAdapter
    {
        private ConstructorInfo rootConstructorInfo;
        private Dictionary<string, ConstructorInfo> constructorInfos = new Dictionary<string, ConstructorInfo>();
        private Dictionary<string, MemberInfo> memberInfos = new Dictionary<string, MemberInfo>();
        private Dictionary<MemberInfo, CollectionHelper> collectionHelpers = new Dictionary<MemberInfo, CollectionHelper>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectStructuralDataAdapter"/> class.
        /// </summary>
        /// <param name="clrType">Type of the data. Must be constructable type.</param>
        /// <exception cref="TaupoInvalidOperationException">If <paramref name="clrType"/> does not have default public constructor.</exception>
        public ObjectStructuralDataAdapter(Type clrType)
        {
            ExceptionUtilities.CheckArgumentNotNull(clrType, "clrType");
            this.rootConstructorInfo = this.GetConstructorInfo(clrType);
        }

        /// <summary>
        /// Creates data with the specified members' values.
        /// </summary>
        /// <param name="memberValues">Key-value pairs where Key is a member path and Value is a member value.</param>
        /// <returns>New data with the specified members' values</returns>
        public object CreateData(IEnumerable<NamedValue> memberValues)
        {
            object obj = this.rootConstructorInfo.Invoke(null);

            this.SetMemberValues(obj, memberValues);

            return obj;
        }

        /// <summary>
        /// Gets the member value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="data">The object.</param>
        /// <param name="memberPath">The member path.</param>
        /// <returns>Value of the member with specified path.</returns>
        public TValue GetMemberValue<TValue>(object data, string memberPath)
        {
            object currentObj;
            MemberInfo memberInfo;
            this.DeterminePropertyOrField(data, memberPath, out currentObj, out memberInfo, false);

            return (TValue)this.GetMemberValue(currentObj, memberInfo);
        }

        /// <summary>
        /// Sets the member value.
        /// </summary>
        /// <param name="data">The object.</param>
        /// <param name="memberPath">The member path.</param>
        /// <param name="value">The value.</param>
        /// <returns>member value set on the data</returns>
        /// <exception cref="TaupoArgumentNullException">
        /// When <paramref name="data"/> or <paramref name="memberPath"/> is null.
        /// </exception>
        /// <exception cref="TaupoArgumentException">
        /// When <paramref name="memberPath"/> is empty.
        /// </exception>
        /// <exception cref="TaupoInvalidOperationException">
        /// When one of the intermediate member is null and type of this member does not have default public constructor.
        /// When one of the intermediate member does not exist on the object.
        /// </exception>
        public object SetMemberValue(object data, string memberPath, object value)
        {
            object currentObj;
            MemberInfo memberInfo;
            this.DeterminePropertyOrField(data, memberPath, out currentObj, out memberInfo, true);

            value = this.TranslateClrTypeIfEnum(memberInfo, value);
            this.SetMemberValueInternal(currentObj, memberInfo, value);
            return value;
        }

        /// <summary>
        /// Sets the members values. Ignores <paramref name="memberValues"/> if it's null.
        /// </summary>
        /// <param name="data">The object.</param>
        /// <param name="memberValues">Key-value pairs where key is a member path and value is a member value.</param>
        /// <returns>collection of set member values</returns>
        /// <exception >
        /// Throws the same exceptions as <see cref="SetMemberValue"/>
        /// </exception>
        public IEnumerable<NamedValue> SetMemberValues(object data, IEnumerable<NamedValue> memberValues)
        {
            if (memberValues == null)
            {
                return null;
            }

            return this.SetMemberValues(data, memberValues.OrderBy(nv => nv.Name).ToList(), string.Empty);
        }

        /// <summary>
        /// Gets initialized collection member: before returning collection member initializes it if it's null.
        /// </summary>
        /// <param name="data">The data that contains specified collection member.</param>
        /// <param name="memberPath">The collection member path.</param>
        /// <returns>The collection member.</returns>
        public object GetInitializedCollection(object data, string memberPath)
        {
            CollectionHelper collectionHelper;
            object collection = this.GetCollection(data, memberPath, out collectionHelper);
            return collection;
        }

        /// <summary>
        /// Adds an element to a collection with the specified member path.
        /// </summary>
        /// <param name="data">The object.</param>
        /// <param name="memberPath">The collection member path.</param>
        /// <param name="element">The element to add to collection.</param>
        public void AddToCollection(object data, string memberPath, object element)
        {
            CollectionHelper collectionHelper;
            object collection = this.GetCollection(data, memberPath, out collectionHelper);
            collectionHelper.AddTo(collection, this.TranslateClrTypeIfEnum(collectionHelper.ElementType, element));
        }

        /// <summary>
        /// Removes an element from a collection with the specified member path.
        /// </summary>
        /// <param name="data">The object.</param>
        /// <param name="memberPath">The collection member path.</param>
        /// <param name="element">The element to remove from collection.</param>
        /// <returns>true if element was successfully removed from the collection; otherwise, false.
        /// This method also returns false if item is not found in the original collection.
        /// </returns>
        public bool RemoveFromCollection(object data, string memberPath, object element)
        {
            CollectionHelper collectionHelper;
            object collection = this.GetCollection(data, memberPath, out collectionHelper);
            return collectionHelper.RemoveFrom(collection, this.TranslateClrTypeIfEnum(collectionHelper.ElementType, element));
        }

        private IEnumerable<NamedValue> SetMemberValues(object data, List<NamedValue> memberValues, string currentPath)
        {
            var processedMemberValues = new List<NamedValue>();
            while (processedMemberValues.Count != memberValues.Count)
            {
                NamedValue currentNamedValue = memberValues[processedMemberValues.Count];
                int indexOfDot = currentNamedValue.Name.IndexOf('.', currentPath.Length);

                if (indexOfDot < 0)
                {
                    // Primitive value or empty data
                    string currentMemberName = currentNamedValue.Name.Substring(currentPath.Length);
                    object value = currentNamedValue.Value;
                    if (value == EmptyData.Value)
                    {
                        CollectionHelper collectionHelper;
                        ExceptionUtilities.Assert(
                            this.TryGetCollectionHelper(currentMemberName, data.GetType(), out collectionHelper),
                            "Could not get collection helper for path '{0}' that had EmptyData as its value. EmptyData should only be used to represent collections.",
                            currentPath);

                        // get an empty instance of the collection type
                        value = collectionHelper.CreateNewCollection();
                    }

                    var memberInfo = this.GetPropertyOrFieldInfo(currentMemberName, data.GetType());

                    value = this.TranslateClrTypeIfEnum(memberInfo, value);
                    this.SetMemberValueInternal(data, memberInfo, value);
                    processedMemberValues.Add(new NamedValue(currentNamedValue.Name, value));
                }
                else
                {
                    // Collection or complex value
                    string currentMemberName = currentNamedValue.Name.Substring(currentPath.Length, indexOfDot - currentPath.Length);
                    object currentMemberValue = this.GetMemberValue<object>(data, currentMemberName);

                    // For recursion to move forward add "." to the path, so that we move right to the collection elements or members of a complex type.
                    // Example:
                    // "MyComplexProperty.MyCollectionProperty.0", "MyComplexProperty.MyCollectionProperty.1"
                    //      newCurrentPath == "MyComplexProperty."
                    //      currentMemberName == "MyCollectionProperty"
                    // next path == "MyComplexProperty.MyCollectionProperty."
                    string newCurrentPath = currentPath + currentMemberName + ".";
                    List<NamedValue> namedValuesForCurrentMember = memberValues.Where(nv => nv.Name.StartsWith(newCurrentPath, StringComparison.Ordinal)).ToList();

                    CollectionHelper collectionHelper;
                    if (this.TryGetCollectionHelper(currentMemberName, data.GetType(), out collectionHelper))
                    {
                        if (currentMemberValue == null)
                        {
                            currentMemberValue = collectionHelper.CreateNewCollection();
                            this.SetMemberValueInternal(data, this.GetPropertyOrFieldInfo(currentMemberName, data.GetType()), currentMemberValue);
                        }

                        var addedNamedValues = this.SetCollectionMemberValues(collectionHelper, currentMemberValue, namedValuesForCurrentMember, newCurrentPath);
                        processedMemberValues.AddRange(addedNamedValues);
                        namedValuesForCurrentMember = namedValuesForCurrentMember.Where(nv => !addedNamedValues.Any(anv => anv.Name == nv.Name)).ToList();
                    }
                    else if (currentMemberValue == null)
                    {
                        var memberInfo = this.GetPropertyOrFieldInfo(currentMemberName, data.GetType());
                        currentMemberValue = this.GetConstructorInfo(this.GetMemberType(memberInfo)).Invoke(null);
                        this.SetMemberValueInternal(data, memberInfo, currentMemberValue);
                    }

                    if (namedValuesForCurrentMember.Any())
                    {
                        processedMemberValues.AddRange(this.SetMemberValues(currentMemberValue, namedValuesForCurrentMember, newCurrentPath));
                    }
                }
            }

            return processedMemberValues;
        }

        private IEnumerable<NamedValue> SetCollectionMemberValues(CollectionHelper collectionHelper, object collection, IEnumerable<NamedValue> namedValues, string collectionPath)
        {
            collectionHelper.Clear(collection);

            if (collectionHelper.IsCollectionOfPrimitiveType || this.GetTypeIfNullable(collectionHelper.ElementType).IsEnum())
            {
                return this.AddPrimitiveValuesToCollection(collectionHelper, collection, namedValues);
            }
            else
            {
                return this.AddComplexValuesToCollection(collectionHelper, collection, namedValues, collectionPath);
            }
        }

        private IEnumerable<NamedValue> AddPrimitiveValuesToCollection(CollectionHelper collectionHelper, object collection, IEnumerable<NamedValue> namedValues)
        {
            var addedNamedValues = new List<NamedValue>();
            NamedValue namedValue;
            int i = 0;
            while ((namedValue = namedValues.Where(nv => nv.Name.EndsWith("." + i, StringComparison.Ordinal)).SingleOrDefault()) != null)
            {
                var value = this.TranslateClrTypeIfEnum(collectionHelper.ElementType, namedValue.Value);

                collectionHelper.AddTo(collection, value);
                addedNamedValues.Add(new NamedValue(namedValue.Name, value));
                i++;
            }

            return addedNamedValues;
        }

        private IEnumerable<NamedValue> AddComplexValuesToCollection(CollectionHelper collectionHelper, object collection, IEnumerable<NamedValue> namedValues, string collectionPath)
        {
            var addedNamedValues = new List<NamedValue>();
            int i = 0;
            string currentElementPath = collectionPath + i + ".";
            List<NamedValue> elementNamedValues;
            while ((elementNamedValues = namedValues.Where(nv => nv.Name.StartsWith(currentElementPath, StringComparison.Ordinal)).ToList()).Count > 0)
            {
                var element = collectionHelper.CreateNewElement();
                var settedNamedValues = this.SetMemberValues(element, elementNamedValues, currentElementPath);
                collectionHelper.AddTo(collection, element);

                addedNamedValues.AddRange(settedNamedValues);

                i++;
                currentElementPath = collectionPath + i + ".";
            }

            return addedNamedValues;
        }

        private void DeterminePropertyOrField(object data, string memberPath, out object currentObj, out MemberInfo memberInfo, bool createIntermediateMemberIfNull)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(memberPath, "memberPath");

            string[] memberNames = memberPath.Split('.');
            currentObj = data;
            string currentMemberName;
            Type currentMemberParentType = data.GetType();

            for (int i = 0; i < memberNames.Length - 1; i++)
            {
                currentMemberName = memberNames[i];
                memberInfo = this.GetPropertyOrFieldInfo(currentMemberName, currentMemberParentType);
                currentMemberParentType = this.GetMemberType(memberInfo);

                object nextObj = null;
                nextObj = this.GetMemberValue(currentObj, memberInfo);
                if (nextObj == null)
                {
                    if (createIntermediateMemberIfNull)
                    {
                        ConstructorInfo ci = this.GetConstructorInfo(currentMemberParentType);
                        nextObj = ci.Invoke(null);
                        this.SetMemberValueInternal(currentObj, memberInfo, nextObj);
                    }
                    else
                    {
                        string currentPath = string.Join(".", memberNames.Take(i + 1).ToArray());
                        throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Intermediate member value is null. Path: '{0}'.", currentPath));
                    }
                }

                currentObj = nextObj;
            }

            currentMemberName = memberNames[memberNames.Length - 1];
            memberInfo = this.GetPropertyOrFieldInfo(currentMemberName, currentMemberParentType);
        }

        private object GetCollection(object obj, string memberPath, out CollectionHelper collectionHelper)
        {
            object currentObj;
            MemberInfo memberInfo;
            this.DeterminePropertyOrField(obj, memberPath, out currentObj, out memberInfo, true);

            this.TryGetCollectionHelper(memberInfo, out collectionHelper);
            ExceptionUtilities.CheckObjectNotNull(collectionHelper, "Specified member '{0}' is not a collection on type '{1}'.", memberPath, obj.GetType().Name);

            object collection = this.GetMemberValue(currentObj, memberInfo);
            if (collection == null)
            {
                collection = collectionHelper.CreateNewCollection();
                this.SetMemberValueInternal(currentObj, memberInfo, collection);
            }

            return collection;
        }

        private bool TryGetCollectionHelper(string memberName, Type parentType, out CollectionHelper collectionHelper)
        {
            MemberInfo memberInfo = this.GetPropertyOrFieldInfo(memberName, parentType);

            return this.TryGetCollectionHelper(memberInfo, out collectionHelper);
        }

        private bool TryGetCollectionHelper(MemberInfo memberInfo, out CollectionHelper collectionHelper)
        {
            collectionHelper = null;
            if (!this.collectionHelpers.TryGetValue(memberInfo, out collectionHelper))
            {
                Type memberType = this.GetMemberType(memberInfo);
                List<Type> selfAndInterfaces = new List<Type>(memberType.GetInterfaces());
                selfAndInterfaces.Add(memberType);

                var enumerableInterface = selfAndInterfaces.Where(t => t.IsGenericType() && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)).SingleOrDefault();
                var elementType = enumerableInterface != null ? enumerableInterface.GetGenericArguments().Single() : null;
                if (enumerableInterface != null && selfAndInterfaces.Any(
                    t => t.GetMethod("Add", new Type[] { elementType }) != null && t.GetMethod("Remove", new Type[] { elementType }) != null && t.GetMethod("Clear") != null))
                {
                    collectionHelper = new CollectionHelper(memberType, elementType);
                    this.collectionHelpers.Add(memberInfo, collectionHelper);
                }
            }

            return collectionHelper != null;
        }

        private Type GetMemberType(MemberInfo memberInfo)
        {
            PropertyInfo pi = memberInfo as PropertyInfo;
            if (pi != null)
            {
                return pi.PropertyType;
            }

            FieldInfo fi = (FieldInfo)memberInfo;
            return fi.FieldType;
        }

        private object GetMemberValue(object data, MemberInfo memberInfo)
        {
            PropertyInfo pi = memberInfo as PropertyInfo;
            if (pi != null)
            {
                return pi.GetValue(data, null);
            }

            FieldInfo fi = (FieldInfo)memberInfo;
            return fi.GetValue(data);
        }

        private void SetMemberValueInternal(object obj, MemberInfo memberInfo, object value)
        {
            PropertyInfo pi = memberInfo as PropertyInfo;
            if (pi != null)
            {
                pi.SetValue(obj, value, null);
                return;
            }

            FieldInfo fi = (FieldInfo)memberInfo;
            fi.SetValue(obj, value);
        }

        private ConstructorInfo GetConstructorInfo(Type clrType)
        {
            var key = this.GetConstructorInfoKey(clrType);

            ConstructorInfo ci;
            if (!this.constructorInfos.TryGetValue(key, out ci))
            {
                ci = clrType.GetInstanceConstructor(true, PlatformHelper.EmptyTypes);
                if (ci == null)
                {
                    throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Type '{0}' does not have default public constructor.", clrType.FullName));
                }

                this.constructorInfos.Add(key, ci);
            }

            return ci;
        }

        private MemberInfo GetPropertyOrFieldInfo(string memberName, Type parentType)
        {
            if (string.IsNullOrEmpty(memberName))
            {
                throw new TaupoInvalidOperationException("Member name cannot be empty.");
            }

            var key = this.GetMemberInfoKey(memberName, parentType);

            MemberInfo mi;
            if (!this.memberInfos.TryGetValue(key, out mi))
            {
                mi = parentType.GetProperty(memberName, null, false);
                if (mi == null)
                {
                    mi = parentType.GetField(memberName, null, false);
                    if (mi == null)
                    {
                        throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot find non-static property or field '{0}' on type '{1}'.", memberName, parentType.FullName));
                    }
                }

                this.memberInfos.Add(key, mi);
            }

            return mi;
        }

        private string GetMemberInfoKey(string propertyName, Type clrType)
        {
            return clrType.FullName + "." + propertyName;
        }

        private string GetConstructorInfoKey(Type clrType)
        {
            return clrType.FullName;
        }

        private object TranslateClrTypeIfEnum(Type clrType, object value)
        {
            var memberType = this.GetTypeIfNullable(clrType);

            if (memberType.IsEnum())
            {
                value = this.TranslateEnum(memberType, value);
            }

            return value;
        }

        private object TranslateClrTypeIfEnum(MemberInfo memberInfo, object value)
        {
            var memberType = this.GetMemberType(memberInfo);

            return this.TranslateClrTypeIfEnum(memberType, value);
        }

        private object TranslateEnum(Type clrEnumType, object member)
        {
            return DataUtilities.ConvertToEnum(clrEnumType, member);
        }

        private Type GetTypeIfNullable(Type clrType)
        {
            return Nullable.GetUnderlyingType(clrType) ?? clrType;
        }

        /// <summary>
        /// Helper to work with collections.
        /// </summary>
        private class CollectionHelper
        {
            private Type collectionType;
            private ConstructorInfo constructorInfo;
            private ConstructorInfo elementConstructorInfo;

            /// <summary>
            /// Initializes a new instance of the CollectionHelper class.
            /// </summary>
            /// <param name="collectionType">Collection type.</param>
            /// <param name="elementType">Element type.</param>
            public CollectionHelper(Type collectionType, Type elementType)
            {
                this.collectionType = collectionType;
                this.ElementType = elementType;
            }

            /// <summary>
            /// Gets the clr type of the collection elements.
            /// </summary>
            public Type ElementType { get; private set; }

            /// <summary>
            /// Gets a value indicating whether collection element type is primitive.
            /// </summary>
            public bool IsCollectionOfPrimitiveType
            {
                get
                {
                    return TypeUtilities.IsPrimitiveType(this.ElementType);
                }
            }

            /// <summary>
            /// Creates a new element for a colleciton. Can be called only when element type is not primitive.
            /// </summary>
            /// <returns>Created element for a colleciton.</returns>
            public object CreateNewElement()
            {
                if (this.elementConstructorInfo == null)
                {
                    this.elementConstructorInfo = this.ElementType.GetInstanceConstructor(true, PlatformHelper.EmptyTypes);
                }

                ExceptionUtilities.CheckObjectNotNull(
                    this.elementConstructorInfo,
                    "Cannot find default constructor for the collection element type. Colleciton type: '{0}'.",
                    this.collectionType.FullName);

                return this.elementConstructorInfo.Invoke(null);
            }

            /// <summary>
            /// Creates a new instance of a collection.
            /// </summary>
            /// <returns>New instance of a collection</returns>
            public object CreateNewCollection()
            {
                if (this.constructorInfo == null)
                {
                    // If there is a default constructor - use it.
                    this.constructorInfo = this.collectionType.GetInstanceConstructor(true, PlatformHelper.EmptyTypes);

                    // If there is no default constructor - check if colection type is a well known interface.
                    if (this.constructorInfo == null)
                    {
                        Type genericTypeDef = this.collectionType.GetGenericTypeDefinition();
                        if (genericTypeDef == typeof(IList<>) || genericTypeDef == typeof(ICollection<>))
                        {
                            this.constructorInfo = typeof(List<>).MakeGenericType(this.collectionType.GetGenericArguments()).GetInstanceConstructor(true, PlatformHelper.EmptyTypes);
                        }
                    }

                    ExceptionUtilities.CheckObjectNotNull(
                        this.constructorInfo,
                        "Cannot find default constructor for the collection type: '{0}'.",
                        this.collectionType.FullName);
                }

                return this.constructorInfo.Invoke(new object[] { });
            }

            /// <summary>
            /// Adds an element to the collection.
            /// </summary>
            /// <param name="collection">The collection to add element to.</param>
            /// <param name="element">The element to add.</param>
            public void AddTo(object collection, object element)
            {
                collection.GetType().GetMethod("Add").Invoke(collection, new object[] { element });
            }

            /// <summary>
            /// Removes an element from the collection.
            /// </summary>
            /// <param name="collection">The collection to remove element from.</param>
            /// <param name="element">The element to remove.</param>
            /// <returns>True if element was removed from the collection, false otherwise.</returns>
            public bool RemoveFrom(object collection, object element)
            {
                return (bool)collection.GetType().GetMethod("Remove").Invoke(collection, new object[] { element });
            }

            /// <summary>
            /// Clears collection.
            /// </summary>
            /// <param name="collection">The collection to clear.</param>
            public void Clear(object collection)
            {
                collection.GetType().GetMethod("Clear").Invoke(collection, new object[] { });
            }
        }
    }
}
