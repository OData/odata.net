namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _maxpagesizePreferenceParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._maxpagesizePreference> Instance { get; } = from _ʺx6Fx64x61x74x61x2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx6Fx64x61x74x61x2EʺParser.Instance.Optional()
from _ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺParser.Instance
from _EQⲻh_1 in __GeneratedOdataV2.Parsers.Rules._EQⲻhParser.Instance
from _oneToNine_1 in __GeneratedOdataV2.Parsers.Rules._oneToNineParser.Instance
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._maxpagesizePreference(_ʺx6Fx64x61x74x61x2Eʺ_1.GetOrElse(null), _ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ_1, _EQⲻh_1, _oneToNine_1, _DIGIT_1);
    }
    
}
