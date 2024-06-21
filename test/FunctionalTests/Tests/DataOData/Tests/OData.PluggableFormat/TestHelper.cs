//---------------------------------------------------------------------
// <copyright file="TestHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.PluggableFormat
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;

    public static class TestHelper
    {
        public static Stream GetResourceStream(string resourceName, Assembly assembly = null)
        {
            Assembly resAssembly = assembly ?? Assembly.GetCallingAssembly();
            Stream stream = resAssembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("Failed to get stream with name '{0}', available resource names:{{\n", resourceName);
                builder.AppendLine(string.Join(Environment.NewLine, resAssembly.GetManifestResourceNames()));
                builder.AppendLine("}");
                throw new ApplicationException(builder.ToString());
            }

            return stream;
        }

        public static string GetResourceString(string resourceName)
        {
            Stream stream = null;

            try
            {
                stream = GetResourceStream(resourceName, Assembly.GetCallingAssembly());
                using (var sr = new StreamReader(stream))
                {
                    stream = null;
                    return sr.ReadToEnd();
                }
            }
            finally
            {
                if (stream != null) stream.Dispose();
            }
        }

        public static IEdmModel GetModel(string modelResource)
        {
            IEdmModel model;
            Stream edmxStream = null;
            try
            {
                edmxStream = GetResourceStream(modelResource, Assembly.GetCallingAssembly());
                Debug.Assert(edmxStream != null, "stream must exist.");

                using (var xmlReader = XmlReader.Create(edmxStream))
                {
                    edmxStream = null;
                    IEnumerable<EdmError> errors;
                    bool valid = CsdlReader.TryParse(xmlReader, out model, out errors);
                    if (!valid)
                    {
                        ShowErrors(errors);
                    }
                    Debug.Assert(valid, "model should be parsed");

                    valid = model.Validate(out errors);
                    if (!valid)
                    {
                        ShowErrors(errors);
                    }

                    Debug.Assert(valid, "should not have semantic errors");
                }
            }
            finally
            {
                if (edmxStream != null)
                {
                    edmxStream.Dispose();
                }
            }

            return model;
        }

        public static ODataMessageReader CreateMessageReader(
           Stream stream,
           IServiceProvider container,
           string contenttype = "application/json",
           IEdmModel model = null,
           bool isResponse = false)
        {
            var message = new InMemoryMessage { Stream = stream, ServiceProvider = container };
            message.SetHeader("Content-Type", contenttype);
            var messageSettings = new ODataMessageReaderSettings()
            {
                ShouldIncludeAnnotation = st => true,
            };

            // Have untyped value in test
            messageSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

            if (isResponse)
            {
                return new ODataMessageReader((IODataResponseMessage)message, messageSettings, model);
            }
            else
            {
                return new ODataMessageReader((IODataRequestMessage)message, messageSettings, model);
            }
        }

        public static ODataMessageWriter CreateMessageWriter(
           Stream stream,
           IServiceProvider container,
           string contenttype = "application/json",
           IEdmModel model = null,
           bool isResponse = true)
        {
            var message = new InMemoryMessage { Stream = stream, ServiceProvider = container };
            message.SetHeader("Content-Type", contenttype);
            var messageSettings = new ODataMessageWriterSettings
            {
                EnableMessageStreamDisposal = false,
            };

            // Have untyped value in test
            messageSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

            if (isResponse)
            {
                return new ODataMessageWriter((IODataResponseMessage)message, messageSettings, model);
            }
            else
            {
                return new ODataMessageWriter((IODataRequestMessage)message, messageSettings, model);
            }
        }

        public static string GetToplevelPropertyPayloadString(
            object value,
            IServiceProvider container,
            string contenttype = "application/json")
        {
            Stream stream = null;

            try
            {
                stream = new MemoryStream();
                using (var omw = CreateMessageWriter(stream, container, contenttype, null, false))
                {
                    var odataWriter = omw.CreateODataResourceWriter();
                    odataWriter.WriteStart((ODataResource)value);
                    odataWriter.WriteEnd();
                }

                stream.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(stream))
                {
                    stream = null;
                    return sr.ReadToEnd();
                }
            }
            finally
            {
                if (stream != null) stream.Dispose();
            }
        }

        public static bool FloatEqual(float a, float b, float e = 0.0001f)
        {
            return Math.Abs((a - b) / a) < e;
        }

        public static bool EntryEqual(ODataResource a, ODataResource b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            return PropertiesEqual(a.Properties, b.Properties);
        }

        private static bool PropertiesEqual(IEnumerable<ODataPropertyInfo> collection1, IEnumerable<ODataPropertyInfo> collection2)
        {
            if (collection1 == null || collection2 == null)
            {
                return false;
            }

            if (collection1.Count() != collection2.Count())
            {
                return false;
            }

            foreach (var collection1PropertyInfo in collection1)
            {
                var matchedProperties = collection2.Where(p => p.Name == collection1PropertyInfo.Name);
                
                if (matchedProperties.Count() != 1)
                {
                    return false;
                }

                var collection2PropertyInfo = matchedProperties.First();

                if (!collection1PropertyInfo.GetType().Equals(collection2PropertyInfo.GetType()))
                {
                    return false;
                }

                var collection1Property = collection1PropertyInfo as ODataProperty;
                var collection2Property = collection2PropertyInfo as ODataProperty;

                if (collection1Property == null || collection2Property == null)
                {
                    continue;
                }

                if (collection1Property.Value is float)
                {
                    if (!(collection2Property.Value is float)
                        || !FloatEqual((float)collection1Property.Value, (float)collection2Property.Value))
                    {
                        return false;
                    }
                }
                else if (collection1Property.Value is byte[])
                {
                    if (!(collection2Property.Value is byte[])
                        || !((byte[])collection1Property.Value).SequenceEqual((byte[])collection2Property.Value))
                    {
                        return false;
                    }
                }
                else if (collection1Property.Value is ODataCollectionValue)
                {
                    if (!(collection2Property.Value is ODataCollectionValue)
                        || !CollectionValueEqual((ODataCollectionValue)collection1Property.Value, (ODataCollectionValue)collection2Property.Value))
                    {
                        return false;
                    }
                }
                else if (!Equals(collection1Property.Value, collection2Property.Value))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CollectionValueEqual(ODataCollectionValue a, ODataCollectionValue b)
        {
            var ae = a.Items.GetEnumerator();
            var be = a.Items.GetEnumerator();

            bool am = ae.MoveNext();
            bool bm = be.MoveNext();
            while (am && bm)
            {
                if (!object.Equals(ae.Current, be.Current))
                {
                    return false;
                }

                am = ae.MoveNext();
                bm = be.MoveNext();
            }

            return !am && !bm;
        }

        private static void ShowErrors(IEnumerable<EdmError> errors)
        {
            foreach (var edmError in errors)
            {
                Console.WriteLine("{0} - {1}", edmError.ErrorLocation, edmError.ErrorMessage);
            }
        }
    }
}
