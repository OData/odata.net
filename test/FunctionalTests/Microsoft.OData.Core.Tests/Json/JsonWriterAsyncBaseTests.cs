using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Verifies that different implementations of IJsonWriterAsync produce similar output for the same inputs
    /// </summary>
    public abstract class JsonWriterAsyncBaseTests
    {
        const string MixedObjectJson = "{\"StringProp\":\"John\",\"IntProp\":10,\"BoolPropFalse\":false,\"DateProp\":\"2014-12-31\",\"RawStringProp\":\"foobar\",\"BoolPropTrue\":false,\"RawArrayProp\":[1, 2, 3, 4, 5],\"DateTimeOffsetProp\":\"2014-12-31T12:42:30+01:20\",\"DateProp\":\"2014-12-31\",\"TimeSpanProp\":\"PT12H42M30S\",\"TimeOfDayProp\":\"12:42:30.1000000\",\"ObjectProp\":{\"FloatProp\":3.1,\"NestedRawValue\":\"test\",\"ShortProp\":1124,\"ByteProp\":10,\"LongProp\":234234,\"SignedByteProp\":-10,\"GuidProp\":\"00000012-0000-0000-0000-012345678900\",\"ArrayPropWithEveryOtherValueRaw\":[\"test\",\"raw\",10,\"raw\",true,\"raw\",\"2014-12-31\",\"raw\",\"2014-12-31T12:42:30+01:20\",\"raw\",[1,2,3],1124,\"raw\",10,\"raw\",-10,\"raw\",25253,\"raw\",\"00000012-0000-0000-0000-012345678900\",\"raw\",\"foo\",\"raw\",12.3,\"raw\",2.6,\"raw\",{},\"raw\",[\"rawAtArrayStartBeforeString\",\"test\"],[\"rawAtArrayStartBeforeBool\",false],[\"rawAtArrayStartBeforeByte\",10],[\"rawAtArrayStartBeforeSignedByte\",-10],[\"rawAtArrayStartBeforeShort\",10],[\"rawAtArrayStartBeforeInt\",10],[\"rawAtArrayStartBeforeLong\",10],[\"rawAtArrayStartBeforeDouble\",10.2],[\"rawAtArrayStartBeforeFloat\",10.2],[\"rawAtArrayStartBeforeDecimal\",10.2],[\"rawAtArrayStartBeforeGuid\",\"00000012-0000-0000-0000-012345678900\"],[\"rawAtArrayStartBeforeObject\",{}],[\"rawAtArrayStartBeforeDateTimeOffset\",\"2014-12-31T12:42:30+01:20\"],[\"rawAtArrayStartBeforeDate\",\"2014-12-31\"],[\"rawAtArrayStartBeforeTimeOfDay\",\"12:42:30.1000000\"],[\"rawAtArrayStartBeforeTimeSpan\",\"PT12H42M30S\"],[\"rawAtArrayStartBeforeArray\",[]],[\"rawAtArrayStartBeforeRaw\",\"raw\",\"test\",\"raw\"],\"raw\",\"raw\",\"raw\"]},\"ArrayProp\":[10,\"baz\",20,12.3,2.6,{\"RawObjectInArray\": true }],\"UntypedObjectProp\":{\"foo\":\"bar\"}}";
        protected abstract IJsonWriterAsync CreateJsonWriterAsync(Stream stream, bool isIeee754Compatible, Encoding encoding);

        [Fact]
        public async Task WritesMixedObjectWithRawValuesCorrectlyAsync()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriterAsync jsonWriter = CreateJsonWriterAsync(stream, false, Encoding.UTF8);
                await jsonWriter.StartObjectScopeAsync();

                await jsonWriter.WriteNameAsync("StringProp");
                await jsonWriter.WriteValueAsync("John");

                await jsonWriter.WriteNameAsync("IntProp");
                await jsonWriter.WriteValueAsync(10);

                await jsonWriter.WriteNameAsync("BoolPropFalse");
                await jsonWriter.WriteValueAsync(false);

                await jsonWriter.WriteNameAsync("DateProp");
                await jsonWriter.WriteValueAsync(new Date(2014, 12, 31));

                await jsonWriter.WriteNameAsync("RawStringProp");
                await jsonWriter.WriteRawValueAsync(@"""foobar""");

                await jsonWriter.WriteNameAsync("BoolPropTrue");
                await jsonWriter.WriteValueAsync(false);

                await jsonWriter.WriteNameAsync("RawArrayProp");
                await jsonWriter.WriteRawValueAsync("[1, 2, 3, 4, 5]");

                await jsonWriter.WriteNameAsync("DateTimeOffsetProp");
                await jsonWriter.WriteValueAsync(new DateTimeOffset(2014, 12, 31, 12, 42, 30, new TimeSpan(1, 20, 0)));

                await jsonWriter.WriteNameAsync("DateProp");
                await jsonWriter.WriteValueAsync(new Date(2014, 12, 31));

                await jsonWriter.WriteNameAsync("TimeSpanProp");
                await jsonWriter.WriteValueAsync(new TimeSpan(12, 42, 30));

                await jsonWriter.WriteNameAsync("TimeOfDayProp");
                await jsonWriter.WriteValueAsync(new TimeOfDay(12, 42, 30, 100));

                await jsonWriter.WriteNameAsync("ObjectProp");
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.WriteNameAsync("FloatProp");
                await jsonWriter.WriteValueAsync(3.1f);
                await jsonWriter.WriteNameAsync("NestedRawValue");
                await jsonWriter.WriteRawValueAsync("\"test\"");
                await jsonWriter.WriteNameAsync("ShortProp");
                await jsonWriter.WriteValueAsync((short)1124);
                await jsonWriter.WriteNameAsync("ByteProp");
                await jsonWriter.WriteValueAsync((byte)10);
                await jsonWriter.WriteNameAsync("LongProp");
                await jsonWriter.WriteValueAsync(234234L);
                await jsonWriter.WriteNameAsync("SignedByteProp");
                await jsonWriter.WriteValueAsync((sbyte)-10);
                await jsonWriter.WriteNameAsync("GuidProp");
                await jsonWriter.WriteValueAsync(new Guid("00000012-0000-0000-0000-012345678900"));
                await jsonWriter.WriteNameAsync("ArrayPropWithEveryOtherValueRaw");

                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteValueAsync(10);
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteValueAsync(true);
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteValueAsync(new Date(2014, 12, 31));
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteValueAsync(new DateTimeOffset(2014, 12, 31, 12, 42, 30, new TimeSpan(1, 20, 0)));
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteRawValueAsync("[1,2,3]");
                await jsonWriter.WriteValueAsync((short)1124);
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteValueAsync((byte)10);
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteValueAsync((sbyte)-10);
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteValueAsync(25253L);
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteValueAsync(new Guid("00000012-0000-0000-0000-012345678900"));
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteValueAsync("foo");
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteValueAsync(12.3m);
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteValueAsync(2.6f);
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.EndObjectScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeString""");
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeBool""");
                await jsonWriter.WriteValueAsync(false);
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeByte""");
                await jsonWriter.WriteValueAsync((byte)10);
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeSignedByte""");
                await jsonWriter.WriteValueAsync((sbyte)-10);
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeShort""");
                await jsonWriter.WriteValueAsync((short)10);
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeInt""");
                await jsonWriter.WriteValueAsync(10);
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeLong""");
                await jsonWriter.WriteValueAsync(10L);
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeDouble""");
                await jsonWriter.WriteValueAsync(10.2);
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeFloat""");
                await jsonWriter.WriteValueAsync(10.2f);
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeDecimal""");
                await jsonWriter.WriteValueAsync(10.2m);
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeGuid""");
                await jsonWriter.WriteValueAsync(new Guid("00000012-0000-0000-0000-012345678900"));
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeObject""");
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.EndObjectScopeAsync();
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeDateTimeOffset""");
                await jsonWriter.WriteValueAsync(new DateTimeOffset(2014, 12, 31, 12, 42, 30, new TimeSpan(1, 20, 0)));
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeDate""");
                await jsonWriter.WriteValueAsync(new Date(2014, 12, 31));
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeTimeOfDay""");
                await jsonWriter.WriteValueAsync(new TimeOfDay(12, 42, 30, 100));
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeTimeSpan""");
                await jsonWriter.WriteValueAsync(new TimeSpan(12, 42, 30));
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeArray""");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeRaw""");
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.EndObjectScopeAsync();

                await jsonWriter.WriteNameAsync("ArrayProp");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(10);
                await jsonWriter.WriteValueAsync("baz");
                await jsonWriter.WriteRawValueAsync("20");
                await jsonWriter.WriteValueAsync(12.3m);
                await jsonWriter.WriteValueAsync(2.6f);
                await jsonWriter.WriteRawValueAsync(@"{""RawObjectInArray"": true }");
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("UntypedObjectProp");
                await jsonWriter.WriteRawValueAsync(@"{""foo"":""bar""}");

                await jsonWriter.EndObjectScopeAsync();

                await jsonWriter.FlushAsync();
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
