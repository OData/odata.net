//---------------------------------------------------------------------
// <copyright file="ConcurrencyUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AstoriaUnitTests.Data;

namespace System.Data.Test.Astoria
{
    public static class ConcurrencyUtil
    {
        public const string WeakPrefix = "W/";
        public const string IfMatchHeader = "If-Match";
        public const string IfNoneMatchHeader = "If-None-Match";
        public const string ETagHeader = "ETag";
        public const string Wildcard = "*";
        public const string Null = "null";

        public static string PrimitiveToETagString(object value)
        {
            if (value == null)
                return Null;

            // only expect/use the D when its a v2 server
            bool useDLiteralForDouble = Versioning.Server.SupportsV2Features;

            string valueString = TypeData.FormatForKey(value, null, useDLiteralForDouble);

            // fixups for floating point types
            Type t = value.GetType();

            if (t == typeof(Single))
            {
                if (valueString.EndsWith(".0f"))
                    valueString = valueString.Replace(".0f", "f");
            }

            if (t == typeof(Double))
            {
                if (valueString.EndsWith(".0D"))
                    valueString = valueString.Replace(".0D", "D");
                else if (!Versioning.Server.SupportsV2Features && valueString.EndsWith(".0"))
                    valueString = valueString.Replace(".0", string.Empty);

                // for some silly reason, TypeData.FormatForKey does this for already for doubles UNLESS IT PUTS THE 'D'
                if (useDLiteralForDouble)
                    valueString = Uri.EscapeDataString(valueString);
            }
            else
            {
                // for some silly reason, TypeData.FormatForKey does this for already for doubles UNLESS IT PUTS THE 'D'
                valueString = Uri.EscapeDataString(valueString);
            }
            return valueString;
        }

        public static string ConstructETag(ResourceContainer container, IDictionary<string, object> propertyValues)
        {
            ResourceType type = container.BaseType;

            List<NodeProperty> etagProperties = type.Properties.Where(p => p.Facets.ConcurrencyModeFixed).ToList();

            string[] etagValues = etagProperties.Select(p => PrimitiveToETagString(propertyValues[p.Name])).ToArray();
            return ConstructETag(etagValues);
        }

        public static KeyExpression ConstructKey(ResourceContainer container, PayloadObject entity)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            ResourceType type = container.BaseType;
            if (entity.Type != type.Namespace + "." + type.Name)
            {
                type = container.ResourceTypes.SingleOrDefault(rt => rt.Namespace + "." + rt.Name == entity.Type);
                if (type == null)
                    AstoriaTestLog.FailAndThrow("Could not find resource type for payload type value: " + entity.Type);
            }

            foreach (ResourceProperty property in type.Properties.OfType<ResourceProperty>())
            {
                if (property.IsNavigation || property.IsComplexType)
                    continue;

                string propertyName = property.Name;
                string valueString;
                if (entity.PayloadProperties.Any(p => p.Name == propertyName))
                {
                    PayloadSimpleProperty simpleProperty = entity[propertyName] as PayloadSimpleProperty;
                    if (simpleProperty == null)
                        continue;

                    valueString = simpleProperty.Value;
                }
                else
                {
                    continue;
                }

                object value = CommonPayload.DeserializeStringToObject(valueString, property.Type.ClrType, false, entity.Format);
                if (value is DateTime && entity.Format == SerializationFormatKind.JSON)
                {
                    // TODO: this is because the server will make any JSON datetime into UTC, and currently we always send 'unspecified' values
                    value = new DateTime(((DateTime)value).Ticks, DateTimeKind.Unspecified);
                }

                properties[propertyName] = value;
            }

            return new KeyExpression(container, type, properties);
        }

        public static string ConstructETag(ResourceContainer container, PayloadObject entity)
        {
            KeyExpression key = ConstructKey(container, entity);
            return key.ETag;
        }

        public static string ConstructETag(string[] values)
        {
            return ConstructETag(values, true);
        }

        public static string ConstructETag(string[] values, bool weak)
        {
            string strongETag;
            if (!values.Any())
                strongETag = null;
            else
                strongETag = '"' + string.Join(",", values) + '"';

            if (weak)
                return WeakPrefix + strongETag;
            else
                return strongETag;
        }

        public static string ConstructRandomETag(ResourceContainer container)
        {
            ResourceType type = container.BaseType;
            
            Dictionary<string, object> randomProperties = new Dictionary<string, object>();

            foreach (NodeProperty p in type.Properties.Where(p => p.Facets.ConcurrencyModeFixed))
                randomProperties[p.Name] = (p as ResourceProperty).CreateRandomResourceSimpleInstanceProperty().PropertyValue;

            return ConstructETag(container, randomProperties);
        }

        public static string ConstructEquivalentETag(ResourceType type, string ETag)
        {
            string[] pieces = SplitETag(ETag);
            List<NodeProperty> etagProperties = type.Properties.Where(p => p.Facets.ConcurrencyModeFixed).ToList();
            for (int i = 0; i < pieces.Length; i++)
            {
                string piece = pieces[i];
                if (piece == Null)
                    continue;

                NodeProperty property = etagProperties[i];
                NodeType propertyType = etagProperties[i].Type;
                if (propertyType == Clr.Types.String && property.Facets.FixedLength)
                {
                    piece = piece.Trim('\'');
                    if (piece.Length < property.Facets.MaxSize)
                        piece = piece + " ";
                    else if (piece.EndsWith(" "))
                        piece = piece.Remove(piece.Length - 1);
                    piece = "'" + piece + "'";
                }
                else if (propertyType.IsNumeric)
                {
                    if (piece.Contains("INF") || piece.Contains("NaN"))
                        continue;
                    else if (piece.ToLower().Contains("e")) //must be a floating point
                    {
                        // add 0's and flip capitalization
                        piece = piece.Replace("E%2b", "0e%2B0");
                        piece = piece.Replace("e%2b", "0E%2B0");
                        piece = piece.Replace("E%2B", "0e%2b0");
                        piece = piece.Replace("e%2B", "0E%2b0");
                        piece = piece.Replace("E+", "0e+0");
                        piece = piece.Replace("e+", "0E+0");
                    }
                    else if (propertyType.ClrType == typeof(double) && !(piece.EndsWith("D") || piece.EndsWith("d")))
                    {
                        if (!piece.Contains('.'))
                            piece = piece + ".0";
                        else
                            piece = piece + "0";
                 
                        piece = piece + "E+0";
                    }
                    else if (propertyType.ClrType == typeof(float) || propertyType.ClrType == typeof(Single) || propertyType.ClrType == typeof(double))
                    {
                        if (!piece.Contains('.'))
                            piece = piece.Insert(piece.Length - 1, ".0"); //just before the 'f' or 'D'
                        else
                            piece = piece.Insert(piece.Length - 1, "0"); //just before the 'f' or 'D'

                        piece = piece.Insert(piece.Length - 1, "E+0");
                    }
                    else if (propertyType.ClrType == typeof(decimal))
                    {
                        if (!piece.Contains('.'))
                            piece = piece.Insert(piece.Length - 1, ".0"); //just before the 'M'
                        else
                            piece = piece.Insert(piece.Length - 1, "0"); //just before the 'M'
                    }
                
                    if (piece.StartsWith("-"))
                        piece = piece.Insert(1, "0");
                    else
                        piece = "0" + piece;
                }
                
                pieces[i] = piece;
            }

            return ConstructETag(pieces);
        }

        public static string[] SplitETag(string ETag)
        {
            if (ETag.StartsWith(WeakPrefix))
                ETag = ETag.Remove(0, WeakPrefix.Length);
            ETag = ETag.Trim('"');
            string[] split = ETag.Split(',');
            return split;
        }

        public static Dictionary<string, string> ParseETag(ResourceType type, string ETag)
        {
            string[] values = SplitETag(ETag);
            List<string> etagProperties = type.Properties.Where(p => p.Facets.ConcurrencyModeFixed).Select(p => p.Name).ToList();
            if (values.Length != etagProperties.Count)
            {
                AstoriaTestLog.FailAndThrow("Could not parse etag");
                return null;
            }
            else
            {
                Dictionary<string, string> etagMap = new Dictionary<string, string>();
                for (int i = 0; i < values.Length; i++)
                {
                    etagMap[etagProperties[i]] = values[i];
                }
                return etagMap;
            }
        }
    }
}
