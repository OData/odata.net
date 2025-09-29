using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Models;

internal class ExtensionOpenPropertyValue(Dictionary<string, object>? data = null) : IOpenPropertyValue
{
    private Dictionary<string, object> odataProperties = data ?? [];
    private Dictionary<string, Dictionary<string, object>> odataAnnotations = new();

    public IEnumerable<KeyValuePair<string, object>> GetAllOpenProperties()
    {
        return odataProperties;
    }

    public object? GetPropertyValue(string propertyName)
    {
        if (odataProperties.TryGetValue(propertyName, out var value))
        {
            return value;
        }

        return null;
    }

    public void SetODataProperty(string propertyName, object propertyValue)
    {
        this.odataProperties.Add(propertyName, propertyValue);
    }

    public IDictionary<string, object>? GetODataAnnotations(string propertyName)
    {
        if (odataAnnotations.TryGetValue(propertyName, out var annotations))
        {
            return annotations;
        }

        return null;
    }

    public void SetODataAnnotation(string propertyName, string annotationName, object annotationValue)
    {
        if (!this.odataAnnotations.TryGetValue(propertyName, out var annotations))
        {
            annotations = new Dictionary<string, object>();
            this.odataAnnotations.Add(propertyName, annotations);
        }

        annotations[annotationName] = annotationValue;
    }
}
