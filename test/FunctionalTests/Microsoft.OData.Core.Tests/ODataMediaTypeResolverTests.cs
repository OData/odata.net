//---------------------------------------------------------------------
// <copyright file="ODataMediaTypeResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataMediaTypeResolverTests
    {
        private static readonly List<ODataMediaTypeFormat> JsonMediaTypes = new List<ODataMediaTypeFormat>();

        /// <summary>
        /// An array that maps stores the supported media types for all <see cref="ODataPayloadKind"/>, ATOM excluded.
        /// Here is a comparsion baseline
        /// </summary>
        /// <remarks>
        /// The set of supported media types is ordered (desc) by their precedence/priority with respect to (1) format and (2) media type.
        /// As a result the default media type for a given payloadKind is the first entry in the MediaTypeWithFormat array.
        /// </remarks>
        private static readonly IList<ODataMediaTypeFormat>[] MediaTypeCollection = {
            // feed
            JsonMediaTypes,
            // entry
            JsonMediaTypes,
            // property
            JsonMediaTypes,
            // entity reference link
            JsonMediaTypes,
            // entity reference links
            JsonMediaTypes,
            // value
            new ODataMediaTypeFormat[]
            { 
                new ODataMediaTypeFormat (new ODataMediaType(MimeConstants.MimeTextType, MimeConstants.MimePlainSubType),ODataFormat.RawValue),
            },
            // binary
            new ODataMediaTypeFormat[]
            { 
                new ODataMediaTypeFormat ( new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeOctetStreamSubType) ,ODataFormat.RawValue),
            },
            // collection
            JsonMediaTypes,
            // service document
            JsonMediaTypes,
            // metadata document
            new ODataMediaTypeFormat[]
            { 
                new ODataMediaTypeFormat ( new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeXmlSubType), ODataFormat.Metadata),
            },
            // error
            JsonMediaTypes,
            // batch
            new ODataMediaTypeFormat[]
            { 
                // Note that as per spec the multipart/mixed must have a boundary parameter which is not specified here. We will add that parameter
                // when using this mime type because we need to generate a new boundary every time.
                new ODataMediaTypeFormat (new ODataMediaType(MimeConstants.MimeMultipartType, MimeConstants.MimeMixedSubType) ,ODataFormat.Batch),
            },
            // parameter
            JsonMediaTypes,
            // individual property
            JsonMediaTypes,
            // delta
            new ODataMediaTypeFormat[]
            { 
            },
            // async
            new ODataMediaTypeFormat[]
            { 
                new ODataMediaTypeFormat ( new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeHttpSubType) ,ODataFormat.RawValue),
            },
        };

        static ODataMediaTypeResolverTests()
        {
            foreach (var metadata in new[] { MimeConstants.MimeMetadataParameterValueMinimal, MimeConstants.MimeMetadataParameterValueFull, MimeConstants.MimeMetadataParameterValueNone, null })
            {
                foreach (var streaming in new[] { MimeConstants.MimeParameterValueTrue, MimeConstants.MimeParameterValueFalse, null })
                {
                    foreach (var ieee754Compatible in new[] { MimeConstants.MimeParameterValueFalse, MimeConstants.MimeParameterValueTrue, null })
                    {
                        var parameters = new List<KeyValuePair<string, string>>();
                        if (metadata != null)
                        {
                            parameters.Add(new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, metadata));
                        }

                        if (streaming != null)
                        {
                            parameters.Add(new KeyValuePair<string, string>(MimeConstants.MimeStreamingParameterName, streaming));
                        }

                        if (ieee754Compatible != null)
                        {
                            parameters.Add(new KeyValuePair<string, string>(MimeConstants.MimeIeee754CompatibleParameterName, ieee754Compatible));
                        }

                        JsonMediaTypes.Add(new ODataMediaTypeFormat
                        (new ODataMediaType(
                                MimeConstants.MimeApplicationType,
                                MimeConstants.MimeJsonSubType,
                                parameters.Any() ? parameters : null),
                            ODataFormat.Json));
                    }
                }
            }
        }

        [Fact]
        public void TestJsonMediaType()
        {
            var resolver = ODataMediaTypeResolver.GetMediaTypeResolver(false);
            foreach (var payloadKind in Enum.GetValues(typeof(ODataPayloadKind)).Cast<ODataPayloadKind>())
            {
                if (payloadKind == ODataPayloadKind.Unsupported)
                {
                    continue;
                }

                var expected = MediaTypeCollection[(int)payloadKind];
                var actual = resolver.GetMediaTypeFormats(payloadKind);
                Console.WriteLine(payloadKind);
                actual.ShouldBeEquivalentTo(expected);
            }
        }

        [Fact]
        public void TestCustomMediaTypeGetMediaTypesForPayloadKind()
        {
            var resolver = new MyMediaTypeResolver();
            foreach (var payloadKind in Enum.GetValues(typeof(ODataPayloadKind)).Cast<ODataPayloadKind>())
            {
                if (payloadKind == ODataPayloadKind.Unsupported)
                {
                    continue;
                }

                var expected = MediaTypeCollection[(int)payloadKind].ToList();
                expected.Insert(0, MyFormat.MediaTypeWithFormatA);

                var actual = resolver.GetMediaTypeFormats(payloadKind);
                Console.WriteLine(payloadKind);
                actual.ShouldBeEquivalentTo(expected);
            }
        }

        [Fact]
        public void TestCustomMediaTypeGetFormatFromContentType()
        {
            var resolver = new MyMediaTypeResolver();
            foreach (var payloadKind in Enum.GetValues(typeof(ODataPayloadKind)).Cast<ODataPayloadKind>())
            {
                if (payloadKind == ODataPayloadKind.Unsupported)
                {
                    continue;
                }

                string contentType = "text/x-A";
                string expectedBoundary = null;
                ODataMediaType expectedMediaType = MyFormat.MediaTypeWithFormatA.MediaType;
                if (payloadKind == ODataPayloadKind.Batch)
                {
                    expectedBoundary = "ba_" + Guid.NewGuid();
                    contentType += ";boundary=" + expectedBoundary;
                    expectedMediaType = new ODataMediaType("text", "x-A", new KeyValuePair<string, string>("boundary", expectedBoundary));
                }

                ODataMediaType mediaType;
                Encoding encoding;
                ODataPayloadKind selectedPayloadKind;
                string batchBoundary;
                ODataFormat actual = MediaTypeUtils.GetFormatFromContentType(contentType, new[] { payloadKind }, resolver, out mediaType, out encoding, out selectedPayloadKind, out batchBoundary);

                Console.WriteLine(payloadKind);
                actual.ShouldBeEquivalentTo(MyFormat.Instance);
                mediaType.ShouldBeEquivalentTo(expectedMediaType);
                encoding.ShouldBeEquivalentTo(payloadKind == ODataPayloadKind.BinaryValue ? null : Encoding.UTF8);
                selectedPayloadKind.ShouldBeEquivalentTo(payloadKind);
                batchBoundary.ShouldBeEquivalentTo(expectedBoundary);
            }
        }

        private class MyFormat : ODataFormat
        {
            public static ODataFormat Instance = new MyFormat();

            public static ODataMediaTypeFormat MediaTypeWithFormatA = new ODataMediaTypeFormat(new ODataMediaType("text", "x-A"), Instance);

            public override IEnumerable<ODataPayloadKind> DetectPayloadKind(ODataMessageInfo messageInfo, ODataMessageReaderSettings settings)
            {
                throw new System.NotImplementedException();
            }

            public override ODataInputContext CreateInputContext(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
            {
                throw new NotImplementedException();
            }

            public override ODataOutputContext CreateOutputContext(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings)
            {
                throw new NotImplementedException();
            }

            public override System.Threading.Tasks.Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings settings)
            {
                throw new NotImplementedException();
            }

            public override System.Threading.Tasks.Task<ODataInputContext> CreateInputContextAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
            {
                throw new NotImplementedException();
            }

            public override System.Threading.Tasks.Task<ODataOutputContext> CreateOutputContextAsync(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings)
            {
                throw new NotImplementedException();
            }
        }

        private class MyMediaTypeResolver : ODataMediaTypeResolver
        {
            public override IEnumerable<ODataMediaTypeFormat> GetMediaTypeFormats(ODataPayloadKind payloadKind)
            {
                return new[] { MyFormat.MediaTypeWithFormatA }.Concat(base.GetMediaTypeFormats(payloadKind));
            }
        }
    }
}
