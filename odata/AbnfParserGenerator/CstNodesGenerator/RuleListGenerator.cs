using AbnfParser.CstNodes;
using Root;
using System.Text;

namespace AbnfParserGenerator.CstNodesGenerator
{
    public sealed class RuleListGenerator
    {
        public Void Generate(RuleList node, Classes builder)
        {
            foreach (var inner in node.Inners)
            {
                new InnerGenerator().Visit(inner, builder);
            }

            return default;
        }

        public sealed class InnerGenerator : RuleList.Inner.Visitor<Void, Classes>
        {
            protected internal override Void Accept(RuleList.Inner.RuleInner node, Classes context)
            {
                new RuleGenerator().Generate(node.Rule, context);
                return default;
            }

            protected internal override Void Accept(RuleList.Inner.CommentInner node, Classes context)
            {
                //// TODO preserve the comments as xmldoc?
                return default;
            }
        }
    }
}
