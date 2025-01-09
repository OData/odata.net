namespace _GeneratorV5
{
    using System.Collections.Generic;

    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParserGenerator;

    public sealed class Generator
    {
        private readonly _GeneratorV4.Generator generator;

        public Generator(string cstNodesRulesNamespace)
        {
            this.generator = new _GeneratorV4.Generator(cstNodesRulesNamespace);
        }

        public IEnumerable<Class> Generate(_rulelist rulelist)
        {
            return this.generator.Generate(rulelist);
        }
    }
}
