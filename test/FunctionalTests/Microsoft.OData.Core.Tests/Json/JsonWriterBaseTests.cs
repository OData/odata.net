//---------------------------------------------------------------------
// <copyright file="JsonWriterBaseTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Verifies that different implementations of IJsonWriter produce similar output for the same inputs
    /// </summary>
    public abstract class JsonWriterBaseTests
    {
        const string MixedObjectJson = "{\"StringProp\":\"John\",\"IntProp\":10,\"BoolPropFalse\":false,\"DateProp\":\"2014-12-31\",\"RawStringProp\":\"foobar\",\"BoolPropTrue\":false,\"RawArrayProp\":[1,2,3,4,5],\"DateTimeOffsetProp\":\"2014-12-31T12:42:30+01:20\",\"DateProp\":\"2014-12-31\",\"TimeSpanProp\":\"PT12H42M30S\",\"TimeOfDayProp\":\"12:42:30.1000000\",\"ObjectProp\":{\"FloatProp\":3.1,\"NestedRawValue\":\"test\",\"ShortProp\":1124,\"ByteProp\":10,\"LongProp\":234234,\"SignedByteProp\":-10,\"GuidProp\":\"00000012-0000-0000-0000-012345678900\",\"ArrayPropWithEveryOtherValueRaw\":[\"test\",\"raw\",10,\"raw\",true,\"raw\",\"2014-12-31\",\"raw\",\"2014-12-31T12:42:30+01:20\",\"raw\",[1,2,3],1124,\"raw\",10,\"raw\",-10,\"raw\",25253,\"raw\",\"00000012-0000-0000-0000-012345678900\",\"raw\",\"foo\",\"raw\",12.3,\"raw\",2.6,\"raw\",{},\"raw\",[\"rawAtArrayStartBeforeString\",\"test\"],[\"rawAtArrayStartBeforeBool\",false],[\"rawAtArrayStartBeforeByte\",10],[\"rawAtArrayStartBeforeSignedByte\",-10],[\"rawAtArrayStartBeforeShort\",10],[\"rawAtArrayStartBeforeInt\",10],[\"rawAtArrayStartBeforeLong\",10],[\"rawAtArrayStartBeforeDouble\",10.2],[\"rawAtArrayStartBeforeFloat\",10.2],[\"rawAtArrayStartBeforeDecimal\",10.2],[\"rawAtArrayStartBeforeGuid\",\"00000012-0000-0000-0000-012345678900\"],[\"rawAtArrayStartBeforeObject\",{}],[\"rawAtArrayStartBeforeDateTimeOffset\",\"2014-12-31T12:42:30+01:20\"],[\"rawAtArrayStartBeforeDate\",\"2014-12-31\"],[\"rawAtArrayStartBeforeTimeOfDay\",\"12:42:30.1000000\"],[\"rawAtArrayStartBeforeTimeSpan\",\"PT12H42M30S\"],[\"rawAtArrayStartBeforeArray\",[]],[\"rawAtArrayStartBeforeNull\",null],[\"rawAtArrayStartBeforeByteArray\",\"TWFu\"],[\"rawAtArrayStartBeforeRaw\",\"raw\",\"test\",\"raw\"],\"raw\",\"raw\",\"raw\"]},\"ArrayProp\":[10,\"baz\",20,12.3,2.6,{\"RawObjectInArray\":true}],\"UntypedObjectProp\":{\"foo\":\"bar\"}}";
        const string SampleJsonInput = "{\"jsonInput\":{\"foo\":\"bar\"}}";
        const string MixedJsonInputAndRawValue = "{\"StringProp\":\"John\",\"JsonInputProp\":{\"jsonInput\":{\"foo\":\"bar\"}},\"RawStringProp\":\"foobar\",\"JsonInputAfterRawValue\":{\"jsonInput\":{\"foo\":\"bar\"}},\"ArrayProp\":[\"raw\",{\"jsonInput\":{\"foo\":\"bar\"}},\"foobar\",{\"jsonInput\":{\"foo\":\"bar\"}},\"raw\"]}";

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
                jsonWriter.WriteRawValue("[1,2,3,4,5]");

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
                jsonWriter.WriteValue(new TimeOfDay(12, 42, 30, 100));
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
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeNull""");
                jsonWriter.WriteValue((string)null);
                jsonWriter.EndArrayScope();
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""rawAtArrayStartBeforeByteArray""");
                jsonWriter.WriteValue(new byte[] { 77, 97, 110 });
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
                jsonWriter.WriteRawValue(@"{""RawObjectInArray"":true}");
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

        public void WritesJsonElementCorrectly()
        {
            using (JsonDocument jsonDoc = JsonDocument.Parse(MixedObjectJson))
            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);
                jsonWriter.WriteValue(jsonDoc.RootElement);

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

        [Fact]
        public void WriteObjectWithJsonInputAndRawValuesCorrectly()
        {
            using (JsonDocument jsonInput = JsonDocument.Parse(SampleJsonInput))
            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);
                jsonWriter.StartObjectScope();

                jsonWriter.WriteName("StringProp");
                jsonWriter.WriteValue("John");
                jsonWriter.WriteName("JsonInputProp");
                jsonWriter.WriteValue(jsonInput.RootElement);
                jsonWriter.WriteName("RawStringProp");
                jsonWriter.WriteRawValue(@"""foobar""");
                jsonWriter.WriteName("JsonInputAfterRawValue");
                jsonWriter.WriteValue(jsonInput.RootElement);

                jsonWriter.WriteName("ArrayProp");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.WriteValue(jsonInput.RootElement);
                jsonWriter.WriteValue("foobar");
                jsonWriter.WriteValue(jsonInput.RootElement);
                jsonWriter.WriteRawValue(@"""raw""");
                jsonWriter.EndArrayScope();

                jsonWriter.EndObjectScope();

                jsonWriter.Flush();
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

        [Fact]
        public void WritesLargeByteArraysCorrectly()
        {
            byte[] input = GenerateByteArray(1024 * 1024);// 1MB

            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);

                jsonWriter.WriteValue(input);
                jsonWriter.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                string expected = Convert.ToBase64String(input);

                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = reader.ReadToEnd();
                    Assert.Equal($"\"{expected}\"", rawOutput);
                }
            }
        }

        [Fact]
        public void WritesSimpleLargeStringsCorrectly()
        {
            int inputLength = 1024 * 1024; // 1MB
            string input = new string('a', inputLength);


            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);

                jsonWriter.WriteValue(input);
                jsonWriter.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = reader.ReadToEnd();
                    Assert.Equal($"\"{input}\"", rawOutput);
                }
            }
        }

        [Fact]
        public void WriteMixedJsonWithLargeValues()
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
                jsonWriter.StartObjectScope();

                jsonWriter.WriteName("s1");
                jsonWriter.WriteValue("test");
                jsonWriter.WriteName("lgS1");
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteName("i1");
                jsonWriter.WriteValue(10);
                jsonWriter.WriteName("lgB641");
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteName("b1");
                jsonWriter.WriteValue(true);

                jsonWriter.WriteName("obj1");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("lgB64AtObjStart");
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteName("s2");
                jsonWriter.WriteValue("test");
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("obj2");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("dtProp");
                jsonWriter.WriteValue(new Date(2014, 12, 31));
                jsonWriter.WriteName("lgB64AtObjEnd");
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("obj3");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("lgB64BeforeRawVal");
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteName("raw1");
                jsonWriter.WriteRawValue("\"foobar\"");
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("obj4");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("raw2");
                jsonWriter.WriteRawValue("\"foobar\"");
                jsonWriter.WriteName("lgB64AfterRawVal");
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("arrWithLgB64");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithLgB64AtStart");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteValue("test");
                jsonWriter.WriteValue("test");
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithLgB64AtEnd");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(1);
                jsonWriter.WriteValue(2);
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithManyLgB64AtStart");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteValue(1);
                jsonWriter.WriteValue("test");
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithManyLgB64AtEnd");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(1);
                jsonWriter.WriteValue("test");
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithManyLgB64Only");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("obj5");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("lgSAtObjStart");
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteName("s2");
                jsonWriter.WriteValue("test");
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("obj6");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("dtProp");
                jsonWriter.WriteValue(new Date(2014, 12, 31));
                jsonWriter.WriteName("lgSAtObjEnd");
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("obj7");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("lgSBeforeRawVal");
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteName("raw1");
                jsonWriter.WriteRawValue("\"foobar\"");
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("obj8");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("raw2");
                jsonWriter.WriteRawValue("\"foobar\"");
                jsonWriter.WriteName("lgSAfterRawVal");
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("arrWithLgS");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithLgSAtStart");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue("test");
                jsonWriter.WriteValue("test");
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithLgSAtEnd");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(1);
                jsonWriter.WriteValue(2);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithManyLgSAtStart");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(1);
                jsonWriter.WriteValue("test");
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithManyLgSAtEnd");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(1);
                jsonWriter.WriteValue("test");
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithManyLgSOnly");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.EndArrayScope();

                // mixed scenarios
                jsonWriter.WriteName("arrWithLgSAndLgB64Mix1");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithLgSAndLgB64Mix2");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithLgSAndLgB64Mix3");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithLgSAndLgB64Mix4");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.EndArrayScope();

                jsonWriter.EndObjectScope();

                jsonWriter.Flush();
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

        [Fact]
        public void WriteMixedJsonWithStreamingDataLargeValues()
        {
            byte[] largeBinaryData = GenerateByteArray(256 * 1024);
            string largeBase64Data = Convert.ToBase64String(largeBinaryData);

            string largeStringData = new string('a', 256 * 1024);

            string streamLargeStringData = new string('a', 1024 * 1024);

            //Returns a string with special characters (in this case emoji) at different sections of the string
            string streamLargeStringDataWithSpecialChars = ExpectedOutPutStringWithSpecialCharacters(streamLargeStringData);

            byte[] inputLargeByteArray = GenerateByteArray(1024 * 1024);
            string largeByteArray = Convert.ToBase64String(inputLargeByteArray);

            string expectedJson =
                $"{{\"s1\":\"test\"," +
                $"\"lgS1\":\"{streamLargeStringData}\"," +
                $"\"i1\":10," +
                $"\"lgB641\":\"{largeBase64Data}\"," +
                $"\"b1\":true," +
                $"\"obj1\":{{\"lgB64AtObjStart\":\"{largeBase64Data}\",\"s2\":\"test\"}}," +
                $"\"lgStreamLargeBytes\":\"{largeByteArray}\"," +
                $"\"obj2\":{{\"dtProp\":\"2014-12-31\",\"lgB64AtObjEnd\":\"{largeBase64Data}\"}}," +
                $"\"obj3\":{{\"lgB64BeforeRawVal\":\"{largeBase64Data}\",\"raw1\":\"foobar\"}}," +
                $"\"obj4\":{{\"raw2\":\"foobar\",\"lgB64AfterRawVal\":\"{largeBase64Data}\"}}," +
                $"\"arrWithStreamLgB64\":[\"{largeByteArray}\"]," +
                $"\"arrWithStreamLgB64AtStart\":[\"{largeByteArray}\",\"test\",\"test\"]," +
                $"\"arrWithStreamLgB64AtEnd\":[1,2,\"{largeByteArray}\"]," +
                $"\"arrWithManyStreamLgB64AtStart\":[\"{largeByteArray}\",\"{largeByteArray}\",\"{largeByteArray}\",1,\"test\"]," +
                $"\"arrWithManyStreamLgB64AtEnd\":[1,\"test\",\"{largeByteArray}\",\"{largeByteArray}\",\"{largeByteArray}\"]," +
                $"\"arrWithManyStreamLgB64Only\":[\"{largeByteArray}\",\"{largeByteArray}\",\"{largeByteArray}\"]," +
                $"\"obj5\":{{\"lgSAtObjStart\":\"{streamLargeStringDataWithSpecialChars}\",\"s2\":\"test\"}}," +
                $"\"obj6\":{{\"dtProp\":\"2014-12-31\",\"lgSAtObjEnd\":\"{streamLargeStringData}\"}}," +
                $"\"obj7\":{{\"lgSBeforeRawVal\":\"{largeStringData}\",\"raw1\":\"foobar\"}}," +
                $"\"obj8\":{{\"raw2\":\"foobar\",\"lgSAfterRawVal\":\"{streamLargeStringData}\"}}," +
                $"\"arrWithLgS\":[\"{streamLargeStringData}\"]," +
                $"\"arrWithLgSAtStart\":[\"{streamLargeStringData}\",\"test\",\"test\"]," +
                $"\"arrWithLgSAtEnd\":[1,2,\"{streamLargeStringData}\"]," +
                $"\"arrWithManyLgSAtStart\":[\"{streamLargeStringData}\",\"{streamLargeStringDataWithSpecialChars}\",\"{streamLargeStringData}\",1,\"test\"]," +
                $"\"arrWithManyLgSAtEnd\":[1,\"test\",\"{streamLargeStringData}\",\"{streamLargeStringData}\",\"{streamLargeStringData}\"]," +
                $"\"arrWithManyLgSOnly\":[\"{streamLargeStringData}\",\"{streamLargeStringData}\",\"{streamLargeStringData}\"]," +
                $"\"arrWithLgSAndLgB64Mix1\":[\"{largeByteArray}\",\"{streamLargeStringData}\",\"{largeBase64Data}\",\"{largeStringData}\"]," +
                $"\"arrWithLgSAndLgB64Mix2\":[\"{largeStringData}\",\"{largeBase64Data}\",\"{largeStringData}\",\"{largeBase64Data}\"]," +
                $"\"arrWithLgSAndLgB64Mix3\":[\"{largeStringData}\",\"raw\",\"{largeBase64Data}\",\"raw\",\"{largeStringData}\",\"{largeBase64Data}\",\"raw\",\"raw\"]," +
                $"\"arrWithLgSAndLgB64Mix4\":[\"raw\",\"raw\",\"{largeStringData}\",\"raw\",\"{largeBase64Data}\",\"{largeStringData}\",\"{largeBase64Data}\"]" +
                $"}}";

            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);
                jsonWriter.StartObjectScope();

                jsonWriter.WriteName("s1");
                jsonWriter.WriteValue("test");
                jsonWriter.WriteName("lgS1");
                var tw = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(tw, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                jsonWriter.WriteName("i1");
                jsonWriter.WriteValue(10);
                jsonWriter.WriteName("lgB641");
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteName("b1");
                jsonWriter.WriteValue(true);

                jsonWriter.WriteName("obj1");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("lgB64AtObjStart");
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteName("s2");
                jsonWriter.WriteValue("test");
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("lgStreamLargeBytes");
                var st = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(st, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();

                jsonWriter.WriteName("obj2");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("dtProp");
                jsonWriter.WriteValue(new Date(2014, 12, 31));
                jsonWriter.WriteName("lgB64AtObjEnd");
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("obj3");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("lgB64BeforeRawVal");
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.WriteName("raw1");
                jsonWriter.WriteRawValue("\"foobar\"");
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("obj4");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("raw2");
                jsonWriter.WriteRawValue("\"foobar\"");
                jsonWriter.WriteName("lgB64AfterRawVal");
                jsonWriter.WriteValue(largeBinaryData);
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("arrWithStreamLgB64");
                jsonWriter.StartArrayScope();
                var st1 = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(st1, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithStreamLgB64AtStart");
                jsonWriter.StartArrayScope();
                var st2 = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(st2, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();
                jsonWriter.WriteValue("test");
                jsonWriter.WriteValue("test");
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithStreamLgB64AtEnd");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(1);
                jsonWriter.WriteValue(2);
                var st3 = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(st3, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithManyStreamLgB64AtStart");
                jsonWriter.StartArrayScope();
                var st4 = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(st4, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();
                var st5 = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(st5, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();
                var st6 = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(st6, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();
                jsonWriter.WriteValue(1);
                jsonWriter.WriteValue("test");
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithManyStreamLgB64AtEnd");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(1);
                jsonWriter.WriteValue("test");
                var st7 = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(st7, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();
                var st8 = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(st8, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();
                var st9 = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(st9, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithManyStreamLgB64Only");
                jsonWriter.StartArrayScope();
                var sta = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(sta, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();
                var stb = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(stb, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();
                var stc = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(stc, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("obj5");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("lgSAtObjStart");
                var tw1 = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringsWithSpecialCharactersInChunks(tw1, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                jsonWriter.WriteName("s2");
                jsonWriter.WriteValue("test");
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("obj6");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("dtProp");
                jsonWriter.WriteValue(new Date(2014, 12, 31));
                jsonWriter.WriteName("lgSAtObjEnd");
                var tw3 = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(tw3, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("obj7");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("lgSBeforeRawVal");
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteName("raw1");
                jsonWriter.WriteRawValue("\"foobar\"");
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("obj8");
                jsonWriter.StartObjectScope();
                jsonWriter.WriteName("raw2");
                jsonWriter.WriteRawValue("\"foobar\"");
                jsonWriter.WriteName("lgSAfterRawVal");
                var tw4 = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(tw4, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                jsonWriter.EndObjectScope();

                jsonWriter.WriteName("arrWithLgS");
                jsonWriter.StartArrayScope();
                var tw5 = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(tw5, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithLgSAtStart");
                jsonWriter.StartArrayScope();
                var twa = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(twa, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                jsonWriter.WriteValue("test");
                jsonWriter.WriteValue("test");
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithLgSAtEnd");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(1);
                jsonWriter.WriteValue(2);
                var twb = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(twb, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithManyLgSAtStart");
                jsonWriter.StartArrayScope();
                var twa1 = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(twa1, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                var twb1 = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringsWithSpecialCharactersInChunks(twb1, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                var twc1 = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(twc1, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                jsonWriter.WriteValue(1);
                jsonWriter.WriteValue("test");
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithManyLgSAtEnd");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(1);
                jsonWriter.WriteValue("test");
                var twc = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(twc, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                var twd = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(twd, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                var twe = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(twe, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithManyLgSOnly");
                jsonWriter.StartArrayScope();
                var tw11 = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(tw11, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                var tw12 = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(tw12, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                var tw13 = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(tw13, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                jsonWriter.EndArrayScope();

                // mixed scenarios
                jsonWriter.WriteName("arrWithLgSAndLgB64Mix1");
                jsonWriter.StartArrayScope();
                var st11 = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(st11, inputLargeByteArray);
                jsonWriter.EndStreamValueScope();
                var tw33 = jsonWriter.StartTextWriterValueScope("text/plain");
                WriteLargeStringInChunks(tw33, streamLargeStringData);
                jsonWriter.EndTextWriterValueScope();
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithLgSAndLgB64Mix2");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithLgSAndLgB64Mix3");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.EndArrayScope();

                jsonWriter.WriteName("arrWithLgSAndLgB64Mix4");
                jsonWriter.StartArrayScope();
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteRawValue("\"raw\"");
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.WriteValue(largeStringData);
                jsonWriter.WriteValue(largeBase64Data);
                jsonWriter.EndArrayScope();

                jsonWriter.EndObjectScope();

                jsonWriter.Flush();
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

        [Fact]
        public void CorrectlyStreamsLargeByteArrayToOutput()
        {
            byte[] input = GenerateByteArray(1024 * 1024);

            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);
                var st = jsonWriter.StartStreamValueScope();
                WriteByteArrayInChunks(st, input);
                jsonWriter.EndStreamValueScope();
                jsonWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);

                string expected = Convert.ToBase64String(input);

                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = reader.ReadToEnd();
                    Assert.Equal($"\"{expected}\"", rawOutput);
                }
            }
        }

        [Fact]
        public void CorrectlyStreamsLargeStringsToOutput_ApplicationJson_ContentType()
        {
            int inputLength = 1024 * 1024; // 1MB
            string input = new string('a', inputLength);
            string expectedOutput = ExpectedOutPutStringWithSpecialCharacters_ApplicationJson(input);

            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);

                var tw = jsonWriter.StartTextWriterValueScope("application/json");

                WriteLargeStringsWithSpecialCharactersInChunks(tw, input);

                jsonWriter.EndTextWriterValueScope();
                jsonWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);

                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = reader.ReadToEnd();
                    Assert.Equal(expectedOutput, rawOutput);
                }
            }
        }

        [Fact]
        public void CorrectlyStreamsLargeStrings_WithOnlySpecialCharacters_ToOutput_UsingApplicationJson()
        {
            string input = "\n\n\n\n\"\"\n\n\n\n\"\"";
            string expectedOutput = "\n\n\n\n\"\"\n\n\n\n\"\"";

            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);

                var tw = jsonWriter.StartTextWriterValueScope("application/json");

                WriteSpecialCharsInChunksOfOddStringInChunks(tw, input);

                jsonWriter.EndTextWriterValueScope();
                jsonWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);

                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = reader.ReadToEnd();
                    Assert.Equal(expectedOutput, rawOutput);
                }
            }
        }

        [Theory]
        [InlineData("application/json", 'a', "a")]
        [InlineData("text/html", 'a', "\"a\"")]
        [InlineData("text/plain", 'a', "\"a\"")]
        // JSON special char
        [InlineData("application/json", '"', "\"")]
        [InlineData("text/html", '"', "\"\\\"\"")]
        [InlineData("text/plain", '"', "\"\\\"\"")]
        // non-ascii
        [InlineData("application/json", '你', "你")]
        [InlineData("text/html", '你', "\"你\"")]
        [InlineData("text/plain", '你', "\"你\"")]
        public void TextWriter_CorrectlyWritesSingleCharacter(string contentType, char value, string expectedOutput)
        {
            using MemoryStream stream = new MemoryStream();
            IJsonWriter jsonWriter = CreateJsonWriter(stream, isIeee754Compatible: false, Encoding.UTF8);
            var tw = jsonWriter.StartTextWriterValueScope(contentType);
            tw.Write(value);
            jsonWriter.EndTextWriterValueScope();
            jsonWriter.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            using StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8);
            string rawOutput = reader.ReadToEnd();
            Assert.Equal(expectedOutput, rawOutput);
        }

        [Theory]
        [InlineData(124.45, "124.45")]
        // We write the .0 for doubles without a fractional part
        // for consistency with the original JsonWriter implementation
        // and with previous versions. However, it's not a hard requirement.
        // Clients should not rely on the .0 to decide whether a value
        // is an integer or double.
        // In the future we can consider relaxing the need to add a .0 (possibly behind a feature flag).
        [InlineData(124.0, "124.0")]
        [InlineData(1.123456789012345, "1.123456789012345")]
        [InlineData(1.245E+24, "1.245E+24")]
        [InlineData(1.245E-24, "1.245E-24")]
        [InlineData(double.PositiveInfinity, "\"INF\"")]
        [InlineData(double.NegativeInfinity, "\"-INF\"")]
        [InlineData(double.NaN, "\"NaN\"")]
        public void WriteDouble(double value, string expectedOutput)
        {
            using MemoryStream stream = new MemoryStream();
            IJsonWriter jsonWriter = CreateJsonWriter(stream, isIeee754Compatible: false, Encoding.UTF8);
            jsonWriter.WriteValue(value);
            jsonWriter.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            using StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8);
            string rawOutput = reader.ReadToEnd();
            Assert.Equal(expectedOutput, rawOutput);
        }

        [Theory]
        [InlineData(124.45f, "124.45")]
        [InlineData(124.0f, "124")]
        [InlineData(1.123456f, "1.123456")]
        [InlineData(1.245E+10f, "1.245E+10")]
        [InlineData(1.245E-10f, "1.245E-10")]
        [InlineData(float.PositiveInfinity, "\"INF\"")]
        [InlineData(float.NegativeInfinity, "\"-INF\"")]
        [InlineData(float.NaN, "\"NaN\"")]
        public void WriteFloat(float value, string expectedOutput)
        {
            using MemoryStream stream = new MemoryStream();
            IJsonWriter jsonWriter = CreateJsonWriter(stream, isIeee754Compatible: false, Encoding.UTF8);
            jsonWriter.WriteValue(value);
            jsonWriter.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            using StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8);
            string rawOutput = reader.ReadToEnd();
            Assert.Equal(expectedOutput, rawOutput);
        }

        [Fact]
        public void WriteDateOnly()
        {
            using MemoryStream stream = new MemoryStream();
            IJsonWriter jsonWriter = CreateJsonWriter(stream, isIeee754Compatible: false, Encoding.UTF8);
            jsonWriter.WriteValue(new DateOnly(2024, 10, 1));
            jsonWriter.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            using StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8);
            string rawOutput = reader.ReadToEnd();
            Assert.Equal("\"2024-10-01\"", rawOutput);
        }

        [Fact]
        public void WriteTimeOnly()
        {
            using MemoryStream stream = new MemoryStream();
            IJsonWriter jsonWriter = CreateJsonWriter(stream, isIeee754Compatible: false, Encoding.UTF8);
            jsonWriter.WriteValue(new TimeOnly(4, 3, 2, 1));
            jsonWriter.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            using StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8);
            string rawOutput = reader.ReadToEnd();
            Assert.Equal("\"04:03:02.0010000\"", rawOutput);
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
                // Utf8JsonWriter escapes double-quotes using \u0022
                // OData JsonWrtier uses \"
                .Replace(@"\u0022", @"\""")
                // Utf8JsonWriter writes + as \u002b when writing JsonElement
                .Replace(@"\u002b", "+");
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

        private static void WriteByteArrayInChunks(Stream streamWriter, byte[] input)
        {
            const int chunkSize = 4096;
            int bytesWritten = 0;

            // Stream the byte array and write it in chunks
            while (bytesWritten < input.Length)
            {
                int remainingBytes = input.Length - bytesWritten;
                int bytesToWrite = Math.Min(chunkSize, remainingBytes);
                streamWriter.Write(input, bytesWritten, bytesToWrite); // Write the chunk to the stream
                bytesWritten += bytesToWrite;
            }
        }

        internal static void WriteLargeStringInChunks(TextWriter tw, string input)
        {
            // Define chunk size.
            int chunkSize = 4096;

            // Stream the string to the output stream in chunks.
            for (int i = 0; i < input.Length; i += chunkSize)
            {
                int remainingLength = Math.Min(chunkSize, input.Length - i);
                string chunk = input.Substring(i, remainingLength);
                tw.Write(chunk);
            }
        }

        internal static void WriteSpecialCharsInChunksOfOddStringInChunks(TextWriter tw, string input)
        {
            // Define chunk size
            int chunkSize = 3;

            // Stream the string to the output stream in chunks
            for (int i = 0; i < input.Length; i += chunkSize)
            {
                int remainingLength = Math.Min(chunkSize, input.Length - i);
                string chunk = input.Substring(i, remainingLength);
                tw.Write(chunk);
            }
        }

        internal static void WriteLargeStringsWithSpecialCharactersInChunks(TextWriter tw, string input)
        {
            // Define chunk size
            int chunkSize = 4096;

            // Stream the string to the output stream in chunks
            for (int i = 0; i < input.Length; i += chunkSize)
            {
                int remainingLength = Math.Min(chunkSize, input.Length - i);
                string chunk = input.Substring(i, remainingLength);
                chunk += "\n\n\n\n\"\"\n\n\n\n\"\"";
                tw.Write(chunk);
            }

        }

        internal static string ExpectedOutPutStringWithSpecialCharacters(string input)
        {
            string s = "";
            // Define chunk size
            int chunkSize = 4096;

            // Stream the string to the output stream in chunks
            for (int i = 0; i < input.Length; i += chunkSize)
            {
                int remainingLength = Math.Min(chunkSize, input.Length - i);
                string chunk = input.Substring(i, remainingLength);
                s += chunk;
                s += "\\n\\n\\n\\n\\\"\\\"\\n\\n\\n\\n\\\"\\\"";
            }

            return s;
        }

        internal static string ExpectedOutPutStringWithSpecialCharacters_ApplicationJson(string input)
        {
            string s = "";
            // Define chunk size
            int chunkSize = 4096;

            // Stream the string to the output stream in chunks
            for (int i = 0; i < input.Length; i += chunkSize)
            {
                int remainingLength = Math.Min(chunkSize, input.Length - i);
                string chunk = input.Substring(i, remainingLength);
                s += chunk;
                s += "\n\n\n\n\"\"\n\n\n\n\"\"";
            }

            return s;
        }

        internal static string ExpectedOutPutStringWithSpecialCharacters_ODataUtf8Encoding(string input)
        {
            string s = "";
            // Define chunk size
            int chunkSize = 4096;

            // Stream the string to the output stream in chunks
            for (int i = 0; i < input.Length; i += chunkSize)
            {
                int remainingLength = Math.Min(chunkSize, input.Length - i);
                string chunk = input.Substring(i, remainingLength);
                s += chunk;
                s += "\\n\\n\\n\\n\\\"\\\"\\n\\n\\n\\n\\\"\\\"";
            }

            return s;
        }
    }
}
