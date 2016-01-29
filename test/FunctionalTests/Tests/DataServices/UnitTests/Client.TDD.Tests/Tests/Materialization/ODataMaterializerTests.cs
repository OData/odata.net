//---------------------------------------------------------------------
// <copyright file="ODataMaterializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public partial class ODataMaterializerTests
    {
        private const string ServiceUri = "http://localhost/foo/";
        private const string Namespace = "NS";

        private DefaultContainer dsc;

        [TestInitialize]
        public void TestInitialize()
        {
            this.dsc = new DefaultContainer(new Uri(ServiceUri));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.dsc = null;
        }

        #region Expected collection of Non-Abstract type, return collection of Non-Abstract type

        [TestMethod]
        public void MaterializeColOfNonAbstractComplexByFunctionImport()
        {
            MaterializeColOfBaseComplex("Collection(NS.BaseCT)", () => this.dsc.RColOfBaseCT().ToList());
            MaterializeColOfNullableBaseComplex("Collection(NS.BaseCT)", () => this.dsc.RColOfNullableBaseCT().ToList());
            MaterializeColOfDerivedComplex("Collection(NS.CT)", () => this.dsc.RColOfDerivedCT().ToList());
        }

        [TestMethod]
        public void MaterializeColOfNonAbstractComplexProperty()
        {
            MaterializeColOfBaseComplex("BaseETs(0)/EColOfBaseCTP", () => this.dsc.BaseETs.ByKey(0).Select(e => e.EColOfBaseCTP).GetValue().ToList());
            MaterializeColOfNullableBaseComplex("BaseETs(0)/EColOfNullableBaseCTP", () => this.dsc.BaseETs.ByKey(0).Select(e => e.EColOfNullableBaseCTP).GetValue().ToList());
            MaterializeColOfDerivedComplex("DerivedETs(0)/EColOfDerviedCTP", () => this.dsc.DerivedETs.ByKey(0).Select(e => e.EColOfDerivedCTP).GetValue().ToList());
        }

        [TestMethod]
        public void MaterializeColOfNonAbstractComplexByBoundFunctionContextUriIsColOfBaseComplex()
        {
            MaterializeColOfBaseComplex("Collection(NS.BaseCT)", () => this.dsc.DerivedETs.ByKey(0).BETRColOfBaseCT().ToList());
            MaterializeColOfNullableBaseComplex("Collection(NS.BaseCT)", () => this.dsc.DerivedETs.ByKey(0).BETRColOfNullableBaseCT().ToList());
            MaterializeColOfDerivedComplex("Collection(NS.CT)", () => this.dsc.DerivedETs.ByKey(0).BETRColOfDerivedCT().ToList());
        }

        [TestMethod]
        public void MaterializeColOfNonAbstractComplexByBoundFunctionContextUriIsProperty()
        {
            MaterializeColOfBaseComplex("BaseETs(0)/EColOfBaseCTP", () => this.dsc.DerivedETs.ByKey(0).BETRColOfBaseCT().ToList());
            MaterializeColOfBaseComplex("BaseETs(0)/EColOfNullableBaseCTP", () => this.dsc.DerivedETs.ByKey(0).BETRColOfNullableBaseCT().ToList());
            MaterializeColOfDerivedComplex("DerivedETs(0)/EColOfDerviedCTP", () => this.dsc.DerivedETs.ByKey(0).BETRColOfDerivedCT().ToList());
        }

        private void MaterializeColOfBaseComplex(string contextUri, Func<List<BaseCT>> func)
        {
            var response = @"
{
    ""@odata.context"":""http://localhost/foo/$metadata#Collection(NS.BaseCT)"",
    ""value"":
    [
        {
            ""CP0"":1,
            ""CP1"":2
        },
        {
            ""CP0"":3
        },
        {
            ""CP1"":4
        }
	]
}";
            if (!string.IsNullOrEmpty(contextUri))
            {
                response = response.Replace("Collection(NS.BaseCT)", contextUri);
            }

            SetResponse(response);

            var cts = func();
            Assert.AreEqual(3, cts.Count);

            Assert.AreEqual(1, cts.ElementAt(0).CP0);
            Assert.AreEqual(2, cts.ElementAt(0).CP1);

            Assert.AreEqual(3, cts.ElementAt(1).CP0);
            Assert.AreEqual(0, cts.ElementAt(1).CP1);

            Assert.AreEqual(0, cts.ElementAt(2).CP0);
            Assert.AreEqual(4, cts.ElementAt(2).CP1);
        }

        private void MaterializeColOfNullableBaseComplex(string contextUri, Func<List<BaseCT>> func)
        {
            var response = @"
{
    ""@odata.context"":""http://localhost/foo/$metadata#Collection(NS.BaseCT)"",
    ""value"":
    [
        {
            ""CP0"":1,
            ""CP1"":2
        },
        null
	]
}";
            if (!string.IsNullOrEmpty(contextUri))
            {
                response = response.Replace("Collection(NS.BaseCT)", contextUri);
            }

            SetResponse(response);

            var cts = func();
            Assert.AreEqual(2, cts.Count);

            Assert.AreEqual(1, cts.ElementAt(0).CP0);
            Assert.AreEqual(2, cts.ElementAt(0).CP1);

            Assert.IsNull(cts.ElementAt(1));
        }

        private void MaterializeColOfDerivedComplex(string contextUri, Func<List<CT>> func)
        {
            var response = @"
{
    ""@odata.context"":""http://localhost/foo/$metadata#Collection(NS.CT)"",
    ""value"":
    [
        {
            ""CP0"":1,
            ""CP1"":2,
            ""CP2"":3
        },
        {
            ""CP0"":4,
            ""CP1"":5
        },
        {
            ""CP2"":6            
        }
	]
}";
            if (!string.IsNullOrEmpty(contextUri))
            {
                response = response.Replace("Collection(NS.CT)", contextUri);
            }

            SetResponse(response);

            var cts = func();

            Assert.AreEqual(3, cts.Count);
            Assert.AreEqual(1, cts.ElementAt(0).CP0);
            Assert.AreEqual(2, cts.ElementAt(0).CP1);
            Assert.AreEqual(3, cts.ElementAt(0).CP2);

            Assert.AreEqual(4, cts.ElementAt(1).CP0);
            Assert.AreEqual(5, cts.ElementAt(1).CP1);
            Assert.AreEqual(0, cts.ElementAt(1).CP2);

            Assert.AreEqual(0, cts.ElementAt(2).CP0);
            Assert.AreEqual(0, cts.ElementAt(2).CP1);
            Assert.AreEqual(6, cts.ElementAt(2).CP2);
        }

        #endregion

        #region Expected collection of the ancestor type, return the contextUri with collection of ancestor type, but the real object might be sub type

        [TestMethod]
        public void MaterializeColOfAncestorComplexByFunctionImport_ReturnSubtype()
        {
            MaterializeColOfComplex("Collection(NS.AbstractCT)", () => this.dsc.RColOfAbsCT().ToList());
            MaterializeColOfComplex("Collection(NS.AbstractBaseCT)", () => this.dsc.RColOfAbsBaseCT().ToList());
            MaterializeColOfComplex("Collection(NS.BaseCT)", () => this.dsc.RColOfBaseCT().ToList());
            MaterializeColOfNullableComplex("Collection(NS.AbstractCT)", () => this.dsc.RColOfNullableAbsCT().ToList());
            MaterializeColOfNullableComplex("Collection(NS.BaseCT)", () => this.dsc.RColOfNullableBaseCT().ToList());
        }

        [TestMethod]
        public void MaterializeColOfAncestorComplexProperty_ReturnSubtype()
        {
            MaterializeColOfComplex("BaseETs(0)/EColOfAbsCTP", () => this.dsc.BaseETs.ByKey(0).Select(e => e.EColOfAbsCTP).GetValue().ToList());
            MaterializeColOfComplex("BaseETs(0)/EColOfAbsBaseCTP", () => this.dsc.BaseETs.ByKey(0).Select(e => e.EColOfAbsBaseCTP).GetValue().ToList());
            MaterializeColOfComplex("BaseETs(0)/EColOfBaseCTP", () => this.dsc.BaseETs.ByKey(0).Select(e => e.EColOfBaseCTP).GetValue().ToList());
            MaterializeColOfNullableComplex("BaseETs(0)/EColOfNullableAbsCTP", () => this.dsc.BaseETs.ByKey(0).Select(e => e.EColOfNullableAbsCTP).GetValue().ToList());
            MaterializeColOfNullableComplex("BaseETs(0)/EColOfNullableAbsBaseCTP", () => this.dsc.BaseETs.ByKey(0).Select(e => e.EColOfNullableAbsBaseCTP).GetValue().ToList());
            MaterializeColOfNullableComplex("DerivedETs(0)/EColOfNullableBaseCTP", () => this.dsc.DerivedETs.ByKey(0).Select(e => e.EColOfNullableBaseCTP).GetValue().ToList());
        }

        [TestMethod]
        public void MaterializeColOfAncestorComplexByBoundFunctionContextUriIsColOfAncestorComplex_ReturnSubtype()
        {
            MaterializeColOfComplex("Collection(NS.AbstractCT)", () => this.dsc.BaseETs.ByKey(0).BETRColOfAbsCT().ToList());
            MaterializeColOfComplex("Collection(NS.AbstractBaseCT)", () => this.dsc.BaseETs.ByKey(0).BETRColOfAbsBaseCT().ToList());
            MaterializeColOfComplex("Collection(NS.BaseCT)", () => this.dsc.DerivedETs.ByKey(0).BETRColOfBaseCT().ToList());
            MaterializeColOfNullableComplex("Collection(NS.AbstractCT)", () => this.dsc.BaseETs.ByKey(0).BETRColOfNullableAbsCT().ToList());
            MaterializeColOfNullableComplex("Collection(NS.BaseCT)", () => this.dsc.DerivedETs.ByKey(0).BETRColOfNullableBaseCT().ToList());
        }

        [TestMethod]
        public void MaterializeColOfAncestorComplexByBoundFunctionContextUriIsProperty_ReturnSubtype()
        {
            MaterializeColOfComplex("BaseETs(0)/EColOfAbsCTP", () => this.dsc.BaseETs.ByKey(0).BETRColOfAbsCT().ToList());
            MaterializeColOfComplex("BaseETs(0)/EColOfAbsBaseCTP", () => this.dsc.BaseETs.ByKey(0).BETRColOfAbsBaseCT().ToList());
            MaterializeColOfComplex("DerivedETs(0)/EColOfBaseCTP", () => this.dsc.DerivedETs.ByKey(0).BETRColOfBaseCT().ToList());
            MaterializeColOfComplex("BaseETs(0)/EColOfNullableAbsCTP", () => this.dsc.BaseETs.ByKey(0).BETRColOfNullableAbsCT().ToList());
            MaterializeColOfComplex("DerivedETs(0)/EColOfNullableBaseCTP", () => this.dsc.DerivedETs.ByKey(0).BETRColOfNullableBaseCT().ToList());
        }

        private void MaterializeColOfComplex<T>(string contextUri, Func<List<T>> func)
        {
            var response = @"
{
    ""@odata.context"":""http://localhost/foo/$metadata#Collection(NS.AbstractBaseCT)"",
    ""value"":
    [
        {
            ""@odata.type"":""#NS.BaseCT"",
            ""CP0"":1,
            ""CP1"":2
        },
        {
            ""@odata.type"":""#NS.CT"",
            ""CP1"":3,
            ""CP2"":4
        }
	]
}";

            response = response.Replace("Collection(NS.AbstractBaseCT)", contextUri);
            if (contextUri == "Collection(NS.BaseCT)")
            {
                response = response.Replace(@"""@odata.type"":""#NS.BaseCT"",", string.Empty);
            }

            SetResponse(response);

            List<T> cts = func();
            Assert.AreEqual(2, cts.Count);

            var e0 = cts.ElementAt(0) as BaseCT;
            Assert.IsNotNull(e0);
            Assert.AreEqual(1, e0.CP0);
            Assert.AreEqual(2, e0.CP1);

            var e1 = cts.ElementAt(1) as CT;
            Assert.IsNotNull(e1);
            Assert.AreEqual(0, e1.CP0);
            Assert.AreEqual(3, e1.CP1);
            Assert.AreEqual(4, e1.CP2);
        }

        private void MaterializeColOfNullableComplex<T>(string contextUri, Func<List<T>> func)
        {
            var response = @"
{
    ""@odata.context"":""http://localhost/foo/$metadata#Collection(NS.AbstractBaseCT)"",
    ""value"":
    [
        {
            ""@odata.type"":""#NS.BaseCT"",
            ""CP0"":1,
            ""CP1"":2
        },
        {
            ""@odata.type"":""#NS.CT"",
            ""CP1"":3,
            ""CP2"":4
        },
        null
	]
}";

            response = response.Replace("Collection(NS.AbstractBaseCT)", contextUri);
            if (contextUri == "Collection(NS.BaseCT)")
            {
                response = response.Replace(@"""@odata.type"":""#NS.BaseCT"",", string.Empty);
            }

            SetResponse(response);

            List<T> cts = func();
            Assert.AreEqual(3, cts.Count);

            var e0 = cts.ElementAt(0) as BaseCT;
            Assert.IsNotNull(e0);
            Assert.AreEqual(1, e0.CP0);
            Assert.AreEqual(2, e0.CP1);

            var e1 = cts.ElementAt(1) as CT;
            Assert.IsNotNull(e1);
            Assert.AreEqual(0, e1.CP0);
            Assert.AreEqual(3, e1.CP1);
            Assert.AreEqual(4, e1.CP2);

            Assert.IsNull(cts.ElementAt(2));
        }

        #endregion

        #region Expected collection of the ancestor Type, return the contextUri with collection of sub type

        [TestMethod]
        public void MaterializeColOfAncestorComplexByFunctionImportContextUriIsColOfSubtype()
        {
            MaterializeColOfAncestorTypeFromColOfBaseComplex("Collection(NS.BaseCT)", () => this.dsc.RColOfAbsCT().ToList());
            MaterializeColOfAncestorTypeFromColOfBaseComplex("Collection(NS.BaseCT)", () => this.dsc.RColOfAbsBaseCT().ToList());
            MaterializeColOfAncestorTypeActualColOfDerivedComplex("Collection(NS.CT)", () => this.dsc.RColOfAbsBaseCT().ToList());
            MaterializeColOfAncestorTypeActualColOfDerivedComplex("Collection(NS.CT)", () => this.dsc.RColOfBaseCT().ToList());
        }

        [TestMethod]
        public void MaterializeColOfAncestorComplexByBoundFunctionContextUriIsColOfSubtype()
        {
            MaterializeColOfAncestorTypeFromColOfBaseComplex("Collection(NS.BaseCT)", () => this.dsc.BaseETs.ByKey(0).BETRColOfAbsCT().ToList());
            MaterializeColOfAncestorTypeFromColOfBaseComplex("Collection(NS.BaseCT)", () => this.dsc.BaseETs.ByKey(0).BETRColOfAbsBaseCT().ToList());
            MaterializeColOfAncestorTypeActualColOfDerivedComplex("Collection(NS.CT)", () => this.dsc.BaseETs.ByKey(0).BETRColOfAbsBaseCT().ToList());
            MaterializeColOfAncestorTypeActualColOfDerivedComplex("Collection(NS.CT)", () => this.dsc.DerivedETs.ByKey(0).BETRColOfBaseCT().ToList());
        }

        [TestMethod]
        public void MaterializeColOfAncestorComplexByBoundFunctionContextUriIsPropertyOfColOfSubtype()
        {
            MaterializeColOfAncestorTypeFromColOfBaseComplex("BaseETs(0)/EColOfBaseCTP", () => this.dsc.BaseETs.ByKey(0).BETRColOfAbsCT().ToList());
            MaterializeColOfAncestorTypeFromColOfBaseComplex("BaseETs(0)/EColOfBaseCTP", () => this.dsc.BaseETs.ByKey(0).BETRColOfAbsBaseCT().ToList());
            MaterializeColOfAncestorTypeActualColOfDerivedComplex("DerivedETs(0)/EColOfDerivedCTP", () => this.dsc.DerivedETs.ByKey(0).BETRColOfAbsBaseCT().ToList());
            MaterializeColOfAncestorTypeActualColOfDerivedComplex("DerivedETs(0)/EColOfDerivedCTP", () => this.dsc.DerivedETs.ByKey(0).BETRColOfBaseCT().ToList());
        }

        private void MaterializeColOfAncestorTypeFromColOfBaseComplex<T>(string contextUri, Func<List<T>> func)
        {
            var response = @"
{
    ""@odata.context"":""http://localhost/foo/$metadata#Collection(NS.BaseCT)"",
    ""value"":
    [
        {
            ""CP0"":1,
            ""CP1"":2
        },
        {
            ""@odata.type"":""#NS.CT"",
            ""CP1"":3,
            ""CP2"":4
        }
	]
}";
            if (!string.IsNullOrEmpty(contextUri))
            {
                response = response.Replace("Collection(NS.BaseCT)", contextUri);
            }

            SetResponse(response);

            List<T> cts = func();
            Assert.AreEqual(2, cts.Count);

            BaseCT e0 = cts.ElementAt(0) as BaseCT;
            Assert.IsNotNull(e0);
            Assert.AreEqual(1, e0.CP0);
            Assert.AreEqual(2, e0.CP1);

            CT e1 = cts.ElementAt(1) as CT;
            Assert.IsNotNull(e1);
            Assert.AreEqual(0, e1.CP0);
            Assert.AreEqual(3, e1.CP1);
            Assert.AreEqual(4, e1.CP2);
        }

        private void MaterializeColOfAncestorTypeActualColOfDerivedComplex<T>(string contextUri, Func<List<T>> func)
        {
            var response = @"
{
    ""@odata.context"":""http://localhost/foo/$metadata#Collection(NS.CT)"",
    ""value"":
    [
        {
            ""CP0"":1,
            ""CP1"":2
        },
        {
            ""CP1"":3,
            ""CP2"":4
        }
	]
}";
            if (!string.IsNullOrEmpty(contextUri))
            {
                response = response.Replace("Collection(NS.CT)", contextUri);
            }

            SetResponse(response);

            List<T> cts = func();
            Assert.AreEqual(2, cts.Count);

            CT e0 = cts.ElementAt(0) as CT;
            Assert.IsNotNull(e0);
            Assert.AreEqual(1, e0.CP0);
            Assert.AreEqual(2, e0.CP1);
            Assert.AreEqual(0, e0.CP2);

            CT e1 = cts.ElementAt(1) as CT;
            Assert.IsNotNull(e1);
            Assert.AreEqual(0, e1.CP0);
            Assert.AreEqual(3, e1.CP1);
            Assert.AreEqual(4, e1.CP2);
        }

        #endregion

        #region Expected collection of primitive type, or collection of nullable primitive type.

        [TestMethod]
        public void MaterializeColOfPrimitiveByFunctionImport()
        {
            MaterializeColOfPrimitive("Collection(Edm.Int32)", () => this.dsc.RColOfInt().ToList());
            MaterializeColOfNullablePrimitive("Collection(Edm.Int32)", () => this.dsc.RColOfNullableInt().ToList());
        }

        [TestMethod]
        public void MaterializeColOfPrimitiveProperty()
        {
            MaterializeColOfPrimitive("BaseETs(0)/EColOfInt", () => this.dsc.BaseETs.ByKey(0).Select(e => e.EColOfInt).GetValue().ToList());
            MaterializeColOfNullablePrimitive("BaseETs(0)/EColOfNullableInt", () => this.dsc.BaseETs.ByKey(0).Select(e => e.EColOfNullableInt).GetValue().ToList());
        }

        [TestMethod]
        public void MaterializeColOfPrimitiveByBoundFunctionContextUriIsColOfPrimitive()
        {
            MaterializeColOfPrimitive("Collection(Edm.Int32)", () => this.dsc.DerivedETs.ByKey(0).BETRColOfInt().ToList());
            MaterializeColOfNullablePrimitive("Collection(Edm.Int32)", () => this.dsc.DerivedETs.ByKey(0).BETRColOfNullableInt().ToList());
        }

        [TestMethod]
        public void MaterializeColOfPrimitiveByBoundFunctionContextUriIsProperty()
        {
            MaterializeColOfPrimitive("DerivedETs(0)/EColOfInt", () => this.dsc.DerivedETs.ByKey(0).BETRColOfInt().ToList());
            MaterializeColOfNullablePrimitive("DerivedETs(0)/EColOfNullableInt", () => this.dsc.DerivedETs.ByKey(0).BETRColOfNullableInt().ToList());
        }

        private void MaterializeColOfPrimitive(string contextUri, Func<List<Int32>> func)
        {
            var response = @"
{
    ""@odata.context"":""http://localhost/foo/$metadata#Collection(Edm.Int32)"",
    ""value"":
    [
        0,
        1,
        2
	]
}";
            if (!string.IsNullOrEmpty(contextUri))
            {
                response = response.Replace("Collection(Edm.Int32)", contextUri);
            }

            SetResponse(response);

            var cts = func();
            Assert.AreEqual(3, cts.Count);

            Assert.AreEqual(0, cts.ElementAt(0));
            Assert.AreEqual(1, cts.ElementAt(1));
            Assert.AreEqual(2, cts.ElementAt(2));
        }

        private void MaterializeColOfNullablePrimitive(string contextUri, Func<List<Int32?>> func)
        {
            var response = @"
{
    ""@odata.context"":""http://localhost/foo/$metadata#Collection(Edm.Int32)"",
    ""value"":
    [
        0,
        null,
        2
	]
}";
            if (!string.IsNullOrEmpty(contextUri))
            {
                response = response.Replace("Collection(Edm.Int32)", contextUri);
            }

            SetResponse(response);

            var cts = func();
            Assert.AreEqual(3, cts.Count);

            Assert.AreEqual(0, cts.ElementAt(0));
            Assert.IsNull(cts.ElementAt(1));
            Assert.AreEqual(2, cts.ElementAt(2));
        }

        #endregion

        #region Expected collection of enum type, or collection of nullable enum type, or nullable enum

        [TestMethod]
        public void MaterializeColOfEnumProperty()
        {
            MaterializeColOfEnum("BaseETs(0)/EColOfEnum", () => this.dsc.BaseETs.ByKey(0).Select(e => e.EColOfEnum).GetValue().ToList());
            MaterializeColOfNullableEnum("DerivedETs(0)/EColOfNullableEnum", () => this.dsc.DerivedETs.ByKey(0).Select(e => e.EColOfNullableEnum).GetValue().ToList());
        }

        [TestMethod]
        public void MaterializeColOfEnumByBoundFunctionContextUriIsColOfEnum()
        {
            MaterializeColOfEnum("Collection(NS.EnumT)", () => this.dsc.DerivedETs.ByKey(0).BETRColOfEnum().ToList());
            MaterializeColOfNullableEnum("Collection(NS.EnumT)", () => this.dsc.DerivedETs.ByKey(0).BETRColOfNullableEnum().ToList());
        }

        [TestMethod]
        public void MaterializeColOfEnumByBoundFunctionContextUriIsProperty()
        {
            MaterializeColOfEnum("DerivedETs(0)/EColOfEnum", () => this.dsc.DerivedETs.ByKey(0).BETRColOfEnum().ToList());
            MaterializeColOfNullableEnum("DerivedETs(0)/EColOfNullableEnum", () => this.dsc.DerivedETs.ByKey(0).BETRColOfNullableEnum().ToList());
        }

        [TestMethod]
        public void MaterializeEnumProperty()
        {
            MaterializeNullableEnum("BaseETs(0)/ENullableEnum", () => this.dsc.BaseETs.ByKey(0).Select(e => e.ENullableEnum).GetValue());
        }

        [TestMethod]
        public void MaterializeEnumByBoundFunctionContextUriIsEnum()
        {
            MaterializeNullableEnum("NS.EnumT", () => this.dsc.DerivedETs.ByKey(0).BETRNullableEnum().GetValue());
        }

        [TestMethod]
        public void MaterializeEnumByBoundFunctionContextUriIsProperty()
        {
            MaterializeNullableEnum("DerivedETs(0)/ENullableEnum", () => this.dsc.DerivedETs.ByKey(0).BETRNullableEnum().GetValue());
        }

        private void MaterializeColOfEnum(string contextUri, Func<List<EnumT>> func)
        {
            var response = @"
{
    ""@odata.context"":""http://localhost/foo/$metadata#Collection(NS.EnumT)"",
    ""value"":
    [
        ""EnumP1"",
        ""EnumP2""
	]
}";
            if (!string.IsNullOrEmpty(contextUri))
            {
                response = response.Replace("Collection(NS.EnumT)", contextUri);
            }

            SetResponse(response);

            var ets = func();
            Assert.AreEqual(2, ets.Count);

            Assert.AreEqual(EnumT.EnumP1, ets.ElementAt(0));
            Assert.AreEqual(EnumT.EnumP2, ets.ElementAt(1));
        }

        private void MaterializeColOfNullableEnum(string contextUri, Func<List<EnumT?>> func)
        {
            var response = @"
{
    ""@odata.context"":""http://localhost/foo/$metadata#Collection(NS.EnumT)"",
    ""value"":
    [
        ""EnumP1"",
        null,
        ""EnumP2""
	]
}";
            if (!string.IsNullOrEmpty(contextUri))
            {
                response = response.Replace("Collection(NS.EnumT)", contextUri);
            }

            SetResponse(response);

            var ets = func();
            Assert.AreEqual(3, ets.Count);

            Assert.AreEqual(EnumT.EnumP1, ets.ElementAt(0));
            Assert.IsNull(ets.ElementAt(1));
            Assert.AreEqual(EnumT.EnumP2, ets.ElementAt(2));
        }

        private void MaterializeNullableEnum(string contextUri, Func<EnumT?> func)
        {
            var response = @"
{
    ""@odata.context"":""http://localhost/foo/$metadata#NS.EnumT"",
    ""value"": ""EnumP1""
}";
            if (!string.IsNullOrEmpty(contextUri))
            {
                response = response.Replace("NS.EnumT", contextUri);
            }

            SetResponse(response);

            var enumP = func();
            Assert.AreEqual(EnumT.EnumP1, enumP);
        }

        #endregion

        private void SetResponse(string response)
        {
            dsc.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                return new CustomizedHttpWebRequestMessage(args,
                    response,
                    new Dictionary<string, string>()
                    {
                        {"Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8"},
                    });
            };
        }
    }
}
