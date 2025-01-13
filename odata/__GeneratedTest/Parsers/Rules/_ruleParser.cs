namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _ruleParser
    {
        public static Parser<__Generated.CstNodes.Rules._rule> Instance { get; } = from _rulename_1 in __GeneratedTest.Parsers.Rules._rulenameParser.Instance
from _definedⲻas_1 in __GeneratedTest.Parsers.Rules._definedⲻasParser.Instance
from _elements_1 in __GeneratedTest.Parsers.Rules._elementsParser.Instance
from _cⲻnl_1 in __GeneratedTest.Parsers.Rules._cⲻnlParser.Instance
select new __Generated.CstNodes.Rules._rule(_rulename_1, _definedⲻas_1, _elements_1, _cⲻnl_1);
    }
    
}
