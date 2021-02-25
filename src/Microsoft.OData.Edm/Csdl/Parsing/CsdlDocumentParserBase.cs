//---------------------------------------------------------------------
// <copyright file="CsdlDocumentParserBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Csdl.Parsing.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// CSDL document xml base parser, mainly for annotation value and type parser.
    /// </summary>
    internal abstract class CsdlDocumentParserBase<TResult> : EdmXmlDocumentParser<TResult>
    {
        internal CsdlDocumentParserBase(string documentPath, XmlReader reader)
            : base(documentPath, reader)
        {
        }

        protected virtual CsdlNamedTypeReference ParseNamedTypeReference(string typeName, bool isNullable, CsdlLocation parentLocation)
        {
            bool isUnbounded;
            int? maxLength;
            bool? unicode;
            int? precision;
            int? scale;
            int? srid;

            EdmPrimitiveTypeKind kind = EdmCoreModel.Instance.GetPrimitiveTypeKind(typeName);
            switch (kind)
            {
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Date:
                case EdmPrimitiveTypeKind.PrimitiveType:
                    return new CsdlPrimitiveTypeReference(kind, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Binary:
                    this.ParseBinaryFacets(out isUnbounded, out maxLength);
                    return new CsdlBinaryTypeReference(isUnbounded, maxLength, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    this.ParseTemporalFacets(out precision);
                    return new CsdlTemporalTypeReference(kind, precision, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Decimal:
                    this.ParseDecimalFacets(out precision, out scale);
                    return new CsdlDecimalTypeReference(precision, scale, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.String:
                    this.ParseStringFacets(out isUnbounded, out maxLength, out unicode);
                    return new CsdlStringTypeReference(isUnbounded, maxLength, unicode, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    this.ParseSpatialFacets(out srid, CsdlConstants.Default_SpatialGeographySrid);
                    return new CsdlSpatialTypeReference(kind, srid, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    this.ParseSpatialFacets(out srid, CsdlConstants.Default_SpatialGeometrySrid);
                    return new CsdlSpatialTypeReference(kind, srid, typeName, isNullable, parentLocation);

                case EdmPrimitiveTypeKind.None:
                    if (string.Equals(typeName, CsdlConstants.TypeName_Untyped, StringComparison.Ordinal))
                    {
                        return new CsdlUntypedTypeReference(typeName, parentLocation);
                    }

                    break;
            }

            this.ParseTypeDefinitionFacets(out isUnbounded, out maxLength, out unicode, out precision, out scale, out srid);
            return new CsdlNamedTypeReference(isUnbounded, maxLength, unicode, precision, scale, srid, typeName, isNullable, parentLocation);
        }

        protected virtual CsdlTypeReference ParseTypeReference(string typeString, XmlElementValueCollection childValues, CsdlLocation parentLocation, Optionality typeInfoOptionality)
        {
            bool isNullable = OptionalBoolean(CsdlConstants.Attribute_Nullable) ?? CsdlConstants.Default_Nullable;

            CsdlTypeReference elementType = null;
            if (typeString != null)
            {
                string[] typeInformation = typeString.Split(new char[] { '(', ')' });
                string typeName = typeInformation[0];
                switch (typeName)
                {
                    case CsdlConstants.Value_Collection:
                        {
                            string elementTypeName = typeInformation.Length > 1 ? typeInformation[1] : typeString;
                            elementType = new CsdlExpressionTypeReference(
                                          new CsdlCollectionType(
                                          this.ParseNamedTypeReference(elementTypeName, isNullable, parentLocation), parentLocation), isNullable, parentLocation);
                        }

                        break;
                    case CsdlConstants.Value_Ref:
                        {
                            string elementTypeName = typeInformation.Length > 1 ? typeInformation[1] : typeString;
                            elementType = new CsdlExpressionTypeReference(
                                          new CsdlEntityReferenceType(
                                          this.ParseNamedTypeReference(elementTypeName, isNullable, parentLocation), parentLocation), CsdlConstants.Default_Nullable, parentLocation);
                        }

                        break;
                    default:
                        elementType = this.ParseNamedTypeReference(typeName, isNullable, parentLocation);
                        break;
                }
            }
            else if (childValues != null)
            {
                elementType = childValues.ValuesOfType<CsdlTypeReference>().FirstOrDefault();
            }

            if (elementType == null && typeInfoOptionality == Optionality.Required)
            {
                if (childValues != null)
                {
                    // If childValues is null, then it is the case when a required type attribute was expected.
                    // In this case, we do not report the error as it should already be reported by EdmXmlDocumentParser.RequiredType method.
                    this.ReportError(parentLocation, EdmErrorCode.MissingType, Edm.Strings.CsdlParser_MissingTypeAttributeOrElement);
                }
                else
                {
                    Debug.Assert(this.Errors.Any(), "There should be an error reported for the missing required type attribute.");
                }

                elementType = new CsdlNamedTypeReference(String.Empty, isNullable, parentLocation);
            }

            return elementType;
        }

        protected virtual XmlElementParser<CsdlAnnotation> CreateAnnotationParser()
        {
            var stringConstantExpressionParser =
                //// <String/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_String, OnStringConstantExpression);

            var binaryConstantExpressionParser =
                //// <Binary/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Binary, OnBinaryConstantExpression);

            var intConstantExpressionParser =
                //// <Int/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Int, OnIntConstantExpression);

            var floatConstantExpressionParser =
                //// <Float/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Float, OnFloatConstantExpression);

            var guidConstantExpressionParser =
                //// <Guid/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Guid, OnGuidConstantExpression);

            var decimalConstantExpressionParser =
                //// <Decimal/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Decimal, OnDecimalConstantExpression);

            var boolConstantExpressionParser =
                //// <Bool/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Bool, OnBoolConstantExpression);

            var durationConstantExpressionParser =
                //// <Duration/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Duration, OnDurationConstantExpression);

            var dateConstantExpressionParser =
                //// <Date/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Date, OnDateConstantExpression);

            var timeOfDayConstantExpressionParser =
                //// <TimeOfDay/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_TimeOfDay, OnTimeOfDayConstantExpression);

            var dateTimeOffsetConstantExpressionParser =
                //// <DateTimeOffset/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_DateTimeOffset, OnDateTimeOffsetConstantExpression);

            var nullConstantExpressionParser =
               //// <Null/>
               CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Null, OnNullConstantExpression);

            var pathExpressionParser =
                //// <Path/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Path, OnPathExpression);

            var propertyPathExpressionParser =
                //// <PropertyPath/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_PropertyPath, OnPropertyPathExpression);

            var navigationPropertyPathExpressionParser =
                //// <NavigationPropertyPath/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_NavigationPropertyPath, OnNavigationPropertyPathExpression);

            var enumMemberExpressionParser =
                //// <EnumMember/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_EnumMember, this.OnEnumMemberExpression);

            var ifExpressionParser =
                //// <If>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_If, this.OnIfExpression);

            var castExpressionParser =
                //// <Cast>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Cast, this.OnCastExpression);

            var isTypeExpressionParser =
                //// <IsType>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_IsType, this.OnIsTypeExpression);

            var propertyValueParser =
                //// <PropertyValue>
                CsdlElement<CsdlPropertyValue>(CsdlConstants.Element_PropertyValue, this.OnPropertyValueElement);

            var recordExpressionParser =
                //// <Record>
                CsdlElement<CsdlRecordExpression>(CsdlConstants.Element_Record, this.OnRecordElement,
                    //// <PropertyValue />
                    propertyValueParser);
            //// </Record>

            var labeledElementParser =
                //// <LabeledElement>
                CsdlElement<CsdlLabeledExpression>(CsdlConstants.Element_LabeledElement, this.OnLabeledElement);

            var collectionExpressionParser =
                //// <Collection>
                CsdlElement<CsdlCollectionExpression>(CsdlConstants.Element_Collection, this.OnCollectionElement);

            var applyExpressionParser =
                //// <Apply>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_Apply, this.OnApplyElement);

            var labeledElementReferenceExpressionParser =
                //// <LabeledElementReference/>
                CsdlElement<CsdlExpressionBase>(CsdlConstants.Element_LabeledElementReference, this.OnLabeledElementReferenceExpression);

            XmlElementParser[] expressionParsers =
            {
                //// <String/>
                stringConstantExpressionParser,
                //// <Binary/>
                binaryConstantExpressionParser,
                //// <Int/>
                intConstantExpressionParser,
                //// <Float/>
                floatConstantExpressionParser,
                //// <Guid/>
                guidConstantExpressionParser,
                //// <Decimal/>
                decimalConstantExpressionParser,
                //// <Bool/>
                boolConstantExpressionParser,
                //// <Date/>
                dateConstantExpressionParser,
                //// <DateTimeOffset/>
                dateTimeOffsetConstantExpressionParser,
                //// <Duration/>
                durationConstantExpressionParser,
                //// <TimeOfDay/>
                timeOfDayConstantExpressionParser,
                //// <Null/>
                nullConstantExpressionParser,
                //// <Path/>
                pathExpressionParser,
                //// <PropertyPath/>
                propertyPathExpressionParser,
                //// <NavigationPropertyPath/>
                navigationPropertyPathExpressionParser,
                //// <If/>
                ifExpressionParser,
                //// <IsType/>
                isTypeExpressionParser,
                //// <Cast>
                castExpressionParser,
                //// <Record/>
                recordExpressionParser,
                //// <Collection/>
                collectionExpressionParser,
                //// <LabeledElementReference/>
                labeledElementReferenceExpressionParser,
                //// <PropertyValue/>
                propertyValueParser,
                //// <LabeledElement/>
                labeledElementParser,
                //// <EnumConstant/>
                enumMemberExpressionParser,
                //// </Apply>
                applyExpressionParser
            };

            AddChildParsers(ifExpressionParser, expressionParsers);
            AddChildParsers(castExpressionParser, expressionParsers);
            AddChildParsers(isTypeExpressionParser, expressionParsers);
            AddChildParsers(propertyValueParser, expressionParsers);
            AddChildParsers(collectionExpressionParser, expressionParsers);
            AddChildParsers(labeledElementParser, expressionParsers);
            AddChildParsers(applyExpressionParser, expressionParsers);

            var annotationParser =
                //// <Annotation>
                CsdlElement(CsdlConstants.Element_Annotation, this.OnAnnotationElement);

            AddChildParsers(annotationParser, expressionParsers);

            return annotationParser;
        }

        protected override void AnnotateItem(object result, XmlElementValueCollection childValues)
        {
            CsdlElement annotatedItem = result as CsdlElement;
            if (annotatedItem == null)
            {
                return;
            }

            foreach (var xmlAnnotation in this.currentElement.Annotations)
            {
                annotatedItem.AddAnnotation(new CsdlDirectValueAnnotation(xmlAnnotation.NamespaceName, xmlAnnotation.Name, xmlAnnotation.Value, xmlAnnotation.IsAttribute, xmlAnnotation.Location));
            }

            foreach (var annotation in childValues.ValuesOfType<CsdlAnnotation>())
            {
                annotatedItem.AddAnnotation(annotation);
            }
        }

        private CsdlAnnotation OnAnnotationElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string term = RequiredQualifiedName(CsdlConstants.Attribute_Term);
            string qualifier = Optional(CsdlConstants.Attribute_Qualifier);
            CsdlExpressionBase expression = this.ParseAnnotationExpression(element, childValues);

            return new CsdlAnnotation(term, qualifier, expression, element.Location);
        }

        private CsdlPropertyValue OnPropertyValueElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string property = Required(CsdlConstants.Attribute_Property);
            CsdlExpressionBase expression = this.ParseAnnotationExpression(element, childValues);

            return new CsdlPropertyValue(property, expression, element.Location);
        }

        private CsdlRecordExpression OnRecordElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string type = OptionalQualifiedName(CsdlConstants.Attribute_Type);
            IEnumerable<CsdlPropertyValue> propertyValues = childValues.ValuesOfType<CsdlPropertyValue>();

            return new CsdlRecordExpression(type != null ? new CsdlNamedTypeReference(type, false, element.Location) : null, propertyValues, element.Location);
        }

        private CsdlCollectionExpression OnCollectionElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = OptionalType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location, Optionality.Optional);
            IEnumerable<CsdlExpressionBase> elementValues = childValues.ValuesOfType<CsdlExpressionBase>();

            return new CsdlCollectionExpression(type, elementValues, element.Location);
        }

        private CsdlLabeledExpression OnLabeledElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            IEnumerable<CsdlExpressionBase> expressions = childValues.ValuesOfType<CsdlExpressionBase>();
            if (expressions.Count() != 1)
            {
                this.ReportError(element.Location, EdmErrorCode.InvalidLabeledElementExpressionIncorrectNumberOfOperands, Edm.Strings.CsdlParser_InvalidLabeledElementExpressionIncorrectNumberOfOperands);
            }

            return new CsdlLabeledExpression(
                name,
                expressions.ElementAtOrDefault(0),
                element.Location);
        }

        private CsdlApplyExpression OnApplyElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string function = Optional(CsdlConstants.Attribute_Function);
            IEnumerable<CsdlExpressionBase> arguments = childValues.ValuesOfType<CsdlExpressionBase>();

            return new CsdlApplyExpression(function, arguments, element.Location);
        }

        private static void AddChildParsers(XmlElementParser parent, IEnumerable<XmlElementParser> children)
        {
            foreach (XmlElementParser child in children)
            {
                parent.AddChildParser(child);
            }
        }

        private static CsdlConstantExpression ConstantExpression(EdmValueKind kind, XmlElementValueCollection childValues, CsdlLocation location)
        {
            XmlTextValue text = childValues.FirstText;
            return new CsdlConstantExpression(kind, text != null ? text.TextValue : string.Empty, location);
        }

        private static CsdlConstantExpression OnIntConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Integer, childValues, element.Location);
        }

        private static CsdlConstantExpression OnStringConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.String, childValues, element.Location);
        }

        private static CsdlConstantExpression OnBinaryConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Binary, childValues, element.Location);
        }

        private static CsdlConstantExpression OnFloatConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Floating, childValues, element.Location);
        }

        private static CsdlConstantExpression OnGuidConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Guid, childValues, element.Location);
        }

        private static CsdlConstantExpression OnDecimalConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Decimal, childValues, element.Location);
        }

        private static CsdlConstantExpression OnBoolConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Boolean, childValues, element.Location);
        }

        private static CsdlConstantExpression OnDurationConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Duration, childValues, element.Location);
        }

        private static CsdlConstantExpression OnDateConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Date, childValues, element.Location);
        }

        private static CsdlConstantExpression OnDateTimeOffsetConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.DateTimeOffset, childValues, element.Location);
        }

        private static CsdlConstantExpression OnTimeOfDayConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.TimeOfDay, childValues, element.Location);
        }

        private static CsdlConstantExpression OnNullConstantExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            return ConstantExpression(EdmValueKind.Null, childValues, element.Location);
        }

        private static CsdlPathExpression OnPathExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            XmlTextValue text = childValues.FirstText;
            return new CsdlPathExpression(text != null ? text.TextValue : string.Empty, element.Location);
        }

        private static CsdlPropertyPathExpression OnPropertyPathExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            XmlTextValue text = childValues.FirstText;
            return new CsdlPropertyPathExpression(text != null ? text.TextValue : string.Empty, element.Location);
        }

        private static CsdlNavigationPropertyPathExpression OnNavigationPropertyPathExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            XmlTextValue text = childValues.FirstText;
            return new CsdlNavigationPropertyPathExpression(text != null ? text.TextValue : string.Empty, element.Location);
        }

        private CsdlLabeledExpressionReferenceExpression OnLabeledElementReferenceExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string name = Required(CsdlConstants.Attribute_Name);
            return new CsdlLabeledExpressionReferenceExpression(name, element.Location);
        }

        private CsdlEnumMemberExpression OnEnumMemberExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string enumMemberPath = this.RequiredEnumMemberPath(childValues.FirstText);
            return new CsdlEnumMemberExpression(enumMemberPath, element.Location);
        }

        private CsdlExpressionBase OnIfExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            IEnumerable<CsdlExpressionBase> expressions = childValues.ValuesOfType<CsdlExpressionBase>();
            if (expressions.Count() != 3)
            {
                this.ReportError(element.Location, EdmErrorCode.InvalidIfExpressionIncorrectNumberOfOperands, Edm.Strings.CsdlParser_InvalidIfExpressionIncorrectNumberOfOperands);
            }

            return new CsdlIfExpression(
                expressions.ElementAtOrDefault(0),
                expressions.ElementAtOrDefault(1),
                expressions.ElementAtOrDefault(2),
                element.Location);
        }

        private CsdlExpressionBase OnCastExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = OptionalType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location, Optionality.Required);

            IEnumerable<CsdlExpressionBase> expressions = childValues.ValuesOfType<CsdlExpressionBase>();
            if (expressions.Count() != 1)
            {
                this.ReportError(element.Location, EdmErrorCode.InvalidCastExpressionIncorrectNumberOfOperands, Edm.Strings.CsdlParser_InvalidCastExpressionIncorrectNumberOfOperands);
            }

            return new CsdlCastExpression(type, expressions.ElementAtOrDefault(0), element.Location);
        }

        private CsdlExpressionBase OnIsTypeExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string typeName = OptionalType(CsdlConstants.Attribute_Type);
            CsdlTypeReference type = this.ParseTypeReference(typeName, childValues, element.Location, Optionality.Required);

            IEnumerable<CsdlExpressionBase> expressions = childValues.ValuesOfType<CsdlExpressionBase>();
            if (expressions.Count() != 1)
            {
                this.ReportError(element.Location, EdmErrorCode.InvalidIsTypeExpressionIncorrectNumberOfOperands, Edm.Strings.CsdlParser_InvalidIsTypeExpressionIncorrectNumberOfOperands);
            }

            return new CsdlIsTypeExpression(type, expressions.ElementAtOrDefault(0), element.Location);
        }

        private CsdlExpressionBase ParseAnnotationExpression(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            CsdlExpressionBase expression = childValues.ValuesOfType<CsdlExpressionBase>().FirstOrDefault();
            if (expression != null)
            {
                return expression;
            }

            string pathValue = Optional(CsdlConstants.Attribute_Path);
            if (pathValue != null)
            {
                return new CsdlPathExpression(pathValue, element.Location);
            }

            string propertyPathValue = Optional(CsdlConstants.Attribute_PropertyPath);
            if (propertyPathValue != null)
            {
                return new CsdlPropertyPathExpression(propertyPathValue, element.Location);
            }

            string navigationPropertyPathValue = Optional(CsdlConstants.Attribute_NavigationPropertyPath);
            if (navigationPropertyPathValue != null)
            {
                return new CsdlNavigationPropertyPathExpression(navigationPropertyPathValue, element.Location);
            }

            string enumMemberValue = Optional(CsdlConstants.Attribute_EnumMember);
            if (enumMemberValue != null)
            {
                return new CsdlEnumMemberExpression(this.ValidateEnumMembersPath(enumMemberValue), element.Location);
            }

            string annotationPath = Optional(CsdlConstants.Attribute_AnnotationPath);
            if (annotationPath != null)
            {
                return new CsdlAnnotationPathExpression(annotationPath, element.Location);
            }

            EdmValueKind kind;

            string value = Optional(CsdlConstants.Attribute_String);
            if (value != null)
            {
                kind = EdmValueKind.String;
            }
            else
            {
                value = Optional(CsdlConstants.Attribute_Bool);
                if (value != null)
                {
                    kind = EdmValueKind.Boolean;
                }
                else
                {
                    value = Optional(CsdlConstants.Attribute_Int);
                    if (value != null)
                    {
                        kind = EdmValueKind.Integer;
                    }
                    else
                    {
                        value = Optional(CsdlConstants.Attribute_Float);
                        if (value != null)
                        {
                            kind = EdmValueKind.Floating;
                        }
                        else
                        {
                            value = Optional(CsdlConstants.Attribute_DateTimeOffset);
                            if (value != null)
                            {
                                kind = EdmValueKind.DateTimeOffset;
                            }
                            else
                            {
                                value = Optional(CsdlConstants.Attribute_Duration);
                                if (value != null)
                                {
                                    kind = EdmValueKind.Duration;
                                }
                                else
                                {
                                    value = Optional(CsdlConstants.Attribute_Decimal);
                                    if (value != null)
                                    {
                                        kind = EdmValueKind.Decimal;
                                    }
                                    else
                                    {
                                        value = Optional(CsdlConstants.Attribute_Binary);
                                        if (value != null)
                                        {
                                            kind = EdmValueKind.Binary;
                                        }
                                        else
                                        {
                                            value = Optional(CsdlConstants.Attribute_Guid);
                                            if (value != null)
                                            {
                                                kind = EdmValueKind.Guid;
                                            }
                                            else
                                            {
                                                value = Optional(CsdlConstants.Attribute_Date);
                                                if (value != null)
                                                {
                                                    kind = EdmValueKind.Date;
                                                }
                                                else
                                                {
                                                    value = Optional(CsdlConstants.Attribute_TimeOfDay);
                                                    if (value != null)
                                                    {
                                                        kind = EdmValueKind.TimeOfDay;
                                                    }
                                                    else
                                                    {
                                                        //// Annotation expressions are always optional.
                                                        return null;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new CsdlConstantExpression(kind, value, element.Location);
        }

        private void ParseMaxLength(out bool Unbounded, out int? maxLength)
        {
            string max = Optional(CsdlConstants.Attribute_MaxLength);
            if (max == null)
            {
                Unbounded = false;
                maxLength = null;
            }
            else if (max.EqualsOrdinalIgnoreCase(CsdlConstants.Value_Max))
            {
                Unbounded = true;
                maxLength = null;
            }
            else
            {
                Unbounded = false;
                maxLength = OptionalMaxLength(CsdlConstants.Attribute_MaxLength);
            }
        }

        private void ParseBinaryFacets(out bool Unbounded, out int? maxLength)
        {
            this.ParseMaxLength(out Unbounded, out maxLength);
        }

        private void ParseDecimalFacets(out int? precision, out int? scale)
        {
            precision = OptionalInteger(CsdlConstants.Attribute_Precision);
            scale = OptionalScale(CsdlConstants.Attribute_Scale);
        }

        private void ParseStringFacets(out bool Unbounded, out int? maxLength, out bool? unicode)
        {
            this.ParseMaxLength(out Unbounded, out maxLength);
            unicode = OptionalBoolean(CsdlConstants.Attribute_Unicode) ?? CsdlConstants.Default_IsUnicode;
        }

        private void ParseTemporalFacets(out int? precision)
        {
            precision = OptionalInteger(CsdlConstants.Attribute_Precision) ?? CsdlConstants.Default_TemporalPrecision;
        }

        private void ParseSpatialFacets(out int? srid, int defaultSrid)
        {
            srid = OptionalSrid(CsdlConstants.Attribute_Srid, defaultSrid);
        }

        private void ParseTypeDefinitionFacets(
            out bool isUnbounded,
            out int? maxLength,
            out bool? unicode,
            out int? precision,
            out int? scale,
            out int? srid)
        {
            this.ParseMaxLength(out isUnbounded, out maxLength);
            unicode = OptionalBoolean(CsdlConstants.Attribute_Unicode);
            precision = OptionalInteger(CsdlConstants.Attribute_Precision);
            scale = OptionalScale(CsdlConstants.Attribute_Scale);
            srid = OptionalSrid(CsdlConstants.Attribute_Srid, CsdlConstants.Default_UnspecifiedSrid);
        }

        protected enum Optionality
        {
            Optional,
            Required
        }
    }
}
