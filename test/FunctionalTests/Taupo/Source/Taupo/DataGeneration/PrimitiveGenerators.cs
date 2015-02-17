//---------------------------------------------------------------------
// <copyright file="PrimitiveGenerators.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;

    /// <summary>
    /// Primitive data generators.
    /// </summary>
    public static class PrimitiveGenerators
    {
        private static Dictionary<Type, long> seedLimits = new Dictionary<Type, long>()
            {
                { typeof(sbyte), sbyte.MaxValue },
                { typeof(byte), byte.MaxValue },
                { typeof(short), short.MaxValue },
                { typeof(ushort), ushort.MaxValue },
                { typeof(int), int.MaxValue },
                { typeof(uint), uint.MaxValue },
            };

        /// <summary>
        /// Constant data generator that generates specified value.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>Constant data generator.</returns>
        public static IDataGenerator<TData> Constant<TData>(TData value)
        {
            return new ConstantGenerator<TData>(value);
        }

        /// <summary>
        /// Sequential data generator that generates sequence  1, 2, 3, 4, ... and converts it to the targeted type.
        /// </summary>
        /// <typeparam name="TData">The data type. Convertion using IConvertible must be supported from long to this type.</typeparam>
        /// <returns>Sequential data generator.</returns>
        public static IDataGenerator<TData> Sequence<TData>()
        {
            return Sequence<TData>(1);
        }

        /// <summary>
        /// Sequential data generator that generates sequence starting from the seed converts it to the targeted type.
        /// </summary>
        /// <typeparam name="TData">The data type. Convertion using IConvertible must be supported from long to this type.</typeparam>
        /// <param name="initialSeed">The initial seed of the generator</param>
        /// <returns>Sequential data generator.</returns>
        public static IDataGenerator<TData> Sequence<TData>(long initialSeed)
        {
            return new DelegatedSeededDataGenerator<TData>(
                   initialSeed,
                   FromSeedStrategies.ConvertSeed<TData>());
        }

        /// <summary>
        /// Sequential data generator with unlimited capacity: generates repeated sequence  1, 2, 3, 4, ... , limit.
        /// Limit value is based on the data type while the initial seed and step are always 1.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <returns>Sequential data generator with unlimited capacity.</returns>
        public static IDataGenerator<TData> RepeatedSequence<TData>()
        {
            return RepeatedSequence<TData>(1, 1);
        }

        /// <summary>
        /// Sequential data generator with unlimited capacity: generates repeated sequence starting from the seed and incrementing by the step
        /// Limit value is based on the data type.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <param name="initialSeed">The initial seed of the generator</param>
        /// <param name="step">The amount to increment by</param>
        /// <returns>Sequential data generator with unlimited capacity.</returns>
        public static IDataGenerator<TData> RepeatedSequence<TData>(long initialSeed, long step)
        {
            long limit;
            if (!seedLimits.TryGetValue(typeof(TData), out limit))
            {
                limit = long.MaxValue;
            }

            return new DelegatedSeededDataGenerator<TData>(
                   initialSeed,
                   FromSeedStrategies.ConvertSeed<TData>(),
                   TryGetNextSeedStrategies.RepeatedSequence(initialSeed, limit, step));
        }

        /// <summary>
        /// Guid data generator that each time returns new Guid.
        /// </summary>
        /// <returns>Guid data generator.</returns>
        public static IDataGenerator<Guid> NewGuid()
        {
            return new DelegatedDataGenerator<Guid>(() => Guid.NewGuid());
        }

        /// <summary>
        /// Data generator that returns the default value for its type.
        /// </summary>
        /// <typeparam name="TData">The type of the data to generate</typeparam>
        /// <returns>Default data generator.</returns>
        public static IDataGenerator<TData> Default<TData>()
        {
            return Constant(default(TData));
        }

        /// <summary>
        /// Data generator that returns the default value for its type.
        /// </summary>
        /// <param name="clrType">The clr type</param>
        /// <returns>Default data generator.</returns>
        public static IDataGenerator Default(Type clrType)
        {
            var method = typeof(PrimitiveGenerators)
                .GetMethods(true, true)
                .SingleOrDefault(m => m.Name == "Default" && m.IsGenericMethod);
            ExceptionUtilities.CheckObjectNotNull(method, "Could not find generic static method 'Default' on type 'PrimitiveGenerators'");
            return (IDataGenerator)method.MakeGenericMethod(new Type[] { clrType }).Invoke(null, null);
        }

        /// <summary>
        /// DateTimeOffset data generator that generates sequence of DateTimeOffset values 
        /// with 0 offset, with the specified step and starting with the specified initial value.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="step">The step for the sequence.</param>
        /// <param name="offset">The offset for the DateTimeOffset values.</param>
        /// <returns>DateTime data generator.</returns>
        public static IDataGenerator<DateTimeOffset> DateTimeOffsetSequence(DateTime initialValue, TimeSpan step, TimeSpan offset)
        {
            return new DelegatedSeededDataGenerator<DateTimeOffset>(
                initialValue.Ticks,
                FromSeedStrategies.DateTimeOffsetFromSeed(offset),
                TryGetNextSeedStrategies.SequenceWithStep(step.Ticks));
        }

        /// <summary>
        /// DateTime data generator that generates sequence of DateTime values
        /// with the specified step and starting with the specified initial value.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="step">The step for the sequence.</param>
        /// <returns>DateTime data generator.</returns>
        public static IDataGenerator<DateTime> DateTimeSequence(DateTime initialValue, TimeSpan step)
        {
            return new DelegatedSeededDataGenerator<DateTime>(
                initialValue.Ticks,
                FromSeedStrategies.DateTimeFromSeed(),
                TryGetNextSeedStrategies.SequenceWithStep(step.Ticks));
        }

        /// <summary>
        /// TimeSpan data generator that generates sequence of TimeSpan values
        /// with the specified step and starting with the specified initial value.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="step">The step for the sequence.</param>
        /// <returns>TimeSpan data generator.</returns>
        public static IDataGenerator<TimeSpan> TimeSpanSequence(TimeSpan initialValue, TimeSpan step)
        {
            return new DelegatedSeededDataGenerator<TimeSpan>(
                initialValue.Ticks,
                FromSeedStrategies.TimeSpanFromSeed(),
                TryGetNextSeedStrategies.SequenceWithStep(step.Ticks));
        }

        /// <summary>
        /// Binary data generator that generates binary representation of strings 
        /// in the sequence "1", "2", "3", ... .
        /// </summary>
        /// <returns>Binary data generator.</returns>
        public static IDataGenerator<byte[]> BinarySequence()
        {
            return new DelegatedSeededDataGenerator<byte[]>(1, FromSeedStrategies.BinaryFromSeedAsString());
        }

        /// <summary>
        /// Boolean data generator that alternates between true and false, using true for odd seed values
        /// </summary>
        /// <returns>Boolean data generator</returns>
        public static IDataGenerator<bool> BooleanAlternatingSequence()
        {
            return new DelegatedSeededDataGenerator<bool>(1, FromSeedStrategies.TrueForOdd());
        }

        /// <summary>
        /// String data generator that generates sequence of values based on the sequence of seeds 1, 2, 3
        /// where seed is converted to string with the length in the specified range.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>String data generator.</returns>
        public static IDataGenerator<string> StringSequence(int minLength, int maxLength)
        {
            return new DelegatedSeededDataGenerator<string>(1, FromSeedStrategies.StringFromSeed(minLength, maxLength));
        }

        /// <summary>
        /// Binary data generator that generates sequence of values based on the sequence of seeds 1, 2, 3
        /// where seed is converted to byte[] representation with the length in the specified range.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>Binary data generator.</returns>
        public static IDataGenerator<byte[]> BinarySequence(int minLength, int maxLength)
        {
            return new DelegatedSeededDataGenerator<byte[]>(1, FromSeedStrategies.BinaryFromSeed(minLength, maxLength));
        }

        /// <summary>
        /// Predefined data generator which selects a value from a predefined set of values
        /// </summary>
        /// <typeparam name="TData">The data type</typeparam>
        /// <param name="random">A Random number generator to allow the framework to pick a random value from the set of values</param>
        /// <param name="values">The set of allowed values</param>
        /// <returns>predefined data generator</returns>
        public static IDataGenerator<TData> PredefinedValues<TData>(IRandomNumberGenerator random, params TData[] values)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckCollectionNotEmpty(values, "values");
            return new DelegatedDataGenerator<TData>(() => random.ChooseFrom(values));
        }

        /// <summary>
        /// Constant data generator.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        private class ConstantGenerator<TData> : DataGenerator<TData>
        {
            private TData constant;

            /// <summary>
            /// Initializes a new instance of the ConstantGenerator class.
            /// </summary>
            /// <param name="constant">The constant.</param>
            public ConstantGenerator(TData constant)
            {
                this.constant = constant;
            }

            /// <summary>
            /// Generates constant data.
            /// </summary>
            /// <returns>Constant specified during construction.</returns>
            public override TData GenerateData()
            {
                return this.constant;
            }
        }
    }
}
