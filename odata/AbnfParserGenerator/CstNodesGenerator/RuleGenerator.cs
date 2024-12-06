using AbnfParser.CstNodes;
using AbnfParser.CstNodes.Core;
using Root;
using System.Collections.Generic;
using System.Text;

namespace AbnfParserGenerator.CstNodesGenerator
{
    public sealed class RuleGenerator
    {
        public Void Generate(Rule node, Classes builder)
        {
            //// TODO do a second attempt at implementing this once you've got this roughly fleshed out
            //// TODO singletons everywhere
            var ruleNameBuilder = new StringBuilder();
            new RuleNameToString().Convert(node.RuleName, ruleNameBuilder);
            var @class = new Class(ruleNameBuilder.ToString());
            builder.Value.Add(@class);
            return default;
        }

        private sealed class RuleNameToString
        {
            public void Convert(RuleName ruleName, StringBuilder builder)
            {
                var alphaToString = new AlphaToString();
                alphaToString.Visit(ruleName.Alpha, builder);
                foreach (var inner in ruleName.Inners)
                {
                    new RuleNameInnerToString().Visit(inner, builder);
                }
            }

            private sealed class RuleNameInnerToString : RuleName.Inner.Visitor<Void, StringBuilder>
            {
                protected internal override Void Accept(RuleName.Inner.AlphaInner node, StringBuilder context)
                {
                    new AlphaToString().Visit(node.Alpha, context);
                    return default;
                }

                protected internal override Void Accept(RuleName.Inner.DigitInner node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(RuleName.Inner.DashInner node, StringBuilder context)
                {
                    return default;
                }
            }

            private sealed class AlphaToString : Alpha.Visitor<Void, StringBuilder>
            {
                protected internal override Void Accept(Alpha.x41 node, StringBuilder context)
                {
                    //// TODO traverse all the way down
                    context.Append((char)0x41);
                    return default;
                }

                protected internal override Void Accept(Alpha.x42 node, StringBuilder context)
                {
                    //// TODO actually implement these
                    return default;
                }

                protected internal override Void Accept(Alpha.x43 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x44 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x45 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x46 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x47 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x48 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x49 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x4A node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x4B node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x4C node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x4D node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x4E node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x4F node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x50 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x51 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x52 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x53 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x54 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x55 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x56 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x57 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x58 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x59 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x5A node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x61 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x62 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x63 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x64 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x65 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x66 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x67 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x68 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x69 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x6A node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x6B node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x6C node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x6D node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x6E node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x6F node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x70 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x71 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x72 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x73 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x74 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x75 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x76 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x77 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x78 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x79 node, StringBuilder context)
                {
                    return default;
                }

                protected internal override Void Accept(Alpha.x7A node, StringBuilder context)
                {
                    return default;
                }
            }
        }
    }

    public sealed class Classes
    {
        public List<Class> Value { get; set; } = new List<Class>();
    }

    public sealed class Class
    {
        public Class(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        /// <summary>
        /// TODO true -> abstract, false -> sealed, null -> neither
        /// </summary>
        public bool? IsAbstract { get; set; }

        public Class? BaseClass { get; set; }

        public List<Class>? NestedClasses { get; set; } = new List<Class>();

        /// <summary>
        /// TODO null means singleton
        /// </summary>
        public ConstructorParameters? ConstructorParameters { get; set; }
    }

    public sealed class ConstructorParameters
    {
        public List<(Class type, string Name)>? Value { get; set; }
    }
}
