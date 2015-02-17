//---------------------------------------------------------------------
// <copyright file="AstoriaCanonicalFunctions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;

    /// <summary>
    /// Contains Astoria canonical functions.
    /// </summary>
    public static class AstoriaCanonicalFunctions
    {                
        /// <summary>
        /// Initializes static members of the AstoriaCanonicalFunctions class.
        /// </summary>
        static AstoriaCanonicalFunctions()
        {            
            // Math
            Ceiling = CreateFunction("System.Math", "Ceiling", "ceiling", ReturnTypeSameAsFirstInputType, LinqBuiltInFunctionKind.StaticMethod);
            Floor = CreateFunction("System.Math", "Floor", "floor", ReturnTypeSameAsFirstInputType, LinqBuiltInFunctionKind.StaticMethod);
            Round = CreateFunction("System.Math", "Round", "round", ReturnTypeSameAsFirstInputType, LinqBuiltInFunctionKind.StaticMethod);
            
            // String
            Concat = CreateFunction("System.String", "Concat", "concat", ReturnTypeSameAsFirstInputType, LinqBuiltInFunctionKind.StaticMethod);
            EndsWith = CreateFunction("System.String", "EndsWith", "endswith", EdmDataTypes.Boolean, LinqBuiltInFunctionKind.InstanceMethod);
            IndexOf = CreateFunction("System.String", "IndexOf", "indexof", EdmDataTypes.Int32, LinqBuiltInFunctionKind.InstanceMethod);
            Length = CreateFunction("System.String", "Length", "length", EdmDataTypes.Int32, LinqBuiltInFunctionKind.InstanceProperty);
            Replace = CreateFunction("System.String", "Replace", "replace", ReturnTypeSameAsFirstInputType, LinqBuiltInFunctionKind.InstanceMethod);
            StartsWith = CreateFunction("System.String", "StartsWith", "startswith", EdmDataTypes.Boolean, LinqBuiltInFunctionKind.InstanceMethod);
            Substring = CreateFunction("System.String", "Substring", "substring", ReturnTypeSameAsFirstInputType, LinqBuiltInFunctionKind.InstanceMethod);
            ToLower = CreateFunction("System.String", "ToLower", "tolower", ReturnTypeSameAsFirstInputType, LinqBuiltInFunctionKind.InstanceMethod);
            ToUpper = CreateFunction("System.String", "ToUpper", "toupper", ReturnTypeSameAsFirstInputType, LinqBuiltInFunctionKind.InstanceMethod);
            Trim = CreateFunction("System.String", "Trim", "trim", ReturnTypeSameAsFirstInputType, LinqBuiltInFunctionKind.InstanceMethod);
            
            // DateTime
            Day = CreateFunction("System.DateTime", "Day", "day", EdmDataTypes.Int32, LinqBuiltInFunctionKind.InstanceProperty);
            Hour = CreateFunction("System.DateTime", "Hour", "hour", EdmDataTypes.Int32, LinqBuiltInFunctionKind.InstanceProperty);
            Minute = CreateFunction("System.DateTime", "Minute", "minute", EdmDataTypes.Int32, LinqBuiltInFunctionKind.InstanceProperty);
            Month = CreateFunction("System.DateTime", "Month", "month", EdmDataTypes.Int32, LinqBuiltInFunctionKind.InstanceProperty);
            Second = CreateFunction("System.DateTime", "Second", "second", EdmDataTypes.Int32, LinqBuiltInFunctionKind.InstanceProperty);
            Year = CreateFunction("System.DateTime", "Year", "year", EdmDataTypes.Int32, LinqBuiltInFunctionKind.InstanceProperty);
        }

        /// <summary>
        /// Gets Ceiling canonical function.
        /// </summary>
        public static LinqBuiltInFunction Ceiling { get; private set; }

        /// <summary>
        /// Gets Floor canonical function.
        /// </summary>
        public static LinqBuiltInFunction Floor { get; private set; }

        /// <summary>
        /// Gets Round canonical function.
        /// </summary>
        public static LinqBuiltInFunction Round { get; private set; }

        /// <summary>
        /// Gets Concat canonical function.
        /// </summary>
        public static LinqBuiltInFunction Concat { get; private set; }

        /// <summary>
        /// Gets EndsWht canonical function.
        /// </summary>
        public static LinqBuiltInFunction EndsWith { get; private set; }

        /// <summary>
        /// Gets IndexOf canonical function.
        /// </summary>
        public static LinqBuiltInFunction IndexOf { get; private set; }

        /// <summary>
        /// Gets Length canonical function.
        /// </summary>
        public static LinqBuiltInFunction Length { get; private set; }

        /// <summary>
        /// Gets Replace canonical function.
        /// </summary>
        public static LinqBuiltInFunction Replace { get; private set; }

        /// <summary>
        /// Gets StartsWith canonical function.
        /// </summary>
        public static LinqBuiltInFunction StartsWith { get; private set; }

        /// <summary>
        /// Gets Substring canonical function.
        /// </summary>
        public static LinqBuiltInFunction Substring { get; private set; }

        /// <summary>
        /// Gets ToLower canonical function.
        /// </summary>
        public static LinqBuiltInFunction ToLower { get; private set; }

        /// <summary>
        /// Gets ToUpper canonical function.
        /// </summary>
        public static LinqBuiltInFunction ToUpper { get; private set; }

        /// <summary>
        /// Gets Trim canonical function.
        /// </summary>
        public static LinqBuiltInFunction Trim { get; private set; }

        /// <summary>
        /// Gets Day canonical function.
        /// </summary>
        public static LinqBuiltInFunction Day { get; private set; }

        /// <summary>
        /// Gets Hour canonical function.
        /// </summary>
        public static LinqBuiltInFunction Hour { get; private set; }

        /// <summary>
        /// Gets Minute canonical function.
        /// </summary>
        public static LinqBuiltInFunction Minute { get; private set; }

        /// <summary>
        /// Gets Month canonical function.
        /// </summary>
        public static LinqBuiltInFunction Month { get; private set; }

        /// <summary>
        /// Gets Second canonical function.
        /// </summary>
        public static LinqBuiltInFunction Second { get; private set; }

        /// <summary>
        /// Gets Year canonical function.
        /// </summary>
        public static LinqBuiltInFunction Year { get; private set; }
        
        private static QueryType ReturnTypeSameAsFirstInputType(QueryType[] argumentTypes)
        {
            ExceptionUtilities.CheckArgumentNotNull(argumentTypes, "argumentTypes");
            if (argumentTypes.Length == 0)
            {
                return QueryType.Unresolved;
            }

            return argumentTypes[0];
        }

        private static LinqBuiltInFunction CreateFunction(string className, string clrMethodOrPropertyName, string protocolFunctionName, Func<QueryType[], QueryType> returnTypeResolver, LinqBuiltInFunctionKind kind)
        {
            var function = new LinqBuiltInFunction(className, clrMethodOrPropertyName, new QueryBuiltInFunction(className, clrMethodOrPropertyName, returnTypeResolver), kind);
            function.Annotations.Add(new ODataCanonicalFunctionNameAnnotation() { Name = protocolFunctionName });
            return function;
        }

        private static LinqBuiltInFunction CreateFunction(string className, string clrMethodOrPropertyName, string protocolFunctionName, DataType returnType, LinqBuiltInFunctionKind kind)
        {
            var function = new LinqBuiltInFunction(className, clrMethodOrPropertyName, new QueryBuiltInFunction(className, clrMethodOrPropertyName, returnType), kind);
            function.Annotations.Add(new ODataCanonicalFunctionNameAnnotation() { Name = protocolFunctionName });
            return function;
        }
    }
}
