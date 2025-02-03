namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchPhraseParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._searchPhrase> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _qcharⲻnoⲻAMPⲻDQUOTE_1 in __GeneratedOdataV2.Parsers.Rules._qcharⲻnoⲻAMPⲻDQUOTEParser.Instance.Repeat(1, null)
from _quotationⲻmark_2 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._searchPhrase(_quotationⲻmark_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE>(_qcharⲻnoⲻAMPⲻDQUOTE_1), _quotationⲻmark_2);
    }
    
}
