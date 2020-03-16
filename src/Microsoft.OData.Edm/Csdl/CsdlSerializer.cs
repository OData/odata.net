//---------------------------------------------------------------------
// <copyright file="CsdlSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Reader;

namespace Microsoft.OData.Edm.Csdl
{
    public interface IJsonWriter
    {
        void StartObject();
    }

    /// <summary>
    /// Provides functionality to serialize <see cref="IEdmModel"/> to CSDL (JSON/XML)
    /// or to deserialize CSDL (JSON/XML) into <see cref="IEdmModel"/>.
    /// </summary>
    public static class CsdlSerializer
    {
        #region Serialize EdmModel to CSDL
        /// <summary>
        /// Parse the text representing a single CSDL value into a <see cref="IEdmModel"/>
        /// </summary>
        /// <param name="csdl">CSDL text to parse.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <remarks>Using a <see cref="string"/> is not as efficient as using the
        /// UTF-8 methods since the implementation natively uses UTF-8.
        /// </remarks>
        public static void Serialize(Stream stream, IEdmModel model)
        {
            Serialize(stream, model, CsdlSerializerOptions.DefaultOptions);
        }

        public static void Serialize(Stream stream, IEdmModel model, CsdlSerializerOptions options)
        {
            return;
        }

        public static string Serialize(IEdmModel model)
        {
            return Serialize(model, CsdlSerializerOptions.DefaultOptions);
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static string Serialize(IEdmModel model, CsdlSerializerOptions options)
        {
            return null;
        }

        /// <summary>
        /// Serializes the specified <see cref="Object"/> and writes the JSON structure
        /// using the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the JSON structure.</param>
        /// <param name="value">The <see cref="Object"/> to serialize.</param>
        public static void Serialize(XmlWriter textWriter, IEdmModel model)
        {
            // EdmModel => Xml Csdl
        }

        /// <summary>
        /// Serializes the specified <see cref="Object"/> and writes the JSON structure
        /// using the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the JSON structure.</param>
        /// <param name="value">The <see cref="Object"/> to serialize.</param>
        public static void Serialize(IJsonWriter jsonWriter, IEdmModel model)
        {
            // EdmModel => Json Csdl
        }
        #endregion

        #region Deserialize CSDL to Edm Model
        /// <summary>
        /// Parse the text (JSON/XML...) representing a single CSDL into a <see cref="IEdmModel"/>
        /// </summary>
        /// <param name="csdl">CSDL text to parse.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <remarks>Using a <see cref="string"/> is not as efficient as using the
        /// UTF-8 methods since the implementation natively uses UTF-8.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEdmModel Deserialize(string csdl, CsdlSerializerOptions options = null)
        {
            if (csdl == null)
            {
                throw new ArgumentNullException("csdl");
            }

            if (options == null)
            {
                options = CsdlSerializerOptions.DefaultOptions;
            }

            // Valid JSON starts always with '{' or '['
            // However, A CSDL JSON document consists of a single JSON object.
            // So, let's only test the single JSON object
            csdl = csdl.Trim();
            bool isJson = csdl.StartsWith("{", StringComparison.Ordinal) && csdl.EndsWith("}", StringComparison.Ordinal);

            using (TextReader txtReader = new StringReader(csdl))
            {
                if (isJson)
                {
                    return Deserialize(txtReader, options);
                }
                else
                {
                    // So far, we only supports JSON and XML
                    using (XmlReader xmlReader = XmlReader.Create(txtReader))
                    {
                        return Deserialize(xmlReader, options);
                    }

                }
            }
        }

        /// <summary>
        /// Parse the JSON representing a single CSDL into a <see cref="IEdmModel"/>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEdmModel Deserialize(TextReader reader, CsdlSerializerOptions options = null)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (options == null)
            {
                options = CsdlSerializerOptions.DefaultOptions;
            }

            // Maybe it's a CSDL, or it maybe multiple Schemas (Check with Michael),
            // So, we should change the follow code to support it.
            CsdlJsonReader csdlReader = new CsdlJsonReader(reader, options);
            return csdlReader.BuildEdmModel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonReader"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEdmModel Deserialize(IJsonReader jsonReader, CsdlSerializerOptions options = null)
        {
            if (jsonReader == null)
            {
                throw new ArgumentNullException("jsonReader");
            }

            if (options == null)
            {
                options = CsdlSerializerOptions.DefaultOptions;
            }


            // stream => IJsonValue, will throw exception.
            // Only check the JSON syntax
            //JsonObjectValue csdlObject = jsonReader.ReadAsObject();

            //CsdlJsonParser parser = new CsdlJsonParser(options);
            //parser.

            //return model;

            return null;
        }

        /// <summary>
        /// Parse the XML representing a single CSDL into a <see cref="IEdmModel"/>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEdmModel Deserialize(XmlReader reader, CsdlSerializerOptions options = null)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (options == null)
            {
                options = CsdlSerializerOptions.DefaultOptions;
            }

            return null;
        }

        #endregion

        //private static bool IsJson(this string csdl)
        //{
        //    csdl = csdl.Trim();
        //    return csdl.StartsWith("{", StringComparison.Ordinal) && csdl.EndsWith("}", StringComparison.Ordinal)
        //           || csdl.StartsWith("[", StringComparison.Ordinal) && csdl.EndsWith("]", StringComparison.Ordinal);
        //}
    }
}
