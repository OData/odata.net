﻿namespace AbnfParserGenerator
{
    using System;
    using System.Collections.Generic;

    public sealed class Class
    {
        public Class(
            AccessModifier accessModifier,
            bool? isAbstract,
            string name,
            IEnumerable<string> genericTypeParameters,
            string? baseType,
            IEnumerable<ConstructorDefinition> constructors,
            IEnumerable<MethodDefinition> methods,
            IEnumerable<Class> nestedClasses,
            IEnumerable<PropertyDefinition> properties)
        {
            AccessModifier = accessModifier;
            IsAbstract = isAbstract;
            Name = name;
            GenericTypeParameters = genericTypeParameters;
            this.BaseType = baseType;
            Constructors = constructors;
            this.Methods = methods;
            this.NestedClasses = nestedClasses;
            this.Properties = properties;
        }

        public AccessModifier AccessModifier { get; }

        public bool? IsAbstract { get; }

        public string Name { get; }

        public IEnumerable<string> GenericTypeParameters { get; }

        public string? BaseType { get; }

        public IEnumerable<ConstructorDefinition> Constructors { get; }

        public IEnumerable<MethodDefinition> Methods { get; }

        public IEnumerable<Class> NestedClasses { get; }

        public IEnumerable<PropertyDefinition> Properties { get; }
    }

    public sealed class PropertyDefinition
    {
        public PropertyDefinition(AccessModifier accessModifier, string type, string name, bool hasGet, bool hasSet)
        {
            AccessModifier = accessModifier;
            Type = type;
            Name = name;
            HasGet = hasGet;
            HasSet = hasSet;
        }

        public AccessModifier AccessModifier { get; }

        public string Type { get; }

        public string Name { get; }

        public bool HasGet { get; }

        public bool HasSet { get; }
    }

    public sealed class MethodDefinition
    {
        public MethodDefinition(AccessModifier accessModifier, bool? isAbstract, bool @override, string returnType, IEnumerable<string> genericTypeParameters, string methodName, IEnumerable<MethodParameter> parameters, string? body)
        {
            AccessModifier = accessModifier;
            IsAbstract = isAbstract;
            IsOverride = @override;
            ReturnType = returnType;
            GenericTypeParameters = genericTypeParameters;
            Name = methodName;
            Parameters = parameters;
            this.Body = body;
        }

        public AccessModifier AccessModifier { get; }

        public bool? IsAbstract { get; }

        public bool IsOverride { get; }

        public string ReturnType { get; }

        public IEnumerable<string> GenericTypeParameters { get; }

        public string Name { get; }

        public IEnumerable<MethodParameter> Parameters { get; }

        /// <summary>
        /// null is no body (like an abstract method)
        /// </summary>
        public string? Body { get; }
    }

    public sealed class MethodParameter
    {
        public MethodParameter(string type, string name)
        {
            Type = type;
            Name = name;
        }

        public string Type { get; }

        public string Name { get; }
    }

    public sealed class ConstructorDefinition
    {
        public ConstructorDefinition(AccessModifier accessModifier, IEnumerable<MethodParameter> parameters, string body)
        {
            this.AccessModifier = accessModifier;
            this.Parameters = parameters;
            this.Body = body;
        }

        public AccessModifier AccessModifier { get; }

        public IEnumerable<MethodParameter> Parameters { get; }

        public string Body { get; }
    }

    [Flags]
    public enum AccessModifier
    {
        Public = 0,
        Internal = 1,
        Protected = 2,
        Private = 3,
    }
}