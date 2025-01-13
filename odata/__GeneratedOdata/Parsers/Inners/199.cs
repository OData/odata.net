namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _searchPhraseⳆsearchWordParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._searchPhraseⳆsearchWord> Instance { get; } = (_searchPhraseParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._searchPhraseⳆsearchWord>(_searchWordParser.Instance);
        
        public static class _searchPhraseParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._searchPhraseⳆsearchWord._searchPhrase> Instance { get; } = from _searchPhrase_1 in __GeneratedOdata.Parsers.Rules._searchPhraseParser.Instance
select new __GeneratedOdata.CstNodes.Inners._searchPhraseⳆsearchWord._searchPhrase(_searchPhrase_1);
        }
        
        public static class _searchWordParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._searchPhraseⳆsearchWord._searchWord> Instance { get; } = from _searchWord_1 in __GeneratedOdata.Parsers.Rules._searchWordParser.Instance
select new __GeneratedOdata.CstNodes.Inners._searchPhraseⳆsearchWord._searchWord(_searchWord_1);
        }
    }
    
}
