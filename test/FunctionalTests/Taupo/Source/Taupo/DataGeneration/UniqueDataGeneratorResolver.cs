//---------------------------------------------------------------------
// <copyright file="UniqueDataGeneratorResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;

    /// <summary>
    /// Resolver for unique data generators.
    /// </summary>
    [ImplementationName(typeof(IUniqueDataGeneratorResolver), "Default")]
    public class UniqueDataGeneratorResolver : IUniqueDataGeneratorResolver
    {
        private static Dictionary<Type, Func<IDataGenerator>> dataGeneratorCreators = new Dictionary<Type, Func<IDataGenerator>>()
                {
                  { typeof(sbyte),   () => PrimitiveGenerators.RepeatedSequence<sbyte>(-10, 1) },
                  { typeof(short),   () => PrimitiveGenerators.RepeatedSequence<short>(-10, 1) },
                  { typeof(int),     () => PrimitiveGenerators.RepeatedSequence<int>(-10, 1) }, 
                  { typeof(long),    () => PrimitiveGenerators.Sequence<long>(-10) }, 
                  { typeof(byte),    PrimitiveGenerators.RepeatedSequence<byte> },
                  { typeof(ushort),  PrimitiveGenerators.RepeatedSequence<ushort> },
                  { typeof(uint),    PrimitiveGenerators.RepeatedSequence<uint> },
                  { typeof(ulong),   PrimitiveGenerators.Sequence<ulong> },
                  { typeof(float),   PrimitiveGenerators.Sequence<float> },
                  { typeof(double),  PrimitiveGenerators.Sequence<double> },
                  { typeof(decimal), PrimitiveGenerators.Sequence<decimal> },
                  { typeof(Guid),    PrimitiveGenerators.NewGuid },
                  { typeof(bool),    PrimitiveGenerators.BooleanAlternatingSequence },
                  { typeof(DateTime),       () => PrimitiveGenerators.DateTimeSequence(PlatformHelper.CreateDateTimeWithInvariantCultureCalendar(2000, 1, 1), new TimeSpan(1, 0, 0, 0)) },
                  { typeof(DateTimeOffset), () => PrimitiveGenerators.DateTimeOffsetSequence(PlatformHelper.CreateDateTimeWithInvariantCultureCalendar(2000, 1, 1), new TimeSpan(1, 0, 0), DateTimeOffset.Now.Offset) },
                  { typeof(TimeSpan),       () => PrimitiveGenerators.TimeSpanSequence(TimeSpan.Zero, new TimeSpan(0, 0, 1)) },
                };

        /// <summary>
        /// Resolves data generator which generates unique data.
        /// </summary>
        /// <param name="clrType">The type of the data.</param>
        /// <param name="random">Random number generator.</param>
        /// <param name="hints">Data generation hints.</param>
        /// <returns>The unique data generator.</returns>
        public IDataGenerator ResolveUniqueDataGenerator(Type clrType, IRandomNumberGenerator random, params DataGenerationHint[] hints)
        {
            ExceptionUtilities.CheckArgumentNotNull(clrType, "clrType");
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(hints, "hints");

            IDataGenerator dataGenerator;
            if (clrType == typeof(string))
            {
                dataGenerator = ResolveStringDataGenerator(hints);
            }
            else if (clrType == typeof(byte[]))
            {
                dataGenerator = ResolveBinaryDataGenerator(hints);
            }
            else
            {
                // TODO: for now just return the same generator as we used to (i.e. ignore hints).
                Func<IDataGenerator> createDataGen;
                if (!dataGeneratorCreators.TryGetValue(clrType, out createDataGen))
                {
                    throw new TaupoNotSupportedException("Unique data generator resolution is not supported for the type: " + clrType.FullName);
                }

                dataGenerator = createDataGen();
            }

            return this.ResolveUniqueDataGenerator(clrType, dataGenerator, hints);
        }

        private static IDataGenerator<string> ResolveStringDataGenerator(IEnumerable<DataGenerationHint> hints)
        {
            int minLength = hints.Max<MinLengthHint, int>(-1);
            int maxLength = hints.Min<MaxLengthHint, int>(minLength);

            if (minLength == -1 && maxLength == -1)
            {
                return PrimitiveGenerators.Sequence<string>();
            }

            if (minLength == -1)
            {
                minLength = 0;
            }

            return PrimitiveGenerators.StringSequence(minLength, maxLength);
        }

        private static IDataGenerator<byte[]> ResolveBinaryDataGenerator(IEnumerable<DataGenerationHint> hints)
        {
            int minLength = hints.Max<MinLengthHint, int>(-1);
            int maxLength = hints.Min<MaxLengthHint, int>(minLength);

            if (minLength == -1 && maxLength == -1)
            {
                return PrimitiveGenerators.BinarySequence();
            }

            if (minLength == -1)
            {
                minLength = 0;
            }

            return PrimitiveGenerators.BinarySequence(minLength, maxLength);
        }

        private IDataGenerator ResolveUniqueDataGenerator(Type clrType, IDataGenerator dataGenerator, IEnumerable<DataGenerationHint> hints)
        {
            var genericMethod = this.GetType().GetMethods(false, false).Single(m => m.Name == "ResolveUniqueDataGenerator" && m.IsGenericMethodDefinition);
            genericMethod = genericMethod.MakeGenericMethod(clrType);
            return (IDataGenerator)genericMethod.Invoke(this, new object[] { dataGenerator, hints });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Invoked via reflection due to the generic type")]
        private IDataGenerator<TData> ResolveUniqueDataGenerator<TData>(IDataGenerator dataGenerator, IEnumerable<DataGenerationHint> hints)
        {
            var typedGenerator = dataGenerator as IDataGenerator<TData>;
            ExceptionUtilities.CheckObjectNotNull(typedGenerator, "Generator for type '{0}' unexpectedly resolved to a null value or a generator of the wrong type", typeof(TData));

            var interestingValues = hints.OfType<InterestingValueHint<TData>>().Select(i => i.Value).ToList();
            if (interestingValues.Count > 0)
            {
                return new InterestingValuesUniqueDataGeneratorWrapper<TData>(typedGenerator, interestingValues);
            }

            return typedGenerator;
        }

        /// <summary>
        /// Class to wrap a unique data generator to extend it with interesting values
        /// </summary>
        /// <typeparam name="TData">The type of the data</typeparam>
        private class InterestingValuesUniqueDataGeneratorWrapper<TData> : IDataGenerator<TData>
        {
            private IDataGenerator<TData> uniqueGenerator;
            private Queue<TData> interestingValuesRemaining;
            private HashSet<TData> interestingValuesUsed = new HashSet<TData>();
            
            /// <summary>
            /// Initializes a new instance of the InterestingValuesUniqueDataGeneratorWrapper class
            /// </summary>
            /// <param name="uniqueGenerator">The underlying unique generator</param>
            /// <param name="interestingValues">The interesting values</param>
            public InterestingValuesUniqueDataGeneratorWrapper(IDataGenerator<TData> uniqueGenerator, IEnumerable<TData> interestingValues)
            {
                this.uniqueGenerator = uniqueGenerator;
                this.interestingValuesRemaining = new Queue<TData>(interestingValues.Distinct());
            }

            /// <summary>
            /// Generates data of the TData type.
            /// </summary>
            /// <returns>Generated data.</returns>
            public TData GenerateData()
            {
                // As each interesting value is used, add it to the hash-set so that when the underlying generator
                // produces that value we can skip it exactly once. The effect of this is that the interesting values
                // simply 'move' to the beginning of whatever sequence the underlying generator is using.
                TData value;
                if (this.interestingValuesRemaining.Count > 0)
                {
                    value = this.interestingValuesRemaining.Dequeue();
                    this.interestingValuesUsed.Add(value);
                }
                else
                {
                    // generate a value using the underlying generator, but skip interesting values we've already generated once.
                    do
                    {
                        value = this.uniqueGenerator.GenerateData();
                    }
                    while (this.interestingValuesUsed.Remove(value));
                }
                
                return value;
            }

            /// <summary>
            /// Generates data of the TData type.
            /// </summary>
            /// <returns>Generated data.</returns>
            object IDataGenerator.GenerateData()
            {
                return this.GenerateData();
            }
        }
    }
}
