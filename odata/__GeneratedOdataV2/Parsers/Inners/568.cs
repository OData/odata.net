namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_preferenceParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_preference> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _preference_1 in __GeneratedOdataV2.Parsers.Rules._preferenceParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_preference(_COMMA_1, _preference_1);
    }
    
}
