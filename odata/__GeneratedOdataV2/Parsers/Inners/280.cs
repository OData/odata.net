namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_functionExprParameterParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_functionExprParameter> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _functionExprParameter_1 in __GeneratedOdataV2.Parsers.Rules._functionExprParameterParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_functionExprParameter(_COMMA_1, _functionExprParameter_1);
    }
    
}
