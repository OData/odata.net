using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;

namespace Repro
{
    class Program
    {
        static void Main(string[] args)
        {
            TestLocal();
        }

        static void Test()
        {
            var csdl = File.ReadAllText("MainCsdl.xml");
            var reader = XmlReader.Create(File.OpenRead("MainCsdl.xml"));
            var permReader = XmlReader.Create(File.OpenRead("PermissionsCsdl.xml"));
            var mainModel = CsdlReader.Parse(reader);
            var permModel = CsdlReader.Parse(permReader);

            var mainAnnotations = mainModel.VocabularyAnnotations.ToArray();
            var permAnnotations = permModel.VocabularyAnnotations.ToArray();


            reader = XmlReader.Create(File.OpenRead("MainCsdl.xml"));
            var edmModel = CsdlReader.Parse(reader, GetReferenceModelReaderFunc);
            var annotations = edmModel.VocabularyAnnotations.ToArray();

        }

        static void TestLocal()
        {
            //var mainReader = XmlReader.Create(File.OpenRead("SimpleModel.xml"));
            //var permReader = XmlReader.Create(File.OpenRead("SimplePermissions.xml"));
            //var mainModel = CsdlReader.Parse(mainReader);
            //var permModel = CsdlReader.Parse(permReader);

            //var mainAnnotations = mainModel.VocabularyAnnotations.ToArray();
            //var permAnnotaitons = permModel.VocabularyAnnotations.ToArray();

            var reader = XmlReader.Create(File.OpenRead("SimpleModel.xml"));
            var model = CsdlReader.Parse(reader, GetLocalReferenceModelReaderFunc);
            //model.ReferencedModels.SelectMany(m => m.VocabularyAnnotations)
            var annotations = model.VocabularyAnnotations.ToArray();
            var container = model.FindEntityContainer("Container");
            //var entitySet = container.FindEntitySet("Products");
            var entitySet = model.FindDeclaredEntitySet("Products");

            model.FindVocabularyAnnotations(entitySet);
            var entitySetAnnotations = entitySet.VocabularyAnnotations(model);
            
            
        }

        private static XmlReader GetLocalReferenceModelReaderFunc(Uri uri)
        {
            if (uri != null)
            {
                var xmlReader = XmlReader.Create(File.OpenRead(uri.OriginalString));
                return xmlReader;
            }

            return null;
        }

        static XmlReader GetReferenceModelReaderFunc(Uri uri)
        {
            if (uri != null)
            {
                var httpClient = new HttpClient();
                //var csdl = httpClient.GetStringAsync(uri.OriginalString).Result;
                //var xmlReader = XElement.Parse(csdl).CreateReader();
                var stream = httpClient.GetStreamAsync(uri.OriginalString).Result;
                var xmlReader = XmlReader.Create(stream);
                return xmlReader;
            }

            return null;
        }
    }
}