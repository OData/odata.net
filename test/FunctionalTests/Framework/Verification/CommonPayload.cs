//---------------------------------------------------------------------
// <copyright file="CommonPayload.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using Microsoft.OData.Client;
    using Microsoft.Test.ModuleCore;
#if !ClientSKUFramework
    using System.Data.Test.Astoria.NonClr;
    using System.Data.Objects.DataClasses;
#endif
#if !ClientSKUFramework
    using System.Data.Linq.Mapping;
#endif
    using System.Globalization;
    using System.Data.Objects;
    using System.Data.Test.Astoria.FullTrust;

    public abstract class CommonPayload
    {
        #region static methods
        public static CommonPayload CreateCommonPayload(AstoriaRequestResponseBase rr)
        {
            if (rr == null)
                throw new ArgumentNullException("rr");

            SerializationFormatKind format;
            if (rr is AstoriaRequest)
                format = (rr as AstoriaRequest).Format;
            else
                format = (rr as AstoriaResponse).Request.Format;

            switch (format)
            {
                case SerializationFormatKind.JSON:
                    return new JSONPayload(rr);
            }

            throw new Exception("Unrecognized format");
        }

        public static void ComparePrimitiveValuesObjectAndString(object expected, Type expectedType, string actual, bool valueUri, SerializationFormatKind serializationKind, bool throwOnError)
        {
            object actualObject = DeserializeStringToObject(actual, expectedType, valueUri, serializationKind);
            string expectedStr = "null";
            if (expected != null)
                expectedStr = AstoriaUnitTests.Data.TypeData.XmlValueFromObject(expected);

            string errorMessage = String.Format("Error Primitive Value of type:{0} not equal: \r\n\tExpected:\t{1} \r\n\tActual:\t{2}", expectedType.Name, expectedStr, actual);

            //Fixups
            if (expected != null && expectedType.Equals(typeof(string)))
            {
                if (actualObject != null)
                    actualObject = (actualObject as string).Trim();
                expected = (expected as string).Trim();
            }

            if (expectedType == typeof(System.Xml.Linq.XElement))
            {
                expectedStr = null;
                if (expected != null)
                {
                    System.Xml.Linq.XElement xElement = expected as System.Xml.Linq.XElement;
                    expectedStr = xElement.ToString();
                }
                AstoriaTestLog.AreEqual(expectedStr, actual, errorMessage);
            }
            else if (expectedType == typeof(Single) || expectedType == typeof(Double))
            {
                if (serializationKind == SerializationFormatKind.JSON)
                {
                    if (expectedType == typeof(Double))
                    {
                        Double d;
                        if (expected != null)
                        {
                            d = Convert.ToDouble(expected);
                            expectedStr = d.ToString("r", System.Globalization.CultureInfo.InvariantCulture);
                        }
                        if (actual != null)
                        {
                            if (double.TryParse(actual, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out d))
                                actual = d.ToString("r", CultureInfo.InvariantCulture.NumberFormat);
                        }
                    }
                    else if (expectedType == typeof(Single))
                    {
                        Single s;
                        if (expected != null)
                        {
                            s = Convert.ToSingle(expected);
                            expectedStr = s.ToString("r", System.Globalization.CultureInfo.InvariantCulture);
                        }
                        if (actual != null)
                        {
                            if (Single.TryParse(actual, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out s))
                                actual = s.ToString("r", CultureInfo.InvariantCulture.NumberFormat);
                        }
                    }
                }
                //Fixup
                if (expectedStr == "null")
                    expectedStr = null;
                if (expectedStr != null && actual != null)
                {
                    expectedStr = expectedStr.Trim();
                    actual = actual.Trim();
                }
                if (throwOnError)
                {
                    if (expectedStr != null)
                        AstoriaTestLog.Compare(expectedStr.Equals(actual), errorMessage);
                    else
                    {
                        AstoriaTestLog.Compare(actual == null, errorMessage);
                    }
                }
                else
                    AstoriaTestLog.AreEqual(expectedStr, actual, errorMessage);
            }
#if !ClientSKUFramework

            //For some reason byte[] comparision is not working correctly
            else if ((expectedType == typeof(byte[]) || expectedType == typeof(System.Data.Linq.Binary)) && valueUri)
            {

                string expectedBytesStr = null;
                if (expected == null)
                    expectedBytesStr = null;
                else if (expectedType == typeof(byte[]))
                    expectedBytesStr = (new System.Text.UTF8Encoding()).GetString((byte[])expected);


                else if (expectedType == typeof(System.Data.Linq.Binary))
                {
                    System.Data.Linq.Binary binary = expected as System.Data.Linq.Binary;
                    expectedBytesStr = (new System.Text.UTF8Encoding()).GetString((byte[])binary.ToArray());
                }

                if (throwOnError)
                {
                    if (expected == null)
                        AstoriaTestLog.Compare(null == actual, errorMessage);
                    else
                        AstoriaTestLog.Compare(expectedBytesStr.Equals(actual), errorMessage);
                }
                else
                    AstoriaTestLog.AreEqual(expectedBytesStr, actual, errorMessage);
            }
#endif

            else if (expectedType == typeof(byte[]) && valueUri == false)
            {
                AreBytesEqual((byte[])expected, (byte[])actualObject, throwOnError);
            }
            else
            {
                if (throwOnError)
                {
                    bool objectsEqual = true;
                    if (expected == null)
                    {
                        if (actualObject != null)
                            objectsEqual = false;
                    }
                    else
                        objectsEqual = expected.Equals(actualObject);
                    AstoriaTestLog.Compare(objectsEqual, errorMessage);
                }
                else
                    AstoriaTestLog.AreEqual(expected, actualObject, errorMessage);
            }
        }

        internal static object DeserializeStringToObject(string val, Type valType, bool valueUri, SerializationFormatKind serializationKind)
        {
            object rehydratedObject = null;
            if (val == null)
                rehydratedObject = null;
            else
            {
                if (valueUri)
                {
                    if (valType == typeof(byte[]))
                    {
                        if (val != null)
                            return new UTF8Encoding().GetBytes(val);
                        return null;
                    }
#if !ClientSKUFramework


                    else if (valType == typeof(System.Data.Linq.Binary))
                    {
                        byte[] bytes = new UTF8Encoding().GetBytes(val);
                        System.Data.Linq.Binary binary = new System.Data.Linq.Binary(bytes);
                        return binary;
                    }
#endif

                    else
                    {
                        rehydratedObject = AstoriaUnitTests.Data.TypeData.ObjectFromXmlValue(val, valType);
                    }
                }
                else if (serializationKind == SerializationFormatKind.JSON)
                {
                    //Special case that seems to be failing
                    if (valType == typeof(DateTime) && val.Equals("null", StringComparison.InvariantCulture))
                        rehydratedObject = null;
                    else
                        rehydratedObject = System.Data.Test.Astoria.Util.JsonPrimitiveTypesUtil.StringToPrimitive(val, valType);
                }
                else //xml
                {
                    rehydratedObject = AstoriaUnitTests.Data.TypeData.ObjectFromXmlValue(val, valType);
                }
            }
            return rehydratedObject;
        }

        private static void AreBytesEqual(byte[] expected, byte[] actual, bool throwOnError)
        {
            if (expected == null && actual == null)
                return;
            else if (expected == null && actual != null)
            {
                if (throwOnError)
                    AstoriaTestLog.Compare(false, "Expected Actual to be null like expected");
                else
                    AstoriaTestLog.AreEqual(expected, actual, "Bytes not equal");
                return;
            }
            else if (expected != null && actual == null)
            {
                if (throwOnError)
                    AstoriaTestLog.Compare(false, "Expected Actual to be a value like expected");
                else
                    AstoriaTestLog.AreEqual(expected, actual, "Bytes not equal");
                return;
            }
            if (throwOnError)
                AstoriaTestLog.Compare(expected.Length == actual.Length, "Bytes lengths are not equal");
            else
                AstoriaTestLog.AreEqual(expected.Length, actual.Length, "Bytes lengths are not equal");
            bool errorFound = false;
            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] != actual[i])
                {
                    errorFound = true;
                    string errorMessage = String.Format("Byte arrays not equal at index {0}, expected:{1} actual:{2}", i, expected[i], actual[i]);
                    if (throwOnError)
                    {
                        AstoriaTestLog.Compare(expected[i] == actual[i], errorMessage);
                    }
                    else
                    {
                        AstoriaTestLog.AreEqual(expected.Length, actual.Length, errorMessage);
                    }
                }
                if (errorFound == true)
                    break;
            }

        }

        internal static object GetSingleValueFromIQueryable(IQueryable querable, bool throwOnError)
        {
            object val = null;
            IEnumerator enumerator = querable.GetEnumerator();
            bool moved = enumerator.MoveNext();
            if (!moved && throwOnError)
                throw new TestFailedException("Expected to find one result but none exist in Iqueryable");
            else if (!moved && !throwOnError)
            {
                AstoriaTestLog.WriteLine("Error occured, cannot find a result in IQueryable");
                AstoriaTestLog.FailureFound = true;
            }
            else
            {
                val = enumerator.Current;
            }

            moved = enumerator.MoveNext();
            if (moved && throwOnError)
                throw new TestFailedException("Expected to find one result but found 2 exist in Iqueryable");
            else if (moved && !throwOnError)
            {
                AstoriaTestLog.WriteLine("Expected to find one result but found 2 exist in Iqueryable");
                AstoriaTestLog.FailureFound = true;
            }
            return val;
        }
        #endregion

        protected CommonPayload(AstoriaRequestResponseBase rr)
        {
            if (rr is AstoriaRequest)
            {
                this.Request = rr as AstoriaRequest;
                this.Response = null;
                this.WasResponse = false;
            }
            else
            {
                this.Response = rr as AstoriaResponse;
                this.Request = this.Response.Request;
                this.WasResponse = true;
            }
        }

        public string RawPayload
        {
            get
            {
                if (WasResponse)
                    return this.Response.Payload;
                else
                    return this.Request.Payload;
            }
        }

        public long? Count
        {
            get;
            set;
        }

        public string NextLink
        {
            get;
            set;
        }

        public SerializationFormatKind Format
        {
            get
            {
                return this.Request.Format;
            }
        }

        public AstoriaRequest Request
        {
            get;
            private set;
        }

        public AstoriaResponse Response
        {
            get;
            private set;
        }

        public bool WasResponse
        {
            get;
            private set;
        }

        public object Resources
        {
            get;
            protected set;
        }

        public string Value
        {
            get;
            protected set;
        }

        public Workspace Workspace
        {
            get
            {
                return this.Request.Workspace;
            }
        }

        #region comparison
        public virtual void CompareValue(IQueryable baseline)
        {
            this.CompareValue(baseline, false, false);
        }

        public virtual void CompareSingleValue(object expected, bool stripQuotes, bool valueUri)
        {
            if (this.Resources == null && expected == null)
                return;

            PayloadSimpleProperty simpleProperty = this.Resources as PayloadSimpleProperty;

            if (simpleProperty == null)
                AstoriaTestLog.FailAndThrow("Payload did not represent a single value");

            if (expected == null)
            {
                if (!simpleProperty.IsNull)
                    AstoriaTestLog.FailAndContinue(new TestFailedException("Compare failed - Expected: null, got: " + simpleProperty.Value));
            }
            else
                ComparePrimitiveValuesObjectAndString(expected, expected.GetType(), simpleProperty.Value, valueUri, this.Format, false);
        }

        public virtual void CompareValue(IQueryable baseline, bool stripQuotes, bool valueUri)
        {
            // TODO: There should only be 1 element in baseline
            foreach (object element in TrustedMethods.IQueryableGetResultList(baseline))
            {
                object expected = element;
                CompareSingleValue(expected, stripQuotes, valueUri);
            }
        }

        public virtual void CompareCountInline(IQueryable objectsInResponse, object baselineCount)
        {
            if (baselineCount == null) baselineCount = 0;

            AstoriaTestLog.Compare(this.Count == (Int32)baselineCount, "Count in Payload not equal to Baseline");

            // Compare the other objects normally.
            Compare(objectsInResponse);
        }

        public virtual void Compare(IQueryable baseline)
        {
            System.Collections.ArrayList baseLineEntities = CommonPayload.CreateList(baseline);

            if (this.Resources is PayloadComplexProperty)
            {
                PayloadComplexProperty complexProperty = (PayloadComplexProperty)this.Resources;
                this.CompareComplexType(complexProperty, baseLineEntities[0], false);
            }
            else if (this.Resources is PayloadSimpleProperty)
            {
                PayloadSimpleProperty simpleProperty = (PayloadSimpleProperty)this.Resources;
                this.CompareSimpleType(simpleProperty, baseLineEntities[0], false);
            }
            else if (this.Resources is List<PayloadObject>)
            {
                List<PayloadObject> payloadObjects = (List<PayloadObject>)this.Resources;
                Compare(baseLineEntities.OfType<object>().ToList(), payloadObjects, false);
            }
        }

        private void Compare(IList<object> baselineObjects, IList<PayloadObject> payloadObjects, bool ignoreOrder)
        {
            AstoriaTestLog.AreEqual(baselineObjects.Count, payloadObjects.Count, "Count of baseline not equal to returned elements.", false);

            for (int position = 0; position < baselineObjects.Count; position++)
            {
                PayloadObject fromPayload = payloadObjects[position];
                object fromBaseline = null;

                if (ignoreOrder)
                {
                    foreach (object entity in baselineObjects)
                    {
                        if (this.CompareProperties(fromPayload, entity, false))
                        {
                            fromBaseline = entity;
                            break;
                        }
                    }

                    if (fromBaseline == null)
                        throw new TestFailedException("Could not find matching entity");
                    else
                        baselineObjects.Remove(fromBaseline);
                }
                else
                {
                    fromBaseline = baselineObjects[position];

                    try
                    {
                        this.CompareProperties(fromPayload, fromBaseline, true);
                    }
                    catch (Exception e)
                    {
                        throw new TestFailedException("Payload properties do not match baseline for entity at position " + position + " in payload", null, null, e);
                    }
                }

                try
                {
                    this.CompareObjects(fromPayload, fromBaseline);
                }
                catch (Exception e)
                {
                    throw new TestFailedException("Payload objects do not match baseline for entity at position " + position + " in payload", null, null, e);
                }
            }
        }

        internal void CompareObjects(PayloadObject payloadObject, object element)
        {
            bool found;

            foreach (PayloadObject nestedObject in payloadObject.PayloadObjects)
            {
                if (nestedObject.Deferred)
                {
                    // do something
                }
                else if (nestedObject.Reference)
                {
                    object nestedElement = LoadReference(element, nestedObject.Name);

                    //this.CompareUri(referenceObject.Uri, referenceObject.Id, referenceObject.Name, nestedObject.Name);
                    this.CompareProperties(nestedObject, nestedElement, true);
                    this.CompareObjects(nestedObject, nestedElement);
                }
                else
                {
                    // Call load on nested element
                    List<object> enumerableList = LoadCollection(element, nestedObject.Name).OfType<object>().ToList();
                    Compare(enumerableList, nestedObject.PayloadObjects, true);
                }
            }
        }

        internal bool CompareProperties(PayloadObject payloadObject, object element, bool throwOnFailure)
        {
            // Verify resource properties and navigation
            foreach (PayloadProperty property in payloadObject.PayloadProperties)
            {
                if (property is PayloadComplexProperty)
                {
                    PayloadComplexProperty payloadProperty = (PayloadComplexProperty)property;
                    if (!this.CompareComplexType(payloadProperty, element, true))
                    {
                        if (throwOnFailure)
                            throw new TestFailedException("Value for complex property '" + property.Name + "' does not batch baseline");
                        return false;
                    }
                }
                else if (property is PayloadSimpleProperty)
                {
                    PayloadSimpleProperty payloadProperty = (PayloadSimpleProperty)property;
                    if (!this.CompareSimpleType(payloadProperty, element, true))
                    {
                        if (throwOnFailure)
                            throw new TestFailedException("Value for simple property '" + property.Name + "' does not batch baseline");
                        return false;
                    }
                }
            }
            return true;
        }

        internal bool CompareComplexType(PayloadComplexProperty payloadProperty, object element, bool isEntity)
        {
            bool passResult = true;
            PropertyInfo propertyInfo;
            Object complexType;

            string propertyName = payloadProperty.Name;

            if (propertyName == "__metadata" || propertyName == "__deferred")
                return true; // TODO: single objects should be payloadobjects

            if (isEntity)
            {
                if (element is System.Collections.ArrayList)
                {
                    propertyInfo = element.GetType().GetProperty("Item");
                    complexType = propertyInfo.GetValue(element, new object[] { 0 });
                }
#if !ClientSKUFramework

                else if (element is RowEntityType)
                {
                    propertyInfo = null;
                    complexType = ((RowEntityType)element).Properties[propertyName];
                }
                else if (element is RowComplexType)
                {
                    propertyInfo = null;
                    complexType = ((RowComplexType)element).Properties[propertyName];
                }
#endif

                else if (element.GetType().Name.Contains("Anonymous"))
                {
                    //mfrintu
                    complexType = element;

                }
                else
                {
                    propertyInfo = element.GetType().GetProperty(propertyName);
                    complexType = propertyInfo.GetValue(element, null);
                }

                if (complexType == null)
                {
                    complexType = LoadReference(element, propertyName);
                    if (complexType == null)
                        return true;
                }
            }
            else
            {
                complexType = element;
            }

            foreach (KeyValuePair<string, PayloadProperty> pair in payloadProperty.PayloadProperties)
            {
                if (pair.Value is PayloadComplexProperty)
                {
                    PayloadComplexProperty complexProperty = (PayloadComplexProperty)pair.Value;
                    if (!this.CompareComplexType(complexProperty, complexType, true))
                        passResult = false;
                }
                else if (pair.Value is PayloadSimpleProperty)
                {
                    PayloadSimpleProperty simpleProperty = (PayloadSimpleProperty)pair.Value;
                    if (!this.CompareSimpleType(simpleProperty, complexType, true))
                        passResult = false;
                }
            }

            return passResult;
        }

        internal bool CompareSimpleType(PayloadSimpleProperty payloadProperty, object element, bool isEntity)
        {
            Object expectedResult, actualResult = null;

            // Verify property data
            string propertyName = payloadProperty.Name;

            if (isEntity)
            {
#if !ClientSKUFramework

                if (element is RowEntityType)
                {
                    expectedResult = ((RowEntityType)element).Properties[propertyName];
                }

                else if (element is RowComplexType)
                {
                    expectedResult = ((RowComplexType)element).Properties[propertyName];
                }

                else
#endif

                {
                    PropertyInfo propertyInfo = element.GetType().GetProperty(propertyName);

                    if (propertyInfo == null)
                    {
                        AstoriaTestLog.TraceInfo(String.Format("Couldn't find property {0}", propertyName));
                        return true;
                    }

                    expectedResult = propertyInfo.GetValue(element, null);
                }
            }
            else
            {
                expectedResult = element;
            }

            if (expectedResult is System.Byte[])
            {
                int actualInt32Result;
                object newExpectedResult;

                // If prop value is int, then it just contains the length, not the acutally binary data so compare that
                if (Int32.TryParse(payloadProperty.Value, out actualInt32Result))
                {
                    actualResult = actualInt32Result;
                    newExpectedResult = ((byte[])expectedResult).Length;
                }
                else
                {
                    actualResult = payloadProperty.Value;
                    newExpectedResult = System.Convert.ToBase64String((System.Byte[])expectedResult);
                }

                return this.InternalEquals(actualResult, newExpectedResult);
            }
            else
            {
                if (expectedResult == null)
                {
                    actualResult = payloadProperty.Value;
                }
                else if (expectedResult is System.DateTime)
                {
                    switch (((DateTime)expectedResult).Kind)
                    {
                        case DateTimeKind.Local:
                            expectedResult = ((DateTime)expectedResult).ToUniversalTime();
                            break;
                        case DateTimeKind.Unspecified:
                            expectedResult = new DateTime(((DateTime)expectedResult).Ticks, DateTimeKind.Utc);
                            break;
                        case DateTimeKind.Utc:
                            break;
                    }
                    actualResult = this.ParseDate(payloadProperty.Value);
                }
                else if (expectedResult is System.Guid)
                {
                    expectedResult = ((Guid)expectedResult).ToString();
                    actualResult = payloadProperty.Value;
                }
#if !ClientSKUFramework
                else if (expectedResult is System.Data.Linq.Binary || expectedResult is System.Xml.Linq.XElement)
                {
                    actualResult = StripQuotes(payloadProperty.Value);
                    expectedResult = StripQuotes(expectedResult.ToString());
                }
#endif
                else
                {
                    if (payloadProperty.Value == null)
                        actualResult = null;
                    else
                    {
#if !ClientSKUFramework
                        actualResult = AstoriaUnitTests.Data.TypeData.ObjectFromXmlValue(payloadProperty.Value, expectedResult.GetType());
#endif
                    }


                }

                return this.InternalEquals(actualResult, expectedResult);
            }
        }

        public static System.Collections.ArrayList CreateList(IQueryable baseline)
        {
            // Get all the data into a list (this closes reader so that "Load" can be called on nested elements)
            System.Collections.ArrayList list = new System.Collections.ArrayList();

            // we know that this thing is potentially a DataServiceQuery, so get the enumerator safely
            IEnumerator enumerator = SocketExceptionHandler.Execute<IEnumerator>(baseline.GetEnumerator);
            while (enumerator.MoveNext())
                list.Add(enumerator.Current);

            return list;
        }

        private static string StripQuotes(string value)
        {
            if ((null != value) && (2 <= value.Length) && (value[0] == '\"') && (value[value.Length - 1] == '\"'))
            {
                value = value.Substring(1, value.Length - 2);
            }
            return value;
        }

        protected virtual object ParseDate(string dateData)
        {
            if (dateData == null)
                return null;

            return DateTime.Parse(dateData);
        }

        internal bool InternalEquals(object actual, object expected)
        {
            //Handle null comparison
            if (actual == null && expected == null)
                return true;
            else if (actual == null || expected == null)
                return false;
            else if (Convert.IsDBNull(expected))
                return Convert.IsDBNull(actual);

            //Otherwise
            return expected.Equals(actual);
        }

        private object LoadReference(object entity, string propertyName)
        {
            return LoadProperty<object>(entity, propertyName);
        }

        private IEnumerable LoadCollection(object entity, string propertyName)
        {
            return LoadProperty<IEnumerable>(entity, propertyName);
        }

        private T LoadProperty<T>(object entity, string propertyName)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            object value;
            if (TryLoadProperty_EF(entity, propertyName, out value))
                return (T)value;

            // L2S types have the DataServiceKey attribute, need to try them first
            if (TryLoadProperty_L2S(entity, propertyName, out value))
                return (T)value;

            if (TryLoadProperty_Client(entity, propertyName, out value))
                return (T)value;
#if !ClientSKUFramework


            if (TryLoadProperty_RowEntityType(entity, propertyName, out value))
                return (T)value;
#endif

            if (TryLoadProperty_Reflection(entity, propertyName, out value))
                return (T)value;

            AstoriaTestLog.FailAndThrow("Could not load property '" + propertyName + "' on entity of type '" + entity.GetType() + "'");
            return default(T);
        }

        private bool TryLoadProperty_EF(object entity, string propertyName, out object propertyValue)
        {
            propertyValue = null;

#if !ClientSKUFramework

            Type type = ObjectContext.GetObjectType(entity.GetType());
            var edmWorkspace = this.Workspace as EdmWorkspace;
            if (edmWorkspace == null)
            {
                return false;
            }

            // Get object context which was used to load the entity
            ObjectContext ctx = edmWorkspace.GetAssociatedObjectContext(entity);
            if (ctx == null)
            {
                return false;
            }

            // Load navigation property
            // catch invalid operation exception if violating multiplicity constraints
            try
            {
                ctx.LoadProperty(entity, propertyName);
            }
            catch (InvalidOperationException)
            {
                return true;
            }

            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                return false;
            }

            propertyValue = propertyInfo.GetValue(entity, null);
#endif
            return true;
        }

        private bool TryLoadProperty_L2S(object entity, string propertyName, out object propertyValue)
        {
            propertyValue = null;

            Type type = entity.GetType();
#if !ClientSKUFramework

            if (!type.GetCustomAttributes(typeof(TableAttribute), true).Any())
                return false;
#endif
            return TryLoadProperty_Reflection(entity, propertyName, out propertyValue);
        }

        private bool TryLoadProperty_Client(object entity, string propertyName, out object propertyValue)
        {
            propertyValue = null;

            Type type = entity.GetType();
            if (!type.GetCustomAttributes(typeof(KeyAttribute), true).Any())
                return false;

            PropertyInfo property = type.GetProperty(propertyName);
            if (propertyName == null)
                return false;

            WebDataCtxWrapper ctx = WebDataCtxWrapper.MostRecentContext;
            ctx.LoadProperty(entity, propertyName);

            propertyValue = property.GetValue(entity, null);
            return true;
        }
#if !ClientSKUFramework


        private bool TryLoadProperty_RowEntityType(object entity, string propertyName, out object propertyValue)
        {
            propertyValue = null;

            RowEntityType row = entity as RowEntityType;
            if (row == null)
                return false;

            return row.Properties.TryGetValue(propertyName, out propertyValue);
        }
#endif


        private bool TryLoadProperty_Reflection(object entity, string propertyName, out object propertyValue)
        {
            propertyValue = null;

            PropertyInfo property = entity.GetType().GetProperty(propertyName);
            if (property == null)
                return false;

            propertyValue = property.GetValue(entity, null);
            return true;
        }
        #endregion
    }
}
