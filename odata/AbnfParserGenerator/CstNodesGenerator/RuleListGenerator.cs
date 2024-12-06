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
        public Root.Void Generate(RuleList node, Classes builder)
        {
            foreach (var inner in node.Inners)
            {
                new InnerGenerator().Visit(inner, builder);
            }

            foreach (var @class in builder.Value)
            {
                foreach (var nestedClass in @class.NestedClasses)
                {
                    SetPropertyTypes(builder, nestedClass);
                }
            }

            return default;
        }

        private void SetPropertyTypes(Classes classes, Class @class)
        {
            //// TODO the `property.Type` property doesn't really need to be a `class` probably, so you could actually just set this at the time you set `property.name` if you change it to `string`
            foreach (var property in @class.Properties)
            {
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

        public sealed class InnerGenerator : RuleList.Inner.Visitor<Root.Void, Classes>
        {
            protected internal override Root.Void Accept(RuleList.Inner.RuleInner node, Classes context)
            {
                new RuleGenerator().Generate(node.Rule, context);
                return default;
            }

            protected internal override Root.Void Accept(RuleList.Inner.CommentInner node, Classes context)
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

        /// <summary>
        /// TODO null means singleton
        /// </summary>
        public ConstructorParameters? ConstructorParameters { get; set; }

        public List<Property> Properties { get; set; } = new List<Property>();
    }

    public sealed class Property
    {
        public Class? Type { get; set; }

        public StringBuilder Name { get; set; } = new StringBuilder();
    }

    public sealed class ConstructorParameters
    {
        public List<(Class type, string Name)> Value { get; set; } = new List<(Class type, string Name)>();
    }
}
