//---------------------------------------------------------------------
// <copyright file="JSONPayload.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.JScript;

namespace System.Data.Test.Astoria
{
    public class JSONPayload : CommonPayload
    {
        private static readonly Regex jsonSecurityRegex =
            new Regex("^\\{\\s*\"d\"\\s*\\:(.*)\\}$", RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>
        /// Removes the { d: } wrapper from a json string
        /// </summary>
        /// <param name="json">the json string</param>
        /// <returns>the wrapped portion of the json string, or the same string if the wrapper is not present</returns>
        public static string StripSecurityWrapper(string json)
        {
            Match match = jsonSecurityRegex.Match(json);
            if (match.Success)
                json = match.Groups[1].Value;
            return json;
        }

        private const string v2JsonResultsField = "results";
        private const string v2CountField = "__count";
        private const string v2NextField = "__next";        

        // DO NOT INSTANTIATE THIS DIRECTLY. Use either CommonPayload.CreateCommonPayload or AstoriaResponse.CommonPayload
        internal JSONPayload(AstoriaRequestResponseBase rr)
            : base(rr)
        {
            string jsonString = this.RawPayload;
            if (jsonString == null)
            {
                this.Value = null;
                return;
            }

            jsonString = StripSecurityWrapper(jsonString);

            object jsonData = null;
            try
            {
                jsonData = Evaluator.EvalToObject("(" + jsonString + ")");
            }
            catch (Exception e)
            {
                this.Value = jsonString;
                return;
            }

            if (jsonData is ArrayObject)
            {
                this.Resources = ParseJsonArray((ArrayObject)jsonData);
            }
            else if (jsonData is JSObject)
            {
                FieldInfo[] fields = ((JSObject)jsonData).GetFields(BindingFlags.Default);

                // v2-style array, with count/page
                if (fields.Any(field => field.Name == v2JsonResultsField))
                {
                    foreach (FieldInfo field in fields)
                    {
                        object value = field.GetValue(field);
                        switch (field.Name)
                        {
                            case v2NextField:
                                this.NextLink = value.ToString();
                                break;

                            case v2CountField:
                                this.Count = Convert.ToInt64(value.ToString());
                                break;

                            case v2JsonResultsField:
                                if (value is ArrayObject)
                                    this.Resources = ParseJsonArray((ArrayObject)value);
                                else
                                    AstoriaTestLog.FailAndThrow("Array expected in results field");
                                break;

                            default:
                                AstoriaTestLog.FailAndThrow("Field '" + field.Name + "' unexpected");
                                break;
                        }
                    }
                }
                else if (fields.Any(field => field.Name == "__metadata"))
                {
                    this.Resources = ParseSingleJsonObject((JSObject)jsonData);   // entry
                }
                else
                {
                    if (fields.Length > 1)
                        AstoriaTestLog.FailAndThrow("Single field expected for non-entry payload");

                    FieldInfo field = fields[0];
                    object value = field.GetValue(field);

                    PayloadObject payloadObject = new PayloadObject(this);
                    PayloadProperty property;
                    if (value is JSObject)
                        property = parseComplexObject(payloadObject, (JSField)field);
                    else
                        property = parseSimpleObject(payloadObject, (JSField)field);

                    if (property.Name != "uri")
                        this.Resources = property;
                    else
                    {
                        // this is to match the resulting structure for parsing links in ATOM / json collections
                        PayloadObject temp = new PayloadObject(this);
                        temp.Name = property.Name;
                        temp.PayloadProperties.Add(property);
                        this.Resources = new List<PayloadObject>() { temp };
                    }
                }
            }
            else
            {
                this.Value = jsonString; // must be $value
            }
        }

        public static string ConvertJsonValue(object o)
        {
            if (o is System.DBNull)
                return null;

            //return System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.PrimitiveToStringUnquoted(o, o.GetType());
            return AstoriaUnitTests.Data.TypeData.XmlValueFromObject(o);
        }

        private List<PayloadObject> ParseSingleJsonObject(JSObject jsonData)
        {
            List<PayloadObject> payloadObjects = new List<PayloadObject>();
            payloadObjects.Add(this.parseJsonObject(jsonData));
            return payloadObjects;
        }

        private List<PayloadObject> ParseJsonArray(ArrayObject jsonData)
        {
            List<PayloadObject> payloadObjects = new List<PayloadObject>();

            for (int i = 0; i < (int)jsonData.length; i++)
            {
                JSObject resource = (JSObject)jsonData[i];
                payloadObjects.Add(parseJsonObject(resource));
            }

            return payloadObjects;
        }

        private PayloadObject parseJsonObject(JSObject jsObject)
        {
            PayloadObject payloadObject = new PayloadObject(this);

            FieldInfo[] fields = jsObject.GetFields(BindingFlags.Default);

            for (int i = 0; i < fields.Length; i++)
            {
                JSField field = (JSField)fields[i];
                var value = field.GetValue(field);

                if (value is ArrayObject)
                {
                    PayloadObject payloadNestedParentObject = new PayloadObject(this);
                    payloadNestedParentObject.Name = field.Name;

                    ArrayObject nestedPayloadObjects = (ArrayObject)field.GetValue(field);
                    payloadNestedParentObject.PayloadObjects.AddRange(this.ParseJsonArray(nestedPayloadObjects));

                    foreach (PayloadObject po in payloadNestedParentObject.PayloadObjects)
                    {
                        po.Name = field.Name;
                    }

                    payloadObject.PayloadObjects.Add(payloadNestedParentObject);
                }
                else if (value is JSObject)
                {
                    if (field.Name == "__metadata")
                    {
                        payloadObject.Uri = this.GetArrayString(field, "uri");
                        payloadObject.Type = this.GetArrayString(field, "type");

                        string etag = this.GetArrayString(field, "etag");
                        if (etag != null)
                        {
                            payloadObject.ETag = etag;
                        }
                    }
                    else
                    {
                        JSField firstChild = this.GetArrayField(field, 0);

                        if (firstChild != null && firstChild.Name == v2JsonResultsField)
                        {
                            PayloadObject payloadNestedParentObject = new PayloadObject(this);
                            payloadNestedParentObject.Name = field.Name;

                            ArrayObject nestedPayloadObjects = (ArrayObject)firstChild.GetValue(firstChild);
                            payloadNestedParentObject.PayloadObjects.AddRange(this.ParseJsonArray(nestedPayloadObjects));

                            foreach (PayloadObject po in payloadNestedParentObject.PayloadObjects)
                            {
                                po.Name = field.Name;
                            }

                            payloadObject.PayloadObjects.Add(payloadNestedParentObject);
                        }
                        else if (firstChild != null && firstChild.Name == "__deferred") // Deferred reference/collection
                        {
                            PayloadObject deferredObject = new PayloadObject(this);
                            deferredObject.Name = field.Name;
                            deferredObject.Uri = this.GetArrayString(firstChild, "uri");
                            deferredObject.Deferred = true;

                            payloadObject.PayloadObjects.Add(deferredObject);
                        }
                        else if (firstChild != null && firstChild.Name == "__mediaresource")
                        {
                            PayloadNamedStream stream = new PayloadNamedStream();
                            stream.Name = field.Name;
                            stream.ContentType = this.GetArrayString(firstChild, "content-type");
                            stream.EditLink = this.GetArrayString(firstChild, "edit_media");
                            stream.SelfLink = this.GetArrayString(firstChild, "media_src");
                            stream.ETag = this.GetArrayString(firstChild, "etag");
                            payloadObject.NamedStreams.Add(stream);
                        }
                        else
                        {
                            JSObject objectValue = (JSObject)field.GetValue(field);
                            var objectValueFields = objectValue.GetFields(BindingFlags.Default);
                            if (objectValueFields.Any(f => f.Name == "__metadata" && GetArrayString((JSField)f, "uri") != null))
                            {
                                PayloadObject referencePayloadObject = parseJsonObject(objectValue);
                                referencePayloadObject.Name = field.Name;
                                referencePayloadObject.Reference = true;

                                payloadObject.PayloadObjects.Add(referencePayloadObject);
                            }
                            else
                            {
                                PayloadComplexProperty payloadProperty = this.parseComplexObject(payloadObject, field);    // Complex object
                                payloadObject.PayloadProperties.Add(payloadProperty);
                            }
                        }
                    }
                }
                else
                {
                    PayloadProperty payloadProperty = this.parseSimpleObject(payloadObject, field);
                    payloadObject.PayloadProperties.Add(payloadProperty);
                }
            }

            return payloadObject;
        }

        internal PayloadSimpleProperty parseSimpleObject(PayloadObject parent, JSField field)
        {
            PayloadSimpleProperty payloadProperty = new PayloadSimpleProperty(parent);
            payloadProperty.Name = field.Name;

            object val = field.GetValue(field);

            if (val is System.DBNull)
            {
                payloadProperty.Value = null;
                payloadProperty.Type = null;
                payloadProperty.IsNull = true;
            }
            else
            {
                payloadProperty.Type = AstoriaUnitTests.Data.TypeData.FindForType(val.GetType()).GetEdmTypeName();
                payloadProperty.Value = ConvertJsonValue(val);
                payloadProperty.IsNull = false;
            }

            return payloadProperty;
        }

        internal PayloadComplexProperty parseComplexObject(PayloadObject parent, JSField field)
        {
            PayloadComplexProperty payloadProperty = new PayloadComplexProperty(parent);
            payloadProperty.Name = field.Name;

            JSObject fieldValue = (JSObject)field.GetValue(field);
            FieldInfo[] fieldInfo = fieldValue.GetFields(BindingFlags.Default);

            for (int j = 0; j < fieldInfo.Length; j++)
            {
                JSField currentField = (JSField)fieldInfo[j];

                if (currentField.GetValue(currentField) is JSObject)
                {
                    PayloadComplexProperty payloadComplexProperty = this.parseComplexObject(parent, (JSField)fieldInfo[j]);
                    payloadProperty.PayloadProperties.Add(payloadComplexProperty.Name, payloadComplexProperty);
                }
                else
                {
                    PayloadProperty payloadSimpleProperty = this.parseSimpleObject(parent, (JSField)fieldInfo[j]);
                    payloadProperty.PayloadProperties.Add(payloadSimpleProperty.Name, payloadSimpleProperty);
                }
            }

            return payloadProperty;
        }

        private string GetArrayString(JSField field, string name)
        {
            JSField jsField = this.GetArrayField(field, name);
            if (jsField != null)
                return jsField.GetValue(jsField).ToString();
            else
                return null;
        }

        private JSField GetArrayField(JSField field, int index)
        {
            JSObject fieldObject = (JSObject)field.GetValue(field);
            FieldInfo[] fields = fieldObject.GetFields(BindingFlags.Default);
            JSField indexField = (JSField)fields[index];

            return indexField;
        }

        private JSField GetArrayField(JSField field, string name)
        {
            JSObject fieldObject = (JSObject)field.GetValue(field);
            FieldInfo[] fields = fieldObject.GetFields(BindingFlags.Default);

            for (int i = 0; i < fields.Length; i++)
            {
                JSField indexField = (JSField)fields[i];
                if (indexField.Name == name)
                    return indexField;
            }

            return null;
        }

        public override void Compare(IQueryable baseline)
        {
            base.Compare(baseline);
        }

        protected override object ParseDate(string dateData)
        {
            if (dateData == null)
                return null;

            return Util.JsonPrimitiveTypesUtil.StringToPrimitive(dateData, typeof(DateTime));
        }
    }

    public class Evaluator
    {
        public static object EvalToObject(string statement)
        {
            return _evaluatorType.InvokeMember(
                        "Eval",
                        BindingFlags.InvokeMethod,
                        null,
                        _evaluator,
                        new object[] { statement }
                     );
        }

        static Evaluator()
        {
            ICodeCompiler compiler = new JScriptCodeProvider().CreateCompiler();

            CompilerParameters parameters;
            parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;

            CompilerResults results;
            results = compiler.CompileAssemblyFromSource(parameters, _jscriptSource);

            Assembly assembly = results.CompiledAssembly;
            _evaluatorType = assembly.GetType("Evaluator.Evaluator");

            _evaluator = Activator.CreateInstance(_evaluatorType);
        }

        private static object _evaluator = null;
        private static Type _evaluatorType = null;
        private static readonly string _jscriptSource =
            @"package Evaluator
            {
               class Evaluator
               {
                  public function Eval(expr : String) : Object 
                  { 
                     return eval(expr); 
                  }
               }
            }";
    }
}
