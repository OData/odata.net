//---------------------------------------------------------------------
// <copyright file="FromSeedStrategies.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contains FromSeed stategies for different primitive types.
    /// </summary>
    internal static class FromSeedStrategies
    {
        private static readonly Type[] supportedTypesForConvertSeed = new Type[]
        {
            typeof(string),
            typeof(sbyte),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(double),
            typeof(float),
            typeof(decimal),
            typeof(byte),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
        };
        
        /// <summary>
        /// FromSeed strategy that converts seed to the targeted type using IConvertible.
        /// </summary>
        /// <typeparam name="TData">The targeted type.</typeparam>
        /// <returns>'FromSeed' delegate.</returns>
        /// <remarks>The <typeparamref name="TData"/> such that long can be converted to.</remarks> 
        public static Func<long, TData> ConvertSeed<TData>()
        {
            Type clrType = typeof(TData);

            if (!supportedTypesForConvertSeed.Contains(typeof(TData)))
            {
                throw new TaupoNotSupportedException("This strategy is not supported for the type " + clrType.FullName + ". Try other strategies.");
            }

            return delegate(long seed)
                {
                    try
                    {
                        var value = seed.ConvertToType<TData>();
                        return value;
                    }
                    catch (OverflowException ex)
                    {
                        throw DataGenerationUtilities.FromSeedFailedException<TData>(seed, ex);
                    }
                };
        }

        /// <summary>
        /// Boolean FromSeed strategy: odd number - true,  even number - false.
        /// </summary>
        /// <returns>Boolean 'FromSeed' delegate.</returns>
        public static Func<long, bool> TrueForOdd()
        {
            return delegate(long seed)
            {
                return seed % 2 != 0;
            };
        }

        /// <summary>
        /// Boolean FromSeed strategy: even number - true,  odd number - false.
        /// </summary>
        /// <returns>Boolean 'FromSeed' delegate.</returns>
        public static Func<long, bool> TrueForEven()
        {
            return delegate(long seed)
            {
                return seed % 2 == 0;
            };
        }

        /// <summary>
        /// String 'FromSeed' strategy that echoes the seed and prepends prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>String 'FromSeed' delegate.</returns>
        public static Func<long, string> EchoSeedWithPrefix(string prefix)
        {
            ExceptionUtilities.CheckArgumentNotNull(prefix, "prefix");

            return delegate(long seed)
            {
                return prefix + seed.ToString(CultureInfo.InvariantCulture);
            };
        }

        /// <summary>
        /// String 'FromSeed' strategy where seed is converted to string and then can be truncated or padded with spaces such that length is within specified range.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>String 'FromSeed' delegate.</returns>
        public static Func<long, string> StringFromSeed(int minLength, int maxLength)
        {
            ExceptionUtilities.CheckValidRange<int>(minLength, "minLength", maxLength, "maxLength");

            return delegate(long seed)
            {
                string data = seed.ToString(CultureInfo.InvariantCulture);
                if (data.Length > maxLength)
                {
                    data = data.Substring(0, maxLength);
                }
                else if (data.Length < minLength)
                {
                    data = data.PadRight(minLength);
                }

                return data;
            };
        }

        /// <summary>
        /// Binary 'FromSeed' strategy where seed is converted to string and byte[] representation of this string is returned.
        /// </summary>
        /// <returns>Binary 'FromSeed' delegate.</returns>
        public static Func<long, byte[]> BinaryFromSeedAsString()
        {
             return delegate(long seed)
                {
                    byte[] bytes = Encoding.Unicode.GetBytes(seed.ToString(CultureInfo.InvariantCulture));
                    return bytes;
                };
        }

        /// <summary>
        /// Binary 'FromSeed' strategy where seed is converted to byte[] representation with the length in the specified range.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>Binary 'FromSeed' delegate.</returns>
        public static Func<long, byte[]> BinaryFromSeed(int minLength, int maxLength)
        {
            ExceptionUtilities.CheckValidRange<int>(minLength, "minLength", maxLength, "maxLength");

            return delegate(long seed)
            {
                List<byte> values = new List<byte>();
                seed = Math.Abs(seed);

                while (values.Count < maxLength && seed > 0)
                {
                    byte b = (byte)(seed % 256);
                    values.Add(b);
                    seed = seed / 256;
                }

                while (values.Count < minLength)
                {
                    values.Add(0);
                }

                return values.ToArray();
            };
        }

        /// <summary>
        /// DateTime 'FromSeed' strategy that returns new DataTime(seed).
        /// </summary>
        /// <returns>DateTime 'FromSeed' delegate.</returns>
        public static Func<long, DateTime> DateTimeFromSeed()
        {
            return delegate(long seed)
                {
                    try
                    {
                        return new DateTime(seed);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        throw DataGenerationUtilities.FromSeedFailedException<DateTime>(seed, ex);
                    }
                };
        }

        /// <summary>
        /// DateTimeOffset 'FromSeed' strategy that returns DateTimeOffset(seed, offset).
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>DateTimeOffset 'FromSeed' delegate.</returns>
        public static Func<long, DateTimeOffset> DateTimeOffsetFromSeed(TimeSpan offset)
        {
            return delegate(long seed)
            {
                try
                {
                    return new DateTimeOffset(seed, offset);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    throw DataGenerationUtilities.FromSeedFailedException<DateTimeOffset>(seed, ex);
                }
            };
        }

        /// <summary>
        /// TimeSpan 'FromSeed' strategy that returns new TimeSpan(seed).
        /// </summary>
        /// <returns>TimeSpan 'FromSeed' delegate.</returns>
        public static Func<long, TimeSpan> TimeSpanFromSeed()
        {
            return delegate(long seed)
                {
                    return new TimeSpan(seed);
                };
        }
    }
}
