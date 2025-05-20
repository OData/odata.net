//---------------------------------------------------------------------
// <copyright file="EdmLibTestCaseBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.Common;

public class EdmLibTestCaseBase
{
    protected Dictionary<EdmVersion, Version> toProductVersionlookup = new Dictionary<EdmVersion, Version>()
    {
        { EdmVersion.V40, EdmConstants.EdmVersion4 },
        { EdmVersion.V401, EdmConstants.EdmVersion401 }
    };

    private EdmVersion v;
    public EdmVersion EdmVersion
    {
        get { return v; }
        set { v = value; }
    }

    public Version GetProductVersion(EdmVersion edmVersion)
    {
        return toProductVersionlookup[EdmVersion];
    }

    public CsdlXElementComparer CsdlXElementComparer = new CsdlXElementComparer();

    protected IEnumerable<string> GetSerializerResult(IEdmModel edmModel, EdmVersion edmVersion, out IEnumerable<EdmError> errors)
    {
        var writerSettings = new CsdlXmlWriterSettings
        {
            LibraryCompatibility = EdmLibraryCompatibility.UseLegacyVariableCasing
        };

        var stringBuilders = new List<StringBuilder>();
        var xmlWriters = new List<XmlWriter>();
        edmModel.SetEdmVersion(toProductVersionlookup[edmVersion]);
        edmModel.TryWriteSchema(
            s =>
            {
                stringBuilders.Add(new StringBuilder());
                xmlWriters.Add(XmlWriter.Create(stringBuilders.Last()));

                return xmlWriters.Last();
            }, out errors);

        for (int i = 0; i < stringBuilders.Count; i++)
        {
            xmlWriters[i].Close();
        }

        var strings = new List<string>();
        foreach (var sb in stringBuilders)
        {
            strings.Add(sb.ToString());
        }

        return strings;
    }

    protected IEnumerable<string> GetSerializerResult(IEdmModel edmModel)
    {
        var csdls = GetSerializerResult(edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());
        return csdls;
    }

    protected IEnumerable<string> GetSerializerResult(IEdmModel edmModel, out IEnumerable<EdmError> errors)
    {
        return GetSerializerResult(edmModel, EdmVersion, out errors);
    }

    protected static XElement ExtractElementByName(IEnumerable<XElement> inputSchemas, string elementNameToExtract)
    {
        XNamespace csdlXNamespace = inputSchemas.First().Name.Namespace;
        var containers = new XElement(csdlXNamespace + "Schema",
                                  new XAttribute("Namespace", "ExtractedElements"));

        foreach (var s in inputSchemas)
        {
            foreach (var c in s.Elements(csdlXNamespace + elementNameToExtract).ToArray())
            {
                c.Remove();
                containers.Add(c);
            }
        }

        return containers;
    }

    public class EdmLibTestErrors : List<EdmError>
    {
        public void Add(int? lineNumber, int? linePostion, EdmErrorCode edmErrorCode)
        {
            EdmLibTestCsdlLocation? errorLocation = null;
            if (lineNumber != null && linePostion != null)
            {
                errorLocation = new EdmLibTestCsdlLocation(lineNumber, linePostion);
            }
            Add(new EdmError(errorLocation, edmErrorCode, string.Empty));
        }

        public void Add(string typeName, EdmErrorCode edmErrorCode)
        {
            var errorLocation = new EdmLibTestObjectLocation(typeName);
            Add(new EdmError(errorLocation, edmErrorCode, string.Empty));
        }
    }

    private class EdmLibTestObjectLocation : EdmLocation
    {
        /// <summary>
        /// Gets the object type name where the error happens.
        /// </summary>
        public string ObjectTypeName { get; set; }
        public EdmLibTestObjectLocation(string typeName)
        {
            ObjectTypeName = typeName;
        }
        /// <summary>
        /// Gets a string representation of the location.
        /// </summary>
        /// <returns>A string representation of the location.</returns>
        public override string ToString()
        {
            return ObjectTypeName;
        }
    }

    private class EdmLibTestCsdlLocation : EdmLocation
    {
        /// <summary>
        /// Gets the line number in the file.
        /// </summary>
        public int? LineNumber { get; set; }

        /// <summary>
        /// Gets the position in the line.
        /// </summary>
        public int? LinePosition { get; set; }

        public EdmLibTestCsdlLocation(int? lineNumber, int? linePostion)
        {
            LineNumber = lineNumber;
            LinePosition = linePostion;
        }

        /// <summary>
        /// Gets a string representation of the location.
        /// </summary>
        /// <returns>A string representation of the location.</returns>
        public override string ToString()
        {
            return "(" + Convert.ToString(LineNumber, CultureInfo.InvariantCulture) + ", " + Convert.ToString(LinePosition, CultureInfo.InvariantCulture) + ")";
        }
    }
}
