//---------------------------------------------------------------------
// <copyright file="CommonUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client
#else
namespace Microsoft.OData.Service
#endif
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Common definitions and functions for ALL product assemblies
    /// </summary>
    internal static partial class CommonUtil
    {
        // Only StackOverflowException & ThreadAbortException are sealed classes.

        /// <summary>Type of OutOfMemoryException.</summary>
        private static readonly Type OutOfMemoryType = typeof(OutOfMemoryException);

        /// <summary>Type of StackOverflowException.</summary>
        private static readonly Type StackOverflowType = typeof(StackOverflowException);

        /// <summary>Type of ThreadAbortException.</summary>
        private static readonly Type ThreadAbortType = typeof(ThreadAbortException);

        public static object ParseJsonToPrimitiveValue(string rawValue)
        {
            Debug.Assert(rawValue != null && rawValue.Length > 0 && rawValue.IndexOf('{') != 0 && rawValue.IndexOf('[') != 0,
                  "rawValue != null && rawValue.Length > 0 && rawValue.IndexOf('{') != 0 && rawValue.IndexOf('[') != 0");
            ODataCollectionValue collectionValue = (ODataCollectionValue)
                Microsoft.OData.ODataUriUtils.ConvertFromUriLiteral(string.Format(CultureInfo.InvariantCulture, "[{0}]", rawValue), ODataVersion.V4);
            foreach (object item in collectionValue.Items)
            {
                return item;
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified exception can be caught and
        /// handled, or whether it should be allowed to continue unwinding.
        /// </summary>
        /// <param name="e"><see cref="Exception"/> to test.</param>
        /// <returns>
        /// true if the specified exception can be caught and handled;
        /// false otherwise.
        /// </returns>
        internal static bool IsCatchableExceptionType(Exception e)
        {
            if (e == null)
            {
                return true;
            }

            // a 'catchable' exception is defined by what it is not.
            Type type = e.GetType();
            return (type != ThreadAbortType) && (type != StackOverflowType) && (type != OutOfMemoryType);
        }
    }
}
