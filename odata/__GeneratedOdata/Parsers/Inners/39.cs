namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_keyValuePairParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._COMMA_keyValuePair> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _keyValuePair_1 in __GeneratedOdata.Parsers.Rules._keyValuePairParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_keyValuePair(_COMMA_1, _keyValuePair_1);
    }
    
}
