namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_functionParameterParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._COMMA_functionParameter> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _functionParameter_1 in __GeneratedOdata.Parsers.Rules._functionParameterParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_functionParameter(_COMMA_1, _functionParameter_1);
    }
    
}
