namespace _GeneratorV5
{
    using System.Collections.Generic;
    using System.Linq;

    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParserGenerator;

    public sealed class Generator
    {
        private readonly string ruleCstNodesNamespace;

        private readonly _GeneratorV4.Generator generator;

        public Generator(string ruleCstNodesNamespace)
        {

            this.ruleCstNodesNamespace = ruleCstNodesNamespace;

            this.generator = new _GeneratorV4.Generator(ruleCstNodesNamespace);
        }

        public (Namespace RuleCstNodes, Namespace InnerCstNodes) Generate(_rulelist rulelist)
        {
            return 
                (
                    new Namespace(this.ruleCstNodesNamespace, this.generator.Generate(rulelist)), 
                    new Namespace("TODO", Enumerable.Empty<Class>())
                );
        }
    }
}
