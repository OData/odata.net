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
        public Classes Generate(RuleList node, Root.Void context)
        {
            var classes = new Classes();
            foreach (var inner in node.Inners)
            {
                var @class = new InnerGenerator().Visit(inner, default);
                if (@class != null)
                {
                    classes.Value.Add(@class);
                }
            }

            foreach (var @class in classes.Value)
            {
                TraverseClasses(classes, @class);
            }

            return default;
        }

        private void TraverseClasses(Classes classes, Class @class)
        {
            SetPropertyTypes(classes, @class);

            foreach (var nestedClass in @class.NestedClasses)
            {
                TraverseClasses(classes, nestedClass);
            }
        }

        private void SetPropertyTypes(Classes classes, Class @class)
        {
            //// TODO the `property.Type` property doesn't really need to be a `class` probably, so you could actually just set this at the time you set `property.name` if you change it to `string`
            foreach (var property in @class.Properties)
            {
                if (property.Type != null)
                {
                    continue;
                }

                try
                {
                    property.Type = classes.Value.Single(_ => string.Equals(property.Name.ToString(), _.Name.ToString(), System.StringComparison.OrdinalIgnoreCase)); //// TODO the `tostring` calls here are really bad
                }
                catch (InvalidOperationException e)
                {
                    throw new Exception("TODO can't find property type", e);
                }
            }
        }

        public sealed class InnerGenerator : RuleList.Inner.Visitor<Class, Root.Void>
        {
            protected internal override Class Accept(RuleList.Inner.RuleInner node, Root.Void context)
            {
                return new RuleGeneratorToDiscriminatedUnion().Generate(node.Rule, context);
            }

            protected internal override Class Accept(RuleList.Inner.CommentInner node, Root.Void context)
            {
                //// TODO preserve the comments as xmldoc?
                return default;
            }
        }
    }

    public sealed class Classes
    {
        public List<Class> Value { get; set; } = new List<Class>();
    }

    public sealed class Class
    {
        public StringBuilder Name { get; set; } = new StringBuilder();

        /// <summary>
        /// TODO true -> abstract, false -> sealed, null -> neither
        /// </summary>
        public bool? IsAbstract { get; set; }

        public Class? BaseClass { get; set; }

        public List<Class> NestedClasses { get; set; } = new List<Class>();

        public List<ConstructorDefinition> ConstructorDefinitions { get; set; } = new List<ConstructorDefinition>();

        public List<Property> Properties { get; set; } = new List<Property>();
    }

    public sealed class Property
    {
        public Class? Type { get; set; }

        public StringBuilder Name { get; set; } = new StringBuilder();
    }

    public sealed class MethodDefinition
    {
        public MethodDefinition(AccessModifier accessModifier, bool? isAbstract, string returnType, IEnumerable<string> genericTypeParameters, IEnumerable<MethodParameter> parameters)
        {
            AccessModifier = accessModifier;
            IsAbstract = isAbstract;
            ReturnType = returnType;
            GenericTypeParameters = genericTypeParameters;
            Parameters = parameters;
        }

        public AccessModifier AccessModifier { get; }

        public bool? IsAbstract { get; }

        public string ReturnType { get; }

        public IEnumerable<string> GenericTypeParameters { get; }

        public IEnumerable<MethodParameter> Parameters { get; }
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
        public ConstructorDefinition(AccessModifier accessModifier, ConstructorParameters constructorParameters)
        {
            this.AccessModifier = accessModifier;
            this.ConstructorParameters = constructorParameters;
        }

        public AccessModifier AccessModifier { get; }

        public ConstructorParameters ConstructorParameters { get; }
    }

    public sealed class ConstructorParameters
    {
        public List<(Class Type, string Name)> Value { get; set; } = new List<(Class Type, string Name)>();
    }

    public enum AccessModifier
    {
        Public = 0,
        Internal = 1,
        Protected = 2,
        Private = 3,
    }
}
