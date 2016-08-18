//   OData .NET Libraries ver. 5.6.3
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

namespace System.Data.Services.Client
{
#region Namespaces
    using System.Data.Services.Common;
    using System.Diagnostics;
#endregion

    /// <summary>Utilities for binding related operations</summary>
    internal static class BindingUtils
    {
        /// <summary>
        /// Throw if the entity set name is null or empty
        /// </summary>
        /// <param name="entitySetName">entity set name.</param>
        /// <param name="entity">entity instance for which the entity set name is generated.</param>
        internal static void ValidateEntitySetName(string entitySetName, object entity)
        {
            if (String.IsNullOrEmpty(entitySetName))
            {
                throw new InvalidOperationException(Strings.DataBinding_Util_UnknownEntitySetName(entity.GetType().FullName));
            }
        }
        
        /// <summary>
        /// Given a collection type, gets it's entity type
        /// </summary>
        /// <param name="collectionType">Input collection type</param>
        /// <returns>Generic type argument for the collection</returns>
        internal static Type GetCollectionEntityType(Type collectionType)
        {
            while (collectionType != null)
            {
                if (collectionType.IsGenericType() && WebUtil.IsDataServiceCollectionType(collectionType.GetGenericTypeDefinition()))
                {
                    return collectionType.GetGenericArguments()[0];
                }

                collectionType = collectionType.GetBaseType();
            }

            return null;
        }

#if DEBUG
        /// <summary>Verifies the absence of observer for an DataServiceCollection</summary>
        /// <typeparam name="T">Type of DataServiceCollection</typeparam>
        /// <param name="oec">Non-typed collection object</param>
        /// <param name="sourceProperty">Collection property of the source object which is being assigned to</param>
        /// <param name="sourceType">Type of the source object</param>
        /// <param name="model">The client model.</param>
        internal static bool IsBeingObserved<T>(object oec, string sourceProperty, Type sourceType, ClientEdmModel model)
#else
        /// <summary>Verifies the absence of observer for an DataServiceCollection</summary>
        /// <typeparam name="T">Type of DataServiceCollection</typeparam>
        /// <param name="oec">Non-typed collection object</param>
        /// <param name="sourceProperty">Collection property of the source object which is being assigned to</param>
        /// <param name="sourceType">Type of the source object</param>
        internal static bool IsBeingObserved<T>(object oec, string sourceProperty, Type sourceType)
#endif
        {
#if DEBUG
            Debug.Assert(BindingEntityInfo.IsDataServiceCollection(oec.GetType(), model), "Must be an DataServiceCollection.");
#endif
            DataServiceCollection<T> typedCollection = oec as DataServiceCollection<T>;

            if (typedCollection.Observer != null)
            {
                return true;
            }
            return false;
        }
    }
}
