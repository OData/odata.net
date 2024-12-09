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

        public List<MethodDefinition> Methods { get; ; set; } = new List<MethodDefinition>();
    }

    public sealed class Class2
    {
        public Class2(
            AccessModifier accessModifier,
            bool? isAbstract,
            string name,
            IEnumerable<string> genericTypeParameters,
            IEnumerable<ConstructorDefinition> constructors,
            IEnumerable<MethodDefinition> methods,
            IEnumerable<Class2> nestedClasses)
        {
            AccessModifier = accessModifier;
            IsAbstract = isAbstract;
            Name = name;
            GenericTypeParameters = genericTypeParameters;
            Constructors = constructors;
            this.Methods = methods;
            this.NestedClasses = nestedClasses;
        }

        public AccessModifier AccessModifier { get; }

        public bool? IsAbstract { get; }

        public string Name { get; }

        public IEnumerable<string> GenericTypeParameters { get; }

        public IEnumerable<ConstructorDefinition> Constructors { get; }

        public IEnumerable<MethodDefinition> Methods { get; }

        public IEnumerable<Class2> NestedClasses { get; }
    }

    public sealed class Property
    {
        public Class? Type { get; set; }

        public StringBuilder Name { get; set; } = new StringBuilder();
    }

    public sealed class MethodDefinition
    {
        public MethodDefinition(AccessModifier accessModifier, bool? isAbstract, string returnType, IEnumerable<string> genericTypeParameters, string methodName, IEnumerable<MethodParameter> parameters)
        {
            AccessModifier = accessModifier;
            IsAbstract = isAbstract;
            ReturnType = returnType;
            GenericTypeParameters = genericTypeParameters;
            MethodName = methodName;
            Parameters = parameters;
        }

        public AccessModifier AccessModifier { get; }

        public bool? IsAbstract { get; }

        public string ReturnType { get; }

        public IEnumerable<string> GenericTypeParameters { get; }

        public string MethodName { get; }

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
        public ConstructorDefinition(AccessModifier accessModifier, IEnumerable<MethodParameter> parameters)
        {
            this.AccessModifier = accessModifier;
            this.Parameters = parameters;
        }

        public AccessModifier AccessModifier { get; }

        public IEnumerable<MethodParameter> Parameters { get; }
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
