namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _searchPhraseParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._searchPhrase> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdata.Parsers.Rules._quotationⲻmarkParser.Instance
from _qcharⲻnoⲻAMPⲻDQUOTE_1 in __GeneratedOdata.Parsers.Rules._qcharⲻnoⲻAMPⲻDQUOTEParser.Instance.Many()
from _quotationⲻmark_2 in __GeneratedOdata.Parsers.Rules._quotationⲻmarkParser.Instance
select new __GeneratedOdata.CstNodes.Rules._searchPhrase(_quotationⲻmark_1, _qcharⲻnoⲻAMPⲻDQUOTE_1, _quotationⲻmark_2);
    }
    
}
