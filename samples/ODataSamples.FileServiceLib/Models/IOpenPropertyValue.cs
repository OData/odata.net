using Microsoft.OData.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Models;

public interface IOpenPropertyValue
{
    object? GetPropertyValue(string propertyName);
    IEnumerable<KeyValuePair<string, object>> GetAllOpenProperties();

    void SetODataProperty(string propertyName, object propertyValue);
    IDictionary<string, object>? GetODataAnnotations(string propertyName);
    void SetODataAnnotation(string propertyName, string annotationName, object annotationValue);
}
