//---------------------------------------------------------------------
// <copyright file="ConstantExpressionModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Annotations;
    using Microsoft.OData.Edm.Library.Values;

    class ConstantExpressionModelBuilder
    {
        public static IEnumerable<XElement> ValueAnnotationInvalidIntegerConstantExpressionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Int32"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Int>foo</Int>
        </Annotation>
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidIntegerConstantAttributeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Int32"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Int=""foo"" />
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidBooleanConstantExpressionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Boolean"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Bool>foo</Bool>
        </Annotation>
    </Annotations>
</Schema>");
        }
        
        public static IEnumerable<XElement> ValueAnnotationInvalidBooleanConstantAttributeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Boolean"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Bool=""foo"" />
    </Annotations>
</Schema>");
        }
        
        public static IEnumerable<XElement> ValueAnnotationInvalidFloatConstantExpressionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Double"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Float>foo</Float>
        </Annotation>
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidFloatConstantAttributeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Double"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Float=""foo"" />
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidDecimalConstantExpressionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Decimal"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Decimal>foo</Decimal>
        </Annotation>
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidDecimalConstantAttributeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Decimal"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Decimal=""foo"" />
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidDurationConstantExpressionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Duration>foo</Duration>
        </Annotation>
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidDurationConstantAttributeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""foo"" />
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationValidDefaultDurationConstantExpressionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Duration>PT0S</Duration>
        </Annotation>
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationValidDefaultDurationConstantAttributeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""PT0S"" />
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationValidDurationConstantAttributeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""P1DT1H1M1.001S"" />
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationValidLargeDurationConstantModelCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""DefaultNamespace.Note"">
    <Annotation Term=""DefaultNamespace.Note"" Duration=""P5DT4H40M39.999S"" />
  </Annotations>
  <Term Name=""Note"" Type=""Edm.Duration"" />
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidValueDurationConstantAttributeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""10675199.02:48:05.4775807"" />
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidFormatDurationConstantAttributeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""1:2:3"" />
    </Annotations>
</Schema>");
        }

        public static IEdmModel ValueAnnotationValidDefaultDurationConstantModel()
        {
            var model = new EdmModel();
            var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetDuration(true));
            model.AddElement(valueTerm);

            var valueAnnotation = new EdmAnnotation(
                valueTerm,
                valueTerm,
                new EdmDurationConstant(new TimeSpan()));

            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static IEdmModel ValueAnnotationValidDurationConstantModel()
        {
            var model = new EdmModel();
            var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetDuration(true));
            model.AddElement(valueTerm);

            var valueAnnotation = new EdmAnnotation(
                valueTerm,
                valueTerm,
                new EdmDurationConstant(new TimeSpan(1, 1, 1, 1, 1)));

            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static IEdmModel ValueAnnotationValidLargeDurationConstantModel()
        {
            var model = new EdmModel();
            var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetDuration(true));
            model.AddElement(valueTerm);

            var valueAnnotation = new EdmAnnotation(
                valueTerm,
                valueTerm,
                new EdmDurationConstant(new TimeSpan(1, 99, 99, 99, 999)));

            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static IEdmModel ValueAnnotationInvalidTypeReferenceDurationConstantModel()
        {
            var model = new EdmModel();
            var valueTerm = new EdmTerm("DefaultNamespace", "Note", EdmCoreModel.Instance.GetDuration(true));
            model.AddElement(valueTerm);

            var valueAnnotation = new EdmAnnotation(
                valueTerm,
                valueTerm,
                new EdmDurationConstant(EdmCoreModel.Instance.GetDateTimeOffset(false), new TimeSpan(1, 99, 99, 99, 999)));

            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }
        
        public static IEnumerable<XElement> ValueAnnotationInvalidMaxDurationConstantAttributeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Duration=""P10775199DT2H48M5.4775807S"" />
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidGuidConstantExpressionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Guid"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Guid>foo</Guid>
        </Annotation>
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidGuidConstantAttributeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Guid"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Guid=""foo"" />
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidDateTimeOffsetConstantExpressionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.DateTimeOffset"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <DateTimeOffset>foo</DateTimeOffset>
        </Annotation>
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidDateTimeOffsetConstantAttributeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.DateTimeOffset"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" DateTimeOffset=""foo"" />
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidBinaryConstantExpressionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Binary"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Binary>foo</Binary>
        </Annotation>
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ValueAnnotationInvalidBinaryConstantAttributeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Binary"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"" Binary=""foo"" />
    </Annotations>
</Schema>");
        }

        private static IEnumerable<XElement> ConvertCsdlsToXElements(params string[] csdls)
        {
            return csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        }
    }
}
