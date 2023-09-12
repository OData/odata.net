using DataSourceGenerator;
using System.Reflection.Emit;
using System.Reflection;
using System;

namespace ODataServiceTests
{
    [TestClass]
    public class DataSourceGeneratorTests
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
            //Assert.AreEqual(6, properties.Length);
            // todo: add additional tests for generated types
        }

        [TestMethod]
        public void GenerateNwind2()
        {
            IServiceProvider sp = null;
            //var nwind = new NWind.NWindDataSource(sp);
        }
    }
}