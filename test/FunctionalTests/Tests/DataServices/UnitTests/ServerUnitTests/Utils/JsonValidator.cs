//---------------------------------------------------------------------
// <copyright file="JsonValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Spatial;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Data;
    using Microsoft.OData.Edm;
    using System.Linq;

    public class JsonValidator
    {
        public const string ObjectString = "Object";
        public const string ResultsString = "results";
        public const string ArrayString = "Array";
        public const string ValueString = "Value";
        public const string Metadata = "__metadata";
        public const string Deferred = "__deferred";

        internal static readonly long DatetimeMinTimeTicks = System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.DatetimeMinTimeTicks;

        // Adapted from http://blogs.msdn.com/ericlippert/archive/2005/10/12/480154.aspx,
        // %sdxroot%\public\sdk\inc\ActivScp.h and http://support.microsoft.com/kb/221992
        static readonly Guid CLSID_VBScript = new Guid(0xb54f3741, 0x5b07, 0x11cf, 0xa4, 0xb0, 0x00, 0xaa, 0x00, 0x4a, 0x55, 0xe8);
        static readonly Guid CLSID_JScript = new Guid(0xf414c260, 0x6ac0, 0x11cf, 0xb6, 0xd1, 0x00, 0xaa, 0x00, 0xbb, 0xbb, 0x58);

        public static string ConvertToXml(string json, string[] xPaths, Type clrType)
        {
            StringBuilder stringBuilder = new StringBuilder();

            // Create xmlwriter to preserve whitespace
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = false;
            settings.CloseOutput = false;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Entitize;
            string strippedString = string.Empty;
            using (XmlWriter writer = XmlWriter.Create(stringBuilder, settings))
            {
                if (json.StartsWith("{"))
                {
                    json = '(' + json + ')';
                }
                else if (json.StartsWith("["))
                {
                    // Since there is no good way of knowing empty array
                    strippedString = json.Replace(Environment.NewLine, "");
                    strippedString = strippedString.Replace(" ", "");
                }

                if (!json.Contains("[") && !json.Contains("}"))
                {
                    writer.WriteStartElement(JsonValidator.ValueString);
                    writer.WriteValue(json);
                    writer.WriteEndElement();
                }
                else if (strippedString.Equals("[]"))
                {
                    writer.WriteStartElement(JsonValidator.ArrayString);
                    writer.WriteEndElement();
                }
                else
                {
                    // Convert string to Jscript Object
                    object jsonObject = JSonToObject(json);

                    WriteXml(writer, jsonObject);
                }
            }
            return stringBuilder.ToString();
        }

        public static XmlDocument ConvertToXmlDocument(System.IO.Stream stream)
        {
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(stream, "stream");
            string json;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }

            return ConvertToXmlDocument(json);
        }

        public static XmlDocument ConvertToXmlDocument(string json)
        {
            json = System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.FilterJson(json).Trim();
            string xml = ConvertToXml(json, null, null);
            XmlDocument document = new XmlDocument(System.Data.Test.Astoria.TestUtil.TestNameTable);
            document.PreserveWhitespace = true;
            document.LoadXml(xml);
            return document;
        }

        public static XDocument ConvertToXDocument(System.IO.Stream stream)
        {
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(stream, "stream");
            string json;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }

            return ConvertToXDocument(json);
        }

        public static XDocument ConvertToXDocument(string json)
        {
            json = System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.FilterJson(json).Trim();
            string xml = ConvertToXml(json, null, null);
            XDocument document = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
            return document;
        }

        public static string GetJsonTypeXPath(Type type, bool isArray)
        {
            string xPath;

            if (isArray)
            {
                xPath = String.Format("/{0}/{1}/{2}/type[text()='{3}']",
                    JsonValidator.ArrayString, JsonValidator.ObjectString, JsonValidator.Metadata,
                    type.FullName);
            }
            else
            {
                xPath = String.Format("/{0}/{1}/type[text()='{2}']",
                    JsonValidator.ObjectString, JsonValidator.Metadata, type.FullName);
            }

            return xPath;
        }

        public static string GetJsonUriXPath(string uri, bool isArray)
        {
            string xPath;

            if (isArray)
            {
                xPath = String.Format("/{0}/{1}/{2}/uri[text()='{3}']",
                    JsonValidator.ArrayString, JsonValidator.ObjectString, JsonValidator.Metadata, uri);
            }
            else
            {
                xPath = String.Format("/{0}/{1}/uri[text()='{2}']",
                    JsonValidator.ObjectString, JsonValidator.Metadata, uri);
            }

            return xPath;
        }

        private static object JSonToObject(string json)
        {
            JScriptEngine engine = new JScriptEngine();
            IActiveScript script = (IActiveScript)engine;
            MySite site = new MySite();
            engine.SetScriptSite(site);

            IActiveScriptParse32 scriptParser = (IActiveScriptParse32)engine;
            scriptParser.InitNew();
            engine.SetScriptState(ScriptState.SCRIPTSTATE_CONNECTED);

            // SCRIPTTEXT_ISEXPRESSION
            // If the distinction between a computational expression and a 
            // statement is important but syntactically ambiguous in the 
            // script language, this flag specifies that the scriptlet is 
            // to be interpreted as an expression, rather than as a 
            // statement or list of statements. By default, statements are 
            // assumed unless the correct choice can be determined from 
            // the syntax of the scriptlet text. 
            const int SCRIPTTEXT_ISEXPRESSION = 0x00000020;
            // Tricky: http://msdn2.microsoft.com/en-us/library/system.runtime.interopservices.variantwrapper.aspx.
            object result = null;
            System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo;

            Trace.WriteLine("Parsing JSON:");
            if (json.Length < 256)
            {
                Trace.WriteLine(json);
            }
            else
            {
                Trace.WriteLine(json.Substring(0, 128) + "..." + json.Substring(json.Length - 128, 128));
            }

            scriptParser.ParseScriptText(json, null, null, null, 0, 0,
                SCRIPTTEXT_ISEXPRESSION, ref result, out exceptionInfo);

            if (exceptionInfo.scode != 0)
            {
                throw new Exception(exceptionInfo.bstrDescription);
            }

            //engine.Close();
            return result;
        }

        private static void WriteArray(XmlWriter xmlWriter, object[] elements)
        {
            Debug.Assert(xmlWriter != null, "xmlWriter != null");
            Debug.Assert(elements != null, "elements != null");

            xmlWriter.WriteStartElement(ArrayString);
            for (int i = 0; i < elements.Length; i++)
            {
                WriteXml(xmlWriter, elements[i]);
            }
            xmlWriter.WriteEndElement();
        }

        private static void WriteXml(XmlWriter xmlWriter, object jsonObject)
        {
            object[] elements;

            if (IsArray(jsonObject, out elements))
            {
                WriteArray(xmlWriter, elements);
            }
            else
            {
                xmlWriter.WriteStartElement(ObjectString); 
                WriteProperties(xmlWriter, jsonObject);
                xmlWriter.WriteEndElement();
            }
        }

        private static void WriteProperties(XmlWriter xmlWriter, object jsonObject)
        {
            Dictionary<string, object> properties;
            GetProperties(jsonObject, out properties);

            object[] arrayElements;
            if (IsArray(jsonObject, out arrayElements))
            {
                WriteArray(xmlWriter, arrayElements);
            }
            else if (properties.Count == 0)
            {
                string stringValue = TypeData.XmlValueFromObject(jsonObject);
                if (stringValue == null)
                {
                    xmlWriter.WriteAttributeString("IsNull", "true");
                }
                else
                {
                    xmlWriter.WriteValue(stringValue);
                }
            }
            else if (IsGeoJson(properties))
            {
                // If __metadata is specified, then we need to write it seperately
                // since GeoJsonFormatter will ignore that element.
                object jsonMetadataProperties;
                if (properties.TryGetValue(JsonValidator.Metadata, out jsonMetadataProperties))
                {
                    xmlWriter.WriteStartElement(JsonValidator.Metadata);
                    WriteProperties(xmlWriter, jsonMetadataProperties);
                    xmlWriter.WriteEndElement();
                }

                GeoJsonToGml(properties, xmlWriter);
            }
            else
            {
                foreach (KeyValuePair<string, object> property in properties)
                {
                    xmlWriter.WriteStartElement(XmlConvert.EncodeName(property.Key));
                    WriteProperties(xmlWriter, property.Value);
                    xmlWriter.WriteEndElement();
                }
            }
        }
        
        /// <summary>
        /// Convert JsonObject back to Json Text
        /// </summary>
        /// <param name="json">The Json Object</param>
        /// <returns>The Json Text</returns>
        private static object ConvertFromDispatch(object json)
        {
            object[] arrayContent;
            if (IsArray(json, out arrayContent))
            {
                return arrayContent.Select(o => ConvertFromDispatch(o));
            }
            else if (json is IDispatch)
            {
                
                // Json Object
                Dictionary<String, object> innerProps;
                GetProperties(json, out innerProps);
                return ConvertFromDispatch(innerProps);
            }
            else if (json is String)
            {
                return json;
            }
            else if (json is DBNull)
            {
                // MS JScript Engine converts 'null' into DBNull
                return null;
            }
            else
            {
                return json;
            }
        }

        /// <summary>
        /// Convert a dictionary of Json properties into Json Text
        /// </summary>
        /// <param name="properties">The properties</param>
        /// <returns>Json Text</returns>
        private static IDictionary<string, object> ConvertFromDispatch(Dictionary<string, object> properties)
        {
            return properties.ToDictionary(pair => pair.Key, pair => ConvertFromDispatch(pair.Value));
        }

        /// <summary>
        /// Convert GeoJson to GML
        /// </summary>
        /// <param name="source">GeoJson Dictionary</param>
        /// <param name="target">XmlWriter that outputs Gml</param>
        private static void GeoJsonToGml(Dictionary<string, object> source, XmlWriter target)
        {
            GeoJsonObjectFormatter sourceFormatter = GeoJsonObjectFormatter.Create();
            GmlFormatter targetFormatter = GmlFormatter.Create();

            var converted = ConvertFromDispatch(source);

            sourceFormatter.Read<Geography>(converted).SendTo(targetFormatter.CreateWriter(target));
        }

        private static bool IsGeoJson(Dictionary<string, object> properties)
        {
            return
                (properties.Count == 3 && properties.ContainsKey("type") && (properties.ContainsKey("coordinates") || properties.ContainsKey("geometries")) && properties.ContainsKey("crs")) ||
                (properties.Count == 4 && properties.ContainsKey("type") && (properties.ContainsKey("coordinates") || properties.ContainsKey("geometries")) && properties.ContainsKey("crs") && properties.ContainsKey("__metadata"));
        }

        private static bool IsArray(object jsonResult, out object[] elements)
        {
            IDispatch dispatch = jsonResult as IDispatch;
            if (dispatch == null)
            {
                elements = null;
                return false;
            }

            int length = -1;
            Guid iidNull = Guid.Empty;
            int[] ids = new int[1];
            if (0 == dispatch.GetIDsOfNames(ref iidNull, new string[] { "length"}, 1, 0, ids))
            {
                // we have "length" property. That means that this type is an array type. [Note: If the object has a length property, this might be a problem]
                length = (int)jsonResult.GetType().InvokeMember("length", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, jsonResult, null);
            }
            else
            {
                elements = null;
                return false;
            }

            List<object> objectList = new List<object>();
            for (int i = 0; i < length; i++)
            {
                object element = jsonResult.GetType().InvokeMember(i.ToString(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, jsonResult, null);
                objectList.Add(element);
            }

            elements = objectList.ToArray();
            return true;
        }

        const int DISPID_START_ENUM = -1;
        const int fdexEnumDefault = 1;
        const int fdexEnumAll = 2;

        private static void GetProperties(object jsonObject, out Dictionary<string, object> properties)
        {
            properties = new Dictionary<string, object>();

            IDispatchEx dispatch = jsonObject as IDispatchEx;
            if (dispatch == null)
            {
                return;
            }

            int id = DISPID_START_ENUM;
            int nextId = -2;

            while (0 == dispatch.GetNextDispID(fdexEnumAll, id, out nextId))
            {
                string name;
                id = nextId;
                int result = dispatch.GetMemberName(id, out name);
                Debug.Assert(result == 0, "Member name must be present");

                object o = jsonObject.GetType().InvokeMember(name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null,
                    jsonObject, null);

                // now, the property name maybe the instance annotation, while the instance annotation starts with "@"
                if (name.StartsWith("@"))
                {
                    name = name.Substring(1); // remove the @ sign
                }

                properties.Add(name, o);
            }
       }

        public static string GetJsonDateTimeStringValue(DateTime dateTime)
        {
            return System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.GetJsonDateTimeStringValue(dateTime);
        }

        internal static string PrimitiveToString(object value, Type inputPrimitiveType)
        {
            return System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.PrimitiveToString(value, inputPrimitiveType);
        }

        public static object StringToPrimitive(string value, Type primitiveType)
        {
            return System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.StringToPrimitive(value, primitiveType);
        }

        public static DateTime ToUniversal(DateTime dateTime)
        {
            return System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.ToUniversal(dateTime);
        }

        // Json engine removes the back slash since its used for escaping. So while
        // comparing xpaths for date time values, we need to remove the backslash from the
        // date time value
        public static string GetEscapedDateTimeString(string dateTime)
        {
            Debug.Assert(dateTime.StartsWith(@"\/") && dateTime.EndsWith(@"\/"), "dateTime.StartsWith(\"\\/\") && dateTime.EndsWith(\"\\/\")");
            return dateTime.Replace("\\", "");
        }

        internal static string GetJsonPayload(IEdmEntityType entityType, object value)
        {
            Type valueType = value.GetType();
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append(Environment.NewLine);
            sb.Append("\t");
            sb.Append("@odata.type: \"" + entityType.FullName() + "\",");
            var properties = entityType.StructuralProperties().ToList();
            for (int i = 0; i < properties.Count; i++)
            {
                sb.Append("\t");
                IEdmProperty property = properties[i];
                // Type propertyType = ((PrimitiveType)property.TypeUsage.EdmType).ClrEquivalentType;
                PropertyInfo propertyInfo = valueType.GetProperty(property.Name);
                Type propertyType = propertyInfo.PropertyType;
                propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;            
                object propertyValue = propertyInfo.GetValue(value, null);
                sb.Append(String.Format("{0}:{1}", properties[i].Name, PrimitiveToString(propertyValue, propertyType)));
                if (i != properties.Count - 1)
                {
                    sb.Append(",");
                }
                sb.Append(Environment.NewLine);
            }
            sb.Append("}");

            return sb.ToString();
        }

        private static Dictionary<Type, bool> quotedTypes;
    }

    [
    ComImport,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("00020400-0000-0000-C000-000000000046")
    ]
    interface IDispatch {
        void GetTypeInfoCount(out uint pctinfo);

        void GetTypeInfo(uint iTInfo, int lcid, out IntPtr info);

        [PreserveSig]
        int GetIDsOfNames(
            ref Guid iid,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2)]
            string[] names,
            uint cNames,
            int lcid,
            [Out]
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 2)]
            int[] rgDispId);

        [PreserveSig]
        int Invoke(
            int dispIdMember,
            ref Guid riid,
            int lcid,
            ushort wFlags,
            out System.Runtime.InteropServices.ComTypes.DISPPARAMS pDispParams,
            out object VarResult,
            out System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo,
            out int puArgErr);
    }

    [
    ComImport,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("A6EF9860-C720-11d0-9337-00A0C90DCAA9")
    ]
    interface IDispatchEx
    {
        void GetTypeInfoCount(out uint pctinfo);

        void GetTypeInfo(uint iTInfo, int lcid, out IntPtr info);

        [PreserveSig]
        int GetIDsOfNames(
            ref Guid iid,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2)]
            string[] names,
            uint cNames,
            int lcid,
            [Out]
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 2)]
            int[] rgDispId);

        [PreserveSig]
        int Invoke(
            int dispIdMember,
            ref Guid riid,
            int lcid,
            ushort wFlags,
            out System.Runtime.InteropServices.ComTypes.DISPPARAMS pDispParams,
            out object VarResult,
            out System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo,
            out int puArgErr);

        void Unused1();
        // NOTES:
        // (*) grfdex can contain any subset of the bits
        //     { fdexNameCaseSensitive, fdexNameEnsure, fdexNameImplicit }.
        // (*) pvarInit may be NULL.
        //HRESULT GetDispID(
        //    [in] BSTR bstrName,
        //    [in] DWORD grfdex,
        //    [out] DISPID *pid);

        // NOTES:
        // (*) pvarRes, pei and pspCaller may be NULL.
        // (*) When DISPATCH_METHOD is set in wFlags, there may be a "named
        //     parameter" for the "this" value. The dispID will be DISPID_THIS and
        //     it must be the first named parameter.
        // (*) There is a new value for wFlags: DISPATCH_CONSTRUCT. This indicates
        //     that the item is being used as a constructor.
        // (*) The legal values for wFlags are:
        //     DISPATCH_PROPERTYGET
        //     DISPATCH_METHOD
        //     DISPATCH_PROPERTYGET | DISPATCH_METHOD
        //     DISPATCH_PROPERTYPUT
        //     DISPATCH_PROPERTYPUTREF
        //     DISPATCH_PROPERTYPUT | DISPATCH_PROPERTYPUTREF
        //     DISPATCH_CONSTRUCT
        // (*) IDispatchEx::Invoke should support the same values that
        //     IDispatchEx::InvokeEx supports (eg, DISPID_THIS, DISPATCH_CONSTRUCT).
        //[local]
        //HRESULT InvokeEx(
        //    [in] DISPID id,
        //    [in] LCID lcid,
        //    [in] WORD wFlags,
        //    [in] DISPPARAMS *pdp,
        //    [out] VARIANT *pvarRes,	// Can be NULL.
        //    [out] EXCEPINFO *pei,	// Can be NULL.
        //    [in, unique] IServiceProvider *pspCaller);

        void Unused2();

        //[call_as(InvokeEx)]
        //HRESULT RemoteInvokeEx(
        //    [in] DISPID id,
        //    [in] LCID lcid,
        //    [in] DWORD dwFlags, 	// Hiword used for private marshalling flags.
        //    [in] DISPPARAMS *pdp,
        //    [out] VARIANT *pvarRes,
        //    [out] EXCEPINFO *pei,
        //    [in, unique] IServiceProvider *pspCaller,
        //    [in] UINT cvarRefArg,
        //    [in, size_is(cvarRefArg)] UINT *rgiRefArg,
        //    [in, out, size_is(cvarRefArg)] VARIANT *rgvarRefArg);

        void Unused3();

        // NOTES:
        // (*) grfdex can optionally contain the bit fdexNameCaseSensitive.
        // (*) If the member doesn't exist, return S_OK.
        // (*) If the member exists but can't be deleted, return S_FALSE.
        // (*) If the member is deleted, the DISPID still needs to be valid for
        //     GetNextDispID and if a member of the same name is recreated, the
        //     dispID should be the same.
        //HRESULT DeleteMemberByName([in] BSTR bstr, [in] DWORD grfdex);
        void Unused4();

        // NOTES:
        // (*) If the member doesn't exist, return S_OK.
        // (*) If the member exists but can't be deleted, return S_FALSE.
        // (*) If the member is deleted, the DISPID still needs to be valid for
        //     GetNextDispID and if a member of the same name is recreated, the
        //     dispID should be the same.
        //HRESULT DeleteMemberByDispID([in] DISPID id);
        void Unused5();

        //HRESULT GetMemberProperties(
        //    [in] DISPID id,
        //    [in] DWORD grfdexFetch,
        //    [out] DWORD *pgrfdex);
        //Bvoid Unused6();

        //HRESULT GetMemberName(
        //    [in] DISPID id,
        //    [out] BSTR *pbstrName);
        [PreserveSig]
        int GetMemberName(int id, out string pbstrName);

        //HRESULT GetNextDispID(
        //    [in] DWORD grfdex,
        //    [in] DISPID id,
        //    [out] DISPID *pid);
        [PreserveSig]
        int GetNextDispID(
            int grfdex,
            int id,
            out int pid);

        //HRESULT GetNameSpaceParent([out] IUnknown **ppunk);
        void Unused7();
    }


    enum ScriptState: uint // Just a guess
    {
        SCRIPTSTATE_UNINITIALIZED	= 0,
        SCRIPTSTATE_INITIALIZED	= 5,
        SCRIPTSTATE_STARTED	= 1,
        SCRIPTSTATE_CONNECTED	= 2,
        SCRIPTSTATE_DISCONNECTED	= 3,
        SCRIPTSTATE_CLOSED	= 4
    }

    enum ScriptThreadState: uint
    {
        SCRIPTTHREADSTATE_NOTINSCRIPT	= 0,
        SCRIPTTHREADSTATE_RUNNING	= 1
    }

    class MySite : IActiveScriptSite
    {
        #region IActiveScriptSite Members

        public int GetLCID(IntPtr lcid)
        {
            return 0;
        }

        public int GetItemInfo(string name, out object unknown, out ITypeInfo typeInfo)
        {
            unknown = null;
            typeInfo = null;
            return 0;
        }

        public int GetDocVersionString(out string pbstrVersion)
        {
            pbstrVersion = null;
            return 0;
        }

        public int OnScriptTerminate(VariantWrapper varResult, ref System.Runtime.InteropServices.ComTypes.EXCEPINFO excepInfo)
        {
            return 0;
        }

        public int OnStateChange(ScriptState scriptState)
        {
            return 0;
        }

        public int OnScriptError(IActiveScriptError scriptError)
        {
            Debug.WriteLine("MySite::OnScriptError");
            System.Runtime.InteropServices.ComTypes.EXCEPINFO info = scriptError.GetExceptionInfo();
            Debug.WriteLine("  Description: " + info.bstrDescription);
            Debug.WriteLine("  Source: " + info.bstrSource);
            return -1;
        }

        public int OnEnterScript()
        {
            return 0;
        }

        public int OnLeaveScript()
        {
            return 0;
        }

        #endregion
    }

    [Guid("DB01A1E3-A42B-11cf-8F20-00805F2CD064")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IActiveScriptSite
    {
        [PreserveSig]
        Int32 GetLCID(IntPtr lcid);
        [PreserveSig]
        Int32 GetItemInfo(string name, out object unknown, out ITypeInfo typeInfo);
        [PreserveSig]
        Int32 GetDocVersionString(out string pbstrVersion);
        [PreserveSig]
        Int32 OnScriptTerminate(VariantWrapper varResult,
            ref System.Runtime.InteropServices.ComTypes.EXCEPINFO excepInfo);
        [PreserveSig]
        Int32 OnStateChange(ScriptState scriptState);
        [PreserveSig]
        Int32 OnScriptError(IActiveScriptError scriptError);
        [PreserveSig]
        Int32 OnEnterScript();
        [PreserveSig]
        Int32 OnLeaveScript();
    }

    [Guid("EAE1BA61-A4ED-11cf-8F20-00805F2CD064")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IActiveScriptError
    {
        System.Runtime.InteropServices.ComTypes.EXCEPINFO GetExceptionInfo();
        [PreserveSig]
        Int32 GetSourcePosition(out Int32 pdwSourceContext,
            out Int32 pulLineNumber, out Int32 plCharacterPosition);
        [return:MarshalAs(UnmanagedType.BStr)] string GetSourceLineText();
    }

    [Guid("BB1A2AE2-A4F9-11cf-8F20-00805F2CD064")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IActiveScriptParse32
    {
        void InitNew();
        [PreserveSig]
        Int32 AddScriptlet(
            [MarshalAs(UnmanagedType.BStr)] string defaultName,
            [MarshalAs(UnmanagedType.BStr)] string code,
            [MarshalAs(UnmanagedType.BStr)] string itemName,
            [MarshalAs(UnmanagedType.BStr)] string subItemName,
            [MarshalAs(UnmanagedType.BStr)] string eventName,
            [MarshalAs(UnmanagedType.BStr)] string delimiter,
            Int32 dwSourceContextCookie,
            Int32 ulStartingLineNumber,
            Int32 dwFlags,
            [MarshalAs(UnmanagedType.BStr)] out string name,
            out System.Runtime.InteropServices.ComTypes.EXCEPINFO pexcepinfo);
        
        [PreserveSig]
        Int32 ParseScriptText( 
            [MarshalAs(UnmanagedType.BStr)] string code,
            [MarshalAs(UnmanagedType.BStr)] string itemName,
            [MarshalAs(UnmanagedType.IUnknown)] object context,
            [MarshalAs(UnmanagedType.BStr)] string delimiter,
            Int32 dwSourceContextCookie,
            Int32 ulStartingLineNumber,
            Int32 dwFlags,
            [MarshalAs(UnmanagedType.Struct)] ref object result,
            //IntPtr result,
            out System.Runtime.InteropServices.ComTypes.EXCEPINFO pexcepinfo);
            //IntPtr pexcepinfo);
    }

    [Guid("BB1A2AE1-A4F9-11cf-8F20-00805F2CD064")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IActiveScript 
    {
        void SetScriptSite(IActiveScriptSite pass);
        IActiveScriptSite GetScriptSite(Guid riid);
        void SetScriptState(ScriptState ss);
        ScriptState GetScriptState();
        void Close();
        void AddNamedItem(string name, Int32 dwFlags);
        void AddTypeLib(ref Guid rguidTypeLib, Int32 dwMajor, Int32 dwMinor, Int32 dwFlags);
        object GetScriptDispatch(string itemName);
        Int32 GetScriptThreadID();
        ScriptThreadState GetScriptThreadState(Int32 thread);
        void InterruptScriptThread(Int32 thread, ref System.Runtime.InteropServices.ComTypes.EXCEPINFO pexecpinfo,
            Int32 dwFlags);
        IActiveScript Clone();
    };

    [ComImport]
    [Guid("f414c260-6ac0-11cf-b6d1-00aa00bbbb58")]
    class JScriptEngine: IActiveScript
    {
        #region IActiveScript Members

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void SetScriptSite(IActiveScriptSite pass);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern IActiveScriptSite GetScriptSite(Guid riid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void SetScriptState(ScriptState ss);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern ScriptState GetScriptState();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Close();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void AddNamedItem(string name, int dwFlags);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void AddTypeLib(ref Guid rguidTypeLib, int dwMajor, int dwMinor, int dwFlags);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern object GetScriptDispatch(string itemName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern int GetScriptThreadID();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern ScriptThreadState GetScriptThreadState(int thread);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void InterruptScriptThread(int thread, ref System.Runtime.InteropServices.ComTypes.EXCEPINFO pexecpinfo, int dwFlags);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern IActiveScript Clone();

        #endregion
    }
}
