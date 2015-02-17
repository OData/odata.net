//---------------------------------------------------------------------
// <copyright file="DataGenerationHints.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Spatial;

    /// <summary>
    /// Available data generation hints.
    /// </summary>
    public static class DataGenerationHints
    {
        private static readonly XName ValueAttributeName = "Value";
        private static Dictionary<string, Func<string, DataGenerationHint>> hintsCreators;

        /// <summary>
        /// Initializes static members of the DataGenerationHints class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Need explicit constructor to suppress the next message in source code instead of global suppression list.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complexity is not really high.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class contains all data-generation hints so the class coupling is by design.")]
        static DataGenerationHints()
        {
            hintsCreators = new Dictionary<string, Func<string, DataGenerationHint>>()
            {
                { GetHintKey<AnsiStringHint>(), (s) => DataGenerationHints.AnsiString },
                { GetHintKey<NoNullsHint>(), (s) => DataGenerationHints.NoNulls },
                { GetHintKey<EnumMembersOnlyHint>(), (s) => DataGenerationHints.EnumMembersOnly },
                { GetHintKey<AllNullsHint>(), (s) => DataGenerationHints.AllNulls },
                { GetHintKey<AllUniqueHint>(), (s) => DataGenerationHints.AllUnique },
                { GetHintKey<NoSecondsHint>(), (s) => DataGenerationHints.NoSeconds },
                { GetHintKey<NoTimeHint>(), (s) => DataGenerationHints.NoTime },
                { GetHintKey<NoRandomValuesHint>(), (s) => DataGenerationHints.NoRandomValues },
                { GetHintKey<CollectionMaxCountHint>(), (s) => DataGenerationHints.MaxCount(XmlConvert.ToInt32(s)) },
                { GetHintKey<CollectionMinCountHint>(), (s) => DataGenerationHints.MinCount(XmlConvert.ToInt32(s)) },
                { GetHintKey<FractionalDigitsHint>(), (s) => DataGenerationHints.FractionalDigits(XmlConvert.ToInt32(s)) },
                { GetHintKey<FractionalSecondsHint>(), (s) => DataGenerationHints.FractionalSeconds(XmlConvert.ToInt32(s)) },
                { GetHintKey<MaxLengthHint>(), (s) => DataGenerationHints.MaxLength(XmlConvert.ToInt32(s)) },
                { GetHintKey<MinLengthHint>(), (s) => DataGenerationHints.MinLength(XmlConvert.ToInt32(s)) },
                { GetHintKey<NumericPrecisionHint>(), (s) => DataGenerationHints.NumericPrecision(XmlConvert.ToInt32(s)) },
                { GetHintKey<NumericScaleHint>(), (s) => DataGenerationHints.NumericScale(XmlConvert.ToInt32(s)) },
                { GetHintKey<MaxValueHint<int>>(), (s) => DataGenerationHints.MaxValue(XmlConvert.ToInt32(s)) },
                { GetHintKey<MinValueHint<int>>(), (s) => DataGenerationHints.MinValue(XmlConvert.ToInt32(s)) },
                { GetHintKey<MaxValueHint<short>>(), (s) => DataGenerationHints.MaxValue(XmlConvert.ToInt16(s)) },
                { GetHintKey<MinValueHint<short>>(), (s) => DataGenerationHints.MinValue(XmlConvert.ToInt16(s)) },
                { GetHintKey<MaxValueHint<long>>(), (s) => DataGenerationHints.MaxValue(XmlConvert.ToInt64(s)) },
                { GetHintKey<MinValueHint<long>>(), (s) => DataGenerationHints.MinValue(XmlConvert.ToInt64(s)) },
                { GetHintKey<MaxValueHint<byte>>(), (s) => DataGenerationHints.MaxValue(XmlConvert.ToByte(s)) },
                { GetHintKey<MinValueHint<byte>>(), (s) => DataGenerationHints.MinValue(XmlConvert.ToByte(s)) },
                { GetHintKey<MaxValueHint<DateTime>>(), (s) => DataGenerationHints.MaxValue(PlatformHelper.ConvertStringToDateTime(s)) },
                { GetHintKey<MinValueHint<DateTime>>(), (s) => DataGenerationHints.MinValue(PlatformHelper.ConvertStringToDateTime(s)) },
                { GetHintKey<MaxValueHint<DateTimeOffset>>(), (s) => DataGenerationHints.MaxValue(XmlConvert.ToDateTimeOffset(s)) },
                { GetHintKey<MinValueHint<DateTimeOffset>>(), (s) => DataGenerationHints.MinValue(XmlConvert.ToDateTimeOffset(s)) },
                { GetHintKey<MaxValueHint<TimeSpan>>(), (s) => DataGenerationHints.MaxValue(XmlConvert.ToTimeSpan(s)) },
                { GetHintKey<MinValueHint<TimeSpan>>(), (s) => DataGenerationHints.MinValue(XmlConvert.ToTimeSpan(s)) },
                { GetHintKey<MaxValueHint<float>>(), (s) => DataGenerationHints.MaxValue(XmlConvert.ToSingle(s)) },
                { GetHintKey<MinValueHint<float>>(), (s) => DataGenerationHints.MinValue(XmlConvert.ToSingle(s)) },
                { GetHintKey<MaxValueHint<double>>(), (s) => DataGenerationHints.MaxValue(XmlConvert.ToDouble(s)) },
                { GetHintKey<MinValueHint<double>>(), (s) => DataGenerationHints.MinValue(XmlConvert.ToDouble(s)) },
                { GetHintKey<MaxValueHint<decimal>>(), (s) => DataGenerationHints.MaxValue(XmlConvert.ToDecimal(s)) },
                { GetHintKey<MinValueHint<decimal>>(), (s) => DataGenerationHints.MinValue(XmlConvert.ToDecimal(s)) },
                { GetHintKey<InterestingValueHint<int>>(), (s) => DataGenerationHints.InterestingValue(XmlConvert.ToInt32(s)) },
                { GetHintKey<InterestingValueHint<short>>(), (s) => DataGenerationHints.InterestingValue(XmlConvert.ToInt16(s)) },
                { GetHintKey<InterestingValueHint<long>>(), (s) => DataGenerationHints.InterestingValue(XmlConvert.ToInt64(s)) },
                { GetHintKey<InterestingValueHint<byte>>(), (s) => DataGenerationHints.InterestingValue(XmlConvert.ToByte(s)) },
                { GetHintKey<InterestingValueHint<DateTime>>(), (s) => DataGenerationHints.InterestingValue(PlatformHelper.ConvertStringToDateTime(s)) },
                { GetHintKey<InterestingValueHint<DateTimeOffset>>(), (s) => DataGenerationHints.InterestingValue(XmlConvert.ToDateTimeOffset(s)) },
                { GetHintKey<InterestingValueHint<TimeSpan>>(), (s) => DataGenerationHints.InterestingValue(XmlConvert.ToTimeSpan(s)) },
                { GetHintKey<InterestingValueHint<float>>(), (s) => DataGenerationHints.InterestingValue(XmlConvert.ToSingle(s)) },
                { GetHintKey<InterestingValueHint<double>>(), (s) => DataGenerationHints.InterestingValue(XmlConvert.ToDouble(s)) },
                { GetHintKey<InterestingValueHint<decimal>>(), (s) => DataGenerationHints.InterestingValue(XmlConvert.ToDecimal(s)) },
                { GetHintKey<InterestingValueHint<Guid>>(), (s) => DataGenerationHints.InterestingValue(XmlConvert.ToGuid(s)) },
                { GetHintKey<InterestingValueHint<bool>>(), (s) => DataGenerationHints.InterestingValue(XmlConvert.ToBoolean(s)) },
                { GetHintKey<InterestingValueHint<string>>(), (s) => DataGenerationHints.InterestingValue(s) },
                { GetHintKey<StringPrefixHint>(), (s) => DataGenerationHints.StringPrefixHint(s) },
                { GetHintKey<SpatialReferenceNumberHint>(), (s) => DataGenerationHints.SpatialReferenceNumber(XmlConvert.ToInt32(s)) },
                { GetHintKey<SpatialShapeKindHint>(), (s) => DataGenerationHints.SpatialShapeKind((SpatialShapeKind)Enum.Parse(typeof(SpatialShapeKind), s, true)) },
            };
        }

        /// <summary>
        /// Gets the Asni string hint.
        /// </summary>
        public static AnsiStringHint AnsiString
        {
            get { return AnsiStringHint.Instance; }
        }

        /// <summary>
        /// Gets the Enum Members Only hint.
        /// </summary>
        public static EnumMembersOnlyHint EnumMembersOnly
        {
            get { return EnumMembersOnlyHint.Instance; }
        }

        /// <summary>
        /// Gets the hint to not generate seconds part.
        /// </summary>
        public static NoSecondsHint NoSeconds
        {
            get { return NoSecondsHint.Instance; }
        }

        /// <summary>
        /// Gets the hint to not generate time part.
        /// </summary>
        public static NoTimeHint NoTime
        {
            get { return NoTimeHint.Instance; }
        }

        /// <summary>
        /// Gets the hint to not generate null values.
        /// </summary>
        public static NoNullsHint NoNulls
        {
            get { return NoNullsHint.Instance; }
        }

        /// <summary>
        /// Gets the hint to generate null values.
        /// </summary>
        public static AllNullsHint AllNulls
        {
            get { return AllNullsHint.Instance; }
        }

        /// <summary>
        /// Gets the hint to not generate random values
        /// </summary>
        public static NoRandomValuesHint NoRandomValues
        {
            get { return NoRandomValuesHint.Instance; }
        }

        /// <summary>
        /// Gets the unique data hint.
        /// </summary>
        public static AllUniqueHint AllUnique
        {
            get { return AllUniqueHint.Instance; }
        }

        /// <summary>
        /// Returns the hint for maximum length.
        /// </summary>
        /// <param name="value">The maximum length.</param>
        /// <returns>The hint for the maximum length.</returns>
        public static MaxLengthHint MaxLength(int value)
        {
            return new MaxLengthHint(value);
        }

        /// <summary>
        /// Returns the hint for minimum length.
        /// </summary>
        /// <param name="value">The minimum length.</param>
        /// <returns>The hint for the minimum length.</returns>
        public static MinLengthHint MinLength(int value)
        {
            return new MinLengthHint(value);
        }

        /// <summary>
        /// Returns the hint for maximum value.
        /// </summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="value">The maximum value.</param>
        /// <returns>The hint for the maximum value</returns>
        public static MaxValueHint<TValue> MaxValue<TValue>(TValue value) where TValue : struct, IComparable<TValue>
        {
            return new MaxValueHint<TValue>(value);
        }

        /// <summary>
        /// Returns the hint for minimum value.
        /// </summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="value">The minimum value.</param>
        /// <returns>The hint for the minimum value</returns>
        public static MinValueHint<TValue> MinValue<TValue>(TValue value) where TValue : struct, IComparable<TValue>
        {
            return new MinValueHint<TValue>(value);
        }

        /// <summary>
        /// Returns the hint for the limit of fractional digits.
        /// </summary>
        /// <param name="value">Number of fractional digits.</param>
        /// <returns>The hint for the limit of fractional digits.</returns>
        public static FractionalDigitsHint FractionalDigits(int value)
        {
            return new FractionalDigitsHint(value);
        }

        /// <summary>
        /// Returns the hint for the limit of fractional seconds.
        /// </summary>
        /// <param name="value">Number of fractional seconds.</param>
        /// <returns>The hint for the limit of fractional seconds.</returns>
        public static FractionalSecondsHint FractionalSeconds(int value)
        {
            return new FractionalSecondsHint(value);
        }

        /// <summary>
        /// Returns the hint for the numberic precision.
        /// </summary>
        /// <param name="value">The numeric precision.</param>
        /// <returns>The hint for the numberic precision.</returns>
        public static NumericPrecisionHint NumericPrecision(int value)
        {
            return new NumericPrecisionHint(value);
        }

        /// <summary>
        /// Returns the hint for the numberic scale.
        /// </summary>
        /// <param name="value">The numeric scale.</param>
        /// <returns>The hint for the numberic scale.</returns>
        public static NumericScaleHint NumericScale(int value)
        {
            return new NumericScaleHint(value);
        }

        /// <summary>
        /// Returns the hint for a collection maximum count.
        /// </summary>
        /// <param name="value">The maximum count.</param>
        /// <returns>The hint for a collection maximum count.</returns>
        public static CollectionMaxCountHint MaxCount(int value)
        {
            return new CollectionMaxCountHint(value);
        }

        /// <summary>
        /// Returns the hint for a collection minimum count.
        /// </summary>
        /// <param name="value">The minimum count.</param>
        /// <returns>The hint for a collection minimum count.</returns>
        public static CollectionMinCountHint MinCount(int value)
        {
            return new CollectionMinCountHint(value);
        }

        /// <summary>
        /// Returns the hint for the spatial reference number.
        /// </summary>
        /// <param name="value">the Spatial reference ID</param>
        /// <returns>the hint for the spatial reference number.</returns>
        public static SpatialReferenceNumberHint SpatialReferenceNumber(int value)
        {
            return new SpatialReferenceNumberHint(value);
        }

        /// <summary>
        /// Returns the hint for the spatial shape kind.
        /// </summary>
        /// <param name="value">the spatial shape kind.</param>
        /// <returns>the hint for spatial shape kind.</returns>
        public static SpatialShapeKindHint SpatialShapeKind(SpatialShapeKind value)
        {
            return new SpatialShapeKindHint(value);
        }

        /// <summary>
        /// Returns the hint for string prefix.
        /// </summary>
        /// <param name="prefix">The prefix of generated string.</param>
        /// <returns>The hint for string prefix.</returns>
        public static StringPrefixHint StringPrefixHint(string prefix)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(prefix, "prefix");
            return new StringPrefixHint(prefix);
        }

        /// <summary>
        /// Returns a hint for a specific interesting value which should be generated often.
        /// </summary>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <param name="value">The interesting value</param>
        /// <returns>The hint for the interesting value</returns>
        public static InterestingValueHint<TValue> InterestingValue<TValue>(TValue value)
        {
            return new InterestingValueHint<TValue>(value);
        }

        /// <summary>
        /// Converts specified data generation hint into Xml.
        /// </summary>
        /// <param name="hint">The data generation hint to convert.</param>
        /// <returns>Xml representation of the data generation hint.</returns>
        public static XElement ToXml(DataGenerationHint hint)
        {
            Type hintType = hint.GetType();

            string key = GetHintKey(hintType);

            if (hint is SingletonDataGenerationHint)
            {
                return new XElement(key);
            }

            PropertyInfo valuePropertyInfo = hintType.GetProperty("Value");

            ExceptionUtilities.CheckObjectNotNull(valuePropertyInfo, "Unsupported data generation hint type: '{0}'.", hintType.FullName);
            object value = valuePropertyInfo.GetValue(hint, null);

            return new XElement(key, new XAttribute(ValueAttributeName, value));
        }

        /// <summary>
        /// Converts from Xml to data generation hint.
        /// </summary>
        /// <param name="element">Xml representation of the data generation hint.</param>
        /// <returns>Data generation hint.</returns>
        public static DataGenerationHint FromXml(XElement element)
        {
            var valueAttribute = element.Attribute(ValueAttributeName);

            string key = element.Name.LocalName;

            Func<string, DataGenerationHint> hintCreator = null;
            if (!hintsCreators.TryGetValue(key, out hintCreator))
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Conversion to data generation hint from Xml failed. Element:\r\n{0}.", element.ToString()));
            }

            return hintCreator(valueAttribute != null ? valueAttribute.Value : null);
        }

        private static string GetHintKey<THint>() where THint : DataGenerationHint
        {
            return GetHintKey(typeof(THint));
        }

        private static string GetHintKey(Type hintType)
        {
            var genericArguments = hintType.GetGenericArguments();

            string key = hintType.Name;

            if (genericArguments.Length > 0)
            {
                ExceptionUtilities.Assert(genericArguments.Length == 1, "Unsupported data generation hint type: '{0}'.", hintType.FullName);
                key = key.Substring(0, key.IndexOf("`", StringComparison.Ordinal)) + "." + genericArguments[0].Name;
            }

            return key;
        }
    }
}
