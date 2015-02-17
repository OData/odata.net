//---------------------------------------------------------------------
// <copyright file="PayloadVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.ModuleCore;
using System.Net;
using AstoriaUnitTests.Data;
using System.Text.RegularExpressions;

namespace System.Data.Test.Astoria
{
    public abstract class PayloadVerifier
    {
        protected static bool TryGetSingleObjectFromPayload(CommonPayload payload, out PayloadObject payloadObject)
        {
            List<PayloadObject> list = payload.Resources as List<PayloadObject>;
            if (list == null || list.Count != 1)
            {
                payloadObject = null;
                return false;
            }
            payloadObject = list[0];
            return true;
        }

        protected PayloadVerifier(AstoriaResponse response)
        {
            Response = response;
        }

        protected AstoriaResponse Response
        {
            get;
            private set;
        }

        protected abstract void Verify();

        protected void ComparePropertyValues(NodeProperty property, PayloadProperty first, PayloadProperty second, bool checkValue)
        {
            // in case we need to do any facet-based logic
            ComparePropertyValues(property.Type, first, second, checkValue);
        }

        protected void ComparePropertyValues(NodeType type, PayloadProperty first, PayloadProperty second, bool checkValue)
        {
            if (type is ComplexType)
            {
                if(first.IsNull)
                {
                    if (!second.IsNull)
                        AstoriaTestLog.FailAndThrow("Second property is unexpectedly non-null");
                    return;
                }
                else if(second.IsNull)
                    AstoriaTestLog.FailAndThrow("Second property is unexpectedly null");

                PayloadComplexProperty firstComplex = first as PayloadComplexProperty;
                PayloadComplexProperty secondComplex = second as PayloadComplexProperty;

                if (firstComplex == null)
                    AstoriaTestLog.FailAndThrow("First property was not a complex property");
                if( secondComplex == null)
                    AstoriaTestLog.FailAndThrow("Second property was not a complex property");

                foreach (string propertyName in firstComplex.PayloadProperties.Keys.Union(secondComplex.PayloadProperties.Keys))
                {
                    // TODO: verify typing
                    if (propertyName == "__metadata")
                        continue;

                    PayloadProperty firstProperty;
                    bool firstHadProperty = firstComplex.PayloadProperties.TryGetValue(propertyName, out firstProperty);

                    PayloadProperty secondProperty;
                    bool secondHadProperty = secondComplex.PayloadProperties.TryGetValue(propertyName, out secondProperty);

                    // so that we can ignore this case later on, check it now
                    if (!firstHadProperty && !secondHadProperty)
                        AstoriaTestLog.FailAndThrow("Property list contained property '" + propertyName + "' despite neither complex property containing it");

                    // since a complex type is never open, there shouldnt be any unexpected properties
                    NodeProperty property = (type as ComplexType).Properties.SingleOrDefault(p => p.Name == propertyName);
                    if (property == null)
                    {
                        if (firstHadProperty && secondHadProperty)
                            AstoriaTestLog.FailAndThrow("Both complex properties contained sub-property '" + propertyName + "' which is not defined on type '" + type.Name + "'");
                        else if(firstHadProperty)
                            AstoriaTestLog.FailAndThrow("First complex property contained sub-property '" + propertyName + "' which is not defined on type '" + type.Name + "'");
                        else if (secondHadProperty)
                            AstoriaTestLog.FailAndThrow("Second complex property contained sub-property '" + propertyName + "' which is not defined on type '" + type.Name + "'");
                    }

                    if (!firstHadProperty)
                        AstoriaTestLog.FailAndThrow("First complex property property missing sub-property '" + propertyName + "'");
                    else if (!secondHadProperty)
                        AstoriaTestLog.FailAndThrow("Second complex property property missing sub-property '" + propertyName + "'");

                    ComparePropertyValues(property, firstProperty, secondProperty, checkValue && !property.Facets.ServerGenerated);
                }
            }
            else
            {
                PayloadSimpleProperty firstSimple = first as PayloadSimpleProperty;
                PayloadSimpleProperty secondSimple = second as PayloadSimpleProperty;

                if (firstSimple == null)
                    AstoriaTestLog.FailAndThrow("First property was not a simple property");
                if (secondSimple == null)
                    AstoriaTestLog.FailAndThrow("Second property was not a simple property");

                object expectedObject = CommonPayload.DeserializeStringToObject(firstSimple.Value, type.ClrType, false, first.ParentObject.Format);
                if (checkValue)
                    CommonPayload.ComparePrimitiveValuesObjectAndString(expectedObject, type.ClrType, secondSimple.Value, false, second.ParentObject.Format, true);
                else
                    CommonPayload.DeserializeStringToObject(secondSimple.Value, type.ClrType, false, second.ParentObject.Format);
            }
        }

        protected void CompareDynamicPropertyValues(PayloadProperty inserted, PayloadProperty returned)
        {
            if (inserted.IsNull || returned.IsNull)
            {
                if (!inserted.IsNull)
                    AstoriaTestLog.FailAndThrow("Null inserted value was non-null in return payload");
                if (!returned.IsNull)
                    AstoriaTestLog.FailAndThrow("Non-null inserted value was null in return payload");
            }
            else
            {
                NodeType insertType = GetDynamicPropertyType(inserted);
                NodeType returnType = GetDynamicPropertyType(returned);

                if (inserted.MappedOutOfContent == returned.MappedOutOfContent)
                {
                    bool equivalentTypes = false;
                    if (EquivalentValues(insertType, inserted, returnType, returned))
                        equivalentTypes = true;
                    if (EquivalentValues(returnType, returned, insertType, inserted))
                        equivalentTypes = true;

                    if (!equivalentTypes)
                        AstoriaTestLog.AreEqual(insertType, returnType, "Type of inserted/updated dynamic property value does not match returned type for property", false);
                }
                else if(inserted.MappedOutOfContent)
                {
                    insertType = returnType;
                }

                ComparePropertyValues(insertType, inserted, returned, true);
            }
        }

        protected bool EquivalentValues(NodeType firstType, PayloadProperty first, NodeType secondType, PayloadProperty second)
        {
            PayloadSimpleProperty firstSimple = first as PayloadSimpleProperty;
            PayloadSimpleProperty secondSimple = second as PayloadSimpleProperty;

            if (firstSimple == null || secondSimple == null)
                return false;

            if ((firstType == Clr.Types.Double || firstType == Clr.Types.Float))
            {
                if (second.ParentObject.Format == SerializationFormatKind.JSON)
                {
                    if (secondType == Clr.Types.Int16 || secondType == Clr.Types.Int32 || secondType == Clr.Types.Int64)
                    {
                        if (((long)double.Parse(firstSimple.Value)) == long.Parse(secondSimple.Value))
                            return true;
                    }
                }
            }
            return false;
        }

        protected NodeType GetDynamicPropertyType(PayloadProperty property)
        {
            string typeName = property.Type;
            if(typeName == null)
            {
                if (property.ParentObject.Format == SerializationFormatKind.JSON)
                {
                    if (property is PayloadComplexProperty)
                    {
                        PayloadComplexProperty complexProperty = property as PayloadComplexProperty;
                        PayloadProperty metadata;
                        if (!complexProperty.PayloadProperties.TryGetValue("__metadata", out metadata) || !(metadata is PayloadComplexProperty))
                            AstoriaTestLog.FailAndThrow("Complex type properties did not contain __metadata complex property");
                        PayloadProperty metadataType;
                        if(!(metadata as PayloadComplexProperty).PayloadProperties.TryGetValue("type", out metadataType) || !(metadataType is PayloadSimpleProperty))
                            AstoriaTestLog.FailAndThrow("__metadata property did not contain complex type's name");
                        typeName = (metadataType as PayloadSimpleProperty).Value;
                    }

                    if(typeName == null)
                    {
                        typeName = "String";
                    }
                }
                else
                {
                    typeName = "String";
                }
            }
            else if (typeName.StartsWith("Edm."))
            {
                typeName = typeName.Replace("Edm.", null);
            }

            if (typeName == "Binary")
                typeName = "Byte[]";

            // json datetime check
            if (typeName == "String" && property.ParentObject.Format == SerializationFormatKind.JSON && property is PayloadSimpleProperty)
            {
                PayloadSimpleProperty simpleProperty = property as PayloadSimpleProperty;
                if (Util.JsonPrimitiveTypesUtil.DateTimeRegex.IsMatch(simpleProperty.Value))
                    typeName = "DateTime";
            }

            NodeType type = Clr.Types.SingleOrDefault(t => t.Name.Equals(typeName));
            
            if (type == null)
                type = Response.Workspace.ServiceContainer.ComplexTypes.SingleOrDefault(ct => typeName.Equals(ct.Namespace + "." + ct.Name));
            
            if (type == null)
                AstoriaTestLog.FailAndThrow("Could not find type '" + typeName + "'");
            
            return type;
        }
    }
}
