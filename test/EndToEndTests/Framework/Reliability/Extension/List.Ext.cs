//---------------------------------------------------------------------
// <copyright file="List.Ext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// IList extension
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Randomize the list
        /// </summary>
        /// <typeparam name="T">The item</typeparam>
        /// <param name="array">The list</param>
        public static void Randomize<T>(this IList<T> array)
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < array.Count; i++)
            {
                int moveTo = rnd.Next(array.Count);
                var tmp = array[moveTo];
                array[moveTo] = array[i];
                array[i] = tmp;
            }
        }
    }
}