//---------------------------------------------------------------------
// <copyright file="CsdlMicrobenchmarks.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance.Microbenchmarks
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using BenchmarkDotNet.Attributes;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;

    /// <summary>
    /// CSDL XML read / write roundtrip benchmark for a representative model.
    /// Exercises CSDL parser dispatch (TryGetValue), serializer set.Add, and
    /// XML LINQ / namespace lookup hotspots.
    /// See REPORT.md findings #20, #21, #22.
    /// </summary>
    [MemoryDiagnoser]
    public class CsdlMicrobenchmarks
    {
        private byte[] _csdl;
        private IEdmModel _model;

        [GlobalSetup]
        public void Setup()
        {
            _csdl = TestUtils.ReadTestResource("AdventureWorksPlus.csdl");
            _model = TestUtils.GetAdventureWorksModel();
        }

        [Benchmark]
        public IEdmModel ReadCsdl()
        {
            using var ms = new MemoryStream(_csdl);
            using var xr = XmlReader.Create(ms);
            CsdlReader.TryParse(xr, out IEdmModel model, out IEnumerable<EdmError> _);
            return model;
        }

        [Benchmark]
        public int WriteCsdl()
        {
            using var ms = new MemoryStream(64 * 1024);
            using var xw = XmlWriter.Create(ms);
            CsdlWriter.TryWriteCsdl(_model, xw, CsdlTarget.OData, out _);
            xw.Flush();
            return (int)ms.Length;
        }
    }
}
