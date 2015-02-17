//---------------------------------------------------------------------
// <copyright file="ODataJsonDateTimeFormattingValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Default implementation of the contract for validating json datetime/datetimeoffset formatting
    /// </summary>
    [ImplementationName(typeof(IODataJsonDateTimeFormattingValidator), "Default")]
    public class ODataJsonDateTimeFormattingValidator : IODataJsonDateTimeFormattingValidator
    {
        /// <summary>
        /// Validates that any datetime or datetimeoffset values in the payload are formmatted correctly for the given version.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public void ValidateDateTimeFormatting(ODataPayloadElement payload)
        {
            new ValidatingVisitor().Validate(payload);
        }

        /// <summary>
        /// Visitor which validates json datetime/datetimeoffset formatting
        /// </summary>
        internal class ValidatingVisitor : ODataPayloadElementVisitorBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValidatingVisitor"/> class.
            /// </summary>
            internal ValidatingVisitor()
            {
            }

            public void Validate(ODataPayloadElement payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                payloadElement.Accept(this);
            }

            /// <summary>
            /// Visits the payload element
            /// </summary>
            /// <param name="payloadElement">The payload element to visit</param>
            public override void Visit(EntityInstance payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                // ideally, we would strictly validate dynamic properties, but far more complex than it is worth
                // for now, if we hit an open type, we will not validate the formatting of datetimes
                bool isOpen = payloadElement.Annotations
                    .OfType<DataTypeAnnotation>()
                    .Select(a => a.DataType)
                    .OfType<EntityDataType>()
                    .Select(e => e.Definition)
                    .Any(t => t.IsOpen);

                if (isOpen)
                {
                    return;
                }

                base.Visit(payloadElement);
            }

            public override void Visit(PrimitiveValue payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var value = payloadElement.ClrValue;
                if (value != null)
                {
                    bool isDateTime = value is DateTime;
                    bool isDateTimeOffset = value is DateTimeOffset;

                    if (isDateTime || isDateTimeOffset)
                    {
                        var originalString = payloadElement.Annotations
                            .OfType<ReplacedElementAnnotation>()
                            .Select(a => a.Original)
                            .OfType<PrimitiveValue>()
                            .Select(p => p.ClrValue)
                            .OfType<string>()
                            .SingleOrDefault();

                        if (isDateTime)
                        {
                            VerifyDateTime(originalString);
                        }
                        else
                        {
                            VerifyDateTimeOffset(originalString);
                        }
                    }
                }
            }

            internal static void VerifyDateTime(string serialized)
            {
                Verify(serialized, PlatformHelper.ConvertStringToDateTime);
            }

            internal static void VerifyDateTimeOffset(string serialized)
            {
                Verify(serialized, XmlConvert.ToDateTimeOffset);
            }

            internal static void Verify<TDateTime>(string serialized, Func<string, TDateTime> parseCallback)
            {
                try
                {
                    parseCallback(serialized);
                }
                catch (Exception e)
                {
                    throw new AssertionFailedException(string.Format(CultureInfo.InvariantCulture, "{0} string value was not readable by xml-convert: '{1}'", typeof(TDateTime).Name, serialized), e);
                }
            }
        }
    }
}
