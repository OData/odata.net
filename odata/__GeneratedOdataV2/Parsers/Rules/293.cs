namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _durationParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._duration> Instance { get; } = from _ʺx64x75x72x61x74x69x6Fx6Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx64x75x72x61x74x69x6Fx6EʺParser.Instance.Optional()
from _SQUOTE_1 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
from _durationValue_1 in __GeneratedOdataV2.Parsers.Rules._durationValueParser.Instance
from _SQUOTE_2 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._duration(_ʺx64x75x72x61x74x69x6Fx6Eʺ_1.GetOrElse(null), _SQUOTE_1, _durationValue_1, _SQUOTE_2);
    }
    
}
