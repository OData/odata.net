using AbnfParserGenerator;
using System.Linq;

namespace _GeneratorV7
{
    public sealed class CstNodesDeferredGenerator
    {
        public CstNodesDeferredGenerator()
        {
        }

        public (Namespace RuleCstNodes, Namespace InnerCstNodes) CreateDeferred(
            (Namespace RuleCstNodes, Namespace InnerCstNodes) cstNodes)
        {
            var ruleCstNodes =
                new Namespace(
                    cstNodes.RuleCstNodes.Name,
                    cstNodes.RuleCstNodes.Classes.ToList(),
                    cstNodes.RuleCstNodes.UsingDeclarations);
            var innerCstNodes =
                new Namespace(
                    cstNodes.InnerCstNodes.Name,
                    cstNodes.InnerCstNodes.Classes.ToList(),
                    cstNodes.InnerCstNodes.UsingDeclarations);

            Translate(ruleCstNodes);
            Translate(innerCstNodes);

            return
                (
                    ruleCstNodes,
                    innerCstNodes
                );
        }

        private void Translate(Namespace toTranslate)
        {
            for (int i = 0; i < toTranslate.Classes.Count; ++i)
            {
                var cstNode = toTranslate.Classes[i];
                var translated = Translate(cstNode);
                if (translated == null)
                {
                    toTranslate.Classes.RemoveAt(i);
                    --i;
                    continue;
                    //// throw new System.Exception("TODO");
                }

                if (translated != null)
                {
                    toTranslate.Classes[i] = translated;
                }
            }
        }

        private Class? Translate(Class @class)
        {
            if (@class.Properties.Where(property => property.Name == "Instance" && property.IsStatic).Any())
            {

            }

            return null;
        }
    }
}
