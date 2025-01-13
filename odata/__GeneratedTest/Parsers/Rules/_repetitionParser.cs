namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _repetitionParser
    {
        public static Parser<__Generated.CstNodes.Rules._repetition> Instance { get; } = from _repeat_1 in __GeneratedTest.Parsers.Rules._repeatParser.Instance.Optional()
from _element_1 in __GeneratedTest.Parsers.Rules._elementParser.Instance
select new __Generated.CstNodes.Rules._repetition(_repeat_1.GetOrElse(null), _element_1);
    }
    
}
