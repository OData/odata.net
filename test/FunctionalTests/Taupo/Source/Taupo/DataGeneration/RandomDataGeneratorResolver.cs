//---------------------------------------------------------------------
// <copyright file="RandomDataGeneratorResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;

    /// <summary>
    /// Resolver for random data generators.
    /// </summary>
    [ImplementationName(typeof(IRandomDataGeneratorResolver), "Default")]
    public class RandomDataGeneratorResolver : IRandomDataGeneratorResolver
    {
        private static Type[] numericTypes = new Type[]
        {
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(byte),
            typeof(sbyte),
            typeof(uint),
            typeof(ulong),
            typeof(ushort),
            typeof(sbyte),
        };

        private Dictionary<Type, Func<IRandomNumberGenerator, DataGenerationHint[], bool, IDataGenerator>> dataGeneratorCreators;

        /// <summary>
        /// Initializes a new instance of the RandomDataGeneratorResolver class.
        /// </summary>
        public RandomDataGeneratorResolver()
        {
            this.dataGeneratorCreators = new Dictionary<Type, Func<IRandomNumberGenerator, DataGenerationHint[], bool, IDataGenerator>>()
                {
                  { typeof(short),      this.ResolveShortGenerator },
                  { typeof(int),        this.ResolveIntGenerator }, 
                  { typeof(long),       this.ResolveLongGenerator }, 
                  { typeof(byte),       this.ResolveByteGenerator },
                  { typeof(sbyte),      this.ResolveSByteGenerator },
                  { typeof(string),     this.ResolveStringGenerator },
                  { typeof(float),      this.ResolveFloatGenerator },
                  { typeof(double),     this.ResolveDoubleGenerator },
                  { typeof(decimal),    this.ResolveDecimalGenerator },
                  { typeof(Guid),       this.ResolveGuidGenerator },
                  { typeof(byte[]),     this.ResolveBinaryGenerator },
                  { typeof(bool),       this.ResolveBooleanGenerator },
                  { typeof(DateTime),       this.ResolveDateTimeGenerator },
                  { typeof(DateTimeOffset), this.ResolveDateTimeOffsetGenerator },
                  { typeof(TimeSpan),      this.ResolveTimeSpanGenerator },
                };

            this.AnsiStringGenerator = StringGenerators.ApiWords;

            // TODO: change this for globalization testing
            this.UnicodeStringGenerator = new WeightedStringGenerator()
            {
               { 1, StringGenerators.English },
               { 1, StringGenerators.German },
               { 1, StringGenerators.Japanese },

               // By default, disable surrogate pairs (supplementary characters), since it's causing trouble in SqlServer comparison (collation dependent)
               // for more information, see http://technet.microsoft.com/en-us/library/ms180942(SQL.90).aspx
               //// { 1, StringGenerators.SurrogateCharacters },
            };

            this.LengthLimitForUnlimitedData = 100;
        }

        /// <summary>
        /// Gets or sets unicode string generator.
        /// </summary>
        [InjectDependency]
        public IStringGenerator UnicodeStringGenerator { get; set; }

        /// <summary>
        /// Gets or sets ANSI string generator.
        /// </summary>
        [InjectDependency]
        public IAnsiStringGenerator AnsiStringGenerator { get; set; }

        /// <summary>
        /// Gets or sets length limit for unlimited data.
        /// </summary>
        [InjectTestParameter("LengthLimitForUnlimitedData", DefaultValueDescription = "100", HelpText = "Length limit for unlimited data. Example: length limit for a string property or varchar(max) column.")]
        public int LengthLimitForUnlimitedData { get; set; }

        /// <summary>
        /// Resolves data generator which generates random data.
        /// </summary>
        /// <param name="clrType">The type of the data.</param>
        /// <param name="random">Random number generator.</param>
        /// <param name="hints">Data generation hints.</param>
        /// <returns>The random data generator.</returns>
        public IDataGenerator ResolveRandomDataGenerator(Type clrType, IRandomNumberGenerator random, params DataGenerationHint[] hints)
        {
            ExceptionUtilities.CheckArgumentNotNull(clrType, "clrType");
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(hints, "hints");

            ExceptionUtilities.CheckAllRequiredDependencies(this);

            bool isNullable;
            var originalClrType = clrType;
            if (clrType.IsGenericType() && clrType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                clrType = clrType.GetGenericArguments()[0];
                isNullable = true;
            }
            else
            {
                isNullable = clrType.IsClass();
            }

            if (hints.OfType<AllNullsHint>().Any())
            {
                ExceptionUtilities.Assert(isNullable, "Cannot use all-nulls hint with non-nullable type. Type was: '{0}'", originalClrType);
                return PrimitiveGenerators.Default(originalClrType);
            }

            Func<IRandomNumberGenerator, DataGenerationHint[], bool, IDataGenerator> createDataGen;
            if (!this.dataGeneratorCreators.TryGetValue(clrType, out createDataGen))
            {
                throw new TaupoNotSupportedException(
                    string.Format(CultureInfo.InvariantCulture, "Creating data generator for the type '{0}' is not supported by this resolver.", clrType.FullName));
            }

            return createDataGen(random, hints, isNullable);
        }

        /// <summary>
        /// Resolves data generator which generates random data.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <param name="random">Random number generator.</param>
        /// <param name="hints">Data generation hints.</param>
        /// <returns>The random data generator.</returns>
        public IDataGenerator<TData> ResolveRandomDataGenerator<TData>(IRandomNumberGenerator random, params DataGenerationHint[] hints)
        {
            return (IDataGenerator<TData>)this.ResolveRandomDataGenerator(typeof(TData), random, hints);
        }

        internal static int NextFromRange(IRandomNumberGenerator random, int min, int max)
        {
            long range = (long)max - (long)min + 1;
            long rnd = (long)random.Next(int.MaxValue) * (long)random.Next(int.MaxValue);
            int result = (int)(min + (rnd % range));
            return result;
        }

        internal static double NextDoubleBetweenZeroAndOne(IRandomNumberGenerator random)
        {
            double result = (double)random.Next(int.MaxValue) * (double)random.Next(int.MaxValue) / int.MaxValue / int.MaxValue;
            return result;
        }

        internal static double NextDoubleFromRange(IRandomNumberGenerator random, double min, double max, double factorToAvoidOverflow, int fractionalDigits)
        {
            double tmpMin = min / factorToAvoidOverflow;
            double tmpMax = max / factorToAvoidOverflow;

            double result = tmpMin + (NextDoubleBetweenZeroAndOne(random) * (tmpMax - tmpMin));
            result = result * factorToAvoidOverflow;

            if (fractionalDigits >= 0)
            {
                result = Math.Round(result, fractionalDigits);
            }

            return result;
        }

        internal static float NextFloatFromRange(IRandomNumberGenerator random, float min, float max, int fractionalDigits)
        {
            double range = (double)max - (double)min;
            float result = (float)((double)min + (NextDoubleBetweenZeroAndOne(random) * range));
            
            if (fractionalDigits >= 0)
            {
                result = (float)Math.Round(result, fractionalDigits);
            }

            return result;
        }

        internal static decimal NextDecimalFromRange(IRandomNumberGenerator random, decimal min, decimal max, int scale)
        {
            double r = NextDoubleBetweenZeroAndOne(random);
            decimal result = (decimal)((double)min + (((double)max - (double)min) * r));
            
            if (scale >= 0)
            {
                decimal rounded = Math.Round(result, random.Next(scale + 1));
                while (rounded < min || max < rounded)
                {
                    rounded = Math.Round(result, random.Next(scale + 1));
                }

                return rounded;
            }

            return result;
        }

        internal static DateTime NextDateTimeFromRange(IRandomNumberGenerator random, DateTime min, DateTime max, long factor)
        {
            long range = max.Ticks - min.Ticks + 1;
            long ticks = (((long)random.Next(int.MaxValue) * (long)random.Next(int.MaxValue)) % range) + min.Ticks;
            ticks = (ticks / factor) * factor;

            return new DateTime(ticks);
        }

        internal static DateTimeOffset NextDateTimeOffsetFromRange(IRandomNumberGenerator random, DateTimeOffset min, DateTimeOffset max, long factor)
        {
            long range = (max.Ticks - max.Offset.Ticks) - (min.Ticks - min.Offset.Ticks) + 1;
            long ticks = (((long)random.Next(int.MaxValue) * (long)random.Next(int.MaxValue)) % range) + (min.Ticks - min.Offset.Ticks);
            ticks = (ticks / factor) * factor;

            var offsetTicks = (NextOffset(random).Ticks / factor) * factor;
            if (ticks + offsetTicks >= DateTime.MaxValue.Ticks || ticks + offsetTicks <= DateTime.MinValue.Ticks)
            {
                offsetTicks = 0;
            }

            var offset = new TimeSpan(offsetTicks);
            return new DateTimeOffset(ticks + offsetTicks, offset);
        }

        internal static TimeSpan NextOffset(IRandomNumberGenerator random)
        {
            int hours = random.Next(15);
            int minutes = hours == 14 ? 0 : random.Next(60);
            TimeSpan offset = new TimeSpan(hours, minutes, 0);
            if (random.Next(100) % 2 == 0)
            {
                offset = TimeSpan.Zero - offset;
            }

            return offset;
        }

        internal static TimeSpan NextTimeSpanFromRange(IRandomNumberGenerator random, TimeSpan min, TimeSpan max, long factor)
        {
            double range = (double)max.Ticks - (double)min.Ticks;
            long ticks = (long)((double)min.Ticks + (NextDoubleBetweenZeroAndOne(random) * range));
            ticks = (ticks / factor) * factor;
            return new TimeSpan(ticks);
        }

        internal static byte[] NextBytes(IRandomNumberGenerator random, int minLength, int maxLength)
        {
            int length = NextFromRange(random, minLength, maxLength);
            byte[] data = new byte[length];

            for (int i = 0; i < length; i++)
            {
                data[i] = (byte)random.Next(256);
            }

            return data;
        }

        private static void AdjustTicksBasedOnFactorAndCheckRange(DataGenerationHint[] hints, ref long minTicks, ref long maxTicks, out long factor)
        {
            factor = GetDateTimeFactor(hints);
            if (factor != 1)
            {
                long roundedMinTicks = (minTicks / factor) * factor;
                long roundedMaxTicks = (maxTicks / factor) * factor;
                if (roundedMinTicks < minTicks)
                {
                    roundedMinTicks += factor;
                }

                if (roundedMaxTicks > maxTicks)
                {
                    roundedMaxTicks -= factor;
                }

                minTicks = roundedMinTicks;
                maxTicks = roundedMaxTicks;
            }

            ExceptionUtilities.CheckValidRange(minTicks, "minimum ticks based on hints", maxTicks, "maximum ticks based on hints");
        }

        private static DateTime GetMaxValueAndCheckRange(DataGenerationHint[] hints, out DateTime minValue, out long factor)
        {
            long minTicks = hints.Max<MinValueHint<DateTime>, DateTime>(DateTime.MinValue).Ticks;
            long maxTicks = hints.Min<MaxValueHint<DateTime>, DateTime>(DateTime.MaxValue).Ticks;

            AdjustTicksBasedOnFactorAndCheckRange(hints, ref minTicks, ref maxTicks, out factor);

            minValue = new DateTime(minTicks);

            return new DateTime(maxTicks);
        }

        private static DateTimeOffset GetMaxValueAndCheckRange(DataGenerationHint[] hints, out DateTimeOffset minValue, out long factor)
        {
            minValue = hints.Max<MinValueHint<DateTimeOffset>, DateTimeOffset>(DateTimeOffset.MinValue);
            DateTimeOffset maxValue = hints.Min<MaxValueHint<DateTimeOffset>, DateTimeOffset>(DateTimeOffset.MaxValue);

            long minTicks = minValue.Ticks - minValue.Offset.Ticks;
            long maxTicks = maxValue.Ticks - maxValue.Offset.Ticks;

            AdjustTicksBasedOnFactorAndCheckRange(hints, ref minTicks, ref maxTicks, out factor);

            var minValueOffsetTicks = (minValue.Offset.Ticks / factor) * factor;
            var maxValueOffsetTicks = (maxValue.Offset.Ticks / factor) * factor;
            
            minValue = new DateTimeOffset(minTicks + minValueOffsetTicks, new TimeSpan(minValueOffsetTicks));

            return new DateTimeOffset(maxTicks + maxValueOffsetTicks, new TimeSpan(maxValueOffsetTicks));
        }

        private static TimeSpan GetMaxValueAndCheckRange(DataGenerationHint[] hints, out TimeSpan minValue, out long factor)
        {
            long minTicks = hints.Max<MinValueHint<TimeSpan>, TimeSpan>(TimeSpan.MinValue).Ticks;
            long maxTicks = hints.Min<MaxValueHint<TimeSpan>, TimeSpan>(TimeSpan.MaxValue).Ticks;

            AdjustTicksBasedOnFactorAndCheckRange(hints, ref minTicks, ref maxTicks, out factor);

            minValue = new TimeSpan(minTicks);

            return new TimeSpan(maxTicks);
        }

        private static TData GetNumericMaxValueAndCheckRange<TData>(DataGenerationHint[] hints, TData defaultMinValue, TData defaultMaxValue, out TData minValue)
            where TData : struct, IComparable<TData>
        {
            // For numeric types max and min hints could be of different type, 
            // Example: max and min value could be int, while value type is double
            List<double> minValues = new List<double>();
            List<double> maxValues = new List<double>();

            minValues.Add((double)Convert.ChangeType(defaultMinValue, typeof(double), CultureInfo.InvariantCulture));
            maxValues.Add((double)Convert.ChangeType(defaultMaxValue, typeof(double), CultureInfo.InvariantCulture));

            foreach (var hint in hints)
            {
                Type hintType = hint.GetType();
                if (hintType.IsGenericType())
                {
                    Type hintGenericType = hintType.GetGenericTypeDefinition();
                    Type valueType = hintType.GetGenericArguments()[0];

                    if (numericTypes.Contains(valueType))
                    {
                        List<double> listToAdd = null;
                        if (hintGenericType == typeof(MaxValueHint<>))
                        {
                            listToAdd = maxValues;
                        }
                        else if (hintGenericType == typeof(MinValueHint<>))
                        {
                            listToAdd = minValues;
                        }

                        // not all generic hints are min/max value, so don't assume the list was populated
                        if (listToAdd != null)
                        {
                            object value = hintType.GetProperty("Value").GetValue(hint, null);
                            listToAdd.Add((double)Convert.ChangeType(value, typeof(double), CultureInfo.InvariantCulture));
                        }
                    }
                }
            }

            double doubleMinValue = minValues.Max();
            double doubleMaxValue = maxValues.Min();
            ExceptionUtilities.CheckValidRange(doubleMinValue, "minimum value from hints", doubleMaxValue, "maximum value from hints");
            object convertedMinValue = WorkaroundForConvert(doubleMinValue, typeof(TData));
            object convertedMaxValue = WorkaroundForConvert(doubleMaxValue, typeof(TData));

            if ((double)Convert.ChangeType(convertedMinValue, typeof(double), CultureInfo.InvariantCulture) < doubleMinValue)
            {
                convertedMinValue = Convert.ChangeType(doubleMinValue + 1, typeof(TData), CultureInfo.InvariantCulture);
            }

            if ((double)Convert.ChangeType(convertedMaxValue, typeof(double), CultureInfo.InvariantCulture) > doubleMaxValue)
            {
                convertedMaxValue = Convert.ChangeType(doubleMaxValue - 1, typeof(TData), CultureInfo.InvariantCulture);
            }

            minValue = (TData)convertedMinValue;
            TData maxValue = (TData)convertedMaxValue;

            ExceptionUtilities.CheckValidRange(minValue, "minimum value from hints", maxValue, "maximum value from hints");

            return maxValue;
        }

        private static object WorkaroundForConvert(double value, Type targetType)
        {
            if (value == long.MaxValue && targetType == typeof(long))
            {
                return long.MaxValue;
            }

            if (targetType == typeof(decimal))
            {
                if (value == (double)decimal.MinValue)
                {
                    return decimal.MinValue;
                }

                if (value == (double)decimal.MaxValue)
                {
                    return decimal.MaxValue;
                }
            }

            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }

        private static double GetMaxValueAndCheckRange(DataGenerationHint[] hints, double defaultMinValue, double defaultMaxValue, out double minValue, out int fractionalDigits)
        {
            double maxValue = GetNumericMaxValueAndCheckRange(hints, defaultMinValue, defaultMaxValue, out minValue);

            fractionalDigits = hints.Min<FractionalDigitsHint, int>(-1);

            if (fractionalDigits >= 0 && fractionalDigits <= DataGenerationUtilities.MaxFractionalDigits)
            {
                minValue = AdjustMin(minValue, fractionalDigits);
                maxValue = AdjustMax(maxValue, fractionalDigits);
            }

            ExceptionUtilities.CheckValidRange(minValue, "minimum value based on hints", maxValue, "maximum value based on hints");

            return maxValue;
        }

        private static double GetMaxValueAndCheckRange(DataGenerationHint[] hints, out double minValue, out int fractionalDigits)
        {
            return GetMaxValueAndCheckRange(hints, double.MinValue, double.MaxValue, out minValue, out fractionalDigits);
        }

        private static float GetMaxValueAndCheckRange(DataGenerationHint[] hints, out float minValue, out int fractionalDigits)
        {
            double doubleMinValue;
            double doubleMaxValue = GetMaxValueAndCheckRange(hints, float.MinValue, float.MaxValue, out doubleMinValue, out fractionalDigits);

            minValue = (float)doubleMinValue;

            return (float)doubleMaxValue;
        }

        private static decimal GetMaxValueAndCheckRange(DataGenerationHint[] hints, out decimal minValue, ref int scale)
        {
            decimal maxValue = GetNumericMaxValueAndCheckRange(hints, decimal.MinValue, decimal.MaxValue, out minValue);

            int? precisionFromHint = hints.OfType<NumericPrecisionHint>().Any() ? hints.OfType<NumericPrecisionHint>().Min(h => h.Value) : (int?)null;
            int? scaleFromHint = hints.OfType<NumericScaleHint>().Any() ? hints.OfType<NumericScaleHint>().Min(h => h.Value) : (int?)null;

            if (!precisionFromHint.HasValue && !scaleFromHint.HasValue)
            {
                scale = -1;
            }
            else
            {
                scale = !scaleFromHint.HasValue ? Math.Min(precisionFromHint.Value, scale) : scaleFromHint.Value;
                DataGenerationUtilities.CheckNumericScale(scale);

                minValue = AdjustMin(minValue, scale);
                maxValue = AdjustMax(maxValue, scale);

                if (precisionFromHint.HasValue)
                {
                    DataGenerationUtilities.CheckNumericPrecisionAndScale(precisionFromHint.Value, scale);
                    decimal maxBasedOnPrecisionAndScale = (decimal)Math.Pow(10, precisionFromHint.Value - scale) - (1 / (decimal)Math.Pow(10, scale));
                    decimal minBasedOnPrecisionAndScale = -maxBasedOnPrecisionAndScale;

                    minValue = Math.Max(minValue, minBasedOnPrecisionAndScale);
                    maxValue = Math.Min(maxValue, maxBasedOnPrecisionAndScale);
                }

                ExceptionUtilities.CheckValidRange(minValue, "minimum value based on hints", maxValue, "maximum value based on hints");
            }

            return maxValue;
        }

        private static decimal AdjustMin(decimal minValue, int scale)
        {
            decimal rounded = Math.Round(minValue, scale);
            if (rounded < minValue)
            {
                rounded = rounded + (1 / (decimal)Math.Pow(10, scale));
            }

            return rounded;
        }

        private static decimal AdjustMax(decimal maxValue, int scale)
        {
            decimal rounded = Math.Round(maxValue, scale);
            if (rounded > maxValue)
            {
                rounded = rounded - (1 / (decimal)Math.Pow(10, scale));
            }

            return rounded;
        }

        private static double AdjustMin(double minValue, int fractionalDigits)
        {
            double rounded = Math.Round(minValue, fractionalDigits);
            if (rounded < minValue)
            {
                rounded = rounded + (1 / Math.Pow(10, fractionalDigits));
            }

            return rounded;
        }

        private static double AdjustMax(double maxValue, int fractionalDigits)
        {
            double rounded = Math.Round(maxValue, fractionalDigits);
            if (rounded > maxValue)
            {
                rounded = rounded - (1 / Math.Pow(10, fractionalDigits));
            }

            return rounded;
        }

        private static long GetDateTimeFactor(DataGenerationHint[] hints)
        {
            bool noSeconds = hints.OfType<NoSecondsHint>().Any();
            bool noTime = hints.OfType<NoTimeHint>().Any();

            long factor = noTime ? TimeSpan.TicksPerDay : (noSeconds ? TimeSpan.TicksPerMinute : 1);

            if (factor == 1)
            {
                int fractionalSeconds = hints.Min<FractionalSecondsHint, int>(-1);

                if (fractionalSeconds >= 0 && fractionalSeconds < DataGenerationUtilities.MaxFractionalSeconds)
                {
                    factor = (long)Math.Pow(10, DataGenerationUtilities.MaxFractionalSeconds - fractionalSeconds);
                }
            }

            return factor;
        }

        private static bool NullsAllowed(IEnumerable<DataGenerationHint> hints)
        {
            return !hints.OfType<NoNullsHint>().Any();
        }

        private static IDataGenerator CreateRandomValueTypeGenerator<TData>(
                        IRandomNumberGenerator random,
                        Func<IRandomNumberGenerator, TData> generateDataDelegate,
                        bool isNullable,
                        IEnumerable<DataGenerationHint> hints,
                        params TData[] interestingValues)
                where TData : struct
        {
            return CreateRandomValueTypeGenerator<TData>(
                    random,
                    new Func<IRandomNumberGenerator, TData>[] { generateDataDelegate },
                    isNullable,
                    hints,
                    interestingValues);
        }

        private static IDataGenerator CreateRandomValueTypeGenerator<TData>(
                        IRandomNumberGenerator random,
                        IEnumerable<Func<IRandomNumberGenerator, TData>> generateDataDelegates,
                        bool isNullable,
                        IEnumerable<DataGenerationHint> hints,
                        params TData[] interestingValues)
                where TData : struct
        {
            var interestingValuesWithHints = new HashSet<TData>(interestingValues);
            foreach (var interestingValueHint in hints.OfType<InterestingValueHint<TData>>())
            {
                interestingValuesWithHints.Add(interestingValueHint.Value);
            }

            interestingValues = interestingValuesWithHints.ToArray();

            bool useRandom = !hints.OfType<NoRandomValuesHint>().Any();

            if (isNullable)
            {
                List<TData?> values = interestingValues.Cast<TData?>().ToList();
                if (NullsAllowed(hints))
                {
                    values.Insert(0, null);
                }

                if (!useRandom)
                {
                    return new FixedSetDataGenerator<TData?>(random, values.ToArray());
                }

                return new DelegatedRandomDataGenerator<TData?>(
                    random,
                    generateDataDelegates.Select<Func<IRandomNumberGenerator, TData>, Func<IRandomNumberGenerator, TData?>>(g => (r) => (TData?)g(r)),
                    values.ToArray());
            }
            else
            {
                if (!useRandom)
                {
                    return new FixedSetDataGenerator<TData>(random, interestingValues);
                }

                return new DelegatedRandomDataGenerator<TData>(random, generateDataDelegates, interestingValues);
            }
        }

        private static IDataGenerator CreateRandomGenerator<TData>(
                        IRandomNumberGenerator random,
                        Func<IRandomNumberGenerator, TData> generateDataDelegate,
                        IEnumerable<DataGenerationHint> hints,
                        params TData[] interestingValues)
                where TData : class
        {
            List<TData> values = interestingValues.ToList();
            if (NullsAllowed(hints))
            {
                values.Insert(0, null);
            }

            return new DelegatedRandomDataGenerator<TData>(random, generateDataDelegate, values.ToArray());
        }

        private IDataGenerator ResolveBinaryGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            int minLength;
            int maxLength = this.GetMaxLengthAndCheckLengthRange(hints, out minLength);

            return CreateRandomGenerator<byte[]>(random, (r) => NextBytes(r, minLength, maxLength), hints);
        }

        private IDataGenerator ResolveStringGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            int minLength;
            int maxLength = this.GetMaxLengthAndCheckLengthRange(hints, out minLength);

            bool ansiString = hints.OfType<AnsiStringHint>().Any();

            IStringGenerator stringGenerator = ansiString ? this.AnsiStringGenerator : this.UnicodeStringGenerator;

            var stringPrefixHint = hints.OfType<StringPrefixHint>().SingleOrDefault();
            if (stringPrefixHint != null)
            {
                string prefix = stringPrefixHint.Value;
                ExceptionUtilities.Assert(prefix.Length < maxLength, "the length of string prefix in data generation exceeds the max length limit");
                return CreateRandomGenerator<string>(random, (r) => prefix + stringGenerator.GenerateString(r, Math.Max(minLength - prefix.Length, 0), maxLength - prefix.Length), hints);
            }

            return CreateRandomGenerator<string>(random, (r) => stringGenerator.GenerateString(r, minLength, maxLength), hints);
        }

        private IDataGenerator ResolveDateTimeGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            long factor;
            DateTime minValue;
            DateTime maxValue = GetMaxValueAndCheckRange(hints, out minValue, out factor);

            List<DateTime> interestingValues = new List<DateTime>() { minValue, maxValue };
            DateTime now = DateTime.Parse("2013-08-07T09:27:34.1548726-07:00");
            if (minValue < now && now < maxValue)
            {
                interestingValues.Add(now);
            }

            List<Func<IRandomNumberGenerator, DateTime>> generateDataDelegates = new List<Func<IRandomNumberGenerator, DateTime>>();

            generateDataDelegates.Add((r) => NextDateTimeFromRange(r, minValue, maxValue, factor));

            generateDataDelegates.Add((r) => new DateTime(NextDateTimeFromRange(r, minValue, maxValue, factor).Ticks, DateTimeKind.Local));

            TimeSpan oneYearInterval = TimeSpan.FromDays(365);
            if (minValue < now - oneYearInterval && now + oneYearInterval < maxValue)
            {
                generateDataDelegates.Add((r) => NextDateTimeFromRange(r, now - oneYearInterval, now + oneYearInterval, factor));
            }

            TimeSpan tenYearsInterval = TimeSpan.FromDays(3650);
            if (minValue < now - tenYearsInterval && now + tenYearsInterval < maxValue)
            {
                generateDataDelegates.Add((r) => NextDateTimeFromRange(r, now - tenYearsInterval, now + tenYearsInterval, factor));
            }

            return CreateRandomValueTypeGenerator(
                        random,
                        generateDataDelegates,
                        isNullable,
                        hints,
                        interestingValues.ToArray());
        }

        private IDataGenerator ResolveDateTimeOffsetGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            long factor;
            DateTimeOffset minValue;
            DateTimeOffset maxValue = GetMaxValueAndCheckRange(hints, out minValue, out factor);

            List<DateTimeOffset> interestingValues = new List<DateTimeOffset>() { minValue, maxValue };
            DateTimeOffset now = new DateTimeOffset(DateTime.Parse("2013-02-3T09:27:34.1548726-07:00"));
            if (minValue < now && now < maxValue)
            {
                interestingValues.Add(now);
            }

            List<Func<IRandomNumberGenerator, DateTimeOffset>> generateDataDelegates = new List<Func<IRandomNumberGenerator, DateTimeOffset>>();

            generateDataDelegates.Add((r) => NextDateTimeOffsetFromRange(r, minValue, maxValue, factor));

            TimeSpan oneYearInterval = TimeSpan.FromDays(365);
            if (minValue < now - oneYearInterval && now + oneYearInterval < maxValue)
            {
                generateDataDelegates.Add((r) => NextDateTimeOffsetFromRange(r, now - oneYearInterval, now + oneYearInterval, factor));
            }

            TimeSpan tenYearsInterval = TimeSpan.FromDays(3650);
            if (minValue < now - tenYearsInterval && now + tenYearsInterval < maxValue)
            {
                generateDataDelegates.Add((r) => NextDateTimeOffsetFromRange(r, now - tenYearsInterval, now + tenYearsInterval, factor));
            }

            return CreateRandomValueTypeGenerator(
                        random,
                        generateDataDelegates,
                        isNullable,
                        hints,
                        interestingValues.ToArray());
        }

        private IDataGenerator ResolveTimeSpanGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            long factor;
            TimeSpan minValue;
            TimeSpan maxValue = GetMaxValueAndCheckRange(hints, out minValue, out factor);

            List<TimeSpan> interestingValues = new List<TimeSpan>() { minValue, maxValue };
            if (minValue < TimeSpan.Zero && TimeSpan.Zero < maxValue)
            {
                interestingValues.Add(TimeSpan.Zero);
            }

            List<Func<IRandomNumberGenerator, TimeSpan>> generateDataDelegates = new List<Func<IRandomNumberGenerator, TimeSpan>>();

            generateDataDelegates.Add((r) => NextTimeSpanFromRange(r, minValue, maxValue, factor));

            TimeSpan oneMonthInterval = TimeSpan.FromDays(30);
            if (minValue < TimeSpan.Zero - oneMonthInterval && oneMonthInterval < maxValue)
            {
                generateDataDelegates.Add((r) => NextTimeSpanFromRange(r, TimeSpan.Zero - oneMonthInterval, oneMonthInterval, factor));
            }

            TimeSpan oneYearInterval = TimeSpan.FromDays(365);
            if (minValue < TimeSpan.Zero - oneYearInterval && oneYearInterval < maxValue)
            {
                generateDataDelegates.Add((r) => NextTimeSpanFromRange(r, TimeSpan.Zero - oneYearInterval, oneYearInterval, factor));
            }

            return CreateRandomValueTypeGenerator(
                        random,
                        generateDataDelegates,
                        isNullable,
                        hints,
                        interestingValues.ToArray());
        }

        private IDataGenerator ResolveDecimalGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            List<Func<IRandomNumberGenerator, decimal>> generateDataDelegates = new List<Func<IRandomNumberGenerator, decimal>>();

            int scale = random.Next(DataGenerationUtilities.MaxNumericScale);
            decimal minValue;
            decimal maxValue = GetMaxValueAndCheckRange(hints, out minValue, ref scale);
            generateDataDelegates.Add((r) => NextDecimalFromRange(r, minValue, maxValue, scale));
     
            // Add generator from -1 to 1
            if (minValue <= -1  && 1 <= maxValue)
            {
                generateDataDelegates.Add((r) => NextDecimalFromRange(r, -1, 1, scale));
            }

            // Add "small" range generator
            if (minValue <= -100 && 100 <= maxValue)
            {
                generateDataDelegates.Add((r) => NextDecimalFromRange(r, -100, 100, scale));
            }

            // Add "medium" range generator
            if (minValue <= -10000 && 10000 <= maxValue)
            {
                generateDataDelegates.Add((r) => NextDecimalFromRange(r, -10000, 10000, scale));
            }

            List<decimal> interestingValues = new List<decimal>() { minValue, maxValue };
            if (minValue < 0 && 0 < maxValue)
            {
                interestingValues.Add(0);
            }

            return CreateRandomValueTypeGenerator(
                        random,
                        generateDataDelegates,
                        isNullable,
                        hints,
                        interestingValues.ToArray());
        }

        private IDataGenerator ResolveDoubleGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            List<Func<IRandomNumberGenerator, double>> generateDataDelegates = new List<Func<IRandomNumberGenerator, double>>();

            int fractionalDigits;
            double minValue;
            double maxValue = GetMaxValueAndCheckRange(hints, out minValue, out fractionalDigits);

            double factorToAvoidOverflow = 1;
            if (minValue <= double.MinValue / 2 || maxValue >= double.MaxValue / 2)
            {
                factorToAvoidOverflow = Math.Max(Math.Abs(minValue), Math.Abs(maxValue)) / 2;
            }

            generateDataDelegates.Add((r) => NextDoubleFromRange(r, minValue, maxValue, factorToAvoidOverflow, fractionalDigits));

            // Add generator from -1 to 1
            if (-1 >= minValue && 1 <= maxValue)
            {
                generateDataDelegates.Add((r) => NextDoubleFromRange(r, -1, 1, 1, fractionalDigits));
            }

            // Add "small" range generator
            if (minValue <= -100 && 100 <= maxValue)
            {
                generateDataDelegates.Add((r) => NextDoubleFromRange(r, -100, 100, 1, fractionalDigits));
            }

            // Add "medium" range generator
            if (minValue <= -10000 && 10000 <= maxValue)
            {
                generateDataDelegates.Add((r) => NextDoubleFromRange(r, -10000, 10000, 1, fractionalDigits));
            }

            List<double> interestingValues = new List<double>() { minValue, maxValue };
            if (minValue < 0 && 0 < maxValue)
            {
                interestingValues.Add(0);
            }

            return CreateRandomValueTypeGenerator(
                        random,
                        generateDataDelegates,
                        isNullable,
                        hints,
                        interestingValues.ToArray());
        }

        private IDataGenerator ResolveFloatGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            List<Func<IRandomNumberGenerator, float>> generateDataDelegates = new List<Func<IRandomNumberGenerator, float>>();

            int fractionalDigits;
            float minValue;
            float maxValue = GetMaxValueAndCheckRange(hints, out minValue, out fractionalDigits);

            generateDataDelegates.Add((r) => NextFloatFromRange(r, minValue, maxValue, fractionalDigits));

            // Add generator from -1 to 1
            if (minValue <= -1 && 1 <= maxValue)
            {
                generateDataDelegates.Add((r) => NextFloatFromRange(r, -1, 1, fractionalDigits));
            }

            // Add "small" range generator
            if (minValue <= -100 && 100 <= maxValue)
            {
                generateDataDelegates.Add((r) => NextFloatFromRange(r, -100, 100, fractionalDigits));
            }

            // Add "medium" range generator
            if (minValue <= -10000 && 10000 <= maxValue)
            {
                generateDataDelegates.Add((r) => NextFloatFromRange(r, 10000, 10000, fractionalDigits));
            }

            List<float> interestingValues = new List<float>() { minValue, maxValue };
            if (minValue < 0 && 0 < maxValue)
            {
                interestingValues.Add(0);
            }

            return CreateRandomValueTypeGenerator(
                        random,
                        generateDataDelegates,
                        isNullable,
                        hints,
                        interestingValues.ToArray());
        }

        private IDataGenerator ResolveShortGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            List<Func<IRandomNumberGenerator, short>> generateDataDelegates = new List<Func<IRandomNumberGenerator, short>>();

            short minValue;
            short maxValue = GetNumericMaxValueAndCheckRange(hints, short.MinValue, short.MaxValue, out minValue);

            generateDataDelegates.Add((r) => (short)NextFromRange(r, minValue, maxValue));

            // Add "small" range generator
            if (minValue <= -100 && 100 <= maxValue)
            {
                generateDataDelegates.Add((r) => (short)NextFromRange(r, -100, 100));
            }

            // Add "medium" range generator
            if (minValue <= -10000 && 10000 <= maxValue)
            {
                generateDataDelegates.Add((r) => (short)NextFromRange(r, -10000, 10000));
            }

            List<short> interestingValues = new List<short>() { minValue, maxValue };
            if (minValue < 0 && 0 < maxValue)
            {
                interestingValues.Add(0);
            }

            return CreateRandomValueTypeGenerator(
                        random,
                        generateDataDelegates,
                        isNullable,
                        hints,
                        interestingValues.ToArray());
        }

        private IDataGenerator ResolveIntGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            List<Func<IRandomNumberGenerator, int>> generateDataDelegates = new List<Func<IRandomNumberGenerator, int>>(); 
            
            int minValue;
            int maxValue = GetNumericMaxValueAndCheckRange(hints, int.MinValue, int.MaxValue, out minValue);

            generateDataDelegates.Add((r) => NextFromRange(r, minValue, maxValue));

            // Add "small" range generator
            if (minValue <= -100 && 100 <= maxValue)
            {
                generateDataDelegates.Add((r) => NextFromRange(r, -100, 100));
            }

            // Add "medium" range generator
            if (minValue <= -10000 && 10000 <= maxValue)
            {
                generateDataDelegates.Add((r) => NextFromRange(r, -10000, 10000));
            }

            List<int> interestingValues = new List<int>() { minValue, maxValue };
            if (minValue < 0 && 0 < maxValue)
            {
                interestingValues.Add(0);
            }

            return CreateRandomValueTypeGenerator(
                        random,
                        generateDataDelegates,
                        isNullable,
                        hints,
                        interestingValues.ToArray());
        }

        private IDataGenerator ResolveLongGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            List<Func<IRandomNumberGenerator, long>> generateDataDelegates = new List<Func<IRandomNumberGenerator, long>>();
            
            long minValue;
            long maxValue = GetNumericMaxValueAndCheckRange(hints, long.MinValue, long.MaxValue, out minValue);
            double range = (double)maxValue - (double)minValue;

            generateDataDelegates.Add((r) => (long)((double)minValue + (NextDoubleBetweenZeroAndOne(r) * range)));
            
            // Add "small" range generator
            if (minValue <= -100 && 100 <= maxValue)
            {
                generateDataDelegates.Add((r) => NextFromRange(r, -100, 100));
            }

            // Add "medium" range generator
            if (minValue <= -10000 && 10000 <= maxValue)
            {
                generateDataDelegates.Add((r) => NextFromRange(r, -10000, 10000));
            }

            List<long> interestingValues = new List<long>() { minValue, maxValue };
            if (minValue < 0 && 0 < maxValue)
            {
                interestingValues.Add(0);
            }

            return CreateRandomValueTypeGenerator(
                        random,
                        generateDataDelegates,
                        isNullable,
                        hints,
                        interestingValues.ToArray());
        }

        private IDataGenerator ResolveByteGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            byte minValue;
            byte maxValue = GetNumericMaxValueAndCheckRange(hints, byte.MinValue, byte.MaxValue, out minValue);

            return CreateRandomValueTypeGenerator(
                        random,
                        (r) => (byte)NextFromRange(r, minValue, maxValue),
                        isNullable,
                        hints,
                        minValue,
                        maxValue);
        }

        private IDataGenerator ResolveSByteGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            sbyte minValue;
            sbyte maxValue = GetNumericMaxValueAndCheckRange(hints, sbyte.MinValue, sbyte.MaxValue, out minValue);

            return CreateRandomValueTypeGenerator(
                        random,
                        (r) => (sbyte)NextFromRange(r, minValue, maxValue),
                        isNullable,
                        hints,
                        minValue,
                        maxValue);
        }

        private IDataGenerator ResolveBooleanGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            return CreateRandomValueTypeGenerator(
                        random,
                        (r) => r.Next(100) % 2 == 0,
                        isNullable,
                        hints);
        }

        private IDataGenerator ResolveGuidGenerator(IRandomNumberGenerator random, DataGenerationHint[] hints, bool isNullable)
        {
            return CreateRandomValueTypeGenerator(
                        random,
                        (r) => Guid.NewGuid(),
                        isNullable,
                        hints,
                        Guid.Empty);
        }

        private int GetMaxLengthAndCheckLengthRange(DataGenerationHint[] hints, out int minLength)
        {
            minLength = hints.Max<MinLengthHint, int>(0);
            int maxLength = hints.Min<MaxLengthHint, int>(int.MaxValue);

            if (maxLength > this.LengthLimitForUnlimitedData)
            {
                maxLength = Math.Max(minLength, this.LengthLimitForUnlimitedData);
            }

            ExceptionUtilities.CheckValidRange(minLength, "minimum length hint", maxLength, "maximum length hint");
            return maxLength;
        }
    }
}
