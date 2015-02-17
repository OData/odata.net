//---------------------------------------------------------------------
// <copyright file="EqualsImplementationUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	internal static class EqualsImplementationUtils
	{
		/// <summary>
		/// Checks equality between two possibly null objects.
		/// </summary>
		/// <param name="firstValue">The first value.</param>
		/// <param name="secondValue">The second value.</param>
		/// <returns></returns>
		public static bool SafeEquals(object firstValue, object secondValue)
		{
			if (firstValue == null || secondValue == null)
				return firstValue == secondValue;
			else
				return firstValue.Equals(secondValue);
		}

		/// <summary>
		/// Gets the hash value for a possibly null object.
		/// </summary>
		/// <param name="o">The o.</param>
		/// <returns></returns>
		public static int GetSafeHashValue(object o)
		{
			if (o == null)
				return 0;
			return o.GetHashCode();
		}
	}
}
