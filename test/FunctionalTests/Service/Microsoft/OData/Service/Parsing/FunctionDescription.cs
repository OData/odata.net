//---------------------------------------------------------------------
// <copyright file="FunctionDescription.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Spatial;

    /// <summary>Use this class to represent a system function for Astoria expressions.</summary>
    [DebuggerDisplay("FunctionDescription {name}")]
    internal class FunctionDescription
    {
        /// <summary>Function name for type casts.</summary>
        private const string FunctionNameCast = "cast";

        /// <summary>Function name for type checks.</summary>
        private const string FunctionNameIsOf = "isof";

        /// <summary>CLR function name for replace.</summary>
        private const string FunctionNameClrStringReplace = "Replace";

        /// <summary>CLR member for property or method invocation.</summary>
        private readonly MemberInfo member;

        /// <summary>Function name.</summary>
        private readonly string name;

        /// <summary>Parameter types for method invocation.</summary>
        private readonly Type[] parameterTypes;

        /// <summary>Conversion to expression for this function.</summary>
        private Func<Expression, Expression[], Expression> conversionFunction;

        /// <summary>Initializes a new <see cref="FunctionDescription"/>.</summary>
        /// <param name="member">CLR member for property or method invocation.</param>
        /// <param name="parameterTypes">Parameter types for method invocation.</param>
        public FunctionDescription(MemberInfo member, Type[] parameterTypes)
            : this(member, parameterTypes, null, member.Name)
        {
        }

        /// <summary>Initializes a new <see cref="FunctionDescription"/>.</summary>
        /// <param name="name">Name for conversion function.</param>
        /// <param name="parameterTypes">Parameter types for method invocation.</param>
        /// <param name="conversionFunction">Conversion to expression for this function.</param>
        public FunctionDescription(string name, Type[] parameterTypes, Func<Expression, Expression[], Expression> conversionFunction)
            : this(null, parameterTypes, conversionFunction, name)
        {
        }

        /// <summary>Initializes a new <see cref="FunctionDescription"/>.</summary>
        /// <param name="member">CLR member for property or method invocation.</param>
        /// <param name="parameterTypes">Parameter types for method invocation.</param>
        /// <param name="conversionFunction">Conversion to expression for this function.</param>
        /// <param name="name">Name for conversion function.</param>
        private FunctionDescription(
            MemberInfo member,
            Type[] parameterTypes,
            Func<Expression, Expression[], Expression> conversionFunction,
            string name)
        {
            this.member = member;
            this.parameterTypes = parameterTypes;
            this.conversionFunction = conversionFunction;
            this.name = name;

            // infer boolean flags based on the function name
            this.IsTypeCast = name == FunctionNameCast;
            this.IsTypeCheckOrCast = this.IsTypeCast || name == FunctionNameIsOf;
            this.IsReplace = name == FunctionNameClrStringReplace;

            // by default, all functions other than 'isof' and 'cast' require null propagation
            // some functions will have this overridden later
            this.RequiresNullPropagation = !this.IsTypeCheckOrCast;
        }

        /// <summary>Conversion to expression for this function.</summary>
        public Func<Expression, Expression[], Expression> ConversionFunction
        {
            [DebuggerStepThrough]
            get { return this.conversionFunction; }
            [DebuggerStepThrough]
            set { this.conversionFunction = value; }
        }

        /// <summary>Gets a value indicating whether this function is a type check or cast.</summary>
        public bool IsTypeCheckOrCast
        {
            get;
            private set;
        }

        /// <summary>Gets a value indicating whether this function is a type cast.</summary>
        public bool IsTypeCast
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns true if the function is a Replace function, otherwise returns false.
        /// </summary>
        public bool IsReplace
        {
            get;
            private set;
        }

        /// <summary>Gets a value indicating whether this function requires null propagation on its parameters.</summary>
        public bool RequiresNullPropagation
        {
            get;
            set;
        }

        /// <summary>Parameter types for method invocation.</summary>
        public Type[] ParameterTypes
        {
            [DebuggerStepThrough]
            get { return this.parameterTypes; }
        }

        /// <summary>Performs an instance method invocation.</summary>
        /// <param name="target">"it" expression; unused by this function.</param>
        /// <param name="arguments">Arguments for method invocation; first one should be the target 'this'.</param>
        /// <returns>A new expression with the method invocation.</returns>
        public Expression InstanceMethodConversionFunction(Expression target, Expression[] arguments)
        {
            Expression instanceArgument = arguments[0];
            Expression[] methodArguments = new Expression[arguments.Length - 1];
            Array.Copy(arguments, 1, methodArguments, 0, arguments.Length - 1);
            return Expression.Call(instanceArgument, (MethodInfo)this.member, methodArguments);
        }

        /// <summary>Performs a static method invocation.</summary>
        /// <param name="target">"it" expression; unused by this function.</param>
        /// <param name="arguments">Arguments for method invocation.</param>
        /// <returns>A new expression with the method invocation.</returns>
        public Expression StaticMethodConversionFunction(Expression target, Expression[] arguments)
        {
            return Expression.Call((MethodInfo)this.member, arguments);
        }

        /// <summary>Performs an instance property access.</summary>
        /// <param name="target">"it" expression; unused by this function.</param>
        /// <param name="arguments">Argument for property access; instance.</param>
        /// <returns>A new expression with the property access.</returns>
        public Expression InstancePropertyConversionFunction(Expression target, Expression[] arguments)
        {
            return Expression.Property(arguments[0], (PropertyInfo)this.member);
        }

        /// <summary>
        /// Invoke the open typed method for this function.
        /// </summary>
        /// <param name="arguments">list of parameters to pass to the late bound method.</param>
        /// <returns>A new expression with the late bound function</returns>
        public Expression InvokeOpenTypeMethod(Expression[] arguments)
        {
            Debug.Assert(arguments != null, "arguments != null");
            Debug.Assert(arguments.Length == this.ParameterTypes.Length, "arguments.Length == this.ParameterTypes.Length");

            Type[] argumentTypes = new Type[this.parameterTypes.Length];
            for (int i = 0; i < argumentTypes.Length; i++)
            {
                argumentTypes[i] = typeof(object);
            }

            MethodInfo methodInfo = typeof(OpenTypeMethods).GetMethod(
                this.name,
                BindingFlags.Static | BindingFlags.Public,
                null,
                argumentTypes,
                null);

            Debug.Assert(methodInfo != null, "methodInfo != null");
            return Expression.Call(null, methodInfo, arguments);
        }

        /// <summary>Builds a list of function signatures.</summary>
        /// <param name="name">Function name.</param>
        /// <param name="descriptions">Function descriptions.</param>
        /// <returns>A string with ';'-separated list of function signatures.</returns>
        internal static string BuildSignatureList(string name, IEnumerable<FunctionDescription> descriptions)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            string descriptionSeparator = string.Empty;
            foreach (FunctionDescription description in descriptions)
            {
                builder.Append(descriptionSeparator);
                descriptionSeparator = "; ";

                string parameterSeparator = string.Empty;
                builder.Append(name);
                builder.Append('(');
                foreach (Type type in description.ParameterTypes)
                {
                    builder.Append(parameterSeparator);
                    parameterSeparator = ", ";

                    Type underlyingType = Nullable.GetUnderlyingType(type);
                    if (underlyingType != null)
                    {
                        builder.Append(underlyingType.FullName);
                        builder.Append('?');
                    }
                    else
                    {
                        builder.Append(type.FullName);
                    }
                }

                builder.Append(')');
            }

            return builder.ToString();
        }

        /// <summary>Creates and populates a dictionary of system functions.</summary>
        /// <returns>A new dictionary of functions.</returns>
        internal static Dictionary<string, FunctionDescription[]> CreateFunctions()
        {
            Dictionary<string, FunctionDescription[]> result = new Dictionary<string, FunctionDescription[]>(StringComparer.Ordinal);

            // String functions.
            result.Add("endswith", new FunctionDescription[] { StringInstanceFunction("EndsWith", typeof(string)) });
            result.Add("indexof", new FunctionDescription[] { StringInstanceFunction("IndexOf", typeof(string)) });
            result.Add("replace", new FunctionDescription[] { StringInstanceFunction(FunctionNameClrStringReplace, typeof(string), typeof(string)) });
            result.Add("startswith", new FunctionDescription[] { StringInstanceFunction("StartsWith", typeof(string)) });
            result.Add("tolower", new FunctionDescription[] { StringInstanceFunction("ToLower", Type.EmptyTypes) });
            result.Add("toupper", new FunctionDescription[] { StringInstanceFunction("ToUpper", Type.EmptyTypes) });
            result.Add("trim", new FunctionDescription[] { StringInstanceFunction("Trim", Type.EmptyTypes) });

            FunctionDescription[] signatures = new FunctionDescription[]
                                                   {
                                                       StringInstanceFunction("Substring", typeof(int)),
                                                       StringInstanceFunction("Substring", typeof(int), typeof(int)) 
                                                   };
            result.Add("substring", signatures);

            signatures = new FunctionDescription[]
            {
                new FunctionDescription("Contains", new Type[] { typeof(string), typeof(string) }, Contains)
            };
            result.Add("contains", signatures);

            signatures = new FunctionDescription[]
            {
                CreateFunctionDescription(typeof(string), false /* instance */, true /* method */, "Concat", typeof(string), typeof(string))
            };
            result.Add("concat", signatures);

            signatures = new FunctionDescription[]
            { 
                CreateFunctionDescription(typeof(string), true /* instance */, false /* method */, "Length", Type.EmptyTypes)
            };
            result.Add("length", signatures);

            // DateTime, DateTimeOffset and Duration functions
            signatures = new FunctionDescription[]
            {
                CreatePropertyBasedFunction(typeof(DateTime), "Year"),
                CreatePropertyBasedFunction(typeof(DateTimeOffset), "Year")
            };
            result.Add("year", signatures);

            signatures = new FunctionDescription[]
            {
                CreatePropertyBasedFunction(typeof(DateTime), "Month"),
                CreatePropertyBasedFunction(typeof(DateTimeOffset), "Month"),
            };
            result.Add("month", signatures);

            signatures = new FunctionDescription[]
            {
                CreatePropertyBasedFunction(typeof(DateTime), "Day"),
                CreatePropertyBasedFunction(typeof(DateTimeOffset), "Day"),
            };
            result.Add("day", signatures);

            signatures = new FunctionDescription[]
            {
                CreatePropertyBasedFunction(typeof(DateTime), "Hour"),
                CreatePropertyBasedFunction(typeof(DateTimeOffset), "Hour"),
                CreatePropertyBasedFunction(typeof(TimeSpan), "Hours"),
            };
            result.Add("hour", signatures);

            signatures = new FunctionDescription[]
            {
                CreatePropertyBasedFunction(typeof(DateTime), "Minute"),
                CreatePropertyBasedFunction(typeof(DateTimeOffset), "Minute"),
                CreatePropertyBasedFunction(typeof(TimeSpan), "Minutes"),
            };
            result.Add("minute", signatures);

            signatures = new FunctionDescription[]
            {
                CreatePropertyBasedFunction(typeof(DateTime), "Second"),
                CreatePropertyBasedFunction(typeof(DateTimeOffset), "Second"),
                CreatePropertyBasedFunction(typeof(TimeSpan), "Seconds"),
            };
            result.Add("second", signatures);

            // TODO: what function do we need to add for GetTotalOffsetMinutes

            // Mathematical functions.
            result.Add("round", MathFunctionArray("Round"));
            result.Add("floor", MathFunctionArray("Floor"));
            result.Add("ceiling", MathFunctionArray("Ceiling"));

            // Type functions.
            signatures = new FunctionDescription[]
            {
                new FunctionDescription(FunctionNameIsOf, new Type[] { typeof(Type) }, FunctionDescription.UnaryIsOf),
                new FunctionDescription(FunctionNameIsOf, new Type[] { typeof(object), typeof(Type) }, FunctionDescription.BinaryIsOf),
                new FunctionDescription(FunctionNameIsOf, new Type[] { typeof(ResourceType) }, FunctionDescription.UnaryIsOfResourceType),
                new FunctionDescription(FunctionNameIsOf, new Type[] { typeof(object), typeof(ResourceType) }, FunctionDescription.BinaryIsOfResourceType),
            };
            result.Add(FunctionNameIsOf, signatures);

            // For cast() signatures, we need to add all primitive types directly as well as the object (open type)
            // and unary versions; otherwise expression will convert to object, then again to whatever other type
            // is required.
            Microsoft.OData.Service.Providers.ResourceType[] primitiveTypes = PrimitiveResourceTypeMap.TypeMap.AllPrimitives;
            List<FunctionDescription> castSignatures = new List<FunctionDescription>(primitiveTypes.Length + 4);
            for (int i = 0; i < primitiveTypes.Length; i++)
            {
                Debug.Assert(
                    primitiveTypes[i].InstanceType != typeof(Type),
                    "primitiveTypes[i].Type != typeof(Type) -- otherwise extra signatures will be added for cast()");
                Debug.Assert(
                    primitiveTypes[i].InstanceType != typeof(object),
                    "primitiveTypes[i].Type != typeof(object) -- otherwise extra signatures will be added for cast()");
                castSignatures.Add(new FunctionDescription(FunctionNameCast, new Type[] { primitiveTypes[i].InstanceType, typeof(Type) }, FunctionDescription.BinaryCast));
            }

            castSignatures.Add(new FunctionDescription(FunctionNameCast, new Type[] { typeof(Type) }, FunctionDescription.UnaryCast));
            castSignatures.Add(new FunctionDescription(FunctionNameCast, new Type[] { typeof(object), typeof(Type) }, FunctionDescription.BinaryCast));
            castSignatures.Add(new FunctionDescription(FunctionNameCast, new Type[] { typeof(ResourceType) }, FunctionDescription.UnaryCastResourceType));
            castSignatures.Add(new FunctionDescription(FunctionNameCast, new Type[] { typeof(object), typeof(ResourceType) }, FunctionDescription.BinaryCastResourceType));

            result.Add(FunctionNameCast, castSignatures.ToArray());

            #region Spatial Functions
            // geo.distance()
            var geoDistanceSignatures = new List<FunctionDescription>()
            {
                CreateFunctionDescription(typeof(GeographyOperationsExtensions), false /* instance */, true /* method */, "Distance", typeof(GeographyPoint), typeof(GeographyPoint)),
                CreateFunctionDescription(typeof(GeometryOperationsExtensions), false /* instance */, true /* method */, "Distance", typeof(GeometryPoint), typeof(GeometryPoint)),
            };

            // spatial functions do not require null propagation
            foreach (var geoDistanceSignature in geoDistanceSignatures)
            {
                geoDistanceSignature.RequiresNullPropagation = false;
            }

            result.Add("geo.distance", geoDistanceSignatures.ToArray());

            // geo.length()
            var geoLengthSignatures = new List<FunctionDescription>()
            {
                CreateFunctionDescription(typeof(GeographyOperationsExtensions), false /* instance */, true /* method */, "Length", typeof(GeographyLineString)),
                CreateFunctionDescription(typeof(GeometryOperationsExtensions), false /* instance */, true /* method */, "Length", typeof(GeometryLineString)),
            };

            // spatial functions do not require null propagation
            foreach (var tmp in geoLengthSignatures)
            {
                tmp.RequiresNullPropagation = false;
            }

            result.Add("geo.length", geoLengthSignatures.ToArray());

            // geo.intersects()
            var geoIntersectsSignatures = new List<FunctionDescription>()
            {
                CreateFunctionDescription(typeof(GeographyOperationsExtensions), false /* instance */, true /* method */, "Intersects", typeof(GeographyPoint), typeof(GeographyPolygon)),
                CreateFunctionDescription(typeof(GeometryOperationsExtensions), false /* instance */, true /* method */, "Intersects", typeof(GeometryPoint), typeof(GeometryPolygon)),
            };

            // spatial functions do not require null propagation
            foreach (var tmp in geoIntersectsSignatures)
            {
                tmp.RequiresNullPropagation = false;
            }

            result.Add("geo.intersects", geoIntersectsSignatures.ToArray());
            #endregion

            return result;
        }

        /// <summary>Transforms a URI-style "contains(a,b)" function into "a.contains(b)".</summary>
        /// <param name="target">Target of query; not used.</param>
        /// <param name="arguments">Arguments to function.</param>
        /// <returns>The conversion for this method.</returns>
        internal static Expression Contains(Expression target, Expression[] arguments)
        {
            Debug.Assert(arguments != null, "arguments != null");
            Debug.Assert(arguments.Length == 2, "arguments.Length == 2");

            const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;
            Type[] parameterTypes = new Type[] { typeof(string) };
            MethodInfo method = typeof(string).GetMethod("Contains", Flags, null, parameterTypes, null);
            Debug.Assert(method != null, "method != null -- otherwise couldn't find string.Contains(string)");
            return Expression.Call(arguments[0], method, arguments[1]);
        }

        /// <summary>Performs a type check for the "it" expression.</summary>
        /// <param name="target">"it" expression.</param>
        /// <param name="arguments">Argument for type check; type.</param>
        /// <returns>A new expression with the type check.</returns>
        internal static Expression UnaryIsOf(Expression target, Expression[] arguments)
        {
            ConstantExpression ce = (ConstantExpression)arguments[0];
            return Expression.TypeIs(target, (Type)ce.Value);
        }

        /// <summary>Performs a type check for a given expression.</summary>
        /// <param name="target">"it" expression; unused by this function.</param>
        /// <param name="arguments">Arguments for type check; instance and type.</param>
        /// <returns>A new expression with the type check.</returns>
        internal static Expression BinaryIsOf(Expression target, Expression[] arguments)
        {
            ConstantExpression ce = (ConstantExpression)arguments[1];
            return Expression.TypeIs(arguments[0], (Type)ce.Value);
        }

        /// <summary>Performs a type check for the "it" expression.</summary>
        /// <param name="target">"it" expression.</param>
        /// <param name="arguments">Argument for type check; type.</param>
        /// <returns>A new expression with the type check.</returns>
        internal static Expression UnaryIsOfResourceType(Expression target, Expression[] arguments)
        {
            Debug.Assert(arguments != null, "arguments != null");
            Debug.Assert(arguments.Length == 1, "arguments.Length == 1");
            Debug.Assert(arguments[0].NodeType == ExpressionType.Constant, "Constant expression expected for argument[0]");
            Debug.Assert(((ConstantExpression)arguments[0]).Type == typeof(ResourceType), "Constant expression type should be ResourceType");
            return Expression.Call(null, DataServiceProviderMethods.TypeIsMethodInfo, target, arguments[0]);
        }

        /// <summary>Performs a type check for a given expression.</summary>
        /// <param name="target">"it" expression; unused by this function.</param>
        /// <param name="arguments">Arguments for type check; instance and resource type.</param>
        /// <returns>A new expression with the type check.</returns>
        internal static Expression BinaryIsOfResourceType(Expression target, Expression[] arguments)
        {
            Debug.Assert(arguments != null, "arguments != null");
            Debug.Assert(arguments.Length == 3, "arguments.Length == 3");
            Debug.Assert(arguments[1].NodeType == ExpressionType.Constant, "Constant expression expected for argument[1]");
            Debug.Assert(((ConstantExpression)arguments[1]).Type == typeof(ResourceType), "Constant expression type should be ResourceType");
            Debug.Assert(arguments[2].NodeType == ExpressionType.Constant, "Constant expression expected for argument[2]");
            Debug.Assert(((ConstantExpression)arguments[2]).Type == typeof(bool), "Constant expression type should be boolean");

            bool callOpenTypeMethod = (bool)((ConstantExpression)arguments[2]).Value;
            return Expression.Call(null, callOpenTypeMethod ? OpenTypeMethods.TypeIsMethodInfo : DataServiceProviderMethods.TypeIsMethodInfo, arguments[0], arguments[1]);
        }

        /// <summary>Performs a cast for the "it" expression.</summary>
        /// <param name="target">"it" expression.</param>
        /// <param name="arguments">Argument for cast; type.</param>
        /// <returns>A new expression with the cast.</returns>
        internal static Expression UnaryCast(Expression target, Expression[] arguments)
        {
            Debug.Assert(arguments.Length == 1, "arguments.Length == 1");
            ConstantExpression ce = (ConstantExpression)arguments[0];
            return Expression.Convert(target, (Type)ce.Value);
        }

        /// <summary>Performs a cast for a given expression.</summary>
        /// <param name="target">"it" expression; unused by this function.</param>
        /// <param name="arguments">Arguments for cast; instance and type.</param>
        /// <returns>A new expression with the cast.</returns>
        internal static Expression BinaryCast(Expression target, Expression[] arguments)
        {
            Debug.Assert(arguments.Length == 2, "arguments.Length == 2");
            ConstantExpression ce = (ConstantExpression)arguments[1];

            // Work around for Protocol: exception thrown in XML with filter=null
            //
            // We need this in place so we can recognize null constant reliably and generate
            // trees that work for both LINQ to Entities and LINQ to Objects for the cases where
            // conversions of null literals generate expressions that don't guard for nulls in the 
            // EF case, but EF ends up calling them anyway because they can be evaluated on the client.
            Type targetType = (Type)ce.Value;
            if (ExpressionUtils.IsNullConstant(arguments[0]))
            {
                targetType = WebUtil.GetTypeAllowingNull(targetType);
                return Expression.Constant(null, targetType);
            }

            return Expression.Convert(arguments[0], targetType);
        }

        /// <summary>Performs a cast for the "it" expression.</summary>
        /// <param name="target">"it" expression.</param>
        /// <param name="arguments">Argument for cast; type.</param>
        /// <returns>A new expression with the cast.</returns>
        internal static Expression UnaryCastResourceType(Expression target, Expression[] arguments)
        {
            Debug.Assert(arguments != null, "arguments != null");
            Debug.Assert(arguments.Length == 1, "arguments.Length == 1");
            Debug.Assert(arguments[0].NodeType == ExpressionType.Constant, "Constant expression expected for argument[0]");
            Debug.Assert(((ConstantExpression)arguments[0]).Type == typeof(ResourceType), "Constant expression type should be ResourceType");
            return Expression.Call(null, DataServiceProviderMethods.ConvertMethodInfo, target, arguments[0]);
        }

        /// <summary>Performs a cast for a given expression.</summary>
        /// <param name="target">"it" expression; unused by this function.</param>
        /// <param name="arguments">Arguments for cast; instance and type.</param>
        /// <returns>A new expression with the cast.</returns>
        internal static Expression BinaryCastResourceType(Expression target, Expression[] arguments)
        {
            Debug.Assert(arguments != null, "arguments != null");
            Debug.Assert(arguments.Length == 3, "arguments.Length == 3");
            Debug.Assert(arguments[1].NodeType == ExpressionType.Constant, "Constant expression expected for argument[1]");
            Debug.Assert(((ConstantExpression)arguments[1]).Type == typeof(ResourceType), "Constant expression type should be ResourceType");
            Debug.Assert(arguments[2].NodeType == ExpressionType.Constant, "Constant expression expected for argument[2]");
            Debug.Assert(((ConstantExpression)arguments[2]).Type == typeof(bool), "Constant expression type should be boolean");

            bool callOpenTypeMethod = (bool)((ConstantExpression)arguments[2]).Value;
            return Expression.Call(null, callOpenTypeMethod ? OpenTypeMethods.ConvertMethodInfo : DataServiceProviderMethods.ConvertMethodInfo, arguments[0], arguments[1]);
        }

        /// <summary>Creates a new function description for a method or property.</summary>
        /// <param name="targetType">Type on which property or method is declared.</param>
        /// <param name="instance">Whether an instance member is looked for.</param>
        /// <param name="method">Whether a method (rather than a property) is looked for.</param>
        /// <param name="name">Name of member.</param>
        /// <param name="parameterTypes">Parameter types.</param>
        /// <returns>A new function description.</returns>
        private static FunctionDescription CreateFunctionDescription(
            Type targetType,
            bool instance,
            bool method,
            string name,
            params Type[] parameterTypes)
        {
            Debug.Assert(targetType != null, "targetType != null");
            Debug.Assert(name != null, "name != null");
            Debug.Assert(parameterTypes.Length == 0 || method, "parameterTypes.Length == 0 || method");
            Debug.Assert(method || instance, "method || instance");

            BindingFlags flags = BindingFlags.Public | (instance ? BindingFlags.Instance : BindingFlags.Static);
            MemberInfo member;

            if (method)
            {
                member = targetType.GetMethod(name, flags, null, parameterTypes, null);
                Debug.Assert(member != null, "methodInfo != null");
            }
            else
            {
                member = targetType.GetProperty(name, flags);
                Debug.Assert(member != null, "propertyInfo != null");
            }

            Type[] functionParameterTypes;
            if (instance)
            {
                functionParameterTypes = new Type[parameterTypes.Length + 1];
                functionParameterTypes[0] = targetType;
                parameterTypes.CopyTo(functionParameterTypes, 1);
            }
            else
            {
                functionParameterTypes = parameterTypes;
            }

            FunctionDescription result = new FunctionDescription(member, functionParameterTypes);
            if (method)
            {
                if (instance)
                {
                    result.ConversionFunction = new Func<Expression, Expression[], Expression>(result.InstanceMethodConversionFunction);
                }
                else
                {
                    result.ConversionFunction = new Func<Expression, Expression[], Expression>(result.StaticMethodConversionFunction);
                }
            }
            else
            {
                Debug.Assert(instance, "instance");
                result.ConversionFunction = new Func<Expression, Expression[], Expression>(result.InstancePropertyConversionFunction);
            }

            return result;
        }

        /// <summary>Creates a description for a string instance method.</summary>
        /// <param name="name">Name of method to look up.</param>
        /// <param name="parameterTypes">Parameter types to match.</param>
        /// <returns>A new function description.</returns>
        private static FunctionDescription StringInstanceFunction(string name, params Type[] parameterTypes)
        {
            return CreateFunctionDescription(typeof(string), true /* instance */, true /* method */, name, parameterTypes);
        }

        /// <summary>Creates an instance of function description for the property on the given type.</summary>
        /// <param name="type">Type on which the property is declared on.</param>
        /// <param name="name">Name of property to look up.</param>
        /// <returns>an instance of function description.</returns>
        private static FunctionDescription CreatePropertyBasedFunction(Type type, string name)
        {
            return CreateFunctionDescription(type, true /* instance */, false /* method */, name, Type.EmptyTypes);
        }

        /// <summary>Creates an array of function description for math method with decimal and double overloads.</summary>
        /// <param name="name">Name of method to look up.</param>
        /// <returns>A new function description array.</returns>
        private static FunctionDescription[] MathFunctionArray(string name)
        {
            return new FunctionDescription[]
            {
                CreateFunctionDescription(typeof(Math), false /* instance */, true /* method */, name, typeof(double)),
                CreateFunctionDescription(typeof(Math), false /* instance */, true /* method */, name, typeof(decimal)),
            };
        }
    }
}
