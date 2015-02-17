//---------------------------------------------------------------------
// <copyright file="UpdateVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.ModuleCore;
using System.Net;
using System.Text.RegularExpressions;
using System.Reflection;

namespace System.Data.Test.Astoria
{
    public class UpdateVerifier : PayloadVerifier
    {
        public static void Verify(AstoriaResponse response)
        {
            UpdateVerifier verifier = new UpdateVerifier(response);
            verifier.Verify();
        }

        public static bool Applies(AstoriaResponse response)
        {
            if (response.Request.EffectiveVerb != RequestVerb.Patch && response.Request.EffectiveVerb != RequestVerb.Put)
                return false;
            if (response.ActualStatusCode != HttpStatusCode.NoContent)
                return false;
            if (response.Request.IsBlobRequest)
                return false;
            if(response.Request.URI.Contains("$ref"))
                return false;
            return true;
        }

        private UpdateVerifier(AstoriaResponse response)
            : base(response)
        { }

        public T DefaultValue<T>()
        {
            return default(T);
        }

        public object DefaultValue(Type t)
        {
            MethodInfo method = this.GetType().GetMethod("DefaultValue", Type.EmptyTypes);
            method = method.MakeGenericMethod(t);
            return method.Invoke(this, null);
        }

        protected override void Verify()
        {
            if (!Applies(Response))
                return;

            AstoriaRequest request = Response.Request;

            if (!request.SnapshotForUpdate)
                return;

            try
            {
                Verify(request);
            }
            catch (Exception e)
            {
                AstoriaTestLog.WriteLineIgnore("Pre-update snapshot:");
                AstoriaTestLog.WriteLineIgnore(request.BeforeUpdatePayload.RawPayload);
                AstoriaTestLog.WriteLineIgnore("--------------------");
                AstoriaTestLog.WriteLineIgnore("Post-update snapshot:");
                AstoriaTestLog.WriteLineIgnore(request.AfterUpdatePayload.RawPayload);

                throw new TestFailedException("Update verification failed", null, null, e);
            }
        }


        private void Verify(AstoriaRequest request)
        {
            RequestVerb verb = request.EffectiveVerb;
            bool merge = verb == RequestVerb.Patch;

            PayloadObject entityBefore;
            if (!TryGetSingleObjectFromPayload(request.BeforeUpdatePayload, out entityBefore))
                AstoriaTestLog.FailAndThrow("Pre-update payload did not contain a single entity");

            // determine the type based on what was there (doing .Single() doesn't work for MEST, because the type shows up twice)
            ResourceType type = request.Workspace.ServiceContainer.ResourceTypes.First(rt => entityBefore.Type.Equals(rt.Namespace + "." + rt.Name));

            PayloadObject entityAfter;
            if (!TryGetSingleObjectFromPayload(request.AfterUpdatePayload, out entityAfter))
                AstoriaTestLog.FailAndThrow("Post-update payload did not contain a single entity");

            CommonPayload updatePayload = request.CommonPayload;
            PayloadObject entityUpdate = null;

            if (request.URI.EndsWith("$value"))
            {
                Match match = Regex.Match(request.URI, @".*/(.+)/\$value");
                if (!match.Success)
                    AstoriaTestLog.FailAndThrow("Could not determine property name for $value request");

                string propertyValue = null;
                if (request.ContentType.Equals(SerializationFormatKinds.MimeApplicationOctetStream, StringComparison.InvariantCultureIgnoreCase))
                {
                    object value = (request.UpdateTree as ResourceInstanceSimpleProperty).PropertyValue;
                    if (updatePayload.Format == SerializationFormatKind.JSON)
                        propertyValue = JSONPayload.ConvertJsonValue(value);
                    else
                        AstoriaTestLog.FailAndThrow("Unsure how to fake property value");
                }
                else
                {
                    propertyValue = request.Payload;
                }
                
                entityUpdate = new PayloadObject(updatePayload);
                PayloadSimpleProperty propertyUpdate = new PayloadSimpleProperty(entityUpdate);
                propertyUpdate.Name = match.Groups[1].Value;
                propertyUpdate.Value = propertyValue;
                entityUpdate.PayloadProperties.Add(propertyUpdate);
                merge = true; //PUT to a single property doesn't reset the resource
            }
            else if (!TryGetSingleObjectFromPayload(updatePayload, out entityUpdate))
            {
                // must be a single property update
                PayloadProperty propertyUpdate = updatePayload.Resources as PayloadProperty;
                if (propertyUpdate == null)
                    AstoriaTestLog.FailAndThrow("Expected either a property or an entity in the update payload");
                entityUpdate = new PayloadObject(updatePayload);
                entityUpdate.PayloadProperties.Add(propertyUpdate);
                propertyUpdate.ParentObject = entityUpdate;
                merge = true; //PUT to a single property doesn't reset the resource
            }

            List<string> allPropertyNames = entityBefore.PayloadProperties
                .Union(entityAfter.PayloadProperties)
                .Union(entityUpdate.PayloadProperties)
                .Select(p => p.Name)
                .ToList();
            allPropertyNames.AddRange(entityBefore.CustomEpmMappedProperties.Keys);
            allPropertyNames.AddRange(entityUpdate.CustomEpmMappedProperties.Keys);
            allPropertyNames.AddRange(entityAfter.CustomEpmMappedProperties.Keys);

            foreach (string propertyName in allPropertyNames.Distinct())
            {
                PayloadProperty original = null;
                bool originalHadProperty = entityBefore.PayloadProperties.Any(p => (original = p).Name == propertyName);
              
                PayloadProperty update = null;
                bool updateHadProperty = entityUpdate.PayloadProperties.Any(p => (update = p).Name == propertyName);
                
                PayloadProperty final = null;
                bool finalHadProperty = entityAfter.PayloadProperties.Any(p => (final = p).Name == propertyName);
                
                if (type.Properties.Any(p => p.Name == propertyName && p.Facets.IsDeclaredProperty))
                {
                    // declared property

                    if (!finalHadProperty)
                        AstoriaTestLog.FailAndThrow("Final version of entity is missing declared property '" + propertyName + "'");

                    ResourceProperty property = type.Properties[propertyName] as ResourceProperty;

                    if (property.IsNavigation)
                        continue;

                    if (!originalHadProperty)
                        AstoriaTestLog.FailAndThrow("Original version of entity is missing declared property '" + propertyName + "'");

                    bool checkValue = property.PrimaryKey == null && !property.Facets.ServerGenerated;

                    // if we changed it, we don't care what it was
                    if (updateHadProperty)
                        ComparePropertyValues(property, update, final, checkValue);
                    else if (merge)
                        ComparePropertyValues(property, original, final, checkValue);
                    else if(checkValue)
                        CompareResetProperty(property, final);
                }
                else
                {
                    // dynamic property
                    if (updateHadProperty)
                    {
                        if (!finalHadProperty)
                            AstoriaTestLog.FailAndThrow("Final version of entity is missing dynamic property '" + propertyName + "'");
                        CompareDynamicPropertyValues(update, final);
                    }
                    else if (merge)
                    {
                        if(!finalHadProperty)
                            AstoriaTestLog.FailAndThrow("Final version of entity is missing dynamic property '" + propertyName + "'");
                        CompareDynamicPropertyValues(original, final);
                    }
                    else if(finalHadProperty)
                    {
                        AstoriaTestLog.FailAndThrow("Dynamic property '" + propertyName + "' was not cleared after reset");
                    }
                }
            }
        }

        private void CompareResetProperty(NodeProperty property, PayloadProperty value)
        {
            bool complexType = property.Type is ComplexType;

            Workspace w = value.ParentObject.Payload.Workspace;
            bool expectNull = false;
            if (w.DataLayerProviderKind == DataLayerProviderKind.NonClr && (!property.Facets.IsClrProperty || complexType))
                expectNull = true;
            if (w.DataLayerProviderKind == DataLayerProviderKind.InMemoryLinq && complexType)
                expectNull = true;
            if (!property.Facets.IsDeclaredProperty)
                expectNull = true;
            if (property.Facets.Nullable)
                expectNull = true;
           
            if (property.Type is ComplexType)
            {
                if (value.IsNull)
                {
                    if (!expectNull)
                        AstoriaTestLog.FailAndThrow("Complex property '" + property.Name + " is unexpectedly null after reset");
                }
                else
                {
                    if (!(value is PayloadComplexProperty))
                        AstoriaTestLog.FailAndThrow("Property '" + property.Name + "' is complex, but a simple property was found instead");

                    ComplexType ct = property.Type as ComplexType;
                    PayloadComplexProperty complexProperty = value as PayloadComplexProperty;
                    foreach (NodeProperty subProperty in ct.Properties)
                    {
                        PayloadProperty subValue;
                        if (!complexProperty.PayloadProperties.TryGetValue(subProperty.Name, out subValue))
                            AstoriaTestLog.FailAndThrow("Property of complex type '" + ct.Name + "' missing sub-property '" + subProperty.Name + "'");

                        CompareResetProperty(subProperty, subValue);
                    }
                }
            }
            else
            {
                PayloadSimpleProperty simpleProperty = value as PayloadSimpleProperty;
                if (simpleProperty == null)
                    AstoriaTestLog.FailAndThrow("Property was unexpectedly not a simple property");

                Type propertyType = property.Type.ClrType;
                object expected = null;
                if (!expectNull)
                    expected = DefaultValue(propertyType);
                CommonPayload.ComparePrimitiveValuesObjectAndString(expected, propertyType, simpleProperty.Value, false, value.ParentObject.Format, true);
            }
        }
    }
}
