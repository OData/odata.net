﻿//---------------------------------------------------------------------
// <copyright file="Parameterized.Microsoft.Spatial.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
//
//      GENERATED FILE.  DO NOT MODIFY.
//
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial {
    using System;
    using System.Resources;

    /// <summary>
    ///    Strongly-typed and parameterized string resources.
    /// </summary>
    internal static class Strings {
        /// <summary>
        /// A string like "No operations are registered. Please provide operations using SpatialImplementation.CurrentImplementation.Operations property."
        /// </summary>
        internal static string SpatialImplementation_NoRegisteredOperations {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.SpatialImplementation_NoRegisteredOperations);
            }
        }

        /// <summary>
        /// A string like "The value '{0}' is not valid for the coordinate '{1}'."
        /// </summary>
        internal static string InvalidPointCoordinate(object p0, object p1) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.InvalidPointCoordinate, p0, p1);
        }

        /// <summary>
        /// A string like "Access to the coordinate properties of an empty point is not supported."
        /// </summary>
        internal static string Point_AccessCoordinateWhenEmpty {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Point_AccessCoordinateWhenEmpty);
            }
        }

        /// <summary>
        /// A string like "The builder cannot create an instance until all pipeline calls are completed."
        /// </summary>
        internal static string SpatialBuilder_CannotCreateBeforeDrawn {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.SpatialBuilder_CannotCreateBeforeDrawn);
            }
        }

        /// <summary>
        /// A string like "Incorrect GML Format: The XmlReader instance encountered an unexpected element "{0}"."
        /// </summary>
        internal static string GmlReader_UnexpectedElement(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GmlReader_UnexpectedElement, p0);
        }

        /// <summary>
        /// A string like "Incorrect GML Format: the XmlReader instance is expected to be at the start of a GML element."
        /// </summary>
        internal static string GmlReader_ExpectReaderAtElement {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GmlReader_ExpectReaderAtElement);
            }
        }

        /// <summary>
        /// A string like "Incorrect GML Format: unknown spatial type tag "{0}"."
        /// </summary>
        internal static string GmlReader_InvalidSpatialType(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GmlReader_InvalidSpatialType, p0);
        }

        /// <summary>
        /// A string like "Incorrect GML Format: a LinearRing element must not be empty."
        /// </summary>
        internal static string GmlReader_EmptyRingsNotAllowed {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GmlReader_EmptyRingsNotAllowed);
            }
        }

        /// <summary>
        /// A string like "Incorrect GML Format: a pos element must contain at least two coordinates."
        /// </summary>
        internal static string GmlReader_PosNeedTwoNumbers {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GmlReader_PosNeedTwoNumbers);
            }
        }

        /// <summary>
        /// A string like "Incorrect GML Format: a posList element must contain an even number of coordinates."
        /// </summary>
        internal static string GmlReader_PosListNeedsEvenCount {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GmlReader_PosListNeedsEvenCount);
            }
        }

        /// <summary>
        /// A string like "Incorrect GML Format: a srsName attribute must begin with the namespace "{0}"."
        /// </summary>
        internal static string GmlReader_InvalidSrsName(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GmlReader_InvalidSrsName, p0);
        }

        /// <summary>
        /// A string like "The attribute '{0}' on element '{1}' is not supported."
        /// </summary>
        internal static string GmlReader_InvalidAttribute(object p0, object p1) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GmlReader_InvalidAttribute, p0, p1);
        }

        /// <summary>
        /// A string like "Expecting token type "{0}" with text "{1}" but found "{2}"."
        /// </summary>
        internal static string WellKnownText_UnexpectedToken(object p0, object p1, object p2) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.WellKnownText_UnexpectedToken, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Unexpected character '{0}' found in text."
        /// </summary>
        internal static string WellKnownText_UnexpectedCharacter(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.WellKnownText_UnexpectedCharacter, p0);
        }

        /// <summary>
        /// A string like "Unknown Tagged Text "{0}"."
        /// </summary>
        internal static string WellKnownText_UnknownTaggedText(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.WellKnownText_UnknownTaggedText, p0);
        }

        /// <summary>
        /// A string like "The WellKnownTextReader is configured to allow only two dimensions, and a third dimension was encountered."
        /// </summary>
        internal static string WellKnownText_TooManyDimensions {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.WellKnownText_TooManyDimensions);
            }
        }

        /// <summary>
        /// A string like "Invalid spatial data: An instance of spatial type can have only one unique CoordinateSystem for all of its coordinates."
        /// </summary>
        internal static string Validator_SridMismatch {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_SridMismatch);
            }
        }

        /// <summary>
        /// A string like "Invalid spatial data: Invalid spatial type "{0}"."
        /// </summary>
        internal static string Validator_InvalidType(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_InvalidType, p0);
        }

        /// <summary>
        /// A string like "Invalid spatial data: the spatial type "FullGlobe" cannot be part of a collection type."
        /// </summary>
        internal static string Validator_FullGlobeInCollection {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_FullGlobeInCollection);
            }
        }

        /// <summary>
        /// A string like "Invalid spatial data: the spatial type "LineString" must contain at least two points."
        /// </summary>
        internal static string Validator_LineStringNeedsTwoPoints {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_LineStringNeedsTwoPoints);
            }
        }

        /// <summary>
        /// A string like "Invalid spatial data: the spatial type "FullGlobe" cannot contain figures."
        /// </summary>
        internal static string Validator_FullGlobeCannotHaveElements {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_FullGlobeCannotHaveElements);
            }
        }

        /// <summary>
        /// A string like "Invalid spatial data: only {0} levels of nesting are supported in collection types."
        /// </summary>
        internal static string Validator_NestingOverflow(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_NestingOverflow, p0);
        }

        /// <summary>
        /// A string like "Invalid spatial data: the coordinates ({0} {1} {2} {3}) are not valid."
        /// </summary>
        internal static string Validator_InvalidPointCoordinate(object p0, object p1, object p2, object p3) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_InvalidPointCoordinate, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "Invalid spatial data: expected call to "{0}" but got call to "{1}"."
        /// </summary>
        internal static string Validator_UnexpectedCall(object p0, object p1) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_UnexpectedCall, p0, p1);
        }

        /// <summary>
        /// A string like "Invalid spatial data: expected call to "{0}" or "{1}" but got call to "{2}"."
        /// </summary>
        internal static string Validator_UnexpectedCall2(object p0, object p1, object p2) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_UnexpectedCall2, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Invalid spatial data: A polygon ring must contain at least four points, and the last point must be equal to the first point."
        /// </summary>
        internal static string Validator_InvalidPolygonPoints {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_InvalidPolygonPoints);
            }
        }

        /// <summary>
        /// A string like "Invalid latitude coordinate {0}. A latitude coordinate must be a value between -90.0 and +90.0 degrees."
        /// </summary>
        internal static string Validator_InvalidLatitudeCoordinate(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_InvalidLatitudeCoordinate, p0);
        }

        /// <summary>
        /// A string like "Invalid longitude coordinate {0}. A longitude coordinate must be a value between -15069.0 and +15069.0 degrees"
        /// </summary>
        internal static string Validator_InvalidLongitudeCoordinate(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_InvalidLongitudeCoordinate, p0);
        }

        /// <summary>
        /// A string like "A geography operation was called while processing a geometric shape."
        /// </summary>
        internal static string Validator_UnexpectedGeography {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_UnexpectedGeography);
            }
        }

        /// <summary>
        /// A string like "A geometry operation was called while processing a geographic shape."
        /// </summary>
        internal static string Validator_UnexpectedGeometry {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.Validator_UnexpectedGeometry);
            }
        }

        /// <summary>
        /// A string like "Invalid GeoJSON. The '{0}' member is required, but was not found."
        /// </summary>
        internal static string GeoJsonReader_MissingRequiredMember(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GeoJsonReader_MissingRequiredMember, p0);
        }

        /// <summary>
        /// A string like "Invalid GeoJSON. A position must contain at least two and no more than four elements."
        /// </summary>
        internal static string GeoJsonReader_InvalidPosition {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GeoJsonReader_InvalidPosition);
            }
        }

        /// <summary>
        /// A string like "Invalid GeoJSON. The value '{0}' is not a valid value for the 'type' member."
        /// </summary>
        internal static string GeoJsonReader_InvalidTypeName(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GeoJsonReader_InvalidTypeName, p0);
        }

        /// <summary>
        /// A string like "Invalid GeoJSON. A null value was found in an array element where nulls are not allowed."
        /// </summary>
        internal static string GeoJsonReader_InvalidNullElement {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GeoJsonReader_InvalidNullElement);
            }
        }

        /// <summary>
        /// A string like "Invalid GeoJSON. A non-numeric value was found in an array element where a numeric value was expected."
        /// </summary>
        internal static string GeoJsonReader_ExpectedNumeric {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GeoJsonReader_ExpectedNumeric);
            }
        }

        /// <summary>
        /// A string like "Invalid GeoJSON. A primitive value was found in an array element where an array was expected."
        /// </summary>
        internal static string GeoJsonReader_ExpectedArray {
            get {
                return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GeoJsonReader_ExpectedArray);
            }
        }

        /// <summary>
        /// A string like "Invalid GeoJSON. The value '{0}' is not a recognized CRS type."
        /// </summary>
        internal static string GeoJsonReader_InvalidCrsType(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GeoJsonReader_InvalidCrsType, p0);
        }

        /// <summary>
        /// A string like "Invalid GeoJSON. The value '{0}' is not a recognized CRS name."
        /// </summary>
        internal static string GeoJsonReader_InvalidCrsName(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.GeoJsonReader_InvalidCrsName, p0);
        }

        /// <summary>
        /// A string like "Cannot read the value '{0}' for the property '{1}' as a quoted JSON string value."
        /// </summary>
        internal static string JsonReaderExtensions_CannotReadPropertyValueAsString(object p0, object p1) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.JsonReaderExtensions_CannotReadPropertyValueAsString, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot read the value '{0}' as a JSON object."
        /// </summary>
        internal static string JsonReaderExtensions_CannotReadValueAsJsonObject(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.JsonReaderExtensions_CannotReadValueAsJsonObject, p0);
        }

        /// <summary>
        /// A string like "The time zone information is missing on the DateTimeOffset value '{0}'. A DateTimeOffset value must contain the time zone information."
        /// </summary>
        internal static string PlatformHelper_DateTimeOffsetMustContainTimeZone(object p0) {
            return Microsoft.Spatial.TextRes.GetString(Microsoft.Spatial.TextRes.PlatformHelper_DateTimeOffsetMustContainTimeZone, p0);
        }

    }

    /// <summary>
    ///    Strongly-typed and parameterized exception factory.
    /// </summary>
    internal static partial class Error {

        /// <summary>
        /// The exception that is thrown when a null reference (Nothing in Visual Basic) is passed to a method that does not accept it as a valid argument.
        /// </summary>
        internal static Exception ArgumentNull(string paramName) {
            return new ArgumentNullException(paramName);
        }

        /// <summary>
        /// The exception that is thrown when the value of an argument is outside the allowable range of values as defined by the invoked method.
        /// </summary>
        internal static Exception ArgumentOutOfRange(string paramName) {
            return new ArgumentOutOfRangeException(paramName);
        }

        /// <summary>
        /// The exception that is thrown when the author has not yet implemented the logic at this point in the program. This can act as an exception based TODO tag.
        /// </summary>
        internal static Exception NotImplemented() {
            return new NotImplementedException();
        }

        /// <summary>
        /// The exception that is thrown when an invoked method is not supported, or when there is an attempt to read, seek, or write to a stream that does not support the invoked functionality.
        /// </summary>
        internal static Exception NotSupported() {
            return new NotSupportedException();
        }
    }
}
