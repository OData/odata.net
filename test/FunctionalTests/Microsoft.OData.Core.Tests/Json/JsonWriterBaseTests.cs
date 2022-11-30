using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Verifies that different implementations of IJsonWriter produce similar output for the same inputs
    /// </summary>
    public abstract class JsonWriterBaseTests
    {
        const string MixedObjectJson = "{\"StringProp\":\"John\",\"IntProp\":10,\"BoolPropFalse\":false,\"DateProp\":\"2014-12-31\",\"RawStringProp\":\"foobar\",\"BoolPropTrue\":false,\"RawArrayProp\":[1, 2, 3, 4, 5],\"DateTimeOffsetProp\":\"2014-12-31T12:42:30+01:20\",\"DateProp\":\"2014-12-31\",\"TimeSpanProp\":\"PT12H42M30S\",\"TimeOfDayProp\":\"12:42:30.1000000\",\"ObjectProp\":{\"FloatProp\":3.1,\"NestedRawValue\":\"test\",\"ShortProp\":1124,\"ByteProp\":10,\"LongProp\":234234,\"SignedByteProp\":-10,\"GuidProp\":\"00000012-0000-0000-0000-012345678900\",\"ArrayPropWithEveryOtherValueRaw\":[\"test\",\"raw\",10,\"raw\",true,\"raw\",\"2014-12-31\",\"raw\",\"2014-12-31T12:42:30+01:20\",\"raw\",[1,2,3],1124,\"raw\",10,\"raw\",-10,\"raw\",25253,\"raw\",\"00000012-0000-0000-0000-012345678900\",\"raw\",\"foo\",\"raw\",12.3,\"raw\",2.6,\"raw\",{},\"raw\",[\"rawAtArrayStartBeforeString\",\"test\"],[\"rawAtArrayStartBeforeBool\",false],[\"rawAtArrayStartBeforeByte\",10],[\"rawAtArrayStartBeforeSignedByte\",-10],[\"rawAtArrayStartBeforeShort\",10],[\"rawAtArrayStartBeforeInt\",10],[\"rawAtArrayStartBeforeLong\",10],[\"rawAtArrayStartBeforeDouble\",10.2],[\"rawAtArrayStartBeforeFloat\",10.2],[\"rawAtArrayStartBeforeDecimal\",10.2],[\"rawAtArrayStartBeforeGuid\",\"00000012-0000-0000-0000-012345678900\"],[\"rawAtArrayStartBeforeObject\",{}],[\"rawAtArrayStartBeforeDateTimeOffset\",\"2014-12-31T12:42:30+01:20\"],[\"rawAtArrayStartBeforeDate\",\"2014-12-31\"],[\"rawAtArrayStartBeforeTimeOfDay\",\"12:42:30.1000000\"],[\"rawAtArrayStartBeforeTimeSpan\",\"PT12H42M30S\"],[\"rawAtArrayStartBeforeArray\",[]],[\"rawAtArrayStartBeforeRaw\",\"raw\",\"test\",\"raw\"],\"raw\",\"raw\",\"raw\"]},\"ArrayProp\":[10,\"baz\",20,12.3,2.6,{\"RawObjectInArray\": true }],\"UntypedObjectProp\":{\"foo\":\"bar\"}}";
        protected abstract IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding);

        [Fact]
        public void WritesMixedObjectWithRawValuesCorrectly()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);
                jsonWriter.StartObjectScope();

                jsonWriter.WriteName("StringProp");
                jsonWriter.WriteValue("John");

                jsonWriter.WriteName("IntProp");
                jsonWriter.WriteValue(10);

                jsonWriter.WriteName("BoolPropFalse");
                jsonWriter.WriteValue(false);

                jsonWriter.WriteName("DateProp");
                jsonWriter.WriteValue(new Date(2014, 12, 31));

                jsonWriter.WriteName("RawStringProp");
                jsonWriter.WriteRawValue(@"""foobar""");

                jsonWriter.WriteName("BoolPropTrue");
                jsonWriter.WriteValue(false);

                jsonWriter.WriteName("RawArrayProp");
                jsonWriter.WriteRawValue("[1, 2, 3, 4, 5]");

                jsonWriter.WriteName("DateTimeOffsetProp");
                jsonWriter.WriteValue(new DateTimeOffset(2014, 12, 31, 12, 42, 30, new TimeSpan(1, 20, 0)));

                jsonWriter.WriteName("DateProp");
                jsonWriter.WriteValue(new Date(2014, 12, 31));

                jsonWriter.WriteName("TimeSpanProp");
                jsonWriter.WriteValue(new TimeSpan(12, 42, 30));

                jsonWriter.WriteName("TimeOfDayProp");
                jsonWriter.WriteValue(new TimeOfDay(12, 42, 30, 100));

                jsonWriter.WriteName("ObjectProp");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("FloatProp");
                jsonWriter.WriteValue(3.1f);
                jsonWriter.WriteName("NestedRawValue");
                jsonWriter.WriteRawValue("\"test\"");
                jsonWriter.WriteName("ShortProp");
                jsonWriter.WriteValue((short)1124);
                jsonWriter.WriteName("ByteProp");
                jsonWriter.WriteValue((byte)10);
                jsonWriter.WriteName("LongProp");
                jsonWriter.WriteValue(234234L);
                jsonWriter.WriteName("SignedByteProp");
                jsonWriter.WriteValue((sbyte)-10);
                jsonWriter.WriteName("GuidProp");
                jsonWriter.WriteValue(new Guid("00000012-0000-0000-0000-012345678900"));
                jsonWriter.WriteName("ArrayPropWithEveryOtherValueRaw");

                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue("test");
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteValue(10);
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteValue(true);
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteValue(new Date(2014, 12, 31));
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteValue(new DateTimeOffset(2014, 12, 31, 12, 42, 30, new TimeSpan(1, 20, 0)));
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteRawValue("[1,2,3]");
                jsonWriter.WriteValue((short)1124);
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteValue((byte)10);
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteValue((sbyte)-10);
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteValue(25253L);
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteValue(new Guid("00000012-0000-0000-0000-012345678900"));
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteValue("foo");
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteValue(12.3m);
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteValue(2.6f);
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.StartObjectScope();
                jsonWriter.EndObjectScope();
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeString""");
                jsonWriter.WriteValue("test");
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeBool""");
                jsonWriter.WriteValue(false);
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeByte""");
                jsonWriter.WriteValue((byte)10);
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeSignedByte""");
                jsonWriter.WriteValue((sbyte)-10);
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeShort""");
                jsonWriter.WriteValue((short)10);
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeInt""");
                jsonWriter.WriteValue(10);
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeLong""");
                jsonWriter.WriteValue(10L);
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeDouble""");
                jsonWriter.WriteValue(10.2);
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeFloat""");
                jsonWriter.WriteValue(10.2f);
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeDecimal""");
                jsonWriter.WriteValue(10.2m);
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeGuid""");
                jsonWriter.WriteValue(new Guid("00000012-0000-0000-0000-012345678900"));
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeObject""");
                jsonWriter.StartObjectScope();
                jsonWriter.EndObjectScope();
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeDateTimeOffset""");
                jsonWriter.WriteValue(new DateTimeOffset(2014, 12, 31, 12, 42, 30, new TimeSpan(1, 20, 0)));
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeDate""");
                jsonWriter.WriteValue(new Date(2014, 12, 31));
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeTimeOfDay""");
                jsonWriter.WriteValue(new TimeOfDay(12, 42, 30,100));
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeTimeSpan""");
                jsonWriter.WriteValue(new TimeSpan(12, 42, 30));
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeArray""");
                jsonWriter.StartArrayScope();
                jsonWriter.EndArrayScope();
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeRaw""");
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteValue("test");
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.EndArrayScope();
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.EndArrayScope();

                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("ArrayProp");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(10);
                jsonWriter.WriteValue("baz");
                jsonWriter.WriteRawValue("20");
                jsonWriter.WriteValue(12.3m);
                jsonWriter.WriteValue(2.6f);
                jsonWriter.WriteRawValue(@"{""RawObjectInArray"": true }");
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("UntypedObjectProp");
                jsonWriter.WriteRawValue(@"{""foo"":""bar""}");

                jsonWriter.EndObjectScope();

                jsonWriter.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = reader.ReadToEnd();
                    string normalizedOutput = NormalizeJsonText(rawOutput);
                    string normalizedExpectedOutput = NormalizeJsonText(MixedObjectJson);
                    Assert.Equal(normalizedExpectedOutput, normalizedOutput);
                }
            }
        }


        /// <summary>
        /// Normalizes the differences between JSON text encoded
        /// by Utf8JsonWriter and OData's JsonWriter, to make
        /// it possible to compare equivalent outputs.
        /// </summary>
        /// <param name="source">Original JSON text.</param>
        /// <returns>Normalized JSON text.</returns>
        private string NormalizeJsonText(string source)
        {
            return source
                // Utf8JsonWriter uses uppercase letters when encoding unicode characters e.g. \uDC05
                // OData's JsonWriter uses lowercase letters: \udc05
                .ToLowerInvariant()
                // Utf8JsonWrites escapes double-quotes using \u0022
                // OData JsonWrtier uses \"
                .Replace(@"\u0022", @"\""");
        }
    }
}
