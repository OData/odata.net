namespace __Generated.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _repetitionParser
    {
        public static IParser<char, __Generated.CstNodes.Rules._repetition> Instance { get; } = from _repeat_1 in __Generated.Parsers.Rules._repeatParser.Instance.Optional()
from _element_1 in __Generated.Parsers.Rules._elementParser.Instance
select new __Generated.CstNodes.Rules._repetition(_repeat_1.GetOrElse(null), _element_1);
    }
    
}
