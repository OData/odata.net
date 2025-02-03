namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _searchPhraseⳆsearchWordParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._searchPhraseⳆsearchWord> Instance { get; } = (_searchPhraseParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._searchPhraseⳆsearchWord>(_searchWordParser.Instance);
        
        public static class _searchPhraseParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._searchPhraseⳆsearchWord._searchPhrase> Instance { get; } = from _searchPhrase_1 in __GeneratedOdataV3.Parsers.Rules._searchPhraseParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._searchPhraseⳆsearchWord._searchPhrase(_searchPhrase_1);
        }
        
        public static class _searchWordParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._searchPhraseⳆsearchWord._searchWord> Instance { get; } = from _searchWord_1 in __GeneratedOdataV3.Parsers.Rules._searchWordParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._searchPhraseⳆsearchWord._searchWord(_searchWord_1);
        }
    }
    
}
