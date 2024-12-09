using AbnfParser.CstNodes;
using Root;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AbnfParserGenerator.CstNodesGenerator
{
    public sealed class RuleListGenerator
    {
        private RuleListGenerator()
        {
        }

        public static RuleListGenerator Instance { get; } = new RuleListGenerator();

        public Classes Generate(RuleList node, Root.Void context)
        {
            return new Classes(
                node
                    .Inners
                    .Select(inner => InnerToDiscriminatedUnion.Instance.Visit(inner, default))
                    .OfType<Class>());
        }

        private sealed class InnerToDiscriminatedUnion : RuleList.Inner.Visitor<Class?, Root.Void>
        {
            private InnerToDiscriminatedUnion()
            {
            }

            public static InnerToDiscriminatedUnion Instance { get; } = new InnerToDiscriminatedUnion();

            protected internal override Class? Accept(RuleList.Inner.RuleInner node, Root.Void context)
            {
                return RuleToDiscriminatedUnion.Instance.Generate(node.Rule, context);
            }

            protected internal override Class? Accept(RuleList.Inner.CommentInner node, Root.Void context)
            {
                //// TODO preserve the comments as xmldoc?
                return default;
            }
        }
    }

    public sealed class Classes
    {
        public Classes(IEnumerable<Class> value)
        {
            this.Value = value;
        }

        public IEnumerable<Class> Value { get; }
    }

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
            MethodName = methodName;
            Parameters = parameters;
            this.Body = body;
        }

        public AccessModifier AccessModifier { get; }

        public bool? IsAbstract { get; }

        public bool IsOverride { get; }

        public string ReturnType { get; }

        public IEnumerable<string> GenericTypeParameters { get; }

        public string MethodName { get; }

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
