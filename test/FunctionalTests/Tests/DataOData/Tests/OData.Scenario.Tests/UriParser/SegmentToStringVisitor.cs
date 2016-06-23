//---------------------------------------------------------------------
// <copyright file="SegmentToStringVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser
{
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using System;
    using System.Linq;
    using Microsoft.Spatial;


    internal class SegmentToStringVisitor : PathSegmentTranslator<string>
    {
        public override string Translate(TypeSegment segment)
        {
            return string.Format("(Type: {0})", segment.EdmType.ToTraceString());
        }

        public override string Translate(NavigationPropertySegment segment)
        {
            return string.Format("(NavigationProperty: {0})", segment.NavigationProperty.Name);
        }

        public override string Translate(EntitySetSegment segment)
        {
            return string.Format("(EntitySet: {0})", segment.EntitySet.Name);
        }

        public override string Translate(SingletonSegment segment)
        {
            return string.Format("(Singleton: {0})", segment.Singleton.Name);
        }

        public override string Translate(KeySegment segment)
        {
            return string.Format("(Key: {0})", segment.Keys.Aggregate("", (current, key) => current + (key.Key + " = " + key.Value)));
        }

        public override string Translate(PropertySegment segment)
        {
            return string.Format("(Property: {0})", segment.Property.Name);
        }

        public override string Translate(OperationSegment segment)
        {
            string parameters = string.Empty;
            if (segment.Parameters.Any())
            {
                foreach (var parameter in segment.Parameters)
                {
                    parameters += parameter.Name + " = " + Translate(parameter) + ", ";
                }
                parameters = "(" + parameters.TrimEnd(',', ' ') + ")";
            }

            return string.Format("(Operation: {0}{1})", segment.Operations.ElementAt(0).Name, parameters);
        }

        public string Translate(OperationSegmentParameter value)
        {
            return WriteParameterValue(value.Value);
        }

        public override string Translate(DynamicPathSegment segment)
        {
            return string.Format("(OpenProperty: {0})", segment.Identifier);
        }

        public override string Translate(CountSegment segment)
        {
            return "($count)";
        }

        public override string Translate(NavigationPropertyLinkSegment segment)
        {
            return "($ref)" + segment.NavigationProperty.Name;
        }

        public override string Translate(BatchSegment segment)
        {
            return "($batch)";
        }

        public override string Translate(BatchReferenceSegment segment)
        {
            return string.Format("(${0})", segment.ContentId);
        }

        public override string Translate(ValueSegment segment)
        {
            return "($value)";
        }

        public override string Translate(MetadataSegment segment)
        {
            return "($metadata)";
        }

        public static string WriteParameterValue(object value)
        {
            if (value == null)
            {
                return "Null";
            }

            string outputString = String.Empty;

            ODataPrimitiveValue primitiveValue = value as ODataPrimitiveValue;
            if (primitiveValue != null)
            {
                value = primitiveValue.Value;
            }

            var geogPoint = value as GeographyPoint;
            if (geogPoint != null)
            {

                outputString = String.Format("GeographyPoint(Latitude = {0}, Longitude = {1}", geogPoint.Latitude, geogPoint.Longitude);

                if (geogPoint.M != null) outputString += String.Format(", M = {0}", geogPoint.M);
                if (geogPoint.Z != null) outputString += String.Format(", Z = {0}", geogPoint.Z);
                outputString += ")";
            }

            var geomPoint = value as GeometryPoint;
            if (geomPoint != null)
            {
                outputString = String.Format("GeometryPoint(X = {0}, Y = {1}", geomPoint.X, geomPoint.Y);

                if (geomPoint.M != null) outputString += String.Format(", M = {0}", geomPoint.M);
                if (geomPoint.Z != null) outputString += String.Format(", Z = {0}", geomPoint.Z);
                outputString += ")";
            }

            var str = value as string;
            if (str != null)
            {
                outputString = str;
            }

            if (value is double)
            {
                outputString = ((double)value).ToString();
            }

            if (value is int)
            {
                outputString = ((int)value).ToString();
            }

            var nullval = value as ODataNullValue;
            if (nullval != null)
            {
                outputString = "Null";
            }

            var aliasedparam = value as ODataUnresolvedFunctionParameterAlias;
            if (aliasedparam != null)
            {
                outputString = String.Format("Unresolved Aliased Parameter({0})", aliasedparam.Alias);
            }

            ConvertNode convertNode = value as ConvertNode;
            if (convertNode != null)
            {
                outputString = String.Format("ConvertNode(Type = {0}, Source = {1})", convertNode.TypeReference, WriteParameterValue(convertNode.Source));
            }

            ConstantNode constNode = value as ConstantNode;
            if (constNode != null)
            {
                outputString = String.Format("ConstantNode(Type = {0}, Value = {1})", constNode.TypeReference, WriteParameterValue(constNode.Value));
            }

            ParameterAliasNode parameterAliasNode = value as ParameterAliasNode;
            if (parameterAliasNode != null)
            {
                outputString = String.Format("ParameterAliasNode(Type Name = {0}, Alias Name = {1})", parameterAliasNode.TypeReference, parameterAliasNode.Alias);
            }

            if (outputString == String.Empty)
            {
                throw new NotImplementedException("Type for property value not yet supported in ToString visitor");
            }

            return outputString;
        }
    }
}
