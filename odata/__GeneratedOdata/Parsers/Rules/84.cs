namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _searchTermParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._searchTerm> Instance { get; } = from _ʺx4Ex4Fx54ʺ_RWS_1 in __GeneratedOdata.Parsers.Inners._ʺx4Ex4Fx54ʺ_RWSParser.Instance.Optional()
from _ⲤsearchPhraseⳆsearchWordↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤsearchPhraseⳆsearchWordↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._searchTerm(_ʺx4Ex4Fx54ʺ_RWS_1.GetOrElse(null), _ⲤsearchPhraseⳆsearchWordↃ_1);
    }
    
}
