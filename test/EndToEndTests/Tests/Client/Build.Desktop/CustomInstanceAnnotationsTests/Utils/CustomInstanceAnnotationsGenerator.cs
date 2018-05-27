//---------------------------------------------------------------------
// <copyright file="CustomInstanceAnnotationsGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CustomInstanceAnnotationsTests.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Spatial;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWriterDefaultServiceReference;

    public static class CustomInstanceAnnotationsGenerator
    {
        internal static int NextNameIndex = 0;
        private static int next = 0;
        private static Func<Type, string> ResolveName = ServiceDescriptors.ODataWriterService.CreateDataServiceContext(new Uri("http://var1")).ResolveName;

        public static List<ODataValue> Values;
        public static readonly string DuplicateAnnotationName = "duplicate.annotation";

        static CustomInstanceAnnotationsGenerator()
        {
            Values = new List<ODataValue>();
            Values.Add(new ODataNullValue());
            Values.AddRange(Data<Byte[]>.PrimitiveValues);
            Values.AddRange(Data<Boolean>.PrimitiveValues);
            Values.AddRange(Data<Byte>.PrimitiveValues);
            Values.AddRange(Data<DateTimeOffset>.PrimitiveValues);
            Values.AddRange(Data<Decimal>.PrimitiveValues);
            Values.AddRange(Data<Double>.PrimitiveValues);
            Values.AddRange(Data<Int16>.PrimitiveValues);
            Values.AddRange(Data<Int32>.PrimitiveValues);
            Values.AddRange(Data<Int64>.PrimitiveValues);
            Values.AddRange(Data<SByte>.PrimitiveValues);
            Values.AddRange(Data<Single>.PrimitiveValues);
            Values.AddRange(Data<String>.PrimitiveValues);
            Values.AddRange(Data<Geography>.PrimitiveValues);
            Values.AddRange(Data<Geometry>.PrimitiveValues);
            Values.Add(Data<Byte[]>.CollectionValue);
            Values.Add(Data<Boolean>.CollectionValue);
            Values.Add(Data<Byte>.CollectionValue);
            Values.Add(Data<DateTimeOffset>.CollectionValue);
            Values.Add(Data<Decimal>.CollectionValue);
            Values.Add(Data<Double>.CollectionValue);
            Values.Add(Data<Int16>.CollectionValue);
            Values.Add(Data<Int32>.CollectionValue);
            Values.Add(Data<Int64>.CollectionValue);
            Values.Add(Data<SByte>.CollectionValue);
            Values.Add(Data<Single>.CollectionValue);
            Values.Add(Data<String>.CollectionValue);
            Values.Add(Data<Geography>.CollectionValue);
            Values.Add(Data<Geometry>.CollectionValue);
            Values.Add(new ODataPrimitiveValue(true));

            // TODO : Fix #625
            //Values.Add(new ODataComplexValue
            //{
            //    TypeName = ResolveName(typeof(ComplexWithAllPrimitiveTypes)),
            //    Properties = new[] 
            //    { 
            //        new ODataProperty { Name = "Binary", Value = Data<Byte[]>.Values.FirstOrDefault() },
            //        new ODataProperty { Name = "Boolean", Value = Data<Boolean>.Values.FirstOrDefault() },
            //        new ODataProperty { Name = "Byte", Value = Data<Byte>.Values.FirstOrDefault() },
            //        new ODataProperty { Name = "DateTimeOffset", Value = Data<DateTimeOffset>.Values.FirstOrDefault() },
            //        new ODataProperty { Name = "Decimal", Value = Data<Decimal>.Values.FirstOrDefault() },
            //        new ODataProperty { Name = "Double", Value = Data<Double>.Values.FirstOrDefault() },
            //        new ODataProperty { Name = "Int16", Value = Data<Int16>.Values.FirstOrDefault() },
            //        new ODataProperty { Name = "Int32", Value = Data<Int32>.Values.FirstOrDefault()  },
            //        new ODataProperty { Name = "Int64", Value = Data<Int64>.Values.FirstOrDefault()  },
            //        new ODataProperty { Name = "SByte", Value = Data<SByte>.Values.FirstOrDefault() },
            //        new ODataProperty { Name = "String", Value = Data<String>.Values.FirstOrDefault() },
            //        new ODataProperty { Name = "Single", Value = Data<Single>.Values.FirstOrDefault() },
            //        new ODataProperty { Name = "GeographyPoint", Value = Data<Geography>.Values.FirstOrDefault() },
            //        new ODataProperty { Name = "GeometryPoint", Value = Data<Geometry>.Values.FirstOrDefault() },
            //    }
            //});
        }

        private static string GetNextName(string namePrefix = null)
        {
            var name = "index." + NextNameIndex.ToString();
            return (namePrefix == null) ? name : namePrefix + name;
        }

        private static ODataValue GetNextValue()
        {
            return Values[next % Values.Count];
        }

        private static ODataInstanceAnnotation GetNextAnnotation(string namePrefix = null)
        {
            var annotation = new ODataInstanceAnnotation(GetNextName(namePrefix), GetNextValue());
            next++; 
            NextNameIndex++;
            return annotation;
        }

        public static IEnumerable<ODataInstanceAnnotation> GetAnnotations(int count, string namePrefix = null)
        {
            for (int i = 0; i < count; i++)
            {
                yield return GetNextAnnotation(namePrefix);
            }
        }

        public static IEnumerable<ODataInstanceAnnotation> GetAnnotations(string namePrefix = null)
        {
            return GetAnnotations(Values.Count, namePrefix);
        }

        public static ODataInstanceAnnotation[] GetAnnotationsWithTermInMetadata()
        {
            return new[] {new ODataInstanceAnnotation("CustomInstanceAnnotations.Term1", new ODataPrimitiveValue("false"))};
        }

        public static ODataInstanceAnnotation[] GetDuplicateAnnotations()
        {
            return new[] 
            { 
                new ODataInstanceAnnotation(DuplicateAnnotationName, new ODataPrimitiveValue("false")), 
                new ODataInstanceAnnotation(DuplicateAnnotationName, new ODataPrimitiveValue("true")) 
            };
        }
    }
}
