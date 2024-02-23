using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;
#if NETCOREAPP
using System.Text.Json;
#endif

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Verifies that different implementations of IJsonWriter produce similar output for the same inputs
    /// </summary>
    public abstract class JsonWriterAsyncBaseTests
    {
        const string MixedObjectJson = "{\"StringProp\":\"John\",\"IntProp\":10,\"BoolPropFalse\":false,\"DateProp\":\"2014-12-31\",\"RawStringProp\":\"foobar\",\"BoolPropTrue\":false,\"RawArrayProp\":[1,2,3,4,5],\"DateTimeOffsetProp\":\"2014-12-31T12:42:30+01:20\",\"DateProp\":\"2014-12-31\",\"TimeSpanProp\":\"PT12H42M30S\",\"TimeOfDayProp\":\"12:42:30.1000000\",\"ObjectProp\":{\"FloatProp\":3.1,\"NestedRawValue\":\"test\",\"ShortProp\":1124,\"ByteProp\":10,\"LongProp\":234234,\"SignedByteProp\":-10,\"GuidProp\":\"00000012-0000-0000-0000-012345678900\",\"ArrayPropWithEveryOtherValueRaw\":[\"test\",\"raw\",10,\"raw\",true,\"raw\",\"2014-12-31\",\"raw\",\"2014-12-31T12:42:30+01:20\",\"raw\",[1,2,3],1124,\"raw\",10,\"raw\",-10,\"raw\",25253,\"raw\",\"00000012-0000-0000-0000-012345678900\",\"raw\",\"foo\",\"raw\",12.3,\"raw\",2.6,\"raw\",{},\"raw\",[\"rawAtArrayStartBeforeString\",\"test\"],[\"rawAtArrayStartBeforeBool\",false],[\"rawAtArrayStartBeforeByte\",10],[\"rawAtArrayStartBeforeSignedByte\",-10],[\"rawAtArrayStartBeforeShort\",10],[\"rawAtArrayStartBeforeInt\",10],[\"rawAtArrayStartBeforeLong\",10],[\"rawAtArrayStartBeforeDouble\",10.2],[\"rawAtArrayStartBeforeFloat\",10.2],[\"rawAtArrayStartBeforeDecimal\",10.2],[\"rawAtArrayStartBeforeGuid\",\"00000012-0000-0000-0000-012345678900\"],[\"rawAtArrayStartBeforeObject\",{}],[\"rawAtArrayStartBeforeDateTimeOffset\",\"2014-12-31T12:42:30+01:20\"],[\"rawAtArrayStartBeforeDate\",\"2014-12-31\"],[\"rawAtArrayStartBeforeTimeOfDay\",\"12:42:30.1000000\"],[\"rawAtArrayStartBeforeTimeSpan\",\"PT12H42M30S\"],[\"rawAtArrayStartBeforeArray\",[]],[\"rawAtArrayStartBeforeNull\",null],[\"rawAtArrayStartBeforeByteArray\",\"TWFu\"],[\"rawAtArrayStartBeforeRaw\",\"raw\",\"test\",\"raw\"],\"raw\",\"raw\",\"raw\"]},\"ArrayProp\":[10,\"baz\",20,12.3,2.6,{\"RawObjectInArray\":true}],\"UntypedObjectProp\":{\"foo\":\"bar\"}}";
        const string SampleJsonInput = "{\"jsonInput\":{\"foo\":\"bar\"}}";
        const string MixedJsonInputAndRawValue = "{\"StringProp\":\"John\",\"JsonInputProp\":{\"jsonInput\":{\"foo\":\"bar\"}},\"RawStringProp\":\"foobar\",\"JsonInputAfterRawValue\":{\"jsonInput\":{\"foo\":\"bar\"}},\"ArrayProp\":[\"raw\",{\"jsonInput\":{\"foo\":\"bar\"}},\"foobar\",{\"jsonInput\":{\"foo\":\"bar\"}},\"raw\"]}";

        protected abstract IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding);

        [Fact]
        public async Task WritesMixedObjectWithRawValuesCorrectlyAsync()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);
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
                await jsonWriter.WriteRawValueAsync("[1,2,3,4,5]");

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
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeNull""");
                await jsonWriter.WriteValueAsync((string)null);
                await jsonWriter.EndArrayScopeAsync();
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""rawAtArrayStartBeforeByteArray""");
                await jsonWriter.WriteValueAsync(new byte[] { 77, 97, 110 });
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
                await jsonWriter.WriteRawValueAsync(@"{""RawObjectInArray"":true}");
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

#if NETCOREAPP
        [Fact]
        public async Task WritesJsonElementCorrectly()
        {
            using (JsonDocument jsonDoc = JsonDocument.Parse(MixedObjectJson))
            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);
                await jsonWriter.WriteValueAsync(jsonDoc.RootElement);

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

        [Fact]
        public async Task WriteObjectWithJsonInputAndRawValuesCorrectly()
        {
            using (JsonDocument jsonInput = JsonDocument.Parse(SampleJsonInput))
            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);
                await jsonWriter.StartObjectScopeAsync();

                await jsonWriter.WriteNameAsync("StringProp");
                await jsonWriter.WriteValueAsync("John");
                await jsonWriter.WriteNameAsync("JsonInputProp");
                
                await jsonWriter.WriteValueAsync (jsonInput.RootElement);
                await jsonWriter.WriteNameAsync("RawStringProp");
                await jsonWriter.WriteRawValueAsync(@"""foobar""");
                await jsonWriter.WriteNameAsync("JsonInputAfterRawValue");
                await jsonWriter.WriteValueAsync(jsonInput.RootElement);

                await jsonWriter.WriteNameAsync("ArrayProp");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.WriteValueAsync(jsonInput.RootElement);
                await jsonWriter.WriteValueAsync("foobar");
                await jsonWriter.WriteValueAsync(jsonInput.RootElement);
                await jsonWriter.WriteRawValueAsync(@"""raw""");
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.EndObjectScopeAsync();

                await jsonWriter.FlushAsync();
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = reader.ReadToEnd();
                    string normalizedOutput = NormalizeJsonText(rawOutput);
                    string normalizedExpectedOutput = NormalizeJsonText(MixedJsonInputAndRawValue);
                    Assert.Equal(normalizedExpectedOutput, normalizedOutput);
                }
            }
        }
#endif

        [Fact]
        public async Task WritesLargeByteArraysCorrectly()
        {
            byte[] input = GenerateByteArray(1024 * 1024);

            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);

                await jsonWriter.WriteValueAsync(input);
                await jsonWriter.FlushAsync();
                stream.Seek(0, SeekOrigin.Begin);

                string expected = Convert.ToBase64String(input);

                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = await reader.ReadToEndAsync();
                    Assert.Equal($"\"{expected}\"", rawOutput);
                }
            }
        }

        [Fact]
        public async Task WritesSimpleLargeStringsCorrectly()
        {
            int inputLength = 1024 * 1024; // 1MB
            string input = new string('a', inputLength);

            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);

                await jsonWriter.WriteValueAsync(input);
                await jsonWriter.FlushAsync();
                stream.Seek(0, SeekOrigin.Begin);

                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = await reader.ReadToEndAsync();
                    Assert.Equal($"\"{input}\"", rawOutput);
                }
            }
        }

        [Fact]
        public async Task WriteMixedJsonWithLargeValues()
        {
            byte[] largeBinaryData = GenerateByteArray(256 * 1024);
            string largeBase64Data = Convert.ToBase64String(largeBinaryData);

            string largeStringData = new string('a', 256 * 1024);

            string expectedJson =
                $"{{\"s1\":\"test\"," +
                $"\"lgS1\":\"{largeStringData}\"," +
                $"\"i1\":10," +
                $"\"lgB641\":\"{largeBase64Data}\"," +
                $"\"b1\":true," +
                $"\"obj1\":{{\"lgB64AtObjStart\":\"{largeBase64Data}\",\"s2\":\"test\"}}," +
                $"\"obj2\":{{\"dtProp\":\"2014-12-31\",\"lgB64AtObjEnd\":\"{largeBase64Data}\"}}," +
                $"\"obj3\":{{\"lgB64BeforeRawVal\":\"{largeBase64Data}\",\"raw1\":\"foobar\"}}," +
                $"\"obj4\":{{\"raw2\":\"foobar\",\"lgB64AfterRawVal\":\"{largeBase64Data}\"}}," +
                $"\"arrWithLgB64\":[\"{largeBase64Data}\"]," +
                $"\"arrWithLgB64AtStart\":[\"{largeBase64Data}\",\"test\",\"test\"]," +
                $"\"arrWithLgB64AtEnd\":[1,2,\"{largeBase64Data}\"]," +
                $"\"arrWithManyLgB64AtStart\":[\"{largeBase64Data}\",\"{largeBase64Data}\",\"{largeBase64Data}\",1,\"test\"]," +
                $"\"arrWithManyLgB64AtEnd\":[1,\"test\",\"{largeBase64Data}\",\"{largeBase64Data}\",\"{largeBase64Data}\"]," +
                $"\"arrWithManyLgB64Only\":[\"{largeBase64Data}\",\"{largeBase64Data}\",\"{largeBase64Data}\"]," +
                $"\"obj5\":{{\"lgSAtObjStart\":\"{largeStringData}\",\"s2\":\"test\"}}," +
                $"\"obj6\":{{\"dtProp\":\"2014-12-31\",\"lgSAtObjEnd\":\"{largeStringData}\"}}," +
                $"\"obj7\":{{\"lgSBeforeRawVal\":\"{largeStringData}\",\"raw1\":\"foobar\"}}," +
                $"\"obj8\":{{\"raw2\":\"foobar\",\"lgSAfterRawVal\":\"{largeStringData}\"}}," +
                $"\"arrWithLgS\":[\"{largeStringData}\"]," +
                $"\"arrWithLgSAtStart\":[\"{largeStringData}\",\"test\",\"test\"]," +
                $"\"arrWithLgSAtEnd\":[1,2,\"{largeStringData}\"]," +
                $"\"arrWithManyLgSAtStart\":[\"{largeStringData}\",\"{largeStringData}\",\"{largeStringData}\",1,\"test\"]," +
                $"\"arrWithManyLgSAtEnd\":[1,\"test\",\"{largeStringData}\",\"{largeStringData}\",\"{largeStringData}\"]," +
                $"\"arrWithManyLgSOnly\":[\"{largeStringData}\",\"{largeStringData}\",\"{largeStringData}\"]," +
                $"\"arrWithLgSAndLgB64Mix1\":[\"{largeBase64Data}\",\"{largeStringData}\",\"{largeBase64Data}\",\"{largeStringData}\"]," +
                $"\"arrWithLgSAndLgB64Mix2\":[\"{largeStringData}\",\"{largeBase64Data}\",\"{largeStringData}\",\"{largeBase64Data}\"]," +
                $"\"arrWithLgSAndLgB64Mix3\":[\"{largeStringData}\",\"raw\",\"{largeBase64Data}\",\"raw\",\"{largeStringData}\",\"{largeBase64Data}\",\"raw\",\"raw\"]," +
                $"\"arrWithLgSAndLgB64Mix4\":[\"raw\",\"raw\",\"{largeStringData}\",\"raw\",\"{largeBase64Data}\",\"{largeStringData}\",\"{largeBase64Data}\"]" +
                $"}}";

            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);
                jsonWriter.StartObjectScopeAsync();

                await jsonWriter.WriteNameAsync("s1");
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.WriteNameAsync("lgS1");
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteNameAsync("i1");
                await jsonWriter.WriteValueAsync(10);
                await jsonWriter.WriteNameAsync("lgB641");
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.WriteNameAsync("b1");
                await jsonWriter.WriteValueAsync(true);

                await jsonWriter.WriteNameAsync("obj1");
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.WriteNameAsync("lgB64AtObjStart");
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.WriteNameAsync("s2");
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.EndObjectScopeAsync();

                await jsonWriter.WriteNameAsync("obj2");
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.WriteNameAsync("dtProp");
                await jsonWriter.WriteValueAsync(new Date(2014, 12, 31));
                await jsonWriter.WriteNameAsync("lgB64AtObjEnd");
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.EndObjectScopeAsync();

                await jsonWriter.WriteNameAsync("obj3");
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.WriteNameAsync("lgB64BeforeRawVal");
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.WriteNameAsync("raw1");
                await jsonWriter.WriteRawValueAsync("\"foobar\"");
                await jsonWriter.EndObjectScopeAsync();

                await jsonWriter.WriteNameAsync("obj4");
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.WriteNameAsync("raw2");
                await jsonWriter.WriteRawValueAsync("\"foobar\"");
                await jsonWriter.WriteNameAsync("lgB64AfterRawVal");
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.EndObjectScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithLgB64");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithLgB64AtStart");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithLgB64AtEnd");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(1);
                await jsonWriter.WriteValueAsync(2);
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithManyLgB64AtStart");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.WriteValueAsync(1);
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithManyLgB64AtEnd");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(1);
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithManyLgB64Only");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.WriteValueAsync(largeBinaryData);
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("obj5");
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.WriteNameAsync("lgSAtObjStart");
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteNameAsync("s2");
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.EndObjectScopeAsync();

                await jsonWriter.WriteNameAsync("obj6");
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.WriteNameAsync("dtProp");
                await jsonWriter.WriteValueAsync(new Date(2014, 12, 31));
                await jsonWriter.WriteNameAsync("lgSAtObjEnd");
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.EndObjectScopeAsync();

                await jsonWriter.WriteNameAsync("obj7");
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.WriteNameAsync("lgSBeforeRawVal");
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteNameAsync("raw1");
                await jsonWriter.WriteRawValueAsync("\"foobar\"");
                await jsonWriter.EndObjectScopeAsync();

                await jsonWriter.WriteNameAsync("obj8");
                await jsonWriter.StartObjectScopeAsync();
                await jsonWriter.WriteNameAsync("raw2");
                await jsonWriter.WriteRawValueAsync("\"foobar\"");
                await jsonWriter.WriteNameAsync("lgSAfterRawVal");
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.EndObjectScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithLgS");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithLgSAtStart");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithLgSAtEnd");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(1);
                await jsonWriter.WriteValueAsync(2);
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithManyLgSAtStart");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteValueAsync(1);
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithManyLgSAtEnd");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(1);
                await jsonWriter.WriteValueAsync("test");
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithManyLgSOnly");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.EndArrayScopeAsync();

                // mixed scenarios
                await jsonWriter.WriteNameAsync("arrWithLgSAndLgB64Mix1");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(largeBase64Data);
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteValueAsync(largeBase64Data);
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithLgSAndLgB64Mix2");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteValueAsync(largeBase64Data);
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteValueAsync(largeBase64Data);
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithLgSAndLgB64Mix3");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteRawValueAsync("\"raw\"");
                await jsonWriter.WriteValueAsync(largeBase64Data);
                await jsonWriter.WriteRawValueAsync("\"raw\"");
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteValueAsync(largeBase64Data);
                await jsonWriter.WriteRawValueAsync("\"raw\"");
                await jsonWriter.WriteRawValueAsync("\"raw\"");
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.WriteNameAsync("arrWithLgSAndLgB64Mix4");
                await jsonWriter.StartArrayScopeAsync();
                await jsonWriter.WriteRawValueAsync("\"raw\"");
                await jsonWriter.WriteRawValueAsync("\"raw\"");
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteRawValueAsync("\"raw\"");
                await jsonWriter.WriteValueAsync(largeBase64Data);
                await jsonWriter.WriteValueAsync(largeStringData);
                await jsonWriter.WriteValueAsync(largeBase64Data);
                await jsonWriter.EndArrayScopeAsync();

                await jsonWriter.EndObjectScopeAsync();

                await jsonWriter.FlushAsync();
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = reader.ReadToEnd();
                    string normalizedOutput = NormalizeJsonText(rawOutput);
                    string normalizedExpectedOutput = NormalizeJsonText(expectedJson);
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
                .Replace(@"\u0022", @"\""")
                // Utf8JsonWriter writes + as \u002b when writing JsonElement
                .Replace(@"\u002b", "+"); ;
        }

        private static byte[] GenerateByteArray(int length)
        {
            byte[] byteArray = new byte[length];
            for (int i = 0; i < length; i++)
            {
                byteArray[i] = (byte)(i % 256);
            }

            return byteArray;
        }
    }
}
