//   WCF Data Services Entity Framework Provider for OData ver. 1.0.0
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

namespace System.Data.Services
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Diagnostics;
    using System.Linq;
    using System.Data.Services.Providers;
    using System.Xml.Linq;
    #endregion Namespaces

    /// <summary>Utility methods for this project.</summary>
    internal static class WebUtil
    {
        /// <summary>Bindings Flags for public instance members.</summary>
        internal const BindingFlags PublicInstanceBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>A zero-length object array.</summary>
        internal static readonly object[] EmptyObjectArray = new object[0];

        /// <summary>A zero-length string array.</summary>
        internal static readonly string[] EmptyStringArray = new string[0];

        /// <summary>A zero-length keyValuePair of string and object.</summary>
        internal static readonly IEnumerable<KeyValuePair<string, object>> EmptyKeyValuePairStringObject = new KeyValuePair<string, object>[0];

        /// <summary> Cache for PropertyInfo for the given resource type.</summary>
        internal static readonly Dictionary<ResourceType, Dictionary<ResourceProperty, PropertyInfo>> PropertyInfoCache = new Dictionary<ResourceType, Dictionary<ResourceProperty, PropertyInfo>>(EqualityComparer<ResourceType>.Default);

        /// <summary>
        /// Checks the argument value for null and throw ArgumentNullException if it is null
        /// </summary>
        /// <typeparam name="T">type of the argument</typeparam>
        /// <param name="value">argument whose value needs to be checked</param>
        /// <param name="parameterName">name of the argument</param>
        /// <returns>returns the argument back</returns>
        internal static T CheckArgumentNull<T>([ValidatedNotNull] T value, string parameterName) where T : class
        {
            if (null == value)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// Checks the string argument value for empty or null and throw ArgumentNullException if it is null
        /// </summary>
        /// <param name="value">argument whose value needs to be checked</param>
        /// <param name="parameterName">name of the argument</param>
        /// <returns>returns the argument back</returns>
        internal static string CheckStringArgumentNullOrEmpty([ValidatedNotNull] string value, string parameterName)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(parameterName, Strings.WebUtil_ArgumentNullOrEmpty);
            }

            return value;
        }

        /// <summary>
        /// Creates the full name for a custom annotation.
        /// </summary>
        /// <param name="namespaceName">Namespace to which the custom annotation belongs to.</param>
        /// <param name="name">Name of the annotation.</param>
        /// <returns>The full name for the annotation</returns>
        internal static string CreateFullNameForCustomAnnotation(string namespaceName, string name)
        {
            if (!String.IsNullOrEmpty(namespaceName))
            {
                return namespaceName + ":" + name;
            }

            return name;
        }

        /// <summary>Checks whether the specified type is a known primitive type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if the specified type is known to be a primitive type; false otherwise.</returns>
        internal static bool IsPrimitiveType(Type type)
        {
            return type != null && ResourceType.GetPrimitiveResourceType(type) != null;
        }

        /// <summary>
        /// return true if the given property kind is of the given kind
        /// </summary>
        /// <param name="property">property that it's kind needs to be checked</param>
        /// <param name="kind">flag which needs to be checked on property kind</param>
        /// <returns>true if the kind flag is set on the given property kind</returns>
        internal static bool IsOfKind(this ResourceProperty property, ResourcePropertyKind kind)
        {
            return ((property.Kind & kind) == kind);
        }

        /// <summary>Tries to find the property for the specified name.</summary>
        /// <param name="resourceType">resource type that may contain the property</param>
        /// <param name="propertyName">Name of property to resolve.</param>
        /// <returns>Resolved property; possibly null.</returns>
        /// <remarks>This will search ALL properties declared on the type.</remarks>
        internal static ResourceProperty TryResolvePropertyName(this ResourceType resourceType, string propertyName)
        {
            // In case of empty property name this will return null, which means propery is not found
            return resourceType.TryResolvePropertyName(propertyName, 0);
        }

        /// <summary>Tries to find the property for the specified name, excluding the specific kinds of property.</summary>
        /// <param name="resourceType">resource type that may contain the property</param>
        /// <param name="propertyName">Name of property to resolve.</param>
        /// <param name="exceptKind">The property kind to filter out.</param>
        /// <remarks>NamedStream is a special kind of property that should be excluded when querying properties declared on a type. The exception is when the scenario specifically asks for ALL properties.</remarks>
        /// <returns>Resolved property; possibly null.</returns>
        internal static ResourceProperty TryResolvePropertyName(this ResourceType resourceType, string propertyName, ResourcePropertyKind exceptKind)
        {
            // In case of empty property name this will return null, which means propery is not found
            return resourceType.Properties.FirstOrDefault(p => p.Name == propertyName && (p.Kind & exceptKind) == 0);
        }

        /// <summary>
        /// Gets the resource type which the resource property is declared on.
        /// </summary>
        /// <param name="resourceType">resource type that is used to look for the resourceProperty</param>
        /// <param name="resourceProperty">resource property in question</param>
        /// <param name="rootType">root type in the hierarchy at which we need to stop.</param>
        /// <returns>actual resource type that declares the property or the root type if the property is declared in a more base type than the given root type.</returns>
        internal static ResourceType GetDeclaringTypeForProperty(this ResourceType resourceType, ResourceProperty resourceProperty, ResourceType rootType = null)
        {
            Debug.Assert(resourceProperty != null, "resourceProperty != null");
            while (resourceType != rootType)
            {
                if (resourceType.TryResolvePropertiesDeclaredOnThisTypeByName(resourceProperty.Name) == resourceProperty)
                {
                    break;
                }

                resourceType = resourceType.BaseType;
            }

            return resourceType;
        }

        /// <summary>Tries to find the property declared on this type for the specified name.</summary>
        /// <param name="resourceType">resource type that may contain the property</param>
        /// <param name="propertyName">Name of property to resolve.</param>
        /// <returns>Resolved property; possibly null.</returns>
        internal static ResourceProperty TryResolvePropertiesDeclaredOnThisTypeByName(this ResourceType resourceType, string propertyName)
        {
            return resourceType.TryResolvePropertiesDeclaredOnThisTypeByName(propertyName, 0);
        }

        /// <summary>Tries to find the property declared on this type for the specified name.</summary>
        /// <param name="resourceType">resource type that may contain the property</param>
        /// <param name="propertyName">Name of property to resolve.</param>
        /// <param name="exceptKind">The property kind to filter out.</param>
        /// <returns>Resolved property; possibly null.</returns>
        internal static ResourceProperty TryResolvePropertiesDeclaredOnThisTypeByName(this ResourceType resourceType, string propertyName, ResourcePropertyKind exceptKind)
        {
            // In case of empty property name this will return null, which means propery is not found
            return resourceType.PropertiesDeclaredOnThisType.FirstOrDefault(p => p.Name == propertyName && (p.Kind & exceptKind) == 0);
        }

        /// <summary>Disposes of <paramref name="o"/> if it implements <see cref="IDisposable"/>.</summary>
        /// <param name="o">Object to dispose, possibly null.</param>
        internal static void Dispose(object o)
        {
            IDisposable disposable = o as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Checks if the given type is assignable to this type. In other words, if this type
        /// is a subtype of the given type or not.
        /// </summary>
        /// <param name="resourceType">resource type that is checked if it's a parent for the subType</param>
        /// <param name="subType">resource type to check.</param>
        /// <returns>true, if the given type is assignable to this type. Otherwise returns false.</returns>
        internal static bool IsAssignableFrom(this ResourceType resourceType, ResourceType subType)
        {
            while (subType != null)
            {
                if (subType == resourceType)
                {
                    return true;
                }

                subType = subType.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Creates a delegate that when called creates a new instance of the specified <paramref name="type" />.
        /// </summary>
        /// <param name="type">Type of the instance.</param>
        /// <param name="fullName">full name of the given clr type.
        /// If the type name is not specified, it takes the full name from the clr type.</param>
        /// <param name="targetType">Type to return from the delegate.</param>
        /// <returns>A delegate that when called creates a new instance of the specified <paramref name="type" />.</returns>
        internal static Delegate CreateNewInstanceConstructor(Type type, string fullName, Type targetType)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(targetType != null, "targetType != null");

            // Create the new instance of the type
            ConstructorInfo emptyConstructor = type.GetConstructor(Type.EmptyTypes);
            if (emptyConstructor == null)
            {
                fullName = fullName ?? type.FullName;
                throw new InvalidOperationException(Strings.NoEmptyConstructorFoundForType(fullName));
            }

            DynamicMethod method = new DynamicMethod("invoke_constructor", targetType, Type.EmptyTypes, false);
            var generator = method.GetILGenerator();
            generator.Emit(OpCodes.Newobj, emptyConstructor);
            if (targetType.IsValueType)
            {
                generator.Emit(OpCodes.Box);
            }

            generator.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Func<>).MakeGenericType(targetType));
        }

        /// <summary>
        /// Validate the given annotation.
        /// </summary>
        /// <param name="customAnnotations">Reference to the dictionary instance where custom annotations are stored.</param>
        /// <param name="namespaceName">NamespaceName to which the custom annotation belongs to.</param>
        /// <param name="name">Name of the annotation.</param>
        /// <param name="annotation">Value of the annotation.</param>
        internal static void ValidateAndAddAnnotation(ref Dictionary<string, object> customAnnotations, string namespaceName, string name, object annotation)
        {
            // DEVNOTE (pratikp): When we allow providers to add custom annotations, we can change this assert into an exception.
            // Ideally all this check must be done in EdmLib and hopefully we can call some method in EdmLib to do this validation.
            Debug.Assert(!String.IsNullOrEmpty(name), "!String.IsNullOrEmpty(name)");
            Debug.Assert(annotation != null, "annotation != null");

            if (customAnnotations == null)
            {
                customAnnotations = new Dictionary<string, object>(StringComparer.Ordinal);
            }

            if (!String.IsNullOrEmpty(namespaceName))
            {
                // custom annotations can be only of string or XElement type
                Debug.Assert(annotation.GetType() == typeof(string) || annotation.GetType() == typeof(XElement), "only string and xelement annotations are supported");
            }

            string fullName = CreateFullNameForCustomAnnotation(namespaceName, name);
            customAnnotations.Add(fullName, annotation);
        }

        /// <summary>
        /// A workaround to a problem with FxCop which does not recognize the CheckArgumentNotNull method
        /// as the one which validates the argument is not null.
        /// </summary>
        /// <remarks>This has been suggested as a workaround in msdn forums by the VS team. Note that even though this is production code
        /// the attribute has no effect on anything else.</remarks>
        private sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}
