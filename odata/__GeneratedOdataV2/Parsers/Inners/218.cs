namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_parameterNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_parameterName> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _parameterName_1 in __GeneratedOdataV2.Parsers.Rules._parameterNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_parameterName(_COMMA_1, _parameterName_1);
    }
    
}
