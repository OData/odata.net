using AbnfParser.CstNodes;
using AbnfParser.CstNodes.Core;
using Root;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace AbnfParserGenerator.CstNodesGenerator
{
    public sealed class RuleGenerator
    {
        public Class Generate(Rule node, Root.Void)
        {
            //// TODO do a second attempt at implementing this once you've got this roughly fleshed out
            //// TODO singletons everywhere

            var @class = new Class();
            new RuleNameToString().Convert(node.RuleName, @class.Name); //// TODO you need to ensure that dashes are stipping //// TODO you need to make sure the first character isn't a digit

            //// TODO skipping over node.DefinedAs means we are potentially skipping comments, if we care about that...

            new ElementsToDiscriminatedUnion().Convert(node.Elements, @class);

            //// TODO finish the `rule` implementation here

            builder.Value.Add(@class);
            return default;
        }

        private sealed class ElementsToDiscriminatedUnion
        {
            public void Convert(Elements elements, Class @class)
            {
                new AlternationToDiscriminatedUnion().Convert(elements.Alternation, @class);
            }

            private sealed class AlternationToDiscriminatedUnion
            {
                public void Convert(Alternation alternation, Class @class)
                {
                    @class.IsAbstract = true; //// TODO is this the right place to set this?

                    var name = 0;
                    var duElement = new Class();
                    duElement.Name.Append($"_{name}"); //// TODO this is a really bad name
                    duElement.IsAbstract = false;
                    ++name;
                    new ConcatenationToDuElement().Convert(alternation.Concatenation, duElement);
                    duElement.BaseClass = @class;
                    @class.NestedClasses.Add(duElement);
                    foreach (var inner in alternation.Inners)
                    {
                        //// TODO probably this loop body should go in `innertoduelement`?
                        duElement = new Class();
                        duElement.Name.Append($"_{name}");
                        duElement.IsAbstract = false;
                        ++name;
                        new InnerToDuElement().Convert(inner, duElement);
                        duElement.BaseClass = @class;
                        @class.NestedClasses.Add(duElement);
                    }

                    //// TODO add visitor
                }

                private sealed class InnerToDuElement
                {
                    public void Convert(Alternation.Inner inner, Class @class)
                    {
                        //// TODO you are skipping comments
                        new ConcatenationToDuElement().Convert(inner.Concatenation, @class);
                    }
                }

                private sealed class ConcatenationToDuElement
                {
                    public void Convert(Concatenation concatenation, Class @class)
                    {
                        new RepetitionToProperty().Visit(concatenation.Repetition, @class);
                        foreach (var inner in concatenation.Inners)
                        {
                            new InnerToProperty().Convert(inner, @class);
                        }
                    }

                    private sealed class InnerToProperty
                    {
                        public void Convert(Concatenation.Inner inner, Class @class)
                        {
                            //// TODO you are skipping comments
                            new RepetitionToProperty().Visit(inner.Repetition, @class);
                        }
                    }

                    private sealed class RepetitionToProperty : Repetition.Visitor<Void, Class>
                    {
                        protected internal override Void Accept(Repetition.ElementOnly node, Class context)
                        {
                            new ElementToProperty().Visit(node.Element, context);
                            return default;
                        }

                        protected internal override Void Accept(Repetition.RepeatAndElement node, Class context)
                        {
                            //// TODO you are skipping repetitions
                            new ElementToProperty().Visit(node.Element, context);
                            return default;
                        }

                        private sealed class ElementToProperty : Element.Visitor<Void, Class>
                        {
                            protected internal override Void Accept(Element.RuleName node, Class context)
                            {
                                //// TODO what about a concatenation that has two repetitions that have an element of the same type?
                                var property = new Property();
                                new RuleNameToString().Convert(node.Value, property.Name); //// TODO you need to strip dashes from this //// TODO you need to make sure the beginning is not a digit
                                context.Properties.Add(property);
                                return default;
                            }

                            protected internal override Void Accept(Element.Group node, Class context)
                            {
                                var className = "_Group0"; //// TODO you need to increment this name
                                var @class = new Class();
                                @class.Name.Append(className);
                                new GroupToDu().Convert(node.Value, @class);

                                context.NestedClasses.Add(@class);
                                
                                var property = new Property();
                                property.Name = @class.Name;
                                property.Type = @class;

                                context.Properties.Add(property);

                                return default;
                            }

                            private sealed class GroupToDu
                            {
                                public void Convert(Group group, Class @class)
                                {
                                    //// TODO you are skipping comments
                                    new AlternationToDiscriminatedUnion().Convert(group.Alternation, @class);
                                }
                            }

                            protected internal override Void Accept(Element.Option node, Class context)
                            {
                                var className = "_Option0"; //// TODO you need to increment this name
                                var @class = new Class();
                                @class.Name.Append(className);
                                new OptionToDu().Convert(node.Value, @class);

                                context.NestedClasses.Add(@class);

                                var property = new Property();
                                property.Name = @class.Name;
                                property.Type = @class;

                                context.Properties.Add(property);

                                return default;
                            }

                            private sealed class OptionToDu
                            {
                                public void Convert(Option option, Class @class)
                                {
                                    //// TODO you are skipping comments
                                    new AlternationToDiscriminatedUnion().Convert(option.Alternation, @class);
                                }
                            }

                            protected internal override Void Accept(Element.CharVal node, Class context)
                            {
                                //// TODO finish this
                                return default;
                            }

                            protected internal override Void Accept(Element.NumVal node, Class context)
                            {
                                //// TODO finish this
                                return default;
                            }

                            protected internal override Void Accept(Element.ProseVal node, Class context)
                            {
                                //// TODO finish this
                                return default;
                            }
                        }
                    }
                }
            }
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
                    new DigitToString().Visit(node.Digit, context);
                    return default;
                }

                protected internal override Void Accept(RuleName.Inner.DashInner node, StringBuilder context)
                {
                    //// TODO traverse all the way down the sub-nodes
                    context.Append((char)0x2D);
                    return default;
                }
            }

            private sealed class DigitToString : Digit.Visitor<Void, StringBuilder>
            {
                protected internal override Void Accept(Digit.x30 node, StringBuilder context)
                {
                    //// TODO traverse all the way down the sub-nodes
                    context.Append((char)0x30);
                    return default;
                }

                protected internal override Void Accept(Digit.x31 node, StringBuilder context)
                {
                    context.Append((char)0x31);
                    return default;
                }

                protected internal override Void Accept(Digit.x32 node, StringBuilder context)
                {
                    context.Append((char)0x32);
                    return default;
                }

                protected internal override Void Accept(Digit.x33 node, StringBuilder context)
                {
                    context.Append((char)0x33);
                    return default;
                }

                protected internal override Void Accept(Digit.x34 node, StringBuilder context)
                {
                    context.Append((char)0x34);
                    return default;
                }

                protected internal override Void Accept(Digit.x35 node, StringBuilder context)
                {
                    context.Append((char)0x35);
                    return default;
                }

                protected internal override Void Accept(Digit.x36 node, StringBuilder context)
                {
                    context.Append((char)0x36);
                    return default;
                }

                protected internal override Void Accept(Digit.x37 node, StringBuilder context)
                {
                    context.Append((char)0x37);
                    return default;
                }

                protected internal override Void Accept(Digit.x38 node, StringBuilder context)
                {
                    context.Append((char)0x38);
                    return default;
                }

                protected internal override Void Accept(Digit.x39 node, StringBuilder context)
                {
                    context.Append((char)0x39);
                    return default;
                }
            }

            private sealed class AlphaToString : Alpha.Visitor<Void, StringBuilder>
            {
                protected internal sealed override Void Accept(Alpha.x41 node, StringBuilder context)
                {
                    //// TODO traverse all the way down the sub-nodes
                    context.Append((char)0x41);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x42 node, StringBuilder context)
                {
                    context.Append((char)0x42);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x43 node, StringBuilder context)
                {
                    context.Append((char)0x43);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x44 node, StringBuilder context)
                {
                    context.Append((char)0x44);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x45 node, StringBuilder context)
                {
                    context.Append((char)0x45);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x46 node, StringBuilder context)
                {
                    context.Append((char)0x46);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x47 node, StringBuilder context)
                {
                    context.Append((char)0x47);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x48 node, StringBuilder context)
                {
                    context.Append((char)0x48);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x49 node, StringBuilder context)
                {
                    context.Append((char)0x49);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x4A node, StringBuilder context)
                {
                    context.Append((char)0x4A);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x4B node, StringBuilder context)
                {
                    context.Append((char)0x4B);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x4C node, StringBuilder context)
                {
                    context.Append((char)0x4C);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x4D node, StringBuilder context)
                {
                    context.Append((char)0x4D);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x4E node, StringBuilder context)
                {
                    context.Append((char)0x4E);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x4F node, StringBuilder context)
                {
                    context.Append((char)0x4F);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x50 node, StringBuilder context)
                {
                    context.Append((char)0x50);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x51 node, StringBuilder context)
                {
                    context.Append((char)0x51);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x52 node, StringBuilder context)
                {
                    context.Append((char)0x52);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x53 node, StringBuilder context)
                {
                    context.Append((char)0x53);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x54 node, StringBuilder context)
                {
                    context.Append((char)0x54);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x55 node, StringBuilder context)
                {
                    context.Append((char)0x55);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x56 node, StringBuilder context)
                {
                    context.Append((char)0x56);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x57 node, StringBuilder context)
                {
                    context.Append((char)0x57);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x58 node, StringBuilder context)
                {
                    context.Append((char)0x58);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x59 node, StringBuilder context)
                {
                    context.Append((char)0x59);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x5A node, StringBuilder context)
                {
                    context.Append((char)0x5A);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x61 node, StringBuilder context)
                {
                    context.Append((char)0x61);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x62 node, StringBuilder context)
                {
                    context.Append((char)0x62);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x63 node, StringBuilder context)
                {
                    context.Append((char)0x63);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x64 node, StringBuilder context)
                {
                    context.Append((char)0x64);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x65 node, StringBuilder context)
                {
                    context.Append((char)0x65);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x66 node, StringBuilder context)
                {
                    context.Append((char)0x66);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x67 node, StringBuilder context)
                {
                    context.Append((char)0x67);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x68 node, StringBuilder context)
                {
                    context.Append((char)0x68);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x69 node, StringBuilder context)
                {
                    context.Append((char)0x69);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x6A node, StringBuilder context)
                {
                    context.Append((char)0x6A);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x6B node, StringBuilder context)
                {
                    context.Append((char)0x6B);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x6C node, StringBuilder context)
                {
                    context.Append((char)0x6C);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x6D node, StringBuilder context)
                {
                    context.Append((char)0x6D);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x6E node, StringBuilder context)
                {
                    context.Append((char)0x6E);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x6F node, StringBuilder context)
                {
                    context.Append((char)0x6F);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x70 node, StringBuilder context)
                {
                    context.Append((char)0x70);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x71 node, StringBuilder context)
                {
                    context.Append((char)0x71);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x72 node, StringBuilder context)
                {
                    context.Append((char)0x72);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x73 node, StringBuilder context)
                {
                    context.Append((char)0x73);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x74 node, StringBuilder context)
                {
                    context.Append((char)0x74);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x75 node, StringBuilder context)
                {
                    context.Append((char)0x75);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x76 node, StringBuilder context)
                {
                    context.Append((char)0x76);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x77 node, StringBuilder context)
                {
                    context.Append((char)0x77);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x78 node, StringBuilder context)
                {
                    context.Append((char)0x78);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x79 node, StringBuilder context)
                {
                    context.Append((char)0x79);
                    return default;
                }

                protected internal sealed override Void Accept(Alpha.x7A node, StringBuilder context)
                {
                    context.Append((char)0x7A);
                    return default;
                }
            }
        }
    }
}
