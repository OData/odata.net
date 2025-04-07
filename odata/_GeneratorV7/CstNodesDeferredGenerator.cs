using AbnfParserGenerator;

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
            return cstNodes;
        }
    }
}
