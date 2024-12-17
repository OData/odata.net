using AbnfParser.CstNodes;
using AbnfParser.CstNodes.Core;
using Root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;

namespace AbnfParserGenerator.CstNodesGenerator
{
    public sealed class RuleToDiscriminatedUnion
    {
        private RuleToDiscriminatedUnion()
        {
        }

        public static RuleToDiscriminatedUnion Instance { get; } = new RuleToDiscriminatedUnion();

        private static void NormalizeClassName(StringBuilder className)
        {
            className.Replace("-", "_").Insert(0, "_");
        }

        public Class Generate(Rule node, Root.Void context)
        {
            //// TODO do a second attempt at implementing this once you've got this roughly fleshed out

            //// TODO you are skipping comments everywhere if you care to preserve them

            var ruleNameBuilder = new StringBuilder();
            RuleNameToString.Instance.Convert(node.RuleName, ruleNameBuilder);
            NormalizeClassName(ruleNameBuilder);
            var ruleName = ruleNameBuilder.ToString();

            var discriminatedUnionElements = ElementsToDiscriminatedUnionElements.Instance.Convert(node.Elements, ruleName);
            return new Class(
                AccessModifier.Public,
                true,
                ruleName,
                Enumerable.Empty<string>(),
                null,
                new[]
                {
                    new ConstructorDefinition(AccessModifier.Private, Enumerable.Empty<MethodParameter>(), string.Empty),
                },
                new[]
                {
                    new MethodDefinition(
                        AccessModifier.Protected,
                        true,
                        false,
                        "TResult",
                        new[]
                        {
                            "TResult",
                            "TContext",
                        },
                        "Dispatch",
                        new[]
                        {
                            new MethodParameter("Visitor<TResult, TContext>", "visitor"),
                            new MethodParameter("TContext", "context"),
                        },
                        null),
                },
                discriminatedUnionElements
                    .Prepend(
                        new Class(
                            AccessModifier.Public,
                            true,
                            "Visitor",
                            new[]
                            {
                                "TResult",
                                "TContext",
                            },
                            null,
                            Enumerable.Empty<ConstructorDefinition>(),
                            discriminatedUnionElements
                                .Select(discriminatedUnionElement =>
                                    new MethodDefinition(
                                        AccessModifier.Protected | AccessModifier.Internal, 
                                        true,
                                        false,
                                        "TResult", 
                                        Enumerable.Empty<string>(),
                                        "Accept",
                                        new[] 
                                        {
                                            new MethodParameter(discriminatedUnionElement.Name, "node"),
                                            new MethodParameter("TContext", "context"),
                                        },
                                        null))
                                .Prepend(new MethodDefinition(
                                    AccessModifier.Public,
                                    null,
                                    false,
                                    "TResult",
                                    Enumerable.Empty<string>(),
                                    "Visit",
                                    new[]
                                    {
                                        new MethodParameter(ruleName, "node"),
                                        new MethodParameter("TContext", "context"),
                                    },
                                    "return node.Dispatch(this, context);")),
                            Enumerable.Empty<Class>(),
                            Enumerable.Empty<PropertyDefinition>())),
                Enumerable.Empty<PropertyDefinition>());
        }

        private sealed class ElementsToDiscriminatedUnionElements
        {
            private ElementsToDiscriminatedUnionElements()
            {
            }

            public static ElementsToDiscriminatedUnionElements Instance { get; } = new ElementsToDiscriminatedUnionElements();

            public IEnumerable<Class> Convert(Elements elements, string baseType)
            {
                return AlternationToDiscriminatedUnionElements.Instance.Convert(elements.Alternation, baseType);
            }

            private sealed class AlternationToDiscriminatedUnionElements
            {
                private AlternationToDiscriminatedUnionElements()
                {
                }

                public static AlternationToDiscriminatedUnionElements Instance { get; } = new AlternationToDiscriminatedUnionElements();

                public IEnumerable<Class> Convert(Alternation alternation, string baseType)
                {
                    yield return ConcatenationToDuElement.Instance.Convert(alternation.Concatenation, baseType);
                    foreach (var inner in alternation.Inners)
                    {
                        yield return InnerToDuElement.Instance.Convert(inner, baseType);
                    }
                }

                private sealed class ConcatenationToDuElement
                {
                    private ConcatenationToDuElement()
                    {
                    }

                    public static ConcatenationToDuElement Instance { get; } = new ConcatenationToDuElement();

                    public Class Convert(Concatenation concatenation, string baseType)
                    {
                        var duElementNameBuilder = new StringBuilder();
                        ConcatenationToDuElementName.Instance.Convert(concatenation, duElementNameBuilder);
                        NormalizeClassName(duElementNameBuilder);
                        var duElementName = duElementNameBuilder.ToString();

                        var duElementProperties = ConcatenationToDuElementProperties.Instance.Convert(concatenation, default);
                        return new Class(
                            AccessModifier.Public,
                            false,
                            duElementName,
                            Enumerable.Empty<string>(),
                            baseType,
                            new[]
                            {
                                new ConstructorDefinition(
                                    AccessModifier.Public,
                                    duElementProperties.Select(duElementProperty =>
                                        new MethodParameter(
                                            duElementProperty.Name,
                                            duElementProperty.Name[0].ToString().ToLower() + duElementProperty.Name.Substring(1))),
                                    string.Join(
                                        Environment.NewLine,
                                        duElementProperties.Select(duElementProperty =>
                                            $"this.{duElementProperty.Name} = {duElementProperty.Name[0].ToString().ToLower() + duElementProperty.Name.Substring(1)};"))),
                            },
                            new[]
                            {
                                new MethodDefinition(
                                    AccessModifier.Protected,
                                    false,
                                    true,
                                    "TResult",
                                    new[]
                                    {
                                        "TResult",
                                        "TContext",
                                    },
                                    "Dispatch",
                                    new[]
                                    {
                                        new MethodParameter("Visitor<TResult, TContext>", "visitor"),
                                        new MethodParameter("TContext", "context"),
                                    },
                                    "return visitor.Accept(this, context);"),
                            },
                            Enumerable.Empty<Class>(), //// TODO implement this //// TODO you wrote this because you thought this is where the "option" and "group" classes would happen
                            duElementProperties);
                    }

                    private sealed class ConcatenationToDuElementName
                    {
                        private ConcatenationToDuElementName()
                        {
                        }

                        public static ConcatenationToDuElementName Instance { get; } = new ConcatenationToDuElementName();

                        public void Convert(Concatenation concatenation, StringBuilder duElementName)
                        {
                            RepetitionToDuElementName.Instance.Visit(concatenation.Repetition, duElementName);
                            if (!concatenation.Inners.Any())
                            {
                                //// TODO this isn't working correctly but you'll definintely need it for some cases:
                                //// duElementName.Append("Only");
                            }
                            else
                            {
                                foreach (var inner in concatenation.Inners)
                                {
                                    duElementName.Append("CombinedWith");
                                    InnerToDuElementName.Instance.Convert(inner, duElementName);
                                }
                            }
                        }

                        private sealed class InnerToDuElementName
                        {
                            private InnerToDuElementName()
                            {
                            }

                            public static InnerToDuElementName Instance { get; } = new InnerToDuElementName();

                            public void Convert(Concatenation.Inner inner, StringBuilder duElementName)
                            {
                                RepetitionToDuElementName.Instance.Visit(inner.Repetition, duElementName);
                            }
                        }

                        private sealed class RepetitionToDuElementName : Repetition.Visitor<Root.Void, StringBuilder>
                        {
                            private RepetitionToDuElementName()
                            {
                            }

                            public static RepetitionToDuElementName Instance { get; } = new RepetitionToDuElementName();

                            protected internal override Root.Void Accept(Repetition.ElementOnly node, StringBuilder context)
                            {
                                ElementToDuElementName.Instance.Visit(node.Element, context);
                                return default;
                            }

                            protected internal override Root.Void Accept(Repetition.RepeatAndElement node, StringBuilder context)
                            {
                                RepeatToDuElementName.Instance.Visit(node.Repeat, context);
                                ElementToDuElementName.Instance.Visit(node.Element, context);
                                return default;
                            }

                            private sealed class ElementToDuElementName : Element.Visitor<Root.Void, StringBuilder>
                            {
                                private ElementToDuElementName()
                                {
                                }

                                public static ElementToDuElementName Instance { get; } = new ElementToDuElementName();

                                protected internal override Root.Void Accept(Element.RuleName node, StringBuilder context)
                                {
                                    RuleNameToString.Instance.Convert(node.Value, context);
                                    return default;
                                }

                                protected internal override Root.Void Accept(Element.Group node, StringBuilder context)
                                {
                                    //// TODO implement this
                                    return default;
                                }

                                protected internal override Root.Void Accept(Element.Option node, StringBuilder context)
                                {
                                    //// TODO implement this
                                    return default;
                                }

                                protected internal override Root.Void Accept(Element.CharVal node, StringBuilder context)
                                {
                                    //// TODO implement this
                                    return default;
                                }

                                protected internal override Root.Void Accept(Element.NumVal node, StringBuilder context)
                                {
                                    //// TODO implement this
                                    return default;
                                }

                                protected internal override Root.Void Accept(Element.ProseVal node, StringBuilder context)
                                {
                                    //// TODO implement this
                                    return default;
                                }
                            }

                            private sealed class RepeatToDuElementName : Repeat.Visitor<Root.Void, StringBuilder>
                            {
                                private RepeatToDuElementName()
                                {
                                }

                                public static RepeatToDuElementName Instance { get; } = new RepeatToDuElementName();

                                protected internal override Root.Void Accept(Repeat.Count node, StringBuilder context)
                                {
                                    var count = DigitsToInt.Instance.Convert(node.Digits, default);
                                    var numberWord = IntToNumberWord(count);
                                    context.Append(numberWord);
                                    //// TODO it would be really nice to put an "s" after the next word taht the caller appends if this is != 1
                                    return default;
                                }

                                private static string IntToNumberWord(int value)
                                {
                                    //// TODO use a standard implementation for this
                                    if (value == 0)
                                    {
                                        return "Zero";
                                    }
                                    else if (value == 1)
                                    {
                                        return "One";
                                    }
                                    else if (value == 2)
                                    {
                                        return "Two";
                                    }
                                    else if (value == 3)
                                    {
                                        return "Three";
                                    }
                                    else if (value == 4)
                                    {
                                        return "Four";
                                    }
                                    else if (value == 5)
                                    {
                                        return "Five";
                                    }
                                    else if (value == 6)
                                    {
                                        return "Six";
                                    }
                                    else if (value == 7)
                                    {
                                        return "Seven";
                                    }
                                    else if (value == 8)
                                    {
                                        return "Eight";
                                    }
                                    else if (value == 9)
                                    {
                                        return "Nine";
                                    }
                                    else
                                    {
                                        throw new Exception("TODO use a standard implementation");
                                    }
                                }

                                protected internal override Root.Void Accept(Repeat.Range node, StringBuilder context)
                                {
                                    if (!node.PrefixDigits.Any())
                                    {
                                        if (!node.SuffixDigits.Any())
                                        {
                                            context.Append("AnyNumberOf");
                                            return default;
                                        }
                                        else
                                        {
                                            context.Append("ZeroTo");
                                            var count = DigitsToInt.Instance.Convert(node.SuffixDigits, default);
                                            var numberWord = IntToNumberWord(count);
                                            context.Append(numberWord);
                                            return default;
                                        }
                                    }
                                    else
                                    {
                                        if (!node.SuffixDigits.Any())
                                        {
                                            context.Append("AtLeast");
                                            var count = DigitsToInt.Instance.Convert(node.PrefixDigits, default);
                                            var numberWord = IntToNumberWord(count);
                                            context.Append(numberWord);
                                            return default;
                                        }
                                        else
                                        {
                                            var prefixCount = DigitsToInt.Instance.Convert(node.PrefixDigits, default);
                                            var prefixNumberWord = IntToNumberWord(prefixCount);
                                            context.Append(prefixNumberWord);
                                            context.Append("To");
                                            
                                            var suffixCount = DigitsToInt.Instance.Convert(node.SuffixDigits, default);
                                            var suffixNumberWord = IntToNumberWord(suffixCount);
                                            context.Append(suffixNumberWord);
                                            return default;
                                        }
                                    }
                                }

                                private sealed class DigitsToInt
                                {
                                    private DigitsToInt()
                                    {
                                    }

                                    public static DigitsToInt Instance { get; } = new DigitsToInt();

                                    public int Convert(IEnumerable<Digit> digits, Root.Void context)
                                    {
                                        var value = 0;
                                        foreach (var digit in digits)
                                        {
                                            value *= 10;
                                            value += DigitToInt.Instance.Visit(digit, default);
                                        }

                                        return value;
                                    }

                                    private sealed class DigitToInt : Digit.Visitor<int, Root.Void>
                                    {
                                        private DigitToInt()
                                        {
                                        }

                                        public static DigitToInt Instance { get; } = new DigitToInt();

                                        protected internal override int Accept(Digit.x30 node, Root.Void context)
                                        {
                                            return 0x30 - '0';
                                        }

                                        protected internal override int Accept(Digit.x31 node, Root.Void context)
                                        {
                                            return 0x31 - '0';
                                        }

                                        protected internal override int Accept(Digit.x32 node, Root.Void context)
                                        {
                                            return 0x32 - '0';
                                        }

                                        protected internal override int Accept(Digit.x33 node, Root.Void context)
                                        {
                                            return 0x33 - '0';
                                        }

                                        protected internal override int Accept(Digit.x34 node, Root.Void context)
                                        {
                                            return 0x34 - '0';
                                        }

                                        protected internal override int Accept(Digit.x35 node, Root.Void context)
                                        {
                                            return 0x35 - '0';
                                        }

                                        protected internal override int Accept(Digit.x36 node, Root.Void context)
                                        {
                                            return 0x36 - '0';
                                        }

                                        protected internal override int Accept(Digit.x37 node, Root.Void context)
                                        {
                                            return 0x37 - '0';
                                        }

                                        protected internal override int Accept(Digit.x38 node, Root.Void context)
                                        {
                                            return 0x38 - '0';
                                        }

                                        protected internal override int Accept(Digit.x39 node, Root.Void context)
                                        {
                                            return 0x39 - '0';
                                        }
                                    }
                                }
                            }
                        }
                    }

                    private sealed class ConcatenationToDuElementProperties
                    {
                        private ConcatenationToDuElementProperties()
                        {
                        }

                        public static ConcatenationToDuElementProperties Instance { get; } = new ConcatenationToDuElementProperties();

                        public IEnumerable<PropertyDefinition> Convert(Concatenation concatenation, Root.Void context)
                        {
                            //// TODO implement this
                            return Enumerable.Empty<PropertyDefinition>();
                        }
                    }
                }

                private sealed class InnerToDuElement
                {
                    private InnerToDuElement()
                    {
                    }

                    public static InnerToDuElement Instance { get; } = new InnerToDuElement();

                    public Class Convert(Alternation.Inner inner, string baseType)
                    {
                        return ConcatenationToDuElement.Instance.Convert(inner.Concatenation, baseType);
                    }
                }

            }
        }

        private sealed class RuleNameToString
        {
            private RuleNameToString()
            {
            }

            public static RuleNameToString Instance { get; } = new RuleNameToString();

            public void Convert(RuleName ruleName, StringBuilder builder)
            {
                var alphaToString = new AlphaToString();
                alphaToString.Visit(ruleName.Alpha, builder);
                foreach (var inner in ruleName.Inners)
                {
                    new RuleNameInnerToString().Visit(inner, builder);
                }
            }

            private sealed class RuleNameInnerToString : RuleName.Inner.Visitor<Root.Void, StringBuilder>
            {
                protected internal override Root.Void Accept(RuleName.Inner.AlphaInner node, StringBuilder context)
                {
                    new AlphaToString().Visit(node.Alpha, context);
                    return default;
                }

                protected internal override Root.Void Accept(RuleName.Inner.DigitInner node, StringBuilder context)
                {
                    new DigitToString().Visit(node.Digit, context);
                    return default;
                }

                protected internal override Root.Void Accept(RuleName.Inner.DashInner node, StringBuilder context)
                {
                    //// TODO traverse all the way down the sub-nodes
                    context.Append((char)0x2D);
                    return default;
                }
            }

            private sealed class DigitToString : Digit.Visitor<Root.Void, StringBuilder>
            {
                protected internal override Root.Void Accept(Digit.x30 node, StringBuilder context)
                {
                    //// TODO traverse all the way down the sub-nodes
                    context.Append((char)0x30);
                    return default;
                }

                protected internal override Root.Void Accept(Digit.x31 node, StringBuilder context)
                {
                    context.Append((char)0x31);
                    return default;
                }

                protected internal override Root.Void Accept(Digit.x32 node, StringBuilder context)
                {
                    context.Append((char)0x32);
                    return default;
                }

                protected internal override Root.Void Accept(Digit.x33 node, StringBuilder context)
                {
                    context.Append((char)0x33);
                    return default;
                }

                protected internal override Root.Void Accept(Digit.x34 node, StringBuilder context)
                {
                    context.Append((char)0x34);
                    return default;
                }

                protected internal override Root.Void Accept(Digit.x35 node, StringBuilder context)
                {
                    context.Append((char)0x35);
                    return default;
                }

                protected internal override Root.Void Accept(Digit.x36 node, StringBuilder context)
                {
                    context.Append((char)0x36);
                    return default;
                }

                protected internal override Root.Void Accept(Digit.x37 node, StringBuilder context)
                {
                    context.Append((char)0x37);
                    return default;
                }

                protected internal override Root.Void Accept(Digit.x38 node, StringBuilder context)
                {
                    context.Append((char)0x38);
                    return default;
                }

                protected internal override Root.Void Accept(Digit.x39 node, StringBuilder context)
                {
                    context.Append((char)0x39);
                    return default;
                }
            }

            private sealed class AlphaToString : Alpha.Visitor<Root.Void, StringBuilder>
            {
                protected internal sealed override Root.Void Accept(Alpha.x41 node, StringBuilder context)
                {
                    //// TODO traverse all the way down the sub-nodes
                    context.Append((char)0x41);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x42 node, StringBuilder context)
                {
                    context.Append((char)0x42);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x43 node, StringBuilder context)
                {
                    context.Append((char)0x43);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x44 node, StringBuilder context)
                {
                    context.Append((char)0x44);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x45 node, StringBuilder context)
                {
                    context.Append((char)0x45);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x46 node, StringBuilder context)
                {
                    context.Append((char)0x46);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x47 node, StringBuilder context)
                {
                    context.Append((char)0x47);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x48 node, StringBuilder context)
                {
                    context.Append((char)0x48);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x49 node, StringBuilder context)
                {
                    context.Append((char)0x49);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x4A node, StringBuilder context)
                {
                    context.Append((char)0x4A);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x4B node, StringBuilder context)
                {
                    context.Append((char)0x4B);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x4C node, StringBuilder context)
                {
                    context.Append((char)0x4C);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x4D node, StringBuilder context)
                {
                    context.Append((char)0x4D);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x4E node, StringBuilder context)
                {
                    context.Append((char)0x4E);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x4F node, StringBuilder context)
                {
                    context.Append((char)0x4F);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x50 node, StringBuilder context)
                {
                    context.Append((char)0x50);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x51 node, StringBuilder context)
                {
                    context.Append((char)0x51);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x52 node, StringBuilder context)
                {
                    context.Append((char)0x52);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x53 node, StringBuilder context)
                {
                    context.Append((char)0x53);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x54 node, StringBuilder context)
                {
                    context.Append((char)0x54);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x55 node, StringBuilder context)
                {
                    context.Append((char)0x55);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x56 node, StringBuilder context)
                {
                    context.Append((char)0x56);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x57 node, StringBuilder context)
                {
                    context.Append((char)0x57);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x58 node, StringBuilder context)
                {
                    context.Append((char)0x58);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x59 node, StringBuilder context)
                {
                    context.Append((char)0x59);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x5A node, StringBuilder context)
                {
                    context.Append((char)0x5A);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x61 node, StringBuilder context)
                {
                    context.Append((char)0x61);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x62 node, StringBuilder context)
                {
                    context.Append((char)0x62);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x63 node, StringBuilder context)
                {
                    context.Append((char)0x63);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x64 node, StringBuilder context)
                {
                    context.Append((char)0x64);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x65 node, StringBuilder context)
                {
                    context.Append((char)0x65);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x66 node, StringBuilder context)
                {
                    context.Append((char)0x66);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x67 node, StringBuilder context)
                {
                    context.Append((char)0x67);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x68 node, StringBuilder context)
                {
                    context.Append((char)0x68);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x69 node, StringBuilder context)
                {
                    context.Append((char)0x69);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x6A node, StringBuilder context)
                {
                    context.Append((char)0x6A);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x6B node, StringBuilder context)
                {
                    context.Append((char)0x6B);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x6C node, StringBuilder context)
                {
                    context.Append((char)0x6C);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x6D node, StringBuilder context)
                {
                    context.Append((char)0x6D);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x6E node, StringBuilder context)
                {
                    context.Append((char)0x6E);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x6F node, StringBuilder context)
                {
                    context.Append((char)0x6F);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x70 node, StringBuilder context)
                {
                    context.Append((char)0x70);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x71 node, StringBuilder context)
                {
                    context.Append((char)0x71);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x72 node, StringBuilder context)
                {
                    context.Append((char)0x72);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x73 node, StringBuilder context)
                {
                    context.Append((char)0x73);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x74 node, StringBuilder context)
                {
                    context.Append((char)0x74);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x75 node, StringBuilder context)
                {
                    context.Append((char)0x75);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x76 node, StringBuilder context)
                {
                    context.Append((char)0x76);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x77 node, StringBuilder context)
                {
                    context.Append((char)0x77);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x78 node, StringBuilder context)
                {
                    context.Append((char)0x78);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x79 node, StringBuilder context)
                {
                    context.Append((char)0x79);
                    return default;
                }

                protected internal sealed override Root.Void Accept(Alpha.x7A node, StringBuilder context)
                {
                    context.Append((char)0x7A);
                    return default;
                }
            }
        }
    }
}
