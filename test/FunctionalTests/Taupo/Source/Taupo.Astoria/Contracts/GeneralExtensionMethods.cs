//---------------------------------------------------------------------
// <copyright file="GeneralExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Extension methods that aren't Astoria specific
    /// </summary>
    public static class GeneralExtensionMethods
    {
        /// <summary>
        /// Concatenates the given parameters onto the given enumerable
        /// </summary>
        /// <typeparam name="TItem">The type of the items</typeparam>
        /// <param name="source">The enumerable to concatenate onto</param>
        /// <param name="second">The items to concatenate onto the source</param>
        /// <returns>The concatenated enumerable</returns>
        public static IEnumerable<TItem> Concat<TItem>(this IEnumerable<TItem> source, params TItem[] second)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            return source.Concat((IEnumerable<TItem>)second);
        }

        /// <summary>
        /// Remove all items that match particular criteria from the collection
        /// </summary>
        /// <typeparam name="T">Type of the item in the collection</typeparam>
        /// <param name="collection">Collection that holds the items</param>
        /// <param name="predicate">Predicate that specifies which items to remove</param>
        /// <returns>Number of items removed</returns>
        public static int RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            var list = collection as List<T>;
            if (list != null)
            {
                Predicate<T> match = i => predicate(i);
                return list.RemoveAll(match);
            }

            var itemsToBeDeleted = collection.Where(predicate).ToList();
            foreach (T item in itemsToBeDeleted)
            {
                collection.Remove(item);
            }

            return itemsToBeDeleted.Count;
        }

        /// <summary>
        /// Adds the specified collection to the end of the current collection
        /// </summary>
        /// <typeparam name="T">The type of the elements</typeparam>
        /// <param name="current">The collection to add to</param>
        /// <param name="collection">The elements to add</param>
        public static void AddRange<T>(this ICollection<T> current, IEnumerable<T> collection)
        {
            ExceptionUtilities.CheckArgumentNotNull(current, "current");
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");

            var list = current as List<T>;
            if (list != null)
            {
                list.AddRange(collection);
            }
            else
            {
                foreach (var element in collection)
                {
                    current.Add(element);
                }
            }
        }

        /// <summary>
        /// Performs the specified action on each element
        /// </summary>
        /// <typeparam name="T">The type of the elements</typeparam>
        /// <param name="elements">The elements to perform the action on</param>
        /// <param name="action">The action to perform on each element</param>
        public static void ForEach<T>(this IEnumerable<T> elements, Action<T> action)
        {
            ExceptionUtilities.CheckArgumentNotNull(elements, "elements");
            ExceptionUtilities.CheckArgumentNotNull(action, "action");

            var list = elements as List<T>;
            if (list != null)
            {
                list.ForEach(action);
            }
            else
            {
                foreach (var element in elements)
                {
                    action(element);
                }
            }
        }

        /// <summary>
        /// Extension method to remove bit-flags from an enum
        /// </summary>
        /// <typeparam name="TEnum">The enum type</typeparam>
        /// <param name="original">The original enum values</param>
        /// <param name="value">The bit-flags to remove</param>
        /// <returns>The new enum value</returns>
        public static TEnum Without<TEnum>(this Enum original, TEnum value)
        {
            return (TEnum)(object)((int)(object)original & ~(int)(object)value);
        }

        /// <summary>
        /// Tries the dequeue.
        /// </summary>
        /// <param name="queue">The queue.</param>
        public static void TryDequeue(this Queue<QueryExpression> queue)
        {
            if (queue.Count > 0)
            {
                queue.Dequeue();
            }
        }

        /// <summary>
        /// Invokes the callback if the given instance is non-null
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance</typeparam>
        /// <param name="instance">The instance</param>
        /// <param name="callback">The callback</param>
        public static void IfValid<TInstance>(this TInstance instance, Action<TInstance> callback)
            where TInstance : class
        {
            if (instance != null)
            {
                callback(instance);
            }
        }

        /// <summary>
        /// Invokes the callback if the given instance is non-null
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="defaultValue">The default value if the instance is null.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>The result of the callback or the default value</returns>
        public static TReturn IfValid<TInstance, TReturn>(this TInstance instance, TReturn defaultValue, Func<TInstance, TReturn> callback)
            where TInstance : class
        {
            if (instance != null)
            {
                return callback(instance);
            }

            return defaultValue;
        }

        /// <summary>
        /// Tries to find an extension method that matches the given parameter types
        /// </summary>
        /// <param name="instanceType">The type of the first argument to the extension method</param>
        /// <param name="methodName">The extension method name</param>
        /// <param name="argumentTypes">The types of the other arguments</param>
        /// <returns>The method info, or null if none was found</returns>
        public static MethodInfo GetExtensionMethod(this Type instanceType, string methodName, params Type[] argumentTypes)
        {
            // gather base types
            var instanceTypeHierarchy = new List<Type>() { instanceType };
            while (instanceTypeHierarchy[0] != typeof(object))
            {
                instanceTypeHierarchy.Insert(0, instanceTypeHierarchy[0].GetBaseType());
            }

            // remove object from the list
            instanceTypeHierarchy.RemoveAt(0);

            // gather distinct assemblies
            var assembliesToCheck = instanceTypeHierarchy.Distinct().Select(t => t.GetAssembly()).Distinct().ToList();

            // gather public types with extensions
            var publicExtensionTypes = assembliesToCheck.SelectMany(a => a.GetTypes().Where(t => t.IsPublic() && t.IsDefined(typeof(ExtensionAttribute), true))).ToList();

            // gather public static methods with the right signature
            var extensionArgTypes = new[] { instanceType }
                .Concat(argumentTypes)
                .ToArray();
            var staticMethodsWithName = publicExtensionTypes.Select(t => t.GetMethod(methodName, extensionArgTypes, true, true)).ToList();

            // filter out non-extension methods
            var extensionMethods = staticMethodsWithName.Where(m => m != null && m.IsDefined(typeof(ExtensionAttribute), true)).ToList();

            // return the method
            return extensionMethods.SingleOrDefault();
        }
    }
}
