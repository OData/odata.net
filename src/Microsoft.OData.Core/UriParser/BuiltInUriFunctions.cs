//---------------------------------------------------------------------
// <copyright file="BuiltInUriFunctions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Class containing definitions of all the built-in functions in OData Protocol.
    /// </summary>
    internal static class BuiltInUriFunctions
    {
        /// <summary>
        /// Dictionary of the name of the built-in function and all the signatures.
        /// </summary>
        private static readonly Dictionary<string, FunctionSignatureWithReturnType[]> builtInFunctions = InitializeBuiltInFunctions();

        /// <summary>
        /// Returns a list of signatures for a function name.
        /// </summary>
        /// <param name="name">The name of the function to look for.</param>
        /// <param name="signatures">The list of signatures available for the function name.</param>
        /// <returns>true if the function was found, or false otherwise.</returns>
        internal static bool TryGetBuiltInFunction(string name, out FunctionSignatureWithReturnType[] signatures)
        {
            Debug.Assert(name != null, "name != null");

            return builtInFunctions.TryGetValue(name, out signatures);
        }

        /// <summary>
        /// Creates all of the spatial functions
        /// </summary>
        /// <param name="functions">Dictionary of functions to add to.</param>
        internal static void CreateSpatialFunctions(IDictionary<string, FunctionSignatureWithReturnType[]> functions)
        {
            // double geo.distance(geographyPoint, geographyPoint)
            // double geo.distance(geometryPoint, geometryPoint)
            FunctionSignatureWithReturnType[] signatures = new FunctionSignatureWithReturnType[]
            {
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDouble(true), 
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true)),
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDouble(true),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true))
            };
            functions.Add("geo.distance", signatures);

            // bool geo.intersects(geometry.Point, geometry.Polygon)
            // bool geo.intersects(geometry.Polygon, geomtery.Point)
            // bool geo.intersects(geography.Point, geography.Polygon)
            // bool geo.intersects(geography.Polygon, geography.Point)
            signatures = new FunctionSignatureWithReturnType[]
            {
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetBoolean(true),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPolygon, true)),
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetBoolean(true),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPolygon, true),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true)),
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetBoolean(true),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, true)),
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetBoolean(true),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, true),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true))
            };
            functions.Add("geo.intersects", signatures);

            // double geo.length(geometryLineString)
            // double geo.length(geographyLineString)
            signatures = new FunctionSignatureWithReturnType[]
            {
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDouble(true),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryLineString, true)),
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDouble(true),
                    EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, true)), 
            };
            functions.Add("geo.length", signatures);
        }

        /// <summary>
        /// Builds the list of all built-in functions.
        /// </summary>
        /// <returns>Returns a dictionary of built in functions.</returns>
        private static Dictionary<string, FunctionSignatureWithReturnType[]> InitializeBuiltInFunctions()
        {
            var functions = new Dictionary<string, FunctionSignatureWithReturnType[]>(StringComparer.Ordinal);
            CreateStringFunctions(functions);
            CreateSpatialFunctions(functions);
            CreateDateTimeFunctions(functions);
            CreateMathFunctions(functions);
            return functions;
        }

        /// <summary>
        /// Creates all string functions.
        /// </summary>
        /// <param name="functions">Dictionary of functions to add to.</param>
        private static void CreateStringFunctions(IDictionary<string, FunctionSignatureWithReturnType[]> functions)
        {
            FunctionSignatureWithReturnType signature;
            FunctionSignatureWithReturnType[] signatures;

            // bool endswith(string, string)
            signature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetBoolean(false),
                EdmCoreModel.Instance.GetString(true),
                EdmCoreModel.Instance.GetString(true));
            functions.Add("endswith", new FunctionSignatureWithReturnType[] { signature });

            // int indexof(string, string)
            signature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetInt32(false),
                EdmCoreModel.Instance.GetString(true),
                EdmCoreModel.Instance.GetString(true));
            functions.Add("indexof", new FunctionSignatureWithReturnType[] { signature });

            // string replace(string, string, string)
            signature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetString(true),
                EdmCoreModel.Instance.GetString(true),
                EdmCoreModel.Instance.GetString(true),
                EdmCoreModel.Instance.GetString(true));
            functions.Add("replace", new FunctionSignatureWithReturnType[] { signature });

            // bool startswith(string, string)
            signature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetBoolean(false),
                EdmCoreModel.Instance.GetString(true),
                EdmCoreModel.Instance.GetString(true));
            functions.Add("startswith", new FunctionSignatureWithReturnType[] { signature });

            // string tolower(string)
            signature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetString(true),
                EdmCoreModel.Instance.GetString(true));
            functions.Add("tolower", new FunctionSignatureWithReturnType[] { signature });

            // string toupper(string)
            signature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetString(true),
                EdmCoreModel.Instance.GetString(true));
            functions.Add("toupper", new FunctionSignatureWithReturnType[] { signature });

            // string trim(string)
            signature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetString(true),
                EdmCoreModel.Instance.GetString(true));
            functions.Add("trim", new FunctionSignatureWithReturnType[] { signature });

            signatures = new FunctionSignatureWithReturnType[] 
            {
                // string substring(string, int)
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetString(true),
                    EdmCoreModel.Instance.GetString(true),
                    EdmCoreModel.Instance.GetInt32(false)), 

                // string substring(string, int?)
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetString(true),
                    EdmCoreModel.Instance.GetString(true),
                    EdmCoreModel.Instance.GetInt32(true)), 

                // string substring(string, int, int)
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetString(true),
                    EdmCoreModel.Instance.GetString(true),
                    EdmCoreModel.Instance.GetInt32(false),
                    EdmCoreModel.Instance.GetInt32(false)), 

                // string substring(string, int?, int)
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetString(true),
                    EdmCoreModel.Instance.GetString(true),
                    EdmCoreModel.Instance.GetInt32(true),
                    EdmCoreModel.Instance.GetInt32(false)), 

                // string substring(string, int, int?)
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetString(true),
                    EdmCoreModel.Instance.GetString(true),
                    EdmCoreModel.Instance.GetInt32(false),
                    EdmCoreModel.Instance.GetInt32(true)), 

                // string substring(string, int?, int?)
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetString(true),
                    EdmCoreModel.Instance.GetString(true),
                    EdmCoreModel.Instance.GetInt32(true),
                    EdmCoreModel.Instance.GetInt32(true)), 
            };
            functions.Add("substring", signatures);

            // bool contains(string, string)
            signature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetBoolean(false),
                EdmCoreModel.Instance.GetString(true),
                EdmCoreModel.Instance.GetString(true));
            functions.Add("contains", new FunctionSignatureWithReturnType[] { signature });

            // string concat(string, string)
            signature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetString(true),
                EdmCoreModel.Instance.GetString(true),
                EdmCoreModel.Instance.GetString(true));
            functions.Add("concat", new FunctionSignatureWithReturnType[] { signature });

            // int length(string)
            signature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetInt32(false),
                EdmCoreModel.Instance.GetString(true));
            functions.Add("length", new FunctionSignatureWithReturnType[] { signature });
        }

        /// <summary>
        /// Creates all date and time functions.
        /// </summary>
        /// <param name="functions">Dictionary of functions to add to.</param>
        private static void CreateDateTimeFunctions(IDictionary<string, FunctionSignatureWithReturnType[]> functions)
        {
            // Basic Signature according to type
            FunctionSignatureWithReturnType[] dateTimeOffsetReturnInt = CreateDateTimeFunctionSignatureArray();
            FunctionSignatureWithReturnType[] dateTimeoffsetReturnDate = CreateDateTimeOffsetReturnDate();
            FunctionSignatureWithReturnType[] dateTimeOffsetReturnDecimal = CreateDateTimeOffsetReturnDecimal();
            FunctionSignatureWithReturnType[] dateTimeoffsetReturnTimeOfDay = CreateDateTimeOffsetReturnTimeOfDay();
            FunctionSignatureWithReturnType[] dateTimeOffsetOrDurationReturnInt = dateTimeOffsetReturnInt.Concat(CreateDurationFunctionSignatures()).ToArray();
            FunctionSignatureWithReturnType[] voidReturnDateTimeOffset = CreateVoidReturnDateTimeOffset();
            FunctionSignatureWithReturnType[] durationReturnDecimal = CreateDurationReturnDecimal();
            FunctionSignatureWithReturnType[] dateReturnInt = CreateDateReturnInt();
            FunctionSignatureWithReturnType[] timeOfDayReturnInt = CreateTimeOfDayReturnInt();
            FunctionSignatureWithReturnType[] timeOfDayReturnDecimal = CreateTimeOfDayReturnDecimal();

            // Signature array according to function name
            FunctionSignatureWithReturnType[] yearMonthDayFunctionSignatures = dateTimeOffsetReturnInt.Concat(dateReturnInt).ToArray();
            FunctionSignatureWithReturnType[] hourMinuteSecondFunctionSignatures = dateTimeOffsetOrDurationReturnInt.Concat(timeOfDayReturnInt).ToArray();
            FunctionSignatureWithReturnType[] fractionalsecondsFunctionSignatures = dateTimeOffsetReturnDecimal.Concat(timeOfDayReturnDecimal).ToArray();

            // int year(DateTimeOffset)
            // int year(DateTimeOffset?)
            // int year(Date)
            // int year(Date?)
            functions.Add("year", yearMonthDayFunctionSignatures);

            // int month(DateTimeOffset)
            // int month(DateTimeOffset?)
            // int month(Date)
            // int month(Date?)
            functions.Add("month", yearMonthDayFunctionSignatures);

            // int day(DateTimeOffset)
            // int day(DateTimeOffset?)
            // int day(Date)
            // int day(Date?)
            functions.Add("day", yearMonthDayFunctionSignatures);

            // int hour(DateTimeOffset)
            // int hour(DateTimeOffset?)
            // int hour(TimeSpan)
            // int hour(TimeSpan?)
            // int hour(TimeOfDay)
            // int hour(TimeOfDay?)
            functions.Add("hour", hourMinuteSecondFunctionSignatures);

            // int minute(DateTimeOffset)
            // int minute(DateTimeOffset?)
            // int minute(TimeSpan)
            // int minute(TimeSpan?)
            // int minute(TimeOfDay)
            // int minute(TimeOfDay?)
            functions.Add("minute", hourMinuteSecondFunctionSignatures);

            // int second(DateTimeOffset)
            // int second(DateTimeOffset?)
            // int second(TimeSpan)
            // int second(TimeSpan?)
            // int second(TimeOfDay)
            // int second(TimeOfDay?)
            functions.Add("second", hourMinuteSecondFunctionSignatures);

            // Use protocol signature
            // Edm.Decimal fractionalseconds(Edm.DateTimeOffset)
            // Edm.Decimal fractionalseconds(Edm.TimeOfDay)
            functions.Add("fractionalseconds", fractionalsecondsFunctionSignatures);

            // Edm.Int32 totaloffsetminutes(Edm.DateTimeOffset)
            functions.Add("totaloffsetminutes", dateTimeOffsetReturnInt);

            // Edm.DateTimeOffset now()
            functions.Add("now", voidReturnDateTimeOffset);

            // Edm.DateTimeOffset maxdatetime()
            functions.Add("maxdatetime", voidReturnDateTimeOffset);

            // Edm.DateTimeOffset mindatetime()
            functions.Add("mindatetime", voidReturnDateTimeOffset);

            // Edm.Decimal totalseconds(Edm.Duration)
            functions.Add("totalseconds", durationReturnDecimal);

            // Edm.Date date(Edm.DateTimeOffset)
            functions.Add("date", dateTimeoffsetReturnDate);

            // Edm.TimeOfDay time(Edm.DateTimeOffset)
            functions.Add("time", dateTimeoffsetReturnTimeOfDay);
        }

        /// <summary>
        /// Builds an array of signatures for date time functions.
        /// </summary>
        /// <returns>The array of signatures for a date time functions.</returns>
        private static FunctionSignatureWithReturnType[] CreateDateTimeFunctionSignatureArray()
        {
            FunctionSignatureWithReturnType dateTimeOffsetNonNullable = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetInt32(false),
                EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false));

            FunctionSignatureWithReturnType dateTimeOffsetNullable = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetInt32(false),
                EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, true));

            return new[] { dateTimeOffsetNonNullable, dateTimeOffsetNullable };
        }

        /// <summary>
        /// Builds the set of signatures for duration functions.
        /// </summary>
        /// <returns>The set of signatures for duration functions.</returns>
        private static IEnumerable<FunctionSignatureWithReturnType> CreateDurationFunctionSignatures()
        {
            yield return new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetInt32(false),
                EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, false));

            yield return new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetInt32(false),
                EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, true));
        }

        /// <summary>
        /// Builds the set of signatures for 'DateTimeOffset function()'.
        /// </summary>
        /// <returns>The set of signatures for 'DateTimeOffset function()'.</returns>
        private static FunctionSignatureWithReturnType[] CreateVoidReturnDateTimeOffset()
        {
            return new[]
            {
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)),
            };
        }

        /// <summary>
        /// Builds the set of signatures for 'Date function(DateTimeOffset)'.
        /// </summary>
        /// <returns>The set of signatures for 'Date function(DateTimeOffset)'.</returns>
        private static FunctionSignatureWithReturnType[] CreateDateTimeOffsetReturnDate()
        {
            return new[]
            {
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDate(false),
                    EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)),
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDate(false),
                    EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, true)),
            };
        }

        /// <summary>
        /// Builds the set of signatures for 'Decimal function(DateTimeOffset)'.
        /// </summary>
        /// <returns>The set of signatures for 'Decimal function(DateTimeOffset)'.</returns>
        private static FunctionSignatureWithReturnType[] CreateDateTimeOffsetReturnDecimal()
        {
            return new[]
            {
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDecimal(false),
                    EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)),
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDecimal(false),
                    EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, true)),
            };
        }

        /// <summary>
        /// Builds the set of signatures for 'TimeOfDay function(DateTimeOffset)'.
        /// </summary>
        /// <returns>The set of signatures for 'TimeOfDay function(DateTimeOffset)'.</returns>
        private static FunctionSignatureWithReturnType[] CreateDateTimeOffsetReturnTimeOfDay()
        {
            return new[]
            {
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, false),
                    EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)),
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, false),
                    EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, true)),
            };
        }

        /// <summary>
        /// Builds the set of signatures for 'Decimal function(Duration)'.
        /// </summary>
        /// <returns>The set of signatures for 'Decimal function(Duration)'.</returns>
        private static FunctionSignatureWithReturnType[] CreateDurationReturnDecimal()
        {
            return new[]
            {
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDecimal(false),
                    EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, false)),
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDecimal(false),
                    EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, true)),
            };
        }

        /// <summary>
        /// Builds the set of signatures for 'Int function(Date)'.
        /// </summary>
        /// <returns>The set of signatures for 'Int function(Date)'.</returns>
        private static FunctionSignatureWithReturnType[] CreateDateReturnInt()
        {
            return new[]
            {
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetInt32(false), 
                    EdmCoreModel.Instance.GetDate(false)), 
                 new FunctionSignatureWithReturnType(
                     EdmCoreModel.Instance.GetInt32(false), 
                     EdmCoreModel.Instance.GetDate(true)),
            };
        }

        /// <summary>
        /// Builds the set of signatures for 'Int function(TimeOfDay)'.
        /// </summary>
        /// <returns>The set of signatures for 'Int function(TimeOfDay)'.</returns>
        private static FunctionSignatureWithReturnType[] CreateTimeOfDayReturnInt()
        {
            return new[]
            {
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetInt32(false), 
                    EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, false)),
                 new FunctionSignatureWithReturnType(
                     EdmCoreModel.Instance.GetInt32(false), 
                     EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, true)),
            };
        }

        /// <summary>
        /// Builds the set of signatures for 'Decimal function(TimeOfDay)'.
        /// </summary>
        /// <returns>The set of signatures for 'Decimal function(TimeOfDay)'.</returns>
        private static FunctionSignatureWithReturnType[] CreateTimeOfDayReturnDecimal()
        {
            return new[]
            {
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDecimal(false), 
                    EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, false)),
                 new FunctionSignatureWithReturnType(
                     EdmCoreModel.Instance.GetDecimal(false), 
                     EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, true)),
            };
        }

        /// <summary>
        /// Creates all math functions.
        /// </summary>
        /// <param name="functions">Dictionary of functions to add to.</param>
        private static void CreateMathFunctions(IDictionary<string, FunctionSignatureWithReturnType[]> functions)
        {
            // double round(double)
            // decimal round(decimal)
            // double round(double?)
            // decimal round(decimal?)
            functions.Add("round", CreateMathFunctionSignatureArray());

            // double floor(double)
            // decimal floor(decimal)
            // double floor(double?)
            // decimal floor(decimal?)
            functions.Add("floor", CreateMathFunctionSignatureArray());

            // double ceiling(double)
            // decimal ceiling(decimal)
            // double ceiling(double?)
            // decimal ceiling(decimal?)
            functions.Add("ceiling", CreateMathFunctionSignatureArray());
        }

        /// <summary>
        /// Builds an array of signatures for math functions.
        /// </summary>
        /// <returns>The array of signatures for math functions.</returns>
        private static FunctionSignatureWithReturnType[] CreateMathFunctionSignatureArray()
        {
            FunctionSignatureWithReturnType doubleSignature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetDouble(false),
                EdmCoreModel.Instance.GetDouble(false));
            FunctionSignatureWithReturnType nullableDoubleSignature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetDouble(false),
                EdmCoreModel.Instance.GetDouble(true));
            FunctionSignatureWithReturnType decimalSignature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetDecimal(false),
                EdmCoreModel.Instance.GetDecimal(false));
            FunctionSignatureWithReturnType nullableDecimalSignature = new FunctionSignatureWithReturnType(
                EdmCoreModel.Instance.GetDecimal(false),
                EdmCoreModel.Instance.GetDecimal(true));

            return new FunctionSignatureWithReturnType[] { doubleSignature, decimalSignature, nullableDoubleSignature, nullableDecimalSignature };
        }
    }
}