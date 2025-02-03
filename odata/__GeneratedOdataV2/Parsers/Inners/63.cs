namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_functionParameterParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_functionParameter> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _functionParameter_1 in __GeneratedOdataV2.Parsers.Rules._functionParameterParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_functionParameter(_COMMA_1, _functionParameter_1);
    }
    
}
