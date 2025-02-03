namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _waitPreferenceParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._waitPreference> Instance { get; } = from _ʺx77x61x69x74ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx77x61x69x74ʺParser.Instance
from _EQⲻh_1 in __GeneratedOdataV2.Parsers.Rules._EQⲻhParser.Instance
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Rules._waitPreference(_ʺx77x61x69x74ʺ_1, _EQⲻh_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
