namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _functionParameter_ЖⲤCOMMA_functionParameterↃParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃ> Instance { get; } = from _functionParameter_1 in __GeneratedOdata.Parsers.Rules._functionParameterParser.Instance
from _ⲤCOMMA_functionParameterↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤCOMMA_functionParameterↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃ(_functionParameter_1, _ⲤCOMMA_functionParameterↃ_1);
    }
    
}