using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ODataSamples.FileServiceLib.Schema;

public static class EdmModelHelper
{
    public static IEdmModel EdmModel { get; } = LoadEdmModel();
    private static IEdmModel LoadEdmModel()
    {
        var resourceName = "ODataSamples.FileServiceLib.Schema.FileServiceModel.xml";
        using var stream = typeof(EdmModelHelper).Assembly.
               GetManifestResourceStream(resourceName)
               ?? throw new Exception($"Could not load embedded schema at '{resourceName}'");

            using var xmlReader = XmlReader.Create(stream);
        var model = CsdlReader.Parse(xmlReader);
        return model;  
    }
}
