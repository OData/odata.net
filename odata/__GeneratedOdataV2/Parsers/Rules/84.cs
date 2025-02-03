namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchTermParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._searchTerm> Instance { get; } = from _ʺx4Ex4Fx54ʺ_RWS_1 in __GeneratedOdataV2.Parsers.Inners._ʺx4Ex4Fx54ʺ_RWSParser.Instance.Optional()
from _ⲤsearchPhraseⳆsearchWordↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤsearchPhraseⳆsearchWordↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._searchTerm(_ʺx4Ex4Fx54ʺ_RWS_1.GetOrElse(null), _ⲤsearchPhraseⳆsearchWordↃ_1);
    }
    
}
