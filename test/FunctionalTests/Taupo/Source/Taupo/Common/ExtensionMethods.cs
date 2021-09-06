//---------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Miscellaneous extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        private const string Base16Chars = "0123456789abcdef";

        /// <summary>
        /// Constructs a <see cref="TestSkippedException"/> and passes it to the given continuation.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        /// <param name="message">The message.</param>
        public static void Skip(this IAsyncContinuation continuation, string message)
        {
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");

            try
            {
                // throw and immediately catch an exception to capture the stack trace
                throw new TestSkippedException(message);
            }
            catch (TestSkippedException ex)
            {
                continuation.Fail(ex);
            }
        }

        /// <summary>
        /// Gets the Base16 string for a given byte array.
        /// </summary>
        /// <param name="data">The input byte array.</param>
        /// <returns>Base16 string for given byte array (lowercase).</returns>
        public static string ToBase16String(this byte[] data)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");

            var sb = new StringBuilder(data.Length * 2);

            foreach (byte b in data)
            {
                sb.Append(Base16Chars[b >> 4]);
                sb.Append(Base16Chars[b & 15]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Computes a cryptographic hash of the data.
        /// </summary>
        /// <param name="data">The data to compute hash for.</param>
        /// <returns>Computet hash.</returns>
        public static byte[] ComputeHash(this byte[] data)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");
            using (var hash = new SHA1Managed())
            {
                return hash.ComputeHash(data);
            }
        }

        /// <summary>
        /// Determines whether contents of two arrays are equal, item-by-item.
        /// </summary>
        /// <typeparam name="T">Type of array item.</typeparam>
        /// <param name="firstArray">The first array.</param>
        /// <param name="secondArray">The second array.</param>
        /// <returns>
        /// A value of <c>true</c> if contents of the first array are equal to the second array, item-by-item.
        /// </returns>
        public static bool IsEqualTo<T>(this T[] firstArray, T[] secondArray)
            where T : IEquatable<T>
        {
            if (firstArray == null)
            {
                return secondArray == null;
            }

            if (secondArray == null)
            {
                return false;
            }

            if (firstArray.Length != secondArray.Length)
            {
                return false;
            }

            for (int i = 0; i < firstArray.Length; ++i)
            {
                if (!firstArray[i].Equals(secondArray[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
