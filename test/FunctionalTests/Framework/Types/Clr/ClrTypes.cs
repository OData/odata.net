//---------------------------------------------------------------------
// <copyright file="ClrTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Globalization;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.Test.ModuleCore;
using System.Linq;

namespace System.Data.Test.Astoria
{
    public abstract class IntegerType<T> : PrimitiveType<T>
    {
        protected abstract long MinValue
        {
            get;
        }

        protected abstract long MaxValue
        {
            get;
        }

        protected abstract long Wrap(T val);
        protected abstract T UnWrap(long val);

        protected virtual long CreateRandomValue(long min, long max)
        {
            int tries = 5;
            while (tries-- >= 0)
            {
                double range = Math.Abs((double)max - (double)min);
                double multiplier = AstoriaTestProperties.Random.NextDouble();
                double offset = range * multiplier;
                double total = offset + min;
                long value = Convert.ToInt64(total);
                if (min <= value && value <= max)
                    return value;
            }
            throw new ArgumentException(String.Format("Could not generate integer value between {0} and {1}", min, max));
        }

        public override NodeValue CreateRandomValueForFacets(NodeFacets propertyFacets)
        {
            long max = MaxValue;
            long min = MinValue;
            
            T temp;
            if (propertyFacets.TryGetMaxValue(out temp) && Wrap(temp) < max)
                max = Wrap(temp);
            if (propertyFacets.TryGetMinValue(out temp) && Wrap(temp) > min)
                min = Wrap(temp);

            long value = CreateRandomValue(min, max);

            return new NodeValue(UnWrap(value), this);
        }

        public override NodeValue CreateRandomValue()
        {
            long value = CreateRandomValue(MinValue, MaxValue);
            return new NodeValue(UnWrap(value), this);   
        }
    }

    ////////////////////////////////////////////////////////
    // ClrInt32
    //
    ////////////////////////////////////////////////////////   
    public class ClrInt32 : IntegerType<int>
    {
        protected override long MaxValue
        {
            get { return int.MaxValue; }
        }

        protected override long MinValue
        {
            get { return int.MinValue; }
        }

        protected override long Wrap(int val)
        {
            return (long)val;
        }

        protected override int UnWrap(long val)
        {
            return (int)val;
        }
    }

    ////////////////////////////////////////////////////////
    // ClrInt32
    //
    ////////////////////////////////////////////////////////   
    public class ClrInt64 : IntegerType<long>
    {
        protected override long MaxValue
        {
            get { return long.MaxValue; }
        }

        protected override long MinValue
        {
            get { return long.MinValue; }
        }

        protected override long Wrap(long val)
        {
            return (long)val;
        }

        protected override long UnWrap(long val)
        {
            return (long)val;
        }
    }

    ////////////////////////////////////////////////////////
    // ClrInt16
    //
    ////////////////////////////////////////////////////////   
    public class ClrInt16 : IntegerType<short>
    {
        protected override long MaxValue
        {
            get { return short.MaxValue; }
        }

        protected override long MinValue
        {
            get { return short.MinValue; }
        }

        protected override long Wrap(short val)
        {
            return (long)val;
        }

        protected override short UnWrap(long val)
        {
            return (short)val;
        }
    }

    ////////////////////////////////////////////////////////
    // ClrString
    //
    ////////////////////////////////////////////////////////   
    public class ClrString : PrimitiveType<string>
    {
        private char[] buffer = new char[1024];

        public ClrString()
            : base()
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                //Changing range to just numeric and alphabetic chars
                //Very difficult to encode things properly so not producing them for now
                char singleChar = char.MinValue;
                do
                {
                    singleChar = (char)AstoriaTestProperties.Random.Next(48, 126);
                }
                while (!char.IsLetterOrDigit(singleChar));
                buffer[i] = singleChar;
            }
        }
        //Data

        //Constructor

        //Accessors
        public override NodeValue CreateRandomValueForFacets(NodeFacets propertyFacets)
        {
            bool isFixedLength = propertyFacets.FixedLength;

            if (propertyFacets.MaxSize != null)
                return CreateRandomValue((int)propertyFacets.MaxSize.Value, isFixedLength);
            
            return CreateRandomValue();
        }

        public NodeValue CreateRandomValue(int maxLength, bool isFixedLength)
        {
            int stringLength = maxLength;
            if (!isFixedLength)
                stringLength = AstoriaTestProperties.Random.Next(1, maxLength);
            
            if (stringLength > 1000)
                stringLength = 1000;

            StringBuilder newString = new StringBuilder(stringLength);
            while (newString.Length < stringLength)
            {
                int offset = AstoriaTestProperties.Random.Next(buffer.Length);
                int length = AstoriaTestProperties.Random.Next(1, 10);
                if (offset + length >= buffer.Length)
                    length = buffer.Length - offset - 1;
                if (length + newString.Length > stringLength)
                    length = stringLength - newString.Length;

                newString.Append(buffer, offset, length);
            }

            return new NodeValue(newString.ToString(), this);
        }

        public override NodeValue CreateRandomValue()
        {
            return CreateRandomValue(50, false);
        }
    }

    ////////////////////////////////////////////////////////
    // ClrBoolean
    //
    ////////////////////////////////////////////////////////   
    public class ClrBoolean : PrimitiveType<bool>
    {
        //Data

        //Constructor

        //Accessors
    }

    ////////////////////////////////////////////////////////
    // ClrBoolean
    //
    ////////////////////////////////////////////////////////   
    public class ClrDateTime : PrimitiveType<DateTime>
    {
        //Data

        protected virtual DateTime MaxValue
        {
            get
            {
                return DateTime.MaxValue;
            }
        }

        protected virtual DateTime MinValue
        {
            get
            {
                return DateTime.MinValue;
            }
        }

        protected virtual long TicksOfPrecision
        {
            get
            {
                return TimeSpan.TicksPerMillisecond; //json can only do down to milliseconds
            }
        }

        public override NodeValue CreateValue()
        {
            return CreateRandomValue(this.MinValue, this.MaxValue);
        }

        public override NodeValue CreateRandomValueForFacets(NodeFacets propertyFacets)
        {
            DateTime minValue = this.MinValue;
            DateTime maxValue = this.MaxValue;

            DateTime temp;
            if (propertyFacets.TryGetMaxValue(out temp) && temp < maxValue)
                maxValue = temp;
            if (propertyFacets.TryGetMinValue(out temp) && temp > minValue)
                minValue = temp;

            return CreateRandomValue(minValue, maxValue);
        }

        protected virtual NodeValue CreateRandomValue(DateTime minValue, DateTime maxValue)
        {
            return new NodeValue(CreateRandomDateTime(minValue, maxValue), this);
        }

        protected DateTime CreateRandomDateTime(DateTime minValue, DateTime maxValue)
        {
            // Never generate things outside the current culture
            // TODO: what about the server's culture?
            Calendar calendar = CultureInfo.CurrentCulture.DateTimeFormat.Calendar;
            DateTime cultureMax = calendar.MaxSupportedDateTime;
            DateTime cultureMin = calendar.MinSupportedDateTime;

            if (cultureMax < maxValue)
                maxValue = cultureMax;
            if (cultureMin > minValue)
                minValue = cultureMin;

            TimeSpan timespan = maxValue.Subtract(minValue);
            long ticks = Convert.ToInt64(timespan.Ticks * AstoriaTestProperties.Random.NextDouble());

            ticks -= ticks % this.TicksOfPrecision;
            
            DateTime randomDateTime = minValue.AddTicks(ticks);
            return randomDateTime;
        }
    }

    public class ClrNullableDateTime : ClrDateTime
    {
        protected override NodeValue CreateRandomValue(DateTime minValue, DateTime maxValue)
        {
            Nullable<DateTime> randomDateTime = CreateRandomDateTime(minValue, maxValue);
            return new NodeValue(randomDateTime, this);
        }
    }
    ////////////////////////////////////////////////////////
    // ClrDecimal
    //
    ////////////////////////////////////////////////////////   
    public class ClrDecimal : PrimitiveType<Decimal>
    {
        //Data

        //Constructor

        //Accessors
        public override NodeValue CreateRandomValueForFacets(NodeFacets propertyFacets)
        {
            if (propertyFacets.Precision != null && propertyFacets.Scale != null)
            {
                //ISSUE: Small money has a range - 214,748.3648 through +214,748.3647
                // In order to always make this work, reducing the Pre by 1 does this

                decimal? max = null;
                decimal? min = null;
                decimal temp;
                if (propertyFacets.TryGetMaxValue(out temp))
                    max = temp;
                if (propertyFacets.TryGetMinValue(out temp))
                    min = temp;

                int scale = propertyFacets.Scale.Value;
                int precision = propertyFacets.Precision.Value;
                if (precision > 28)
                    precision = 28; //can't exceed CLR limit

                Decimal[] specialValues = new Decimal[] { Decimal.MinValue, Decimal.MinusOne, Decimal.Zero, Decimal.One, Decimal.MaxValue };
                Decimal? value = null;
                int random = AstoriaTestProperties.Random.Next(50 + specialValues.Length);
                if (random < specialValues.Length)
                {
                    value = specialValues[random];
                    string asString = Math.Abs(value.Value).ToString();

                    // is there enough precision?
                    if (asString.Length >= precision)
                        value = null;

                    // is there enough scale?
                    if (asString.Length - asString.IndexOf('.') >= scale)
                        value = null;

                    // is it too big?
                    if (max.HasValue && value > max)
                        value = null;

                    // is it too small?
                    if (min.HasValue && value < min)
                        value = null;
                }

                if (value == null)
                {
                    bool negative = (random % 2 == 0);
                    if (min.HasValue && min >= decimal.Zero)
                        negative = false;

                    int fractionLength = AstoriaTestProperties.Random.Next(scale);
                    int integerLength = AstoriaTestProperties.Random.Next(precision - scale); //assume that it will be padded to the maximum scale

                    if (max.HasValue && max <= decimal.One && !negative)
                        integerLength = 0;

                    StringBuilder builder = new StringBuilder();
                    if (negative)
                        builder.Append('-');

                    if (integerLength == 0)
                        builder.Append('0');
                    else
                    {
                        for (int i = 0; i < integerLength; i++)
                            builder.Append(AstoriaTestProperties.Random.Next(10));
                    }

                    if (fractionLength > 0)
                    {
                        builder.Append('.');
                        for (int i = 0; i < fractionLength; i++)
                            builder.Append(AstoriaTestProperties.Random.Next(10));
                    }

                    string asString = builder.ToString();

                    SqlDecimal sqlValue = SqlDecimal.Parse(builder.ToString());
                    value = sqlValue.Value;

                    try
                    {
                        SqlDecimal.ConvertToPrecScale(sqlValue, precision, scale); //in case the DB wants it to use the whole scale
                    }
                    catch(Exception e)
                    {
                        throw new TestFailedException(String.Format("Generated decimal value '{0}' exceeds precision/scale specified", asString), null, null, e);
                    }
                }

                return new NodeValue(value.Value, this);
            }
            else
                return this.CreateRandomValue();
        }
    }

    public abstract class FloatingPointType<T> : PrimitiveType<T>
    {
        protected abstract T MinValue
        {
            get;
        }
        protected abstract T Range
        {
            get;
        }

        protected abstract T[] SpecialValues
        {
            get;
        }

        protected abstract T Cast(double d);
        protected abstract int Precision
        {
            get;
        }

        public override bool IsApproxPrecision
        {
            get { return true; }
        }

        //protected virtual int GetPrecision(T val)
        //{
        //    double d = Math.Abs(Convert.ToDouble(val));
        //    string asString = d.ToString("E25", System.Globalization.CultureInfo.InvariantCulture);

        //    asString = asString.Substring(0, asString.IndexOf("E"));
        //    asString = asString.Trim('0');

        //    int digits = asString.Length - 1; // ignore decimal point
        //    return digits;
        //}

        public override NodeValue CreateRandomValueForFacets(NodeFacets propertyFacets)
        {
            T f = MinValue;
            try
            {
                int r = AstoriaTestProperties.Random.Next(50);
                if (r < SpecialValues.Length)
                {
                    f = SpecialValues[r];
                }
                else
                {
                    double d = Convert.ToDouble(Range);
                    d *= AstoriaTestProperties.Random.NextDouble();
                    d += Convert.ToDouble(MinValue);

                    // 1/4 of the time, generate one that doesn't have a fractional component
                    //
                    if (AstoriaTestProperties.Random.Next(4) == 0)
                        d = (Single)Math.Floor(d);

                    string format = String.Format("G{0}", Precision);
                    string asString = d.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
                    d = double.Parse(asString);
                    f = Cast(d);
                }
            }
            catch (Exception)
            { }
            return new NodeValue(f, this);
        }

        protected virtual bool IsApproxValueComparable(T val)
        {
            double original = Convert.ToDouble(val, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            // attempt to round trip the thing
            double reduced = double.Parse(
                original.ToString("G" + Precision, System.Globalization.CultureInfo.InvariantCulture),
                System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            string format = "G" + (Precision + 1);
            return reduced.ToString(format, System.Globalization.CultureInfo.InvariantCulture)
                .Equals(original.ToString(format, System.Globalization.CultureInfo.InvariantCulture));
        }

        public override bool IsApproxValueComparable(NodeValue nodeValue)
        {
            if (nodeValue.ClrValue is T)
                return IsApproxValueComparable((T)nodeValue.ClrValue);
            else
                throw new ArgumentException("Invalid type used");
        }
    }

    ////////////////////////////////////////////////////////
    // ClrFloat
    //
    ////////////////////////////////////////////////////////   
    public class ClrFloat : FloatingPointType<float>
    {
        // TODO: use a more type specific range
        private static float val = 1000000000000f;
        private static float minValue = -val;
        private static float range = val * 2;
        protected override float MinValue
        {
            get { return minValue; }
        }

        protected override float Range
        {
            get { return range; }
        }

        protected override float[] SpecialValues
        {
            get 
            { 
                if(Versioning.Server.SupportsV2Features)
                    return new float[] { float.NegativeInfinity, float.PositiveInfinity, float.MaxValue, float.MinValue, float.Epsilon }; 
                else //V1 had an issue with parsing -INFf
                    return new float[] { float.MaxValue, float.MinValue, float.Epsilon }; 
            }
        }

        protected override float Cast(double d)
        {
            return (float)d;
        }

        protected override int Precision
        {
            get { return 7; }
        }
    }

    ////////////////////////////////////////////////////////
    // ClrDouble
    //
    ////////////////////////////////////////////////////////   
    public class ClrDouble : FloatingPointType<Double>
    {
        // TODO: use a more type specific range
        private static Double val = 1000000000000.0;
        private static Double minValue = -val;
        private static Double range = val * 2;
        protected override Double MinValue
        {
            get { return minValue; }
        }

        protected override Double Range
        {
            get { return range; }
        }

        protected override double[] SpecialValues
        {
            get { return new double[] { double.NegativeInfinity, double.PositiveInfinity, double.MaxValue, double.MinValue, double.Epsilon }; }
        }

        protected override Double Cast(double d)
        {
            return (Double)d;
        }

        protected override int Precision
        {
            get { return 15; }
        }
    }

    ////////////////////////////////////////////////////////
    // ClrSingle
    //
    ////////////////////////////////////////////////////////   
    public class ClrSingle : FloatingPointType<Single>
    {
        // TODO: use a more type specific range
        private static Single val = 1000000000000f;
        private static Single minValue = -val;
        private static Single range = val * 2;
        protected override Single MinValue
        {
            get { return minValue; }
        }

        protected override Single Range
        {
            get { return range; }
        }

        protected override Single[] SpecialValues
        {
            get { return new float[] { Single.NegativeInfinity, Single.PositiveInfinity, Single.MaxValue, Single.MinValue, Single.Epsilon }; }
        }

        protected override Single Cast(double d)
        {
            return (Single)d;
        }

        protected override int Precision
        {
            get { return 7; }
        }
    }
    ////////////////////////////////////////////////////////
    // ClrSByte
    //
    ////////////////////////////////////////////////////////   
    public class ClrSByte : PrimitiveType<sbyte>
    {
        //Data

        //Constructor

        //Accessors
        public override NodeValue CreateRandomValue()
        {
            return new NodeValue((sbyte)AstoriaTestProperties.Random.Next(sbyte.MinValue, sbyte.MaxValue), this);
        }
    }

    ////////////////////////////////////////////////////////
    // ClrByte
    //
    ////////////////////////////////////////////////////////   
    public class ClrByte : PrimitiveType<byte>
    {
        //Data

        //Constructor

        //Accessors
        public override NodeValue CreateRandomValue()
        {
            return new NodeValue((byte)AstoriaTestProperties.Random.Next(byte.MinValue, byte.MaxValue), this);
        }
    }

    ////////////////////////////////////////////////////////
    // ClrBinary
    //
    ////////////////////////////////////////////////////////   
    public class ClrBinary : PrimitiveType<byte[]>
    {
        //Data

        //Constructor

        //Accessors
        public override NodeValue CreateRandomValueForFacets(NodeFacets propertyFacets)
        {
            int maxSize = propertyFacets.MaxSize != null ? propertyFacets.MaxSize.Value : 512;

            return CreateRandomValue(maxSize, propertyFacets.FixedLength);
        }
        public NodeValue CreateRandomValue(int maxSize, bool isFixedLength)
        {
            int actualSize = maxSize;
            if (!isFixedLength)
            {
                //Set max size to be random
                actualSize = AstoriaTestProperties.Random.Next(maxSize);
                if (actualSize > 1000)
                    actualSize = 1000;
            }

            byte[] bytes = new byte[actualSize];
            AstoriaTestProperties.Random.NextBytes(bytes);
            
            return new NodeValue(bytes, this);
        }
    }
    ////////////////////////////////////////////////////////
    // ClrGuid
    //
    ////////////////////////////////////////////////////////   
    public class ClrGuid : PrimitiveType<Guid>
    {
        //Data
        public override NodeValue CreateRandomValue()
        {
            return new NodeValue(Guid.NewGuid(), this);
        }

        //Constructor

        //Accessors
    }
}
