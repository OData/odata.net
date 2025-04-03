//-----------------------------------------------------------------------------
// <copyright file="ODataModelRefEdmModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Xunit;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef;

public class ODataModelRefEdmModel
{
    private static readonly Uri _baseUri = new("http://localhost/odata/");

    public static IEdmModel GetEdmModel()
    {
        Func<Uri, XmlReader?> getReferencedSchemaFunc = uri =>
        {
            if (uri.AbsoluteUri.StartsWith(_baseUri.AbsoluteUri))
            {
                string filename = uri.AbsoluteUri.Substring(_baseUri.AbsoluteUri.Length, uri.AbsoluteUri.Length - _baseUri.AbsoluteUri.Length);
                return XmlReader.Create(ReadResourceFromAssembly(filename));
            }
            else
            {
                return null;
            }
        };

        IEdmModel model = ReadModel("ODataModelRefServiceCsdl.xml", getReferencedSchemaFunc);
        model.Validate(out var errors);
        if (errors != null && errors.Count() > 0)
        {
            throw new InvalidOperationException("Failed to load model : " + string.Join(Environment.NewLine, errors.Select(e => e.ErrorMessage)));
        }

        return model;
    }

    private static IEdmModel ReadModel(string fileName, Func<Uri, XmlReader?>? getReferencedSchemaFunc = null)
    {
        IEdmModel model;
        using (Stream csdlStream = ReadResourceFromAssembly(fileName))
        {
            bool parseResult;
            IEnumerable<EdmError> errors;
            if (getReferencedSchemaFunc == null)
            {
                parseResult = CsdlReader.TryParse(XmlReader.Create(csdlStream), out model, out errors);
            }
            else
            {
                parseResult = CsdlReader.TryParse(XmlReader.Create(csdlStream), getReferencedSchemaFunc, out model, out errors);
            }

            if (!parseResult)
            {
                throw new InvalidOperationException("Failed to load model : " + string.Join(Environment.NewLine, errors.Select(e => e.ErrorMessage)));
            }
        }

        return model;
    }

    private static Stream ReadResourceFromAssembly(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourcePath = Enumerable.First(Enumerable.OrderBy(Enumerable.Where(assembly.GetManifestResourceNames(), name => name.EndsWith(resourceName)), filteredName => filteredName.Length));
        var resourceStream = assembly.GetManifestResourceStream(resourcePath);

        Assert.NotNull(resourceStream);

        var reader = new StreamReader(resourceStream);
        string str = reader.ReadToEnd();
        str = str.Replace("<edmx:Reference Uri=\"", $"<edmx:Reference Uri=\"{_baseUri.AbsoluteUri}");
        byte[] byteArray = Encoding.UTF8.GetBytes(str);

        resourceStream = new MemoryStream(byteArray);

        return resourceStream;
    }
}
