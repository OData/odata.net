using DataSourceGenerator;
using System.Reflection.Emit;
using System.Reflection;
using System;

namespace ODataServiceTests
{
    [TestClass]
    public class DatSourceGeneratorTests
    {
        [TestMethod]
        public void GenerateNwind()
        {
            string dataSourceName = "NWind";
            string schemaFileName = "SampleCsdl/" + dataSourceName + ".csdl";
            var model = DataSourceGenerator.DataSourceGenerator.ReadModel(schemaFileName);
            Type t = DataSourceGenerator.DataSourceGenerator.GenerateDataSource(model, dataSourceName, true);

            Assert.IsNotNull(t);
            var properties = t.GetProperties();
            Assert.AreEqual(7, properties.Length);
            // todo: add additional tests for generated types
        }
    }
}