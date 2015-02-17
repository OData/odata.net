//---------------------------------------------------------------------
// <copyright file="RandomUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
    internal static class RandomUtilities
    {
        public static IList<T> ShuffleList<T>(IList<T> list, Func<int, int> nextInt)
        {
            for (int index = 0; index < list.Count; index++)
            {
                int randPosition = nextInt(list.Count - index) + index;
                if (randPosition != index)
                {
                    T temp = list[randPosition];
                    list[randPosition] = list[index];
                    list[index] = temp;
                }
            }

            return list;
        }

        /// <summary>
        /// Chooses a random value from the given enum type.  If you need to repeatedly get random enum values for a given type, it is recommended to 
        /// do this yourself since performance is slow on this method due to necessary reflection.
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <returns>A random value from the enum</returns>
        internal static object ChooseRandomEnumValue(Type enumType, Func<int, int> nextInt)
        {
            var allValues = Utilities.GetEnumValues(enumType);
            return allValues[nextInt(allValues.Count)];
        }
    }
}
